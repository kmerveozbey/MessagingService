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
    public class BlockListViewModel
    {
        [Required]
        [Display(Name = "BlockID")]
        public Guid BlockID { get; set; }

        [Required]
        [Display(Name = "Hindering User Name")]
        public string HinderingUserName { get; set; }


        [Required]
        [Display(Name = "Blocked User Name")]
        public string BlockedUserName { get; set; }


        public UserViewModel? HinderUser { get; set; }

        public UserViewModel? BlockedUser { get; set; }
    }
}
