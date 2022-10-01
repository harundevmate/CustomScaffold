using EntityFrameworkCore.Scaffolding.Handlebars;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScaffoldHandler
{
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            //Debugger.Launch();

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
        }
    }

}
