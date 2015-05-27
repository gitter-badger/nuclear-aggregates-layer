using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class SetTitleCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private const string TitleTemplate = "{0} : {1}";

        private readonly IMetadataProvider _metadataProvider;
        private readonly IUserContext _userContext;

        public SetTitleCustomization(IMetadataProvider metadataProvider, IUserContext userContext)
        {
            _metadataProvider = metadataProvider;
            _userContext = userContext;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CardMetadata metadata;
            var metadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(viewModel.ViewConfig.EntityName.Description).Build().AsIdentity();
            if (!_metadataProvider.TryGetMetadata(metadataId.Id, out metadata))
            {
                throw new MetadataNotFoundException(metadataId);
            }

            if (string.IsNullOrWhiteSpace(viewModel.ViewConfig.CardSettings.Title))
            {
                viewModel.ViewConfig.CardSettings.Title = metadata.EntityLocalizationDescriptor != null
                                                                ? metadata.EntityLocalizationDescriptor.GetValue(_userContext.Profile.UserLocaleInfo.UserCultureInfo)
                                                                : metadata.Entity.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);

                if (viewModel.IsNew)
                {
                    viewModel.ViewConfig.CardSettings.Title = string.Format(TitleTemplate, viewModel.ViewConfig.CardSettings.Title, BLResources.Create);
                }
                else
                {
                    var mainAttributeFeature = metadata.Features<MainAttributeFeature>().SingleOrDefault();
                    if (mainAttributeFeature == null)
                    {
                        return;
                    }

                    object mainAttributeValue;
                    if (mainAttributeFeature.PropertyDescriptor.TryGetValue(viewModel, out mainAttributeValue))
                    {
                        viewModel.ViewConfig.CardSettings.Title = string.Format(TitleTemplate, viewModel.ViewConfig.CardSettings.Title, mainAttributeValue);
                    }
                }
            }
        }
    }
}