using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Nuclear.Tracing.API.SystemInfo
{
    /// <summary>
    /// Класс сбора информации о сетевых параметрах (имя машины, в домене или нет)
    /// </summary>
    public static class NetworkInfo
    {
        /// <summary>
        /// Возвращает FullQualifiedDomainName для данного компьютера
        /// </summary>
        /// <value>The computer FQDN.</value>
        // ReSharper disable InconsistentNaming
        public static String ComputerFQDN
        // ReSharper restore InconsistentNaming
        {
            get 
            {
                string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName; 
                string hostName = Dns.GetHostName(); 
                string fqdn; 
                if (!hostName.Contains(domainName)) 
                    fqdn = hostName + "." + domainName; 
                else 
                    fqdn = hostName; 
             
                return fqdn; 
            }
        }

        /// <summary>
        /// Флаг -  <c>true</c> - если данный компьютер является членом домена
        /// </summary>
        public static bool IsDomainMemberComputer
        {
            get 
            {
                return !String.IsNullOrEmpty(IPGlobalProperties.GetIPGlobalProperties().DomainName);
            }
        }

        /// <summary>
        /// Описание является ли данный компьютер членом домена
        /// </summary>
        public static String DomainMembership
        {
            get 
            {
                try
                {
                    return "Computer is " +
                           (String.IsNullOrEmpty(IPGlobalProperties.GetIPGlobalProperties().DomainName) ? "NOT" : "") +
                           "domain member";
                }
                catch (Exception)
                {
                    return "Can't detect computer domain membership";
                }
            }
        }
    }
}
