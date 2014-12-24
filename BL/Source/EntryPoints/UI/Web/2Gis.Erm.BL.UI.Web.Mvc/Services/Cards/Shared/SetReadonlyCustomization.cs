﻿using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class SetReadonlyCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IMetadataProvider _metadataProvider;

        public SetReadonlyCustomization(IMetadataProvider metadataProvider)
        {
            
            _metadataProvider = metadataProvider;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CardMetadata metadata;
            var metadataId = IdBuilder.For<MetadataCardsIdentity>(viewModel.ViewConfig.EntityName.ToString()).AsIdentity();
            if (!_metadataProvider.TryGetMetadata(metadataId.Id, out metadata))
            {
                throw new MetadataNotFoundException(metadataId);
            }

            viewModel.ViewConfig.ReadOnly |= metadata.Uses<ReadOnlyFeature>();
        }
    }
}