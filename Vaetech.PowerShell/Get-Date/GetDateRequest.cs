using System;

namespace Vaetech.PowerShell
{
    public class GetDateRequest
    {
        public static string Command { get; private set; }
        public static DateTime DateTime { get; private set; }
        public GetDateRequest() => GetDate(DateTime.Now);
        public GetDateRequest(DateTime dateTime) => GetDate(dateTime);
        private GetDateRequest(string command) => GetDateRequest.Command = command;
        public static GetDateRequest GetDate(DateTime dateTime) => new GetDateRequest($"(Get-Date -Date \"{(DateTime = dateTime).ToString(PShellSettings.DateFormat)}\")");
        public string GetCommand() => Command;
    }
}
