namespace MessageProject.Models
{
    public class UserAccount
    {
        public string UserName { get; set; }
        public string Password { get; set; }    
    }

    public class ChangePassword
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
