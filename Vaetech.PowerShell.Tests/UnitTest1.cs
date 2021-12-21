using NUnit.Framework;
using System;
using Vaetech.Data.ContentResult;
using Vaetech.PowerShell.Types;

namespace Vaetech.PowerShell.Tests
{
    public class Tests
    {    
        [Test]
        public void GetProcessInFormatList()
        {
            // Settings
            PShellSettings.DateFormat = "yyyy/MM/dd hh:mm:ss";

            // Parameters
            DateTime startTime = DateTime.Now.AddDays(-5);
            string command = $"Get-Process svchost |Where-Object {{$_.StartTime -gt (Get-Date -Date \"{startTime.ToString(PShellSettings.DateFormat)}\")}} |Format-List Name, Id, StartTime";
            
            // Gets the processes that are running on the local computer.
            var getProcessResponse = PShell.GetProcess("svchost").WhereObject(c => c.StartTime > startTime).FormatList(x => new { x.Name, x.Id, x.StartTime });
            
            Assert.AreEqual(command, getProcessResponse.GetCommand());
        }
        [Test]       
        public void GetProcessWithErrorActionInCustomCollection()
        {
            // Settings
            PShellSettings.DateFormat = "yyyy/MM/dd hh:mm:ss";

            // Parameters
            DateTime startTime = DateTime.Now.AddDays(-5);            
            string command = $"Get-Process svchost, w3wp -ErrorAction SilentlyContinue |Where-Object {{$_.StartTime.Date -gt (Get-Date -Date \"{startTime.ToString(PShellSettings.DateFormat)}\").Date}} |Select-Object Name, Id, StartTime |ConvertTo-Json";
            
            // Gets the processes running on the local computer, in custom collection.
            var getProcessResponse = PShell.GetProcess(ErrorAction.SilentlyContinue, "svchost", "w3wp").SelectObject(x => new { x.Name, x.Id, x.StartTime }).WhereObject(c => c.StartTime.Date > startTime.Date).ConvertToJson();
            
            // Execute command
            ActionResult<GetProcessResponse> resultGetProcess = getProcessResponse.Execute();            

            Assert.AreEqual(command, getProcessResponse.GetCommand());
            Assert.IsFalse(resultGetProcess.IB_Exception);
            Assert.IsNull(resultGetProcess.Message);
            Assert.IsNotNull(resultGetProcess.List);            
        }
        [Test]
        public void GetProcessByRangeDate()
        {
            // Settings
            PShellSettings.DateFormat = "yyyy/MM/dd hh:mm:ss";

            // Parameters
            DateTime[] dateTimes = new DateTime[] { DateTime.Now.AddDays(-7), DateTime.Now.AddDays(-1) };
            string command = $"Get-Process svchost, w3wp |Where-Object {{$_.StartTime -gt (Get-Date -Date \"{dateTimes[0].ToString(PShellSettings.DateFormat)}\") -and $_.StartTime -lt (Get-Date -Date \"{dateTimes[1].ToString(PShellSettings.DateFormat)}\")}} |Select-Object Name, Id, StartTime, PM, WS, VM, CPU, Handles |ConvertTo-Json";

            // Get results by date range.
            var getProcessResponse = PShell.GetProcess("svchost", "w3wp").WhereObject(c => c.StartTime > dateTimes[0] && c.StartTime < dateTimes[1]).SelectObject(x => new { x.Name, x.Id, x.StartTime, x.PM, x.WS, x.VM, x.CPU, x.Handles }).ConvertToJson();
            
            // Execute command
            ActionResult<GetProcessResponse> resultGetProcess = getProcessResponse.Execute();

            Assert.AreEqual(command, getProcessResponse.GetCommand());
            Assert.IsFalse(resultGetProcess.IB_Exception);
            Assert.IsNull(resultGetProcess.Message);
            Assert.IsNotNull(resultGetProcess.List);
        }
    }
}