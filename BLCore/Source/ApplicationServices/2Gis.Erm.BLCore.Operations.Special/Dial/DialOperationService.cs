using System;
using System.Linq;
using System.Net.Sockets;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Special.Dial
{
    public class DialOperationService : IDialOperationService
    {       
        private readonly IUserContext _userContext;

        private readonly ITracer _tracer;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserReadModel _userReadModel;

        public DialOperationService(
            IUserContext userContext,
            ITracer tracer,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserReadModel userReadModel)
        {
            this._userContext = userContext;
            this._tracer = tracer;
            this._functionalAccessService = functionalAccessService;
            this._userReadModel = userReadModel;
        }

        public void Dial(string number)
        {
            var userProfile = this._userReadModel.GetProfileForUser(this._userContext.Identity.Code);
            var department = _userReadModel.GetUserDepartment(_userContext.Identity.Code);
            
            if (string.IsNullOrEmpty(userProfile.Phone))
            {
                throw new ArgumentException(BLResources.WorkPhoneIsNotSelected);
            }
      
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.TelephonyAccess, _userContext.Identity.Code))
            {
                throw new SecurityException(BLResources.DialingIsNotAllowed);
            }

            if (string.IsNullOrEmpty(department.TelephonyAddress))
            {
                throw new ArgumentException(BLResources.TelephonyAddressIsNotSelected);
            }

            var uri = new Uri(department.TelephonyAddress);
            
            if (string.IsNullOrEmpty(uri.Scheme) || string.IsNullOrEmpty(uri.Host) || uri.Port == 0)
            {
                throw new ArgumentException(BLResources.TelephonyAddressInIncorrectFormat);
            }
            
            this.SendDialCommand(uri.Scheme,uri.Host, uri.Port, number, userProfile.Phone);
        }

        private void SendDialCommand(string scheme,string address, int port, string number, string line)
        {
            PhoneMode mode;
            switch (scheme)
            {
                case "tapi":
                    mode = PhoneMode.Tapi; 
                    break;
                case "ami":
                    mode = PhoneMode.Ami;
                    break;
                default:
                    throw new NotSupportedException(string.Format(BLResources.TelephonySchemeIsNotSupported, scheme));
            }
            var command = number.MakeXmlCommand(line, mode);
            var tcpClient = new TcpClient(address, port);
           
            var commandData = command.MakeBytesFromCommand();            
            tcpClient.Client.BeginSend(commandData, 0, commandData.Length, SocketFlags.None, this.SendCallback, tcpClient);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var client = (Socket)ar.AsyncState;

                client.EndSend(ar);

                client.Disconnect(false);
                client.Dispose();
            }
            catch (Exception ex)
            {
                this._tracer.ErrorFormat(ex, "Error has occurred in {0}.", this.GetType().Name);
            }
        }
    }
}
