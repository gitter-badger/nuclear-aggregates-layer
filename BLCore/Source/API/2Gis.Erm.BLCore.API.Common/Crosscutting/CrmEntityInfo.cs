using System;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting
{
    public class CrmEntityInfo
    {
        public IEntityType EntityName { get; set; }
        public Guid Id { get; set; }
    }
}