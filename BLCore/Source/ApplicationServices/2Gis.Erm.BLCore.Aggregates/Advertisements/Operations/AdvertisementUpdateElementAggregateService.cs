using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{
    public sealed class AdvertisementUpdateElementAggregateService : IAdvertisementUpdateElementAggregateService
    {
        private readonly ISecureRepository<AdvertisementElement> _secureAdvertisementElementRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public AdvertisementUpdateElementAggregateService(
            ISecureRepository<AdvertisementElement> secureAdvertisementElementRepository,
            IOperationScopeFactory scopeFactory)
        {
            _secureAdvertisementElementRepository = secureAdvertisementElementRepository;
            _scopeFactory = scopeFactory;
        }

        public void Update(IEnumerable<AdvertisementElement> advertisementElements,
                           AdvertisementElementTemplate elementTemplate,
                           string plainText,
                           string formattedText)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, AdvertisementElement>())
            {
                foreach (var advertisementElement in advertisementElements)
                {
                    var templateRestrictionType = elementTemplate.RestrictionType;
                    if (templateRestrictionType == AdvertisementElementRestrictionType.FasComment
                        || templateRestrictionType == AdvertisementElementRestrictionType.Text)
                    {
                        // особая логика для текстовых рекламных материалов
                        advertisementElement.Text = elementTemplate.FormattedText ? formattedText : plainText;
                    }

                    _secureAdvertisementElementRepository.Update(advertisementElement);
                    scope.Updated<AdvertisementElement>(advertisementElement.Id);
                }
                
                _secureAdvertisementElementRepository.Save();
                scope.Complete();
            }
        }
    }
}