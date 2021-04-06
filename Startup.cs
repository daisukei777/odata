using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using odata.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Formatter.Deserialization;
using Microsoft.EntityFrameworkCore.Cosmos;

namespace odata
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //In OData, You have to specify JsonSerializerOptions if you want to use Pascal properties instead of Camel. This will be improved in the future according to the backlog in the Repo.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "odata", Version = "v1" });
            }).AddMvc().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);

            // In-Memory Database
            services.AddDbContext<BookStoreContext>(opt => opt.UseInMemoryDatabase("BookLists"));
            services.AddDbContext<OrderContext>();
            services.AddControllers();

            // Cosmos DB + EF
            IEdmModel model = GetEdmModel();
            services.AddOData(opt => opt.AddModel("v1", model));
            services.AddOData(opt => opt.AddModel("odata", GetOrderEdmModel()).Filter().Select().Expand().SkipToken().SetMaxTop(null).Count());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Odata v1"));
            }
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Book>("Books");
            builder.EntitySet<Press>("Presses");
            return builder.GetEdmModel();
        }

        private static IEdmModel GetOrderEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Order>("Orders");
            return builder.GetEdmModel();
        }

    }
}
