using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WindowsFormsApp1.files
{
    public class IniFile
    {
        private readonly string _iniFilePath;

        public IniFile(string iniFilePath)
        {
            _iniFilePath = iniFilePath;
        }

        public string Read(string section, string key, string defaultValue = "")
        {
            var lines = File.ReadAllLines(_iniFilePath);
            var inSection = false;

            foreach (var line in lines)
            {
                // Trim whitespace
                var trimmedLine = line.Trim();

                // Check for section header
                if (trimmedLine.Equals($"[{section}]", StringComparison.OrdinalIgnoreCase))
                {
                    inSection = true;
                    continue;
                }

                // Check if we are in the right section
                if (inSection)
                {
                    // Check for key-value pair
                    if (trimmedLine.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                    {
                        return trimmedLine.Substring(trimmedLine.IndexOf('=') + 1).Trim();
                    }

                    // If we reach another section, break out of the loop
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        break;
                    }
                }
            }

            // Return default value if key was not found
            return defaultValue;
        }

        public void Write(string section, string key, string value)
        {
            var lines = File.ReadAllLines(_iniFilePath).ToList();
            var inSection = false;
            var keyWritten = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var trimmedLine = lines[i].Trim();

                // Check for section header
                if (trimmedLine.Equals($"[{section}]", StringComparison.OrdinalIgnoreCase))
                {
                    inSection = true;
                    continue;
                }

                // Check if we are in the right section
                if (inSection)
                {
                    // Check for key-value pair
                    if (trimmedLine.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"{key}={value}";
                        keyWritten = true;
                        break;
                    }

                    // If we reach another section, break out of the loop
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        break;
                    }
                }
            }

            // If section doesn't exist, add it
            if (!inSection)
            {
                lines.Add($"[{section}]");
                inSection = true;
            }

            // If the key was not written, add it
            if (!keyWritten)
            {
                lines.Add($"{key}={value}");
            }

            // Write all lines back to the INI file
            File.WriteAllLines(_iniFilePath, lines);
        }
    }
}
