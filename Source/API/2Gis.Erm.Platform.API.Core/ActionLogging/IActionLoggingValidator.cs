using System;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Core.ActionLogging
{
    // FIXME {all, 23.10.2013}: Когда нужно (не)логировать в зависомости от каких-то условий, лучше явно в момент самой реализации бизнес кейса, проверять условия и только потом вызывать логирование, т.е. в таких хитрых бизнес кейсах, лучше выполнять логирование явно, не в аспектном стиле
    [Obsolete("Когда нужно (не)логировать в зависомости от каких-то условий, лучше явно в момент самой реализации бизнес кейса, проверять условия и только потом вызывать логирование")]
    public interface IActionLoggingValidator : ISimplifiedModelConsumer
    {
        EntityName EntityType { get; }

        bool Validate(long entityId);
    }
}