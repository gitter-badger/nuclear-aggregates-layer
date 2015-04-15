using System;
using System.Linq;
using System.Net.Sockets;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Special.Dial
{
    public class DialOperationService : IDialOperationService
    {       
        private readonly IUserContext _userContext;

        private readonly ITracer _tracer;

        private readonly IUserReadModel _userReadModel;

        public DialOperationService(
            IUserContext userContext,
            ITracer tracer,
            IUserReadModel userReadModel)
        {
            this._userContext = userContext;
            this._tracer = tracer;
            this._userReadModel = userReadModel;
        }

        public void Dial(string number)
        {
            var user = this._userReadModel.GetProfileForUser(this._userContext.Identity.Code);
            if (string.IsNullOrEmpty(user.Phone))
            {
                throw new ArgumentException(BLResources.WorkPhoneIsNotSelected);
            }

            if (!user.CanCall)
            {
                throw new SecurityException(BLResources.DialingIsNotAllowed);
            }

            if (string.IsNullOrEmpty(user.TelephonyAddress))
            {
                throw new ArgumentException(BLResources.TelephonyAddressIsNotSelected);
            }

            var serverAddressPort = user.TelephonyAddress.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (serverAddressPort.Count() != 2)
            {
                throw new ArgumentException(BLResources.TelephonyAddressInIncorrectFormat);
            }

            int serverPort;
            if (!int.TryParse(serverAddressPort[1], out serverPort))
            {
                throw new ArgumentException(BLResources.TelephonyAddressInIncorrectFormat);
            }

            var serverAddress = serverAddressPort[0];

            this.SendDialCommand(serverAddress, serverPort, number, user.Phone);
        }

        private void SendDialCommand(string address, int port, string number, string line)
        {
            var command = number.MakeXmlCommand(line);
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
