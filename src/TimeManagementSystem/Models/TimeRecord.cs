using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TimeManagementSystem.Models
{
    public class TimeRecord
    {
        public int ID { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; }

        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public DateTime TimeWorkStart { get; set; }

        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public DateTime? TimeWorkEnd { get; set; }

        [Display(Name = "Duration Worked")]
        [DataType(DataType.Duration)]
        public TimeSpan DurationWork { get; set; }

        [Display(Name = "Break Time (Min)")]
        [Range(0, 300)]
        public int? TimeBreak { get; set; }

        public string Comments { get; set; }
    }
}
