using MessagingService.DAL.ContextInfo;
using MessagingService.DAL.Implementations;
using MessagingService.DAL;
using MessagingService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MessagingService.Entity.ViewModels;

namespace MessagingService.BLL.Implementations
{
    public class MessageService : Service<MessageViewModel, MessageModel, Guid>, IMessageService
    {
        public MessageService(IMapper mapper, IMessageRepo repo) : base(mapper, repo)
        {

        }
    }
}