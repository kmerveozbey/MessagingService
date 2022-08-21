using MessagingService.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessagingService.Entity.ViewModels
{
    public class ActivityLogListViewModel
    {
        [Required]
        [Display(Name = "ActivityID")]
        public Guid ActivityID { get; set; }

        [Required]
        [Display(Name = "Login User Name")]
        public string LoginUserName { get; set; }

        public  UserViewModel? LoginUser { get; set; }

        [Required]
        [Display(Name = "Login Status")]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime ActivityDate { get; set; }
    }
}
