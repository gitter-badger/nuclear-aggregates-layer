using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging.Validators
{
    // FIXME {all, 23.10.2013}: Когда нужно (не)логировать в зависомости от каких-то условий, лучше явно в момент самой реализации бизнес кейса, проверять условия и только потом вызывать логирование, т.е. в таких хитрых бизнес кейсах, лучше выполнять логирование явно, не в аспектном стиле
    [Obsolete("Когда нужно (не)логировать в зависомости от каких-то условий, лучше явно в момент самой реализации бизнес кейса, проверять условия и только потом вызывать логирование")]
    public class LegalPersonActionLoggingValidator : IActionLoggingValidator
    {
        private readonly IFinder _finder;

        public LegalPersonActionLoggingValidator(IFinder finder)
        {
            _finder = finder;
        }

        EntityName IActionLoggingValidator.EntityType
        {
            get { return EntityName.LegalPerson; }
        }

        bool IActionLoggingValidator.Validate(long entityId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(entityId))
                          .Select(x => x.IsInSyncWith1C)
                          .SingleOrDefault();
        }
    }
}