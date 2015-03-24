using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BL.API.Operations.Special.Concrete.Old.Reports;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Report;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Reports;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports.DTO;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using Newtonsoft.Json;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;
using ReportModel = DoubleGis.Erm.BL.UI.Web.Mvc.Models.Report.ReportModel;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class ReportController : ControllerBase
    {
        private readonly IReportsSettings _reportsSettings;
        private readonly ILocalizationSettings _localizationSettings;
        private readonly IReportSimplifiedModel _reportSimplifiedModel;
        private readonly IUserRepository _userRepository;
        private readonly IPublicService _publicService;
        private readonly IClientProxyFactory _clientProxyFactory;

        public ReportController(IMsCrmSettings msCrmSettings,
                                IAPIOperationsServiceSettings operationsServiceSettings,
                                IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                IAPIIdentityServiceSettings identityServiceSettings,
                                IUserContext userContext,
                                ITracer tracer,
                                IGetBaseCurrencyService getBaseCurrencyService,
                                IReportsSettings reportsSettings,
                                ILocalizationSettings localizationSettings,
                                IReportSimplifiedModel reportSimplifiedModel,
                                IUserRepository userRepository,
                                IPublicService publicService,
                                IClientProxyFactory clientProxyFactory)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _reportsSettings = reportsSettings;
            _localizationSettings = localizationSettings;
            _reportSimplifiedModel = reportSimplifiedModel;
            _userRepository = userRepository;
            _publicService = publicService;
            _clientProxyFactory = clientProxyFactory;
        }

        private string ReportServerPath
        {
            get
            {
                var reportServerPath = _reportsSettings.ReportServer;

                if (string.IsNullOrEmpty(reportServerPath))
                {
                    throw new InvalidOperationException(BLResources.ReportServerURLNotFound);
                }

                // add trailing slash
                if (!reportServerPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    reportServerPath = reportServerPath + "/";
                }

                return reportServerPath;
            }
        }

        [HttpGet]
        [UseDependencyFields]
        public ActionResult Edit(long id)
        {
            var currentUserId = UserContext.Identity.Code;
            var report = _reportSimplifiedModel.GetReports(currentUserId).Single(re => re.Id == id);
            var fields = _reportSimplifiedModel.GetReportFields(report.Id, currentUserId);
            var version = fields.Select(dto => dto.Timestamp).Concat(new[] { report.Timestamp }).Max();

            try
            {
                var model = CreateModel(report, fields.ToList(), version);
                model.ReportType = model.GetType().FullName; // название типа отчёта используется при десериализации модели

                foreach (var field in model.Fields)
                {
                    field.GetExpression = CreateGetterLambda(model.GetType(), field.Name);
                    var setter = CreateSetterLambda(model.GetType(), field.Name);
                    setter.Compile().DynamicInvoke(model, field.DefaultValue);
                }

                model.DisplayName = report.DisplayName;
                model.ReportName = report.ReportName;
                model.Format = "preview";
                model.HiddenField = JsonConvert.SerializeObject(fields.Where(dto => dto.IsHidden).Select(dto => dto.Name).ToArray());
                model.ReportServerFormatParameter = report.FormatParameter;

                return View("Edit", model);
            }
            catch (NotificationException exception)
            {
                var model = new ReportModel();
                model.Message = exception.Message;
                model.MessageType = MessageType.CriticalError;

                return View("Edit", model);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, ReportModel model)
        {
            // Сервер ещё не прогрузил тип отчёта, с которым работает клиент. Пытаемся инициализировать этот тип отчета.
            var delayed = model as DelayedReportModel;
            if (delayed != null)
            {
                var currentUserId = UserContext.Identity.Code;
                var report = _reportSimplifiedModel.GetReports(currentUserId).Single(re => re.Id == id);
                var fields = _reportSimplifiedModel.GetReportFields(id, currentUserId);
                var version = fields.Select(dto => dto.Timestamp).Concat(new[] { report.Timestamp }).Max();

                CreateModel(report, fields.ToList(), version);

                model = delayed.GetReportModel();
            }

            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }

            // жесткий костыль, связанный с тем, что один отчёт не был перенесен на ReportService
            switch (model.ReportName.ToLowerInvariant())
            {
                case "отчет по планированию":
                {
                    return PlanningReport(model);
                }
                case "отчет о выручке":
                {
                    return ProceedsReport(model);
                }
            }

            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(ReportServerPath).Append(HttpUtility.UrlEncode(model.ReportName));
            foreach (var property in properties)
            {
                var value = GetValue(model, property);
                stringBuilder.Append("&")
                    .Append(property.Name)
                    .Append("=")
                    .Append(Convert.ToString(value, _localizationSettings.ApplicationCulture));
            }

            stringBuilder.Append("&rs:ParameterLanguage=").Append(_localizationSettings.ApplicationCulture.Name);

            stringBuilder.Append("&rs:Command=Render");

            if (string.Equals(model.Format, "download") && !string.IsNullOrWhiteSpace(model.ReportServerFormatParameter))
            {
                stringBuilder.Append("&").Append(model.ReportServerFormatParameter);
            }
            else
            {
                // Отчего-то этот параметр мешает выгрузке отчётов в форматированном виде, поэтому применяем его в тех случаях, когда выгрузка не требуется.
                // http://msdn.microsoft.com/en-us/library/ms153610(v=sql.105).aspx
                stringBuilder.Append("&rs:ResetSession=true");
            }

            if (!_reportSimplifiedModel.IsUserFromHeadBranch(UserContext.Identity.Code))
            {
                stringBuilder.Append("&rc:Parameters=False");
            }

            stringBuilder.Append("&HostName=").Append(HttpContext.Request.UserHostAddress);
            stringBuilder.Append("&UserName=").Append(UserContext.Identity.Account);

            return new RedirectResult(stringBuilder.ToString());
        }

        [HttpGet]
        public ActionResult Complex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Tree()
        {
            var currentUserId = UserContext.Identity.Code;
            var reports = _reportSimplifiedModel.GetReports(currentUserId);
            var clientReportRepresentation = reports
                .Select(dto => new
            {
                // Регистр - нижний. Потребитель данных - Ext.tree.TreeLoader
                id = dto.Id,
                text = dto.DisplayName,
                leaf = true,
            })
                .ToArray();

            return new JsonNetResult(clientReportRepresentation);
        }

        private object GetValue(ReportModel model, PropertyInfo property)
        {
            if (property.PropertyType == typeof(LookupField))
            {
                var lookup = (LookupField)property.GetValue(model);
                return lookup != null
                           ? lookup.Key ?? 0
                           : 0;
            }

            if (property.PropertyType.IsEnum)
            {
                return (long)property.GetValue(model);
            }

            if (property.PropertyType == typeof(DateTime))
            {
                return ((DateTime)property.GetValue(model)).ToString(_localizationSettings.ApplicationCulture.DateTimeFormat.ShortDatePattern, _localizationSettings.ApplicationCulture);
            }

            if (property.PropertyType == typeof(DateTime?))
            {
                return ((DateTime?)property.GetValue(model) ?? DateTime.Now).ToString(_localizationSettings.ApplicationCulture.DateTimeFormat.ShortDatePattern, _localizationSettings.ApplicationCulture);
            }
            
            return property.GetValue(model);
        }

        #region Model generation

        private static IEnumerable<AttributeProvider> GetAttributeProviders(ReportFieldDto field)
        {
            var result = new List<AttributeProvider> { new DisplayNameAttributeProvider(field.DisplayName ?? field.Name) };

            if (field.IsRequired)
            {
                result.Add(new RequiredAttributeProvider());
            }

            if (field.Dependencies != null)
            {
                result.AddRange(field.Dependencies.Select(dependency => new DependencyAttributeProvider(dependency)));
            }

            result.Add(new DependencyAttributeProvider(new DependencyDefinitionDto()
                {
                    DependencyType = DependencyType.Hidden,
                    DependencyScript = string.Format("isHiddenField('{0}')", field.Name),
                    FieldName = field.Name,
                }));

            if (field.GreaterOrEqualThan != null)
            {
                result.AddRange(field.GreaterOrEqualThan.Select(dto => new GreaterOrEqualThanAttributeProvider(dto.FieldName, dto.ErrorMessage)));
            }

            return result;
        }

        private static void ImplementSetter(MethodBuilder setterBuilder, FieldInfo field)
        {
            var il = setterBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);
        }

        private static void ImplementGetter(MethodBuilder getterBuilder, FieldInfo field)
        {
            var il = getterBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);
        }

        private static LambdaExpression CreateSetterLambda(Type type, string fieldName)
        {
            var fieldType = type.GetProperty(fieldName).PropertyType;

            var instance = Expression.Variable(type);
            var fieldValue = Expression.Variable(fieldType);

            var property = Expression.Property(instance, fieldName);
            var body = Expression.Assign(property, fieldValue);

            var lambda = Expression.Lambda(body, new[] { instance, fieldValue });
            return lambda;
        }

        private static LambdaExpression CreateGetterLambda(Type type, string fieldName)
        {
            var instance = Expression.Variable(type);
            var property = Expression.Property(instance, fieldName);
            var lambda = Expression.Lambda(property, new[] { instance });
            return lambda;
        }

        private ReportModel CreateModel(ReportDto report, IEnumerable<ReportFieldDto> fields, ulong version)
        {
            var reportTypeName = string.Format("ReportModel_{0}_{1}", report.Id, version);
            var assemblyName = new AssemblyName(string.Format("Report_{0}_{1}", report.Id, version));
            var reportType = default(Type);
            var fieldDefinitions = new List<ReportModel.ReportFieldDefinition>();
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == assemblyName.Name);

            if (assembly == null)
            {
                var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
                var moduleBuilder = assemblyBuilder.DefineDynamicModule("ReportModule");
                var enumProvider = new NewEnumProvider(moduleBuilder);
                var tb = moduleBuilder.DefineType(reportTypeName);
                tb.SetParent(typeof(ReportModel));

                foreach (var fieldDto in fields)
                {
                    var field = GetReportFieldDefinition(enumProvider, fieldDto);
                    fieldDefinitions.Add(field);

                    var fieldBuilder = tb.DefineField("_" + field.Name, field.Type, FieldAttributes.Public);
                    var getterBuilder = tb.DefineMethod("get_" + field.Name, MethodAttributes.Public, field.Type, new Type[0]);
                    ImplementGetter(getterBuilder, fieldBuilder);

                    var setterBuilder = tb.DefineMethod("set_" + field.Name, MethodAttributes.Public, null, new[] { field.Type });
                    ImplementSetter(setterBuilder, fieldBuilder);

                    var propertyBuilder = tb.DefineProperty(field.Name, PropertyAttributes.HasDefault, field.Type, new Type[0]);
                    propertyBuilder.SetGetMethod(getterBuilder);
                    propertyBuilder.SetSetMethod(setterBuilder);

                    var attributeProviders = GetAttributeProviders(fieldDto);
                    foreach (var attributeProvider in attributeProviders)
                    {
                        var attribute = attributeProvider.GetPropertyAttribute(fieldDto);
                        propertyBuilder.SetCustomAttribute(attribute);
                    }
                }

                reportType = tb.CreateType();
            }
            else
            {
                reportType = assembly.GetType(reportTypeName);
                if (reportType == null)
                {
                    throw new NotificationException(BLResources.ReportDialogError);
                }

                var enumProvider = new ExistingEnumProvider(assembly);
                fieldDefinitions.AddRange(fields.Select(fieldDto => GetReportFieldDefinition(enumProvider, fieldDto)));
            }

            var constructor = reportType.GetConstructor(new Type[0]);
            if (constructor == null)
            {
                throw new NotificationException(BLResources.ReportDialogError);
            }

            var instance = (ReportModel)constructor.Invoke(new object[0]);
            instance.Fields = fieldDefinitions;
            return instance;
        }

        private ReportModel.ReportFieldDefinition GetReportFieldDefinition(EnumProvider enumProvider, ReportFieldDto reportField)
        {
            switch (reportField.Type)
            {
                case ReportFieldType.Boolean:
                    return new ReportModel.ReportFieldDefinition
                    {
                        Type = typeof(bool),
                        Name = reportField.Name,
                        DefaultValue = GetDefaultBoolean(reportField.Default),
                    };

                case ReportFieldType.DateDay:
                    return new ReportModel.ReportFieldDefinition
                    {
                        Type = reportField.IsRequired ? typeof(DateTime) : typeof(DateTime?),
                        Name = reportField.Name,
                        Config = new DateTimeSettings { ShiftOffset = false, DisplayStyle = DisplayStyle.Full, PeriodType = PeriodType.None },
                        DefaultValue = GetDefaultDate(reportField.Default),
                    };

                case ReportFieldType.DateMonth:
                    return new ReportModel.ReportFieldDefinition
                    {
                        Type = reportField.IsRequired ? typeof(DateTime) : typeof(DateTime?),
                        Name = reportField.Name,
                        Config = new DateTimeSettings { ShiftOffset = false, DisplayStyle = DisplayStyle.WithoutDayNumber, PeriodType = GetDateTimePeriodType(reportField.Default) },
                        DefaultValue = GetDefaultDate(reportField.Default),
                    };

                case ReportFieldType.DropDownList:
                    var enumType = enumProvider.GetEnumType(reportField);
                    var enumLocalizationResourceManager = new RuntimeResourceManager(enumProvider.GetEnumName(reportField), reportField.ListValues);
                    return new ReportModel.ReportFieldDefinition
                    {
                        Type = enumType,
                        Name = reportField.Name,
                        Config = enumLocalizationResourceManager,
                        DefaultValue = GetDefaultEnumValue(enumType),
                    };

                case ReportFieldType.Lookup:
                    return BuildLookup(reportField);

                case ReportFieldType.PlainText:
                    return new ReportModel.ReportFieldDefinition
                        {
                            Type = typeof(string),
                            Name = reportField.Name,
                            DefaultValue = string.Empty,
                        };

                default:
                    throw new NotificationException("Unknown type " + reportField.Type);
            }
        }

        private ReportModel.ReportFieldDefinition BuildLookup(ReportFieldDto reportField)
        {
            switch (reportField.LookupEntityName)
            {
                case EntityName.User:
                    var userExtendedInfo = _reportSimplifiedModel.IsUserFromHeadBranch(UserContext.Identity.Code)
                                               ? null
                                               : "subordinatesOf=" + UserContext.Identity.Code;
                    return new ReportModel.ReportFieldDefinition
                        {
                            Type = typeof(LookupField),
                            Name = reportField.Name,
                            Config = new LookupSettings { EntityName = EntityName.User, ExtendedInfo = userExtendedInfo },
                            DefaultValue = GetDefaultUserLookup(),
                        };

                case EntityName.OrganizationUnit:
                    var organizationUnitExtendedInfo = _reportSimplifiedModel.IsUserFromHeadBranch(UserContext.Identity.Code)
                                                           ? new[] { reportField.LookupExtendedInfo }
                                                           : new[] { "userId=" + UserContext.Identity.Code, reportField.LookupExtendedInfo };
                    return new ReportModel.ReportFieldDefinition
                        {
                            Type = typeof(LookupField),
                            Name = reportField.Name,
                            Config = new LookupSettings
                                {
                                    EntityName = EntityName.OrganizationUnit,
                                    ExtendedInfo = string.Join("&", organizationUnitExtendedInfo.Where(s => !string.IsNullOrWhiteSpace(s)))
                                },
                            DefaultValue = GetDefaultOrganizationUnitLookup(),
                        };

                default:
                    return new ReportModel.ReportFieldDefinition
                    {
                        Type = typeof(LookupField),
                        Name = reportField.Name,
                        Config = new LookupSettings { EntityName = reportField.LookupEntityName, ExtendedInfo = reportField.LookupExtendedInfo },
                        DefaultValue = null,
                    };
            }
        }

        private PeriodType GetDateTimePeriodType(ReportFieldDefault field)
        {
            switch (field)
            {
                case ReportFieldDefault.DateTimeMonthStart:
                case ReportFieldDefault.DateTimeNextMonthStart:
                case ReportFieldDefault.DateTimePrevMonthStart:
                    return PeriodType.MonthlyLowerBound;
                case ReportFieldDefault.DateTimeMonthEnd:
                case ReportFieldDefault.DateTimeNextMonthEnd:
                case ReportFieldDefault.DateTimePrevMonthEnd:
                    return PeriodType.MonthlyUpperBound;
                default:
                    return PeriodType.None;
            }
        }

        private ActionResult ProceedsReport(ReportModel model)
        {
            var startDateField = model.GetType().GetProperty("StartDate");
            var startDate = (DateTime)startDateField.GetValue(model);
            var endDate = startDate.GetEndPeriodOfThisMonth();

            var clientProxy = _clientProxyFactory.GetClientProxy<IReportsApplicationService, BasicHttpBinding>();
            var fileDescription = clientProxy.Execute(service => service.ProceedsReport(startDate, endDate));

            return File(fileDescription.Stream, fileDescription.ContentType, fileDescription.FileName);
        }

        private ActionResult PlanningReport(ReportModel model)
        {
            var organizationUnitField = model.GetType().GetProperty("City");
            var planningMonthField = model.GetType().GetProperty("PlannedMonth");
            var isAdvertisingAgencyField = model.GetType().GetProperty("IsAdvertisingAgency");

            if (organizationUnitField == null || planningMonthField == null || isAdvertisingAgencyField == null)
            {
                throw new NotificationException(BLResources.WrongReportModelFormat);
            }

            var organizationUnit = (LookupField)organizationUnitField.GetValue(model);
            var planningMonth = (DateTime)planningMonthField.GetValue(model);
            var isAdvertisingAgency = (bool)isAdvertisingAgencyField.GetValue(model);

            var response = (PlanningReportResponse)_publicService.Handle(new PlanningReportRequest
            {
                OrganizationUnitId = organizationUnit.Key.Value,
                PlanningMonth = planningMonth,
                IsAdvertisingAgency = isAdvertisingAgency,
            });

            return File(response.OutputStream, response.ContentType, "PlanningReport.xlsx");

            return null;
        }

        #endregion

        #region Default value providers

        private object GetDefaultBoolean(ReportFieldDefault reportFieldDefault)
        {
            return reportFieldDefault == ReportFieldDefault.BooleanTrue;
        }

        private object GetDefaultEnumValue(Type enumType)
        {
            var values = enumType.GetEnumValues();
            return values.GetValue(0);
        }

        private object GetDefaultOrganizationUnitLookup()
        {
            var unit = _userRepository.GetFirstUserOrganizationUnit(UserContext.Identity.Code);
            return unit != null
                       ? new LookupField { Key = unit.Id, Value = unit.Name }
                       : new LookupField();
        }

        private object GetDefaultUserLookup()
        {
            var currentUser = UserContext.Identity;
            return new LookupField { Key = currentUser.Code, Value = currentUser.DisplayName };
        }

        private object GetDefaultDate(ReportFieldDefault def)
        {
            switch (def)
            {
                case ReportFieldDefault.DateTimeMonthStart:
                    return DateTime.Now.GetFirstDateOfMonth();
                case ReportFieldDefault.DateTimeNextMonthStart:
                    return DateTime.Now.GetNextMonthFirstDate();
                case ReportFieldDefault.DateTimePrevMonthStart:
                    return DateTime.Now.GetPrevMonthFirstDate();
                case ReportFieldDefault.DateTimeMonthEnd:
                    return DateTime.Now.GetFirstDateOfMonth().GetLastDateOfMonth();
                case ReportFieldDefault.DateTimeNextMonthEnd:
                    return DateTime.Now.GetNextMonthFirstDate().GetLastDateOfMonth();
                case ReportFieldDefault.DateTimePrevMonthEnd:
                    return DateTime.Now.GetPrevMonthFirstDate().GetLastDateOfMonth();
                default:
                    return DateTime.Now;
            }
        }

        #endregion

        private sealed class RuntimeResourceManager : ResourceManager
        {
            private readonly IDictionary<string, string> _strings;

            public RuntimeResourceManager(string enumName, IDictionary<long, string> enumDefinition)
                : base()
            {
                _strings = enumDefinition.ToDictionary(pair => enumName + "_" + pair.Key, pair => pair.Value);
            }

            public override string GetString(string name)
            {
                var value = default(string);
                return _strings.TryGetValue(name, out value)
                           ? value
                           : null;
            }

            public override string GetString(string name, CultureInfo ignored)
            {
                return GetString(name);
            }
        }

        #region AttributeProvider types

        private abstract class AttributeProvider
        {
            private readonly Type _attributeType;
            private readonly Type[] _constructorParameterTypes;
            private readonly object[] _constructorParameters;

            protected AttributeProvider(Type attributeType, object[] constructorParameters)
                : this(attributeType, constructorParameters, SelectTypes(constructorParameters))
            {
            }

            protected AttributeProvider(Type attributeType, object[] constructorParameters, Type[] constructorParameterTypes)
            {
                _attributeType = attributeType;
                _constructorParameterTypes = constructorParameterTypes;
                _constructorParameters = constructorParameters;
            }

            public CustomAttributeBuilder GetPropertyAttribute(ReportFieldDto fieldDto)
            {
                var attributeConstructor = _attributeType.GetConstructor(_constructorParameterTypes);
                if (attributeConstructor == null)
                {
                    var message = string.Format("Для аттрибута {0} не найден конструктор, принимающий {1}",
                        _attributeType.Name,
                        string.Join(", ", _constructorParameterTypes.Select(type => type.Name)));
                    throw new NotificationException(message);
                }

                return new CustomAttributeBuilder(attributeConstructor, _constructorParameters);
            }

            private static Type[] SelectTypes(IEnumerable<object> constructorParameters)
            {
                return constructorParameters.Select(o => o.GetType()).ToArray();
            }
        }

        private sealed class GreaterOrEqualThanAttributeProvider : AttributeProvider
        {
            public GreaterOrEqualThanAttributeProvider(string fieldName, string errorMessage)
                : base(typeof(GreaterOrEqualThanAttribute), new object[] { fieldName, errorMessage })
            {
            }
        }

        private class DependencyAttributeProvider : AttributeProvider
        {
            public DependencyAttributeProvider(DependencyDefinitionDto dependency)
                : base(typeof(DependencyAttribute), new object[] { dependency.DependencyType, dependency.FieldName, dependency.DependencyScript })
            {
            }
        }

        private class DisplayNameAttributeProvider : AttributeProvider
        {
            public DisplayNameAttributeProvider(string displayName)
                : base(typeof(DisplayNameAttribute), new object[] { displayName })
            {
            }
        }

        private class RequiredAttributeProvider : AttributeProvider
        {
            public RequiredAttributeProvider()
                : base(typeof(RequiredLocalizedAttribute), new object[0])
            {
            }
        }

        #endregion

        #region EnumProvider Nested Types

        private abstract class EnumProvider
        {
            public abstract Type GetEnumType(ReportFieldDto reportField);

            public string GetEnumName(ReportFieldDto reportField)
            {
                return string.Format("ReportModel_{0}", reportField.Name);
            }
        }

        private sealed class ExistingEnumProvider : EnumProvider
        {
            private readonly Assembly _assembly;

            public ExistingEnumProvider(Assembly assembly)
            {
                _assembly = assembly;
            }

            public override Type GetEnumType(ReportFieldDto reportField)
            {
                return _assembly.GetType(GetEnumName(reportField), false);
            }
        }

        private sealed class NewEnumProvider : EnumProvider
        {
            private readonly ModuleBuilder _moduleBuilder;

            public NewEnumProvider(ModuleBuilder moduleBuilder)
            {
                _moduleBuilder = moduleBuilder;
            }

            public override Type GetEnumType(ReportFieldDto reportField)
            {
                var typeName = GetEnumName(reportField);
                var enumBuilder = _moduleBuilder.DefineEnum(typeName, TypeAttributes.Public, typeof(long));

                foreach (var value in reportField.ListValues)
                {
                    enumBuilder.DefineLiteral("_" + value.Key, value.Key);
                }

                return enumBuilder.CreateType();
            }
        }

        #endregion
    }
}
