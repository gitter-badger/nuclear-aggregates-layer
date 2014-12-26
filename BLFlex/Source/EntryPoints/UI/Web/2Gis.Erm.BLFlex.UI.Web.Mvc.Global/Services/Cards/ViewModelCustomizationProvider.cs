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
                typeof(EntityIsInactiveCustomization),
                typeof(SetReadonlyCustomization),
                typeof(SetTitleCustomization),
                typeof(RelatedItemsCustomization),
                typeof(ToolbarElementsAvailabilityCustomization)
            };

        public ViewModelCustomizationProvider(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IEnumerable<Type> GetCustomizations(EntityName entityName)
        {
            ViewModelCustomizationsMetadata customizationsMetadata;
            IEnumerable<Type> result;

            if (!_metadataProvider.TryGetMetadata(IdBuilder.For<ViewModelCustomizationsIdentity>(entityName.ToString()), out customizationsMetadata))
            {
                result = Enumerable.Empty<Type>();
            }
            else
            {
                result = customizationsMetadata.Features
                                               .Cast<IViewModelCustomizationFeature>()
                                               .OrderBy(x => x.Order)
                                               .Select(x => x.CustomizationType)
                                               .ToArray();
            }

            return result.Union(_sharedCustomizations)
                         .ToArray();
        }
    }
}