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

namespace ArmaSpotifyController
{
    public class Image
    {
        // Image directory
        internal static string data_directory;

        internal static void Setup()
        {
            // Set file directory
            data_directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\data\";
            Directory.CreateDirectory(data_directory);

            // Log the directorys
            Log.Message("Image directory: " + data_directory);

            // Delete files in the data directory that are longer then three days old
            string[] files = Directory.GetFiles(data_directory);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                if (info.LastAccessTime < DateTime.Now.AddDays(-3))
                {
                    info.Delete();
                }
            }
        }

        internal static async Task DownloadImage(string uri_string, string variable = "")
        {
            try
            {
                // Don't try to download an empty string
                if (uri_string == "")
                    return;

                // Create new URI from string input
                Uri uri = new Uri(uri_string);

                // Get filename from Spotify web server                
                string filename = Path.GetFileNameWithoutExtension(uri.LocalPath);

                // Create file path along with new extension
                var path = Path.Combine(data_directory, filename + ".jpg");

                if (!File.Exists(path.ToString()))
                {
                    // Download the image and write to the file
                    var imageBytes = await Master.client.GetByteArrayAsync(uri);
                    File.WriteAllBytes(path, imageBytes);

                    Log.Message("Downloaded file: " + path);
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

        internal static string[] DrawText(String text, Font font, Color textColor, Color backColor)
        {
            // Get filename
            byte[] sha_bytes = SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            String filename = Convert.ToBase64String(sha_bytes).Split('=')[0].Replace('+', '-').Replace('/', '_');

            // Check if file already exists, if it does then don't bother creating another one
            if (File.Exists(data_directory + filename + ".jpg"))
            {
                System.Drawing.Image ext_img = System.Drawing.Image.FromFile(data_directory + filename + ".jpg");
                return new string[] { data_directory + filename + ".jpg", ext_img.Width.ToString(), ext_img.Height.ToString() };
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
            img.Save(data_directory + filename + ".jpg", ImageFormat.Jpeg);

            // Send data in return
            return new string[] { data_directory + filename + ".jpg", textSize.Width.ToString(), textSize.Height.ToString() };
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
        
        internal async static void Setup()
        {
            // Set file directory
            log_directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\logs";
            log_file = log_directory + @"\ArmaSpotifyController_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";
            Directory.CreateDirectory(log_directory);

            // Log the image directory
            Log.Message("Log directory: " + log_directory);

            // Delete files in the log directory that are longer then three days old
            string[] files = Directory.GetFiles(log_directory);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                if (info.LastAccessTime < DateTime.Now.AddDays(-3))
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
                await Request.RefeshData(true);
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

    public class Request
    {
        static readonly int refresh_delay = 250;
        static string last_device_id;

        internal async static Task RefeshData(bool save_info = false)
        {
            try
            {
                if (DateTime.Now < Master.client_refresh_time || Master.client_refresh_token == null)
                    return;

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
                    var serializer = new JavaScriptSerializer();
                    Classes.RefreshToken.Root result = serializer.Deserialize<Classes.RefreshToken.Root>(await response.Content.ReadAsStringAsync());

                    Master.client_access_token = result.access_token;
                    Master.client_refresh_token = result.refresh_token;

                    // Delete old token file
                    Log.DeleteToken();

                    // Save new refresh token
                    Log.SaveToken();

                    // Set the refresh timer
                    Master.client_refresh_time = DateTime.Now.AddSeconds(3000);

                    Log.Message("Users refresh token has been updated successfully");

                    if (save_info)
                    {
                        Log.Message("Starting user info task request");

                        await Request.GetUserInfo();
                    }
                }
                else
                {
                    Log.Message("'RefeshData' error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'RefeshData' exception: " + e.Message);
            }
        }

        internal async static Task GetUserInfo()
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);
                var response = await Master.client.GetAsync("https://api.spotify.com/v1/me");

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new JavaScriptSerializer();
                    Classes.UserInfo.Root result = serializer.Deserialize<Classes.UserInfo.Root>(await response.Content.ReadAsStringAsync());

                    Master.client_premium = result.product == "premium";
                    Master.client_nsfw = !result.explicit_content.filter_enabled;   
                    Master.client_country = result.country;

                    // Log messages to allow debugging
                    Log.Message("=====================");
                    Log.Message("Premium: " + Master.client_premium.ToString().ToLower());
                    Log.Message("Show NSFW: " + Master.client_nsfw.ToString().ToLower());
                    Log.Message("Country: " + Master.client_country);
                    Log.Message("=====================");

                    // Callback to game to let it know user info has been saved
                    Master.callback.Invoke("ArmaSpotifyController","setVariable","[\"missionnamespace\", \"aasp_info_saved\", true, false]");
                }
                else
                {
                    Log.Message("'GetUserInfo' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'GetUserInfo' exception: " + e.Message);
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
                    Log.Message("'GetUserDevices' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'GetUserDevices' exception: " + e.Message);
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
                    last_device_id = device_id;
                    await Task.Delay(refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    Log.Message("'SetUserDevice' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'SetUserDevice' exception: " + e.Message);
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
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (last_device_id != null)
                        {
                            Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                            await SetUserDevice(last_device_id);
                        }
                    }
                    Log.Message("'SetUserVolume' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'SetUserVolume' exception: " + e.Message);
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
                    await Task.Delay(refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (last_device_id != null)
                        {
                            Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                            await SetUserDevice(last_device_id);
                        }
                    }
                    Log.Message("'PausePlayback' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'PausePlayback' exception: " + e.Message);
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
                    await Task.Delay(refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (last_device_id != null)
                        {
                            Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                            await SetUserDevice(last_device_id);
                        }
                    }
                    Log.Message("'ResumePlayback' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'ResumePlayback' exception: " + e.Message);
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
                    await Task.Delay(refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (last_device_id != null)
                        {
                            Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                            await SetUserDevice(last_device_id);
                        }
                    }
                    Log.Message("'SkipNext' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'SkipNext' exception: " + e.Message);
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
                    await Task.Delay(refresh_delay);
                    await RequestInfo();
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (last_device_id != null)
                        {
                            Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                            await SetUserDevice(last_device_id);
                        }
                    }
                    Log.Message("'SkipBack' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'SkipBack' exception: " + e.Message);
            }
        }

        internal async static Task Seek(int position_ms)
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.PutAsync("https://api.spotify.com/v1/me/player/seek?position_ms=" + position_ms, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Seeked to: " + position_ms);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (last_device_id != null)
                        {
                            Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                            await SetUserDevice(last_device_id);
                        }
                    }
                    Log.Message("'Seek' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'Seek' exception: " + e.Message);
            }
        }

        internal async static Task Repeat(string mode)
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.PutAsync("https://api.spotify.com/v1/me/player/repeat?state=" + mode, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Repeat mode set to: " + mode);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (last_device_id != null)
                        {
                            Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                            await SetUserDevice(last_device_id);
                        }
                    }
                    Log.Message("'Repeat' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'Repeat' exception: " + e.Message);
            }
        }

        internal async static Task Shuffle(bool mode)
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.PutAsync("https://api.spotify.com/v1/me/player/shuffle?state=" + mode, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    Log.Message("Shuffle mode: " + mode.ToString());
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (last_device_id != null)
                        {
                            Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                            await SetUserDevice(last_device_id);
                        }
                    }
                    Log.Message("'Shuffle' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'Shuffle' exception: " + e.Message);
            }
        }

        internal async static Task SongLiked(string song_id)
        {
            try
            {
                if (song_id != "")
                {
                    Master.client.DefaultRequestHeaders.Clear();
                    Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                    var response = await Master.client.GetAsync("https://api.spotify.com/v1/me/tracks/contains?ids=" + song_id);

                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_like", string.Format("[{0},\"{1}\"]", data.Contains("true"), song_id));
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (last_device_id != null)
                            {
                                Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                                await SetUserDevice(last_device_id);
                            }
                        }
                        Log.Message("'SongLiked' Error: " + response.StatusCode.ToString());
                        Log.Message(response.ReasonPhrase);
                    }
                }
                else
                {
                    Log.Message("Attempted to like song, but no song id was found");
                }
            }
            catch (Exception e)
            {
                Log.Message("'SongLiked' exception: " + e.Message);
            }
        }

        internal async static Task LikeSong(string song_id)
        {
            try
            {
                if (song_id != "")
                {
                    Master.client.DefaultRequestHeaders.Clear();
                    Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                    var response = await Master.client.PutAsync("https://api.spotify.com/v1/me/tracks?ids=" + song_id, new StringContent(""));

                    if (response.IsSuccessStatusCode)
                    {
                        Log.Message("Liked song: " + song_id);
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (last_device_id != null)
                            {
                                Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                                await SetUserDevice(last_device_id);
                            }
                        }
                        Log.Message("'LikeSong' Error: " + response.StatusCode.ToString());
                        Log.Message(response.ReasonPhrase);
                    }
                }
                else
                {
                    Log.Message("Attempted to like song, but no song id was found");
                }
            }
            catch (Exception e)
            {
                Log.Message("'LikeSong' exception: " + e.Message);
            }
        }

        internal async static Task UnlikeSong(string song_id)
        {
            try
            {
                if (song_id != "")
                {
                    Master.client.DefaultRequestHeaders.Clear();
                    Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                    var response = await Master.client.DeleteAsync("https://api.spotify.com/v1/me/tracks?ids=" + song_id);

                    if (response.IsSuccessStatusCode)
                    {
                        Log.Message("Unliked song: " + song_id);
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            if (last_device_id != null)
                            {
                                Log.Message("Spotify has disconnected the player from the API due to inactivity. Attempting reconnection");
                                await SetUserDevice(last_device_id);
                            }
                        }
                        Log.Message("'UnlikeSong' Error: " + response.StatusCode.ToString());
                        Log.Message(response.ReasonPhrase);
                    }
                }
                else
                {
                    Log.Message("Attempted to unlike song, but no song id was found");
                }
            }
            catch (Exception e)
            {
                Log.Message("'UnlikeSong' exception: " + e.Message);
            }
        }

        internal async static Task RequestInfo()
        {
            try
            {
                Master.client.DefaultRequestHeaders.Clear();
                Master.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Master.client_access_token);

                var response = await Master.client.GetAsync("https://api.spotify.com/v1/me/player");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();

                    if (data.Length > 0)
                    {
                        var serializer = new JavaScriptSerializer();
                        Classes.UserPlayback.Root current_data = serializer.Deserialize<Classes.UserPlayback.Root>(data);

                        bool playing = current_data.is_playing;
                        bool nsfw = current_data.item.@explicit;
                        bool shuffle = current_data.shuffle_state;

                        int volume = current_data.device.volume_percent;
                        int length = current_data.item.duration_ms;
                        int progress = current_data.progress_ms;

                        string song_id = current_data.item.id;
                        string repeat = current_data.repeat_state;
                        string song = current_data.item.name
                            .Replace('’', '\'');

                        List<string> artists_list = new List<string>();
                        foreach (Classes.UserPlayback.Artist artist in current_data.item.artists)
                        {
                            artists_list.Add(artist.name);
                        };
                        string artists = string.Join(", ", artists_list);

                        string image = "";
                        foreach (Classes.UserPlayback.Image image_item in current_data.item.album.images)
                        {
                            if (image_item.height == 64 && image_item.width == 64)
                            {
                                image = image_item.url.Remove(0, 18);
                            }
                        };

                        string callback_data = string.Format("[{0},{1},{2},{3},\"{4}\",\"{5}\",\"{6}\",{7},{8},\"{9}\",\"{10}\"]", playing, nsfw, length, progress, song, artists, image, volume, shuffle, repeat, song_id);
                        Master.callback.Invoke("ArmaSpotifyController", "spotify_fnc_update_display", callback_data);

                        // Download text for title + author
                        string[] title_image = Image.DrawText(current_data.item.name, new Font("Arial", 19, FontStyle.Bold), Color.White, ColorTranslator.FromHtml("#2B2B2B"));
                        string[] artist_image = Image.DrawText(artists, new Font("Arial", 12, FontStyle.Regular), ColorTranslator.FromHtml("#ABABAB"), ColorTranslator.FromHtml("#2B2B2B"));

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
                    }
                    else
                    {
                        Master.callback.Invoke("ArmaSpotifyController", "device_required", "");
                    }
                }
                else
                {
                    Log.Message("'RequestInfo' Error: " + response.StatusCode.ToString());
                    Log.Message(response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                Log.Message("'RequestInfo' exception: " + e.Message);
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
        internal static DateTime client_refresh_time;

        // Client information
        internal static bool client_premium;
        internal static bool client_nsfw;
        internal static string client_country;

        // HttpClient for sending/receiving data from Spotify
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
            callback.Invoke("ArmaSpotifyController", "setVariable", "[\"missionnamespace\", \"aasp_image_location\", \""+ Image.data_directory + @"\" +"\", false]");

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
                            await Request.GetUserDevices(parameters[2]);
                            output.Append("true");
                            break;

                        // SET_DEVICE: Transfer audio playback to requested device
                        case "set_device":
                            await Request.SetUserDevice(parameters[2]);
                            output.Append("true");
                            break;

                        // SET_VOLUME: Sets users volume for the current active device
                        case "set_volume":
                            try
                            {
                                await Request.SetUserVolume(int.Parse(parameters[2]));
                                output.Append("true");
                            }
                            catch (Exception e)
                            {
                                Log.Message("SetVolume Request Error: " + e.Message);
                                Log.Message(parameters[2]);
                                output.Append("false");
                            }
                            break;

                        // SKIP: Skips forward/backwards to the next item in playlist
                        case "skip":
                            if (parameters[2] == "next")
                                await Request.SkipNext();
                            else
                                await Request.SkipBack();
                            output.Append("true");
                            break;

                        // PLAY: Starts/resumes users playback of the current song/defined song
                        case "play":
                            await Request.ResumePlayback();
                            output.Append("true");
                            break;

                        // PAUSE: Pauses users playback of the current song
                        case "pause":
                            await Request.PausePlayback();
                            output.Append("true");
                            break;

                        // REPEAT: Set repeat state of player
                        case "repeat":
                            await Request.Repeat(parameters[2]);
                            output.Append("true");
                            break;

                        // SHUFFLE: Set shuffle state of player
                        case "shuffle":
                            await Request.Shuffle(parameters[2] == "true");
                            output.Append("true");
                            break;
                        
                        // LIKE: Like a song by ID
                        case "like":
                            await Request.LikeSong(parameters[2]);
                            output.Append("true");
                            break;

                        // UNLIKE: Unlike a song by ID
                        case "unlike":
                            await Request.UnlikeSong(parameters[2]);
                            output.Append("true");
                            break;

                        // LIKED: Check if song ID is liked by user
                        case "liked":
                            await Request.SongLiked(parameters[2]);
                            output.Append("true");
                            break;

                        // SEEK: Seek to position in current playing track
                        case "seek":
                            try
                            {
                                await Request.Seek(int.Parse(parameters[2]));
                                output.Append("true");
                            }
                            catch (Exception e)
                            {
                                Log.Message("Seek Request Error: " + e.Message);
                                Log.Message(parameters[2]);
                                output.Append("false");
                            }
                            break;

                        // CONNECT_WEBSITE: Open the Spotify Connect website
                        case "connect_website":
                            Process.Start("https://www.spotify.com/connect/");
                            output.Append("true");
                            break;

                        // DOWNLOAD_IMAGE: Downloads image from spotify server and then sets it to a control
                        case "download_image":
                            await Image.DownloadImage(@"https://i.scdn.co/" + parameters[2], parameters[3]);
                            output.Append("true");
                            break;

                        // REFRESH_TOKEN: Sends a request to refresh the users tokens
                        case "refresh_token":
                            await Request.RefeshData(false);
                            output.Append("true");
                            break;

                        // REQUEST_INFO: Request updated info from Spotify
                        case "request_info":
                            await Request.RequestInfo();
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
                                    var serializer = new JavaScriptSerializer();
                                    Classes.AccessToken.Root result = serializer.Deserialize<Classes.AccessToken.Root>(await response.Content.ReadAsStringAsync());

                                    client_access_token = result.access_token;
                                    client_refresh_token = result.refresh_token;

                                    // Delete old token file
                                    Log.DeleteToken();

                                    // Save new refresh token
                                    Log.SaveToken();

                                    // Set the refresh timer
                                    client_refresh_time = DateTime.Now.AddSeconds(3000);

                                    // Callback to game to let it know user is authorised
                                    callback.Invoke("ArmaSpotifyController", "setVariable", "[\"missionnamespace\", \"aasp_authorised\", true, false]");

                                    // Save user info to client variables in DLL for later use
                                    await Request.GetUserInfo();

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
                            "user-modify-playback-state",
                            "user-library-modify",
                            "user-library-read"
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
                        output.Append(Image.data_directory);
                        break;

                    // DEFAULT: Show version information
                    default:
                        output.Append(version_info);
                        break;
                }
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
    }
}