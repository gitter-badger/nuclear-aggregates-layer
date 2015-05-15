using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{   
    public sealed class BulkCreateAdvertisementElementsForAdvertisementAggregateService : IBulkCreateAdvertisementElementsForAdvertisementAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<AdvertisementElement> _secureAdvertisementElementRepository;
        private readonly IRepository<AdvertisementElementStatus> _advertisementElementStatusRepository;
        private readonly IIdentityProvider _identityProvider;

        public BulkCreateAdvertisementElementsForAdvertisementAggregateService(IOperationScopeFactory operationScopeFactory,
                                                                           ISecureRepository<AdvertisementElement> secureAdvertisementElementRepository,
                                                                           IRepository<AdvertisementElementStatus> advertisementElementStatusRepository,
                                                                           IIdentityProvider identityProvider)
        {
            _operationScopeFactory = operationScopeFactory;
            _secureAdvertisementElementRepository = secureAdvertisementElementRepository;
            _advertisementElementStatusRepository = advertisementElementStatusRepository;
            _identityProvider = identityProvider;
        }

        public void Create(IEnumerable<AdvertisementElementCreationDto> advertisementElements, long advertisementId, long ownerCode)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<BulkCreateIdentity, AdvertisementElement>())
            {
                foreach (var elementToCreate in advertisementElements)
                {
                    var advertisementElement = new AdvertisementElement
                        {
                            AdvertisementId = advertisementId,
                            AdvertisementElementTemplateId = elementToCreate.AdvertisementElementTemplateId,
                            OwnerCode = ownerCode,
                            AdsTemplatesAdsElementTemplatesId = elementToCreate.AdsTemplatesAdsElementTemplateId,
                        };

                    // TODO: косяк что значение по умолчанию это 6, из-за этого его теперь надо excplicitly проставлять, должен быть 0
                    if (elementToCreate.IsFasComment)
                    {
                        advertisementElement.FasCommentType = FasComment.NewFasComment;
                    }

                    // заглушку или не требующий выверки или не обязательный для заполнения ЭРМ создаем в статусе  - валиден
                    // таким образом в статусе черновик мы создадим ЭРМ, если он обязателен для заполнения и требует выверки
                    var status = new AdvertisementElementStatus
                        {
                            Status =
                                (int)(elementToCreate.IsRequired && elementToCreate.NeedsValidation && advertisementId != elementToCreate.DummyAdvertisementId
                                          ? AdvertisementElementStatusValue.Draft
                                          : AdvertisementElementStatusValue.Valid)
                        };

                    _identityProvider.SetFor(advertisementElement);
                    status.Id = advertisementElement.Id;

                    _secureAdvertisementElementRepository.Add(advertisementElement);
                    _advertisementElementStatusRepository.Add(status);

                    scope.Added(advertisementElement)
                         .Added(status);
                }

                _secureAdvertisementElementRepository.Save();
                _advertisementElementStatusRepository.Save();

                scope.Complete();
            }
        }
    }
}