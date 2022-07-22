using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using DotNetCoreServer.Common;
using Opw.HttpExceptions.AspNetCore;
using Server.NetCore.Commons;
using RabbitMQ.Client;
//using Newtonsoft.Json.Serialization;
//using Microsoft.OpenApi.Models;


namespace DotNetCoreServer
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
            //services.AddCors(options =>
            //   options.AddPolicy("any",
            //   builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().AllowCredentials()));

            services.AddControllers();

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v2", new OpenApiInfo { Title = "DotNetCoreServer", Version = "v2" });
            //});

            services.AddCors(options =>
            options.AddPolicy("any", builder =>
            builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()));

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(WebApiResultMiddleware));
                options.RespectBrowserAcceptHeader = true;
                options.Filters.Add<CustomExceptionAttribute>();
                //请求过滤，主要用来写日志
                options.Filters.Add<QueryRequiredAttribute>();
                //鉴权
                options.Filters.Add<TokenFilterAttribute>();
            });
            services.AddMvc().AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

            //鉴权
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateActor = true,
                        ValidIssuer = "Security:Tokens:Issuer",
                        ValidateAudience = true,
                        ValidAudience = "Security:Tokens:Audience",
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Security:Tokens:Key")),
                        //ValidateLifetime = true,
                        //ClockSkew = TimeSpan.FromSeconds(1)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            var backload = JsonConvert.SerializeObject(new BaseResultModel(200, "Token已失效，请重新登录！"));
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = 200;
                            context.Response.WriteAsync(backload);
                            return Task.FromResult(0);
                        },
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Headers["Authorization"];
                            context.Token = token.FirstOrDefault();
                            if (string.IsNullOrEmpty(context.Token))
                            {
                                return Task.CompletedTask;
                            }
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var jt = tokenHandler.ReadJwtToken(context.Token);
                            var validTo = jt.ValidTo;
                            if (validTo < DateTime.UtcNow)
                            {
                                var backload = JsonConvert.SerializeObject(new BaseResultModel(200, "Token已失效，请重新登录！"));
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = 200;
                                context.Response.WriteAsync(backload);
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Swagger", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                           new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });

            services.AddSignalR();
            services.AddCors(options =>
            {
                //登录用户使用
                options.AddPolicy("any", builder =>
                {
                    builder.SetIsOriginAllowed(origin => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
                //公开使用
                options.AddPolicy("all", builder =>
                {
                    builder.WithOrigins("*")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            services.AddSignalR(options =>
            {
                options.MaximumReceiveMessageSize = null;
            });

            //redis
            var section = Configuration.GetSection("Redis:Default");
            var _connectionString = SecretHelper.DESDecrypt(section.GetSection("Connection").Value);
            var redisInstanceName = section.GetSection("InstanceName").Value;
            int defaultDb = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
            services.AddSingleton(new RedisHelper(_connectionString, redisInstanceName, defaultDb));

            services.AddControllers().AddHttpExceptions();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            section = Configuration.GetSection("RabbitMQ:Default");
            var host = SecretHelper.DESDecrypt(section.GetSection("Host").Value);
            var user = SecretHelper.DESDecrypt(section.GetSection("User").Value);
            var password = SecretHelper.DESDecrypt(section.GetSection("Password").Value);
            //services.AddRabbitMQ(new ConnectionFactory//创建连接工厂对象
            //{
            //    HostName = host,//IP地址
            //    Port = 5672,//端口号
            //    UserName = user,//用户账号
            //    Password = password//用户密码
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "DotNetCoreServer"));
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "server.v1");
            });

            app.UseHttpsRedirection();
            // app.UseHeaderMiddleware();

            app.UseRouting();
            app.UseCors("any");

            app.UseAuthorization();

            app.UseHttpExceptions();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<IM.ChatHub>("/chatHub");
                endpoints.MapControllers();
            });
        }


    }
}
