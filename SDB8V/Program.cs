using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SymbolDB
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);  // 👈 required
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Catch any unhandled exceptions and show a message box
            Application.ThreadException += (sender, args) =>
            {
                MessageBox.Show(
                    "Unhandled exception:\n" + args.Exception.ToString(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            };
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // Ensure the SQLite database exists, is configured (WAL, foreign_keys), and schema applied.
            SqliteDb.Initialize();
            //MessageBox.Show(SqliteDb.ConnectionString, "DB in use");

            Application.Run(new Main());
        }
    }
}
