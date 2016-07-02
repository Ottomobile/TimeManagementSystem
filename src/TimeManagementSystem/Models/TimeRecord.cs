using System;

namespace TimeManagementSystem.Models
{
    public class TimeRecord
    {
        public int ID { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime TimeWorkStart { get; set; }
        public DateTime TimeWorkEnd { get; set; }
        public DateTime DurationWork { get; set; }
        public DateTime TimeBreakStart { get; set; }
        public DateTime TimeBreakEnd { get; set; }
        public DateTime DurationBreak { get; set; }
        public DateTime TimeTotal { get; set; }
        public string Comments { get; set; }
    }
}
