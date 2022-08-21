using MessagingService.Entity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MessagingService.DAL.ContextInfo
{
    public class MyContext: DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }
        public DbSet<BlockListModel> BlockList { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<ActivityLogListModel> ActivityLogs { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
