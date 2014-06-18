using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    // FIXME {y.baranihin, 17.06.2014}: Предлагаю для Эмиратов использовать ту же самую viewmodel, а нюанс с НДС=0 решить на уровне EmiratesBargainTypeObtainer, 
    //                                  где выставлять entity.VatRate = decimal.Zero явно, а не использовать значение из DTO
    public sealed class EmiratesBargainTypeViewModel : EditableIdEntityViewModelBase<BargainType>, IEmiratesAdapted
    {
        [RequiredLocalized]
        public string Name { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BargainTypeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Timestamp = modelDto.Timestamp;
            IdentityServiceUrl = modelDto.IdentityServiceUrl;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new BargainTypeDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Timestamp = Timestamp,
                    VatRate = decimal.Zero
                };
        }
    }
}