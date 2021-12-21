using System;
using System.ComponentModel.DataAnnotations;

namespace Vaetech.PowerShell
{
    public class GetProcessResponse
    {
        [Display(Order = 0)]
        public string Id { get; set; }
        [Display(Order = 1)]
        public string Name { get; set; }
        [Display(Order = 2)]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Order = 3)]        
        public long NPM { get; set; }
        /// <summary>
        /// The amount of pageable memory that the process is using, in kilobytes.
        /// </summary>
        [Display(Order = 4)]        
        public long PM { get; set; }
        /// <summary>
        /// The size of the working set of the process, in kilobytes.
        /// </summary>
        [Display(Order = 5)]
        public long WS { get; set; }
        /// <summary>
        /// The amount of virtual memory that the process is using, in megabytes.
        /// </summary>
        [Display(Order = 6)]
        public long VM { get; set; }
        /// <summary>
        /// The amount of processor time that the process has used on all processors, in seconds.
        /// </summary>
        [Display(Order = 7)]
        public long CPU { get; set; }
        /// <summary>
        /// The number of handles that the process has opened.
        /// </summary>
        [Display(Order = 8)]
        public int Handles { get; set; }
    }
}
