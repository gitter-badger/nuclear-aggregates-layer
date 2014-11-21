﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class CurrencyObtainer : ISimplifiedModelEntityObtainer<Currency>
    {
        private readonly IFinder _finder;

        public CurrencyObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Currency ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (CurrencyDomainEntityDto)domainEntityDto;

            var currency = _finder.Find(Specs.Find.ById<Currency>(dto.Id)).SingleOrDefault() ??
                           new Currency { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && currency.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            currency.ISOCode = dto.ISOCode;
            currency.Name = dto.Name;
            currency.Symbol = dto.Symbol;
            currency.IsBase = dto.IsBase;
            currency.Timestamp = dto.Timestamp;

            return currency;
        }
    }
}