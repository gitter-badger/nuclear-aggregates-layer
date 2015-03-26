using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Special.Dial
{
    public class DialOperationService : IDialOperationService
    {
        private readonly IUserContext _userContext;

        private readonly IUserReadModel _userReadModel;

        public DialOperationService(
            IUserContext userContext, 
            IUserReadModel userReadModel)
        {
            _userContext = userContext;
            _userReadModel = userReadModel;
        }

        public DialResult Dial(string phone)
        {

            var user = _userReadModel.GetProfileForUser(_userContext.Identity.Code);
            return new DialResult(user.Phone);
        }
    }
}
