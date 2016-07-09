using System.ComponentModel.DataAnnotations;

namespace TimeManagementSystem.Models
{
    public class SubscribeToUser
    {
        public int ID { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        [Display(Name = "Current Username")]
        public string CurrentUser { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        [Display(Name = "Username of Manager")]
        public string ManagingUser { get; set; }
    }
}
