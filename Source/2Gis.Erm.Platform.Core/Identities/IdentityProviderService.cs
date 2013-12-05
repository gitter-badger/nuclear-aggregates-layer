using System;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Identities;

namespace DoubleGis.Erm.Platform.Core.Identities
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

        private readonly IIdentityProviderSettings _settings;
        private readonly object _timeCheckSync = new object();
        private long _previousNowValue;
        private int _incrementedValue;
        
        public IdentityProviderService(IIdentityProviderSettings settings)
        {
            _settings = settings;
            _previousNowValue = 0;
            _incrementedValue = 0;
        }

        public long[] GetIdentities(int count)
        {
            if ((count & ~SequenceMask) != 0)
            {
                throw new ArgumentException(string.Format("Can not generate more than {0} ids at once", SequenceMask));
            }

            if ((_settings.IdentityServiceUniqueId & ~IdentityProviderMask) != 0)
            {
                throw new ArgumentException(string.Format("Can not generate with id provider value greater than {0}", IdentityProviderMask));
            }

            var timestamp = GetTimestamp();
            var startIndex = Interlocked.Add(ref _incrementedValue, count) - count;

            if ((timestamp & ~TimeMask) != 0)
            {
                throw new ArgumentException(string.Format("Erm epoch has ended at {0}, you need to refactor entity identities", new DateTime(ErmEpochStart + (TimeMask * 10000))));
            }

            return Ids(timestamp, count, startIndex, _settings.IdentityServiceUniqueId);
        }

        private static long GetCurrentTicks()
        {
            return DateTime.UtcNow.Ticks;
        }

        private static long[] Ids(long timestamp, int count, int startIndex, int identityProvider)
        {
            var result = new long[count];

            for (var i = 0; i < count; i++)
            {
                var sequence = (startIndex + i) & SequenceMask;
                result[i] = timestamp << (SequenceMaskLength + IdentityProviderMaskLength);
                result[i] |= sequence << IdentityProviderMaskLength;
                result[i] |= (uint)identityProvider;
            }

            return result;
        }

        private static long WaitForTimeAfter(long lastTimestamp)
        {
            var generatedTimeStamp = GetCurrentTicks();
            while (generatedTimeStamp <= lastTimestamp)
            {
                generatedTimeStamp = GetCurrentTicks();
            }

            return generatedTimeStamp;
        }

        private long GetTimestamp()
        {
            lock (_timeCheckSync)
            {
                var now = GetCurrentTicks();
                if (now < _previousNowValue)
                {
                    throw new ArgumentException(string.Format("Clock value was moved back, id generation stopped for {0} milliseconds", (_previousNowValue - now) / 10000));
                }

                if (now == _previousNowValue)
                {
                    now = WaitForTimeAfter(now);
                }

                _previousNowValue = now;
                return (now - ErmEpochStart) / 10000; // There are 10,000 ticks in a millisecond (http://msdn.microsoft.com/en-gb/library/system.datetime.ticks.aspx) 
            }
        }
    }
}
