using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using HandlebarsDotNet;
namespace ClassLibrary1
{
    /// <summary>
    /// Used by EF Core toolchain to register design time services.
    /// </summary>
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        /// <summary>
        /// Register Handlebars scaffolding with design time dependency injection system.
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            services.AddHandlebarsScaffolding(configureOptions: options =>
                 options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities);
        }
    }
}