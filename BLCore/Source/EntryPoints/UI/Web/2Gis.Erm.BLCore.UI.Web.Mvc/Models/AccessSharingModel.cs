using System;

using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class AccessSharingModel : ViewModel
    {
        public IEntityType EntityTypeName { get; set; }

        public long EntityId { get; set; }
        public long EntityOwnerId { get; set; }
        public Guid EntityReplicationCode { get; set; }

        public string JsonData { get; set; }
    }
}