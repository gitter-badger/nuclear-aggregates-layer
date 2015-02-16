using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity
{
     [DataContract]
    public class ChangeActivityStatusIdentity : OperationIdentityBase<ChangeActivityStatusIdentity>
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeActivityStatusIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "ChangeActivityStatus";
            }
        }
    }
}
