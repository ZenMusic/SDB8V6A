using System;
using Microsoft.Win32;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    public class GetDotNetVersion
    {
        public static void Get45PlusFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    Console.WriteLine(".NET Framework Version: " + CheckFor45PlusVersion((int)ndpKey.GetValue("Release")));
                }
                else
                {
                    Console.WriteLine(".NET Framework Version 4.5 or later is not detected.");
                }
            }
        }

        public static string Get45PlusVersionFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return CheckFor45PlusVersion((int)ndpKey.GetValue("Release"));
                }
                else
                {
                    return ".NET Framework Version 4.5 or later is not detected.";
                }
            }
        }

        // Checking the version using >= will enable forward compatibility.
        // keep this up to date 
        // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed#net_d

        private static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 461808)
                return "4.7.2 or later";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394803)
                return "4.6.2 or later";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if ((releaseKey >= 379893))
                return "4.5.2";
            if ((releaseKey >= 378675))
                return "4.5.1";
            if ((releaseKey >= 378389))
                return "4.5";

            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }
    }
    // Calling the GetDotNetVersion.Get45PlusFromRegistry method produces 
    // output like the following:
    //       .NET Framework Version: 4.6.1
}