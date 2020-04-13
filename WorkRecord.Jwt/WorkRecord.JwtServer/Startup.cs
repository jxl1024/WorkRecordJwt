using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WordRecord.IRepository.Repository;
using WordRecord.Repository.Repositories;
using WorkRecord.Data.Context;
using WorkRecord.IService.Service;
using WorkRecord.JwtServer.Jwt;
using WorkRecord.Model.Jwt;
using WorkRecord.Service.Service;

namespace WorkRecord.JwtServer
{
    public class Startup
    {
        // 全局跨域策略
        readonly string MyAllowSpecificOrigins = "MyAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 添加跨域服务
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    // 这里设置的是允许所有的跨域，允许访问Get、Post、Put、Delete方法
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            #endregion


            #region 数据库链接
            string connectionString = Configuration.GetSection("ConnectionString").GetSection("DbConnection").Value;
            services.AddDbContext<AppDbContext>(options =>
            {

                options.UseSqlServer(connectionString);
            }); 
            #endregion

            #region 服务注入
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<ITokenHelper, TokenHelper>();
            #endregion

            #region 身份认证
            // 读取appsettings.json文件
            services.Configure<JWTConfig>(Configuration.GetSection("JWT"));
            // 启用JWT认证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer();
            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            app.UseRouting();

            #region 启用跨域中间件
            // 启用跨域中间件
            app.UseCors(MyAllowSpecificOrigins);
            #endregion

            // 启用认证中间件
            app.UseAuthentication();
            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
