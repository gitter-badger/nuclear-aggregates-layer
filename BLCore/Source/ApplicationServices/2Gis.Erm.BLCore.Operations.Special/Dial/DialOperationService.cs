using System;
using System.IO;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;

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
            _userContext = userContext;
            _tracer = tracer;            
            _functionalAccessService = functionalAccessService;
            _userReadModel = userReadModel;
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
            
            SendDialCommand(uri.Scheme, uri.Host, uri.Port, number, userProfile.Phone);
        }

        private void SendDialCommand(string scheme, string address, int port, string number, string line)
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
            Send(command, address, port);           
        }
      
        private async void Send(string command, string address, int port)
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }

            using (var tcpClient = new TcpClient())
            {
                try
                {
                    await tcpClient.ConnectAsync(address, port);
                }
                catch (SocketException ex)
                {
                    _tracer.ErrorFormat(ex, "Error connecting to telephony service {0}", address);
                }
                catch (Exception ex)
                {
                    _tracer.ErrorFormat(ex, "Unknown connecting to telephony service {0} error", address);
                }

                try
                {
                    using (var stream = tcpClient.GetStream())
                    {
                        var commandData = command.MakeBytesFromCommand();
                        await stream.WriteAsync(commandData, 0, commandData.Length);

                        PhoneSessionStatus status;
                        do
                        {
                            var message = await ReadMessage(stream);
                            _tracer.DebugFormat("Telephony service: {0}", message);
                            status = message.GetStatusFromResponse();                           
                        }
                        while (status != PhoneSessionStatus.Disconnected);
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    _tracer.ErrorFormat(ex, "Probably the client has been closed. {0}", address);
                }
                catch (InvalidOperationException ex)
                {
                    _tracer.ErrorFormat(ex, "Probably the TcpClient is not connected to a remote host {0}", address);
                }
                catch (Exception ex)
                {
                    _tracer.ErrorFormat(ex, "Error occured in {0}", this.GetType().Name);
                }
            }
        }

        private static async Task<string> ReadMessage(Stream stream)
        {
            const int SizePrefixLength = 2;
            var sizeData = new byte[SizePrefixLength];
            await ReadBytes(stream, sizeData);

            var messageLength = BitConverter.ToUInt16(sizeData, 0);
            var messageData = new byte[messageLength - SizePrefixLength];
            await ReadBytes(stream, messageData);

            return Encoding.Unicode.GetString(messageData);
        }

        private static async Task ReadBytes(Stream stream, byte[] data)
        {
            var position = 0;
            var size = data.Length;
            do
            {
                var count = await stream.ReadAsync(data, position, size);

                position += count;
                size -= count;
            }
            while (size > 0);
        }
    }
}
