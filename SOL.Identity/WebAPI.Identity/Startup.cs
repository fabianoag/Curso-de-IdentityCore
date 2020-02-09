using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#region FRAMEWORKS ADICIONADOS
    using WebAPI.Repository;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using WebAPI.Dominio;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using AutoMapper;
    using WebAPI.Identity.Helper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Authorization;
#endregion

namespace WebAPI.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*------------LINHA PADRÃO DO STARTUP-------------
             * OBS: Esta linha será alterada mais a baixo em:
             * 'ESTA LINHA PARA A CONFIGURAÇÃO DA ROTA DA WEBAPI'
             */
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region  ==AREA DE CONFIGURAÇÃO DO IDENTITY==

            #region CONFIGURAÇÃO DO MIGRATION
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                services.AddDbContext<Context>(
                    opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sql =>
                    sql.MigrationsAssembly(migrationAssembly))
                );
                #endregion

                #region CONFIGURAÇÃO DO IDENTITYCORE
                /* Aqui esta as configuração de acesso do para autenticar o usuário com o identity.*/
                services.AddIdentityCore<User>(options =>
                {
                    //options.SignIn.RequireConfirmedEmail = true;

                    options.Password.RequireDigit = false;// Digito não requirido.
                    options.Password.RequireNonAlphanumeric = false;// Alfanumerico não requirido.
                    options.Password.RequireLowercase = false;// Letras maiuscula não requirido.
                    options.Password.RequireUppercase = false;// Letras menusculas não requirido.
                    options.Password.RequiredLength = 4;// Máximo de 4 digitos

                    options.Lockout.MaxFailedAccessAttempts = 3;// Máximo de tentativas.
                    options.Lockout.AllowedForNewUsers = true;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);// Define o tempo que o usuário será bloqueado.
                })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<Context>()
                .AddRoleValidator<RoleValidator<Role>>()
                .AddRoleManager<RoleManager<Role>>()
                .AddSignInManager<SignInManager<User>>()
                .AddDefaultTokenProviders();
                #endregion

                #region CONFIGURAÇÃO DE AUTENTICAÇÃO DA WEBAPI
                /* Esta parte permite configurar a autenticação do usuário.
                 * O recurso que ele usa e o JWT do 'NuGet Package Manager'
                 * ==> Microsoft.AspNetCore.Authentication.JWTBearer */
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options => {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                                    .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                                ValidateIssuer = false,
                                ValidateAudience = false
                            };
                        });

                 #endregion

                #region ESTA LINHA PARA A CONFIGURAÇÃO DA ROTA DA WEBAPI
                /* Aqui configura a rota da aplicação.
                 * Na linha 'AddJsonOptions' ele esta configurada para não repetir registro encontrados dentro
                 * das entidades.*/
                services.AddMvc(options => {
                    /* Aqui ele criar uma politica para toda a ver
                     * que for chamado uma rota dos métodos.
                     * 
                     * OBS = Esta tipo de recurso trabalha muito bem 
                     * com o identity EF.*/
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling =
                                            Newtonsoft.Json.ReferenceLoopHandling.Ignore);
                #endregion

                #region CONFIGURAÇÃO DO AUTOMAPPER
                /* Aqui fica a configuração do 'AutoMapper' usado para
                 * mapear os campos.*/
                var mappingConfig = new MapperConfiguration(mc=> {
                        mc.AddProfile(new AutoMapperProfile());
                    });
                    IMapper mapper = mappingConfig.CreateMapper();                    
                    services.AddSingleton(mapper);            
                #endregion

                //Permite usar o recurso CORS.
                services.AddCors();
             #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region HABILITA A AUTENTICAÇÃO
            /* Esta linha e usada para habilitar a autenticação de usuario colocada
             * no 'ConfigureServices' adiciona no método 'services.AddMVC().*/
            app.UseAuthentication();
            #endregion

            #region CONFIGURAÇÂO DO CORS
            // Permite qualquer Origem, Métodos ou Cabeçalhos.
            app.UseCors(x => x.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            #endregion

            app.UseMvc();
        }
    }
}
