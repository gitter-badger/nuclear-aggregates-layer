using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    // TODO {d.ivanov, 02.09.2014}: Реализовать на подсистеме метаданных, см IMetadataProvider, IMetadataElement, IMetadataProcessor
    public class ViewModelCustomizationProvider : IViewModelCustomizationProvider
    {
        private static readonly Dictionary<IEntityType, IEnumerable<Type>> CustomizationsByEntities =
            new Dictionary<IEntityType, IEnumerable<Type>>
                {
                    { EntityType.Instance.Client(), new[] {typeof(WarnLinkToAdvAgencyExistsVmCustomization) } }
                };

        private static readonly Dictionary<Tuple<IEntityType, BusinessModel>, IEnumerable<Type>> BusinessModelSpecificCustomizations =
            new Dictionary<Tuple<IEntityType, BusinessModel>, IEnumerable<Type>>
                {
                    {
                        Tuple.Create((IEntityType)EntityType.Instance.Client(), BusinessModel.Russia),
                        new[] { typeof(EditIsAdvertisingAgencyViewModelCustomization) }
                    }
                };

        private readonly BusinessModel _currentBusinessModel;

        public ViewModelCustomizationProvider(IBusinessModelSettings businessModelSettings)
        {
            _currentBusinessModel = businessModelSettings.BusinessModel;
        }

        public IEnumerable<Type> GetCustomizations(IEntityType entityName)
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