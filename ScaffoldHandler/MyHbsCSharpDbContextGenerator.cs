﻿using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldHandler
{
    public class MyHbsCSharpDbContextGenerator : HbsCSharpDbContextGenerator
    {
        IOptions<HandlebarsScaffoldingOptions> _options;

        private string _modelNamespace;
        public MyHbsCSharpDbContextGenerator(IProviderConfigurationCodeGenerator providerConfigurationCodeGenerator, 
            IAnnotationCodeGenerator annotationCodeGenerator, IDbContextTemplateService dbContextTemplateService, 
            IEntityTypeTransformationService entityTypeTransformationService, ICSharpHelper cSharpHelper, IOptions<HandlebarsScaffoldingOptions> options) : 
            base(providerConfigurationCodeGenerator, annotationCodeGenerator, dbContextTemplateService, entityTypeTransformationService, cSharpHelper, options)
        {
            this._options = options;
        }
        protected override void GenerateClass(IModel model, string contextName, string connectionString, bool suppressConnectionStringWarning, bool suppressOnConfiguring)
        {
            Check.NotNull(model, "model");
            Check.NotNull(contextName, "contextName");
            Check.NotNull(connectionString, "connectionString");
            IOptions<HandlebarsScaffoldingOptions> options = _options;
            if (options == null || options.Value?.EnableSchemaFolders != true)
            {
                TemplateData.Add("model-namespace", _modelNamespace);
            }
            else
            {
                GenerateModelImports(model);
            }

            TemplateData.Add("class", contextName);
            GenerateDbSets(model);
            GenerateEntityTypeErrors(model);
            if (suppressOnConfiguring)
            {
                TemplateData.Add("suppress-on-configuring", true);
            }
            else
            {
                GenerateOnConfiguring(connectionString, suppressConnectionStringWarning);
            }

            GenerateOnModelCreating(model);
        }

        public override string WriteCode(IModel model, string contextName, string connectionString, string contextNamespace, string modelNamespace, bool useDataAnnotations, bool useNullableReferenceTypes, bool suppressConnectionStringWarning, bool suppressOnConfiguring)
        {
            Check.NotNull(model, "model");
            if (!string.IsNullOrEmpty(modelNamespace) && string.CompareOrdinal(contextNamespace, modelNamespace) != 0)
            {
                _modelNamespace = modelNamespace;
            }

            UseDataAnnotations = useDataAnnotations;
            UseNullableReferenceTypes = useNullableReferenceTypes;
            TemplateData = new Dictionary<string, object>();
            if (_options.Value.TemplateData != null)
            {
                foreach (KeyValuePair<string, object> templateDatum in _options.Value.TemplateData)
                {
                    TemplateData.Add(templateDatum.Key, templateDatum.Value);
                }
            }
            TemplateData.Add("namespace", contextNamespace);
            GenerateClass(model, contextName, connectionString, suppressConnectionStringWarning, suppressOnConfiguring);
            return DbContextTemplateService.GenerateDbContext(TemplateData);
        }

        #region Private

        private string GetEntityTypeName(IEntityType entityType, string entityTypeName)
        {
            string text = ((!string.IsNullOrEmpty(entityType.GetTableName())) ? entityType.GetSchema() : entityType.GetViewSchema());
            IOptions<HandlebarsScaffoldingOptions> options = _options;
            if (options == null || options.Value?.EnableSchemaFolders != true)
            {
                return entityTypeName;
            }

            return text + "." + entityTypeName;
        }
        private void GenerateDbSets(IModel model)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (IEntityType scaffoldEntityType in model.GetScaffoldEntityTypes(_options.Value))
            {
                if (!IsManyToManyJoinEntityType(scaffoldEntityType))
                {
                    string entityTypeName = GetEntityTypeName(scaffoldEntityType, EntityTypeTransformationService.TransformTypeEntityName(scaffoldEntityType.Name));
                    list.Add(new Dictionary<string, object>
                    {
                        { "set-property-type", entityTypeName },
                        {
                            "set-property-name",
                            scaffoldEntityType.GetDbSetName()
                        },
                        { "nullable-reference-types", UseNullableReferenceTypes }
                    });
                }
            }

            TemplateData.Add("dbsets", list);
        }

        private void GenerateModelImports(IModel model)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (string item in (from e in model.GetScaffoldEntityTypes(_options.Value)
                                     select e.GetSchema() into s
                                     orderby s
                                     select s).Distinct())
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
                defaultInterpolatedStringHandler.AppendFormatted(item);
                defaultInterpolatedStringHandler.AppendLiteral(" = ");
                defaultInterpolatedStringHandler.AppendFormatted(_modelNamespace);
                defaultInterpolatedStringHandler.AppendLiteral(".");
                defaultInterpolatedStringHandler.AppendFormatted(item);
                dictionary.Add("model-import", defaultInterpolatedStringHandler.ToStringAndClear());
                list.Add(dictionary);
            }

            TemplateData.Add("model-imports", list);
        }

        private void GenerateEntityTypeErrors(IModel model)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (KeyValuePair<string, string> entityTypeError in model.GetEntityTypeErrors())
            {
                list.Add(new Dictionary<string, object> {
                {
                    "entity-type-error",
                    "// " + entityTypeError.Value + " Please see the warning messages."
                } });
            }

            TemplateData.Add("entity-type-errors", list);
        }


        private static string GenerateLambdaToKey(IEntityType entityType, IReadOnlyList<IProperty> properties, string lambdaIdentifier, Func<IEntityType, string, string, string> nameTransform)
        {
            if (properties.Count > 0)
            {
                if (properties.Count != 1)
                {
                    return "new { " + string.Join(", ", properties.Select((IProperty p) => lambdaIdentifier + "." + nameTransform(entityType, p.Name, p.DeclaringType.Name))) + " }";
                }

                return lambdaIdentifier + "." + nameTransform(entityType, properties[0].Name, properties[0].DeclaringType.Name);
            }

            return "";
        }

        private static void RemoveAnnotation(ref List<IAnnotation> annotations, string annotationName)
        {
            annotations.Remove(annotations.SingleOrDefault((IAnnotation a) => a.Name == annotationName));
        }

        private IList<string> GenerateAnnotations(IEnumerable<IAnnotation> annotations)
        {
            return annotations.Select(new Func<IAnnotation, string>(GenerateAnnotation)).ToList();
        }

        private string GenerateAnnotation(IAnnotation annotation)
        {
            return ".HasAnnotation(" + CSharpHelper.Literal(annotation.Name) + ", " + CSharpHelper.UnknownLiteral(annotation.Value) + ")";
        }

        private static bool IsManyToManyJoinEntityType(IEntityType entityType)
        {
            if (!entityType.GetNavigations().Any() && !entityType.GetSkipNavigations().Any())
            {
                IKey key = entityType.FindPrimaryKey();
                List<IProperty> list = entityType.GetProperties().ToList();
                List<IForeignKey> list2 = entityType.GetForeignKeys().ToList();
                if (key != null && key.Properties.Count > 1 && list2.Count == 2 && key.Properties.Count == list.Count && list2[0].Properties.Count + list2[1].Properties.Count == list.Count && !list2[0].Properties.Intersect(list2[1].Properties).Any() && list2[0].IsRequired && list2[1].IsRequired && !list2[0].IsUnique && !list2[1].IsUnique)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

    }
}