using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArmaSpotifyController
{
    public class DllEntry
    {
        // Predefined version information + author name
        new static readonly string version_info = "ASJ DLL - VER 0.0.1 - Asaayu";


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

            switch (function.ToLower())
            {
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