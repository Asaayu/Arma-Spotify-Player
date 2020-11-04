using RGiesecke.DllExport;
using System;
using System.Management;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Timers;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ArmaSpotifyController
{
    public class Security
    {
        internal static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        internal static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

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

        public static void RefreshTokenLoop()
        {
            // Tokens last 3600 seconds, aka. 1 hour.
            // Attempt to refresh every 50 minutes to allow 10 minutes for errors
            refresh_timer = new Timer(3000 * 1000);
            refresh_timer.Elapsed += RefreshToken;
            refresh_timer.AutoReset = true;
            refresh_timer.Enabled = true;
        }

        public async static Task RefeshData()
        {
            // Setup POST data
            var values = new Dictionary<string, string>
            {

                { "grant_type", "refresh_token" },
                { "refresh_token", Master.client_refresh_token },
                { "client_id", Master.app_client_id }
            };
            var content = new FormUrlEncodedContent(values);

            String data = null;
            for (int i = 0; i < 5; i++)
            {
                var response = await Master.client.PostAsync("https://accounts.spotify.com/api/token", content);

                // Give time for data to load if
                await Task.Delay(5_000);

                if (response.IsSuccessStatusCode)
                {
                    // Save response to variable for later use
                    data = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Non-rate limited errors break the loop
                    if (response.StatusCode != (HttpStatusCode)429)
                    {
                        break;
                    }
                    else
                    {
                        // Wait for rate limit
                        await Task.Delay(5_000);
                    }
                }
            }

            if (data != null)
            {
                int pFrom_access = data.IndexOf("\"access_token\": \"") + "\"access_token\": \"".Length;
                int pTo_access = data.LastIndexOf("\",\"token_type\"");

                int pFrom_refresh = data.IndexOf("\"refresh_token\": \"") + "\"refresh_token\": \"".Length;
                int pTo_refresh = data.LastIndexOf("\",\"scope\"");

                String result_access = data.Substring(pFrom_access + 1, (pTo_access - 1) - pFrom_access);
                String result_refresh = data.Substring(pFrom_refresh + 1, (pTo_refresh - 1) - pFrom_refresh);

                Master.client_access_token = result_access;
                Master.client_refresh_token = result_refresh;

                // Success - reset timer incase it was changed
                refresh_timer.Interval = 3000 * 1000;
            }
            else
            {
                // An error occured, try again in a minute
                refresh_timer.Interval = 60 * 1000;
            }
        }

        public static void RefreshToken(Object source, ElapsedEventArgs e)
        {
            Task ignore = RefeshData();
        }

    }

    public class Data
    {
        public static bool InternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public async static void RequestData_Authorise(string post_url, FormUrlEncodedContent content)
        {
            // Send request + content in POST 
            var response = await Master.client.PostAsync(post_url, content);

            if (response.IsSuccessStatusCode)
            {
                // Save response to variable for later use
                Master.authorise_response = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Check if we are being rate limited
                if (response.StatusCode == (HttpStatusCode)429)
                {
                    Master.authorise_response = "RATE_LIMIT";
                }
                else
                {
                    Master.authorise_response = "";
                }
            }
        }

        public async static void Get_Client_Info()
        {
            // Set authorization headers
            Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

            // Send request + content in GET 
            var response = await Master.client.GetAsync("https://api.spotify.com/v1/me");

            if (response.IsSuccessStatusCode)
            {
                // Save response to variable for later use
                Master.get_response = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Check if we are being rate limited
                if (response.StatusCode == (HttpStatusCode)429)
                {
                    Master.get_response = "RATE_LIMIT";
                }
                else
                {
                    Master.get_response = "";
                }
            }
            Set_Client_Info();
        }

        public static void Set_Client_Info()
        {
            if (Master.get_response != "")
            {
                if (Master.get_response != "RATE_LIMIT")
                {
                    // Save response to variable for later use
                    var data = Master.get_response;

                    String result_product = data.Substring(10, 5);
                    //String result_country = match_country.Value.Substring(12);
                    //String result_name = data.Substring(pFrom_name + 1, (pTo_name - 1) - pFrom_name);
                    //String result_explicit = match_explicit.Value.Substring(17);

                    //Master.client_premium = true;//result_product == "premium";
                    //Master.client_country = data.Substring(18, 2);
                    //Master.client_name = Master.get_response;
                    //Master.client_show_explicit = result_explicit == "false";

                    // This is only used to check if we have alreay got the users info
                    Master.client_ready = "true";
                }
            }
        }

        public async static void GetRequest(string get_url, String header_title, String header_body)
        {
            // Set authorization headers
            Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(header_title, header_body);

            // Send request + content in GET 
            var response = await Master.client.GetAsync(get_url);

            if (response.IsSuccessStatusCode)
            {
                // Save response to variable for later use
                Master.get_response = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Check if we are being rate limited
                if (response.StatusCode == (HttpStatusCode)429)
                {
                    Master.get_response = "RATE_LIMIT";
                }
                else
                {
                    Master.get_response = "";
                }
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

        // Client variables
        internal static string client_ready;
        internal static bool client_premium;
        internal static string client_country;
        internal static string client_name;
        internal static bool client_show_explicit;

        // HttpClient for posting data to Spotify
        internal static readonly HttpClient client = new HttpClient();

        // Variables containing important information
        internal static string verifier_string;
        internal static string id;
        internal static int state;

        // Async responses
        internal static string authorise_response;
        internal static string get_response;

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

            // Generate a new state key
            RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();

            // Buffer storage.
            byte[] data = new byte[4];

            // Fill buffer.
            generator.GetBytes(data);

            // Convert to int 32
            state = BitConverter.ToInt32(data, 0);

            // Get disk drives
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            // Create unique ID for computer/user
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                // Gather information
                id += wmi_HD["Model"].ToString();
                id += wmi_HD["InterfaceType"].ToString();
                id += wmi_HD["Caption"].ToString();
                id += wmi_HD.GetPropertyValue("SerialNumber").ToString();
            }

            // Create base64 string ID
            id = Convert.ToBase64String(Encoding.UTF8.GetBytes(id));
            
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

            if(!Data.InternetConnection())
            {
                output.Append("ERROR: Internet connection is required for this mod");
                return;
            }

            // Split on the spacers, in this case ":"
            String[] parameters = function.Split(':');

            // Check if the function starts with the word "Spotify"
            if (function.Length >= 7 && function.ToLower().Substring(0,7) == "spotify")
            {
                if (parameters.Length > 1)
                {
                    switch (parameters[1].ToLower())
                    {                        
                        // GET_USER_INFO: Check user info
                        case "get_user_info":
                            output.Append(client_name);
                            break;

                        // SET_USER_INFO: Check user info
                        case "set_user_info":
                            Data.Get_Client_Info();
                            output.Append("AAA");
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
                    // AUTHORISE: Get async response and save
                    case "authorise":
                        String data = authorise_response;
                        if (data != null)
                        {
                            if (data != "")
                            {
                                if (data != "RATE_LIMIT")
                                {
                                    int pFrom_access = data.IndexOf("\"access_token\": \"") + "\"access_token\": \"".Length;
                                    int pTo_access = data.LastIndexOf("\",\"token_type\"");

                                    int pFrom_refresh = data.IndexOf("\"refresh_token\": \"") + "\"refresh_token\": \"".Length;
                                    int pTo_refresh = data.LastIndexOf("\",\"scope\"");

                                    String result_access = data.Substring(pFrom_access + 1, (pTo_access - 1) - pFrom_access);
                                    String result_refresh = data.Substring(pFrom_refresh + 1, (pTo_refresh - 1) - pFrom_refresh);

                                    client_access_token = result_access;
                                    client_refresh_token = result_refresh;

                                    // Start the refresh timer
                                    Refresh.RefreshTokenLoop();

                                    output.Append("Success");
                                }
                                else
                                {
                                    output.Append("ERROR: Rate limit reached. Try again in a couple of seconds.");
                                }                                
                            }
                            else
                            {
                                output.Append("ERROR: Request did not return any data. Reauthorization required!");
                            }
                        }
                        else
                        {
                            output.Append("ERROR: Request for data timed out. Reauthorization required!");
                        }
                        break;                        

                    // AUTHORISE_SUBMIT: Receive authorise information from the user to get token
                    case "authorise_submit":
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

                                // Send async post request to spotify
                                Data.RequestData_Authorise(post_url, content);

                                output.Append("Async request sent");
                            }
                            else
                            {
                                // Missing state key
                                output.Append("ERROR: Incorrect 'state' key. Reauthorization required!");
                            }
                        }
                        else
                        {
                            // Incorrect input
                            output.Append("ERROR: Missing key and state parameter for authorisiation.");
                        };
                        break;

                    // AUTHORISE_REQUEST: Open url to allow the user to authorise this application
                    case "authorise_request":
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

                    // ACCESS: All the access stuff
                    case "access":
                        if (parameters.Length >= 2)
                        {
                            switch (parameters[1].ToLower())
                            {
                                // Setting the variable
                                case "set":
                                    if (parameters.Length >= 3)
                                    {
                                        client_access_token = parameters[2];
                                        output.Append("Access token saved");
                                    }
                                    else
                                    {
                                        output.Append("ERROR: No access token given");
                                    }
                                    break;

                                // Getting the variable
                                case "get":
                                    if (client_access_token != null)
                                    {
                                        output.Append(Security.EncryptString(id,client_access_token));
                                    }
                                    else
                                    {
                                        output.Append("ERROR: No access token found");
                                    }                                    
                                    break;

                                // Checking the variable
                                case "check":
                                    if (parameters.Length >= 3)
                                    {
                                        String input = parameters[2];
                                        bool check_bool = Security.DecryptString(id, input) == client_access_token;
                                        output.Append(check_bool.ToString().ToLower());
                                    }
                                    else
                                    {
                                        output.Append("ERROR: No access token given");
                                    }
                                    break;

                                default:
                                    output.Append("ERROR: Missing second parameter");
                                    break;
                            }                            
                        }
                        else
                        {
                            output.Append("ERROR: Not enough parameters passed to function call");
                        }
                        break;

                    // REFRESH: All the refresh stuff
                    case "refresh":
                        if (parameters.Length >= 2)
                        {
                            switch (parameters[1].ToLower())
                            {
                                // Setting the variable
                                case "set":
                                    if (parameters.Length >= 3)
                                    {
                                        client_refresh_token = parameters[2];
                                        output.Append("Refresh token saved");
                                    }
                                    else
                                    {
                                        output.Append("ERROR: No refresh token given");
                                    }
                                    break;

                                // Getting the variable
                                case "get":
                                    if (client_refresh_token != null)
                                    {
                                        output.Append(Security.EncryptString(id, client_refresh_token));
                                    }
                                    else
                                    {
                                        output.Append("ERROR: No refresh token found");
                                    }
                                    break;

                                // Checking the variable
                                case "check":
                                    if (parameters.Length >= 3)
                                    {
                                        String input = parameters[2];
                                        bool check_bool = Security.DecryptString(id, input) == client_refresh_token;
                                        output.Append(check_bool.ToString().ToLower());
                                    }
                                    else
                                    {
                                        output.Append("ERROR: No refresh token given");
                                    }
                                    break;

                                default:
                                    output.Append("ERROR: Missing second parameter");
                                    break;
                            }
                        }
                        else
                        {
                            output.Append("ERROR: Not enough parameters passed to function call");
                        }
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

                    // DEFAULT: Show version information
                    default:
                        output.Append(version_info);
                        break;
                }
            }
        }
    }
}