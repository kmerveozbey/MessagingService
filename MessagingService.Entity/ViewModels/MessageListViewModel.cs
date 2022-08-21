using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessagingService.Entity.ViewModels
{
    public class MessageListViewModel
    {
        [Required]
        [Display(Name = "Receiver User Name")]
        public string ReceiverUserName { get; set; }

        [Required]
        [Display(Name = "Sender User Name")]
        public string SenderUserName { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Required]
        [Display(Name = "Send Date")]
        public DateTime Date { get; set; }
    }
}
