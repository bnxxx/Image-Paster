using System;
using System.IO;
using System.Windows.Forms;

namespace ClipboardPaster
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                // No arguments passed: Launch the setup & installer GUI
                Application.Run(new InstallerForm());
                return;
            }

            bool runOcr = false;
            string firstArg = null;

            foreach (string arg in args)
            {
                string cleanArg = arg.Trim().Trim('"');
                if (string.Equals(cleanArg, "--ocr", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(cleanArg, "-ocr", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(cleanArg, "/ocr", StringComparison.OrdinalIgnoreCase))
                {
                    runOcr = true;
                }
                else if (firstArg == null)
                {
                    firstArg = cleanArg;
                }
            }

            if (firstArg == null)
            {
                firstArg = Environment.CurrentDirectory;
            }

            if (string.Equals(firstArg, "--install", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(firstArg, "/install", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(firstArg, "-i", StringComparison.OrdinalIgnoreCase))
            {
                string error;
                if (RegistryManager.Install(out error))
                {
                    Console.WriteLine("Successfully installed context menu entries.");
                }
                else
                {
                    Console.WriteLine(string.Format("Installation failed: {0}", error));
                    Environment.ExitCode = 1;
                }
                return;
            }

            if (string.Equals(firstArg, "--uninstall", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(firstArg, "/uninstall", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(firstArg, "-u", StringComparison.OrdinalIgnoreCase))
            {
                string error;
                if (RegistryManager.Uninstall(out error))
                {
                    Console.WriteLine("Successfully uninstalled context menu entries.");
                }
                else
                {
                    Console.WriteLine(string.Format("Uninstallation failed: {0}", error));
                    Environment.ExitCode = 1;
                }
                return;
            }

            if (string.Equals(firstArg, "--gui", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(firstArg, "/gui", StringComparison.OrdinalIgnoreCase))
            {
                Application.Run(new InstallerForm());
                return;
            }

            // If clicked from open folder background or if %V was not expanded, resolve to Environment.CurrentDirectory
            if (firstArg == "%V" || firstArg == "." || string.IsNullOrEmpty(firstArg))
            {
                firstArg = Environment.CurrentDirectory;
            }

            // Otherwise, check if firstArg is a directory path (from right-click context menu %V or background)
            if (Directory.Exists(firstArg))
            {
                ClipboardHandler.ProcessPasteRequest(firstArg, runOcr);
            }
            else
            {
                Console.WriteLine(string.Format("Invalid argument or non-existent directory: {0}", firstArg));
            }
        }
    }
}
