using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.Entity.Models
{
    [Table("ActivityLogs")]
    public class ActivityLogListModel
    {

        [Required]
        [Display(Name = "ActivityID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityID { get; set; }

        public string? LoginUserName { get; set; }

        [ForeignKey("LoginUserName")]
        public virtual UserModel LoginUser { get; set; } = new UserModel();

        [Required]
        [Display(Name = "Login Status")]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime ActivityDate { get; set; }
    }
}
