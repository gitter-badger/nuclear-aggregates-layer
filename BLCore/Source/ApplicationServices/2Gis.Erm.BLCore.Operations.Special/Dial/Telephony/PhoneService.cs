using System;
using System.Net.Sockets;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Special.Dial.Telephony
{
    public class PhoneService : IPhoneService
    {       
        private readonly IUserContext _userContext;

        private readonly ITracer _tracer;

        private readonly IUserReadModel _userReadModel;

        public PhoneService(
            IUserContext userContext,
            ITracer tracer,
            IUserReadModel userReadModel)
        {
            _userContext = userContext;
            _tracer = tracer;
            _userReadModel = userReadModel;
        }

        public void Dial(string number)
        {
            var user = _userReadModel.GetProfileForUser(_userContext.Identity.Code);
            if (string.IsNullOrEmpty(user.Phone))
            {
                throw new ArgumentException(BLResources.WorkPhoneIsNotSelected);
            }

            SendDialCommand(number, user.Phone);
        }

        private void SendDialCommand(string number, string line)
        {
            const string ServerAddress = "uk-erm-tapi01";
            const int ServerPort = 1313;
            var command = number.MakeXmlCommand(line);
            var tcpClient = new TcpClient(ServerAddress, ServerPort);
           
                var commandData = command.MakeBytesFromCommand();
            tcpClient.Client.BeginSend(commandData, 0, commandData.Length, SocketFlags.None, SendCallback, tcpClient);
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
                _tracer.ErrorFormat(ex, "Error has occurred in {0}.", GetType().Name);
            }
        }
    }
}
