using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace ClipboardPaster
{
    public static class RegistryManager
    {
        private const string KeyPath = @"Software\Classes\Directory\shell\ClipboardPaster";
        private const string KeyPathBackground = @"Software\Classes\Directory\Background\shell\ClipboardPaster";
        private const string MenuText = "Paste clipboard image";

        /// <summary>
        /// Checks if the context menu option is currently installed for the current user.
        /// </summary>
        public static bool IsInstalled()
        {
            try
            {
                using (RegistryKey key1 = Registry.CurrentUser.OpenSubKey(KeyPath))
                using (RegistryKey key2 = Registry.CurrentUser.OpenSubKey(KeyPathBackground))
                {
                    return key1 != null || key2 != null;
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool InstallKey(string keyPath, string exePath, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (key == null)
                    {
                        errorMessage = string.Format("Failed to create registry subkey: {0}", keyPath);
                        return false;
                    }

                    key.SetValue("", MenuText);
                    key.SetValue("Icon", string.Format("\"{0}\",0", exePath));

                    using (RegistryKey cmdKey = key.CreateSubKey("command"))
                    {
                        if (cmdKey == null)
                        {
                            errorMessage = string.Format("Failed to create command registry subkey under: {0}", keyPath);
                            return false;
                        }

                        cmdKey.SetValue("", string.Format("\"{0}\" \"%V\"", exePath));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Installs the right-click context menu entry under both Directory\shell and Directory\Background\shell for the current user.
        /// Does not require Administrator privileges.
        /// </summary>
        public static bool Install(out string errorMessage)
        {
            errorMessage = null;
            string exePath = Assembly.GetExecutingAssembly().Location;
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            {
                exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClipboardPaster.exe");
            }

            if (!InstallKey(KeyPath, exePath, out errorMessage))
            {
                return false;
            }

            if (!InstallKey(KeyPathBackground, exePath, out errorMessage))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Uninstalls the right-click context menu entries from HKCU.
        /// </summary>
        public static bool Uninstall(out string errorMessage)
        {
            errorMessage = null;
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(KeyPath, false);
                Registry.CurrentUser.DeleteSubKeyTree(KeyPathBackground, false);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
