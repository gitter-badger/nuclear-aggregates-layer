using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Metadata.Cards;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class RelatedItemsCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly UIElementAvailabilityHelper _elementAvailabilityHelper;
        private readonly IUserContext _userContext;

        public RelatedItemsCustomization(IMetadataProvider metadataProvider,
                                         UIElementAvailabilityHelper elementAvailabilityHelper,
                                         IUserContext userContext)
        {
            _metadataProvider = metadataProvider;
            _elementAvailabilityHelper = elementAvailabilityHelper;
            _userContext = userContext;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CardMetadata metadata;
            var metadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(viewModel.ViewConfig.EntityName.ToString()).Build().AsIdentity();
            if (!_metadataProvider.TryGetMetadata(metadataId.Id, out metadata))
            {
                throw new MetadataNotFoundException(metadataId);
            }

            if (metadata.HasRelatedItems)
            {
                foreach (var relatedItem in metadata.RelatedItems)
                {
                    ProcessRelatedItemMetadata(relatedItem, viewModel);
                }
            }
        }

        private void ProcessRelatedItemMetadata(UIElementMetadata relatedItemElementMetadata, IEntityViewModelBase model)
        {
            if (relatedItemElementMetadata.NameDescriptor == null)
            {
                return;

                // Или это не нормально и бросим исключение? Пока не ясно.
            }

            var elementName = relatedItemElementMetadata.NameDescriptor.ToString();

            // А является ли имя идентификатором?
            // Если не по имени, то как сопоставить элемент и его метаданные?
            var elementToEvaluate =
                model.ViewConfig
                     .CardSettings
                     .CardRelatedItems
                     .SelectMany(x => x.Items)
                     .SingleOrDefault(x => x.Name == elementName);

            if (elementToEvaluate == null)
            {
                return;
            }

            EvaluateRelatedItemExtendedInfo(relatedItemElementMetadata, elementToEvaluate, model);
            EvaluateRelatedItemAvailability(relatedItemElementMetadata, elementToEvaluate, model);
        }

        private void EvaluateRelatedItemAvailability(UIElementMetadata relatedItemElementMetadata, CardRelatedItemStructure relatedItemElement, IEntityViewModelBase model)
        {
            relatedItemElement.Disabled |= _elementAvailabilityHelper.IsUIElementDisabled(relatedItemElementMetadata, model);

            if (_elementAvailabilityHelper.IsUIElementInvisible(relatedItemElementMetadata, (IAspect)model))
            {
                model.ViewConfig.CardSettings.CardRelatedItems.SingleOrDefault().Items
                    = model.ViewConfig.CardSettings.CardRelatedItems.SingleOrDefault().Items.Except(new[] { relatedItemElement }).ToArray();
            }
        }

        private void EvaluateRelatedItemExtendedInfo(UIElementMetadata relatedItemElementMetadata, CardRelatedItemStructure relatedItemElement, IEntityViewModelBase model)
        {
            foreach (var extendedInfoFeature in relatedItemElementMetadata.Features.OfType<ExtendedInfoFeature>())
            {
                var templatedExtendedInfo = extendedInfoFeature.ExtendedInfo as TemplateDescriptor;

                if (templatedExtendedInfo != null)
                {
                    string extendedInfo;
                    if (!templatedExtendedInfo.TryFormat(_userContext.Profile.UserLocaleInfo.UserCultureInfo, out extendedInfo, model))
                    {
                        throw new InvalidOperationException(string.Format("Unable to determine extendedInfo for {0} related item", relatedItemElement.Name));
                    }

                    relatedItemElement.AppendExtendedInfo(extendedInfo);
                }
                else
                {
                    relatedItemElement.AppendExtendedInfo(extendedInfoFeature.ExtendedInfo.ToString());
                }
            }
        }
    }
}