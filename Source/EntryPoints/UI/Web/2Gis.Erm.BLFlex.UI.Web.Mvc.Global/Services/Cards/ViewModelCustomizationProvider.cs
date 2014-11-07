﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class ViewModelCustomizationProvider : IViewModelCustomizationProvider
    {
        private readonly IMetadataProvider _metadataProvider;

        public ViewModelCustomizationProvider(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IEnumerable<Type> GetCustomizations(EntityName entityName)
        {
            ViewModelCustomizationsMetada customizationsMetadata;

            if (!_metadataProvider.TryGetMetadata(IdBuilder.For<ViewModelCustomizationsIdentity>(entityName.ToString()), out customizationsMetadata))
            {
                return Enumerable.Empty<Type>();
            }

            return customizationsMetadata.Features.OfType<IViewModelCustomizationFeature>().Select(x => x.CustomizationType).ToArray();
        }
    }
}