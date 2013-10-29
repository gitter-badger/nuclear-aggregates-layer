using System;

namespace DoubleGis.Erm.Platform.UI.Metadata.Indicators
{
    public static class MVVMIndicators
    {
        public static readonly Type View = typeof(IView);
        public static readonly Type ViewModel = typeof(IViewModel);

        public static bool IsView<TView>()
        {
            var checkedType = typeof(TView);
            return checkedType.IsView();
        }

        public static bool IsView(this Type viewType)
        {
            return View.IsAssignableFrom(viewType);
        }

        public static bool IsViewModel<TViewModel>()
        {
            var checkedType = typeof(TViewModel);
            return checkedType.IsViewModel();
        }

        public static bool IsViewModel(this Type viewModelType)
        {
            return ViewModel.IsAssignableFrom(viewModelType);
        }
    }
}
