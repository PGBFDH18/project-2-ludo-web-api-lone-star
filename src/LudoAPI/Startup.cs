using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ludo.API.Service.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ludo.API.Web
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
            services.AddSingleton<Service.ILudoService>(Service.Factory.Create());
            services.AddSingleton<IBoardInfo, CBoardInfo>();
            services.AddSingleton<IBoardState, CBoardState>();
            services.AddSingleton<IConcede, CConcede>();
            services.AddSingleton<ICreateLobby, CCreateLobby>();
            services.AddSingleton<ICreateUser, CCreateUser>();
            services.AddSingleton<IFindUser, CFindUser>();
            services.AddSingleton<IGetCurrent, CGetCurrent>();
            services.AddSingleton<IGetLobby, CGetLobby>();
            services.AddSingleton<IGetPieceInfo, CGetPieceInfo>();
            services.AddSingleton<IGetPlayerReady, CGetPlayerReady>();
            services.AddSingleton<IGetTurnInfo, CGetTurnInfo>();
            services.AddSingleton<IGetUser, CGetUser>();
            services.AddSingleton<IIsKnown, CIsKnown>();
            services.AddSingleton<IJoinLobby, CJoinLobby>();
            services.AddSingleton<ILeaveLobby, CLeaveLobby>();
            services.AddSingleton<IListGames, CListGames>();
            services.AddSingleton<IListLobbies, CListLobbies>();
            services.AddSingleton<IListUsers, CListUsers>();
            services.AddSingleton<IMovePiece, CMovePiece>();
            services.AddSingleton<IPassTurn, CPassTurn>();
            services.AddSingleton<ISlotUser, CSlotUser>();
            services.AddSingleton<IStartGame, CStartGame>();
            services.AddSingleton<IUserNameAcceptable, CUserNameAcceptable>();
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
