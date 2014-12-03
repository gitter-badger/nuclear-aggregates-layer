using System;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public sealed class ViewModelViewPathMapping<TViewModel> : IViewModelViewMapping
        where TViewModel : class, IViewModelAbstract 
    {
        public ViewModelViewPathMapping(string view)
        {
            ViewName = view;
        }

        public Type ViewModelType
        {
            get
            {
                return typeof(TViewModel);
            }
        }

        public string ViewName { get; private set; }
    }
}