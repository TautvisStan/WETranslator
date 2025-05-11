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
    public class CliProcessPool : IDisposable
    {
        private readonly string _executablePath;
        private readonly int _maxPoolSize;
        private readonly ConcurrentBag<Process> _availableProcesses;
        private readonly SemaphoreSlim _poolSemaphore;
        private bool _disposed = false;

        public CliProcessPool(string executablePath, int poolSize = 3)
        {
            _executablePath = executablePath;
            _maxPoolSize = poolSize;
            _availableProcesses = new ConcurrentBag<Process>();
            _poolSemaphore = new SemaphoreSlim(poolSize, poolSize);

            // Pre-initialize processes if desired
            for (int i = 0; i < poolSize; i++)
            {
                _availableProcesses.Add(CreateProcess());
            }
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
            Process process = null;
            Plugin.Log.LogInfo("TEST1");
            try
            {
                Plugin.Log.LogInfo("TEST2");
                // Get or create a process
                if (!_availableProcesses.TryTake(out process))
                {
                    process = CreateProcess();
                }
                Plugin.Log.LogInfo("TEST3");

                // Send input to process
                process.StandardInput.WriteLine(input);
                process.StandardInput.Flush();
                Plugin.Log.LogInfo("TEST4");
                // Read output
                string result = process.StandardOutput.ReadLine();
                Plugin.Log.LogInfo("TEST5");
                // Check for errors
                //string error = process.StandardError.ReadLine();
                //if (!string.IsNullOrEmpty(error))
                //{
                //    Plugin.Log.LogError($"CLI execution error: {error}");
                //}
                Plugin.Log.LogInfo("TEST6");
                return result;
            }
            finally
            {
                // Return process to the pool if it's still healthy
                if (process != null && !process.HasExited)
                {
                    _availableProcesses.Add(process);
                }
                else if (process != null)
                {
                    // Create a replacement process
                    _availableProcesses.Add(CreateProcess());
                }

                //// Release semaphore
                //_poolSemaphore.Release();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Kill all processes in the pool
                    foreach (var process in _availableProcesses)
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                            }
                            process.Dispose();
                        }
                        catch { /* Ignore disposal errors */ }
                    }

                    _poolSemaphore.Dispose();
                }

                _disposed = true;
            }
        }
        
    }
}