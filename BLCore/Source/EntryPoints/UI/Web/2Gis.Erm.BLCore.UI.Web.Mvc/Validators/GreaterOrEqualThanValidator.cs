using System;
using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Validators
{
    public sealed class GreaterOrEqualThanValidator : DataAnnotationsModelValidator<GreaterOrEqualThanAttribute>
    {
        private const string ValidationType = "greaterorequalthan";
        private const string AnotherPropertyParameterName = "anotherProperty";

        public GreaterOrEqualThanValidator(ModelMetadata metadata, ControllerContext context, GreaterOrEqualThanAttribute attribute) : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            var value = Metadata.Model;
            
            var anotherProperty = Metadata.ContainerType.GetProperty(Attribute.AnotherProperty);
            var anotherValue = anotherProperty.GetValue(container, null);

            return value != null && anotherValue != null && ((IComparable)value).CompareTo(anotherValue) >= 0
                       ? new ModelValidationResult[0]
                       : new[] { new ModelValidationResult { Message = ErrorMessage } };
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var rule = new ModelClientValidationRule
                           {
                               ErrorMessage = ErrorMessage,
                               ValidationType = ValidationType
                           };

            var viewContext = (ViewContext)ControllerContext;
            var clientProperyId = viewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(Attribute.AnotherProperty);
            rule.ValidationParameters.Add(AnotherPropertyParameterName, clientProperyId);

            return new[] { rule };
        }
    }
}