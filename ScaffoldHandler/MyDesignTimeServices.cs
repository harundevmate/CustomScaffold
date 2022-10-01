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

            //var opt = ReverseEngineerOptions.EntitiesOnly;
            //services.AddHandlebarsScaffolding(options);

            //services.AddSingleton<ICSharpEntityTypeGenerator, MyHbsCSharpEntityTypeGenerator>();


            services.AddHandlebarsScaffolding(options =>
            {
                options.LanguageOptions = LanguageOptions.CSharp;
                options.ReverseEngineerOptions = ReverseEngineerOptions.EntitiesOnly;
                options.ExcludedTables = new List<string>()
                {
                    "dbo.Item"
                };
                options.TemplateData = new Dictionary<string, object>()
                {
                    {
                        "base-class",nameof(EntityBase)
                    }
                };
            });
            HandlebarsDotNet.Handlebars.RegisterHelper("ignore-property", ignoreProperty);

            services.AddSingleton<ICSharpEntityTypeGenerator, MyHbsCSharpEntityTypeGenerator>();
            //services.AddHandlebarsHelpers(myHelper);
        }

        private void ignoreProperty(EncodedTextWriter output, Context context, Arguments arguments)
        {
            //output.Write(context.Value.ToString()); => System.Collections.Generic.Dictionary`2[System.String,System.Object]
            var values = context.Value as Dictionary<string, object>;
            
            foreach (var item in values)
            {

                output.Write(item.Key + "=>" + item.Value);
                #region 1
                //output.Write(item.Key+"=>"+item.Value);

                //    property - type=>Guid
                //property - name=>Id
                #endregion



                //output.Write("Value:"+item.Value.ToString());
                //output.Write("KEY:"+item.Key.ToString());

                //output.Write($"public {item.Key} | {item.Value} ");

                //if (item.Key.ToString() == "Id")
                //{
                //    output.Write($"public {item.Key} {item.Value}");
                //}
            }

        }
        //        void MarkNotBaseProperties(TextWriter writer, object context, object[] parameters)
        //        {

        //            var values = (Dictionary)context;
        //            var baseProperties = new List<Dictionary>();
        //            var properties = (List<Dictionary>)values[“properties”];
        //            foreach (var item in properties)
        //            {
        //                if (item.ContainsKey(“property - name”) && item[“property - name”].ToString() == “Id”)
        //{
        //                    baseProperties.Add(item);
        //                    item.Add(“is -not - base - property”, false);
        //                }
        //else
        //                {
        //                    item.Add(“is -not - base - property”, true);
        //                }
        //            }

        //            values[“properties”] = properties;
        //        }
        private void MyHbsHelper(EncodedTextWriter output, Context context, Arguments arguments)
        {
            //writer.Write("// My Handlebars Helper");
            output.Write("// My Handlebars Helper");
        }

    }

}
