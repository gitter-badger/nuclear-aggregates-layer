using System;
using System.Threading;

namespace DoubleGis.Erm.Platform.Common.Identities
{
    public sealed class IdentityProviderService : IIdentityProviderService
    {
        private const int TimeMaskLength = 41;
        private const int IdentityProviderMaskLength = 8;
        private const int SequenceMaskLength = 15;

        private const long TimeMask = -1L ^ (-1L << TimeMaskLength);
        private const long SequenceMask = -1L ^ (-1L << SequenceMaskLength);
        private const long IdentityProviderMask = -1L ^ (-1L << IdentityProviderMaskLength);

        private const long ErmEpochStart = 634925952000000000; // Эпоха ЕРМ началась 2013-01-01

        private readonly IIdentityServiceUniqueIdProvider _identityServiceUniqueIdProvider;
        private readonly object _timeCheckSync = new object();
        private long _previousTimestamp;

        public IdentityProviderService(IIdentityServiceUniqueIdProvider identityServiceUniqueIdProvider)
        {
            _identityServiceUniqueIdProvider = identityServiceUniqueIdProvider;
            _previousTimestamp = 0;
        }

        public long[] GetIdentities(int count)
        {
            if ((count & ~SequenceMask) != 0)
            {
                throw new ArgumentException(string.Format("Cannot generate more than {0} ids at once", SequenceMask));
            }

            var identityServiceUniqueId = _identityServiceUniqueIdProvider.GetUniqueId();
            if ((identityServiceUniqueId & ~IdentityProviderMask) != 0)
            {
                throw new ArgumentException(string.Format("Cannot generate with id provider value greater than {0}", IdentityProviderMask));
            }

            var timestamp = GetTimestamp();

            if ((timestamp & ~TimeMask) != 0)
            {
                throw new ArgumentException(string.Format("Erm epoch has ended at {0}, you need to refactor entity identities",
                                                          new DateTime(ErmEpochStart + (TimeMask * 10000))));
            }

            return Ids(timestamp, count, identityServiceUniqueId);
        }

        private static long GetCurrentTimestamp()
        {
            var now = DateTime.UtcNow.Ticks;

            // There are 10,000 ticks in a millisecond (http://msdn.microsoft.com/en-gb/library/system.datetime.ticks.aspx) 
            return (now - ErmEpochStart) / 10000;
        }

        private static long[] Ids(long timestamp, int count, int identityProvider)
        {
            var result = new long[count];

            for (var i = 0; i < count; i++)
            {
                var sequence = i;
                result[i] = timestamp << (SequenceMaskLength + IdentityProviderMaskLength);
                result[i] |= sequence << IdentityProviderMaskLength;
                result[i] |= (uint)identityProvider;
            }

            return result;
        }

        private static long WaitForTimeAfter(long lastTimestamp)
        {
            var generatedTimestamp = GetCurrentTimestamp();
            while (generatedTimestamp <= lastTimestamp)
            {
                Thread.Sleep(1);
                generatedTimestamp = GetCurrentTimestamp();
            }

            return generatedTimestamp;
        }

        private long GetTimestamp()
        {
            lock (_timeCheckSync)
            {
                var currentTimestamp = GetCurrentTimestamp();
                if (currentTimestamp < _previousTimestamp)
                {
                    throw new ArgumentException(string.Format("Clock value was moved back, id generation has been stopped for {0} milliseconds",
                                                              _previousTimestamp - currentTimestamp));
                }

                if (currentTimestamp == _previousTimestamp)
                {
                    currentTimestamp = WaitForTimeAfter(currentTimestamp);
                }

                _previousTimestamp = currentTimestamp;

                return currentTimestamp;
            }
        }
    }
}
