using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils
{
    public sealed class ModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder _modelBinder;

        public ModelBinderProvider()
        {
            _modelBinder = new DefaultModelBinder();
        }

        IModelBinder IModelBinderProvider.GetBinder(Type modelType)
        {
            return _modelBinder;
        }

        #region model binders

        private sealed class DefaultModelBinder : System.Web.Mvc.DefaultModelBinder
        {
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
                    return BindDateTime(bindingContext);
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
                    var undefinedValueAttribute = propertyType.GetCustomAttribute<UndefinedEnumValueAttribute>();
                    var undefinedValue = undefinedValueAttribute != null ? undefinedValueAttribute.Value : 0;

                    if (string.Equals(propertyDescriptor.PropertyType.GetEnumName(undefinedValue),
                                      modelState.Value.AttemptedValue,
                                      StringComparison.OrdinalIgnoreCase))
                    {
                        return ValidateRequiredAttribute(propertyDescriptor, modelState);
                    }
                }

                return true;
            }

            #region binding helper functions

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
                    valueProviderResult = new ValueProviderResult(rawValue, valueProviderResult.AttemptedValue, valueProviderResult.Culture);
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                    return rawValue;
                }

                // if cannot parse datetime, raise an error
                var errorMessage = string.Format("The value '{0}' is not applicable to the field {1}", valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
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
                        var errorMessage = string.Format("The value '{0}' is not applicable to the field {1}", valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
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
                var errorMessage = string.Format("The value '{0}' is not applicable to the field {1}", valueProviderResult.AttemptedValue, bindingContext.ModelMetadata.GetDisplayName());
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, errorMessage);
                return null;
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

            #endregion
        }

        #endregion
    }
}