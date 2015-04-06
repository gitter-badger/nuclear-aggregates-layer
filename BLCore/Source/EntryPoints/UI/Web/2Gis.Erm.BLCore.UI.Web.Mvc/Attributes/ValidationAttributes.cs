using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes
{
    /// <summary>
    /// Атрибут валидации условия, что значение одного свойства класса больше чем значение другого свойства
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class GreaterOrEqualThanAttribute : ValidationAttribute
    {
        private readonly object _typeId = new object();

        public GreaterOrEqualThanAttribute(string anotherProperty)
            : this(anotherProperty, null)
        {
        }

        public GreaterOrEqualThanAttribute(string anotherProperty, string message)
        {
            AnotherProperty = anotherProperty;
            ErrorMessage = message;
        }

        public override object TypeId
        {
            get { return _typeId; }
        }

        public string AnotherProperty { get; private set; }

        public override bool IsValid(object value)
        {
            throw new InvalidOperationException("GreaterOrEqualThanValidator has not been registered");
        }
    }

    // for enum model properties, exclude enum zero value from html dropdown
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ExcludeZeroValue: Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RequiredLocalizedAttribute : RequiredAttribute, IClientValidatable
    {
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.InvariantCulture, ResPlatform.RequiredFieldMessage, name);
        }

        IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "required",

            };
            return new[] { rule };
        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class StringLengthLocalizedAttribute : StringLengthAttribute, IClientValidatable
    {
        public StringLengthLocalizedAttribute(int maximumLength) : base(maximumLength) { }

        public override string FormatErrorMessage(string name)
        {
            if (MaximumLength < 0)
                return string.Format(CultureInfo.InvariantCulture,
                                     BLResources.StringLengthLocalizedAttribute_InvalidMaxLength);

            if (MinimumLength == 0)
            {
                return string.Format(CultureInfo.InvariantCulture,
                                     BLResources.StringLengthLocalizedAttribute_ValidationError,
                                     name, MaximumLength);
            }
            return string.Format(CultureInfo.InvariantCulture, MaximumLength == MinimumLength ? BLResources.StringLengthLocalizedAttribute_ValidationErrorEqualsLimitations : BLResources.StringLengthLocalizedAttribute_ValidationErrorIncludingMinimum, name, MaximumLength, MinimumLength);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "stringlength",
            };
            rule.ValidationParameters.Add(new KeyValuePair<string, object>("minimumLength", MinimumLength));
            rule.ValidationParameters.Add(new KeyValuePair<string, object>("maximumLength", MaximumLength));
            return new[] { rule };
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class NonZeroIntegerAttribute : ValidationAttribute
    {
        public NonZeroIntegerAttribute()
            : base()
        {
        }

        public override bool IsValid(object value)
        {
            long parsed;
            return long.TryParse(value.ToString(), out parsed) && parsed != 0;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ResPlatform.InappropriateValueForField, name);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class EmailLocalizedAttribute : ValidationAttribute, IClientValidatable
    {
        private static readonly EmailAddressAttribute BaseAttribute = new EmailAddressAttribute();
        private static readonly Regex LatinCharsetDetector = new Regex("[a-z]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CyrillicCharsetDetector = new Regex("[а-я]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.InvariantCulture, ResPlatform.InputValidationInvalidEmail, name);
        }

        public override bool IsValid(object value)
        {
            if (value == null || value.ToString().Length == 0)
            {
                return true;
            }

            return BaseAttribute.IsValid(value) && ValidCharset(value.ToString());
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "email",
            };

            return new[] { rule };
        }

        private bool ValidCharset(string value)
        {
            var email = new MailAddress(value);
            var userCharsetCount = CharsetCount(email.User);
            var domainCharsetCount = CharsetCount(email.Host);
            return userCharsetCount <= 1 && domainCharsetCount <= 1;
        }

        private int CharsetCount(string s)
        {
            var latinUsed = LatinCharsetDetector.IsMatch(s) ? 1 : 0;
            var cyrillicUsed = CyrillicCharsetDetector.IsMatch(s) ? 1 : 0;
            return latinUsed + cyrillicUsed;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OnlyDigitsLocalizedAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public OnlyDigitsLocalizedAttribute()
            : base(@"^[0-9]+$")
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.InvariantCulture, ResPlatform.OnlyDigitsAttributeValidationMessage, name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "regex"
            };
            rule.ValidationParameters.Add(new KeyValuePair<string, object>("pattern", @"^[0-9]+$"));
            return new[] { rule };
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class UrlLocalizedAttribute : ValidationAttribute, IClientValidatable
    {
        private const string RegExPattern = @"^https?:\/\/([а-яёa-z0-9-_]+\.)+[а-яёa-z0-9]{2,4}.*$";
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.InvariantCulture, BLResources.InputValidationInvalidUrl, name);
        }
        public override bool IsValid(object value)
        {
            if (value == null || value.ToString().Length == 0)
            {
                return true;
            }

            return Regex.IsMatch(value.ToString(), RegExPattern, RegexOptions.IgnoreCase);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "url"
            };
            return new[] { rule };
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class CustomClientValidationAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly object _typeId = new object();
        public override object TypeId
        {
            get { return _typeId; }
        }

        public string FunctionName { get; private set; }

        public CustomClientValidationAttribute(string functionName)
        {
            FunctionName = functionName;
        }

        public override bool IsValid(object value)
        {
            return true; //Assuming that validation logic has been implemented in Erm.BusinessLogic
        }

        internal static string GetLocalizedDisplayName(Type resourceManagerProvider, string resourceKey)
        {
            if (resourceManagerProvider == null || string.IsNullOrWhiteSpace(resourceKey))
                return null;

            var resourceManager = resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(x => x.PropertyType == typeof(ResourceManager))
                                        .Select(x => x.GetValue(null, null))
                                        .Cast<ResourceManager>().SingleOrDefault();

            return (resourceManager == null) ? resourceKey : resourceManager.GetString(resourceKey);
        }

        public override string FormatErrorMessage(string name)
        {
            var message = ErrorMessage ?? GetLocalizedDisplayName(ErrorMessageResourceType, ErrorMessageResourceName);
            return string.Format(CultureInfo.CurrentCulture, message, name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "customvalidation"
            };

            rule.ValidationParameters.Add("validationfunction", FunctionName);
            return new[] { rule };
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SanitizedStringAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return BLResources.FieldValueContainsControlCharacters;
        }

        public override bool IsValid(object value)
        {
            var stringValue = value as string;
            return stringValue == null || !stringValue.ContainForbiddenCharacters();
        }
    }
}