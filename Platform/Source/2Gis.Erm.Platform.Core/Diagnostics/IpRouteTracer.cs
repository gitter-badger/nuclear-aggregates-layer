using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace DoubleGis.Erm.Platform.Core.Diagnostics
{
    // TODO {all, 17.03.2015}: Удалить после решения проблем с запросами к identity service
    internal static class IpRouteTracer
    {
        public static string Trace(string ipAddressOrHostName, int maxHops = 30)
        {
            IPAddress ipAddress;
            try
            {
                ipAddress = Dns.GetHostEntry(ipAddressOrHostName).AddressList[0];
            }
            catch (SocketException e)
            {
                return e.Message;
            }

            var traceResults = new StringBuilder();
            using (var pingSender = new Ping())
            {
                var pingOptions = new PingOptions { DontFragment = true, Ttl = 1 };
                var stopWatch = new Stopwatch();

                traceResults.AppendLine(string.Format("Tracing route to {0} over a maximum of {1} hops:", ipAddress, maxHops));
                traceResults.AppendLine();

                for (var i = 1; i < maxHops + 1; i++)
                {
                    stopWatch.Restart();
                    var pingReply = pingSender.Send(ipAddress, 5000, new byte[32], pingOptions);
                    stopWatch.Stop();

                    traceResults.AppendLine(string.Format("{0}\t{1} ms\t\t{2}", i, stopWatch.ElapsedMilliseconds, ResolveAddress(pingReply)));

                    if (pingReply.Status == IPStatus.Success)
                    {
                        traceResults.AppendLine();

                        traceResults.AppendLine("Trace complete.");
                        break;
                    }

                    pingOptions.Ttl++;
                }
            }

            return traceResults.ToString();
        }

        private static string ResolveAddress(PingReply pingReply)
        {
            if (pingReply.Address == null)
            {
                return null;
            }

            try
            {
                return string.Format("{0} [{1}]", Dns.GetHostEntry(pingReply.Address).HostName, pingReply.Address);
            }
            catch (Exception)
            {
                return pingReply.Address.ToString();
            }
        }
    }
}