using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    public class FormAvailableBinfingObjectsIdentity : OperationIdentityBase<FormAvailableBinfingObjectsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.FormAvailableBinfingObjectsIdentity; }
        }

        public override string Description
        {
            get { return "Формирование возможных для выбора объектов привязки"; }
        }
    }
}
