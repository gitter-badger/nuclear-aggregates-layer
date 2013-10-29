using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Core.ActionLogging
{
    // FIXME {all, 23.10.2013}: Когда нужно (не)логировать в зависомости от каких-то условий, лучше явно в момент самой реализации бизнес кейса, проверять условия и только потом вызывать логирование, т.е. в таких хитрых бизнес кейсах, лучше выполнять логирование явно, не в аспектном стиле
    [Obsolete("Когда нужно (не)логировать в зависомости от каких-то условий, лучше явно в момент самой реализации бизнес кейса, проверять условия и только потом вызывать логирование")]
    public class ActionLoggingValidatorFactory : IActionLoggingValidatorFactory
    {
        private readonly IActionLoggingValidator[] _validators;

        public ActionLoggingValidatorFactory(IActionLoggingValidator[] validators)
        {
            _validators = validators;
        }

        public IEnumerable<IActionLoggingValidator> GetValidators(EntityName entityType)
        {
            return _validators.Where(x => x.EntityType == entityType).ToArray();
        }
    }
}