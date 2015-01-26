using System;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public sealed class EntitySelectedMessageHandler : UseCaseSyncMessageHandlerBase<EntitySelectedMessage>
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly IViewModelMapperFactory _viewModelMapperFactory;
        private readonly ICardDocumentViewModelFactory _cardDocumentViewModelFactory;
        private readonly IDocumentManager _documentManager;

        public EntitySelectedMessageHandler(
            IOperationServicesManager operationServicesManager,
            IViewModelMapperFactory viewModelMapperFactory,
            ICardDocumentViewModelFactory cardDocumentViewModelFactory, 
            IDocumentManager documentManager)
        {
            _operationServicesManager = operationServicesManager;
            _viewModelMapperFactory = viewModelMapperFactory;
            _cardDocumentViewModelFactory = cardDocumentViewModelFactory;
            _documentManager = documentManager;
        }

        protected override IMessageProcessingResult ConcreteHandle(EntitySelectedMessage message, IUseCase useCase)
        {
            var mapper = _viewModelMapperFactory.GetMapper(message.EntityName);
            
            var service = _operationServicesManager.GetDomainEntityDtoService(message.EntityName);
            var dto = service.GetDomainEntityDto(message.EntityId, false, null, EntityType.Instance.None(), string.Empty);

            var documentViewModel = 
                _cardDocumentViewModelFactory.Create<ICompositeDocumentViewModel>(
                                                useCase,
                                                message.EntityName,
                                                message.EntityId);
            IViewModel cardViewModel;
            if (!documentViewModel.TryGetElement<IViewModel>(vm => vm is ICardViewModel, out cardViewModel))
            {
                throw new InvalidOperationException("Can't use document without card inside for entity show. Entity name: " + message.EntityName + ". Entity id: " + message.EntityId);
            }

            mapper.FromDto(dto, useCase, cardViewModel);
            if (!useCase.State.TryMoveNext(documentViewModel))
            {
                throw new InvalidOperationException(ResPlatformUI.ErrorDetectedWhenChangingUseCaseState);
            }

            _documentManager.Add(documentViewModel);

            return EmptyResult;
        }
    }
}