using MessagingService.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.BLL
{
    public interface IMessageService : IService<MessageViewModel, Guid>
    {
    }
}
