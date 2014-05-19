﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportReferenceAggregateService : IImportReferenceAggregateService
    {
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<Reference> _refrenceGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportReferenceAggregateService(IRepository<Reference> refrenceGenericRepository,
                                               IFinder finder,
                                               IIdentityProvider identityProvider,
                                               IOperationScopeFactory scopeFactory)
        {
            _refrenceGenericRepository = refrenceGenericRepository;
            _finder = finder;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void ImportReferences(IEnumerable<ReferenceServiceBusDto> referenceServiceBusDtos)
        {
            foreach (var referenceDto in referenceServiceBusDtos)
            {
                ProcessReferenceDto(referenceDto);
            }

            _refrenceGenericRepository.Save();
        }

        private void ProcessReferenceDto(ReferenceServiceBusDto referenceDto)
        {
            var referenceExists = _finder.Find<Reference>(x => x.CodeName == referenceDto.Code).Any();
            if (referenceExists)
            {
                return;
            }

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Reference>())
            {
                var reference = new Reference { CodeName = referenceDto.Code };
                _identityProvider.SetFor(reference);
                _refrenceGenericRepository.Add(reference);
                scope.Added<Reference>(reference.Id)
                     .Complete();
            }
        }
    }
}