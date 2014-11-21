using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public sealed class OrdinaryViewModelIdentity : IViewModelIdentity
    {
        private readonly Guid _id = Guid.NewGuid();

        public Guid Id 
        {
            get
            {
                return _id;
            }
        }
    }
}