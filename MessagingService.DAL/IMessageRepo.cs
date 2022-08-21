using MessagingService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.DAL
{
    public interface IMessageRepo : IRepository<MessageModel, Guid>
    {
    }
}
