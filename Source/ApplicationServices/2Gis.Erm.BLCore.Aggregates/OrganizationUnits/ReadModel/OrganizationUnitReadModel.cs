﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.OrganizationUnits.ReadModel
{
    public class OrganizationUnitReadModel : IOrganizationUnitReadModel
    {
        private readonly IFinder _finder;

        public OrganizationUnitReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public OrganizationUnit GetOrganizationUnit(long organizationUnitId)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId)).Single();
        }

        public string GetName(long organizationUnitId)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId)).Select(x => x.Name).Single();
        }

        public long GetCurrencyId(long organizationUnitId)
        {
            var currencyId = _finder.Find<OrganizationUnit>(x => x.Id == organizationUnitId).Select(x => (long?)x.Country.CurrencyId).SingleOrDefault();
            if (!currencyId.HasValue)
            {
                throw new InvalidOperationException(BLResources.CurrencyNotSpecifiedForPrice);
            }

            return currencyId.Value;
        }

        public IReadOnlyDictionary<int, long> GetOrganizationUnitIdsByDgppIds(IEnumerable<int> dgppIds)
        {
            // ReSharper disable once PossibleInvalidOperationException
            return _finder.Find(OrganizationUnitSpecs.Find.ByDgppIds(dgppIds)).ToDictionary(x => x.DgppId.Value, x => x.Id);
        } 
    }
}