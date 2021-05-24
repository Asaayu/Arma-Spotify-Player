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
using System.Web;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Management;
using System.Threading;

namespace ArmaSpotifyController
{
    public class Variable
    {
        // Image
        internal static string data_directory;

        // Logging
        internal static String log_directory;
        internal static String log_file;

        // Debug variable
        internal static bool debug;

        // Token Saving
        internal static String token_file;

        // Request
        internal static readonly int refresh_delay = 250;
        internal static string last_device_id;
        internal static string last_song_id;
        internal static bool is_playing;
        internal static bool last_connection;

        // Master
        internal static readonly string version_info = "AASP DLL - VER 0.0.1 - Asaayu";
        internal static readonly string app_client_id = "8cf42361877d48de877925a8c1dc747d";

        // Token Lifetime
        internal static string client_refresh_token;
        internal static string client_access_token;
        internal static string client_web_access_token;
        internal static DateTime client_refresh_time;

        // Legal
        internal static string legal_update_file = "https://raw.githubusercontent.com/Asaayu/Arma-Spotify-Player/main/legal_update.txt";
        internal static string legal_update;

        // Client information
        internal static bool client_premium;
        internal static bool client_nsfw;
        internal static string client_country;

        // HttpClient for sending/receiving data from Spotify
        internal static readonly HttpClientHandler client_handler = new HttpClientHandler { UseDefaultCredentials = true };
        internal static readonly HttpClient client = new HttpClient(client_handler);

        // Security variables
        internal static string verifier_string;
        internal static int state;

        // Http Listener
        internal static HttpListener httpListener = new HttpListener();
    }

    public class Master
    {
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

            Internal.Setup();

            output.Append(Variable.version_info);
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
            if (function.Length >= 7 && function.ToLower().Substring(0, 7) == "spotify")
            {
                if (parameters.Length > 1)
                {
                    switch (parameters[1].ToLower())
                    {
                        // GET_DEVICES: Request list of users devices
                        case "get_devices":
                            await Request.GetUserDevices(parameters[2]);
                            break;

                        // SET_DEVICE: Transfer audio playback to requested device
                        case "set_device":
                            await Request.SetUserDevice(parameters[2]);
                            break;

                        // SET_VOLUME: Sets users volume for the current active device
                        case "set_volume":
                            try
                            {
                                await Request.SetUserVolume(int.Parse(parameters[2]));
                            }
                            catch (Exception e)
                            {
                                Debug.Info("An error occurred while attempting to set player volume...");
                                Debug.Error(e.ToString());
                            }
                            break;

                        // SKIP: Skips forward/backwards to the next item in playlist
                        case "skip":
                            if (parameters[2] == "next")
                                await Request.SkipNext();
                            else
                                await Request.SkipBack();
                            break;

                        // PLAY: Starts/resumes users playback of the current song/defined song
                        case "play":
                            await Request.ResumePlayback();
                            break;

                        // PAUSE: Pauses users playback of the current song
                        case "pause":
                            await Request.PausePlayback();
                            break;

                        // REPEAT: Set repeat state of player
                        case "repeat":
                            await Request.Repeat(parameters[2]);
                            break;

                        // SHUFFLE: Set shuffle state of player
                        case "shuffle":
                            await Request.Shuffle(parameters[2] == "true");
                            break;

                        // LIKE: Like a song by ID
                        case "like":
                            await Request.LikeSong(parameters[2]);
                            break;

                        // UNLIKE: Unlike a song by ID
                        case "unlike":
                            await Request.UnlikeSong(parameters[2]);
                            break;

                        // LIKED: Check if song ID is liked by user
                        case "liked":
                            await Request.SongLiked(parameters[2]);
                            break;

                        // SEEK: Seek to position in current playing track
                        case "seek":
                            try
                            {
                                await Request.Seek(int.Parse(parameters[2]));
                            }
                            catch (Exception e)
                            {
                                Debug.Info("An error occurred while attempting to seek...");
                                Debug.Error(e.ToString());
                            }
                            break;

                        // CONNECT_WEBSITE: Open the Spotify Connect website
                        case "connect_website":
                            Process.Start("https://www.spotify.com/connect/");
                            break;

                        // DOWNLOAD_IMAGE: Downloads image from spotify server and then sets it to a control
                        case "download_image":
                            await Image.Download(@"https://i.scdn.co/" + parameters[2], parameters[3]);
                            break;

                        // REQUEST_INFO: Request updated info from Spotify
                        case "request_info":
                            await Request.RequestInfo();
                            break;

                        // PREMIUM: Returns if the user who has authorised has an active Spotify Premium subscription
                        case "premium":
                            output.Append(Variable.client_premium.ToString().ToLower());
                            break;

                        // APPEND_TRACK: Appends a track to the players queue
                        case "append_track":
                            await Request.AppendQueue(parameters[2]);
                            break;

                        // PLAY_TRACK: Plays a track using the track ID
                        case "play_track":
                            await Request.PlayTrack(parameters[2]);
                            break;

                        // PLAY_ALBUM: Plays an album using the album ID and start index
                        case "play_album":
                            try
                            {
                                if (parameters.Length >= 4)
                                {
                                    await Request.PlayAlbum(parameters[2], int.Parse(parameters[3]));
                                }
                                else
                                {
                                    await Request.PlayAlbum(parameters[2]);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Info("An error occurred while attempting to play an album...");
                                Debug.Error(e.ToString());
                            }
                            break;

                        // PLAY_PLAYLIST: Plays a playlist using the playlist ID and start index
                        case "play_playlist":
                            try
                            {
                                if (parameters.Length >= 4)
                                {
                                    await Request.PlayPlaylist(parameters[2], int.Parse(parameters[3]));
                                }
                                else
                                {
                                    await Request.PlayPlaylist(parameters[2]);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Info("An error occurred while attempting to play a playlist...");
                                Debug.Error(e.ToString());
                            }
                            break;

                        // GET_RECENT: Get recent tacks played for the home page list
                        case "get_recent":
                            await Request.GetRecentTracks(parameters[2]);
                            break;

                        // GET_RECENT_MAIN: Get recent tacks played for the home page list, for the main recently played section
                        case "get_recent_main":
                            await Request.GetRecentTracksMain(parameters[2]);
                            break;

                        // GET_LIKED_MAIN: Get liked playlist, then add to the liked song list.
                        case "get_liked_main":
                            try
                            {
                                if (parameters.Length >= 4)
                                {
                                    await Request.GetLikedTracksMain(parameters[2], int.Parse(parameters[3]));
                                }
                                else
                                {
                                    await Request.GetLikedTracksMain(parameters[2]);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Info("An error occurred while attempting to get liked tracks...");
                                Debug.Error(e.ToString());
                            }
                            break;

                        // GET_RELEASES: Get new releases from Spotify
                        case "get_releases":
                            await Request.GetNewReleases(parameters[2]);
                            break;

                        // GET_FEATURED: Get featured playlists
                        case "get_featured":
                            await Request.GetFeaturedPlaylists(parameters[2]);
                            break;

                        // REQUEST_PLAYLISTS: Get users playlists
                        case "request_playlists":
                            await Request.GetUsersPlaylists();
                            break;

                        // LOAD_PLAYLIST: Loads a playlist and shows it in the GUI
                        case "load_playlist":
                            try
                            {
                                if (parameters.Length >= 5)
                                {
                                    await Request.LoadPlaylist(parameters[2], parameters[3], int.Parse(parameters[4]));
                                }
                                else
                                {
                                    await Request.LoadPlaylist(parameters[2], parameters[3]);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Info("An error occurred while attempting to load a playlist...");
                                Debug.Error(e.ToString());
                            }
                            break;

                        // LOAD_ALBUM: Loads a album and shows it in the GUI
                        case "load_album":
                            try
                            {
                                if (parameters.Length >= 5)
                                {
                                    await Request.LoadAlbum(parameters[2], parameters[3], int.Parse(parameters[4]));
                                }
                                else
                                {
                                    await Request.LoadAlbum(parameters[2], parameters[3]);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Info("An error occurred while attempting to load an album...");
                                Debug.Error(e.ToString());
                            }
                            break;

                        // TRACK: This opens the track in the spotify player
                        case "track":
                            Process.Start("https://open.spotify.com/track/" + parameters[2]);
                            break;

                        // ARTIST: This opens the artist in the spotify player
                        case "artist":
                            Process.Start("https://open.spotify.com/artist/" + parameters[2]);
                            break;

                        // ALBUM: This opens the album in the spotify player
                        case "album":
                            Process.Start("https://open.spotify.com/album/" + parameters[2]);
                            break;
                    }
                };
            }
            else
            {
                // Switch through all the other options
                switch (parameters[0].ToLower())
                {
                    // AUTHORISE_WEBSITE: Open url to allow the user to authorise this application
                    case "authorise_website":
                        Internal.StartAuth();
                        break;

                    // AUTHORISED: Check if the user is already authorised
                    case "authorised":
                        bool check = Variable.client_access_token != null && Variable.client_refresh_token != null;
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

                    // LOG: Show where logs are being saved to
                    case "log":
                        output.Append(Variable.log_directory);
                        break;

                    // DATA: Show where images are being saved to
                    case "data":
                        output.Append(Variable.data_directory);
                        break;

                    // CLEAR_CACHE: Deletes the cache directory
                    case "clear_cache":
                        Debug.Clear(0);
                        break;

                    // CLEAR_LOGS: Deletes the cache directory
                    case "clear_logs":
                        Debug.Clear(1);
                        break;

                    // OPEN_CACHE: Deletes the cache directory
                    case "open_cache":
                        Process.Start(Variable.data_directory);
                        break;

                    // OPEN_LOGS: Deletes the cache directory
                    case "open_logs":
                        Process.Start(Variable.log_directory);
                        break;

                    // GITHUB: Open the GitHub page for the mod
                    case "github":
                        Process.Start("https://github.com/Asaayu/Arma-Spotify-Player");
                        break;

                    // OPEN_SPOTIFY: Open the open.spotify.com webpage
                    case "open_spotify":
                        Process.Start("https://open.spotify.com");
                        break;

                    // REVOKE: Open the webpage for users to revoke access to their Spotify account
                    case "revoke":
                        Process.Start("https://www.spotify.com/account/apps/");
                        break;

                    // EULA: Opens EULA agreement
                    case "eula":
                        Process.Start("https://github.com/Asaayu/Arma-Spotify-Player/blob/main/EULA.md");
                        break;

                    // PRIVACY: Opens Privacy Policy
                    case "privacy":
                        Process.Start("https://github.com/Asaayu/Arma-Spotify-Player/blob/main/PRIVACY-POLICY.md");
                        break;

                    // LEGAL_UPDATE: Get the EULA & Privacy Policy last update time
                    case "legal_update":
                        output.Append(Variable.legal_update);
                        break;

                    // INFO: Show version information
                    case "info":
                        output.Append(Variable.version_info);
                        break;
                }
            }
        }
    }

    public class Internal
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        internal async static void Setup()
        {
            // Check for debug parameter
            Variable.debug = Environment.CommandLine.Contains("-spotify_console");

            if (Variable.debug)
                AllocConsole();

            // Get current directory
            String current_directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Save locations and files
            Variable.data_directory = current_directory + @"\data\";
            Variable.log_directory = current_directory + @"\logs\";
            Variable.log_file = Variable.log_directory + "ArmaSpotifyController_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";
            Variable.token_file = Path.GetTempPath() + @"\aasp.token";

            try
            {
                // Create directories 
                Directory.CreateDirectory(Variable.data_directory);
                Directory.CreateDirectory(Variable.log_directory);
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to create required directories...");
                Debug.Error(e.ToString());
            };

            // Delete old files in the data directory
            string[] files = Directory.GetFiles(Variable.data_directory);
            foreach (string file in files)
            {
                try
                {
                    FileInfo info = new FileInfo(file);
                    if (info.LastAccessTime < DateTime.Now.AddDays(-3))
                    {
                        info.Delete();
                    }
                }
                catch
                {
                    // If a file fails to delete just skip it.
                    continue;
                };
            }

            // Fix possible SSL/TLS issue when downloading strings
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            Debug.Info("Generating state key...");

            RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();
            byte[] data = new byte[4];
            generator.GetBytes(data);
            Variable.state = BitConverter.ToInt32(data, 0);

            Debug.Info("Connecting to GitHub...");
            Debug.Info("Downloading last legal update...");
            Variable.legal_update = DownloadString(Variable.legal_update_file).Result;

            // Check if token file exists            
            if (File.Exists(Variable.token_file))
            {
                Debug.Info("Discovered a token file...");
                Debug.Info("Reading user info from token file...");

                string serial = (string)new ManagementObject("Win32_OperatingSystem=@")["SerialNumber"];

                // Read refresh token to get access token without user having to authenticate again
                string token = Security.Decrypt(File.ReadAllText(Variable.token_file), serial).Trim();

                // Save refresh token for refresh function
                Variable.client_refresh_token = token;

                Debug.Info("Refreshing user data...");

                // Get new token through task in the background
                await Request.RefeshData(true);
            }
        }

        internal static void Token(int mode)
        {
            try
            {
                if (mode <= 0)
                {
                    Debug.Info("Saving info to token file...");

                    string serial = (string)new ManagementObject("Win32_OperatingSystem=@")["SerialNumber"];

                    File.WriteAllText(Variable.token_file, Security.Encrypt(Variable.client_refresh_token, serial));
                }
                else
                {
                    Debug.Info("Deleting token file...");
                    File.Delete(Variable.token_file);
                };
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to save token file...");
                Debug.Error(e.ToString());
            };
        }

        internal async static Task<string> DownloadString(string uri, bool trim_nl = true)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                string output = await Variable.client.GetStringAsync(uri);
                if (trim_nl)
                {
                    output.Replace("\n", "").Replace("\r", "");
                };
                return output.Trim();
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured downloading string...");
                Debug.Error(e.ToString());
                return "";
            }
        }

        internal static void StartAuth()
        {
            // User wants to re-authenticate, delete the old token file
            Token(1);

            // Create code verifier
            Variable.verifier_string = Security.RandomString(110);

            // Create code challange
            byte[] sha_bytes = SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(Variable.verifier_string));

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
                "user-modify-playback-state",
                "user-library-modify",
                "user-library-read",
                "streaming"
            };

            // URL parameters
            var parameters_dict = new Dictionary<string, string>
            {
                { "response_type", "code" },
                { "client_id", Variable.app_client_id },
                { "redirect_uri", HttpUtility.UrlEncode("http://localhost:5000/callback") },
                { "scope", String.Join("%20", scopes)},
                { "state", Variable.state.ToString() },
                { "code_challenge", challange_string },
                { "code_challenge_method", "S256" }
            };

            // DO NOT ALLOW USER TO CUSTOMIZE THIS.
            Process.Start(string.Format("https://accounts.spotify.com/authorize?{0}", string.Join("&", parameters_dict.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))));

            // Start HTTP Listener
            Variable.httpListener.Prefixes.Add("http://localhost:5000/");
            Variable.httpListener.Start();
            Thread responseThread = new Thread(AuthResponseThread);
            responseThread.Start(); // start the response thread
        }

        internal static async void AuthResponseThread()
        {
            try
            {
                while (true)
                {
                    HttpListenerContext context = Variable.httpListener.GetContext();
                    string user_code = HttpUtility.ParseQueryString(context.Request.Url.Query).Get("code");
                    string user_state = HttpUtility.ParseQueryString(context.Request.Url.Query).Get("state");

                    // Make sure state is correct, else reject authorization
                    if (user_state == Variable.state.ToString())
                    {
                        // POST URL
                        String post_url = "https://accounts.spotify.com/api/token";
                        // Setup POST data
                        var values = new Dictionary<string, string>
                        {
                            { "client_id", Variable.app_client_id },
                            { "grant_type", "authorization_code" },
                            { "redirect_uri", "http://localhost:5000/callback" },
                            { "code", user_code },
                            { "code_verifier", Variable.verifier_string }
                        };
                        var content = new FormUrlEncodedContent(values);

                        // Send request + content in POST 
                        var response = await Variable.client.PostAsync(post_url, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var serializer = new JavaScriptSerializer();
                            Classes.AccessToken.Root result = serializer.Deserialize<Classes.AccessToken.Root>(await response.Content.ReadAsStringAsync());

                            Variable.client_access_token = result.access_token;
                            Variable.client_refresh_token = result.refresh_token;

                            // Refresh token
                            Internal.Token(0);

                            // Set the refresh timer
                            Variable.client_refresh_time = DateTime.Now.AddSeconds(3300);

                            // Callback to game to let it know user is authorised
                            Master.callback.Invoke("ArmaSpotifyController", "setVariable", "[\"uinamespace\", \"aasp_authorised\", true]");

                            // Save user info to client variables in DLL for later use
                            await Request.GetUserInfo();

                            Debug.Info("User authentication successful");
                        }
                        else
                        {
                            Debug.Info("Error: " + response.StatusCode.ToString());
                            Debug.Info(response.ReasonPhrase);
                        }
                    }
                    else
                    {
                        // Missing state key
                        Debug.Info("ERROR: Incorrect 'state' key. Reauthorization required!");
                    }

                    context.Response.Redirect("http://asaayu.com/arma-3/spotify/auth.php");

                    await Task.Delay(150);

                    // Reload UI
                    Master.callback.Invoke("ArmaSpotifyController", "reload_display", "");

                    context.Response.KeepAlive = false;
                    context.Response.Close();
                    Variable.httpListener.Stop();
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.Info(e.ToString());
            }
        }
    };

    public class Image
    {
        internal static async Task<string> Download(string uri_string, string variable = "")
        {
            try
            {
                // Don't try to download an empty string
                if (uri_string == "https://i.scdn.co/")
                {
                    if (variable != "")
                    {
                        Master.callback.Invoke("ArmaSpotifyController", "ctrlSetText", "#(rgb,8,8,3)color(0.3,0.3,0.3,1)" + "|" + variable);
                    }
                    return "#(rgb,8,8,3)color(0.3,0.3,0.3,1)";
                }

                // Create new URI from string input
                Uri uri = new Uri(uri_string);

                // Get filename from Spotify web server                
                string filename = Path.GetFileNameWithoutExtension(uri.LocalPath);

                // Create file path along with new extension
                var path = Path.Combine(Variable.data_directory, filename + ".jpg");

                if (!File.Exists(path.ToString()))
                {
                    // Download the image and write to the file
                    var imageBytes = await Variable.client.GetByteArrayAsync(uri);
                    File.WriteAllBytes(path, imageBytes);
                }

                if (variable != "")
                {
                    Master.callback.Invoke("ArmaSpotifyController", "ctrlSetText", path + "|" + variable);
                }

                return path.ToString();
            }
            catch (Exception e)
            {
                if (variable != "")
                {
                    Master.callback.Invoke("ArmaSpotifyController", "ctrlSetText", "#(rgb,8,8,3)color(0.3,0.3,0.3,1)" + "|" + variable);
                }

                Debug.Info("Attempted to load image from URL but ran into error: " + e.ToString());
                Debug.Info("Uri: " + uri_string);
                return "#(rgb,8,8,3)color(0.3,0.3,0.3,1)";
            }
        }

        internal static string[] Draw(String text, Font font, Color textColor, Color backColor, bool strip_html = false)
        {
            if (text == "")
            {
                // Output an exmpy reply for no text imput, number cannot be 0
                return new string[] { "", "1", "1" };
            }

            if (strip_html)
            {
                // Remove html tags 
                char[] artist_array = new char[text.Length];
                int arrayIndex = 0;
                bool inside = false;

                for (int i = 0; i < text.Length; i++)
                {
                    char let = text[i];
                    if (let == '<')
                    {
                        inside = true;
                        continue;
                    }
                    else if (let == '>')
                    {
                        inside = false;
                        continue;
                    }

                    if (!inside)
                    {
                        artist_array[arrayIndex] = let;
                        arrayIndex++;
                    }
                }
                text = new string(artist_array, 0, arrayIndex);
            }

            // Get filename
            byte[] sha_bytes = SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            String filename = Convert.ToBase64String(sha_bytes).Split('=')[0].Replace('+', '-').Replace('/', '_');

            var path = Path.Combine(Variable.data_directory, filename + "_" + font.Size + "_" + font.Style + "_" + textColor.R.ToString() + textColor.G.ToString() + textColor.B.ToString() + textColor.A.ToString() + "_" + backColor.R.ToString() + backColor.G.ToString() + backColor.B.ToString() + backColor.A.ToString() + ".jpg");

            // Check if file already exists, if it does then don't bother creating another one
            if (File.Exists(@path.ToString()))
            {
                // Wait the refresh time so that we have time for the image to be written
                Task.Delay(250);
                System.Drawing.Image ext_img = System.Drawing.Image.FromFile(path.ToString());
                return new string[] { path.ToString(), ext_img.Width.ToString(), ext_img.Height.ToString() };
            }

            // First, create a dummy bitmap just to get a graphics object
            System.Drawing.Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            // Measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            // Free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            // Create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            // Paint the background
            drawing.Clear(backColor);

            // Create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            // Draw string
            drawing.DrawString(text, font, textBrush, 0, 0);

            // Save drawing
            drawing.Save();

            // Dispose of the evidence ;)
            textBrush.Dispose();
            drawing.Dispose();

            // Save to disk
            img.Save(path.ToString(), ImageFormat.Jpeg);

            // Send data in return
            return new string[] { path.ToString(), textSize.Width.ToString(), textSize.Height.ToString() };
        }
    }

    public class Debug
    {
        internal static bool Info(string message, string prefix = "INFO")
        {
            try
            {
                String text = DateTime.Now.ToString("[dd/MM/yyyy hh:mm:ss tt]") + "[" + prefix + "] " + message;

                if (Variable.debug)
                    Console.WriteLine(text);

                using (StreamWriter sw = File.AppendText(Variable.log_file))
                {
                    sw.WriteLine(text);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool Error(string message)
        {
            return Info(message, "ERROR");
        }

        internal static void Clear(int mode)
        {
            string directory = Variable.log_directory;
            if (mode <= 0)
            {
                directory = Variable.data_directory;
            }

            Info("Clearing files...");

            int deleted = 0;
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                // Do not delete the current log file
                if (file != Variable.log_file)
                {
                    try
                    {
                        new FileInfo(file).Delete();
                        deleted += 1;
                    }
                    catch
                    {
                        continue;
                    };
                }
            }

            Info("Deleted " + deleted + " file(s)...");
        }
    }

    public class Security
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

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

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }

    public class Request
    {
        internal async static Task RefeshData(bool save_info = false)
        {
            try
            {
                if (DateTime.Now < Variable.client_refresh_time || Variable.client_refresh_token == null)
                    return;

                // Setup POST data
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", Variable.client_refresh_token },
                    { "client_id", Variable.app_client_id }
                };

                Variable.client.DefaultRequestHeaders.Clear();

                HttpContent content = new FormUrlEncodedContent(values);

                var response = await Variable.client.PostAsync("https://accounts.spotify.com/api/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.RefreshToken.Root result = serializer.Deserialize<Classes.RefreshToken.Root>(await response.Content.ReadAsStringAsync());

                    string old_token = Variable.client_refresh_token;

                    Variable.client_access_token = result.access_token;
                    Variable.client_refresh_token = result.refresh_token;

                    // Refresh token
                    Internal.Token(0);

                    // Set the refresh timer
                    Variable.client_refresh_time = DateTime.Now.AddSeconds(3300);

                    Debug.Info("Users refresh token has been updated successfully");

                    if (save_info)
                    {
                        Debug.Info("Loading user info...");

                        await GetUserInfo();
                    }
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        // Remove the access + refresh token to unauthorise the user
                        Variable.client_access_token = null;
                        Variable.client_refresh_token = null;
                    }
                    Debug.Info("An error was returned when attempting to refresh user data...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to refresh user data...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task GetUserInfo()
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);
                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/me");

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.UserInfo.Root result = serializer.Deserialize<Classes.UserInfo.Root>(await response.Content.ReadAsStringAsync());

                    Variable.client_premium = result.product == "premium";
                    Variable.client_nsfw = !result.explicit_content.filter_enabled;
                    Variable.client_country = result.country;

                    // Log messages to allow debugging
                    Debug.Info("=====================");
                    Debug.Info("Premium: " + Variable.client_premium.ToString().ToLower());
                    Debug.Info("Show NSFW: " + Variable.client_nsfw.ToString().ToLower());
                    Debug.Info("Country: " + Variable.client_country);
                    Debug.Info("=====================");

                    // Callback to game to let it know user info has been saved
                    Master.callback.Invoke("ArmaSpotifyController", "setVariable", "[\"uinamespace\", \"aasp_info_saved\", true]");
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get user data...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get user data...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task GetUserDevices(string variable_name)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);
                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/me/player/devices");

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.Devices.Root result = serializer.Deserialize<Classes.Devices.Root>(await response.Content.ReadAsStringAsync());

                    List<string> devices = new List<string>();
                    foreach (Classes.Devices.Device device in result.devices)
                    {
                        devices.Add(string.Format("[\"{0}\",{1},{2},{3},\"{4}\",\"{5}\",{6}]",
                            device.id,
                            device.is_active,
                            device.is_private_session,
                            device.is_restricted,
                            device.name,
                            device.type,
                            device.volume_percent
                            ));
                    };

                    // Callback to game to add items to listbox
                    string callback_data = "['display',['" + variable_name + "', [" + string.Join(",", devices) + "]]]";
                    Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_get_devices", callback_data);
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get user devices...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get user devices...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task SetUserDevice(string device_id)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var content = new StringContent("{\"device_ids\" : [\"" + device_id + "\"], \"play\" : " + Variable.is_playing.ToString().ToLower() + "}");

                var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player", content);

                if (response.IsSuccessStatusCode)
                {
                    Variable.last_connection = true;

                    Debug.Info("Transfered audio playback to " + device_id);
                    Debug.Info("Playing: " + Variable.is_playing);
                    Variable.last_device_id = device_id;
                    await Task.Delay(Variable.refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    // Disable reconnection if connection failed
                    Variable.last_connection = false;

                    Debug.Info("An error was returned when attempting to set the users device...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to set the users device...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task SetUserVolume(int volume_level)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/volume?volume_percent=" + volume_level, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Debug.Info("Set users volume to " + volume_level);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (Variable.last_device_id != null)
                        {
                            Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                            await SetUserDevice(Variable.last_device_id);
                            await Task.Delay(Variable.refresh_delay);
                            await SetUserVolume(volume_level);
                        }
                    }
                    Debug.Info("An error was returned when attempting to set player volume...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to set player volume...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task PausePlayback()
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/pause", new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Debug.Info("Paused audio playback");
                    await Task.Delay(Variable.refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (Variable.last_device_id != null && Variable.last_connection)
                        {
                            Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                            await SetUserDevice(Variable.last_device_id);
                            await Task.Delay(Variable.refresh_delay);
                            await PausePlayback();
                        }
                    }
                    Debug.Info("An error was returned when attempting to pause playback...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to pause playback...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task ResumePlayback()
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/play", new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Debug.Info("Resumed audio playback");
                    await Task.Delay(Variable.refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (Variable.last_device_id != null)
                        {
                            Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                            await SetUserDevice(Variable.last_device_id);
                            await Task.Delay(Variable.refresh_delay);
                            await ResumePlayback();
                        }
                    }
                    Debug.Info("An error was returned when attempting to resume playback...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to resume playback...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task PlayTrack(string track_id)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                if (track_id != "")
                {
                    var content = new StringContent("{\"uris\":[\"spotify:track:" + track_id + "\"]}");

                    var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/play", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Info("Playing track: " + track_id);
                        await Task.Delay(Variable.refresh_delay);
                        await RequestInfo();
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (Variable.last_device_id != null)
                            {
                                Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                                await SetUserDevice(Variable.last_device_id);
                                await Task.Delay(Variable.refresh_delay);
                                await PlayTrack(track_id);
                            }
                        }
                        Debug.Info("An error was returned when attempting to play a track...");
                        Debug.Error(response.ReasonPhrase);
                    }
                }
                else
                {
                    Debug.Info("An error was returned due to an incorrect track ID...");
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to play a track...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task PlayAlbum(string album_id, int start_index = 0)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                if (album_id != "")
                {
                    var content = new StringContent("{\"context_uri\":\"spotify:album:" + album_id + "\",\"offset\": {\"position\": " + start_index + "}}");

                    var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/play", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Info("Playing album: " + album_id + " - " + start_index);
                        await Task.Delay(Variable.refresh_delay * 2);
                        await RequestInfo();
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (Variable.last_device_id != null)
                            {
                                Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                                await SetUserDevice(Variable.last_device_id);
                                await Task.Delay(Variable.refresh_delay);
                                await PlayAlbum(album_id, start_index);
                            }
                        }
                        Debug.Info("An error was returned when attempting to play an album...");
                        Debug.Error(response.ReasonPhrase);
                    }
                }
                else
                {
                    Debug.Info("An error was returned due to an incorrect album ID...");
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to play an album...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task PlayPlaylist(string playlist_id, int start_index = 0)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                if (playlist_id != "")
                {
                    var content = new StringContent("{\"context_uri\":\"spotify:playlist:" + playlist_id + "\",\"offset\": {\"position\": " + start_index + "}}");

                    var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/play", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Info("Playing playlist: " + playlist_id + " - " + start_index);
                        await Task.Delay(Variable.refresh_delay);
                        await RequestInfo();
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (Variable.last_device_id != null)
                            {
                                Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                                await SetUserDevice(Variable.last_device_id);
                                await Task.Delay(Variable.refresh_delay);
                                await PlayPlaylist(playlist_id, start_index);
                            }
                        }
                        Debug.Info("An error was returned when attempting to play a playlist...");
                        Debug.Error(response.ReasonPhrase);
                    }
                }
                else
                {
                    Debug.Info("An error was returned due to an incorrect playlist ID...");
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to play a playlist...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task SkipNext()
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.PostAsync("https://api.spotify.com/v1/me/player/next", new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Debug.Info("Skipped to next track in playlist");
                    await Task.Delay(Variable.refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (Variable.last_device_id != null)
                        {
                            Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                            await SetUserDevice(Variable.last_device_id);
                            await Task.Delay(Variable.refresh_delay);
                            await SkipNext();
                        }
                    }
                    Debug.Info("An error was returned when attempting to skip a track...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to skip a track...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task SkipBack()
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.PostAsync("https://api.spotify.com/v1/me/player/previous", new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Debug.Info("Skipped to the prevoius track in playlist");
                    await Task.Delay(Variable.refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (Variable.last_device_id != null)
                        {
                            Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                            await SetUserDevice(Variable.last_device_id);
                            await Task.Delay(Variable.refresh_delay);
                            await SkipBack();
                        }
                    }
                    Debug.Info("An error was returned when attempting to skip back a track...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to skip back a track...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task Seek(int position_ms)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/seek?position_ms=" + position_ms, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Debug.Info("Seeked to: " + position_ms);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (Variable.last_device_id != null)
                        {
                            Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                            await SetUserDevice(Variable.last_device_id);
                            await Task.Delay(Variable.refresh_delay);
                            await Seek(position_ms);
                        }
                    }
                    Debug.Info("An error was returned when attempting to seek in a track...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to seek in a track...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task Repeat(string mode)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/repeat?state=" + mode, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Debug.Info("Repeat mode set to: " + mode);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (Variable.last_device_id != null)
                        {
                            Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                            await SetUserDevice(Variable.last_device_id);
                            await Task.Delay(Variable.refresh_delay);
                            await Repeat(mode);
                        }
                    }
                    Debug.Info("An error was returned when attempting to change repeat state...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to change repeat state...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task Shuffle(bool mode)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/player/shuffle?state=" + mode, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Debug.Info("Shuffle mode: " + mode.ToString());
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (Variable.last_device_id != null)
                        {
                            Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                            await SetUserDevice(Variable.last_device_id);
                            await Task.Delay(Variable.refresh_delay);
                            await Shuffle(mode);
                        }
                    }
                    Debug.Info("An error was returned when attempting to change shuffle state...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to change shuffle state...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task SongLiked(string song_id)
        {
            try
            {
                if (song_id != "")
                {
                    Variable.client.DefaultRequestHeaders.Clear();
                    Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                    var response = await Variable.client.GetAsync("https://api.spotify.com/v1/me/tracks/contains?ids=" + song_id);

                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_like", string.Format("[{0},\"{1}\"]", data.Contains("true"), song_id));
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (Variable.last_device_id != null)
                            {
                                Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                                await SetUserDevice(Variable.last_device_id);
                                await Task.Delay(Variable.refresh_delay);
                                await SongLiked(song_id);
                            }
                        }
                        Debug.Info("An error was returned when attempting to check if a song is liked...");
                        Debug.Error(response.ReasonPhrase);
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to check if a song is liked due to an incorrect track ID...");
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to check if a song is liked...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task LikeSong(string song_id)
        {
            try
            {
                if (song_id != "")
                {
                    Variable.client.DefaultRequestHeaders.Clear();
                    Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                    var response = await Variable.client.PutAsync("https://api.spotify.com/v1/me/tracks?ids=" + song_id, new StringContent(""));

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Info("Liked song: " + song_id);
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (Variable.last_device_id != null)
                            {
                                Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                                await SetUserDevice(Variable.last_device_id);
                                await Task.Delay(Variable.refresh_delay);
                                await LikeSong(song_id);
                            }
                        }
                        Debug.Info("An error was returned when attempting to like a song...");
                        Debug.Error(response.ReasonPhrase);
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to like a song due to an incorrect track ID");
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to like a song...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task UnlikeSong(string song_id)
        {
            try
            {
                if (song_id != "")
                {
                    Variable.client.DefaultRequestHeaders.Clear();
                    Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                    var response = await Variable.client.DeleteAsync("https://api.spotify.com/v1/me/tracks?ids=" + song_id);

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Info("Unliked song: " + song_id);
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (Variable.last_device_id != null)
                            {
                                Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                                await SetUserDevice(Variable.last_device_id);
                                await Task.Delay(Variable.refresh_delay);
                                await UnlikeSong(song_id);
                            }
                        }
                        Debug.Info("An error was returned when attempting to unlike a song...");
                        Debug.Error(response.ReasonPhrase);
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to unlike a song with an incorrect track ID");
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get user data...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task AppendQueue(string song_id)
        {
            try
            {
                if (song_id != "")
                {
                    Variable.client.DefaultRequestHeaders.Clear();
                    Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                    var response = await Variable.client.PostAsync("https://api.spotify.com/v1/me/player/queue?uri=spotify%3Atrack%3A" + song_id, new StringContent(""));

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Info("Appended song to queue: " + song_id);
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (Variable.last_device_id != null)
                            {
                                Debug.Info("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection...");
                                await SetUserDevice(Variable.last_device_id);
                                await Task.Delay(Variable.refresh_delay);
                                await AppendQueue(song_id);
                            }
                        }
                        Debug.Info("An error was returned when attempting to append to the queue...");
                        Debug.Error(response.ReasonPhrase);
                    }
                }
                else
                {
                    Debug.Info("Attempted to append song to queue, but no song id was found");
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to append to the queue...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task GetRecentTracksMain(string variable)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/me/player/recently-played?limit=50");

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.RecentTracks.Root data = serializer.Deserialize<Classes.RecentTracks.Root>(await response.Content.ReadAsStringAsync());

                    List<string> tracks = new List<string>();
                    foreach (Classes.RecentTracks.Item item in data.items)
                    {
                        if (tracks.IndexOf(item.track.id) <= -1)
                        {
                            // Stop multiple songs appearing in the list
                            tracks.Add(item.track.id);

                            List<string> artists_list = new List<string>();
                            foreach (Classes.RecentTracks.Artist artist in item.track.artists)
                            {
                                artists_list.Add(artist.name);
                            };
                            string artists = string.Join(", ", artists_list);

                            // Download text for title + author
                            string[] title_image = Image.Draw(item.track.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"));
                            string[] artist_image = Image.Draw(artists, new Font("Arial", 14, FontStyle.Regular), ColorTranslator.FromHtml("#828282"), ColorTranslator.FromHtml("#242424"));

                            Master.callback.Invoke("ArmaSpotifyController", "append_grid",
                                string.Format("[\"{0}\",\"{1}\",\"{2}\",\"{3}\",[\"{4}\",{5},{6}],[\"{7}\",{8},{9}]]",
                                    variable,
                                    item.track.id,
                                    "track",
                                    item.track.album.images[item.track.album.images.FindIndex(a => a.height == 300 && a.width == 300)].url.Remove(0, 18),
                                    title_image[0],
                                    title_image[1],
                                    title_image[2],
                                    artist_image[0],
                                    artist_image[1],
                                    artist_image[2]
                                )
                            );
                        }
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get recent tracks...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get recent tracks...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task LoadAlbum(string variable, string album_id, int offset = 0)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var items_response = await Variable.client.GetAsync("https://api.spotify.com/v1/albums/" + album_id + "/tracks?limit=50&offset=" + offset.ToString() + "&market=" + Variable.client_country);
                var info_response = await Variable.client.GetAsync("https://api.spotify.com/v1/albums/" + album_id);

                if (items_response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.AlbumItems.Root data = serializer.Deserialize<Classes.AlbumItems.Root>(await items_response.Content.ReadAsStringAsync());

                    List<string> tracks = new List<string>();
                    foreach (Classes.AlbumItems.Item item in data.items)
                    {
                        List<string> artists_list = new List<string>();
                        foreach (Classes.AlbumItems.Artist artist in item.artists)
                        {
                            artists_list.Add(artist.name);
                        };
                        string artists = string.Join(", ", artists_list);

                        TimeSpan t = TimeSpan.FromSeconds(item.duration_ms / 1000);
                        string length = "";
                        if (item.duration_ms / 1000 >= 3600)
                        {
                            length = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                        }
                        else
                        {
                            length = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
                        };

                        // Download text for title + author
                        string[] title_image = Image.Draw(item.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"));
                        string[] artist_image = Image.Draw(artists, new Font("Arial", 19, FontStyle.Regular), Color.White, ColorTranslator.FromHtml("#242424"));
                        string[] length_image = Image.Draw(length, new Font("Arial", 19, FontStyle.Regular), Color.White, ColorTranslator.FromHtml("#242424"));

                        Master.callback.Invoke("ArmaSpotifyController", "append_textlist_playlist",
                            string.Format("[\"{0}\",\"{1}\",[\"{2}\",{3},{4}],[\"{5}\",{6},{7}],[\"{8}\",{9},{10}],\"{11}\",\"{12}\",{13},{14}]",
                                variable,
                                album_id,
                                title_image[0],
                                title_image[1],
                                title_image[2],
                                artist_image[0],
                                artist_image[1],
                                artist_image[2],
                                length_image[0],
                                length_image[1],
                                length_image[2],
                                album_id,
                                item.uri,
                                data.total,
                                data.offset
                            )
                        );
                    }
                }
                else
                {
                    Debug.Info("'LoadAlbum' Error: " + items_response.StatusCode.ToString());
                    Debug.Info(items_response.ReasonPhrase);
                }

                if (info_response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.AlbumInfo.Root data = serializer.Deserialize<Classes.AlbumInfo.Root>(await info_response.Content.ReadAsStringAsync());

                    string playlist_logo_image = "";
                    if (data.images.Count > 1)
                    {
                        playlist_logo_image = Image.Download(data.images[data.images.FindIndex(a => int.Parse(a.height.ToString()) == 300 && int.Parse(a.width.ToString()) == 300)].url).Result;
                    }
                    else
                    {
                        playlist_logo_image = Image.Download(data.images[0].url).Result;
                    };

                    List<string> artists_list = new List<string>();
                    foreach (Classes.AlbumInfo.Artist artist in data.artists)
                    {
                        artists_list.Add(artist.name);
                    };
                    string artists = string.Join(", ", artists_list);

                    List<string> copyright_list = new List<string>();
                    foreach (Classes.AlbumInfo.Copyright copyright in data.copyrights)
                    {
                        copyright_list.Add(copyright.text
                            .Replace("(c)", "©")
                            .Replace("(C)", "©")
                            .Replace("(p)", "℗")
                            .Replace("(P)", "℗")
                            );
                    };
                    string copyrights = string.Join(" ", copyright_list);

                    string[] playlist_title_image = Image.Draw(data.name, new Font("Arial", 26, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"));
                    string[] playlist_subtitle_image = Image.Draw(artists, new Font("Arial", 22, FontStyle.Regular), Color.White, ColorTranslator.FromHtml("#242424"));
                    string[] playlist_copyright_image = Image.Draw(copyrights, new Font("Arial", 20, FontStyle.Regular), ColorTranslator.FromHtml("#757575"), ColorTranslator.FromHtml("#242424"));

                    Master.callback.Invoke("ArmaSpotifyController", "set_album_info",
                        string.Format("[[\"{0}\",{1},{2}],[\"{3}\",{4},{5}],[\"{6}\",{7},{8}],\"{9}\"]",
                            playlist_title_image[0],
                            playlist_title_image[1],
                            playlist_title_image[2],
                            playlist_subtitle_image[0],
                            playlist_subtitle_image[1],
                            playlist_subtitle_image[2],
                            playlist_copyright_image[0],
                            playlist_copyright_image[1],
                            playlist_copyright_image[2],
                            playlist_logo_image
                        )
                    );
                }
                else
                {
                    Debug.Info("An error was returned when attempting to load an album...");
                    Debug.Error(info_response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to load an album...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task LoadPlaylist(string variable, string playlist_id, int offset = 0)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var items_response = await Variable.client.GetAsync("https://api.spotify.com/v1/playlists/" + playlist_id + "/tracks?limit=50&offset=" + offset.ToString() + "&market=" + Variable.client_country);
                var info_response = await Variable.client.GetAsync("https://api.spotify.com/v1/playlists/" + playlist_id + "?fields=name%2Cdescription%2Cimages");

                if (items_response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.PlaylistItems.Root data = serializer.Deserialize<Classes.PlaylistItems.Root>(await items_response.Content.ReadAsStringAsync());

                    List<string> tracks = new List<string>();
                    foreach (Classes.PlaylistItems.Item item in data.items)
                    {
                        List<string> artists_list = new List<string>();
                        foreach (Classes.PlaylistItems.Artist artist in item.track.artists)
                        {
                            artists_list.Add(artist.name);
                        };
                        string artists = string.Join(", ", artists_list);

                        // Download text for title + author
                        string[] title_image = Image.Draw(item.track.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"));
                        string[] artist_image = Image.Draw(artists, new Font("Arial", 19, FontStyle.Regular), Color.White, ColorTranslator.FromHtml("#242424"));
                        string[] album_image = Image.Draw(item.track.album.name, new Font("Arial", 19, FontStyle.Regular), Color.White, ColorTranslator.FromHtml("#242424"));

                        Master.callback.Invoke("ArmaSpotifyController", "append_textlist_playlist",
                            string.Format("[\"{0}\",\"{1}\",[\"{2}\",{3},{4}],[\"{5}\",{6},{7}],[\"{8}\",{9},{10}],\"{11}\",\"{12}\",{13},{14}]",
                                variable,
                                playlist_id,
                                title_image[0],
                                title_image[1],
                                title_image[2],
                                artist_image[0],
                                artist_image[1],
                                artist_image[2],
                                album_image[0],
                                album_image[1],
                                album_image[2],
                                item.track.album.id,
                                item.track.uri,
                                data.total,
                                data.offset
                            )
                        );
                    }
                }
                else
                {
                    Debug.Info("'LoadPlaylist' Error: " + items_response.StatusCode.ToString());
                    Debug.Info(items_response.ReasonPhrase);
                }

                if (info_response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.PlaylistInfo.Root data = serializer.Deserialize<Classes.PlaylistInfo.Root>(await info_response.Content.ReadAsStringAsync());

                    string playlist_logo_image = "";
                    if (data.images.Count > 1)
                    {
                        playlist_logo_image = Image.Download(data.images[data.images.FindIndex(a => int.Parse(a.height.ToString()) == 300 && int.Parse(a.width.ToString()) == 300)].url).Result;
                    }
                    else
                    {
                        playlist_logo_image = Image.Download(data.images[0].url).Result;
                    };

                    string[] playlist_title_image = Image.Draw(data.name, new Font("Arial", 28, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"));
                    string[] playlist_subtitle_image = Image.Draw(data.description, new Font("Arial", 22, FontStyle.Regular), ColorTranslator.FromHtml("#969696"), ColorTranslator.FromHtml("#242424"));

                    Master.callback.Invoke("ArmaSpotifyController", "set_playlist_info",
                        string.Format("[[\"{0}\",{1},{2}],[\"{3}\",{4},{5}],\"{6}\"]",
                            playlist_title_image[0],
                            playlist_title_image[1],
                            playlist_title_image[2],
                            playlist_subtitle_image[0],
                            playlist_subtitle_image[1],
                            playlist_subtitle_image[2],
                            playlist_logo_image
                        )
                    );
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get load a playlist...");
                    Debug.Error(info_response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get load a playlist...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task GetLikedTracksMain(string variable, int offset = 0)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/me/tracks?limit=50&offset=" + offset.ToString());

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.LikedTrack.Root data = serializer.Deserialize<Classes.LikedTrack.Root>(await response.Content.ReadAsStringAsync());

                    List<string> tracks = new List<string>();
                    foreach (Classes.LikedTrack.Item item in data.items)
                    {
                        List<string> artists_list = new List<string>();
                        foreach (Classes.LikedTrack.Artist artist in item.track.artists)
                        {
                            artists_list.Add(artist.name);
                        };
                        string artists = string.Join(", ", artists_list);

                        // Download text for title + author
                        string[] title_image = Image.Draw(item.track.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"));
                        string[] artist_image = Image.Draw(artists, new Font("Arial", 19, FontStyle.Regular), Color.White, ColorTranslator.FromHtml("#242424"));
                        string[] album_image = Image.Draw(item.track.album.name, new Font("Arial", 19, FontStyle.Regular), Color.White, ColorTranslator.FromHtml("#242424"));

                        Master.callback.Invoke("ArmaSpotifyController", "append_textlist_playlist",
                            string.Format("[\"{0}\",\"{1}\",[\"{2}\",{3},{4}],[\"{5}\",{6},{7}],[\"{8}\",{9},{10}],\"{11}\",\"{12}\",{13},{14},{15}]",
                                variable,
                                item.track.id,
                                title_image[0],
                                title_image[1],
                                title_image[2],
                                artist_image[0],
                                artist_image[1],
                                artist_image[2],
                                album_image[0],
                                album_image[1],
                                album_image[2],
                                item.track.album.id,
                                item.track.uri,
                                data.total,
                                data.offset,
                                true
                            )
                        );
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get liked tracks...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get liked tracks...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task GetRecentTracks(string variable, int limit = 10)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/me/player/recently-played?limit=50");

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.RecentTracks.Root data = serializer.Deserialize<Classes.RecentTracks.Root>(await response.Content.ReadAsStringAsync());

                    List<string> tracks = new List<string>();
                    foreach (Classes.RecentTracks.Item item in data.items)
                    {
                        if (tracks.IndexOf(item.track.id) <= -1)
                        {
                            // Stop multiple songs appearing in the list
                            tracks.Add(item.track.id);

                            List<string> artists_list = new List<string>();
                            foreach (Classes.RecentTracks.Artist artist in item.track.artists)
                            {
                                artists_list.Add(artist.name);
                            };
                            string artists = string.Join(", ", artists_list);

                            // Download text for title + author
                            string[] title_image = Image.Draw(item.track.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"));
                            string[] artist_image = Image.Draw(artists, new Font("Arial", 14, FontStyle.Regular), ColorTranslator.FromHtml("#828282"), ColorTranslator.FromHtml("#242424"));

                            Master.callback.Invoke("ArmaSpotifyController", "append_list",
                                string.Format("[\"{0}\",{1},\"{2}\",\"{3}\",\"{4}\",[\"{5}\",{6},{7}],[\"{8}\",{9},{10}]]",
                                    variable,
                                    limit,
                                    "track",
                                    item.track.id,
                                    item.track.album.images[item.track.album.images.FindIndex(a => a.height == 300 && a.width == 300)].url.Remove(0, 18),
                                    title_image[0],
                                    title_image[1],
                                    title_image[2],
                                    artist_image[0],
                                    artist_image[1],
                                    artist_image[2]
                                )
                            );
                        }
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get recent tracks...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get recent tracks...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task GetNewReleases(string variable, int limit = 10)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/browse/new-releases?limit=20&country=" + Variable.client_country);

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.NewReleases.Root data = serializer.Deserialize<Classes.NewReleases.Root>(await response.Content.ReadAsStringAsync());

                    foreach (Classes.NewReleases.Item item in data.albums.items)
                    {
                        List<string> artists_list = new List<string>();
                        foreach (Classes.NewReleases.Artist artist in item.artists)
                        {
                            artists_list.Add(artist.name);
                        };
                        string artists = string.Join(", ", artists_list);

                        // Download text for title + author
                        string[] title_image = Image.Draw(item.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"));
                        string[] artist_image = Image.Draw(artists, new Font("Arial", 14, FontStyle.Regular), ColorTranslator.FromHtml("#828282"), ColorTranslator.FromHtml("#242424"));

                        Master.callback.Invoke("ArmaSpotifyController", "append_list",
                            string.Format("[\"{0}\",{1},\"{2}\",\"{3}\",\"{4}\",[\"{5}\",{6},{7}],[\"{8}\",{9},{10}]]",
                                variable,
                                limit,
                                "album",
                                item.id,
                                item.images[item.images.FindIndex(a => a.height == 300 && a.width == 300)].url.Remove(0, 18),
                                title_image[0],
                                title_image[1],
                                title_image[2],
                                artist_image[0],
                                artist_image[1],
                                artist_image[2]
                            )
                        );
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get new releases data...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get new releases data...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task GetFeaturedPlaylists(string variable, int limit = 10)
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/browse/featured-playlists?country=" + Variable.client_country + "&limit=20");

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.FeaturedPlaylists.Root data = serializer.Deserialize<Classes.FeaturedPlaylists.Root>(await response.Content.ReadAsStringAsync());

                    foreach (Classes.FeaturedPlaylists.Item item in data.playlists.items)
                    {

                        // Download text for title + author
                        string[] title_image = Image.Draw(item.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#242424"), true);
                        string[] artist_image = Image.Draw(item.description, new Font("Arial", 14, FontStyle.Regular), ColorTranslator.FromHtml("#828282"), ColorTranslator.FromHtml("#242424"), true);

                        Master.callback.Invoke("ArmaSpotifyController", "append_list",
                            string.Format("[\"{0}\",{1},\"{2}\",\"{3}\",\"{4}\",[\"{5}\",{6},{7}],[\"{8}\",{9},{10}]]",
                                variable,
                                limit,
                                "playlist",
                                item.id,
                                item.images[0].url.Remove(0, 18),
                                title_image[0],
                                title_image[1],
                                title_image[2],
                                artist_image[0],
                                artist_image[1],
                                artist_image[2]
                            )
                        );
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get featured playlists...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get featured playlists...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task GetUsersPlaylists()
        {
            try
            {
                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/me/playlists?limit=50");

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.UserPlaylists.Root data = serializer.Deserialize<Classes.UserPlaylists.Root>(await response.Content.ReadAsStringAsync());

                    foreach (Classes.UserPlaylists.Item item in data.items)
                    {
                        // Download text for title + author
                        string[] title_image = Image.Draw(item.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#1A1A1A"));

                        Master.callback.Invoke("ArmaSpotifyController", "append_playlist",
                            string.Format("[\"{0}\",{1},{2},\"{3}\",{4},{5}]",
                                title_image[0],
                                title_image[1],
                                title_image[2],
                                item.id,
                                data.total,
                                data.offset
                            )
                        );
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to get users playlists...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to get users playlists...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task ShowNotification(string image_url, string title, string author)
        {
            try
            {
                string image_location = await Image.Download("https://i.scdn.co/" + image_url);
                string[] title_image = Image.Draw(title, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#2B2B2B"));
                string[] artist_image = Image.Draw(author, new Font("Arial", 12, FontStyle.Regular), ColorTranslator.FromHtml("#ABABAB"), ColorTranslator.FromHtml("#2B2B2B"));

                Master.callback.Invoke("ArmaSpotifyController", "show_notification_song",
                    string.Format
                    (
                        "[\"{0}\",[\"{1}\",{2},{3}],[\"{4}\",{5},{6}]]",
                        image_location,
                        title_image[0],
                        title_image[1],
                        title_image[2],
                        artist_image[0],
                        artist_image[1],
                        artist_image[2]
                     )
                );
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to show a notification...");
                Debug.Error(e.ToString());
            }
        }

        internal async static Task RequestInfo()
        {
            try
            {
                Variable.is_playing = false;

                // Refresh token if it has expired
                await RefeshData(false);

                Variable.client.DefaultRequestHeaders.Clear();
                Variable.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Variable.client_access_token);

                var response = await Variable.client.GetAsync("https://api.spotify.com/v1/me/player");

                if (response.IsSuccessStatusCode)
                {
                    string @data = await response.Content.ReadAsStringAsync();

                    if (data.Length > 0)
                    {
                        // Check if this is a private session
                        if (!data.Contains("\"is_private_session\" : true"))
                        {
                            if (!data.Contains("\"item\" : null"))
                            {
                                var serializer = new JavaScriptSerializer();
                                Classes.UserPlayback.Root current_data = serializer.Deserialize<Classes.UserPlayback.Root>(data);

                                // Set device id for resyncing device with Spotify API
                                if (current_data.device.id != null)
                                {
                                    Variable.last_device_id = current_data.device.id;
                                }

                                bool playing = Variable.is_playing = current_data.is_playing;
                                bool nsfw = current_data.item.@explicit;
                                bool shuffle = current_data.shuffle_state;

                                int volume = current_data.device.volume_percent;
                                int length = current_data.item.duration_ms;
                                int progress = current_data.progress_ms;

                                string repeat = current_data.repeat_state;
                                string song = current_data.item.name
                                        .Replace("\"", "\"\"");

                                List<string> artists_list = new List<string>();
                                foreach (Classes.UserPlayback.Artist artist in current_data.item.artists)
                                {
                                    artists_list.Add(artist.name);
                                };
                                string artists = string.Join(", ", artists_list);

                                if (!current_data.item.is_local)
                                {
                                    string song_id = current_data.item.id;

                                    string image = "";
                                    foreach (Classes.UserPlayback.Image image_item in current_data.item.album.images)
                                    {
                                        if (image_item.height == 64 && image_item.width == 64)
                                        {
                                            image = image_item.url.Remove(0, 18);
                                        }
                                    };

                                    await Image.Download(@"https://i.scdn.co/" + image);

                                    string callback_data = string.Format("[{0},{1},{2},{3},\"{4}\",{5},{6},\"{7}\",\"{8}\",{9}]", playing, nsfw, length, progress, image, volume, shuffle, repeat, song_id, false);
                                    Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_display", callback_data);

                                    // Download text for title + author
                                    string[] title_image = Image.Draw(current_data.item.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#2B2B2B"));
                                    string[] artist_image = Image.Draw(artists, new Font("Arial", 12, FontStyle.Regular), ColorTranslator.FromHtml("#ABABAB"), ColorTranslator.FromHtml("#2B2B2B"));

                                    Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_song_info",
                                        string.Format
                                        (
                                            "[[\"{0}\",{1},{2}],[\"{3}\",{4},{5}]]",
                                            title_image[0],
                                            title_image[1],
                                            title_image[2],
                                            artist_image[0],
                                            artist_image[1],
                                            artist_image[2]
                                         )
                                    );

                                    if (current_data.is_playing && Variable.last_song_id != current_data.item.id)
                                    {
                                        await ShowNotification(image, current_data.item.name, artists);
                                        Variable.last_song_id = current_data.item.id;
                                    }
                                }
                                else
                                {
                                    // This is a local file - ignore images and stuff like that

                                    string callback_data = string.Format("[{0},{1},{2},{3},\"{4}\",{5},{6},\"{7}\",\"{8}\",{9}]", playing, nsfw, length, progress, "", volume, shuffle, repeat, "", true);
                                    Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_display", callback_data);

                                    // Download text for title + author
                                    string[] title_image = Image.Draw(current_data.item.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#2B2B2B"));
                                    string[] artist_image = Image.Draw(artists, new Font("Arial", 12, FontStyle.Regular), ColorTranslator.FromHtml("#ABABAB"), ColorTranslator.FromHtml("#2B2B2B"));

                                    Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_song_info",
                                        string.Format
                                        (
                                            "[[\"{0}\",{1},{2}],[\"{3}\",{4},{5}]]",
                                            title_image[0],
                                            title_image[1],
                                            title_image[2],
                                            artist_image[0],
                                            artist_image[1],
                                            artist_image[2]
                                         )
                                    );

                                    if (current_data.is_playing && Variable.last_song_id != current_data.item.id)
                                    {
                                        await ShowNotification("", current_data.item.name, artists);
                                        Variable.last_song_id = current_data.item.id;
                                    }
                                }
                            }
                            else
                            {
                                // No song is playing
                                var serializer = new JavaScriptSerializer();
                                Classes.NoSong.Root current_data = serializer.Deserialize<Classes.NoSong.Root>(data);

                                bool playing = current_data.is_playing;
                                bool nsfw = false;
                                bool shuffle = current_data.shuffle_state;

                                int volume = current_data.device.volume_percent;
                                int length = 0;
                                int progress = 0;

                                string image = "";
                                string repeat = current_data.repeat_state;
                                string song_id = "";


                                string callback_data = string.Format("[{0},{1},{2},{3},\"{4}\",{5},{6},\"{7}\",\"{8}\"]", playing, nsfw, length, progress, image, volume, shuffle, repeat, song_id);
                                Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_display", callback_data);

                                Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_song_info", "[[],[]]");
                            }
                        }
                        else
                        {
                            // Private session found
                            Master.callback.Invoke("ArmaSpotifyController", "private_session", "");
                        }
                    }
                    else
                    {
                        // No devices found
                        Master.callback.Invoke("ArmaSpotifyController", "device_required", "");
                    }
                }
                else
                {
                    Debug.Info("An error was returned when attempting to request user info...");
                    Debug.Error(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Debug.Info("An error has occured when attempting to request user info...");
                Debug.Error(e.ToString());
            }
        }

    }

    class Classes
    {
        internal class LikedSong
        {
            public class Root
            {
                public List<bool> liked { get; set; }
            }
        }

        internal class LikedTrack
        {
            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Artist
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Image
            {
                public int height { get; set; }
                public string url { get; set; }
                public int width { get; set; }
            }

            public class Album
            {
                public string album_type { get; set; }
                public List<Artist> artists { get; set; }
                public List<string> available_markets { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<Image> images { get; set; }
                public string name { get; set; }
                public string release_date { get; set; }
                public string release_date_precision { get; set; }
                public int total_tracks { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class ExternalIds
            {
                public string isrc { get; set; }
            }

            public class Track
            {
                public Album album { get; set; }
                public List<Artist> artists { get; set; }
                public List<string> available_markets { get; set; }
                public int disc_number { get; set; }
                public int duration_ms { get; set; }
                public bool @explicit { get; set; }
                public ExternalIds external_ids { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public bool is_local { get; set; }
                public string name { get; set; }
                public int popularity { get; set; }
                public string preview_url { get; set; }
                public int track_number { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Item
            {
                public DateTime added_at { get; set; }
                public Track track { get; set; }
            }

            public class Root
            {
                public string href { get; set; }
                public List<Item> items { get; set; }
                public int limit { get; set; }
                public string next { get; set; }
                public int offset { get; set; }
                public object previous { get; set; }
                public int total { get; set; }
            }
        }

        internal class UserInfo
        {
            public class ExplicitContent
            {
                public bool filter_enabled { get; set; }
                public bool filter_locked { get; set; }
            }

            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Followers
            {
                public object href { get; set; }
                public int total { get; set; }
            }

            public class Root
            {
                public string country { get; set; }
                public string display_name { get; set; }
                public ExplicitContent explicit_content { get; set; }
                public ExternalUrls external_urls { get; set; }
                public Followers followers { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<object> images { get; set; }
                public string product { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }
        }

        internal class Devices
        {
            public class Device
            {
                public string id { get; set; }
                public bool is_active { get; set; }
                public bool is_private_session { get; set; }
                public bool is_restricted { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public int volume_percent { get; set; }
            }

            public class Root
            {
                public List<Device> devices { get; set; }
            }
        }

        internal class UserPlayback
        {
            public class Device
            {
                public string id { get; set; }
                public bool is_active { get; set; }
                public bool is_private_session { get; set; }
                public bool is_restricted { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public int volume_percent { get; set; }
            }

            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Artist
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Image
            {
                public int height { get; set; }
                public string url { get; set; }
                public int width { get; set; }
            }

            public class Album
            {
                public string album_type { get; set; }
                public List<Artist> artists { get; set; }
                public List<string> available_markets { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<Image> images { get; set; }
                public string name { get; set; }
                public string release_date { get; set; }
                public string release_date_precision { get; set; }
                public int total_tracks { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class ExternalIds
            {
                public string isrc { get; set; }
            }

            public class Item
            {
                public Album album { get; set; }
                public List<Artist> artists { get; set; }
                public List<object> available_markets { get; set; }
                public int disc_number { get; set; }
                public int duration_ms { get; set; }
                public bool @explicit { get; set; }
                public ExternalIds external_ids { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public bool is_local { get; set; }
                public bool is_playable { get; set; }
                public string name { get; set; }
                public int popularity { get; set; }
                public string preview_url { get; set; }
                public int track_number { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Disallows
            {
                public bool pausing { get; set; }
            }

            public class Actions
            {
                public Disallows disallows { get; set; }
            }

            public class Root
            {
                public Device device { get; set; }
                public bool shuffle_state { get; set; }
                public string repeat_state { get; set; }
                public long timestamp { get; set; }
                public object context { get; set; }
                public int progress_ms { get; set; }
                public Item item { get; set; }
                public string currently_playing_type { get; set; }
                public Actions actions { get; set; }
                public bool is_playing { get; set; }
            }
        }

        internal class FeaturedPlaylists
        {
            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Image
            {
                public object height { get; set; }
                public string url { get; set; }
                public object width { get; set; }
            }

            public class Owner
            {
                public string display_name { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Tracks
            {
                public string href { get; set; }
                public int total { get; set; }
            }

            public class Item
            {
                public bool collaborative { get; set; }
                public string description { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<Image> images { get; set; }
                public string name { get; set; }
                public Owner owner { get; set; }
                public object primary_color { get; set; }
                public object @public { get; set; }
                public string snapshot_id { get; set; }
                public Tracks tracks { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Playlists
            {
                public string href { get; set; }
                public List<Item> items { get; set; }
                public int limit { get; set; }
                public object next { get; set; }
                public int offset { get; set; }
                public object previous { get; set; }
                public int total { get; set; }
            }

            public class Root
            {
                public string message { get; set; }
                public Playlists playlists { get; set; }
            }
        }

        internal class RefreshToken
        {
            public class Root
            {
                public string access_token { get; set; }
                public string token_type { get; set; }
                public int expires_in { get; set; }
                public string refresh_token { get; set; }
                public string scope { get; set; }
            }
        }

        internal class AccessToken
        {
            public class Root
            {
                public string access_token { get; set; }
                public string token_type { get; set; }
                public int expires_in { get; set; }
                public string refresh_token { get; set; }
                public string scope { get; set; }
            }
        }

        internal class UserPlaylists
        {
            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Image
            {
                public object height { get; set; }
                public string url { get; set; }
                public object width { get; set; }
            }

            public class Owner
            {
                public string display_name { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Tracks
            {
                public string href { get; set; }
                public int total { get; set; }
            }

            public class Item
            {
                public bool collaborative { get; set; }
                public string description { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<Image> images { get; set; }
                public string name { get; set; }
                public Owner owner { get; set; }
                public object primary_color { get; set; }
                public bool @public { get; set; }
                public string snapshot_id { get; set; }
                public Tracks tracks { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Root
            {
                public string href { get; set; }
                public List<Item> items { get; set; }
                public int limit { get; set; }
                public object next { get; set; }
                public int offset { get; set; }
                public object previous { get; set; }
                public int total { get; set; }
            }
        }

        internal class NewReleases
        {
            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Artist
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Image
            {
                public int height { get; set; }
                public string url { get; set; }
                public int width { get; set; }
            }

            public class Item
            {
                public string album_type { get; set; }
                public List<Artist> artists { get; set; }
                public List<string> available_markets { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<Image> images { get; set; }
                public string name { get; set; }
                public string release_date { get; set; }
                public string release_date_precision { get; set; }
                public int total_tracks { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Albums
            {
                public string href { get; set; }
                public List<Item> items { get; set; }
                public int limit { get; set; }
                public string next { get; set; }
                public int offset { get; set; }
                public object previous { get; set; }
                public int total { get; set; }
            }

            public class Root
            {
                public Albums albums { get; set; }
            }
        }

        internal class AlbumItems
        {
            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Artist
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Item
            {
                public List<Artist> artists { get; set; }
                public int disc_number { get; set; }
                public int duration_ms { get; set; }
                public bool @explicit { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public bool is_local { get; set; }
                public bool is_playable { get; set; }
                public string name { get; set; }
                public string preview_url { get; set; }
                public int track_number { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Root
            {
                public string href { get; set; }
                public List<Item> items { get; set; }
                public int limit { get; set; }
                public string next { get; set; }
                public int offset { get; set; }
                public object previous { get; set; }
                public int total { get; set; }
            }
        }

        internal class AlbumInfo
        {
            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Artist
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Copyright
            {
                public string text { get; set; }
                public string type { get; set; }
            }

            public class ExternalIds
            {
                public string upc { get; set; }
            }

            public class Image
            {
                public int height { get; set; }
                public string url { get; set; }
                public int width { get; set; }
            }

            public class Item
            {
                public List<Artist> artists { get; set; }
                public List<object> available_markets { get; set; }
                public int disc_number { get; set; }
                public int duration_ms { get; set; }
                public bool @explicit { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public bool is_local { get; set; }
                public string name { get; set; }
                public object preview_url { get; set; }
                public int track_number { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Tracks
            {
                public string href { get; set; }
                public List<Item> items { get; set; }
                public int limit { get; set; }
                public object next { get; set; }
                public int offset { get; set; }
                public object previous { get; set; }
                public int total { get; set; }
            }

            public class Root
            {
                public string album_type { get; set; }
                public List<Artist> artists { get; set; }
                public List<object> available_markets { get; set; }
                public List<Copyright> copyrights { get; set; }
                public ExternalIds external_ids { get; set; }
                public ExternalUrls external_urls { get; set; }
                public List<object> genres { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<Image> images { get; set; }
                public string label { get; set; }
                public string name { get; set; }
                public int popularity { get; set; }
                public string release_date { get; set; }
                public string release_date_precision { get; set; }
                public int total_tracks { get; set; }
                public Tracks tracks { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }
        }


        internal class PlaylistItems
        {
            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class AddedBy
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Artist
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Image
            {
                public int height { get; set; }
                public string url { get; set; }
                public int width { get; set; }
            }

            public class Album
            {
                public string album_type { get; set; }
                public List<Artist> artists { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<Image> images { get; set; }
                public string name { get; set; }
                public string release_date { get; set; }
                public string release_date_precision { get; set; }
                public int total_tracks { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class ExternalIds
            {
                public string isrc { get; set; }
            }

            public class LinkedFrom
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Track
            {
                public Album album { get; set; }
                public List<Artist> artists { get; set; }
                public int disc_number { get; set; }
                public int duration_ms { get; set; }
                public bool episode { get; set; }
                public bool @explicit { get; set; }
                public ExternalIds external_ids { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public bool is_local { get; set; }
                public bool is_playable { get; set; }
                public LinkedFrom linked_from { get; set; }
                public string name { get; set; }
                public int popularity { get; set; }
                public string preview_url { get; set; }
                public bool track { get; set; }
                public int track_number { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class VideoThumbnail
            {
                public object url { get; set; }
            }

            public class Item
            {
                public DateTime added_at { get; set; }
                public AddedBy added_by { get; set; }
                public bool is_local { get; set; }
                public object primary_color { get; set; }
                public Track track { get; set; }
                public VideoThumbnail video_thumbnail { get; set; }
            }

            public class Root
            {
                public string href { get; set; }
                public List<Item> items { get; set; }
                public int limit { get; set; }
                public string next { get; set; }
                public int offset { get; set; }
                public object previous { get; set; }
                public int total { get; set; }
            }
        }

        internal class PlaylistInfo
        {
            public class Image
            {
                public object height { get; set; }
                public string url { get; set; }
                public object width { get; set; }
            }

            public class Root
            {
                public string description { get; set; }
                public List<Image> images { get; set; }
                public string name { get; set; }
            }
        }

        internal class RecentTracks
        {
            public class ExternalUrls
            {
                public string spotify { get; set; }
            }

            public class Artist
            {
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Image
            {
                public int height { get; set; }
                public string url { get; set; }
                public int width { get; set; }
            }

            public class Album
            {
                public string album_type { get; set; }
                public List<Artist> artists { get; set; }
                public List<string> available_markets { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public List<Image> images { get; set; }
                public string name { get; set; }
                public string release_date { get; set; }
                public string release_date_precision { get; set; }
                public int total_tracks { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class ExternalIds
            {
                public string isrc { get; set; }
            }

            public class Track
            {
                public Album album { get; set; }
                public List<Artist> artists { get; set; }
                public List<object> available_markets { get; set; }
                public int disc_number { get; set; }
                public int duration_ms { get; set; }
                public bool @explicit { get; set; }
                public ExternalIds external_ids { get; set; }
                public ExternalUrls external_urls { get; set; }
                public string href { get; set; }
                public string id { get; set; }
                public bool is_local { get; set; }
                public bool is_playable { get; set; }
                public string name { get; set; }
                public int popularity { get; set; }
                public string preview_url { get; set; }
                public int track_number { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }

            public class Item
            {
                public Track track { get; set; }
                public DateTime played_at { get; set; }
                public object context { get; set; }
            }

            public class Cursors
            {
                public string after { get; set; }
                public string before { get; set; }
            }

            public class Root
            {
                public List<Item> items { get; set; }
                public string next { get; set; }
                public Cursors cursors { get; set; }
                public int limit { get; set; }
                public string href { get; set; }
            }
        }

        internal class NoSong
        {
            public class Device
            {
                public string id { get; set; }
                public bool is_active { get; set; }
                public bool is_private_session { get; set; }
                public bool is_restricted { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public int volume_percent { get; set; }
            }

            public class Disallows
            {
                public bool resuming { get; set; }
                public bool seeking { get; set; }
                public bool skipping_prev { get; set; }
                public bool skipping_next { get; set; }
            }

            public class Actions
            {
                public Disallows disallows { get; set; }
            }

            public class Root
            {
                public Device device { get; set; }
                public bool shuffle_state { get; set; }
                public string repeat_state { get; set; }
                public int timestamp { get; set; }
                public object context { get; set; }
                public int progress_ms { get; set; }
                public object item { get; set; }
                public string currently_playing_type { get; set; }
                public Actions actions { get; set; }
                public bool is_playing { get; set; }
            }
        }

        internal class Dealers
        {
            public class Root
            {
                public List<string> dealer { get; set; }
                public List<string> spclient { get; set; }
            }
        }

        internal class ListnerData
        {
            public class Headers
            {
                public string SpotifyConnectionId { get; set; }
            }

            public class Root
            {
                public Headers headers { get; set; }
                public string method { get; set; }
                public string type { get; set; }
                public string uri { get; set; }
            }
        }

        internal class WebToken
        {
            public class Root
            {
                public string clientId { get; set; }
                public string accessToken { get; set; }
                public long accessTokenExpirationTimestampMs { get; set; }
                public bool isAnonymous { get; set; }
            }
        }
    }
}