using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils
{
    public sealed class ModelBinderProvider : IModelBinderProvider
    {
        private static readonly Type[] SupportedTypes = new[] 
        {
            typeof(string),
            typeof(Uri),
            typeof(LookupField),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
            typeof(Guid[]),
            typeof(decimal),
            typeof(decimal?),
            typeof(long),
            typeof(long?),
            typeof(IEntityType),
            typeof(IEntityType[])
        };
        private readonly IModelBinder _modelBinder;

        public ModelBinderProvider()
        {
            _modelBinder = new DefaultModelBinder();
        }

        IModelBinder IModelBinderProvider.GetBinder(Type modelType)
        {
            return IsSupportedType(modelType)
                       ? _modelBinder
                       : null;
        }

        private static bool IsSupportedType(Type modelType)
        {
            return SupportedTypes.Contains(modelType) || modelType.IsEnum;
        }

        private sealed class DefaultModelBinder : System.Web.Mvc.DefaultModelBinder
        {
            private const string ValueIsNotApplicableToField = "The value '{0}' is not applicable to the field {1}.";

            public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var modelType = bindingContext.ModelType;

                // STRING, trim
                if (modelType == typeof(string))
                {
                    return BindString(bindingContext);
                }

                // URI
                if (modelType == typeof(Uri))
                {
                    return BindUri(bindingContext);
                }

                // LOOKUP, parse from json
                if (modelType == typeof(LookupField))
                {
                    var lookup = BindLookupField(bindingContext);
                    if (lookup != null)
                    {
                        return lookup;
                    }
                }

                // DATETIME, parse in invariant culture
                if (modelType == typeof(DateTime) || modelType == typeof(DateTime?))
                {
                    return bindingContext.ModelMetadata.AdditionalValues.ContainsKey(CalendarAttribute.Name)
                               ? BindDateTimeAdvanced(bindingContext)
                               : BindDateTime(bindingContext);
                }

                // TIMESPAN
                if (bindingContext.ModelType == typeof(TimeSpan) || bindingContext.ModelType == typeof(TimeSpan?))
                {
                    return BindTimeSpan(bindingContext);
                }

                // GUID ARRAY, Dynamics CRM pass guid arrays splitted by comma
                if (modelType == typeof(Guid[]))
                {
                    return BindGuidArray(bindingContext);
                }

                // ENUM, interpret null as zero value
                if (modelType.IsEnum)
                {
                    return BindEnum(bindingContext);
                }

                if (modelType == typeof(long) || modelType == typeof(long?))
                {
                    return BindInt64(bindingContext);
                }

                if (modelType == typeof(decimal) || modelType == typeof(decimal?))
                {
                    return BindDecimal(bindingContext);
                }

                if (modelType == typeof(IEntityType))
                {
                    return BindEntityType(bindingContext);
                }

                if (modelType == typeof(IEntityType[]))
                {
                    return BindEntityTypeArray(bindingContext);
                }

                return base.BindModel(controllerContext, bindingContext);
            }

            protected override bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
            {
                var propertyName = propertyDescriptor.Name;
                var modelState = bindingContext.ModelState[propertyName];
                if (modelState == null)
                {
                    return true;
                }

                var propertyType = propertyDescriptor.PropertyType;

                // LOOKUP, validate "required" attribute on empty value
                if (propertyType == typeof(LookupField) && string.IsNullOrWhiteSpace(modelState.Value.AttemptedValue))
                {
                    return ValidateRequiredAttribute(propertyDescriptor, modelState);
                }

                // ENUM, validate "required" attribute on zero value
                if (propertyType.IsEnum)
                {
                    var undefinedValue = EnumUIUtils.GetDefaultValue(propertyType);

                    if (string.Equals(propertyDescriptor.PropertyType.GetEnumName(undefinedValue),
                                      modelState.Value.AttemptedValue,
                                      StringComparison.OrdinalIgnoreCase))
                    {
                        return ValidateRequiredAttribute(propertyDescriptor, modelState);
                    }
                }

                return true;
            }

            private static object BindString(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult == null || string.IsNullOrEmpty(valueProviderResult.AttemptedValue))
                {
                    return null;
                }

                return valueProviderResult.AttemptedValue.Trim();
            }

            private static object BindUri(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult == null || string.IsNullOrEmpty(valueProviderResult.AttemptedValue))
                {
                    return null;
                }

                Uri uri;
                Uri.TryCreate(valueProviderResult.AttemptedValue.Trim(), UriKind.RelativeOrAbsolute, out uri);
                return uri;
            }

            private static object BindGuidArray(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                if (valueProviderResult == null || string.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue))
                {
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return null;
                }

                var splittedAttemptedValue = valueProviderResult.AttemptedValue.Split(new[] { ',' });

                var guidList = new List<Guid>();

                foreach (var attemptedValue in splittedAttemptedValue)
                {
                    Guid guid;
                    if (Guid.TryParse(attemptedValue, out guid))
                    {
                        guidList.Add(guid);
                    }
                }

                var rawValue = guidList.ToArray();
                valueProviderResult = new ValueProviderResult(rawValue, valueProviderResult.AttemptedValue, valueProviderResult.Culture);
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                return rawValue;
            }

            private static object BindLookupField(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult == null)
                {   // не можем обработать данные для lookup в таком виде
                    return null;
                }

                var rawValue = LookupField.ParseFromJson(valueProviderResult.AttemptedValue);
                rawValue = rawValue ?? new LookupField();

                valueProviderResult = new ValueProviderResult(rawValue, valueProviderResult.AttemptedValue, valueProviderResult.Culture);
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                return rawValue;
            }

            private static object BindDateTime(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                // set null for nullable datetime
                if (bindingContext.ModelType == typeof(DateTime?) && (valueProviderResult == null || string.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue)))
                {
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return null;
                }

                DateTime rawValue;
                if (DateTime.TryParse(valueProviderResult.AttemptedValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out rawValue))
                {
                    valueProviderResult = new ValueProviderResult(rawValue, valueProviderResult.AttemptedValue, CultureInfo.InvariantCulture);
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return rawValue;
                }

                // if cannot parse datetime, raise an error
                var errorMessage = string.Format(ValueIsNotApplicableToField, valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, errorMessage);
                return null;
            }

            private static object BindDateTimeAdvanced(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                // set null for nullable datetime
                if (bindingContext.ModelType == typeof(DateTime?) && (valueProviderResult == null || string.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue)))
                {
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return null;
                }

                if (bindingContext.ModelType == typeof(DateTime) && (valueProviderResult == null || string.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue)))
                {
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return default(DateTime);
                }

                // Используем формат, определённый в iso-8601
                // Этот формат может нести информацию о часовом поясе, но при парсинге мы переводим его в UTC, 
                // благодаря чему в коде не возникнет вопросов "а в какой-же зоне это время" - структура DateTime не способна нести информацию о поясе.
                DateTimeOffset rawValue;
                if (DateTimeOffset.TryParse(valueProviderResult.AttemptedValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind | DateTimeStyles.AssumeUniversal, out rawValue))
                {
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return rawValue.UtcDateTime;
                }

                // if cannot parse datetime, raise an error
                var errorMessage = string.Format(ValueIsNotApplicableToField, valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, errorMessage);
                return null;
            }

            private static object BindTimeSpan(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                // set null for nullable TimeSpan
                if (bindingContext.ModelType == typeof(TimeSpan?) && (valueProviderResult == null || string.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue)))
                {
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return null;
                }

                DateTime rawValue;
                if (DateTime.TryParse(valueProviderResult.AttemptedValue, valueProviderResult.Culture, DateTimeStyles.AssumeLocal, out rawValue))
                {
                    var result = rawValue.TimeOfDay;
                    valueProviderResult = new ValueProviderResult(result, valueProviderResult.AttemptedValue, valueProviderResult.Culture);
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return result;
                }

                // if cannot parse datetime, raise an error
                var errorMessage = string.Format(ValueIsNotApplicableToField, valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, errorMessage);
                return null;
            }
            
            private static object BindEnum(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                if (valueProviderResult == null || string.Equals(valueProviderResult.AttemptedValue, "null", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue))
                {
                    var zeroValue = Enum.ToObject(bindingContext.ModelType, 0);
                    valueProviderResult = new ValueProviderResult(zeroValue, null, CultureInfo.InvariantCulture);
                }
                else
                {
                    try
                    {
                        var enumValue = Enum.Parse(bindingContext.ModelType, valueProviderResult.AttemptedValue);
                        valueProviderResult = new ValueProviderResult(enumValue, valueProviderResult.AttemptedValue, valueProviderResult.Culture);
                    }
                    catch (ArgumentException)
                    {
                        var errorMessage = string.Format(ValueIsNotApplicableToField, valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, errorMessage);
                        return null;
                    }
                }

                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                return valueProviderResult.RawValue;
            }

            private static object BindInt64(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult == null || string.IsNullOrEmpty(valueProviderResult.AttemptedValue) || string.Equals(valueProviderResult.AttemptedValue, "null"))
                {
                    return null;
                }

                // Если строка не парсится - кидаем исключение. TryParse не использован намеренно.
                long parsedInt64;
                if (long.TryParse(valueProviderResult.AttemptedValue, out parsedInt64))
                {
                    return parsedInt64;
                }

                // if cannot parse datetime, raise an error
                var errorMessage = string.Format(ValueIsNotApplicableToField, valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, errorMessage);
                return null;
            }

            private static object BindDecimal(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult == null || string.IsNullOrEmpty(valueProviderResult.AttemptedValue) || string.Equals(valueProviderResult.AttemptedValue, "null"))
                {
                    return null;
                }

                decimal parsedDecimal;
                if (decimal.TryParse(valueProviderResult.AttemptedValue, NumberStyles.Any, valueProviderResult.Culture, out parsedDecimal))
                {
                    return parsedDecimal;
                }

                var errorMessage = string.Format("The value '{0}' is not applicable to the field {1}", valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, errorMessage);
                return null;
            }

            private static object BindEntityType(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                if (valueProviderResult == null || string.Equals(valueProviderResult.AttemptedValue, "null", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue))
                {
                    var zeroValue = EntityType.Instance.None();
                    valueProviderResult = new ValueProviderResult(zeroValue, null, CultureInfo.InvariantCulture);
                }
                else
                {
                    try
                    {
                        IEntityType value;
                        if (EntityType.Instance.TryParse(valueProviderResult.AttemptedValue, out value))
                        {
                            valueProviderResult = new ValueProviderResult(value, valueProviderResult.AttemptedValue, valueProviderResult.Culture);
                        }
                    }
                    catch (ArgumentException)
                    {
                        var errorMessage = string.Format(ValueIsNotApplicableToField, valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, errorMessage);
                        return null;
                    }
                }

                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                return valueProviderResult.RawValue;
            }

            private static object BindEntityTypeArray(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                if (valueProviderResult == null || string.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue))
                {
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return null;
                }

                var splittedAttemptedValue = valueProviderResult.AttemptedValue.Split(new[] { ',' });

                var entityTypes = new List<IEntityType>();
                foreach (var attemptedValue in splittedAttemptedValue)
                {
                    IEntityType value;
                    if (EntityType.Instance.TryParse(attemptedValue, out value))
                    {
                        entityTypes.Add(value);
                    }
                }

                var rawValue = entityTypes.ToArray();
                valueProviderResult = new ValueProviderResult(rawValue, valueProviderResult.AttemptedValue, valueProviderResult.Culture);
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                return rawValue;
            }

            private static bool ValidateRequiredAttribute(MemberDescriptor memberDescriptor, ModelState modelState)
            {
                var requiredAttribute = (RequiredAttribute)memberDescriptor.Attributes[typeof(RequiredAttribute)];
                if (requiredAttribute == null)
                {
                    return true;
                }

                modelState.Errors.Add(requiredAttribute.FormatErrorMessage(memberDescriptor.DisplayName));
                return false;
            }
        }
    }
}