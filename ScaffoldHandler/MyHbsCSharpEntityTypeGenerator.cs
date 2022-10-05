using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Shared;
using System.Security;

namespace ScaffoldHandler
{
    public class MyHbsCSharpEntityTypeGenerator : HbsCSharpEntityTypeGenerator
    {
        private readonly IOptions<HandlebarsScaffoldingOptions> _options;
        public MyHbsCSharpEntityTypeGenerator(IAnnotationCodeGenerator annotationCodeGenerator, ICSharpHelper cSharpHelper, 
            IEntityTypeTemplateService entityTypeTemplateService, IEntityTypeTransformationService entityTypeTransformationService, 
            IOptions<HandlebarsScaffoldingOptions> options) : base(annotationCodeGenerator, cSharpHelper, entityTypeTemplateService, 
                entityTypeTransformationService, options)
        {
            _options = options;
        }
        protected override void GenerateProperties(IEntityType entityType)
        {
            var r = typeof(EntityBase).GetProperties().Select(s => s.Name);
            //Check.NotNull(entityType, "entityType");
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (IProperty item in from p in entityType.GetProperties()
                                       orderby p.GetColumnOrder() ?? (-1)
                                       select p)
            {
                PropertyAnnotationsData = new List<Dictionary<string, object>>();
                if (UseDataAnnotations)
                {
                    GeneratePropertyDataAnnotations(item);
                }

                string text = CSharpHelper.Reference(item.ClrType);
                if (UseNullableReferenceTypes && item.IsNullable && !text.EndsWith("?"))
                {
                    text += "?";
                }
                var isnotbase = !r.Any(s => s.ToLower() == item.Name.ToLower());
                bool flag = UseNullableReferenceTypes && (item.ClrType.IsValueType || item.IsNullable);
                Dictionary<string, object> obj = new Dictionary<string, object>
                {
                    { "property-type", text },
                    { "property-name", item.Name },
                    { "property-annotations", PropertyAnnotationsData },
                    
                    // Add new item to template data
                    { "property-isprimarykey", item.IsKey() },
                    //{ "property-isforeignkey", item.IsForeignKey() },
                    { "property-not-base",  isnotbase}
                };
                IOptions<HandlebarsScaffoldingOptions> options = _options;
                obj.Add("property-comment", (options != null && options.Value?.GenerateComments == true) ? GenerateComment(item.GetComment(), 2) : null);
                obj.Add("property-isnullable", flag);
                obj.Add("nullable-reference-types", UseNullableReferenceTypes);
                list.Add(obj);
            }

            List<Dictionary<string, object>> value = EntityTypeTransformationService.TransformProperties(entityType, list);

            // Add to transformed properties
            for (int i = 0; i < value.Count; i++)
            {
                value[i].Add("property-isprimarykey", list[i]["property-isprimarykey"]);
                value[i].Add("property-not-base", list[i]["property-not-base"]);
            }


            TemplateData.Add("properties", value);
        }

        private string GenerateComment(string comment, int indents)
        {
            IndentedStringBuilder indentedStringBuilder = new IndentedStringBuilder();
            if (!string.IsNullOrWhiteSpace(comment))
            {
                for (int i = 0; i < indents; i++)
                {
                    indentedStringBuilder.IncrementIndent();
                }

                string[] array = comment.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string str in array)
                {
                    indentedStringBuilder.AppendLine("/// " + SecurityElement.Escape(str));
                }

                for (int k = 0; k < indents; k++)
                {
                    indentedStringBuilder.DecrementIndent();
                }
            }

            return indentedStringBuilder.ToString().Trim(Environment.NewLine.ToCharArray());
        }
    }
}
