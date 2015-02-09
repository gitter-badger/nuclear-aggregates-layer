using System;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting
{
    public class CrmEntityInfo
    {
        public EntityName TypeName { get; set; }
        public Guid Id { get; set; }
    }
}