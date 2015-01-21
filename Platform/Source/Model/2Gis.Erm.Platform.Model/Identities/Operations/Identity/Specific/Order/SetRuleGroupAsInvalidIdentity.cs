using System;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    [Obsolete("Выводиться из обращения, после обработки всех операций можно будет удалить, также как и ключ OperationIdentityIds.ResetValidationGroupIdentity")]
    public sealed class SetRuleGroupAsInvalidIdentity : OperationIdentityBase<SetRuleGroupAsInvalidIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ResetValidationGroupIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Отметка результатов проверок заказов в группе как уже невалидных";
            }
        }
    }
}
