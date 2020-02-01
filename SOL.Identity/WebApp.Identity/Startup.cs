using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;// AULA 18
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Identity
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region AREA DE CONFIGURAÇÃO DO IDENTITY
            // AULA 18 - INICIO
            var connectionString = @"Password=teste1239;Persist Security Info=True;User ID=sa;Initial Catalog=IdentityCurso;Data Source=fabiano";
            // Passando o nome para configura a migrations
            var migrationAssembly = typeof(Startup)
                .GetTypeInfo().Assembly
                .GetName().Name;

            services.AddDbContext<MyUserDbContext>(
                opt => opt.UseSqlServer(connectionString,sql =>
                sql.MigrationsAssembly(migrationAssembly))
            );
            // AULA 18 - FIM

            //Configurando o IdentityCore - INICIO
            services.AddIdentity<MyUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;

                options.Password.RequireDigit = false;// Digito não requirido.
                options.Password.RequireNonAlphanumeric = false;// Alfanumerico não requirido.
                options.Password.RequireLowercase = false;// Letras maiuscula não requirido.
                options.Password.RequireUppercase = false;// Letras menusculas não requirido.
                options.Password.RequiredLength = 4;// Máximo de 4 digitos

                options.Lockout.MaxFailedAccessAttempts = 3;// Máximo de tentativas.
                options.Lockout.AllowedForNewUsers = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);// Define o tempo que o usuário será bloqueado.

            })
                .AddEntityFrameworkStores<MyUserDbContext>()
                .AddDefaultTokenProviders()
                .AddPasswordValidator<NaoContemValidadorSenha<MyUser>>();
            //Configurando o IdentityCore - FIM

            services.AddScoped<IUserClaimsPrincipalFactory<MyUser>, MyUserClaimsPrincipalFactory>();

            services.Configure<DataProtectionTokenProviderOptions>(
                  options => options.TokenLifespan = TimeSpan.FromHours(3)    
            );

            //AULA 23 - Especifica o uso de cookies - INCIO
            services.ConfigureApplicationCookie(options => 
                options.LoginPath = "/Home/Login"
            );
            //AULA 23 - FIM
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //AULA 15 - Especifica o uso de cookies - INCIO
            app.UseAuthentication();
            //AULA 15 - FIM

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
