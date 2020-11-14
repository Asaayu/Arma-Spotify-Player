using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace ArmaSpotifyController
{
    public class Log
    {
        // Log directory and current log file path
        internal static String log_directory;
        internal static String log_file;

        // Toekn directory and current token file path
        internal static String token_directory;
        internal static String token_file;
        
        internal static void Setup()
        {
            // Set file directory
            log_directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\logs";
            log_file = log_directory + @"\SpotifyController_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";
            Directory.CreateDirectory(log_directory);

            // Delete files in the log directory that are longer then a week old - to avoid clogging folder with files
            string[] files = Directory.GetFiles(log_directory);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                if (info.LastAccessTime < DateTime.Now.AddDays(-7))
                {
                    info.Delete();
                }   
            }

            // Check if token file exists
            token_directory = Path.GetTempPath();
            token_file = token_directory + @"\aasp.token";
            if (File.Exists(token_file))
            {
                Error("Reading user info from file");

                // Read refresh token to get access token without user having to authenticate again
                string[] info_list = File.ReadAllText(token_file).Trim().Split(':');

                // Save refresh token for refresh function
                Master.client_refresh_token = info_list[0];

                // Get new token through task in the background
                Task ignore_1 = Refresh.RefeshData();

                // Current unix time
                long current_time = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
                // Saved unix time
                long saved_time = long.Parse(info_list[4]);

                if (current_time - saved_time <= 604800)
                {
                    // Save data to client variables
                    Master.client_premium = info_list[1].Contains("True");
                    Master.client_nsfw = info_list[2].Contains("True");
                    Master.client_country = info_list[3];

                    // List information in log file
                    Error("Information read from file!");
                    Error("Premium: " + Master.client_premium.ToString());
                    Error("Show NSFW: " + Master.client_nsfw.ToString());
                    Error("Country: " + Master.client_country);
                }
                else
                {
                    Error("Information stored in file is over a week old!");
                    Error("User info will be requested again to update to latest information.");
                    
                    while (true)
                    {
                        // Client token must exist for the user info request
                        if (Master.client_access_token != null)
                        {
                            // Save current user info
                            Task ignore_2 = Request.GetUserInfo();
                            break;
                        }
                    }
                }
            }
        }

        internal static bool Error(string message)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(log_file))
                {
                    sw.WriteLine(DateTime.Now.ToString("[dd/MM/yyyy hh:mm:ss.fff tt] ") + message);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool SaveToken()
        {
            try
            {
                // Save required information to the token file
                File.WriteAllText(token_file, Master.client_refresh_token + ":" + Master.client_premium + ":" + Master.client_nsfw + ":" + Master.client_country + ":" + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
                return true;
            }
            catch (Exception e)
            {
                Error("Attempted to save token but ran into error: " + e.Message);
                return false;
            }
        }

        internal static bool DeleteToken()
        {
            try
            {
                File.Delete(token_file);
                return true;
            }
            catch (Exception e)
            {
                Error("Attempted to delete token but ran into error: " + e.Message);
                return false;
            }
        }
    }

    public class Security
    {
        internal static string RandomString(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = new RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }
    }

    public class Refresh
    {
        // Internal refresh token timer
        private static Timer refresh_timer;
        private static int refresh_interval = 3_000 * 1_000;

        public static void RefreshTokenLoop()
        {
            // Tokens last 3600 seconds, aka. 1 hour.
            // Attempt to refresh every 50 minutes to allow 10 minutes for errors
            refresh_timer = new Timer(refresh_interval);
            refresh_timer.Elapsed += RefreshToken;
            refresh_timer.AutoReset = true;
            refresh_timer.Enabled = true;
        }

        public async static Task RefeshData()
        {
            try
            {
                // Setup POST data
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", Master.client_refresh_token },
                    { "client_id", Master.app_client_id }
                };
                var content = new FormUrlEncodedContent(values);

                var response = await Master.client.PostAsync("https://accounts.spotify.com/api/token", content);

                if (response.IsSuccessStatusCode)
                {
                    // Save response to variable for later use
                    String data = await response.Content.ReadAsStringAsync();

                    int pFrom_access = data.IndexOf("\"access_token\": \"") + "\"access_token\": \"".Length;
                    int pTo_access = data.LastIndexOf("\",\"token_type\"");

                    int pFrom_refresh = data.IndexOf("\"refresh_token\": \"") + "\"refresh_token\": \"".Length;
                    int pTo_refresh = data.LastIndexOf("\",\"scope\"");

                    String result_access = data.Substring(pFrom_access + 1, (pTo_access - 1) - pFrom_access);
                    String result_refresh = data.Substring(pFrom_refresh + 1, (pTo_refresh - 1) - pFrom_refresh);

                    Master.client_access_token = result_access;
                    Master.client_refresh_token = result_refresh;

                    Log.Error("Users refresh token has been updated successfully");

                    // Success - reset timer incase it was changed
                    refresh_timer.Interval = refresh_interval;
                }
                else
                {
                    // Non-rate limited errors break the loop
                    if (response.StatusCode != (HttpStatusCode)429)
                    {
                        Log.Error(response.StatusCode.ToString() + " error occured. Attempting again in 1 minute");
                        // An error occured, try again in a minute
                        refresh_timer.Interval = 60 * 1_000;
                    }
                    else
                    {
                        Log.Error("Attempted to gather refresh token, ran into rate limit. Attempting again in 5 seconds");
                        // Wait for rate limit
                        await Task.Delay(5_000);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static void RefreshToken(Object source, ElapsedEventArgs e)
        {
            Task ignore = RefeshData();
        }

    }

    public class Request
    {
        public async static Task GetUserInfo()
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);
                var response = await Master.client.GetAsync("https://api.spotify.com/v1/me");

                if (response.IsSuccessStatusCode)
                {
                    // Save response to variable for later use
                    String data = await response.Content.ReadAsStringAsync();

                    Master.client_premium = data.Contains("\"product\" : \"premium\"");
                    Master.client_nsfw = data.Contains("\"filter_enabled\" : false");
                    Master.client_country = data.Substring(data.IndexOf("\"country\" : \"") + 13, 2);
                    Log.Error("Premium: " + Master.client_premium.ToString());
                    Log.Error("Show NSFW: " + Master.client_nsfw.ToString());
                    Log.Error("Country: " + Master.client_country);

                    // Save information + refresh token to token file
                    Log.SaveToken();

                    // Callback to game to let it know user info has been saved
                    Master.callback.Invoke("ArmaSpotifyController","setVariable","[\"missionnamespace\", \"aasp_info_saved\", true, false]");
                }
                else
                {
                    if (response.StatusCode != (HttpStatusCode)429)
                    {
                        Log.Error("ERROR: " + response.StatusCode.ToString());
                        Master.get_async_response = "ERROR: " + response.StatusCode.ToString();
                    }
                    else
                    {
                        Log.Error("ERROR: Rate limited");
                        Master.get_async_response = "ERROR: Rate limited";
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }

    public class Master
    {
        // Predefined version information + author name
        internal static readonly string version_info = "ASJ DLL - VER 0.0.1 - Asaayu";
        internal static readonly string app_client_id = "8cf42361877d48de877925a8c1dc747d";

        // Variables for access & refresh tokens
        internal static string client_refresh_token;
        internal static string client_access_token;

        // Client information
        internal static bool client_premium;
        internal static bool client_nsfw;
        internal static string client_country;

        // Async returns
        internal static string get_async_response;

        // HttpClient for posting data to Spotify
        internal static readonly HttpClient client = new HttpClient();

        // Variables containing important information
        internal static string verifier_string;
        internal static int state;

        // Function call back stuff
        public static ExtensionCallback callback;
        public delegate int ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);

        // Do not remove these six lines
#if WIN64
        [DllExport("RVExtensionRegisterCallback", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionRegisterCallback@4", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RVExtensionRegisterCallback([MarshalAs(UnmanagedType.FunctionPtr)] ExtensionCallback func)
        {
            callback = func;
        }

        // Do not remove these six lines
#if WIN64
        [DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionVersion@8", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtensionVersion(StringBuilder output, int outputSize)
        {
            // Reduce output by 1 to avoid accidental overflow
            outputSize--;

            // Log setup called
            Log.Error("DLL setup running");

            // Setup log file and stuff for error logging
            Log.Setup();

            // Generate a new state key
            RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();

            // Buffer storage.
            byte[] data = new byte[4];

            // Fill buffer.
            generator.GetBytes(data);

            // Convert to int 32
            state = BitConverter.ToInt32(data, 0);
            
            // Auto output the version information to the report file
            output.Append(version_info);
        }

        // Do not remove these six lines
#if WIN64
        [DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
#endif
        public async static void RvExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function)
        {
            // Reduce output by 1 to avoid accidental overflow
            outputSize--;

            // Split on the spacers, in this case ":"
            String[] parameters = function.Split(':');

            // Check if the function starts with the word "Spotify"
            if (function.Length >= 7 && function.ToLower().Substring(0,7) == "spotify")
            {
                if (parameters.Length > 1)
                {
                    switch (parameters[1].ToLower())
                    {
                        // PREMIUM: Returns if the user who has authorised has an active Spotify Premium subscription
                        case "premium":
                            output.Append(client_premium.ToString());
                            break;

                        // PLAY: Starts/resumes users playback of the current song/defined song
                        case "play":
                            output.Append("play");
                            break;

                        // PAUSE: Pauses users playback of the current song
                        case "pause":
                            output.Append("pause");
                            break;

                        // DEFAULT: Show version information
                        default:
                            output.Append(version_info);
                            break;
                    }
                }
                else
                {
                    output.Append(version_info);
                };
            }
            else
            {
                // Switch through all the other options
                switch (parameters[0].ToLower())
                {
                    // AUTHORISE: Receive authorise information from the user to get token
                    case "authorise":
                        if (parameters.Length >= 3)
                        {
                            String user_code = parameters[1];
                            String user_state = parameters[2];

                            // Make sure state is correct, else reject authorization
                            if (user_state == state.ToString())
                            {
                                // POST URL
                                String post_url = "https://accounts.spotify.com/api/token";
                                // Setup POST data
                                var values = new Dictionary<string, string>
                                {
                                    { "client_id", app_client_id },
                                    { "grant_type", "authorization_code" },
                                    { "redirect_uri", "http://asaayu.com/arma-3/spotify/auth.php" },
                                    { "code", user_code },
                                    { "code_verifier", verifier_string }
                                };
                                var content = new FormUrlEncodedContent(values);

                                // Send request + content in POST 
                                var response = await client.PostAsync(post_url, content);

                                if (response.IsSuccessStatusCode)
                                {
                                    // Save response to variable for later use
                                    String data = await response.Content.ReadAsStringAsync();

                                    int pFrom_access = data.IndexOf("access_token") + 15;
                                    int pTo_access = data.IndexOf("token_type") - 3;

                                    int pFrom_refresh = data.IndexOf("refresh_token") + 16;
                                    int pTo_refresh = data.LastIndexOf("scope") - 3;

                                    String result_access = data.Substring(pFrom_access, pTo_access - pFrom_access);
                                    String result_refresh = data.Substring(pFrom_refresh, pTo_refresh - pFrom_refresh);

                                    client_access_token = result_access;
                                    client_refresh_token = result_refresh;

                                    // Start the refresh timer
                                    Refresh.RefreshTokenLoop();

                                    // Callback to game to let it know user is authorised
                                    Master.callback.Invoke("ArmaSpotifyController", "setVariable", "[\"missionnamespace\", \"aasp_authorised\", true, false]");

                                    // Save user info to client variables in DLL for later use
                                    Task ignore = Request.GetUserInfo();

                                    Log.Error("User authentication success");
                                    output.Append("User authentication success");
                                    break;
                                }
                                else
                                {
                                    // Check if we are being rate limited
                                    if (response.StatusCode == (HttpStatusCode)429)
                                    {
                                        Log.Error("[" + function + "]" + "ERROR: Request was rate limited, try again in a few seconds.");
                                        output.Append("ERROR: Request was rate limited, try again in a few seconds.");
                                        break;
                                    }
                                    else
                                    {
                                        Log.Error("[" + function + "]" + "ERROR: Post request returned error status code (" + response.StatusCode.ToString() + ")");
                                        output.Append("ERROR: Post request returned error status code (" + response.StatusCode.ToString() + ")");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // Missing state key
                                Log.Error("ERROR: Incorrect 'state' key. Reauthorization required!");
                                output.Append("ERROR: Incorrect 'state' key. Reauthorization required!");
                                break;
                            }
                        }
                        else
                        {
                            // Incorrect input
                            Log.Error("ERROR: Missing 'key' and 'state' parameter for authorisiation.");
                            output.Append("ERROR: Missing 'key' and 'state' parameter for authorisiation.");
                            break;
                        };

                    // AUTHORISE_WEBSITE: Open url to allow the user to authorise this application
                    case "authorise_website":
                        // User wants to re-authenticate, delete the old token file
                        Log.DeleteToken();

                        // Create code verifier
                        verifier_string = Security.RandomString(110);

                        // Create code challange
                        byte[] sha_bytes = SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(verifier_string));

                        // Encode to base64 url safe
                        String challange_string = Convert.ToBase64String(sha_bytes).Split('=')[0].Replace('+', '-').Replace('/', '_');                        

                        // Scopes required by the application
                        String[] scopes =
                        {
                            "user-read-recently-played",
                            "user-read-currently-playing",
                            "playlist-read-collaborative",
                            "playlist-read-private",
                            "user-read-private",
                            "user-read-playback-state",
                            "user-read-playback-position",
                            "user-top-read",
                            "user-modify-playback-state"
                        };

                        // URL parameters
                        var parameters_dict = new Dictionary<string, string>
                        {
                            { "response_type", "code" },
                            { "client_id", app_client_id },
                            { "redirect_uri", HttpUtility.UrlEncode("http://asaayu.com/arma-3/spotify/auth.php") },
                            { "scope", String.Join("%20", scopes)},
                            { "state", state.ToString() },
                            { "code_challenge", challange_string },
                            { "code_challenge_method", "S256" }
                        };

                        // DO NOT ALLOW USER TO CUSTOMIZE THIS.
                        System.Diagnostics.Process.Start(string.Format("https://accounts.spotify.com/authorize?{0}", string.Join("&", parameters_dict.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))));
                        break;
                    
                    // AUTHORISED: Check if the user is already authorised
                    case "authorised":
                        bool check = client_access_token != null && client_refresh_token != null;
                        output.Append(check.ToString().ToLower());
                        break;

                    // LEGAL: View legal information about mod
                    case "legal":
                        output.Append("Go to https://github.com/Asaayu/Arma-Spotify-Player to view the GitHub repo and view important legal information.");
                        break;

                    // PREMIUM_WEBSITE: Open Spotify Premium webpage, redirects to local version
                    case "premium_website":
                        System.Diagnostics.Process.Start("https://www.spotify.com/premium/");
                        break;

                    // ERROR: Log error to log file for debuging help later
                    case "error":
                        Log.Error(parameters[1]);
                        output.Append("true");
                        break;

                    // LOG: Show where logs are being saved to
                    case "log":                        
                        output.Append(Log.log_directory);
                        break;
                        
                    // TOKEN: Show where tokens are being saved to
                    case "token":                        
                        output.Append(Log.token_directory);
                        break;

                    // DEFAULT: Show version information
                    default:
                        output.Append(version_info);
                        break;
                }
            }
        }
    }
}