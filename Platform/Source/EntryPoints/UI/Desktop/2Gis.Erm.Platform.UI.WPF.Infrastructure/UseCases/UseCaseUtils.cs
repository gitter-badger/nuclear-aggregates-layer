using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public static class UseCaseUtils
    {
        public static bool TryGetViewModelById<TElement>(this IUseCase useCase, Guid targetId, out TElement element)
            where TElement : class, IViewModel
        {
            IUseCaseNode useCaseNode;
            return useCase.TryGetViewModelById(targetId, out element, out useCaseNode);
        }

        public static bool TryGetViewModelById<TElement>(this IUseCase useCase, Guid targetId, out TElement element, out IUseCaseNode useCaseNode)
            where TElement : class, IViewModel
        {
            element = null;
            useCaseNode = null;

            if (useCase.State.IsEmpty)
            {
                return false;
            }

            foreach (var node in useCase.State.NodesSnapshot)
            {
                var viewModel = node.Context as IViewModel;
                if (viewModel == null)
                {
                    continue;
                }

                if (viewModel.TryGetElementById(targetId, out element))
                {
                    useCaseNode = node;
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsViewModel(this IUseCase useCase, Guid targetId)
        {
            IViewModel viewModel;
            return useCase.TryGetViewModelById(targetId, out viewModel);
        }

        public static bool TryGetElement<TElement>(this IUseCase useCase, Predicate<object> predicate, out TElement element)
            where TElement : class
        {
            IUseCaseNode useCaseNode;
            return useCase.TryGetElement(predicate, out element, out useCaseNode);
        }

        public static bool TryGetElement<TElement>(this IUseCase useCase, Predicate<object> predicate, out TElement element, out IUseCaseNode useCaseNode)
            where TElement : class
        {
            element = null;
            useCaseNode = null;

            if (useCase.State.IsEmpty)
            {
                return false;
            }

            foreach (var node in useCase.State.NodesSnapshot)
            {
                if (predicate(node.Context))
                {
                    element = node.Context as TElement;
                    useCaseNode = node;
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsElement(this IUseCase useCase, Predicate<object> predicate)
        {
            object context;
            return useCase.TryGetElement(predicate, out context);
        }
    }
}
