using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    // FIXME {all, 23.09.2014}: Зачем?
    public class CategoryOrganizationUnitViewModel : EntityViewModelBase<CategoryOrganizationUnit>
    {
        #region Overrides of EntityViewModelBase<CategoryOrganizationUnit>

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            throw new NotImplementedException();
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
