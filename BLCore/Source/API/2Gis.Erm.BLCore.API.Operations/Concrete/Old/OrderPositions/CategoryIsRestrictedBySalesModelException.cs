using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class CategoryIsRestrictedBySalesModelException : BusinessLogicException
    {
        public CategoryIsRestrictedBySalesModelException(IEnumerable<string> categoryNames, string organizationUnitName, SalesModel salesModel)
            : base(
                string.Format(BLResources.CategoryIsNotSupportedForSalesModel,
                              string.Join(", ", categoryNames),
                              salesModel.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                              organizationUnitName))
        {
        }

// ReSharper disable UnusedMember.Local
        private CategoryIsRestrictedBySalesModelException(SerializationInfo serializationInfo, StreamingContext streamingContext)
// ReSharper restore UnusedMember.Local
            : base(serializationInfo, streamingContext)
        {
        }
    }
}