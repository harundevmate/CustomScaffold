using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldHandler
{
    public class MyHbsCSharpDbContextGenerator : HbsCSharpDbContextGenerator
    {
        public MyHbsCSharpDbContextGenerator(IProviderConfigurationCodeGenerator providerConfigurationCodeGenerator, IAnnotationCodeGenerator annotationCodeGenerator, IDbContextTemplateService dbContextTemplateService, IEntityTypeTransformationService entityTypeTransformationService, ICSharpHelper cSharpHelper, IOptions<HandlebarsScaffoldingOptions> options) : base(providerConfigurationCodeGenerator, annotationCodeGenerator, dbContextTemplateService, entityTypeTransformationService, cSharpHelper, options)
        {
        }
    }
}
