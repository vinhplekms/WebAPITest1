using Microsoft.EntityFrameworkCore;
using WebAPITest2.Data;
using WebAPITest2.Models;

namespace WebAPITest2.Services
{
    public class RoleService : IRoleService
    {
        private WebAPITest2Context _context;
        private readonly IConfiguration _configuration;

        public RoleService(WebAPITest2Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        public async Task<List<RoleDTO>> FindAll()
        {
            return await _context.Roles.Select(u => roleToDTO(u)).ToListAsync();
        }

        public async Task<Role> FindById(int Id)
        {
           return await _context.Roles.FindAsync(Id);
        }

        public async Task<Role> Create(Role role)
        {
            await _context.Roles.AddAsync(role);
             await _context.SaveChangesAsync();
            return role;
        }

        

        public async Task<Role> Update(int Id, RoleDTO Role)
        {
            var role = _context.Roles.Find(Id);
            role.Name = Role.Name;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return role;
        }

        public async Task Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

        }

        public bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }

        private static RoleDTO roleToDTO(Role role) =>
           new RoleDTO
           {
               Id = role.Id,
               Name = role.Name,
           };

        public Role FindByUserId(int userId)
        {
            return _context.Roles.SingleOrDefault(r => r.Id == _context.UserRoles.SingleOrDefault(u => u.UserId == userId).RoleId);
        }
    }
}
