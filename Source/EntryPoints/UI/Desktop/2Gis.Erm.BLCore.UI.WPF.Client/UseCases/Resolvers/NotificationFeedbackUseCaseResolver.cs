using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging.Concrete;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Resolvers
{
    public sealed class NotificationFeedbackUseCaseResolver : UseCaseResolverBase<NotificationFeedbackMessage>
    {
        protected override bool IsAppropriate(IUseCase checkingUseCase, NotificationFeedbackMessage message)
        {
            var currentElement = checkingUseCase.State.Current;

            if (checkingUseCase.State.IsEmpty || checkingUseCase.State.Root.IsDegenerated || currentElement == null)
            {
                return false;
            }

            return ContainsRequiredNotificationsSource(checkingUseCase, message);
        }

        private static bool ContainsRequiredNotificationsSource(IUseCase checkingUseCase, NotificationFeedbackMessage message)
        {
            // TODO {all, 21.07.2013}: поддержку поиска по всему usecase, а не только по текущему элементу
            IViewModelWithFeedback viewModelWithFeedback;
            var viewModel = checkingUseCase.State.Current.Context as IViewModel;
            if (viewModel == null)
            {
                return false;
            }

            return viewModel.TryGetElement(vm => vm.Identity.Id == message.TargetId && vm is IViewModelWithFeedback, out viewModelWithFeedback);
        }
    }
}