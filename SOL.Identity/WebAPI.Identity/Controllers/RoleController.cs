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
    /// <summary>
    /// Nesta class e definida os papeis que o usuário tera no sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Método construtor de role que são os papeis.
        /// </summary>
        /// <param name="roleManager">Injected das configurações de papeis.</param>
        /// <param name="userManager">Injected das configurações de usuário.</param>
        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: api/Role
        /// <summary>
        /// Demostrativos dos Dtos. 
        /// </summary>
        /// <returns>Retorna OK.</returns>
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
        /// <summary>
        /// O teste sem função alguma.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        [Authorize(Roles = "GERENTE, VENDEDOR")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Role
        /// <summary>
        /// Cadastra uma Role que e um papel.
        /// </summary>
        /// <param name="roleDto">Recebe o nome da Role</param>
        /// <returns>Retorna o resultado</returns>
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(RoleDto roleDto)
        {
            try
            {
                //Resultado
                var retorno = await _roleManager.CreateAsync( new Role { Name = roleDto.Name});

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                //Erro
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"ERROR: {ex.Message}");
            }
        }

        // PUT: api/Role/5
        /// <summary>
        /// Atualiza os papeis de usuario no sistema.
        /// </summary>
        /// <param name="model">Recebe as atualiza de UserRole.</param>
        /// <returns></returns>
        [HttpPut("UpdateUserRole")]
        public async Task<IActionResult> UpdateUserRoles(UpdateUserRoleDto model)
        {
            try
            {
                //Verifica se usuário existe.
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    //Verifica se delete e igual a 'true'.
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
                //Erro
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
