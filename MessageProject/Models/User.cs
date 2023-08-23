using Microsoft.AspNetCore.Identity;

namespace MessageProject.Models
{
    public class User : IdentityUser
    {
        /// <summary>
        /// 權限設計
        /// </summary>
        public int Permission { get; set; }
        
    }
}
