using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace GameTranslator
{

    public static class ExternalCLIExecutor
    {
        public static string ExecuteTranslation(string lang, string text)
        {
            // Create process start info
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(Plugin.PluginPath, "TerminalApp.exe"),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("-l");
            startInfo.ArgumentList.Add(lang);
            startInfo.ArgumentList.Add("-t");
            startInfo.ArgumentList.Add(text);
            Plugin.Log.LogInfo(startInfo.Arguments);
            // Create and start the process
            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();

                // Read output and error asynchronously
                var outputTask = process.StandardOutput.ReadToEnd();
                var errorTask = process.StandardError.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Get the output
                string output = outputTask;
                string error = errorTask;

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"CLI execution error: {error}");
                }

                return output.Trim();
            }
        }
    }
}