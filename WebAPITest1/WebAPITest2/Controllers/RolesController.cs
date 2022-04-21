#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPITest2.Data;
using WebAPITest2.Helpers;
using WebAPITest2.Models;
using WebAPITest2.Enums;
using WebAPITest2.Services;

namespace WebAPITest2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly WebAPITest2Context _context;
        private readonly IRoleService roleService;

        public RolesController(WebAPITest2Context context, IRoleService roleService)
        {
            _context = context;
            this.roleService = roleService;
        }


        // GET: api/Roles
        [Authorize(RoleEnum.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            return await roleService.FindAll();
        }

        // GET: api/Roles/5
        [Authorize(RoleEnum.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRole(int id)
        {
            var role = await roleService.FindById(id);

            if (role == null)
            {
                return NotFound();
            }

            return roleToDTO(role);
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(RoleEnum.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleDTO role)
        {
            if (id != role.Id)
            {
                return BadRequest();
            }
            if (!roleService.RoleExists(id))
            {
                return NotFound();
            }

            await roleService.Update(id, role);
            return NoContent();
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(RoleEnum.Admin)]
        [HttpPost]
        public async Task<ActionResult<RoleDTO>> PostRole(RoleDTO roleDTO)
        {
            var role = new Role
            {
                Name = roleDTO.Name
            };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        // DELETE: api/Roles/5
        [Authorize(RoleEnum.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }

        private static RoleDTO roleToDTO(Role role) =>
            new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
            };
    }
}
