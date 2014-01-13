
using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
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
