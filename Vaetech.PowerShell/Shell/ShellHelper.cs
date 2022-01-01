using System.Diagnostics;
using System.Text;
using System.IO;
using System;
using Vaetech.Runtime.Utils.Platforms;

namespace Vaetech.PowerShell
{
    public static class ShellHelper
    {        
        public static ShellExecutionResult Execute<T>(string cmd, Action<string> stdErrDataReceivedCallback = null, Action<string> stdOutDataReceivedCallback = null)
        {            
            var escapedArgs = cmd.Replace("\"", "\\\"");
            var outputBuilder = new StringBuilder();
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = ShellHelper.GetWorkingDirectory(),
                    FileName = ShellHelper.GetArguments(),
                    Arguments = $"{escapedArgs}",                    
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
        private static string GetWorkingDirectory()
        {
            if (OSPlatform.IsWindows)
                return "C:/";
            if (OSPlatform.IsLinux)
                return "/usr/bin"; ///usr/share/dotnet/
            if (OSPlatform.IsMacOSX)
                return "/usr/local/bin"; ///usr/local/share/dotnet
            throw new InvalidOperationException("Unsupported platform.");
        }
        private static string GetArguments()
        {
            if (OSPlatform.IsWindows)
                return "PowerShell.exe";
            if (OSPlatform.IsLinux)
                ///bin/sh -c 'PATH="<dotnet-root-dir>:$PATH" DOTNET_ROOT="<dotnet-root-dir>" exec ~/.dotnet/tools/pwsh'
                return "sh -c 'exec ~/.dotnet/tools/pwsh"; 
            if (OSPlatform.IsMacOSX)
                ///bin/sh -lc 'PATH="<dotnet-root-dir>:$PATH" DOTNET_ROOT="<dotnet-root-dir>" exec ~/.dotnet/tools/pwsh'
                return "sh -lc 'PATH=\"/usr/local/share/dotnet:$PATH\" exec ~/.dotnet/tools/pwsh'"; ///usr/local/share/dotnet
            throw new InvalidOperationException("Unsupported platform.");
        }
    }
}
