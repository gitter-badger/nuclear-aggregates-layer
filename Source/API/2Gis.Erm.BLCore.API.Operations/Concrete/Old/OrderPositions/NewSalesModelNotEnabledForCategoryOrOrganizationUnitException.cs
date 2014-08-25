using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class NewSalesModelNotEnabledForCategoryOrOrganizationUnitException : BusinessLogicException
    {
        public NewSalesModelNotEnabledForCategoryOrOrganizationUnitException(IEnumerable<string> categoryNames, string organizationUnitName)
            : base(string.Format(BLResources.CategoryIsNotSupportedForSpecifiedRateTypeAndOrgUnit, organizationUnitName, string.Join(", ", categoryNames)))
        {
        }

// ReSharper disable UnusedMember.Local
        private NewSalesModelNotEnabledForCategoryOrOrganizationUnitException(SerializationInfo serializationInfo, StreamingContext streamingContext)
// ReSharper restore UnusedMember.Local
            : base(serializationInfo, streamingContext)
        {
        }
    }
}