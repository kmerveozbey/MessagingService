using MessagingService.DAL.ContextInfo;
using MessagingService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.DAL.Implementations
{
    public class MessageRepo : Repository<MessageModel, Guid>, IMessageRepo
    {
        public MessageRepo(MyContext myContext) : base(myContext)
        {

        }
    }
}
