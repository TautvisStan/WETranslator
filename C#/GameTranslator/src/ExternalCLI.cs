using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

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
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            process.Start();
            return process;
        }

        public string Execute(string input)
        {
            try
            {
                if (_process == null)
                {
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