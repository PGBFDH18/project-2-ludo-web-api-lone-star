using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ludo.WebAPI
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<GameService.ILudoService, GameService.LudoService>();

            services.AddSingleton<Components.IBoardInfo, Components.CBoardInfo>();
            services.AddSingleton<Components.IBoardState, Components.CBoardState>();
            services.AddSingleton<Components.ICreateLobby, Components.CCreateLobby>();
            services.AddSingleton<Components.ICreateUser, Components.CCreateUser>();
            services.AddSingleton<Components.IFindUser, Components.CFindUser>();
            services.AddSingleton<Components.IGetLobby, Components.CGetLobby>();
            services.AddSingleton<Components.IGetPlayerReady, Components.CGetPlayerReady>();
            services.AddSingleton<Components.IGetUser, Components.CGetUser>();
            services.AddSingleton<Components.IIsKnown, Components.CIsKnown>();
            services.AddSingleton<Components.IJoinLobby, Components.CJoinLobby>();
            services.AddSingleton<Components.ILeaveLobby, Components.CLeaveLobby>();
            services.AddSingleton<Components.IListLobbies, Components.CListLobbies>();
            services.AddSingleton<Components.IListUsers, Components.CListUsers>();
            services.AddSingleton<Components.ISlotUser, Components.CSlotUser>();
            services.AddSingleton<Components.IStartGame, Components.CStartGameMock>(); // <--- FIXME! TODO
            services.AddSingleton<Components.IUserNameAcceptable, Components.CUserNameAcceptable>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
