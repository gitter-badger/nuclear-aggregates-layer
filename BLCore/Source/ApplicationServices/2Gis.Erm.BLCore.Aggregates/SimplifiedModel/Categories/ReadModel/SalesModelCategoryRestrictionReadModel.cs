using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Categories.ReadModel
{
    public class SalesModelCategoryRestrictionReadModel : ISalesModelCategoryRestrictionReadModel
    {
        private readonly IFinder _finder;

        public SalesModelCategoryRestrictionReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public IEnumerable<SalesModelCategoryRestriction> GetRestrictionsByProject(long projectId)
        {
            return _finder.FindMany(CategorySpecs.SalesModelCategoryRestrictions.Find.ByProject(projectId));
        }
    }
}