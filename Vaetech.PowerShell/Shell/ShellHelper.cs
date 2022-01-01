using System.Diagnostics;
using System.Text;
using System.IO;
using System;

namespace Vaetech.PowerShell
{
    public static class ShellHelper
    {
        public static ShellExecutionResult Execute<T>(string cmd, Action<string> stdErrDataReceivedCallback = null, Action<string> stdOutDataReceivedCallback = null)
            => Execute<T>("C:\\", cmd, stdErrDataReceivedCallback, stdOutDataReceivedCallback);
        public static ShellExecutionResult Execute<T>(string directoryWorking, string cmd, Action<string> stdErrDataReceivedCallback = null, Action<string> stdOutDataReceivedCallback = null)
        {
            bool isWindows = true;
            var escapedArgs = cmd.Replace("\"", "\\\"");
            var outputBuilder = new StringBuilder();
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = directoryWorking,
                    FileName = isWindows ? "PowerShell.exe" : "pwsh.exe",
                    Arguments = isWindows ? $"{escapedArgs}" : $"-c \"{escapedArgs}\"",                    
                    RedirectStandardOutput = true,  
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };
            process.ErrorDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {                    
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        stdErrDataReceivedCallback?.Invoke(e.Data);
                    }
                    if (stdErrDataReceivedCallback == null)
                    {
                        Debug.WriteLine(e.Data);
                    }
                    outputBuilder.AppendLine(e.Data);
                }
            );
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        stdOutDataReceivedCallback?.Invoke(e.Data);
                    }
                    if (stdOutDataReceivedCallback == null)
                    {
                        Debug.WriteLine(e.Data);
                    }
                    outputBuilder.AppendLine(e.Data);
                }
            );

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.CancelOutputRead();
            process.CancelErrorRead();

            Debug.WriteLine("ExitCode: {0}", process.ExitCode);
            ///Console.WriteLine("Output: {0}", outputBuilder.ToString());

            return new ShellExecutionResult()
            {
                ExitCode = process.ExitCode,
                Output = outputBuilder.ToString()
            };
        }

        public static string SanitizeFilename(string filename)
        {
            filename = filename.Replace("/", "").Replace("\\", "").Replace("\"", "");
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
