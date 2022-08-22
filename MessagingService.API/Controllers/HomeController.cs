using MessagingService.API.Properties;
using MessagingService.BLL;
using MessagingService.BLL.Implementations;
using MessagingService.BLL.Process;
using MessagingService.DAL.ContextInfo;
using MessagingService.Entity.Models;
using MessagingService.Entity.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;

namespace MessagingService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IBlockListService _blockListService;
        private readonly IActivityLogListService _activityListService;

        public HomeController(IUserService userService, IMessageService messageService, IBlockListService blockListService, IActivityLogListService activityListService)
        {
            _userService = userService;
            _messageService = messageService;
            _blockListService = blockListService;
            _activityListService = activityListService;
        }
        
        private string activityLogCreate(string userName, string status)
        {
            try
            {
                ActivityLogListViewModel activityLogsError = new ActivityLogListViewModel()
                {
                    ActivityID = Guid.NewGuid(),
                    LoginUserName = userName,
                    ActivityDate = DateTime.Now,
                    Status = status,
                };

                var resultActivityLogs = _activityListService.Add(activityLogsError);

                if (!resultActivityLogs.IsSuccess)
                {
                    return Resources.UnexpectedErrorMsg;
                }

                return Resources.SuccessMsg;
            }
            catch (Exception ex)
            {
                return Resources.UnexpectedErrorMsg + ex.Message;
            }
        }

        [HttpGet(Name = "Index")]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Problem(Resources.ValidErrorMsg);
                }
                UserViewModel user = new UserViewModel()
                {
                    Email = model.Email,
                    Phone = model.Phone,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.UserName
                };

                var userCheck = _userService.GetByConditions(x => x.UserName == model.UserName || x.Email == model.Email).Data;

                if (userCheck != null)
                {
                    return Problem(Resources.FoundUserErrorMsg);
                }

                string md5password = ProcessForPassword.MD5Hash(model.Password);
                user.Password = md5password;

                var result = _userService.Add(user);

                if (!result.IsSuccess)
                {
                    return Problem(Resources.SaveErrorMsg);
                }

                return Ok(Resources.SuccessMsg);
            }
            catch (Exception ex)
            {
                return Problem(Resources.UnexpectedErrorMsg + ex.Message);
            }

        }
       
        [HttpPost]
        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Problem(Resources.ValidErrorMsg);
                }

                LoginViewModel login = new LoginViewModel()
                {
                    RememberMe = model.RememberMe,
                    UserName = model.UserName,
                };

                var md5Password = ProcessForPassword.MD5Hash(model.Password);

                var user = _userService.GetByConditions(x => x.UserName == model.UserName && x.Password == md5Password).Data;

                if (user == null)
                {
                    var userName = _userService.GetByConditions(x => x.UserName == model.UserName).Data;

                    if (userName != null)
                    {
                        activityLogCreate(userName.UserName, Resources.LoginStatusErrorMsg);
                    }

                    return Problem(Resources.NotFoundUserMsg);
                }

                if (login.RememberMe)
                {
                    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, login.UserName));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                }

                activityLogCreate(user.UserName, Resources.LoginStatusSuccessMsg);

                return Ok(Resources.SuccessMsg);
            }
            catch (Exception ex)
            {
                return Problem(Resources.UnexpectedErrorMsg + ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult RememberMeTestResult()
        {
            var userName = HttpContext.User.Identity.Name;

            var user = _userService.GetByConditions(x => x.UserName == userName).Data;

            if (user == null)
            {
                return Problem(Resources.UnexpectedErrorMsg);
            }

            activityLogCreate(user.UserName, Resources.LoginStatusSuccessMsg);

            return Ok(Resources.SuccessMsg);
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult Logout()
        {
            var userName = HttpContext.User.Identity.Name;

            var user = _userService.GetByConditions(x => x.UserName == userName).Data;

            if (user == null)
            {
                return Problem(Resources.UnexpectedErrorMsg);
            }

            activityLogCreate(user.UserName, Resources.LogOutSuccessMsg);

            HttpContext.SignOutAsync();

            return Ok(Resources.SuccessMsg);
        }

        [HttpPost]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult SendMessage(MessageViewModel model)
        {
            try
            {

                var senderUserName = HttpContext.User.Identity.Name;

                if (senderUserName == null)
                {
                    return Problem(Resources.LoginErrorMsg);
                }

                if (!ModelState.IsValid)
                {
                    return Problem(Resources.ValidErrorMsg);
                }

                var senderuser = _userService.GetById(senderUserName).Data;

                if (senderuser == null)
                {
                    return Problem(Resources.NotFoundUserMsg);
                }

                var receiveruser = _userService.GetById(model.ReceiverUserName).Data;

                if (receiveruser == null)
                {
                    return Problem(Resources.TransactionUserNotFoundMsg);
                }

                model.SenderUserName = senderuser.UserName;
                model.ReceiverUserName = receiveruser.UserName;

                var blocklist = _blockListService.GetByConditions(x => (x.BlockedUserName == model.SenderUserName && x.HinderingUserName == receiveruser.UserName) || (x.BlockedUserName == receiveruser.UserName && x.HinderingUserName == model.SenderUserName)).Data;

                if (blocklist != null)
                {
                    return Problem(Resources.UnexpectedErrorMsg);
                }

                MessageViewModel message = new MessageViewModel()
                {
                    SenderUserName = senderUserName,
                    Message = model.Message,
                    MessageID = Guid.NewGuid(),
                    SendDate = DateTime.Now,
                };

                message.ReceiverUserName = receiveruser.UserName;

                var result = _messageService.Add(message);

                if (!result.IsSuccess)
                {
                    return Problem(Resources.SaveErrorMsg);
                }

                return Ok(Resources.SuccessMsg);
            }
            catch (Exception ex)
            {
                return Problem(Resources.UnexpectedErrorMsg + ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult AddBlockUser(BlockListViewModel model)
        {
            try
            {
                var hinderingUserName = HttpContext.User.Identity.Name;

                if (hinderingUserName == null)
                {
                    return Problem(Resources.LoginErrorMsg);
                }

                if (!ModelState.IsValid)
                {
                    return Problem(Resources.ValidErrorMsg);
                }

                model.HinderingUserName = hinderingUserName;

                var hinderingUser = _userService.GetById(model.HinderingUserName).Data;

                if (hinderingUser == null)
                {
                    return Problem(Resources.LoginErrorMsg);
                }

                var blockedUser = _userService.GetById(model.BlockedUserName).Data;

                if (blockedUser == null)
                {
                    return Problem(Resources.TransactionUserNotFoundMsg);
                }

                var blocklist = _blockListService.GetByConditions(x => x.HinderingUserName == model.HinderingUserName && x.BlockedUserName == model.BlockedUserName).Data;

                if (blocklist != null)
                {
                    return Problem(Resources.ReplaceDataMsg);
                }

                BlockListViewModel blocklistModel = new BlockListViewModel()
                {
                    BlockID = Guid.NewGuid(),
                    HinderingUserName = hinderingUserName,
                    BlockedUserName = model.BlockedUserName,
                };

                var result = _blockListService.Add(blocklistModel);

                if (!result.IsSuccess)
                {
                    return Problem(Resources.SaveErrorMsg);
                }

                return Ok(Resources.SuccessMsg);
            }
            catch (Exception ex)
            {

                return Problem(Resources.UnexpectedErrorMsg + ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult MessageList()
        {
            var user = HttpContext.User.Identity.Name;

            if (user == null)
            {
                return Problem(Resources.LoginErrorMsg);
            }

            var data = _messageService.GetAll().Data.Where(x => x.SenderUserName == user || x.ReceiverUserName == user).OrderBy(x => x.SendDate).ToList();

            return Ok(data);
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult ActivityLogList()
        {
            var user = HttpContext.User.Identity.Name;

            if (user == null)
            {
                return Problem(Resources.LoginErrorMsg);
            }

            var data = _activityListService.GetAll().Data.Where(x => x.LoginUserName == user).OrderBy(x => x.ActivityDate).ToList();

            return Ok(data);
        }

    }

}
