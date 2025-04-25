using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anvil.Common
{
    public static class Util
    {
        static Util()
        {
            LogFilterToStringMap = new Dictionary<LogFilter, string>
            {
                { LogFilter.Normal, "Normal" },
                { LogFilter.Info, "Info" },
                { LogFilter.Model, "Model" },
                { LogFilter.Window, "Window" },
                { LogFilter.Controller, "Controller" },
                { LogFilter.View, "View" },
                { LogFilter.Service, "Service" },
                { LogFilter.Container, "Container" }
            };
        }

        public static readonly Dictionary<LogFilter, string> LogFilterToStringMap;
        // Initialize the dictionary with mappings
        // Conditional Logging to Console Pro
        public static void Log(string inLog, LogFilter filter = LogFilter.Normal)
        {
            #if ANVIL_DEBUG
            if (filter == LogFilter.Normal)
            {
                Debug.Log(inLog);
                return;
            }
            var inFilterName = LogFilterToStringMap[filter];
            Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"Filter\", \"name\":\"" + inFilterName + "\"}");
            #endif
        }

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

    public enum LogFilter
    {
        Normal,
        Info,
        Model,
        Controller,
        View,
        Window,
        Service,
        Container
    }
}