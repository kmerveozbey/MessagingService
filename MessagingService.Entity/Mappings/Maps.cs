using AutoMapper;
using MessagingService.Entity.Models;
using MessagingService.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.Entity.Mappings
{
    public class Maps:Profile
    {
        public Maps()
        {
            CreateMap<UserModel, UserViewModel>().ReverseMap();
            CreateMap<MessageModel, MessageViewModel>().ReverseMap();
            CreateMap<ActivityLogListModel, ActivityLogListViewModel>().ReverseMap();
            CreateMap<BlockListModel, BlockListViewModel>().ReverseMap();
        }
    }
        
}
