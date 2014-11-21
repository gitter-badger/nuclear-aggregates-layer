using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core
{
    [StableContract]
    [DataContract(Namespace = ServiceNamespaces.Common.Common201309)]
    public struct TimePeriod
    {
        [DataMember]
        public readonly DateTime Start;
        [DataMember]
        public readonly DateTime End;

        public TimePeriod(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public static bool operator ==(TimePeriod left, TimePeriod right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimePeriod left, TimePeriod right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TimePeriod && Equals((TimePeriod)obj);
        }

        public bool Equals(TimePeriod other)
        {
            return other.Start.Equals(Start) && other.End.Equals(End);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ End.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("Period: start={0}, end={1}", Start, End);
        }
    }
}