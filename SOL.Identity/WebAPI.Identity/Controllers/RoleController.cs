using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
#region FRAMEWORK USADOS
using WebAPI.Dominio;
using Microsoft.AspNetCore.Identity;
using WebAPI.Identity.Dto;
using Microsoft.AspNetCore.Authorization;
#endregion

namespace WebAPI.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: api/Role
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Get()
        {
            return Ok( new { 
                role = new RoleDto(),
                updateUserRoleDto = new UpdateUserRoleDto()
            });
        }

        // GET: api/Role/5
        [HttpGet("{id}", Name = "Get")]
        [Authorize(Roles = "GERENTE, VENDEDOR")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Role
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(RoleDto roleDto)
        {
            try
            {
                var retorno = await _roleManager.CreateAsync( new Role { Name = roleDto.Name});

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"ERROR: {ex.Message}");
            }
        }

        // PUT: api/Role/5
        [HttpPut("UpdateUserRole")]
        public async Task<IActionResult> UpdateUserRoles(UpdateUserRoleDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (model.Delete)
                        await _userManager.RemoveFromRoleAsync(user, model.Role);
                    else
                        await _userManager.AddToRoleAsync(user, model.Role);
                }
                else
                {
                    return NotFound("Usuário não encontrado.");
                }


                return Ok("Sucesso");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"ERROR: {ex.Message}");
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
