using MessagingService.DAL.ContextInfo;
using MessagingService.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.DAL.Implementations
{
    public class BlockListRepo:Repository<BlockListModel,Guid>,IBlockListRepo
    {
        public BlockListRepo(MyContext myContext) : base(myContext)
        {

        }    
    }
}
