using System;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public sealed class GridViewModelIdentity : IGridViewModelIdentity
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly IEntityType _entityName;

        public GridViewModelIdentity(IEntityType entityName)
        {
            _entityName = entityName;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public IEntityType EntityName
        {
            get { return _entityName; }
        }

        public override string ToString()
        {
            return string.Format("List of {0}s", EntityName);
        }
    }
}