using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RutValidationAttribute : RegularExpressionAttribute, IClientValidatable
    {
        private const string RutFormatPattern = @"^(\d\d?)\.(\d\d\d)\.(\d\d\d)\-[0-9K]$";

        public RutValidationAttribute()
            : base(RutFormatPattern)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.InvariantCulture, Resources.Server.Properties.Resources.RutFormatError, name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "regex"
            };

            rule.ValidationParameters.Add("pattern", RutFormatPattern);

            return new[] { rule };
        }
    }
}
