﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteThemeOrganizationUnitService : IDeleteGenericEntityService<ThemeOrganizationUnit>
    {
        private readonly IDeleteAggregateRepository<ThemeOrganizationUnit> _deleteAggregateRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IFinder _finder;

        public DeleteThemeOrganizationUnitService(
            IDeleteAggregateRepository<ThemeOrganizationUnit> deleteAggregateRepository,
            IOperationScopeFactory operationScopeFactory,
            IFinder finder)
        {
            _deleteAggregateRepository = deleteAggregateRepository;
            _operationScopeFactory = operationScopeFactory;
            _finder = finder;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<ThemeOrganizationUnit>(entityId)).Single();
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DetachIdentity, Theme, OrganizationUnit>())
            {
                _deleteAggregateRepository.Delete(entityId);

                operationScope.Updated<Theme>(entity.ThemeId)
                              .Updated<OrganizationUnit>(entity.OrganizationUnitId);
                operationScope.Complete();

                return null;
            }
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            throw new NotSupportedException("GetConfirmation is not supported by DeleteGenericEntityService. It is need to implement specific delete service for entity, see IDeleteEntityService");
        }
    }
}
