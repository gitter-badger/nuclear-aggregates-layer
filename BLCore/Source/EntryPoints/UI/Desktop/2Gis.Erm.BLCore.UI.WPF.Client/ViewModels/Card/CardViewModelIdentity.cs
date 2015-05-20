using System;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public sealed class CardViewModelIdentity : ICardViewModelIdentity
    {
        private readonly Guid _id = Guid.NewGuid();

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public long EntityId { get; set; }
        public IEntityType EntityName { get; set; }
        public IOperationIdentity OperationIdentity { get; set; }

        public override string ToString()
        {
            return EntityId == 0
                    ? string.Format("New {0}", EntityName)
                    : string.Format("{0} {1}", EntityName, EntityId);
        }
    }
}