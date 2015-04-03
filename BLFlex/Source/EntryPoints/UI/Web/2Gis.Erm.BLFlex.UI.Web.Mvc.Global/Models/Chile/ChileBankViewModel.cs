using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile
{
    public sealed class ChileBankViewModel : EntityViewModelBase<Bank>, IChileAdapted
    {
        public string Name { get; set; }
        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var dto = (BankDomainEntityDto)domainEntityDto;

            this.Id = dto.Id;
            this.Name = dto.Name;
            this.Timestamp = dto.Timestamp;

            this.CreatedBy = dto.CreatedByRef.ToLookupField();
            this.ModifiedBy = dto.ModifiedByRef.ToLookupField();
            this.ModifiedOn = dto.ModifiedOn;
            this.CreatedOn = dto.CreatedOn;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new BankDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Timestamp = Timestamp,
                };
        }
    }
}
