using System;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public sealed class GridViewModelIdentity : IGridViewModelIdentity
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly EntityName _entityName;

        public GridViewModelIdentity(EntityName entityName)
        {
            _entityName = entityName;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public EntityName EntityName
        {
            get { return _entityName; }
        }

        public override string ToString()
        {
            return string.Format("List of {0}s", EntityName);
        }
    }
}