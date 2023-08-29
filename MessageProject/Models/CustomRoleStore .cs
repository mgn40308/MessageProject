using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace MessageProject.Models
{
    public class CustomRoleStore : IRoleStore<CustomRole>
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public CustomRoleStore(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetConnectionString("Default");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateAsync(CustomRole role, CancellationToken cancellationToken)
        {
            //cancellationToken.ThrowIfCancellationRequested();



            //using (var connection = new SqlConnection(connectionString))
            //{

            //    await connection.OpenAsync(cancellationToken);
            //    role.Id = await connection.QuerySingleAsync<string>($@"INSERT INTO [CustomRole] ([Id],[Name])
            //    VALUES (@{Guid.NewGuid().ToString()} @{nameof(CustomRole.Name)});
            //    SELECT CAST(SCOPE_IDENTITY() as varchar(36))", role);
            //}

            //return IdentityResult.Success;
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> DeleteAsync(CustomRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CustomRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normalizedRoleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CustomRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetNormalizedRoleNameAsync(CustomRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetRoleIdAsync(CustomRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetRoleNameAsync(CustomRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="normalizedName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetNormalizedRoleNameAsync(CustomRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="roleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetRoleNameAsync(CustomRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IdentityResult> UpdateAsync(CustomRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
