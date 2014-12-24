using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class ViewModelCustomizationProvider : IViewModelCustomizationProvider
    {
        private readonly IMetadataProvider _metadataProvider;

        private readonly Type[] _sharedCustomizations =
            {
                typeof(SetReadonlyCustomization)
            };

        public ViewModelCustomizationProvider(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IEnumerable<Type> GetCustomizations(EntityName entityName)
        {
            ViewModelCustomizationsMetadata customizationsMetadata;

            if (!_metadataProvider.TryGetMetadata(IdBuilder.For<ViewModelCustomizationsIdentity>(entityName.ToString()), out customizationsMetadata))
            {
                return Enumerable.Empty<Type>();
            }

            return customizationsMetadata.Features
                                         .Cast<IViewModelCustomizationFeature>()
                                         .OrderBy(x => x.Order)
                                         .Select(x => x.CustomizationType)
                                         .Union(_sharedCustomizations)
                                         .ToArray();
        }
    }
}