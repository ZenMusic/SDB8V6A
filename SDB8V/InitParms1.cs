using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SymbolDB
{
    public class InitParms1
    {
        public string dirPath { get; set; } //directory with init file
        public string fpath { get; set; } //fullpath with init file
        public string targetDir1 { get; set; } //
        public string sourceDir1 { get; set; } // last transversal
        public string sourceDir2 { get; set; } // last transversal
        public string sourceDir3 { get; set; } // last transversal
        public string sourceDir4 { get; set; } // last transversal
        public string sourceDir5 { get; set; } // last transversal
        public string sourceDir6 { get; set; } // last transversal
        public string targetDir2 { get; set; }
        public string targetDir3 { get; set; }
        public string alarmSound { get; set; }
        public string watchFolder1 { get; set; }
        public string watchFolder2 { get; set; }
        public string watchFolder3 { get; set; }
        public int maxScreens { get; set; }
        public string history1 { get; set; }
        public string history2 { get; set; }
        public string history3 { get; set; }
        public string mostRecent { get; set; }
        public string mostRecentTarget { get; set; }
        public string mostRecentSubfolder { get; set; }
        public string notesBaseName { get; set; }
        public string mostRecent1 { get; set; }
        public string mostRecent2 { get; set; }
        public string mostRecent3 { get; set; }
        public string mostRecent4 { get; set; }
        public string mostRecent5 { get; set; }
        public string mostRecent6 { get; set; }
        public string ratingsVideosFile { get; set; }
        public string ratingsImagesFile { get; set; }

        private GlobalVars gv;
        public InitParms1()
        {

        }
        public InitParms1(GlobalVars g)
        {
            gv = g;
        }
        public InitParms1(string lstDir, string dirPath2, string fpath2, string targetDir2)
        {
            sourceDir1 = lstDir;
            dirPath = dirPath2;
            fpath = fpath2;
            targetDir1 = targetDir2;
        }
        public void SetGlobalVars(GlobalVars globalVars)
        {
            gv = globalVars;
        }
        public void SetTargetDir1(string dir)
        {
            if (gv == null)
            {
                MessageBox.Show("GlobalVars is not set. Cannot save target history.");
                return;
            }
            targetDir1 = dir;
            mostRecentTarget = dir;

            // Save to TargetHistory.xml if directory exists
            if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir) && gv != null)
            {
                SaveTargetHistory(dir);
            }
        }
        public void SetWatchDir1(string dir)
        {
            watchFolder1 = dir;
            mostRecentTarget = dir;
        }
        public void SetnotesBaseName(string name)
        {
            notesBaseName = name;
        }
        private void SaveTargetHistory(string targetPath)
        {
            try
            {
                string historyFilePath = Path.Combine(gv.dataFolder, "TargetHistory.xml");

                // Ensure the directory exists
                if (!Directory.Exists(gv.dataFolder))
                {
                    Directory.CreateDirectory(gv.dataFolder);
                }
                gv.dirDeletedFolder = Path.Combine(gv.dataFolder, "Deleted");
                if (!Directory.Exists(gv.dirDeletedFolder))
                {
                    Directory.CreateDirectory(gv.dirDeletedFolder);
                }
                
                XDocument doc;
                XElement root;

                // Load existing file or create new
                if (File.Exists(historyFilePath))
                {
                    doc = XDocument.Load(historyFilePath);
                    root = doc.Root;
                }
                else
                {
                    root = new XElement("TargetHistory");
                    doc = new XDocument(root);
                }

                // Check if entry exists
                var existingEntry = root.Elements("Target")
                    .FirstOrDefault(e => e.Element("Path")?.Value.Equals(targetPath, StringComparison.OrdinalIgnoreCase) == true);

                if (existingEntry != null)
                {
                    // Update existing entry
                    existingEntry.Element("LastUsed").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    // Add new entry
                    var newEntry = new XElement("Target",
                        new XElement("Path", targetPath),
                        new XElement("LastUsed", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    );
                    root.Add(newEntry);
                }

                // Save the file
                doc.Save(historyFilePath);
            }
            catch (Exception ex)
            {
                // Log error if gv.debug is available
                if (gv?.debug != null)
                {
                    gv.debug.w("Error saving TargetHistory.xml: ", ex.Message);
                }
            }
        }
    }
}
