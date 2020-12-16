using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using UploaderApp.API.Models;
using UploaderApp.API.Services;
using UploaderApp.API.Data;

namespace UploaderApp.API
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
            services.Configure<DocumentInfoDBSettings>(
                Configuration.GetSection(nameof(DocumentInfoDBSettings))
            );
            services.AddSingleton<IDocumentInfoDBSettings>(sp =>
              sp.GetRequiredService<IOptions<DocumentInfoDBSettings>>().Value
            );

            services.AddSingleton<DocInfoService>();
            services.AddSingleton<UserService>();


            services.AddDbContext<DataContext>(x => x.UseMySQL(Configuration.GetConnectionString("Default")));
            services.AddControllers();
            services.AddCors();
            services.AddScoped<ISendLinkRepository, DocumentRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DocViewing API",
                    Version = "v1",
                    Description = "IndxLogic Licensing and Document Receipt API "
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "api/licensing API v1.0");
            });

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
