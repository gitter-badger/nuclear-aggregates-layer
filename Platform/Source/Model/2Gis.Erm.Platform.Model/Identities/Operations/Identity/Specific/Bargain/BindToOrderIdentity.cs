using System;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain
{
    // FIXME {all, 14.01.2014}: при рефакторинге BargainService и выделение CreateBargainForOrder в самостоятельный operation service нужно избавиться/зарефакторитьBindToOrderIdentity 
    // - логировать нужно createidentity договора, BindToOrderIdentity либо вообще удалить, переименовать в AppendBargain, AttachBargain, BindBargain и т.п. (заиспользовав тот же id операции)
    [Obsolete]
    public sealed class BindToOrderIdentity : OperationIdentityBase<BindToOrderIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.BindBargainToOrderIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Прикрепить к заказу";
            }
        }
    }
}