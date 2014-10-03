using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    // TODO {d.ivanov, 02.09.2014}: Реализовать на подсистеме метаданных, см IMetadataProvider, IMetadataElement, IMetadataProcessor
    public class ViewModelCustomizationProvider : IViewModelCustomizationProvider
    {
        private static readonly Dictionary<EntityName, IEnumerable<Type>> CustomizationsByEntities =
            new Dictionary<EntityName, IEnumerable<Type>>
                {
                    { EntityName.Client, new[] {typeof(WarnLinkToAdvAgencyExistsVmCustomization) } }
                };

        private static readonly Dictionary<Tuple<EntityName, BusinessModel>, IEnumerable<Type>> BusinessModelSpecificCustomizations =
            new Dictionary<Tuple<EntityName, BusinessModel>, IEnumerable<Type>>
                {
                    {
                        Tuple.Create(EntityName.Client, BusinessModel.Russia),
                        new[] { typeof(EditIsAdvertisingAgencyViewModelCustomization) }
                    }
                };

        private readonly BusinessModel _currentBusinessModel;

        public ViewModelCustomizationProvider(IBusinessModelSettings businessModelSettings)
        {
            _currentBusinessModel = businessModelSettings.BusinessModel;
        }

        public IEnumerable<Type> GetCustomizations(EntityName entityName)
        {
            IEnumerable<Type> genericCustomizations;
            if (!CustomizationsByEntities.TryGetValue(entityName, out genericCustomizations))
            {
                genericCustomizations = Enumerable.Empty<Type>();
            }

            IEnumerable<Type> businessModelSpecificCustomizations;
            if (!BusinessModelSpecificCustomizations.TryGetValue(Tuple.Create(entityName, _currentBusinessModel), out businessModelSpecificCustomizations))
            {
                businessModelSpecificCustomizations = Enumerable.Empty<Type>();
            }

            return genericCustomizations.Concat(businessModelSpecificCustomizations);
        }
    }
}