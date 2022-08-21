using AutoMapper;
using MessagingService.DAL;
using MessagingService.DAL.ContextInfo;
using MessagingService.Entity.Models;
using MessagingService.Entity.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.BLL.Implementations
{
    public class BlockListService: Service<BlockListViewModel,BlockListModel, Guid>,IBlockListService
    {
        public BlockListService(IMapper mapper, IBlockListRepo repo) : base(mapper,repo)
        {

        }
    }
}
