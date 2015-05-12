using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.DTOs;
using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BL.Aggregates.SimplifiedModel.ReadModel
{
    public class DenialReasonReadModel : IDenialReasonReadModel
    {
        private readonly IFinder _finder;

        public DenialReasonReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public DenialReason GetDenialReason(long denialReasonId)
        {
            return _finder.FindOne(Specs.Find.ById<DenialReason>(denialReasonId));
        }

        public bool IsThereDuplicateByName(long denialReasonId, string name)
        {
            return
                _finder.Find(DenialReasonSpecs.DenialReasons.Find.DuplicateByName(denialReasonId, name) &&
                             Specs.Find.Active<DenialReason>()).Any();
        }

        public IEnumerable<DenialReasonDto> GetInactiveDenialReasons(IEnumerable<long> denialReasonIds)
        {
            return _finder.Find(Specs.Find.ByIds<DenialReason>(denialReasonIds) && Specs.Find.InactiveEntities<DenialReason>())
                          .Select(x => new DenialReasonDto
                              {
                                  Id = x.Id,
                                  Name = x.Name
                              })
                          .ToArray();
        }
    }
}
