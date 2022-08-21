using AutoMapper;
using MessagingService.DAL;
using MessagingService.DAL.ContextInfo;
using MessagingService.Entity.Models;
using MessagingService.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.BLL.Implementations
{
    public class ActivityLogListService : Service<ActivityLogListViewModel, ActivityLogListModel, Guid>, IActivityLogListService
    {
        public ActivityLogListService(IMapper mapper, IActivityLogListRepo repo) : base(mapper, repo)
        {

        }
    }
}
