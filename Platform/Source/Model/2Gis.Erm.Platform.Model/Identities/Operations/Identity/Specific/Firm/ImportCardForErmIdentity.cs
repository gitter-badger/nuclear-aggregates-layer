using System;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportCardForErmIdentity : OperationIdentityBase<ImportCardForErmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportCardForErmIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщеиня flowCardsForERM.CardForERM"; }
        }
    }
}