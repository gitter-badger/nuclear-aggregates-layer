using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Actions;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public sealed class UnityViewModelActionsResolver : UnityViewModelAspectResolverBase<IActionsContainer, ActionsFeature>
    {
        private readonly ITitleProviderFactory _titleProviderFactory;

        public UnityViewModelActionsResolver(ITitleProviderFactory titleProviderFactory)
        {
            _titleProviderFactory = titleProviderFactory;
        }

        protected override IActionsContainer Create(IUseCase useCase, IViewModelMetadata viewModelMetadata, IViewModelIdentity resolvingViewModelIdentity, ActionsFeature feature)
        {
            var factory = useCase.ResolveFactoryContext();
            var messageSink = factory.Resolve<IMessageSink>();

            var cardViewModelDescriptor = resolvingViewModelIdentity as CardViewModelIdentity;
            if (cardViewModelDescriptor == null)
            {
                throw new NotSupportedException("Card operations supported only");
            }

            return new ActionContainer(GetNavigationItems(cardViewModelDescriptor, feature.ActionsDescriptors, _titleProviderFactory, messageSink));
        }

        private static INavigationItem[] GetNavigationItems(CardViewModelIdentity cardViewModelIdentity,
                                                            OldUIElementMetadata[] actions,
                                                            ITitleProviderFactory titleProviderFactory,
                                                            IMessageSink messageSink)
        {
            if (actions == null || !actions.Any())
            {
                return null;
            }

            var navigationsItems = new List<INavigationItem>();
            foreach (var action in actions)
            {
                NavigationItem navigationItem;
                if (action.HasOperations)
                {
                    // TODO {all, 23.04.2014}: пока поддерживаем только связь один action - одна операция, упомянутая первой,
                    // если в метаданных прописаны дополнительные операции (например, Save(Modify)AndClose, т.е. сохранить и закрыть)
                    // то они пока игнорируются, варианты:
                    // - в самом handler сообщения выводить из метаданных наличие таких сопутствующих операций (главное нужно найти целевой элемент метаданных)
                    // - в момент конфигурирования DelegateCommand прописывать что-то вроде .ContinueWith(...)
                    var operation = action.OperationFeatures.Select(f => f.Identity).First();
                    var actionCommand = new DelegateCommand<INavigationItem>(
                        item => messageSink.Post(new ExecuteActionMessage(operation, cardViewModelIdentity.Id)
                            {
                                // TODO {all, 22.07.2013}: get confirmation settings from metadata, пока hardcode для assign
                                NeedConfirmation = operation.OperationIdentity.Equals(AssignIdentity.Instance)
                            }),
                        item =>
                            {
                                var result = messageSink.Send<bool>(new CanExecuteActionMessage(operation, cardViewModelIdentity.Id));
                                return result != null && result.Result;
                            });

                    navigationItem = new NavigationItem(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Unique().For("CardsStructures/Toolbar"), titleProviderFactory.Create(action.TitleDescriptor), actionCommand)
                        {
                            Items = GetNavigationItems(cardViewModelIdentity, action.Elements.OfType<OldUIElementMetadata>().ToArray(), titleProviderFactory, messageSink)
                        };
                }
                else
                {
                    navigationItem = new NavigationItem(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Unique().For("CardsStructures/Toolbar"), titleProviderFactory.Create(action.TitleDescriptor), null)
                    {
                        Items = GetNavigationItems(cardViewModelIdentity, action.Elements.OfType<OldUIElementMetadata>().ToArray(), titleProviderFactory, messageSink)
                    };
                }

                navigationsItems.Add(navigationItem);
            }

            return navigationsItems.ToArray();
        }
    }
}