using System;
using Vaetech.PowerShell.Types;

namespace Vaetech.PowerShell
{
    public class PShell
    {
        public static string Command { get; private set; }
        public PShell() { }        
        public static GetProcessRequest GetProcess(params string[] process) => GetProcessRequest.SetProcess(process);
        public static GetProcessRequest GetProcess(ErrorAction errorAction, params string[] process) => GetProcessRequest.SetProcess(errorAction, process);
        public static GetDateRequest GetDate() => new GetDateRequest();
        public static GetDateRequest GetDate(DateTime dateTime) => new GetDateRequest(dateTime);        
    }    
}
