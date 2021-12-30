using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Vaetech.PowerShell.Types;

namespace Vaetech.PowerShell
{
    public class GetProcessTypes
    {
        [Display(Order = 0)]
        public static string GetProcess => "Get-Process";
        [Display(Order = 1)]
        public static string WhereObject => "Where-Object";
        [Display(Order = 1)]
        public static string Where => "Where";
        [Display(Order = 2)]
        public static string FormatTable => "Format-Table";
        [Display(Order = 2)]
        public static string FormatList => "Format-List";
        [Display(Order = 2)]
        public static string SelectObject => "Select-Object";
        [Display(Order = 3)]
        public static string ConvertToJson => "ConvertTo-Json";
        [Display(Order = 4)]
        public static string StopProcess => "Stop-Process";
        [Display(Order = 4)]
        public static string StopProcessForce => "Stop-Process -Force";

        public int this[GetProcessEnums type] => typeof(GetProcessTypes).GetProperty(Enum.GetName(typeof(GetProcessEnums), type)).GetCustomAttribute<DisplayAttribute>().GetOrder().Value;
    }
}
