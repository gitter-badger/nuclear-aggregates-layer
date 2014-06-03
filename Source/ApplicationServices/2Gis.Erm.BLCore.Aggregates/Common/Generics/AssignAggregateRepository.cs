﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.DAL.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public class AssignAggregateRepository<T> : IAssignAggregateRepository<T> where T : class, IEntity, IEntityKey, ICuratedEntity
    {
        private readonly IFinder _finder;
        private readonly ISecureRepository<T> _secureRepository;

        public AssignAggregateRepository(IFinder finder, ISecureRepository<T> secureRepository)
        {
            _finder = finder;
            _secureRepository = secureRepository;
        }

        public int Assign(long entityId, long ownerCode)
        {
            var entity = _finder.FindOne<T>(Specs.Find.ById<T>(entityId));
            entity.OwnerCode = ownerCode;
            _secureRepository.Update(entity);
            return _secureRepository.Save();
        }
    }
}