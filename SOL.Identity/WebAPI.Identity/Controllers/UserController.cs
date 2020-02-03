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
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

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
        //[AllowAnonymous]//Permita acessa este método.
        public IActionResult Get()
        {
            return Ok(new UserDto());
        }

        // GET: api/login/5        
        [HttpPost("Login")]
        [AllowAnonymous]//Permita acessa este método.
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
        [AllowAnonymous]//Permita acessa este método.
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

        private async Task<string> GenerateJWTToken(User user)
        {
            /* Criar */
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Texto", "123sdffe") 
            };

            var Roles = await _userManager.GetRolesAsync(user);

            foreach(var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                _config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
