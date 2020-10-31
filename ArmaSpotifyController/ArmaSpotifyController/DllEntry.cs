using RGiesecke.DllExport;
using Newtonsoft.Json.Linq;
using System;
using System.Management;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Net;
using System.Net.Http;
using System.IO;

namespace ArmaSpotifyController
{
    public class DllEntry
    {
        // Predefined version information + author name
        static readonly string version_info = "ASJ DLL - VER 0.0.1 - Asaayu";
        static readonly string app_client_id = "8cf42361877d48de877925a8c1dc747d";

        static string client_refresh_token;
        static string client_access_token;

        static string verifier_string;
        static string async_response;
        static string id;

        static bool connected = false;

        private const int Keysize = 256;
        private const int DerivationIterations = 1000;

        private static readonly HttpClient client = new HttpClient();

        static int state;
        static readonly char[] padding = { '=' };

        public async static void RequestData(string post_url, FormUrlEncodedContent content)
        {
            // Send request + content in POST 
            var response = await client.PostAsync(post_url, content);

            if (response.IsSuccessStatusCode)
            {
                // Save response to variable for later use
                async_response = await response.Content.ReadAsStringAsync();
                connected = true;
            }
            else
            {
                // Empty response due to non-success
                async_response = "";
            }            
        }

        public static bool spotify_connected()
        {
            return !(client_refresh_token == "" || client_access_token == "");
        }

        public async static void RefreshToken(string refresh)
        {
            // POST URL
            String post_url = "https://accounts.spotify.com/api/token";
            // Setup POST data
            var values = new Dictionary<string, string>
            {                
                { "grant_type", "refresh_token" },
                { "refresh_token", refresh },
                { "client_id", app_client_id }
            };
            var content = new FormUrlEncodedContent(values);

            // Send async post request to server
            RequestData(post_url, content);

            System.Threading.Thread.Sleep(2000);

            if (async_response != "")
            {
                JObject response = JObject.Parse(async_response);
                client_refresh_token = response.GetValue("refresh_token").ToString();
                client_access_token = response.GetValue("access_token").ToString();
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

        static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");
            if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
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
            id = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(id));

            // Auto output the version information to the report file
            output.Append(version_info);
        }

        // Do not remove these six lines
#if WIN64
        [DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function)
        {
            // Reduce output by 1 to avoid accidental overflow
            outputSize--;

            // Check if the function starts with the word "Spotify"
            if (function.Length >= 7 && function.ToLower().Substring(0,7) == "spotify")
            {
                // Split on the spacers, in this case ":"
                String[] parameters = function.Split(':');
                
                if (parameters.Length > 1)
                {
                    switch (parameters[1].ToLower())
                    {

                    }
                }
                else
                {
                    // Only the word spotify was passed through. Error?
                    output.Append("Reading you loud and clear! o7");
                };                    
            }
            else
            {
                String[] parameters = function.Split(':');

                // Switch through all the other options
                switch (function.ToLower())
                {
                    // REFRESH: Get refresh token and output encrypted string
                    case "refresh":
                        // Make sure the refresh token is defined
                        if (client_refresh_token == "" && parameters.Length > 1)
                        {
                            client_refresh_token = Encrypt(parameters[1], id);
                        }
                        else
                        {
                            if (client_refresh_token == "")
                            {
                                output.Append("ERROR: No client refresh token was found. Refresh rejected");
                                break;
                            }



                        }
                        break;

                    // GET_REFRESH: Get refresh token and output encrypted string
                    case "get_refresh":
                        if (client_refresh_token != "")
                        {
                            output.Append(Encrypt(client_refresh_token, id));
                        }
                        else
                        {
                            output.Append("");
                        }
                        break;

                    // AUTHORISE: Get async response and save
                    case "authorise":
                        if (async_response != "")
                        {
                            JObject response = JObject.Parse(async_response);
                            client_refresh_token = response.GetValue("refresh_token").ToString();
                            client_access_token = response.GetValue("access_token").ToString();
                        }
                        else
                        {
                            output.Append("ERROR: Async response returned with non-success status code");
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

                                // Send async post request to server
                                RequestData(post_url, content);

                                output.Append("async");
                            }
                            else
                            {
                                // Missing state key
                                output.Append("ERROR: Incorrect 'state' key. Authorization has been rejected.");
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
                        verifier_string = RandomString(110);

                        byte[] sha_bytes;
                        // Create code challange
                        using (SHA256 hash = SHA256Managed.Create())
                        {
                            sha_bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(verifier_string));
                        }

                        // Encode to base64 url safe
                        String challange_string = System.Convert.ToBase64String(sha_bytes)
                            .Split('=')[0].Replace('+', '-').Replace('/', '_');

                        // Base URL
                        String base_url = "https://accounts.spotify.com/authorize";

                        // Redirect URL - Must be added to whitelist in Spotify application API
                        String redirect_uri = "http://asaayu.com/arma-3/spotify/auth.php";

                        // Scopes required by the application
                        String scope = "user-read-currently-playing";

                        String final_url = base_url + "?response_type=code&client_id=" + app_client_id + "&redirect_uri=" + redirect_uri + "&scope=" + scope + "&state=" + state + "&code_challenge=" + challange_string + "&code_challenge_method=S256";

                        // DO NOT ALLOW USER TO CUSTOMIZE THIS.
                        System.Diagnostics.Process.Start(final_url);
                        break;

                    // LEGAL: View legal information about mod
                    case "legal":
                        output.Append("Go to https://github.com/Asaayu/Arma-Spotify-Jukebox to view the GitHub repo and view important legal information.");
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