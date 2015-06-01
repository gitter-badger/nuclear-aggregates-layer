using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

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
            return _finder.Find(Specs.Find.ById<Bank>(bankId)).One();
        }

        public string GetBankName(long bankId)
        {
            return _finder.Find(Specs.Find.ById<DictionaryEntityInstance>(bankId))
                          .Map(q => q.Select(DictionaryEntitySpecs.DictionaryEntity.Select.Name()))
                          .One();
        }

        public bool IsBankUsed(long bankId)
        {
            return _finder.Find(DictionaryEntitySpecs.DictionaryEntity.Find.ByEntityName(EntityType.Instance.Bank()) &&
                                DictionaryEntitySpecs.DictionaryEntity.Find.ByPropertyValue(BankIdIdentity.Instance, bankId))
                          .Any();
        }
    }
}