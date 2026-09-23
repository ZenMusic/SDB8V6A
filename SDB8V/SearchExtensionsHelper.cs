using System;
using System.Collections.Generic;

namespace SymbolDB
{
    public class SearchExtensionsHelper
    {
        public static (string, List<string>) GetSearchExtensionValues(GlobalVars globals, string searchExtensionName)
        {
            if (globals.SearchExtensions.TryGetValue(searchExtensionName, out string[] extensions))
            {
                // Convert the array to a comma-separated string and a list
                string joinedExtensions = string.Join(", ", extensions);
                List<string> extensionList = new List<string>(extensions);

                return (joinedExtensions, extensionList);
            }

            // Return empty values if the key is not found
            return (string.Empty, new List<string>());
        }
    }
}