using WebAPITest2.Data;
using WebAPITest2.Models;

namespace WebAPITest2.Services
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> FindAll();
        Task<Role> FindById(int Id);
        Role FindByUserId(int Id);

        Task<Role> Create(Role role);
        Task<Role> Update(int Id, RoleDTO role);
        Task Delete(int id);
        bool RoleExists(int id);
    }
}
