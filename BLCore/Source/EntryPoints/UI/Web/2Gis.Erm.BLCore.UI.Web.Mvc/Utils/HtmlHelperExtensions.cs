using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils
{
    // ReSharper disable InconsistentNaming
    public enum FieldFlex
    {
        lone,
        twins,
        triplet,
        quadruplet
    }
    // ReSharper restore InconsistentNaming

    public static class HtmlHelperExtensions
    {
        // ReSharper disable UnusedMember.Local
        private enum InputType
        {
            Text,
            TextArea,
            Password,
            Radio,
            CheckBox,
            DropDown,
            Lookup,
            Date,
            YesNoRadio
        }
        // ReSharper restore UnusedMember.Local

        private static IEnumItemsCache _enumItemsCache;

        public static void InitEnumItemsCache(IEnumItemsCache enumItemsCache)
        {
            _enumItemsCache = enumItemsCache;
        }

        public static HtmlHelper CreateHtmlHelper(HtmlHelper helper, Type newGenericParameter)
        {
            var genericType = typeof(HtmlHelper<>);

            var helperType = genericType.MakeGenericType(newGenericParameter);

            var helperConstructor = helperType.GetConstructor(new Type[]
                {
                    typeof(ViewContext),
                    typeof(IViewDataContainer),
                    typeof(RouteCollection)
                });

            if (helperConstructor == null)
            {
                return null;
            }

            return (HtmlHelper)helperConstructor.Invoke(new object[]
                {
                    helper.ViewContext,
                    helper.ViewDataContainer,
                    helper.RouteCollection
                });
        }

        public static IHtmlString Resource<TKey>(this HtmlHelper helper, Expression<Func<TKey>> resourceEntryExpression)
        {
            var resourceEntryKey = ResourceEntryKey.Create(resourceEntryExpression);
            var resourceManager = resourceEntryKey.ResourceHostType.AsResourceManager();
            return helper.Raw(resourceManager.GetString(resourceEntryKey.ResourceEntryName));
        }

        public static MvcHtmlString TemplateGenericField(this HtmlHelper helper, Expression e, object optional)
        {
            var expressionFunc = e.GetType().GetGenericArguments().Single();
            var modelType = expressionFunc.GetGenericArguments().First();
            var fieldType = expressionFunc.GetGenericArguments().Last();

            // Хелпер в представлении Report/Edit параметризован моделью ReportModel, 
            // в то время как реально используется её наследник и выражение e параметризовано им.
            var helperInstance = CreateHtmlHelper(helper, modelType);

            var methods = typeof(HtmlHelperExtensions).GetMethods()
                                                       .Where(info => info.Name == "TemplateField")
                                                       .ToArray();

            var typedMethod = default(MethodInfo);
            var methodParams = default(object[]);

            if (fieldType == typeof(LookupField) || fieldType == typeof(DateTime?) || fieldType == typeof(DateTime))
            {
                var genericMethod = methods.Where(CreateFieldTypeCheck(fieldType))
                                             .FirstOrDefault(info => info.GetParameters().Count() == 4);

                typedMethod = genericMethod != null
                                  ? genericMethod.MakeGenericMethod(new[] { modelType })
                                  : null;
                methodParams = new[] { helperInstance, e, FieldFlex.lone, optional };
            }
            else
            {
                var genericMethod = methods.Where(info =>
                    {
                        var parameters = info.GetParameters();
                        return parameters.Length == 5 &&
                               parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(HtmlHelper<>) &&
                               parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                               parameters[2].ParameterType == typeof(FieldFlex) &&
                               parameters[3].ParameterType == typeof(IDictionary<string, object>) &&
                               parameters[4].ParameterType == typeof(ResourceManager);
                    })
                    .FirstOrDefault();

                typedMethod = genericMethod != null
                                  ? genericMethod.MakeGenericMethod(new[] { modelType, fieldType })
                                  : null;
                methodParams = new object[] { helperInstance, e, FieldFlex.lone, null, optional as ResourceManager };
            }

            if (typedMethod == null)
            {
                return new MvcHtmlString("<span>Ошибка формирования поля</span>");
            }

            return (MvcHtmlString)typedMethod.Invoke(null, methodParams);
        }

        private static Func<MethodInfo, bool> CreateFieldTypeCheck(Type parameterFieldType)
        {
            return info =>
                {
                    var expressionParameter = info.GetParameters()
                        .FirstOrDefault(p => p.ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));
                    if (expressionParameter == null)
                    {
                        return false;
                    }

                    var funcType = expressionParameter.ParameterType.GetGenericArguments().FirstOrDefault();
                    if (funcType == null || funcType.GetGenericTypeDefinition() != typeof(Func<,>))
                    {
                        return false;
                    }

                    // тип свойства модели в конкретной сигнатуре функции
                    var fieldType = funcType.GenericTypeArguments[1];

                    return fieldType == parameterFieldType;
                };
        }

        #region date

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DateFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field)
        {
            return DateFor(htmlHelper, field, null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DateFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field, DateTimeSettings settings)
        {
            settings = settings ?? new DateTimeSettings();

            var rawName = ExpressionHelper.GetExpressionText(field);
            var name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(rawName);
            var id = name.Replace(".", HtmlHelper.IdAttributeDotReplacement); 
            var value = (DateTime?)ModelMetadata.FromLambdaExpression(field, htmlHelper.ViewData).Model;
            var sb = new StringBuilder(100);
            sb.Append("<script type=\"text/javascript\">");
            sb.Append("Ext.onReady(function () {");
            sb.Append("new Ext.ux.Calendar({");
            sb.AppendFormat("id: '{0}',", id);
            sb.AppendFormat("showToday: {0},", settings.ShowToday.ToString(CultureInfo.InvariantCulture).ToLower());
            sb.AppendFormat("periodType: {0},", (int)settings.PeriodType);
            sb.AppendFormat("displayStyle: {0},", (int)settings.DisplayStyle);
            sb.AppendFormat("shiftOffset: {0},", settings.ShiftOffset.ToString(CultureInfo.InvariantCulture).ToLower());
            if (settings.Disabled)
                sb.Append("disabled: true,");
            if (settings.ReadOnly)
                sb.Append("readOnly: true,");
            if (settings.MinDate.HasValue)
                sb.AppendFormat("minValue: '{0}',", settings.MinDate.Value.Date.ToString(CultureInfo.InvariantCulture));
            if (settings.MaxDate.HasValue)
                sb.AppendFormat("maxValue: '{0}',", settings.MaxDate.Value.Date.ToString(CultureInfo.InvariantCulture));
            sb.AppendFormat("applyTo: '{0}'", id);
            sb.Append("})});</script>");

            var htmlDateString = value.HasValue ? (value.Value.ToString(CultureInfo.InvariantCulture)) : BLResources.NotSet;
            sb.Append(htmlHelper.TextBox(rawName, htmlDateString, new Dictionary<string, object> { { "id", id }, { "class", "inputfields x-calendar" } }).ToString());
            return MvcHtmlString.Create(sb.ToString());
        }

        #endregion

        #region yesnoradio

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString YesNoRadioFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> field)
        {
            return YesNoRadioFor(htmlHelper, field, null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString YesNoRadioFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> field, IDictionary<string, object> htmlAttributes)
        {
            var itemBuilder = new StringBuilder();
            itemBuilder.Append(htmlHelper.RadioButtonFor(field, true, htmlAttributes).ToHtmlString());
            itemBuilder.Append(BLResources.Yes);
            itemBuilder.Append(htmlHelper.RadioButtonFor(field, false,htmlAttributes).ToHtmlString());
            itemBuilder.Append(BLResources.No);
            return new MvcHtmlString(itemBuilder.ToString());
        }

        #endregion

        #region lookup

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString LookupFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, LookupField>> field, LookupSettings lookupSettings)
        {
            if (lookupSettings == null)
                throw new ArgumentNullException("lookupSettings");

            var name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(field));
            var sb = new StringBuilder();
            sb.Append("<script type=\"text/javascript\">");
            sb.Append("if (!Ext.CardLookupSettings) {");
            sb.Append("Ext.CardLookupSettings = [];");
            sb.Append("}");
            sb.Append("Ext.CardLookupSettings.push({");
            sb.AppendFormat("disabled:{0}, ", lookupSettings.Disabled.ToString(CultureInfo.InvariantCulture).ToLower());
            sb.AppendFormat("readOnly:{0}, ", lookupSettings.ReadOnly.ToString(CultureInfo.InvariantCulture).ToLower());
            sb.AppendFormat("showReadOnlyCard:{0}, ", lookupSettings.ShowReadOnlyCard.ToString(CultureInfo.InvariantCulture).ToLower());
            sb.AppendFormat("supressMatchesErrors:{0}, ", lookupSettings.SupressMatchesErrors.ToString(CultureInfo.InvariantCulture).ToLower());
            sb.AppendFormat("id:\"{0}\", ", name);
            sb.AppendFormat("name:\"{0}\", ", name);
            sb.AppendFormat("applyTo:\"{0}\", ", name);
            sb.AppendFormat("autoUpdate: {0},", lookupSettings.AutoUpdate.ToString(CultureInfo.InvariantCulture).ToLower());
            sb.AppendFormat("entityName:\"{0}\", ", lookupSettings.EntityName);
            sb.AppendFormat("extendedInfo:\"{0}\", ", lookupSettings.ExtendedInfo);
            sb.AppendFormat("parentEntityName:\"{0}\", ", lookupSettings.ParentEntityName);
            sb.AppendFormat("parentIdPattern:\"{0}\"", lookupSettings.ParentIdPattern);
            if (lookupSettings.Plugins != null && lookupSettings.Plugins.Any())
            {
                sb.AppendFormat(",plugins:[{0}] ", string.Join(",", lookupSettings.Plugins));
            }
            if (lookupSettings.DataFields != null && lookupSettings.DataFields.Any())
            {
                sb.AppendFormat(",tplFields:{0}", WriteJson(htmlHelper, lookupSettings.DataFields));
            }
            if (!string.IsNullOrEmpty(lookupSettings.HeaderTextTemplate))
            {
                sb.AppendFormat(",tplHeaderTextTemplate:{0}", lookupSettings.HeaderTextTemplate);
            }
            sb.Append("});</script>");
            sb.Append(htmlHelper.TextBoxFor(field).ToString());

            return MvcHtmlString.Create(sb.ToString());
        }

        #endregion

        #region enums

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, ResourceManager resourceManager, IDictionary<string, object> htmlAttributes, IEnumerable<object> allowedValues = null)
        {
            var member = (PropertyInfo)((MemberExpression)expression.Body).Member;

            var enumType = typeof(TProperty);
            if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                enumType = enumType.GetGenericArguments()[0];
            }

            IEnumerable<SelectListItem> items;
            if (enumType.IsEnum)
            {
                var excludeZeroValue = false;
                var excludeZeroValueAttributes = (ExcludeZeroValue[])member.GetCustomAttributes(typeof(ExcludeZeroValue), false);
                if (excludeZeroValueAttributes.Length != 0)
                {
                    excludeZeroValue = true;
                }
                items = EnumGetLocalizedNames(enumType, resourceManager, excludeZeroValue);
            }
            else if(htmlAttributes.ContainsKey("combobox"))
            {
                items = new SelectListItem[] {};
            }
            else
            {
                throw new NotSupportedException();
            }

            if (allowedValues != null)
            {
                var allowedStringValues = allowedValues.Select(v => Convert.ToInt32(v) == 0 ? string.Empty : v.ToString()).ToArray();
                items = items.Where(i => allowedStringValues.Contains(i.Value)).ToArray();
            }

            // todo: implement "default value" concept instead of "excludeZeroValue" concept
            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }

        private static IEnumerable<SelectListItem> EnumGetLocalizedNames(Type enumType, ResourceManager resourceManager, bool excludeZeroValue)
        {
            var enumItems = _enumItemsCache.GetOrAdd(enumType);

            IEnumerable<SelectListItem> enumLocalizedNames;
            if (resourceManager != null)
            {
                enumLocalizedNames =
                    enumItems.Select(x => new SelectListItem() { Value = x.Value, Text = resourceManager.GetString(enumType.Name + x.EnumItemName) });
            }
            else
            {
                enumLocalizedNames = enumItems.Select(x => new SelectListItem() { Value = x.Value, Text = x.EnumItemName });
            }

            if (excludeZeroValue)
            {
                enumLocalizedNames = enumLocalizedNames.Where(x => x.Value.Length > 0);
            }

            return enumLocalizedNames;
        }

        #endregion

        #region template field

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString EditableId<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field)
        {
            const string script = "<script language=\"javascript\">new window.Ext.ux.IdField(\"Id\");</script>";
            const string scriptDisabled = "<script language=\"javascript\">var id = new window.Ext.ux.IdField(\"Id\"); id.disable(); id.setValue('{0}');</script>";
            long id;
            var metadata = ModelMetadata.FromLambdaExpression(field, htmlHelper.ViewData);
            if (long.TryParse(metadata.Model.ToString(), out id))
            {
                return id == 0
                           ? new MvcHtmlString(TemplateField(htmlHelper, field, FieldFlex.lone).ToHtmlString() + script)
                           : new MvcHtmlString(TemplateField(htmlHelper, field, FieldFlex.lone).ToHtmlString() + string.Format(scriptDisabled, id));
            }

            return TemplateField(htmlHelper, field, FieldFlex.lone);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field, FieldFlex wrapperCls)
        {
            return TemplateField(htmlHelper, field, wrapperCls, null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field, FieldFlex wrapperCls, IDictionary<string, object> htmlAttributes)
        {
            return TemplateField(htmlHelper, field, wrapperCls, htmlAttributes, null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field, FieldFlex wrapperCls, IDictionary<string, object> htmlAttributes, ResourceManager resourceManager)
        {
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(field));
            var itemBuilder = new StringBuilder(50);
            htmlHelper.RenderTemplateHead(field, itemBuilder, wrapperCls, fieldName);
            itemBuilder.Append(RenderFieldInput(htmlHelper, field, htmlAttributes, resourceManager));
            htmlHelper.RenderTemplateBottom(field, itemBuilder);
            return new MvcHtmlString(itemBuilder.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString EnumField<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field, FieldFlex wrapperCls, IDictionary<string, object> htmlAttributes, ResourceManager resourceManager, IEnumerable<object> availableValues)
        {
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(field));
            var itemBuilder = new StringBuilder(50);
            htmlHelper.RenderTemplateHead(field, itemBuilder, wrapperCls, fieldName);
            itemBuilder.Append(RenderFieldInput(htmlHelper, field, htmlAttributes, resourceManager, availableValues));
            htmlHelper.RenderTemplateBottom(field, itemBuilder);
            return new MvcHtmlString(itemBuilder.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime>> field, FieldFlex wrapperCls)
        {
            return TemplateField(htmlHelper, field, wrapperCls, (DateTimeSettings)null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime?>> field, FieldFlex wrapperCls)
        {
            return TemplateField(htmlHelper, field, wrapperCls, (DateTimeSettings)null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime>> field, FieldFlex wrapperCls, DateTimeSettings settings)
        {
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(field));
            var itemBuilder = new StringBuilder();
            htmlHelper.RenderTemplateHead(field, itemBuilder, wrapperCls, fieldName);
            itemBuilder.Append(htmlHelper.DateFor(field, settings).ToHtmlString());
            htmlHelper.RenderTemplateBottom(field, itemBuilder);
            return new MvcHtmlString(itemBuilder.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime?>> field, FieldFlex wrapperCls, DateTimeSettings settings)
        {
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(field));
            var itemBuilder = new StringBuilder();
            htmlHelper.RenderTemplateHead(field, itemBuilder, wrapperCls, fieldName);
            itemBuilder.Append(htmlHelper.DateFor(field, settings).ToHtmlString());
            htmlHelper.RenderTemplateBottom(field, itemBuilder);
            return new MvcHtmlString(itemBuilder.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime>> field, FieldFlex wrapperCls, CalendarSettings settings)
        {
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(field));
            var itemBuilder = new StringBuilder();
            htmlHelper.RenderTemplateHead(field, itemBuilder, wrapperCls, fieldName);
            itemBuilder.Append(htmlHelper.EditorFor(field, "DateTimeViewModel", new { CalendarSettings = settings }).ToHtmlString());
            htmlHelper.RenderTemplateBottom(field, itemBuilder);
            return new MvcHtmlString(itemBuilder.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime?>> field, FieldFlex wrapperCls, CalendarSettings settings)
        {
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(field));
            var itemBuilder = new StringBuilder();
            htmlHelper.RenderTemplateHead(field, itemBuilder, wrapperCls, fieldName);
            itemBuilder.Append(htmlHelper.EditorFor(field, "DateTimeViewModel", new { CalendarSettings = settings }).ToHtmlString());
            htmlHelper.RenderTemplateBottom(field, itemBuilder);
            return new MvcHtmlString(itemBuilder.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TemplateField<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, LookupField>> field, FieldFlex wrapperCls, LookupSettings settings)
        {
            var fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(field));
            var itemBuilder = new StringBuilder();
            htmlHelper.RenderTemplateHead(field, itemBuilder, wrapperCls, fieldName);
            itemBuilder.Append(htmlHelper.LookupFor(field, settings).ToHtmlString());
            htmlHelper.RenderTemplateBottom(field, itemBuilder);
            return new MvcHtmlString(itemBuilder.ToString());
        }

        private static HtmlString RenderFieldInput<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field, IDictionary<string, object> htmlAttributes = null, ResourceManager resourceManager = null, IEnumerable<object> availableValues = null)
        {
            var attributes = htmlAttributes ?? new Dictionary<string, object>();
            var propertyType = typeof(TProperty);

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                propertyType = propertyType.GetGenericArguments()[0];

            // enum
            if (propertyType.IsEnum)
                return htmlHelper.DropDownListFor(field, resourceManager, attributes.AddCssClassForType(InputType.DropDown), availableValues);

            // Для текстовых полей с аттрибутом "combobox" рендерим пустой выпадающий 
            // список. Он может использоваться для динамического заполнения с помощью js.
            if(propertyType == typeof(String) && attributes.ContainsKey("combobox"))
            {
                return htmlHelper.DropDownListFor(field, resourceManager, attributes.AddCssClassForType(InputType.DropDown));
            }

            switch (Type.GetTypeCode(propertyType))
            {
                case TypeCode.String:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Byte:
                    return (attributes.ContainsKey("rows") || attributes.ContainsKey("cols")) ? htmlHelper.TextAreaFor(field, attributes.AddCssClassForType(InputType.TextArea)) : htmlHelper.TextBoxFor(field, attributes.AddCssClassForType(InputType.Text));
                case TypeCode.Boolean:
                    var boolExpression = Expression.Lambda<Func<TModel, bool>>(field.Body, false, field.Parameters[0]);

                    var test = field.Body as MemberExpression;
                    if (test != null && test.Member.IsDefined(typeof(YesNoRadioAttribute), false))
                    {
                        return htmlHelper.YesNoRadioFor(boolExpression, attributes.AddCssClassForType(InputType.YesNoRadio));
                    }
                    return htmlHelper.CheckBoxFor(boolExpression, attributes.AddCssClassForType(InputType.CheckBox));
                default: 
                    throw new NotSupportedException();
            }
        }

        private static IDictionary<string, object> AddCssClassForType(this IDictionary<string, object> attributes, InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Text:
                case InputType.TextArea:
                case InputType.DropDown:
                    if (attributes.ContainsKey("class"))
                        attributes["class"] = attributes["class"] + " inputfields";
                    else
                        attributes.Add("class", "inputfields");
                    break;

                case InputType.CheckBox:
                    if (attributes.ContainsKey("class"))
                        attributes["class"] = attributes["class"] + " rad";
                    else
                        attributes.Add("class", "rad");
                    break;
                case InputType.YesNoRadio:
                    if (attributes.ContainsKey("class"))
                        attributes["class"] = attributes["class"] + " radio-field";
                    else
                        attributes.Add("class", "radio-field");
                    break;
            }

            if (attributes.ContainsKey("readonly") && attributes["readonly"].ToString() == "readonly")
                if (attributes.ContainsKey("class")) attributes["class"] = attributes["class"] + " ReadOnly"; else attributes.Add("class", "ReadOnly");

            if (attributes.ContainsKey("disabled") && attributes["disabled"].ToString() == "disabled")
                if (attributes.ContainsKey("class")) attributes["class"] = attributes["class"] + " ReadOnly"; else attributes.Add("class", "ReadOnly");

            return attributes;
        }

        private static void RenderTemplateBottom<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field, StringBuilder itemBuilder)
        {
            var validationMessage = htmlHelper.ValidationMessageFor(field, null, new {@class = "error"});
            RenderTemplateBottom(validationMessage, itemBuilder);
        }

        private static void RenderTemplateBottom(MvcHtmlString validationMessage, StringBuilder itemBuilder)
        {
            itemBuilder.Append(validationMessage);
            itemBuilder.Append("</div>");
            itemBuilder.Append("</div>");
        }

        private static void RenderTemplateHead<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> field, StringBuilder itemBuilder, FieldFlex wrapperCls, string fieldName)
        {
            RenderTemplateHead(htmlHelper.LabelFor(field), itemBuilder, wrapperCls, fieldName);
        }

        private static void RenderTemplateHead(MvcHtmlString label, StringBuilder itemBuilder, FieldFlex wrapperCls, string fieldName)
        {
            itemBuilder.AppendFormat("<div class=\"display-wrapper field-wrapper {0}\" id=\"{1}-wrapper\">", wrapperCls, fieldName);
            itemBuilder.AppendFormat("<div class=\"label-wrapper\" id=\"{0}-caption\">", fieldName);
            itemBuilder.Append(label.ToHtmlString());
            itemBuilder.Append(":</div>");
            itemBuilder.Append("<div class=\"input-wrapper\">");
        }

        public static MvcHtmlString SectionHead<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string title)
        {
            return new MvcHtmlString(string.Format("<div id='{0}' class='section-bar'><span>{1}</span></div><br/>", id, title));
        }

        public static MvcHtmlString SectionRow<TModel>(this HtmlHelper<TModel> htmlHelper, params MvcHtmlString[] content)
        {
            return SectionRow(htmlHelper, null, content);
        }

        public static MvcHtmlString SectionRow<TModel>(this HtmlHelper<TModel> htmlHelper, object htmlAttributes = null, params MvcHtmlString[] content)
        {
            var builder = new TagBuilder("div");
            builder.AddCssClass("row-wrapper");
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            builder.InnerHtml = content.Aggregate(new StringBuilder(), (acc, item) => acc.AppendLine(item.ToHtmlString())).ToString();
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString Section<TModel>(this HtmlHelper<TModel> htmlHelper, string tagName, object htmlAttributes = null, params MvcHtmlString[] content)
        {
            return Section(htmlHelper, tagName, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), content);
        }

        public static MvcHtmlString Section<TModel>(this HtmlHelper<TModel> htmlHelper, string tagName, IDictionary<string, object> htmlAttributes = null, params MvcHtmlString[] content)
        {
            var builder = new TagBuilder(tagName);
            builder.MergeAttributes(htmlAttributes);
            builder.InnerHtml = content.Aggregate(new StringBuilder(), (acc, item) => acc.AppendLine(item.ToHtmlString())).ToString();
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString EmptyBlock<TModel>(this HtmlHelper<TModel> htmlHelper, FieldFlex wrapperCls)
        {
            return new MvcHtmlString(string.Format("<div class=\"display-wrapper field-wrapper {0}\"></div>", wrapperCls));
        }

        #endregion

        #region json serialization

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString WriteJson<TModel>(this HtmlHelper<TModel> htmlHelper, object data)
        {
            return new MvcHtmlString(data == null ? "null" : JsonConvert.SerializeObject(data, Formatting.None));
        }

        #endregion

        #region MergeData
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString MergeDataSectionHeader<TModel>(this HtmlHelper<TModel> htmlHelper, string sectionId, string title, bool isDisabled = false)
        {
            return new MvcHtmlString(string.Format("<tr class=\"section-header\" height=" + (isDisabled ? "\"1\"" : "\"23\"") + (isDisabled ? "disabled=\"true\"" : string.Empty) + "  >" +
            "<td>&nbsp;</td><td class=\"datacell\">"+
                "<input  type=\"radio\" name=\"{0}\" id=\"{0}_1\" checked=\"checked\" class=\"rad section left\"/>" +
            "</td><td class=\"datacell\"><label for=\"{0}_1\">{1}</label>"+
            "</td><td class=\"datacell\"><input type=\"radio\" name=\"{0}\" id=\"{0}_2\" class=\"rad section right\"/>"+
            "</td><td class=\"datacell\"><label for=\"{0}_2\">{1}</label></td></tr>", sectionId, title));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString MergeDataRowFor<TModel, TLeftProperty, TRightProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                                           Expression<Func<TModel, TLeftProperty>> leftProp,
                                                                                           Expression<Func<TModel, TRightProperty>> rightProp,
                                                                                           string sectionId,
                                                                                           ResourceManager resourceManager = null,
                                                                                           bool isReadonly = false,
                                                                                           bool useRadioButtons = true)
        {
            const string emptyText = "&nbsp;";

            var propName = ((MemberExpression)leftProp.Body).Member.Name;
            var leftPropValue = ModelMetadata.FromLambdaExpression(leftProp, htmlHelper.ViewData).Model;

            if (IsNullLookup(leftPropValue))
            {
                leftPropValue = null;
            }

            var rightPropValue = ModelMetadata.FromLambdaExpression(rightProp, htmlHelper.ViewData).Model;
            if (IsNullLookup(rightPropValue))
            {
                rightPropValue = null;
            }

            var leftPropDisplay = GetPropertyDisplayValue(typeof(TLeftProperty), leftPropValue, resourceManager, emptyText);
            var rightPropDisplay = GetPropertyDisplayValue(typeof(TRightProperty), rightPropValue, resourceManager, emptyText);
            if (leftPropDisplay == emptyText && rightPropDisplay == emptyText)
            {
                return new MvcHtmlString(string.Empty);
            }

            var sb = new StringBuilder();
            sb.Append("<tr height=\"23\"><td>");
            sb.Append(htmlHelper.LabelFor(leftProp));
            sb.Append("</td><td class=\"datacell\">");
            if (useRadioButtons)
            {
                sb.AppendFormat("<input id=\"{0}_1\" class=\"rad left {1}\" type=\"radio\" name=\"{0}\" value=\"{2}\" {3}>",
                                propName,
                                sectionId,
                                HttpUtility.HtmlEncode(leftPropValue),
                                isReadonly ? " readonly = 'true'" : string.Empty);
            }

            sb.Append("</td><td class=\"datacell\">");
            sb.AppendFormat("<label for=\"{0}_1\">{1}</label>", propName, leftPropDisplay);
            sb.Append("</td><td class=\"datacell\">");
            if (useRadioButtons)
            {
                sb.AppendFormat("<input id=\"{0}_2\" class=\"rad right {1}\" type=\"radio\" name=\"{0}\" value=\"{2}\" {3}>",
                                propName,
                                sectionId,
                                HttpUtility.HtmlEncode(rightPropValue),
                                isReadonly ? "readonly = 'true'" : string.Empty);
            }

            sb.Append("</td><td class=\"datacell\">");
            sb.AppendFormat("<label for=\"{0}_2\">{1}</label>", propName, rightPropDisplay);
            sb.Append("</td></tr>");
            return new MvcHtmlString(sb.ToString());
        }

        private static bool IsNullLookup(object value)
        {
            return (value is LookupField) && (value as LookupField).Key == null && (value as LookupField).Value == null;
        }


        public static string GetPropertyDisplayValue(Type propType, object propValue, ResourceManager resourceManager, string emptyText)
        {
            if (propValue == null || string.IsNullOrEmpty(propValue.ToString()))
            {
                return emptyText;
            }
            if(propType.IsEnum)
            {
                var type = propValue.GetType();
                var name = type.GetEnumName(propValue);
                if (resourceManager == null)
                    return name;
                var localizedName = resourceManager.GetString(type.Name + name);
                return localizedName ?? name;
            }
            if ((propType == typeof(LookupField)))
            {
                return ((LookupField)propValue).Value;
            }
            if ((propType == typeof(Boolean)))
            {
                return (Boolean)propValue ? BLResources.Yes : BLResources.No;
            }
            return propValue.ToString();
        }

        #endregion
    }
}
