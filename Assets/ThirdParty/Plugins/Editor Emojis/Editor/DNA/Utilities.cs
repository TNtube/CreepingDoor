using System;

namespace Anvil.Common
{
    public static class Utilities
    {
        public static string SanitizePath(string path)
        {
            // Array of invalid characters
            char[] invalidChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

            // Replace each invalid character with an empty string
            foreach (char c in invalidChars)
            {
                path = path.Replace(c.ToString(), "");
            }

            // Handle reserved names
            string[] reservedNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

            // Trim leading and trailing spaces and periods
            path = path.Trim().TrimEnd('.');

            // Prevent using reserved names (case-insensitive)
            string folderName = System.IO.Path.GetFileName(path).ToUpperInvariant();
            if (Array.Exists(reservedNames, reserved => reserved.Equals(folderName, StringComparison.OrdinalIgnoreCase)))
            {
                path = path + "_safe";  // Add a suffix to make it valid
            }

            // Replace multiple slashes with a single slash (normalize path)
            while (path.Contains("//"))
            {
                path = path.Replace("//", "/");
            }

            return path;
        }

    }
}