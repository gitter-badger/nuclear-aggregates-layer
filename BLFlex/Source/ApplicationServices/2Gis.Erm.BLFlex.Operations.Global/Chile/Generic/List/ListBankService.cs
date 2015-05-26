using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel.Banks;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.List
{
    public sealed class ListBankService : ListEntityDtoServiceBase<Bank, ChileListBankDto>, IChileAdapted
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListBankService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            // FIXME {all, 10.04.2014}: при рефаторинге EAV попытаться свести просто к For<Bank> и т.п. - то что bank это EAV нужно запрятать куда-то (finder)
            return _finder.Find(BankSpecs.Find.OnlyBanks, BankSpecs.Select.Banks)
                   .Select(x => new ChileListBankDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    })
                    .QuerySettings(_filterHelper, querySettings);
        }
    }
}