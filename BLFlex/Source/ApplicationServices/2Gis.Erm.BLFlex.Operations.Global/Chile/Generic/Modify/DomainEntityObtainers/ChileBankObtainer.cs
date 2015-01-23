using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;


namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public sealed class ChileBankObtainer : ISimplifiedModelEntityObtainer<Bank>, IChileAdapted
    {
        private readonly IFinder _finder;

        public ChileBankObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Bank ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BankDomainEntityDto)domainEntityDto;
            var bank = dto.IsNew()
                           ? new Bank { IsActive = true, IsDeleted = false }
                           : _finder.FindOne(Specs.Find.ById<Bank>(dto.Id));
            
            bank.Name = dto.Name;
            
            return bank;
        }
    }
}
