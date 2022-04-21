using WebAPITest2.Data;
using WebAPITest2.Models;

namespace WebAPITest2.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(UserDTOLogin userDTOLogin);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);

        Task<List<UserDTO>> FindAll();
        Task<User> FindById(int Id);
        Task<User> Create(User user, int roleId);
        Task<User> Update(int id, UserDTO user);
        Task Delete(int id);
        bool UserExists(int id);
    }
}
