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
using System.Diagnostics;

namespace ArmaSpotifyController
{
    public class Image
    {
        // Image directory
        internal static string image_directory;

        internal static void Setup()
        {
            // Set file directory
            image_directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\data";
            Directory.CreateDirectory(image_directory);

            // Log the image directory
            Log.Message("Image_directory: " + image_directory);

            // Delete files in the image directory that are longer then a week old
            string[] files = Directory.GetFiles(image_directory);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                if (info.LastAccessTime < DateTime.Now.AddDays(-7))
                {
                    info.Delete();
                }
            }
        }

        internal static async Task DownloadImage(string uri_string, string variable = "")
        {
            try
            {
                // Create new URI from string input
                Uri uri = new Uri(uri_string);

                // Get filename from Spotify web server                
                string filename = Path.GetFileNameWithoutExtension(uri.LocalPath);

                // Create file path along with new extension
                var path = Path.Combine(image_directory, filename + ".jpg");

                if (!File.Exists(path.ToString()))
                {
                    // Download the image and write to the file
                    var imageBytes = await Master.client.GetByteArrayAsync(uri);
                    File.WriteAllBytes(path, imageBytes);

                    Log.Message("Downloaded file: " + path);
                }
                else
                {
                    // File already exists
                    Log.Message("File already exists on disk: " + filename + ".jpg");
                }

                if (variable != "")
                {
                    Master.callback.Invoke("ArmaSpotifyController", "ctrlSetText", path + "|" + variable);
                }
            }
            catch (Exception e)
            {
                if (variable != "")
                {
                    Master.callback.Invoke("ArmaSpotifyController", "ctrlSetText", "#(rgb,8,8,3)color(0.3,0.3,0.3,1)" + "|" + variable);
                }

                Log.Message("Attempted to load image from URL but ran into error: " + e.Message);
                Log.Message("Uri: " + uri_string);
            }            
        }
    }

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
            log_file = log_directory + @"\ArmaSpotifyController_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";
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
                Message("Reading user info from file");

                // Read refresh token to get access token without user having to authenticate again
                string token = File.ReadAllText(token_file).Trim();

                // Save refresh token for refresh function
                Master.client_refresh_token = token;

                // Get new token through task in the background
                Task ignore = Refresh.RefeshData(true);
            }
        }

        internal static bool Message(string message)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(log_file))
                {
                    sw.WriteLine(DateTime.Now.ToString("[dd/MM/yyyy hh:mm:ss tt] ") + message);
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
                File.WriteAllText(token_file, Master.client_refresh_token);
                return true;
            }
            catch (Exception e)
            {
                Message("Attempted to save token but ran into error: " + e.Message);
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
                Message("Attempted to delete token but ran into error: " + e.Message);
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

        public async static Task RefeshData(bool save_info = false)
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

                    int pFrom_access = data.IndexOf("access_token") + 15;
                    int pTo_access = data.IndexOf("token_type") - 3;

                    int pFrom_refresh = data.IndexOf("refresh_token") + 16;
                    int pTo_refresh = data.LastIndexOf("scope") - 3;

                    String result_access = data.Substring(pFrom_access, pTo_access - pFrom_access);
                    String result_refresh = data.Substring(pFrom_refresh, pTo_refresh - pFrom_refresh);

                    Master.client_access_token = result_access;
                    Master.client_refresh_token = result_refresh;

                    // Delete old token file
                    Log.DeleteToken();

                    // Save new refresh token
                    Log.SaveToken();

                    Log.Message("Users refresh token has been updated successfully");

                    if (save_info)
                    {
                        Log.Message("Starting user info task request");

                        Task ignore_2 = Request.GetUserInfo();
                    }

                    // Success - reset timer incase it was changed
                    refresh_timer.Interval = refresh_interval;
                }
                else
                {
                    // Non-rate limited errors break the loop
                    if (response.StatusCode != (HttpStatusCode)429)
                    {
                        Log.Message("Error: " + response.StatusCode.ToString());
                        Log.Message(response.ReasonPhrase);
                        // An error occured, try again in a minute
                        refresh_timer.Interval = 60 * 1_000;
                    }
                    else
                    {
                        Log.Message("Attempted to gather refresh token, ran into rate limit. Attempting again in 5 seconds");
                        // Wait for rate limit
                        await Task.Delay(5_000);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        public static void RefreshToken(Object source, ElapsedEventArgs e)
        {
            Task ignore = RefeshData();
        }

    }

    public class Player
    {
        // Internal refresh token timer
        private static Timer player_timer;
        private static int player_interval = 5_000;

        internal static void PlayerLoop()
        {
            // Tokens last 3600 seconds, aka. 1 hour.
            // Attempt to refresh every 50 minutes to allow 10 minutes for errors
            player_timer = new Timer(player_interval);
            player_timer.Elapsed += PlayerToken;
            player_timer.AutoReset = true;
            player_timer.Enabled = true;
        }

        internal async static Task PlayerData()
        {
            try
            {
                // Get Information About The User's Current Playback
                var response = await Master.client.GetAsync("https://api.spotify.com/v1/me/player");

                if (response.IsSuccessStatusCode)
                {
                    // Save response to variable for later use
                    String data = await response.Content.ReadAsStringAsync();

                    if (data.Length <= 0)
                    {
                        // No devices found
                        Log.Message("No devices found for playback");
                    }
                    else
                    {
                        Log.Message(data);
                    }
                }
                else
                {
                    Log.Message("Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal static void PlayerToken(Object source, ElapsedEventArgs e)
        {
            Task ignore = PlayerData();
        }

    }

    public class Request
    {
        internal async static Task GetUserInfo()
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
                    Log.Message("Premium: " + Master.client_premium.ToString().ToLower());
                    Log.Message("Show NSFW: " + Master.client_nsfw.ToString().ToLower());
                    Log.Message("Country: " + Master.client_country);

                    // Callback to game to let it know user info has been saved
                    Master.callback.Invoke("ArmaSpotifyController","setVariable","[\"missionnamespace\", \"aasp_info_saved\", true, false]");
                }
                else
                {
                    Log.Message("'GetUserInfo' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);

                    Master.get_async_response = "";
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal async static Task GetUserDevices(string variable_name)
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);
                var response = await Master.client.GetAsync("https://api.spotify.com/v1/me/player/devices");

                if (response.IsSuccessStatusCode)
                {
                    // Save response to variable for later use
                    string data = await response.Content.ReadAsStringAsync();

                    // Find list of devices index
                    int array_start = data.IndexOf("[") + 1;
                    int array_end = data.LastIndexOf("]");

                    // Get string ready for reading
                    string result = data.Substring(array_start, array_end - array_start).Replace('{', '\0').Replace('}', '|').Replace("\n", "").Replace("\r", "");

                    List<string> devices_list = new List<string>(); 

                    string[] device_list = result.Trim().Split('|');
                    foreach (string items in device_list)
                    {
                        if (items.Length <= 0)
                            continue;

                        if (items[0] == ',')
                            items.Remove(0, 1);

                        List<string> output_list = new List<string>();
                        foreach (string item_string in items.Trim().Split(','))
                        {
                            string[] attributes = item_string.Trim().Split(':');
                            if (attributes.Length == 2)
                            {
                                // Append to output list
                                output_list.Add(attributes[1].Trim().Replace("\"", "'"));
                            }
                        }

                        // Append to device list
                        devices_list.Add("[" + string.Join(",", output_list) + "]");
                    }

                    // Callback to game to add items to listbox
                    string callback_data = "['display',['"+ variable_name + "', [" + string.Join(",", devices_list) + "]]]";
                    Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_get_devices", callback_data.Replace("\"\"","'"));
                    Log.Message(callback_data.Replace("\"\"", "'"));
                }
                else
                {
                    Log.Message("'GetUserDevices' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal async static Task SetUserDevice(string device_id)
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var content = new StringContent("{\"device_ids\" : [\"" + device_id + "\"], \"play\" : true}");

                var response = await Master.client.PutAsync("https://api.spotify.com/v1/me/player", content);

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Transfered audio playback to " + device_id);
                }
                else
                {
                    Log.Message("'SetUserDevice' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal async static Task SetUserVolume(int volume_level)
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.PutAsync("https://api.spotify.com/v1/me/player/volume?volume_percent=" + volume_level, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Set users volume to " + volume_level);
                }
                else
                {
                    Log.Message("'SetUserVolume' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal async static Task PausePlayback()
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.PutAsync("https://api.spotify.com/v1/me/player/pause", new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Paused audio playback");
                }
                else
                {
                    Log.Message("'PausePlayback' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal async static Task ResumePlayback()
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.PutAsync("https://api.spotify.com/v1/me/player/play", new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Resumed audio playback");
                }
                else
                {
                    Log.Message("'PausePlayback' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal async static Task SkipNext()
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.PostAsync("https://api.spotify.com/v1/me/player/next", new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Skipped to next track in playlist");
                }
                else
                {
                    Log.Message("'PausePlayback' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal async static Task SkipBack()
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.PostAsync("https://api.spotify.com/v1/me/player/previous", new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Skipped to the prevoius track in playlist");
                }
                else
                {
                    Log.Message("'PausePlayback' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
            }
        }

        internal async static Task LoadDisplay()
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.GetAsync("https://api.spotify.com/v1/me/player");

                if (response.IsSuccessStatusCode)
                {
                    String data = (await response.Content.ReadAsStringAsync())
                        .Replace('\n', '\0').Replace('\r', '\0');

                    Log.Message(data);
                    if (data.Length > 0)
                    {
                        // Playing + NSFW
                        bool playing = data.Contains("\"is_playing\" : true");
                        bool nsfw = data.Contains("\"explicit\" : true");

                        // Song duration
                        int pFrom_duration = data.IndexOf("duration_ms") + 15;
                        int pTo_duration = data.IndexOf("explicit") - 3;
                        int length = int.Parse(data.Substring(pFrom_duration, pTo_duration - pFrom_duration).Replace('\u0020', '\0').Replace(',', '\0'));

                        // Song progress
                        int pFrom_progress = data.IndexOf("progress_ms") + 15;
                        int pTo_progress = data.IndexOf("item") - 3;
                        int progress = int.Parse(data.Substring(pFrom_progress, pTo_progress - pFrom_progress).Replace('\u0020', '\0').Replace(',', '\0'));

                        // Song name
                        int pFrom_name = data.LastIndexOf("\"name\"") + 10;
                        int pTo_name = data.LastIndexOf("\"popularity\"") - 7;
                        string name_song = data.Substring(pFrom_name, pTo_name - pFrom_name);

                        // Artists names
                        int pFrom_artist = data.LastIndexOf("\"artists\" : [ ") + 14;
                        int pTo_artist = data.LastIndexOf("} ],") + 1;
                        
                        string artists_string = data.Substring(pFrom_artist, pTo_artist - pFrom_artist);
                        while (artists_string.Contains("external_urls"))
                        {
                            int start = artists_string.IndexOf("\"external_urls\"");
                            int end = artists_string.IndexOf("\"href\"", start);
                            artists_string = artists_string.Remove(start, end - start);
                        }

                        List<string> artists_list = new List<string>();
                        string[] artists = artists_string.Split(',');
                        foreach (string i in artists)
                        {
                            string[] attributes = i.Split(':');
                            if (attributes[0].Contains('{'))
                                attributes[0] = attributes[0].Remove(attributes[0].IndexOf("{"), 1);

                            if (attributes[0].Contains("name"))
                            {
                                artists_list.Add(attributes[1].Trim().Replace("\"", ""));
                                Log.Message("********");
                                Log.Message(">" + attributes[1].Trim().Replace("\"", "") + "<");
                                Log.Message("^^^^^^^");
                            }
                        }
                        string name_artist = string.Join(", ", artists_list);

                        // Image URI
                        int pFrom_image = data.LastIndexOf("height\" : 64") + 31;
                        int pTo_image = data.LastIndexOf("width\" : 64") - 12;
                        string image = data.Substring(pFrom_image, pTo_image - pFrom_image).Replace('\u0020', '\0').Replace(',', '\0').Remove(0, 18);

                        Log.Message(string.Format("[{0},{1},{2},{3},\"{4}\",\"{5}\",\"{6}\"]", playing, nsfw, length, progress, name_song, name_artist, image));
                        Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_display", string.Format("[{0},{1},{2},{3},\"{4}\",\"{5}\",\"{6}\"]", playing, nsfw, length, progress, name_song, name_artist, image));
                    }
                    else
                    {
                        Log.Message("'LoadDisplay' No devices found currently playing");
                    }
                }
                else
                {
                    Log.Message("'LoadDisplay' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message(e.Message);
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

            // Setup log file and stuff for error logging
            Log.Setup();

            // Log setup called
            Log.Message("DLL setup running");

            // Setup image saving
            Image.Setup();

            // Save image directory to variable - includes backslash so be aware of that!
            callback.Invoke("ArmaSpotifyController", "setVariable", "[\"missionnamespace\", \"aasp_image_location\", \""+ Image.image_directory + @"\" +"\", false]");

            // Generate a new state key
            RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();

            // Buffer storage.
            byte[] data = new byte[4];

            // Fill buffer
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
                        // GET_DEVICES: Request list of users devices
                        case "get_devices":
                            Task ignore = Request.GetUserDevices(parameters[2]);
                            output.Append("true");
                            break;

                        // SET_DEVICE: Transfer audio playback to requested device
                        case "set_device":
                            Task ignore_1 = Request.SetUserDevice(parameters[2]);
                            output.Append("true");
                            break;

                        // SET_VOLUME: Sets users volume for the current active device
                        case "set_volume":
                            Task ignore_2 = Request.SetUserVolume(int.Parse(parameters[2]));
                            output.Append("true");
                            break;

                        // SKIP: Skips forward/backwards to the next item in playlist
                        case "skip":
                            if (parameters[2] == "next")
                            {
                                Task ignore_3 = Request.SkipNext();
                            }
                            else
                            {
                                Task ignore_4 = Request.SkipBack();
                            }                            
                            output.Append("true");
                            break;

                        // PLAY: Starts/resumes users playback of the current song/defined song
                        case "play":
                            Task ignore_5 = Request.ResumePlayback();
                            output.Append("true");
                            break;
                        
                        // LOAD_DISPLAY: Gathers data on current playback and then sends data back to game to setup display
                        case "load_display":
                            Task ignore_6 = Request.LoadDisplay();
                            output.Append("true");
                            break;

                        // PAUSE: Pauses users playback of the current song
                        case "pause":
                            Task ignore_7 = Request.PausePlayback();
                            output.Append("true");
                            break;

                        // CONNECT_WEBSITE: Open the Spotify Connect website
                        case "connect_website":
                            Process.Start("https://www.spotify.com/connect/");
                            output.Append("true");
                            break;

                        // DOWNLOAD_IMAGE: Downloads image from spotify server and then sets it to a control
                        case "download_image":
                            Task ignore_8 = Image.DownloadImage(@"https://i.scdn.co/" + parameters[2], parameters[3]);
                            output.Append("true");
                            break;

                        // PREMIUM: Returns if the user who has authorised has an active Spotify Premium subscription
                        case "premium":
                            output.Append(client_premium.ToString().ToLower());
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

                            client.DefaultRequestHeaders.Clear();

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

                                    // Delete old token file
                                    Log.DeleteToken();

                                    // Save new refresh token
                                    Log.SaveToken();

                                    // Start the refresh timer
                                    Refresh.RefreshTokenLoop();

                                    // Start gathering user info for display
                                    Player.PlayerLoop();

                                    // Callback to game to let it know user is authorised
                                    callback.Invoke("ArmaSpotifyController", "setVariable", "[\"missionnamespace\", \"aasp_authorised\", true, false]");

                                    // Save user info to client variables in DLL for later use
                                    Task ignore = Request.GetUserInfo();

                                    Log.Message("User authentication success");
                                    output.Append("User authentication success");
                                    break;
                                }
                                else
                                {
                                    Log.Message("Error: " + response.StatusCode.ToString());
                                    Log.Message(response.ReasonPhrase);

                                    // Check if we are being rate limited
                                    if (response.StatusCode == (HttpStatusCode)429)
                                    {
                                        output.Append("ERROR: Request was rate limited, try again in a few seconds.");
                                        break;
                                    }
                                    else
                                    {
                                        output.Append("ERROR: Post request returned error");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // Missing state key
                                Log.Message("ERROR: Incorrect 'state' key. Reauthorization required!");
                                output.Append("ERROR: Incorrect 'state' key. Reauthorization required!");
                                break;
                            }
                        }
                        else
                        {
                            // Incorrect input
                            Log.Message("ERROR: Missing 'key' and 'state' parameter for authorisiation.");
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
                        Process.Start(string.Format("https://accounts.spotify.com/authorize?{0}", string.Join("&", parameters_dict.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))));
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
                        Process.Start("https://www.spotify.com/premium/");
                        break;

                    // ERROR: Log error to log file for debuging help later
                    case "error":
                        Log.Message(parameters[1]);
                        output.Append("true");
                        break;

                    // LOG: Show where logs are being saved to
                    case "log":                        
                        output.Append(Log.log_directory);
                        break;
                        
                    // DATA: Show where images are being saved to
                    case "data":
                        output.Append(Image.image_directory);
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