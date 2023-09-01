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
        private readonly CustomUserManager _customUserManager;
        private readonly CustomSignInManager _customSignInManager;
        private readonly CustomRoleManager _customRoleManager;
        public MemberController(CustomUserManager customUserManager, CustomSignInManager customSignInManager, CustomRoleManager customRoleManager)
        {
            _customUserManager = customUserManager;
            _customSignInManager = customSignInManager;
            _customRoleManager = customRoleManager;
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
        [Authorize(policy: "RequireLoggedIn")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// 設置管理者頁面
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> GetRegisterAccountAsync([FromBody] UserAccount userAccount)
        {
            string userName = userAccount.UserName;
            string password = userAccount.Password;
            string pwd = MemberModel.EncryptionPassword(password);
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                Models.User user = new()
                {
                    UserName = userName,
                    Permission = 1,
                    PasswordHash = pwd
                };
                //加密密碼
                var result = await _customUserManager.CreateAsync(user);

                //var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Member");
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
                var result = await _customSignInManager.PasswordSignInAsync (userName, enycrtPassword, false,lockoutOnFailure: false);
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
                var user = await _customUserManager.FindByNameAsync(id);
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

            await _customSignInManager.SignOutAsync();
            return RedirectToAction("Login", "Member"); 
        }

        /// <summary>
        /// 修改密碼功能 -- 密碼會加密後才儲存
        /// </summary>
        /// <param name="form">前端form表單傳回應有</param>
        /// currentpassword 舊的使用者密碼
        /// newpassword  新的使用者密碼
        /// <returns></returns>
        public async Task<IActionResult> UpdatePasswordAsync([FromBody]ChangePassword changePassword)
        {
            string userName = User.Identity.Name;
            string currentPassword = changePassword.CurrentPassword;
            string enycrtCurrentPassword= MemberModel.EncryptionPassword(currentPassword);
            string newPassword = changePassword.NewPassword;
            string enycrtNewPassword = MemberModel.EncryptionPassword(newPassword);

            var user = await _customUserManager.FindByNameAsync(userName);
            
            if (user == null)
            {
                return RedirectToAction("ChangePassword", "Member");
            }
            else
            {
               var result= await _customUserManager.ChangePasswordAsync(user, enycrtCurrentPassword, enycrtNewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("List", "Message");
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
            var user = await _customUserManager.FindByNameAsync(userName);

            if (user != null)
            {
                // 查找管理員角色
                var adminRoleExists = await _customRoleManager.RoleExistsAsync("Admin");
                if (!adminRoleExists)
                {
                    await _customRoleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // 將使用者加到管理員
                await _customUserManager.AddToRoleAsync(user, "Admin");

                return RedirectToAction("List", "Message");
            }

            return View("Error");
        }

        public  bool? CheckIsAuthorize()
        {
            return User.Identity?.IsAuthenticated;
        }

        public bool? CheckIsAdmin()
        {
            return User.IsInRole("Admin");
        }




    }
}
