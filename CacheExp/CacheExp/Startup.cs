using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CacheExp
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
            // cache in memory
            services.AddMemoryCache();
            // caching response for middlewares
            services.AddResponseCaching();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(joptions =>
                {
                    joptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            services.AddLogging();
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IPInfoProvider, IPStackClient>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<MonsterJobs>();
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
                app.UseHsts();
            }
            app.UseResponseCaching();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
