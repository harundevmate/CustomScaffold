using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using System.Diagnostics;

namespace ScaffoldHandler
{
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            Debugger.Launch();

            services.AddHandlebarsScaffolding(options =>
            {
                options.LanguageOptions = LanguageOptions.CSharp;
                options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;
                //options.ExcludedTables = new List<string>()
                //{
                //    "dbo.Item"
                //};
                options.TemplateData = new Dictionary<string, object>()
                {
                    {
                        "base-class",nameof(EntityBase)
                    }
                };
            });
            //HandlebarsDotNet.Handlebars.RegisterHelper("ignore-property", ignoreProperty);

            services.AddSingleton<ICSharpEntityTypeGenerator, MyHbsCSharpEntityTypeGenerator>();
            services.AddSingleton<ICSharpDbContextGenerator, MyHbsCSharpDbContextGenerator>();
        }
    }

}
