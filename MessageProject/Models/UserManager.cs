using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MessageProject.Models
{
    public class CustomUserManager : UserManager<User> 
    {
        private readonly IDbConnection _connection;

        public CustomUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger, IDbConnection connection)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _connection = connection;
        }

        // 重寫 CreateAsync 方法
        public override async Task<IdentityResult> CreateAsync(User user)
        {
            string storedProcedure = "usp_AspNetUsers_Add";

            var param = new DynamicParameters();
            param.Add("p_UserName", user.UserName);
            param.Add("p_Permission", user.Permission);
            param.Add("p_PasswordHash", user.PasswordHash);

            int result = await _connection.ExecuteAsync(storedProcedure, param, commandType: CommandType.StoredProcedure);

            if(result==1)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CreateUserFailed",
                    Description = "Failed to create user." 
                });
            }


            
        }
        public override async Task<User> FindByNameAsync(string userName)
        {
            userName= userName.ToUpper();

            var query = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @UserName";
            var param= new DynamicParameters();
            param.Add("@UserName", userName);
            var user = await _connection.QuerySingleOrDefaultAsync<User>(query, param,commandType: CommandType.Text);
            return user;
        }

        public override async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            if(user.PasswordHash==currentPassword) 
            {
                string query = "Update AspNetUsers SET passwordHash = @passwordHash Where Id=@Id";
                var param = new DynamicParameters();
                param.Add("@Id", user.Id);
                param.Add("@passwordHash", newPassword);
                var result=  _connection.Execute(query, param, commandType: CommandType.Text);
                if (result > 0)
                {
                    return IdentityResult.Success;
                }

            }
            return IdentityResult.Failed(ErrorDescriber.PasswordMismatch());

        }

        public virtual async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            string storedProcedure = "usp_AspNetUserRoles_Add";

            var param = new DynamicParameters();
            param.Add("p_UserId", user.Id);
            param.Add("p_RoleName", roleName);

            int result = await _connection.ExecuteScalarAsync<int>(storedProcedure, param, commandType: CommandType.StoredProcedure);

            if (result == 1)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
            
        }
    }
}
