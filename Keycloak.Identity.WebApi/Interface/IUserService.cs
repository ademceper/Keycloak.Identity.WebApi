using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserService
{
	Task<IEnumerable<UserRepresentation>> GetUsersAsync();
	Task<UserRepresentation> GetUserByIdAsync(string userId);
	Task DeleteUserAsync(string userId);
	Task UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest);
	Task<IEnumerable<UserRepresentation>> GetUsersByParamsAsync(string username = null, string firstName = null, string lastName = null, string email = null);
}
