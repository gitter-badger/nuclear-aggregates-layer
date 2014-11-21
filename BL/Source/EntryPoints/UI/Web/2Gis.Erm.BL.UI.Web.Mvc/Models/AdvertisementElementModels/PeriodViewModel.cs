using System;

using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels
{
    public class PeriodViewModel : ViewModel
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public void LoadDomainEntityDto(IPeriodAdvertisementElementDomainEntityDto dto)
        {
            BeginDate = dto.BeginDate;
            EndDate = dto.EndDate;
        }

        public void TransferToDomainEntityDto(IPeriodAdvertisementElementDomainEntityDto dto)
        {
            dto.BeginDate = BeginDate;
            dto.EndDate = EndDate;
        }
    }
}