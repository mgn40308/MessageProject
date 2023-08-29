using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace MessageProject.Models
{
    public class CustomRoleManager : RoleManager<IdentityRole>
    {
        private readonly IDbConnection _connection;
        public CustomRoleManager(IRoleStore<IdentityRole> store, IEnumerable<IRoleValidator<IdentityRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<IdentityRole>> logger , IDbConnection connection) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _connection = connection;
        }
        public override async Task<bool> RoleExistsAsync(string roleName)
        {

            string query = "SELECT Id FROM AspNetRoles WHERE NormalizedName=@NormalizedName";
            var param = new DynamicParameters();
            param.Add("@NormalizedName", roleName.ToUpper());
            var result= await _connection.QueryFirstOrDefaultAsync<CustomRole>(query, param,commandType: CommandType.Text);


            return result != null;
        }

        public async Task<IdentityResult> CreateAsync(CustomRole role)
        {
            string query = @"INSERT AspNetRoles (Id,Name,NormalizedName,ConcurrencyStamp)
                            Values(@Id,@Name,NormalizedName,ConcurrencyStamp)";
            var param = new DynamicParameters();
            param.Add("@Id",Guid.NewGuid().ToString());
            param.Add("@Name", role.Name);
            param.Add("@NormalizedName", role.Name.ToUpper());
            param.Add("@ConcurrencyStamp", role.ConcurrencyStamp);
            var result=  _connection.ExecuteAsync(query, param, commandType:CommandType.Text);
            if(result != null)
            {
                return IdentityResult.Success;
            }
           
            return IdentityResult.Failed();
        }
    }
}
