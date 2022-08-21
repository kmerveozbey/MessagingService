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
                    return Problem("Giriş bilgilerini eksiksiz doldurunuz.");
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
                    return Problem("Bu kullanıcı adı veya e-mail adresiyle ile sistemde kayıt mevcuttur.");
                }

                string md5password = ProcessForPassword.MD5Hash(model.Password);
                user.Password = md5password;

                var result = _userService.Add(user);
                if (!result.IsSuccess)
                {
                    return Problem("Kayıt sırasında bir hata oluştu.");
                }
                return Ok(result.Message);
            }
            catch (Exception ex)
            {
                return Problem("Beklenmedik bir hata oldu - " + ex.Message);
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
                    return Problem("Giriş bilgilerini eksiksiz doldurunuz.");
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
                    var getUserName = _userService.GetByConditions(x => x.UserName == model.UserName).Data;
                    if (getUserName != null)
                    {
                        ActivityLogListViewModel activityLogsError = new ActivityLogListViewModel()
                        {
                            ActivityID = Guid.NewGuid(),
                            LoginUserName = model.UserName,
                            ActivityDate = DateTime.Now,
                            Status = "Login Error",
                        };
                        var resultActivityLogs = _activityListService.Add(activityLogsError);
                        if (!resultActivityLogs.IsSuccess)
                        {
                            return Problem("Beklenmeyen bir hata oluştu.");
                        }
                    }

                    return Problem("Sistemde kullanıcı bilgisi bulunamadı.");
                }
                if (login.RememberMe)
                {
                    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, login.UserName));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                }
                ActivityLogListViewModel activityLogs = new ActivityLogListViewModel()
                {
                    ActivityID = Guid.NewGuid(),
                    LoginUserName = user.UserName,
                    ActivityDate = DateTime.Now,
                    Status = "Login Success",
                };
                var result = _activityListService.Add(activityLogs);
                if (!result.IsSuccess)
                {
                    return Problem("Beklenmeyen bir hata oluştu.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem("Beklenmedik bir hata oldu - " + ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/[action]")]

        public IActionResult RememberMeTestResult()
        {
            var userName = HttpContext.User.Identity.Name;
            var getUserName = _userService.GetByConditions(x => x.UserName == userName).Data;
            if (getUserName == null)
            {
                return Problem("Beklenmeyen bir hata oluştu. Tekrar giriş yapınız.");
            }
            if (getUserName != null)
            {
                ActivityLogListViewModel activityLogs = new ActivityLogListViewModel()
                {
                    ActivityID = Guid.NewGuid(),
                    LoginUserName = userName,
                    ActivityDate = DateTime.Now,
                    Status = "Login Success",

                };
                var result = _activityListService.Add(activityLogs);
                if (!result.IsSuccess)
                {
                    return Problem("Beklenmeyen bir hata oluştu.");
                }
            }
            return Ok("Giriş Başarılı");
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult Logout()
        {
            var userName = HttpContext.User.Identity.Name;
            var getUserName = _userService.GetByConditions(x => x.UserName == userName).Data;
            if (getUserName == null)
            {
                return Problem("Beklenmeyen bir hata oluştu. Tekrar giriş yapınız.");
            }
            if (getUserName != null)
            {
                ActivityLogListViewModel activityLogs = new ActivityLogListViewModel()
                {
                    ActivityID = Guid.NewGuid(),
                    LoginUserName = userName,
                    ActivityDate = DateTime.Now,
                    Status = "Logout Success",
                };
                var result = _activityListService.Add(activityLogs);
                if (!result.IsSuccess)
                {
                    return Problem("Beklenmeyen bir hata oluştu.");
                }
            }
            HttpContext.SignOutAsync();
            return Ok();
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
                    return Problem("Lütfen giriş yapınız.");
                }
                if (!ModelState.IsValid)
                {
                    return Problem("Mesaj bilgilerini eksiksiz giriniz.");
                }                
                var senderuser = _userService.GetById(senderUserName).Data;
                if (senderuser == null)
                {
                    return Problem("Sistemde kullanıcı bilgisi bulunamadı.");
                }
                var receiveruser = _userService.GetById(model.ReceiverUserName).Data;
                if (receiveruser == null)
                {
                    return Problem("Mesaj gönderilecek kullanıcı sistemde kayıtlı değil.");
                }
                model.SenderUserName = senderuser.UserName;
                model.ReceiverUserName = receiveruser.UserName;
                /*engellenen listesinde ekli mi bakılacak*/
                var blocklist = _blockListService.GetByConditions(x => x.BlockedUserName == model.SenderUserName && x.HinderingUserName == receiveruser.UserName).Data;
                if (blocklist != null)
                {
                    return Problem("Mesajınız iletilemedi.");
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
                    return Problem("Mesaj gönderilirken bir hata oluştu.");
                }
                return Ok("Mesaj gönderildi.");
            }
            catch (Exception ex)
            {
                return Problem("Beklenmedik bir hata oldu - " + ex.Message);
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
                    return Problem("Lütfen giriş yapınız.");
                }
                if (!ModelState.IsValid)
                {
                    return Problem("Kullanıcıyı engellemek için bilgileri eksiksik giriniz.");
                }
                model.HinderingUserName = hinderingUserName;
                var hinderingUser = _userService.GetById(model.HinderingUserName).Data;
                if (hinderingUser == null)
                {
                    return Problem("Lütfen giriş yapınız.");
                }
                var blockedUser = _userService.GetById(model.BlockedUserName).Data;
                if (blockedUser == null)
                {
                    return Problem("Engellenecek kullanıcı sistemde kayıtlı değil.");
                }
                var blocklist = _blockListService.GetByConditions(x => x.HinderingUserName == model.HinderingUserName && x.BlockedUserName == model.BlockedUserName).Data;
                if (blocklist != null)
                {
                    return Problem("Kullanıcıyı daha önceden engellenmiştir.");

                }
                BlockListViewModel blocklistModel = new BlockListViewModel()
                {
                    BlockID = Guid.NewGuid(),
                    HinderingUserName = hinderingUserName,
                    BlockedUserName = model.BlockedUserName,
                };
                //blocklistModel.HinderUser = hinderingUser;
                //blocklistModel.BlockedUser = blockedUser;
                var result = _blockListService.Add(blocklistModel);
                if (!result.IsSuccess)
                {
                    return Problem("Mesaj gönderilirken bir hata oluştu.");
                }
                return Ok("Kullanıcı engellendi.");
            }
            catch (Exception ex)
            {

                return Problem("Beklenmedik bir hata oldu - " + ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult MessageList()
        {
            var senderUserName = HttpContext.User.Identity.Name;
            if (senderUserName == null)
            {
                return Problem("Lütfen giriş yapınız.");
            }
            var data = _messageService.GetAll().Data.Where(x => x.SenderUserName == senderUserName || x.ReceiverUserName == senderUserName).OrderBy(x => x.SendDate).ToList();
            return Ok(data);
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult ActivityLogList()
        {
            var userName = HttpContext.User.Identity.Name;
            if (userName == null)
            {
                return Problem("Lütfen giriş yapınız.");
            }
            var data = _activityListService.GetAll().Data.Where(x => x.LoginUserName == userName).OrderBy(x => x.ActivityDate).ToList();
            return Ok(data);
        }

    }

}
