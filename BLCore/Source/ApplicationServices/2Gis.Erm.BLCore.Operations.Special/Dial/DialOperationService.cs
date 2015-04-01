using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.BLCore.Operations.Special.Dial.Telephony;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Special.Dial
{
    public class DialOperationService : IDialOperationService
    {
        private readonly IPhoneService _phoneService;

        public DialOperationService(IPhoneService phoneService)
        {
            _phoneService = phoneService;
        }

        public void Dial(string phone)
        {
            _phoneService.Dial(phone);
        }
    }
}
