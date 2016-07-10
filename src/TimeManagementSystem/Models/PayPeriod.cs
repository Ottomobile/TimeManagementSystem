using System;
using System.ComponentModel.DataAnnotations;

namespace TimeManagementSystem.Models
{
    public class PayPeriod
    {
        public int ID { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        public string UserName { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime PeriodStart { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime PeriodEnd { get; set; }

        [Display(Name = "Time Worked")]
        [DataType(DataType.Duration)]
        public TimeSpan PeriodTime { get; set; }

        [Display(Name = "Misc. Minutes")]
        public int? MiscMin { get; set; }

        [Display(Name = "Total Time Worked")]
        [DataType(DataType.Duration)]
        public TimeSpan PeriodTotalTime { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        public string Comments { get; set; }
    }
}
