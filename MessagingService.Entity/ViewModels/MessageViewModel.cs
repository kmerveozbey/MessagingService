using MessagingService.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessagingService.Entity.ViewModels
{
    public class MessageViewModel
    {
        [Required]
        [Display(Name = "MessageID")]
        public Guid MessageID { get; set; }
              
        [Required]
        [Display(Name = "Send User Name")]
        public string SenderUserName { get; set; }

        [Required]
        [Display(Name = "Receiver User Name")]
        public string ReceiverUserName { get; set; }


        [Required]
        [Display(Name = "Send Date")]
        public DateTime SendDate { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }
        public  UserViewModel? Sender { get; set; } 
        public  UserViewModel? Receiver { get; set; }


    }
}
