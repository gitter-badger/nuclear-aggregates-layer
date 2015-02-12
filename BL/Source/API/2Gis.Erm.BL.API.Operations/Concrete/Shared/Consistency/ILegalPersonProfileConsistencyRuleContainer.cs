using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency
{
    public interface ILegalPersonProfileConsistencyRuleContainer
    {
        IEnumerable<IConsistencyRule> GetApplicableRules(LegalPerson person, LegalPersonProfile profile);
    }
}
