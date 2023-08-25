using Microsoft.AspNetCore.Mvc;
using MessageProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MessageProject.Controllers
{
   
    public class MemberController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public MemberController(UserManager<User> userManager, SignInManager<User> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 登入頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 註冊頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            return View();
        }
        /// <summary>
        /// 更改密碼頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// 設置管理者頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult AddAdmin()
        {
            return View();
        }
        /// <summary>
        /// 註冊帳號
        /// </summary>
        /// <param name="form">前端form表單傳回應有 </param>
        /// username 使用者帳號
        /// password 使用者密碼
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetRegisterAccountAsync(IFormCollection form)
        {
            string userName = form["username"];
            string password = form["password"];
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                Models.User user = new()
                {
                    UserName = userName,
                    Permission = 1,

                };
                //加密密碼
                string pwd = MemberModel.EncryptionPassword(password);
                var result = await _userManager.CreateAsync(user, pwd);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        
                        return View();
                    }
                }               
            }
            return View("Register");
        }

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="form">前端form表單傳回應有</param>
        /// username 使用者帳號
        /// password 使用者密碼
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetLoginAccount(IFormCollection form)
        {
            string userName = form["username"];
            string password = form["password"];
            //加密密碼
            string enycrtPassword = MemberModel.EncryptionPassword(password);
            if (!string.IsNullOrEmpty(userName)&& !string.IsNullOrEmpty(password))
            {
                var result = await _signInManager.PasswordSignInAsync (userName, enycrtPassword, false,lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("List", "Message");
                }
                ModelState.AddModelError(string.Empty, "登入失敗");
            }
            return RedirectToAction("Login", "Member");
        }

        /// <summary>
        /// 檢查使用者帳號是否重複
        /// </summary>
        /// <param name="id">使用者帳號</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckUserExistsAsync(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByNameAsync(id);
                if (user == null)
                {
                    return Json(false);
                }
                else
                {
                    return Json(true);
                }
            }
            else
            {
                return Json(false);
            }
        }

        /// <summary>
        /// 登出功能
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Member"); 
        }

        /// <summary>
        /// 修改密碼功能 -- 密碼會加密後才儲存
        /// </summary>
        /// <param name="form">前端form表單傳回應有</param>
        /// currentpassword 舊的使用者密碼
        /// newpassword  新的使用者密碼
        /// <returns></returns>
        [Authorize(Policy = "RequireLoggedIn")]
        public async Task<IActionResult> UpdatePasswordAsync(IFormCollection form)
        {
            string userName = User.Identity.Name;
            string currentPassword = form["currentpassword"];
            string enycrtCurrentPassword= MemberModel.EncryptionPassword(currentPassword);
            string newPassword = form["newpassword"];
            string enycrtNewPassword = MemberModel.EncryptionPassword(newPassword);

            User user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return RedirectToAction("ChangePassword", "Member");
            }
            else
            {
               var result= await _userManager.ChangePasswordAsync(user, enycrtCurrentPassword, enycrtNewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Member");
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Member");
                }
            }
        }

        /// <summary>
        /// 將使用者設定成管理者角色
        /// </summary>
        /// <param name="form">前端form表單傳回應有</param>
        /// username 使用者帳號
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToAdminRole(IFormCollection form)
        {
            string userName = form["username"];
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                // 查找管理員角色
                var adminRoleExists = await _roleManager.RoleExistsAsync("Admin");
                if (!adminRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // 將使用者加到管理員
                await _userManager.AddToRoleAsync(user, "Admin");

                return View("Success");
            }

            return View("Error");
        }



    }
}
