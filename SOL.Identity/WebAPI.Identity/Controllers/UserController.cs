using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

#region FRAMEWORKS ADICIONADOS
    using Microsoft.EntityFrameworkCore;
    using System.IdentityModel.Tokens.Jwt;
    using AutoMapper;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using WebAPI.Dominio;
    using WebAPI.Identity.Dto;
#endregion

namespace WebAPI.Identity.Controllers
{
    /// <summary>
    /// Nesta class e definida o controle o usuário.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Método construtor de user que são os usuários.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="mapper"></param>
        public UserController(IConfiguration config,
                              UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IMapper mapper)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }


        // GET: api/User
        [HttpGet]
        //[AllowAnonymous]//Permita acessa de qualquer usuário.
        public IActionResult Get()
        {
            return Ok(new UserDto());
        }

        // GET: api/login/5        
        [HttpPost("Login")]
        [AllowAnonymous]//Permita acessa de qualquer usuário.
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                /*Faz consulta por userName de usuário.*/
                var user = await _userManager.FindByNameAsync(userLogin.UserName);
                /* Verifica se existe retorno de usuário, se for igual a nulo retorna a 
                 * mensagem abaixo*/
                if (user == null) return Unauthorized("Usuário ou senha errada!");
                /* Verifica se a senha do PasswordHash e igual a senha passada */
                var result = await _signInManager
                    .CheckPasswordSignInAsync(user, userLogin.Password, false);

                /* Verifica se houve resultado.*/
                if (result.Succeeded)
                {
                    /* Verifica se o NormalizedUserName e igual ao UserName com letras maiuscula.*/
                    var appUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName.ToUpper());

                    /* Mapeia o retorno do appUser.*/
                    var userToReturn = _mapper.Map<UserDto>(appUser);

                    return Ok(new
                    {
                        token = GenerateJWTToken(appUser).Result,
                        user = userToReturn
                    });
                }
                return Unauthorized();
             }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"ERROR{ex.Message}");
            }
        }

        // POST: api/Register
        [HttpPost("Register")]
        [AllowAnonymous]//Permita acessa de qualquer usuário.
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                /* Pesquisa o usuario por UserName.*/
                var user = await _userManager.FindByNameAsync(userDto.UserName);

                /* Verifica se o usuário existe no banco de dados.*/
                if (user == null)
                {
                    /* Preenche a variavel com os dados passados pelo usuário.*/
                    user = new User
                    {
                        UserName = userDto.UserName,
                        Email = userDto.UserName,
                        NomeCompleto = userDto.NomeCompleto
                    };

                    /* Registra o usuário no banco de dados.*/
                    var result = await _userManager.CreateAsync(user, userDto.Password);


                    if (result.Succeeded)
                    {
                        var appUser = await _userManager.Users
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName.ToUpper());

                        /* Gerar token JWT*/
                        var token = GenerateJWTToken(appUser).Result;

                        /* ==> Código abaixo não utilizado. <==*/
                        //var confirmationEmail = Url.Action("ConfirmEmailAddress", "Home",
                        //    new { token = token, email = user.Email }, Request.Scheme);
                        //System.IO.File.WriteAllText("confirmationEmail.txt", confirmationEmail);

                        /* Retornar token.*/
                        return Ok(token);
                    }

                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"ERROR{ex.Message}");
            }
        }

        /// <summary>
        /// Método usado para gerar um token de usuário.
        /// </summary>
        /// <param name="user">Recebe um usuário</param>
        /// <returns>Retorna um token.</returns>
        private async Task<string> GenerateJWTToken(User user)
        {
            /* Criando uma lista de claim para adiciona no token.*/
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Texto", "123sdffe") 
            };

            // Pesquisar as roles do usuários.
            var Roles = await _userManager.GetRolesAsync(user);

            // Listar roles.
            foreach(var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Pegar a chave em "AppSettings:Token".
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                _config.GetSection("AppSettings:Token").Value));

            //Cria um credencial com a chave.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            //Opcões de configuração do token criado.
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Instância do "JwtSecurityTokenHandler()".
            var tokenHandler = new JwtSecurityTokenHandler();

            //Cria o token.
            var token = tokenHandler.CreateToken(tokenDescription);            

            //Retorna o token escrito.
            return tokenHandler.WriteToken(token);
        }

        // PUT: api/User/UpdateUser/5
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto userDto)
        {
            try
            {
                /* Pesquisa o usuario por UserName.*/
                var user = await _userManager.FindByIdAsync(id.ToString());


                /*===> OBS: A alteração de senha tem que ser feita e um método separado, nela inclui <===
                       - Usuário.
                       - Senha atual.
                       - Nova senha.                 
                  ===> var result = await _userManager.ChangePasswordAsync(user, userDto.CurrentPassword, userDto.NewPassword); <===
                */


                /* Verifica se o usuário existe no banco de dados.*/
                if (user != null)
                {
                    user.UserName = userDto.UserName;
                    user.Email = userDto.UserName;
                    user.NomeCompleto = userDto.NomeCompleto;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        var appUser = await _userManager.Users
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName.ToUpper());

                        /* Gerar token JWT*/
                        var token = GenerateJWTToken(appUser).Result;

                        /* ==> Código abaixo não utilizado. <== */
                        //var confirmationEmail = Url.Action("ConfirmEmailAddress", "Home",
                        //    new { token = token, email = user.Email }, Request.Scheme);
                        //System.IO.File.WriteAllText("confirmationEmail.txt", confirmationEmail);

                        /* Retornar token.*/
                        return Ok(token);
                    }
                }
                return NotFound("Usuário não encontrado");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                        $"ERROR{ex.Message}");
            }
        }

        // PUT: api/User/UpdatePasswordUser/5
        [HttpPut("UpdatePasswordUser/{id}")]
        public async Task<IActionResult> UpdatePasswordUser(int id, UpdatePasswordUserDto userDto)
        {
            try
            {
                /* Pesquisa o usuario por UserName.*/
                var user = await _userManager.FindByIdAsync(id.ToString());

                /* Verifica se o usuário existe no banco de dados.*/
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, userDto.CurrentPassword, userDto.NewPassword);

                    if (result.Succeeded)
                    {                        
                        return Ok("Password Alterado com sucesso.");
                    }
                }
                return NotFound("Usuário não encontrado");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                        $"ERROR{ex.Message}");
            }
        }


        // DELETE: api/DeleteUser
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> Delete(DeleteUserDto dUser)
        {
            /* OBS: Deleta só pelo id não e a forma correta. mais é só um exemplo. */
            try
            {
                /* Pesquisa o usuario por UserName.*/

                var user = await _userManager.FindByIdAsync(dUser.Id.ToString());

                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);

                    if(result.Succeeded)
                    {
                        return Ok("Usuário deletado com sucesso.");
                    }
                }

                return NotFound("Usuário não encontrado");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                        $"ERROR{ex.Message}");
            }

        }
    }
}
