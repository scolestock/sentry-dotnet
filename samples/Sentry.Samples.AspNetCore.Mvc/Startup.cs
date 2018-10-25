using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentry.Extensibility;

namespace Samples.AspNetCore.Mvc
{
    public class Startup : IStartup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Register as many ISentryEventExceptionProcessor as you need. They ALL get called.
            services.AddSingleton<ISentryEventExceptionProcessor, SpecialExceptionProcessor>();

            // You can also register as many ISentryEventProcessor as you need.
            services.AddTransient<ISentryEventProcessor, ExampleEventProcessor>();

            // To demonstrate taking a request-aware service into the event processor above
            services.AddHttpContextAccessor();

            services.AddSingleton<IGameService, GameService>();

            services.AddMvc();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
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
                app.UseExceptionHandler("/Home/Error");
            }


        }
    }
}
