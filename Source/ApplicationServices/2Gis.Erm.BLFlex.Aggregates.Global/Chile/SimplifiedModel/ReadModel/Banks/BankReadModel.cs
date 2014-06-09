﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel.Banks
{
    public sealed class BankReadModel : IBankReadModel
    {
        private readonly IFinder _finder;

        public BankReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Bank GetBank(long bankId)
        {
            return _finder.FindOne(Specs.Find.ById<Bank>(bankId));
        }

        public string GetBankName(long bankId)
        {
            return _finder.Find(DictionaryEntitySpecs.DictionaryEntity.Select.Name(),
                                Specs.Find.ById<DictionaryEntityInstance>(bankId))
                          .SingleOrDefault();
        }

        public bool IsBankUsed(long bankId)
        {
            return _finder.Find(DictionaryEntitySpecs.DictionaryEntity.Find.ByEntityName(EntityName.Bank) &&
                                DictionaryEntitySpecs.DictionaryEntity.Find.ByPropertyValue(BankIdIdentity.Instance, bankId))
                          .Any();
        }
    }
}