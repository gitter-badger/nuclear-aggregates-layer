using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Category;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    public interface IChangeCategoryGroupService : IOperation<ChangeCategoryGroupIdentity>
    {
        void SetCategoryGroupMembership(IEnumerable<CategoryGroupMembershipDto> membership);
    }
}