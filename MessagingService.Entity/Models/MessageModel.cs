using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessagingService.Entity.Models
{
    [Table("Messages")]
    public class MessageModel
    {
        [Required]
        [Display(Name = "MessageID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MessageID { get; set; }

        public string? SenderUserName { get; set; }

        [ForeignKey("SenderUserName")]
        public virtual UserModel Sender { get; set; } = new UserModel();

        public string? ReceiverUserName { get; set; }

        [ForeignKey("ReceiverUserName")]
        public virtual UserModel Receiver { get; set; } = new  UserModel();

        [Required]
        [Display(Name = "Send Date")]
        public DateTime SendDate { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        

    }
}
