using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;

using NuClear.Security.API.UserContext;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Special.Dial
{
    public class DialOperationService : IDialOperationService
    {
        private static readonly Regex PhonePattern = new Regex(@"^\d{1,5}$", RegexOptions.Compiled);

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
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.TelephonyAccess, _userContext.Identity.Code))
            {
                throw new SecurityException(BLResources.DialingIsNotAllowed);
            }

            var userProfile = _userReadModel.GetProfileForUser(_userContext.Identity.Code);
            if (string.IsNullOrEmpty(userProfile.Phone))
            {
                throw new Exception(BLResources.WorkPhoneIsNotSelected);
            }
            
            if (!PhonePattern.IsMatch(userProfile.Phone))
            {
                throw new Exception(string.Format(BLResources.IncorrectPhoneNumber, userProfile.Phone));
            }

            var telephonyServerAddress = _userReadModel.GetTelephonyServerAddress(_userContext.Identity.Code);
            if (telephonyServerAddress == null)
            {
                throw new Exception(BLResources.TelephonyUnitIsNotSelected);
            }

            if (string.IsNullOrEmpty(telephonyServerAddress.Scheme) || string.IsNullOrEmpty(telephonyServerAddress.Host) || telephonyServerAddress.Port == 0)
            {
                throw new ArgumentException(BLResources.TelephonyUnitInIncorrectFormat);
            }

            InvokeDialing(telephonyServerAddress, userProfile.Phone, number);
        }

        private static async Task<TcpClient> ConnectAsync(Uri endpointUri)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(endpointUri.Host, endpointUri.Port);
            return tcpClient;
        }

        private async void InvokeDialing(Uri endpointUri, string line, string phone)
        {            
            try
            {
                using (var tcpClient = await ConnectAsync(endpointUri))
                using (var stream = tcpClient.GetStream())
                using (var writer = new CommandWriter(stream, Encoding.Unicode))
                using (var reader = new ResponseReader(stream, Encoding.Unicode))
                {
                    await writer.WriteAsync(Command.Dial(endpointUri, line, phone));

                    var response = await reader.ReadAsync();
                    if (response.Status == Response.ResponseStatus.Error)
                    {
                        _tracer.ErrorFormat("Telephony service error: {0}", response);
                    }
                    else
                    {
                        _tracer.InfoFormat("Telephony service: {0}", response);    
                    }
                }
            }
            catch (SocketException ex)
            {
                _tracer.ErrorFormat(ex, "Error connecting to telephony service {0}", endpointUri.Host);
            }
            catch (ObjectDisposedException ex)
            {
                _tracer.ErrorFormat(ex, "Probably the client has been closed. {0}", endpointUri.Host);
            }
            catch (InvalidOperationException ex)
            {
                _tracer.ErrorFormat(ex, "Probably the TcpClient is not connected to a remote host {0}", endpointUri.Host);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error occured in {0}", GetType().Name);
            }
        }
        
        #region Command

        private class Command
        {
            private const string CommandTemplate = @"<Message ServerType=""{0:D}"" Command =""{1:D}"" Line=""{2}"" Address=""{3}""/>";

            private Command()
            {
            }
            #region ServerType

            public enum ServerType
            {
                // ReSharper disable UnusedMember.Local
                None = 0,
                Tapi = 2,
                Ami = 3,
                IntegratedVoip = 4
                // ReSharper restore UnusedMember.Local
            }

            #endregion

            #region CommandType

            public enum CommandType
            {
                // ReSharper disable UnusedMember.Local
                None = 0,
                Dial = 1,
                Drop = 2
                // ReSharper restore UnusedMember.Local
            }

            #endregion            

            // ReSharper disable once MemberCanBePrivate.Local
            public CommandType Type { get; private set; }

            // ReSharper disable once MemberCanBePrivate.Local
            public ServerType Mode { get; private set; }

            // ReSharper disable once MemberCanBePrivate.Local
            public string Line { get; private set; }

            // ReSharper disable once MemberCanBePrivate.Local
            public string Number { get; private set; }            

            public static Command Dial(Uri endpointUri, string line, string number)
            {
                return new Command { Mode = ResolveMode(endpointUri), Type = CommandType.Dial, Line = line, Number = number };
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, CommandTemplate, Mode, Type, Line, Number);
            }

            private static ServerType ResolveMode(Uri endpointUri)
            {
                ServerType serverType;
                if (Enum.TryParse(endpointUri.Scheme, true, out serverType))
                {
                    switch (serverType)
                    {
                        case ServerType.Tapi:
                        case ServerType.Ami:
                            return serverType;
                    }
                }

                throw new NotSupportedException(string.Format(BLResources.TelephonySchemeIsNotSupported, endpointUri.Scheme));
            }
        }

        #endregion

        #region Response

        private class Response
        {
            #region ResponseStatus

            [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
            public enum ResponseStatus
            {
                // ReSharper disable UnusedMember.Local
                ReadyToConnect = 0,
                Error = 1,
                Connecting = 2,
                Connected = 3,
                Disconnected = 4,
                Busy = 5,
                Setup = 6
                // ReSharper restore UnusedMember.Local
            }

            #endregion

            private static readonly Regex RegStatusNumberPattern = new Regex(@"Status=""(\d)""", RegexOptions.Compiled);
            private static readonly Regex RegLineNumberPattern = new Regex(@"Line=""(.*?)""", RegexOptions.Compiled);
            private static readonly Regex RegAddressNumberPattern = new Regex(@"Address=""(.*?)""", RegexOptions.Compiled);
            private static readonly Regex RegTextNumberPattern = new Regex(@"Text=""(.*?)""", RegexOptions.Compiled);

            public Response(string message)
            {                
                Status = ResolveStatus(message);
                Line = ResolveField(RegLineNumberPattern, message);
                Address = ResolveField(RegAddressNumberPattern, message);
                Text = ResolveField(RegTextNumberPattern, message);
            }

            // ReSharper disable once MemberCanBePrivate.Local
            public ResponseStatus Status { get; private set; }
            // ReSharper disable once MemberCanBePrivate.Local
            public string Line { get; private set; }
            // ReSharper disable once MemberCanBePrivate.Local
            public string Address { get; private set; }
            // ReSharper disable once MemberCanBePrivate.Local
            public string Text { get; private set; }

            public override string ToString()
            {                
                return string.Format("Status = {0}, Line = {1}, Address = {2}, Text = {3}.", Status, Line, Address, Text);
            }

            private static ResponseStatus ResolveStatus(string message)
            {
                var result = RegStatusNumberPattern.Match(message);
                return (ResponseStatus)int.Parse(result.Groups[1].Value);
            }

            private static string ResolveField(Regex regex, string message)
            {
                if (regex.IsMatch(message))
                {
                    return regex.Match(message).Groups[1].Value;
                }

                return string.Empty;
            }
        }

        #endregion

        #region CommandWriter

        private class CommandWriter : IDisposable
        {
            private const ushort SizePrefixLength = 2;
            
            private readonly Stream _stream;
            private readonly Encoding _encoding;

            public CommandWriter(Stream stream, Encoding encoding)
            {
                _stream = stream;
                _encoding = encoding;
            }

            public async Task WriteAsync(Command command)
            {
                if (command == null)
                {
                    throw new ArgumentNullException("command");
                }

                var commandText = command.ToString();
                var commandSize = (ushort)(SizePrefixLength + _encoding.GetByteCount(commandText));

                await WriteAsync(commandSize);
                await WriteAsync(commandText);
            }

            public void Dispose()
            {
            }

            private Task WriteAsync(ushort value)
            {
                return WriteAsync(BitConverter.GetBytes(value));
            }

            private Task WriteAsync(string value)
            {
                return WriteAsync(_encoding.GetBytes(value));
            }

            private Task WriteAsync(byte[] data)
            {
                return _stream.WriteAsync(data, 0, data.Length);
            }
        }

        #endregion

        #region ResponseReader

        private class ResponseReader : IDisposable
        {
            private const ushort SizePrefixLength = 2;

            private readonly Stream _stream;
            private readonly Encoding _encoding;

            public ResponseReader(Stream stream, Encoding encoding)
            {
                _stream = stream;
                _encoding = encoding;
            }

            public async Task<Response> ReadAsync()
            {
                return new Response(await ReadMessageAsync());
            }

            public void Dispose()
            {
            }

            private async Task<string> ReadMessageAsync()
            {
                var sizeData = new byte[SizePrefixLength];
                await ReadBytesAsync(sizeData);

                var messageLength = BitConverter.ToUInt16(sizeData, 0);
                var messageData = new byte[messageLength - SizePrefixLength];
                await ReadBytesAsync(messageData);

                return _encoding.GetString(messageData);
            }

            private async Task ReadBytesAsync(byte[] data)
            {
                int position = 0,
                    size = data.Length;
                do
                {
                    var count = await _stream.ReadAsync(data, position, size);

                    position += count;
                    size -= count;
                }
                while (size > 0);
            }
        }

        #endregion
    }
}
