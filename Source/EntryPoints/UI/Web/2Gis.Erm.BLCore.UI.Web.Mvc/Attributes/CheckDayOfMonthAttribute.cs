using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class CheckDayOfMonthAttribute : ValidationAttribute, IClientValidatable
    {
        public CheckDayOfMonthAttribute(CheckDayOfMonthType checkDayOfMonthType)
        {
            CheckDayOfMonthType = checkDayOfMonthType;
        }

        public CheckDayOfMonthType CheckDayOfMonthType { get; private set; }

        public override bool IsValid(object value)
        {
            var date = ((DateTime)value).Date;

            return (CheckDayOfMonthType == CheckDayOfMonthType.FirstDay)
                ? date == new DateTime(date.Year, date.Month, 1)
                : date == new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        internal static string GetLocalizedDisplayName(Type resourceManagerProvider, string resourceKey)
        {
            if (resourceManagerProvider == null || string.IsNullOrWhiteSpace(resourceKey))
            {
                return null;
            }

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
                ValidationType = "checkDate",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };

            rule.ValidationParameters.Add("dateField", metadata.PropertyName);
            rule.ValidationParameters.Add("isFirstDay", (CheckDayOfMonthType == CheckDayOfMonthType.FirstDay) ? "1" : "0");
            
            return new[] {rule};
        }
    }
}