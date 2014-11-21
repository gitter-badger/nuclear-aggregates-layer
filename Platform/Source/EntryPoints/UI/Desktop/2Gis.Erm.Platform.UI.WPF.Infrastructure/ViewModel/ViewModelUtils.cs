using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public static class ViewModelUtils
    {
        public static bool TryGetElementById<TElement>(this IViewModel checkingElement, Guid targetId, out TElement element) 
            where TElement : class, IViewModel
        {
            return checkingElement.TryGetElement(vm => vm.Identity.Id == targetId, out element);
        }

        public static bool TryGetElement<TElement>(this IViewModel checkingElement, Predicate<IViewModel> predicate, out TElement element)
            where TElement : class, IViewModel
        {
            element = default(TElement);

            if (checkingElement != null && predicate(checkingElement))
            {
                element = (TElement)checkingElement;
                return true;
            }

            var compositeViewModel = checkingElement as ICompositeViewModel;
            if (compositeViewModel != null && compositeViewModel.ComposedViewModels != null)
            {
                foreach (var composedViewModel in compositeViewModel.ComposedViewModels)
                {
                    if (composedViewModel.TryGetElement(predicate, out element))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool ContainsElement(this IViewModel checkingElement, Guid targetId)
        {
            IViewModel viewModel;
            return checkingElement.TryGetElementById(targetId, out viewModel);
        }

        public static bool ContainsElement(this IDocument checkingElement, Guid targetId)
        {
            var checkingViewModel = checkingElement as IViewModel;
            if (checkingViewModel == null)
            {
                return false;
            }

            IViewModel viewModel;
            return checkingViewModel.TryGetElementById(targetId, out viewModel);
        }

        public static T ExtractPropertyValue<T>(object propertyHostInstance, string propertyName)
        {
            if (propertyHostInstance == null)
            {
                return default(T);
            }

            var propertyInfo = propertyHostInstance.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return default(T);
            }

            return (T)propertyInfo.GetValue(propertyHostInstance, null);
        }
    }
}
