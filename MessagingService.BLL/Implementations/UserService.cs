using AutoMapper;
using MessagingService.DAL;
using MessagingService.Entity.Models;
using MessagingService.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.BLL.Implementations
{
    public class UserService : Service<UserViewModel, UserModel, string>, IUserService
    {
        public UserService(IMapper mapper, IUserRepo repo) : base(mapper, repo)
        {

        }
    }
}