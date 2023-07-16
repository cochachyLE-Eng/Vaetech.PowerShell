using System;
using Vaetech.Data.ContentResult;
using Vaetech.PowerShell.Types;

namespace Vaetech.PowerShell.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Settings
            PShellSettings.DateFormat = "yyyy/MM/dd hh:mm:ss";

            TestExpression();

            //GetProcessInFormatList();
            //GetProcessWithErrorActionInCustomCollection();
            //GetProcessByDateRange();

            //StopProcessByDateRange();

            System.Console.ReadKey();            
        }
        public static void TestExpression()
        {
            var getProcessResponse1 = PShell.GetProcess("svchost", "w3wp").WhereObject(c => c.StartTime == DateTime.Now.Date).SelectObject(x => new { x.Name, x.Id, x.StartTime, x.PM, x.WS, x.VM, x.CPU, x.Handles }).ConvertToJson();
            string command1 = getProcessResponse1.GetCommand();
            System.Console.WriteLine(command1);

            DateTime startTime = DateTime.Now.Date;
            var getProcessResponse2 = PShell.GetProcess("svchost", "w3wp").WhereObject(c => c.StartTime == startTime).SelectObject(x => new { x.Name, x.Id, x.StartTime, x.PM, x.WS, x.VM, x.CPU, x.Handles }).ConvertToJson();
            string command2 = getProcessResponse2.GetCommand();
            System.Console.WriteLine(command2);

            var getProcessResponse3 = PShell.GetProcess("svchost", "w3wp").WhereObject(c => c.StartTime > DateTime.Now.AddDays(-7) && c.StartTime < DateTime.Now.AddDays(-1)).SelectObject(x => new { x.Name, x.Id, x.StartTime, x.PM, x.WS, x.VM, x.CPU, x.Handles }).ConvertToJson();
            string command3 = getProcessResponse3.GetCommand();
            System.Console.WriteLine(command3);

            DateTime[] startTimeArray = new DateTime[2];
            startTimeArray[0] = DateTime.Now.AddDays(-7);
            startTimeArray[1] = DateTime.Now.AddDays(-1);
            var getProcessResponse4 = PShell.GetProcess("svchost", "w3wp").WhereObject(c => c.StartTime > startTimeArray[0] && c.StartTime < startTimeArray[1]).SelectObject(x => new { x.Name, x.Id, x.StartTime, x.PM, x.WS, x.VM, x.CPU, x.Handles }).ConvertToJson();
            string command4 = getProcessResponse4.GetCommand();
            System.Console.WriteLine(command4);
        }
        /// <summary>
        /// Gets the processes that are running on the local computer.        
        /// </summary>
        public static void GetProcessInFormatList()
        {
            System.Console.WriteLine("# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            System.Console.WriteLine("# Gets the processes that are running on the local computer.");
            System.Console.WriteLine("# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            var getProcessResponse = PShell.GetProcess("svchost").WhereObject(c => c.StartTime > DateTime.Now.AddDays(-5)).FormatList(x => new { x.Name, x.Id, x.StartTime });            
            string command = getProcessResponse.GetCommand();            

            System.Console.WriteLine(command);
        }        
        /// <summary>         
        /// Gets the processes running on the local computer, in custom collection.
        /// </summary>
        public static void GetProcessWithErrorActionInCustomCollection()
        {
            System.Console.WriteLine("# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            System.Console.WriteLine("# Gets the processes running on the local computer, in custom collection.");
            System.Console.WriteLine("# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            var getProcessResponse = PShell.GetProcess(ErrorAction.SilentlyContinue, "svchost", "w3wp").SelectObject(x => new { x.Name, x.Id, x.StartTime }).WhereObject(c => c.StartTime.Date > DateTime.Now.AddDays(-24).Date).ConvertToJson();
            string command = getProcessResponse.GetCommand();

            System.Console.WriteLine(command);

            // Execute command
            ActionResult<GetProcessResponse> resultGetProcess = getProcessResponse.Execute();
            
            foreach (var process in resultGetProcess.List)
            {
                System.Console.WriteLine("ID: {0}, Name: {1}, StartTime: {2}, CPU: {3}", process.Id, process.Name, process.StartTime.ToString(PShellSettings.DateFormat), process.CPU);
            }
        }        
        /// <summary>         
        /// Get results by date range
        /// </summary>
        public static void GetProcessByDateRange()
        {
            System.Console.WriteLine("# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            System.Console.WriteLine("# Get results by date range");
            System.Console.WriteLine("# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            var getProcessResponse = PShell.GetProcess("svchost", "w3wp").WhereObject(c => c.StartTime > DateTime.Now.AddDays(-7) && c.StartTime < DateTime.Now.AddDays(-1)).SelectObject(x => new { x.Name, x.Id, x.StartTime, x.PM, x.WS, x.VM, x.CPU, x.Handles }).ConvertToJson();
            string command = getProcessResponse.GetCommand();

            System.Console.WriteLine(command);

            // Execute command
            ActionResult<GetProcessResponse> resultGetProcess = getProcessResponse.Execute();

            if (resultGetProcess.IB_Exception)
                System.Console.WriteLine("Error: {0}",resultGetProcess.Message);            
            else 
            {
                foreach (var process in resultGetProcess.List)
                {
                    System.Console.WriteLine("ID: {0}, Name: {1}, StartTime: {2}, CPU: {3}", process.Id, process.Name, process.StartTime.ToString(PShellSettings.DateFormat), process.CPU);
                }
            }
        }
        public static void StopProcessByDateRange()
        {
            System.Console.WriteLine("# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            System.Console.WriteLine("# Stop Process (-Force) by date range");
            System.Console.WriteLine("# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            var stopProcessResponse = PShell.GetProcess("w3wp").WhereObject(c => c.StartTime > DateTime.Now.AddDays(-7) && c.StartTime < DateTime.Now.AddDays(-1)).StopProcessForce();
            string command = stopProcessResponse.GetCommand();

            System.Console.WriteLine(command);

            // Execute command
            ActionResult<GetProcessResponse> resultStopProcess = stopProcessResponse.Execute();

            if (resultStopProcess.IB_Exception)
                System.Console.WriteLine("Error: {0}", resultStopProcess.Message);
            else
            {
                System.Console.WriteLine("Successful process");
            }
        }
    }    
}
