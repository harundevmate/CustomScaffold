using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Helper;
using System.IO;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            //Debugger.Launch();
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
            TemplateData.Add("namespace", HelperScaffold.NamespaceInfrastructure);
            GenerateClass(model, contextName, connectionString, suppressConnectionStringWarning, suppressOnConfiguring);
            //Add
            GenerateDTOs(model);
            GenerateAutoMapper(model);
            GenerateServices(model);
            //GenerateController(model);
            return DbContextTemplateService.GenerateDbContext(TemplateData);
        }

        #region Generate

        private void GenerateServices(IModel model)
        {
            Check.NotNull(model, nameof(model));

            var sb = new IndentedStringBuilder();
            using (sb.Indent())
            using (sb.Indent())
            {
                var dirInterface = $"{HelperScaffold.DirBusinessCore}\\Interface";
                if (!Directory.Exists(dirInterface))
                {
                    Directory.CreateDirectory(dirInterface);
                }

                var dirServices = $"{HelperScaffold.DirBusinessCore}\\Service";
                if (!Directory.Exists(dirServices))
                {
                    Directory.CreateDirectory(dirServices);
                }

                using StreamWriter outputFileProgramSetup = new StreamWriter(Path.Combine(HelperScaffold.DirApi, "ProgramSetup.cs"));

                outputFileProgramSetup.WriteLine("using BusinessCore.Interface;");
                outputFileProgramSetup.WriteLine("using BusinessCore.Service;");
                outputFileProgramSetup.WriteLine($"namespace Api");
                outputFileProgramSetup.WriteLine("{");

                outputFileProgramSetup.WriteLine("\tpublic partial class ProgramSetup");
                outputFileProgramSetup.WriteLine("\t{");

                outputFileProgramSetup.WriteLine($"\t\tpublic static void AddDepedencyInjectionService(IServiceCollection services)");
                outputFileProgramSetup.WriteLine("\t\t{");



                foreach (var entityType in model.GetScaffoldEntityTypes(_options.Value))
                {

                    outputFileProgramSetup.WriteLine($"\t\t\tservices.AddTransient<I{entityType.Name}Service, {entityType.Name}Service>();");

                    #region Interfaces
                    using StreamWriter outputFileInterface = new StreamWriter(Path.Combine(dirInterface, "I"+entityType.Name + "Service.cs"));
                    outputFileInterface.WriteLine("using Infrastructure;");
                    outputFileInterface.WriteLine("using Shared;");
                    outputFileInterface.WriteLine($"namespace BusinessCore.Interface");
                    outputFileInterface.WriteLine("{");

                    outputFileInterface.WriteLine("\tpublic partial interface I" + entityType.Name + "Service");
                    outputFileInterface.WriteLine("\t{");

                    outputFileInterface.WriteLine($"\t\tTask<(bool success, string message)> AddAsync({entityType.Name}DTO entity);");
                    outputFileInterface.WriteLine($"\t\tTask<(bool success, string message)> UpdateAsync({entityType.Name}DTO entity);");
                    outputFileInterface.WriteLine($"\t\tTask<(bool success, string message)> DeleteAsync(string Id);");
                    outputFileInterface.WriteLine($"\t\tTask<{entityType.Name}> GetByIdAsync(string Id);");

                    outputFileInterface.WriteLine("\t}");
                    outputFileInterface.WriteLine("}");
                    #endregion

                    #region Services
                    using StreamWriter outputFileServices = new StreamWriter(Path.Combine(dirServices,entityType.Name + "Service.cs"));
                    outputFileServices.WriteLine("using AutoMapper;");
                    outputFileServices.WriteLine("using BusinessCore.Interface;");
                    outputFileServices.WriteLine("using Infrastructure;");
                    outputFileServices.WriteLine("using Shared;");
                    outputFileServices.WriteLine("using Infrastructure.Interfaces;");
                    outputFileServices.WriteLine($"namespace BusinessCore.Service");
                    outputFileServices.WriteLine("{");

                    outputFileServices.WriteLine("\tpublic partial class " + entityType.Name + "Service : I"+ entityType.Name+ "Service");
                    outputFileServices.WriteLine("\t{");
                    outputFileServices.WriteLine("\t\tprivate readonly IRepository repository;");
                    outputFileServices.WriteLine("\t\tprivate readonly IMapper mapper;");
                    outputFileServices.WriteLine($"\t\tpublic {entityType.Name}Service(IRepository repository,IMapper mapper)");
                    outputFileServices.WriteLine("\t\t{");

                    outputFileServices.WriteLine("\t\t\tthis.repository = repository;");
                    outputFileServices.WriteLine("\t\t\tthis.mapper = mapper;");

                    outputFileServices.WriteLine("\t\t}");

                    outputFileServices.WriteLine($"\t\tpublic async Task<(bool success, string message)> AddAsync({entityType.Name}DTO entity)");
                    outputFileServices.WriteLine("\t\t{");
                    outputFileServices.WriteLine($"\t\t\tvar item = mapper.Map<{entityType.Name}>(entity);");
                    outputFileServices.WriteLine($"\t\t\titem.CreatedAt = DateTime.Now;");
                    outputFileServices.WriteLine($"\t\t\treturn await repository.AddAsync<{entityType.Name}>(item);");
                    outputFileServices.WriteLine("\t\t}");

                    outputFileServices.WriteLine($"\t\tpublic async Task<(bool success, string message)> DeleteAsync(string Id)");
                    outputFileServices.WriteLine("\t\t{");
                    outputFileServices.WriteLine($"\t\t\treturn await repository.DeleteAsync<{entityType.Name}>(Id);");
                    outputFileServices.WriteLine("\t\t}");

                    outputFileServices.WriteLine($"\t\tpublic async Task<{entityType.Name}> GetByIdAsync(string Id)");
                    outputFileServices.WriteLine("\t\t{");
                    outputFileServices.WriteLine($"\t\t\treturn await repository.GetByIdAsync<{entityType.Name}>(Id);");
                    outputFileServices.WriteLine("\t\t}");

                    outputFileServices.WriteLine($"\t\tpublic async Task<(bool success, string message)> UpdateAsync({entityType.Name}DTO entity)");
                    outputFileServices.WriteLine("\t\t{");
                    outputFileServices.WriteLine($"\t\t\tvar item = mapper.Map<{entityType.Name}>(entity);");
                    outputFileServices.WriteLine($"\t\t\titem.ModifiedAt = DateTime.Now;");
                    outputFileServices.WriteLine($"\t\t\treturn await repository.UpdateAsync<{entityType.Name}>(item);");
                    outputFileServices.WriteLine("\t\t}");

                    outputFileServices.WriteLine("\t}");
                    outputFileServices.WriteLine("}");
                    #endregion
                }


                outputFileProgramSetup.WriteLine("\t\t}");
                outputFileProgramSetup.WriteLine("\t}");
                outputFileProgramSetup.WriteLine("}");
            }

            var onServicesGenerate = sb.ToString();
            TemplateData.Add("on-Services-Generate", onServicesGenerate);
        }
        private void GenerateDTOs(IModel model)
        {
            Check.NotNull(model, nameof(model));

            var sb = new IndentedStringBuilder();
            using (sb.Indent())
            using (sb.Indent())
            {
                TemplateData.TryGetValue("namespace", out var _namespace);
                var _namespaces = _namespace.ToString().Split(".");
                var dbContextpath = _namespaces[^1].ToString();
                string path = "Dto";
                var dir = $"{HelperScaffold.DirShared}\\Data\\{path}";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var propBase = typeof(BaseEntity).GetProperties().Select(s=>s.Name);

                foreach (var entityType in model.GetScaffoldEntityTypes(_options.Value))
                {
                    using StreamWriter outputFile = new StreamWriter(Path.Combine(dir, /*entityType.DisplayName()*/entityType.Name + "DTO.cs"));
                    outputFile.WriteLine("using System;");
                    outputFile.WriteLine("using System.Collections.Generic;");
                    outputFile.WriteLine("using System.ComponentModel.DataAnnotations;");
                    outputFile.WriteLine($"namespace {nameof(Shared)}");
                    outputFile.WriteLine("{");

                    outputFile.WriteLine("  public partial class " + /*entityType.DisplayName()*/entityType.Name + "DTO");
                    outputFile.WriteLine("  {");
                    
                    foreach (var property in entityType.GetProperties().OrderBy(p => p.GetColumnOrder()))
                    {
                        //Ignore dto generate property base required -> assumming handling on be
                        if(!property.IsNullable && !propBase.Any(x=>x == property.Name))
                            outputFile.WriteLine("      [Required]");
                        outputFile.WriteLine("      public " + CSharpHelper.Reference(property.ClrType) + " " + property.Name + " { get; set; }");
                    }

                    Check.NotNull(entityType, nameof(entityType));

                    var sortedNavigations = entityType.GetScaffoldNavigations(_options.Value)
                        .OrderBy(n => n.IsOnDependent ? 0 : 1)
                        .ThenBy(n => n.IsCollection ? 1 : 0);

                    if (sortedNavigations.Any())
                    {
                        var navProperties = new List<Dictionary<string, object>>();

                        foreach (var navigation in sortedNavigations)
                        {
                            if (navigation.IsCollection)
                            {
                                outputFile.WriteLine("      //public virtual ICollection<" + navigation.TargetEntityType.Name + "DTO>" + navigation.Name + " { get; set; }");
                            }
                            else
                            {
                                outputFile.WriteLine("      //public virtual " + navigation.TargetEntityType.Name + "DTO " + navigation.Name + " { get; set; }");
                            }
                        }
                    }

                    outputFile.WriteLine("  }");

                    outputFile.WriteLine("");

                    outputFile.WriteLine("  public partial class " + entityType.Name + $"DTOWithDetail : {nameof(BaseEntity)}");
                    outputFile.WriteLine("  {");
                    Check.NotNull(entityType, nameof(entityType));

                    if (sortedNavigations.Any())
                    {
                        var navProperties = new List<Dictionary<string, object>>();

                        foreach (var navigation in sortedNavigations)
                        {
                            if (navigation.IsCollection)
                            {
                                outputFile.WriteLine("      //public virtual ICollection<" + navigation.TargetEntityType.Name + "DTO>" + navigation.Name + " { get; set; }");
                            }
                            else
                            {
                                outputFile.WriteLine("      //public virtual " + navigation.TargetEntityType.Name + "DTO " + navigation.Name + " { get; set; }");
                            }
                        }
                    }

                    outputFile.WriteLine("  }");
                    outputFile.WriteLine("}");
                }
            }

            var onDTOGenerate = sb.ToString();
            TemplateData.Add("on-DTO-Generate", onDTOGenerate);
        }

        private void GenerateAutoMapper(IModel model)
        {
            Check.NotNull(model, nameof(model));

            var sb = new IndentedStringBuilder();
            using (sb.Indent())
            using (sb.Indent())
            {
                TemplateData.TryGetValue("namespace", out var _namespace);
                var _namespaces = _namespace.ToString().Split(".");
                var dbContextpath = _namespaces[^1].ToString();
                string path = "Dto";
                var dir = $"{HelperScaffold.DirInfrastructure}\\Data";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using StreamWriter outputFile = new StreamWriter(Path.Combine(dir, "MapperProfile.cs"));

                outputFile.WriteLine("using AutoMapper;");
                outputFile.WriteLine("using Shared;");
                outputFile.WriteLine($"namespace {HelperScaffold.NamespaceInfrastructure}");
                outputFile.WriteLine("{");

                outputFile.WriteLine("\tpublic class MapperProfile : MapperConfigurationExpression");
                outputFile.WriteLine("\t{");
                outputFile.WriteLine("\t\tpublic MapperProfile()");
                outputFile.WriteLine("\t\t{");

                foreach (var entityType in model.GetScaffoldEntityTypes(_options.Value))
                {
                    outputFile.WriteLine($"\t\t\tCreateMap<{entityType.Name}, {entityType.Name}DTO>().ReverseMap();");
                }
                outputFile.WriteLine("\t\t}");
                outputFile.WriteLine("\t}");
                outputFile.WriteLine("}");
                var onMapperGenerate = sb.ToString();
                TemplateData.Add("on-Mapper-Generate", onMapperGenerate);
            }
        }


        #region Controller

        private void GenerateController(IModel model)
        {
            Check.NotNull(model, nameof(model));

            var sb = new IndentedStringBuilder();
            using (sb.Indent())
            using (sb.Indent())
            {
                //TemplateData.TryGetValue("namespace", out var _namespace);
                //var _namespaces = _namespace.ToString().Split(".");
                //var dbContextpath = _namespaces[^1].ToString();
                string path = "Dto";
                var dir = $"{HelperScaffold.DirApi}\\Controllers";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                foreach (var entityType in model.GetScaffoldEntityTypes(_options.Value))
                {
                    using StreamWriter outputFile = new StreamWriter(Path.Combine(dir, /*entityType.DisplayName()*/entityType.Name + "Controller.cs"));

                    outputFile.WriteLine("using Api.Controllers.Base;");
                    outputFile.WriteLine("using Api.Error;");
                    outputFile.WriteLine("using Api.Helper;");
                    outputFile.WriteLine("using AutoMapper;");
                    outputFile.WriteLine("using BusinessCore;");
                    outputFile.WriteLine("using BusinessCore.Helper;");
                    outputFile.WriteLine("using Infrastructure;");
                    outputFile.WriteLine("using Microsoft.AspNetCore.Mvc;");
                    outputFile.WriteLine("using Microsoft.EntityFrameworkCore;");
                    outputFile.WriteLine("using Shared.Interfaces;");
                    outputFile.WriteLine("namespace Api");
                    outputFile.WriteLine("{");
                    outputFile.WriteLine("\tpublic partial class " + entityType.Name + "Controller : BaseApiController");
                    outputFile.WriteLine("\t{");
                    outputFile.WriteLine("\t\tprivate readonly IRepository repository;");
                    outputFile.WriteLine("\t\tprivate readonly IMapper mapper;");
                    outputFile.WriteLine($"\t\tpublic {entityType.Name}Controller(IRepository repository, IMapper mapper) : base(repository, mapper)");
                    outputFile.WriteLine("\t\t{");
                    outputFile.WriteLine("\t\t\tthis.repository = repository;");
                    outputFile.WriteLine("\t\t\tthis.mapper = mapper;");
                    outputFile.WriteLine("\t\t}\n");

                    GenerateGetListMethod(outputFile, entityType);
                    GenerateGeByIdMethod(outputFile, entityType);
                    GenerateAddAsync(outputFile, entityType);
                    GenerateUpdateAsync(outputFile, entityType);
                    GenerateDeleteAsync(outputFile, entityType);

                    outputFile.WriteLine("\t}");
                    outputFile.WriteLine("}");


                }
            }

            var onDTOGenerate = sb.ToString();
            TemplateData.Add("on-Controller-Generate", onDTOGenerate);
        }

        private void GenerateDeleteAsync(StreamWriter outputFile, IEntityType entityType)
        {
            outputFile.WriteLine($"\t\t// DELETE: api/{entityType.Name}/1");
            outputFile.WriteLine($"\t\t[HttpDelete(\"{{id}}\")]");
            outputFile.WriteLine($"\t\tpublic async Task<IActionResult> DeleteAsync(Guid id)");
            outputFile.WriteLine("\t\t{");
            //outputFile.WriteLine("\t\t\ttry");
            //outputFile.WriteLine("\t\t\t{");


            outputFile.WriteLine($"\t\t\tif (ValidateDelete(id))");
            outputFile.WriteLine("\t\t\t{");

            outputFile.WriteLine($"\t\t\t\tvar existingItems = await repository.GetByIdAsync<{entityType.Name}>(id);");
            outputFile.WriteLine($"\t\t\t\tif (existingItems == null)");
            outputFile.WriteLine("\t\t\t\t{");
            outputFile.WriteLine($"\t\t\t\t\treturn Requests.Response(this, new ApiStatus(409), null, Constant.Message.NotFound);");
            outputFile.WriteLine("\t\t\t\t}");

            outputFile.WriteLine($"\t\t\t\tvar (Success, Message) = await repository.DeleteAsync<{entityType.Name}>(id);");
            outputFile.WriteLine($"\t\t\t\treturn !Success && Message != \"\" ? Requests.Response(this, new ApiStatus(409), null, Message) : Requests.Response(this, new ApiStatus(200), null, Message);");

            outputFile.WriteLine("\t\t\t}");

            outputFile.WriteLine($"\t\t\telse");
            outputFile.WriteLine("\t\t\t{");
            outputFile.WriteLine("\t\t\t\treturn Requests.Response(this, new ApiStatus(409), ModelState, \"\");");
            outputFile.WriteLine("\t\t\t}");
            //outputFile.WriteLine("\t\t\t}");
            //outputFile.WriteLine($"\t\t\tcatch (Exception ex)");
            //outputFile.WriteLine("\t\t\t{");
            //outputFile.WriteLine("\t\t\t\treturn Requests.Response(this, new ApiStatus(500), null, ex.Message);");
            //outputFile.WriteLine("\t\t\t}");
            outputFile.WriteLine("\t\t}");
        }
        private void GenerateGetListMethod(StreamWriter outputFile, IEntityType entityType)
        {

            outputFile.WriteLine($"\t\t// GET: api/{entityType.Name}");
            outputFile.WriteLine($"\t\t[HttpGet]");
            outputFile.WriteLine($"\t\tpublic async Task<IActionResult> ListAsync()");
            outputFile.WriteLine("\t\t{");
            //outputFile.WriteLine("\t\t\ttry");
            //outputFile.WriteLine("\t\t\t{");
            outputFile.WriteLine($"\t\t\tvar items = await repository.GetQueryable<{entityType.Name}>().AsNoTracking().ToListAsync();");
            outputFile.WriteLine($"\t\t\tvar result = mapper.Map<List<{entityType.Name}DTO>>(items);");
            outputFile.WriteLine($"\t\t\treturn Requests.Response(this, new ApiStatus(200), result, Constant.Message.Success);");
            //outputFile.WriteLine("\t\t\t}");
            //outputFile.WriteLine($"\t\t\tcatch (Exception ex)");
            //outputFile.WriteLine("\t\t\t{");
            //outputFile.WriteLine("\t\t\t\treturn Requests.Response(this, new ApiStatus(500), null, ex.Message);");
            //outputFile.WriteLine("\t\t\t}");
            outputFile.WriteLine("\t\t}");
        }

        private void GenerateGeByIdMethod(StreamWriter outputFile, IEntityType entityType)
        {
            string param = @"""{id}""";
            outputFile.WriteLine($"\t\t// GET: api/{entityType.Name}");
            outputFile.WriteLine($"\t\t[HttpGet({param})]");
            outputFile.WriteLine($"\t\tpublic async Task<IActionResult> GetByIdAsync(Guid id)");
            outputFile.WriteLine("\t\t{");
            //outputFile.WriteLine("\t\t\ttry");
            //outputFile.WriteLine("\t\t\t{");
            outputFile.WriteLine($"\t\t\tvar items = await repository.GetByIdAsync<{entityType.Name}>(id);");
            outputFile.WriteLine($"\t\t\tvar result = mapper.Map<{entityType.Name}>(items);");
            outputFile.WriteLine($"\t\t\treturn Requests.Response(this, new ApiStatus(200), result, Constant.Message.Success);");
            //outputFile.WriteLine("\t\t\t}");
            //outputFile.WriteLine($"\t\t\tcatch (Exception ex)");
            //outputFile.WriteLine("\t\t\t{");
            //outputFile.WriteLine("\t\t\t\treturn Requests.Response(this, new ApiStatus(500), null, ex.Message);");
            //outputFile.WriteLine("\t\t\t}");
            outputFile.WriteLine("\t\t}");
        }

        private void GenerateAddAsync(StreamWriter outputFile, IEntityType entityType)
        {
            outputFile.WriteLine($"\t\t// POST: api/{entityType.Name}");
            outputFile.WriteLine($"\t\t[HttpPost]");
            outputFile.WriteLine($"\t\tpublic async Task<IActionResult> AddAsync([FromBody] {entityType.Name}DTO {entityType.Name}DTO)");
            outputFile.WriteLine("\t\t{");
            //outputFile.WriteLine("\t\t\ttry");
            //outputFile.WriteLine("\t\t\t{");

            outputFile.WriteLine($"\t\t\tvar item = mapper.Map<{entityType.Name}>({entityType.Name}DTO);");
            outputFile.WriteLine($"\t\t\tif (ModelState.IsValid && ValidateCreate(item))");
            outputFile.WriteLine("\t\t\t{");
            outputFile.WriteLine($"\t\t\t\titem.Id = Guid.NewGuid();");
            outputFile.WriteLine($"\t\t\t\tvar (Success, Message) = await repository.AddAsync<{entityType.Name}>(item);");
            outputFile.WriteLine($"\t\t\t\treturn !Success && Message != \"\" ? Requests.Response(this, new ApiStatus(409), null, Message) : Requests.Response(this, new ApiStatus(200), null, Message);");
            outputFile.WriteLine("\t\t\t}");
            outputFile.WriteLine($"\t\t\telse");
            outputFile.WriteLine("\t\t\t{");
            outputFile.WriteLine("\t\t\t\treturn Requests.Response(this, new ApiStatus(409), ModelState, \"\");");
            outputFile.WriteLine("\t\t\t}");

            //outputFile.WriteLine("\t\t\t}");
            //outputFile.WriteLine($"\t\t\tcatch (Exception ex)");
            //outputFile.WriteLine("\t\t\t{");
            //outputFile.WriteLine("\t\t\t\treturn Requests.Response(this, new ApiStatus(500), null, ex.Message);");
            //outputFile.WriteLine("\t\t\t}");
            outputFile.WriteLine("\t\t}");
        }

        private void GenerateUpdateAsync(StreamWriter outputFile, IEntityType entityType)
        {
            outputFile.WriteLine($"\t\t// PATCH: api/{entityType.Name}");
            outputFile.WriteLine($"\t\t[HttpPatch]");
            outputFile.WriteLine($"\t\tpublic async Task<IActionResult> UpdateAsync([FromBody] {entityType.Name}DTO {entityType.Name}DTO)");
            outputFile.WriteLine("\t\t{");
            //outputFile.WriteLine("\t\t\ttry");
            //outputFile.WriteLine("\t\t\t{");

            outputFile.WriteLine($"\t\t\tvar existingItems = await repository.GetByIdAsync<{entityType.Name}>({entityType.Name}DTO.Id);");

            outputFile.WriteLine($"\t\t\tif (existingItems == null)");
            outputFile.WriteLine("\t\t\t{");
            outputFile.WriteLine($"\t\t\t\treturn Requests.Response(this, new ApiStatus(409), null, Constant.Message.NotFound);");
            outputFile.WriteLine("\t\t\t}");
            outputFile.WriteLine($"\t\t\tvar item = mapper.Map<{entityType.Name}DTO, {entityType.Name}>({entityType.Name}DTO, existingItems);");

            outputFile.WriteLine($"\t\t\tif (ModelState.IsValid && ValidateUpdate(item))");

            outputFile.WriteLine("\t\t\t{");

            outputFile.WriteLine($"\t\t\t\tvar (Success, Message) = await repository.UpdateAsync<{entityType.Name}>(item);");
            outputFile.WriteLine($"\t\t\t\treturn !Success && Message != \"\" ? Requests.Response(this, new ApiStatus(409), null, Message) : Requests.Response(this, new ApiStatus(200), null, Message);");

            outputFile.WriteLine("\t\t\t}");

            outputFile.WriteLine($"\t\t\telse");
            outputFile.WriteLine("\t\t\t{");
            outputFile.WriteLine("\t\t\t\treturn Requests.Response(this, new ApiStatus(409), ModelState, \"\");");
            outputFile.WriteLine("\t\t\t}");

            //outputFile.WriteLine("\t\t\t}");
            //outputFile.WriteLine($"\t\t\tcatch (Exception ex)");
            //outputFile.WriteLine("\t\t\t{");
            //outputFile.WriteLine("\t\t\t\treturn Requests.Response(this, new ApiStatus(500), null, ex.Message);");
            //outputFile.WriteLine("\t\t\t}");
            outputFile.WriteLine("\t\t}");
        }

        #endregion

        #endregion

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
