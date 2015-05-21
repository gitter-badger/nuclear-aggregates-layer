using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportReferenceItemAggregateService : IImportReferenceItemAggregateService
    {
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<ReferenceItem> _refrenceItemGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportReferenceItemAggregateService(IRepository<ReferenceItem> refrenceItemGenericRepository,
                                                   IFinder finder,
                                                   IIdentityProvider identityProvider,
                                                   IOperationScopeFactory scopeFactory)
        {
            _refrenceItemGenericRepository = refrenceItemGenericRepository;
            _finder = finder;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void ImportReferenceItems(IEnumerable<ReferenceItemServiceBusDto> referenceItemServiceBusDtos)
        {
            foreach (var referenceItemDto in referenceItemServiceBusDtos)
            {
                ProcessReferenceItemDto(referenceItemDto);
            }

            _refrenceItemGenericRepository.Save();
        }

        private void ProcessReferenceItemDto(ReferenceItemServiceBusDto referenceItemDto)
        {
            var reference = _finder.Find(new FindSpecification<Reference>(x => x.CodeName == referenceItemDto.ReferenceCode)).SingleOrDefault();
            if (reference == null)
            {
                throw new ArgumentException(string.Format(BLResources.ReferenceWithIdNotFound, referenceItemDto.ReferenceCode));
            }

            var referenceItem = _finder.Find(new FindSpecification<ReferenceItem>(x => x.Code == referenceItemDto.Code && x.ReferenceId == reference.Id)).SingleOrDefault() ??
                                new ReferenceItem { Code = referenceItemDto.Code, ReferenceId = reference.Id };

            referenceItem.Name = referenceItemDto.Name;
            referenceItem.IsDeleted = referenceItemDto.IsDeleted;

            if (referenceItem.IsNew())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ReferenceItem>())
                {
                    _identityProvider.SetFor(referenceItem);
                    _refrenceItemGenericRepository.Add(referenceItem);
                    scope.Added<ReferenceItem>(referenceItem.Id)
                         .Complete();
                }
            }
            else
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ReferenceItem>())
                {
                    _refrenceItemGenericRepository.Update(referenceItem);
                    scope.Updated<ReferenceItem>(referenceItem.Id)
                         .Complete();
                }
            }
        }
    }
}