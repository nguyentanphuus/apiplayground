using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPlayground.Models;
using ApiPlayground.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiPlayground
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
			services.AddMvc(options =>
			{
			    options.RespectBrowserAcceptHeader = true;
			    options.ReturnHttpNotAcceptable = true;

                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			//var connection = "Data Source=.;Initial Catalog=FlixOneStore;Integrated Security=True";
		    var connection =
		        "Data Source=WINDOWS-J3NFT6T;Initial Catalog=FlixOneStore;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            services.AddDbContext<FlixOneStoreContext>(option => option.UseSqlServer(connection));
			services.AddScoped<ICustomerService, CustomerService>();

			services.AddCors(option =>
			{
				option.AddPolicy("AllowAll", builder =>
				{
					builder.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader();
				});
			});
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
		        app.UseExceptionHandler(appBuilder =>
		        {
		            appBuilder.Run(async context =>
		            {
		                context.Response.StatusCode = 500;
		                await context.Response.WriteAsync("Unexpected error. Please contact administrators!");
		            });
		        });
		    }

			app.UseMvc(routes => routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}"
                ));
			app.UseCors("AllowAll");
		}
	}
}
