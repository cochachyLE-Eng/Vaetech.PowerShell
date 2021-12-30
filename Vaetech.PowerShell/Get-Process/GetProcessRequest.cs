using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vaetech.Data.ContentResult;
using Vaetech.PowerShell.Types;

namespace Vaetech.PowerShell
{
    public class GetProcessRequest
    {
        public static string Command { get; private set; } = string.Empty;
        public static List<Tuple<GetProcessEnums, string>> Commands { get; private set; } = new List<Tuple<GetProcessEnums, string>>();
        public GetProcessRequest() { }
        public GetProcessRequest(string command) => Command = command;
        public GetProcessRequest(GetProcessEnums command, string arguments)
        {
            if (command == GetProcessEnums.GetProcess) Commands = new List<Tuple<GetProcessEnums, string>>();
            Commands.Add(new Tuple<GetProcessEnums, string>(command, arguments));
        }
        public string GetCommand()
        {            
            string[] comand = Commands.Select(i => new { Position = new GetProcessTypes()[i.Item1], Name = i.Item2 }).OrderBy(c => c.Position).Select(c => c.Name).ToArray();
            return Command = string.Join(" ", comand);
        }
        public static GetProcessRequest SetProcess(ErrorAction errorAction, params string[] names) => new GetProcessRequest(GetProcessEnums.GetProcess, $"{GetProcessTypes.GetProcess} {string.Join(", ", names)} {GetErrorAction(errorAction)}");
        public static GetProcessRequest SetProcess(params string[] names) => new GetProcessRequest(GetProcessEnums.GetProcess, $"{GetProcessTypes.GetProcess} {string.Join(", ", names)}");
        public GetProcessRequest AddCommand(GetProcessEnums command, string arguments) => new GetProcessRequest(command, arguments);
        public static string GetErrorAction(ErrorAction errorAction)
        {
            if (errorAction == ErrorAction.SilentlyContinue)
                return $"-ErrorAction {Enum.GetName(typeof(ErrorAction), ErrorAction.SilentlyContinue)}";
            else
                return string.Empty;
        }
        public ActionResult<GetProcessResponse> Execute()
        {
            ActionResult<GetProcessResponse> actionResult = new ActionResult<GetProcessResponse>();
            var outputBuilder = new StringBuilder();

            ShellHelper.Execute<GetProcessResponse>(
                GetCommand(),
                ex => actionResult = new ActionResult<GetProcessResponse>(true, ex),
                data => outputBuilder.Append(data)
            );

            actionResult.List = JsonConvert.DeserializeObject<List<GetProcessResponse>>(outputBuilder.ToString());

            return actionResult;
        }

    }
}
