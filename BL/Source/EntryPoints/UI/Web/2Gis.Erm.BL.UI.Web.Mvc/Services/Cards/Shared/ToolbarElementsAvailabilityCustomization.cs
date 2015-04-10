using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class ToolbarElementsAvailabilityCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly UIElementAvailabilityHelper _elementAvailabilityHelper;

        public ToolbarElementsAvailabilityCustomization(IMetadataProvider metadataProvider,
                                                        UIElementAvailabilityHelper elementAvailabilityHelper)
        {
            _metadataProvider = metadataProvider;
            _elementAvailabilityHelper = elementAvailabilityHelper;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CardMetadata metadata;
            var metadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(viewModel.ViewConfig.EntityName.ToString()).Build().AsIdentity();
            if (!_metadataProvider.TryGetMetadata(metadataId.Id, out metadata))
            {
                throw new MetadataNotFoundException(metadataId);
            }

            foreach (var actionElement in metadata.ActionsDescriptors)
            {
                ProcessToolbarElementMetadata(actionElement, null, viewModel);
            }
        }

        private void ProcessToolbarElementMetadata(UIElementMetadata toolbarElement, UIElementMetadata parentElement, IEntityViewModelBase model)
        {
            foreach (var childElement in toolbarElement.Elements.OfType<UIElementMetadata>())
            {
                ProcessToolbarElementMetadata(childElement, toolbarElement, model);
            }

            if (toolbarElement.NameDescriptor == null)
            {
                return;

                // Или это не нормально и бросим исключение? Пока не ясно.
            }

            var elementName = toolbarElement.NameDescriptor.ToString();
            if (elementName == "Splitter")
            {
                return;
            }

            var parentName = string.Empty;
            if (parentElement != null && parentElement.NameDescriptor != null)
            {
                parentName = parentElement.NameDescriptor.ToString();
            }

            // А является ли имя идентификатором? Например, splitter'ов у нас несколько
            // Если не по имени, то как сопоставить элемент и его метаданные?
            var elementToEvaluate =
                model
                    .ViewConfig
                    .CardSettings
                    .CardToolbar
                    .SingleOrDefault(x => x.Name == elementName && (parentName == string.Empty || x.ParentName == parentName));

            if (elementToEvaluate == null)
            {
                return;
            }

            EvaluateToolbarElementAvailability(toolbarElement, elementToEvaluate, model);
        }

        private void EvaluateToolbarElementAvailability<TViewModel>(UIElementMetadata toolbarElementMetadata, ToolbarElementStructure toolbarlement, TViewModel model)
            where TViewModel : IEntityViewModelBase
        {
            toolbarlement.Disabled |= _elementAvailabilityHelper.IsUIElementDisabled(toolbarElementMetadata, model);

            if (_elementAvailabilityHelper.IsUIElementInvisible(toolbarElementMetadata, (IAspect)model))
            {
                model.ViewConfig.CardSettings.CardToolbar
                    = model.ViewConfig.CardSettings.CardToolbar.Except(new[] { toolbarlement }).ToArray();
            }
        }
    }
}