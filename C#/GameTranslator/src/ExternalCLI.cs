using System;
using System.Diagnostics;

namespace GameTranslator
{
    public class CLIProcess
    {
        private readonly string _executablePath;
        private Process _process = null;

        public CLIProcess(string executablePath)
        {
            _executablePath = executablePath;
            _process = CreateProcess();
        }

        private Process CreateProcess()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _executablePath,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8,
                    StandardInputEncoding = System.Text.Encoding.UTF8,
                    Environment = { {"PYTHONIOENCODING", "utf-8"} }
                },
                EnableRaisingEvents = true
            };
            process.ErrorDataReceived += (sender, e) => {

                if (!string.IsNullOrEmpty(e.Data))

                    Plugin.Log.LogError($"Translator error: {e.Data}");

            };
            process.Start();
            process.BeginErrorReadLine();
           // string initialOutput = process.StandardOutput.ReadLine();
            return process;
        }

        public string Execute(string input)
        {
            try
            {
                if (_process == null || _process.HasExited)
                {
                    if (_process.HasExited)
                    {
                        Plugin.Log.LogError("Translator ded");
                    }
                    _process = CreateProcess();
                }
                _process.StandardInput.WriteLine(input);
                _process.StandardInput.Flush();
                string result = _process.StandardOutput.ReadLine();
                return result;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError("There was an error trying to run translator:");
                Plugin.Log.LogError(ex);
                return input;
            }
        }
    }
}