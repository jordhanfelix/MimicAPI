using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using MimicAPI.DataBase;
using MimicAPI.Helpers;
using MimicAPI.V1.Repositories;
using MimicAPI.V1.Repositories.Contracts;
using System;
using System.IO;
using System.Linq;

namespace MimicAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region Auto mapper configuração
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            services.AddDbContext<MimicContext>(opt => {
                opt.UseSqlite("Data Source=DataBase\\Mimic.db");
            });
            services.AddMvc();
            services.AddScoped<IPalavraRepository, PalavraRepository>();
            services.AddApiVersioning(cfg => {
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddSwaggerGen(cfg => {
                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());
                cfg.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {

                    Title = "Api de Mimica",
                    Version = "Versão 1.0",
                    Description = "Api criada com finalidade de estudos, está api será consumida posteriormente por um aplicativo desenvolvido em flutter",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Jordhan H. Félix",
                        Email = "jordhanfelix@hotmail.com",
                        Url = new Uri("https://jordhanfelix.github.io/")
                    },

                });

                var caminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var nomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var caminhoXmlDeComentario = Path.Combine(caminhoProjeto, nomeProjeto);
                cfg.IncludeXmlComments(caminhoXmlDeComentario);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseMvc();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseStatusCodePages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(cfg => {
                cfg.SwaggerEndpoint("swagger/v1/swagger.json", "MimicAPI");
                cfg.RoutePrefix = String.Empty;
            });
        }
        
    }
}
