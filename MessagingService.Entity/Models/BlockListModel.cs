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
    [Table("BlockLists")]
    public class BlockListModel
    {
        
        [Required]
        [Display(Name = "BlockID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BlockID { get; set; }

        public string? HinderingUserName { get; set; }

        [ForeignKey("HinderingUserName")]
        public virtual UserModel HinderUser { get; set; } = new UserModel();

        public string? BlockedUserName { get; set; }

        [ForeignKey("BlockedUserName")]
        public virtual UserModel BlockedUser { get; set; } = new UserModel();




    }
}
