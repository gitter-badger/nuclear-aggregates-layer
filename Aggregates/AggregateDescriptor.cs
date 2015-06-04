using System;
using System.Text;

using NuClear.Model.Common.Entities;

namespace NuClear.Aggregates
{
    public class AggregateDescriptor
    {
        private readonly IEntityType _aggregateRoot;
        private readonly IEntityType[] _aggregateEntities;

        public AggregateDescriptor(IEntityType aggregateRoot, IEntityType[] aggregateEntities)
        {
            if (aggregateEntities == null || aggregateEntities.Length == 0)
            {
                throw new ArgumentNullException("aggregateEntities");
            }

            _aggregateRoot = aggregateRoot;
            _aggregateEntities = aggregateEntities;
        }

        public IEntityType AggregateRoot
        {
            get
            {
                return _aggregateRoot;
            }
        }

        public IEntityType[] AggregateEntities
        {
            get
            {
                return _aggregateEntities;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(AggregateRoot + " Aggregate;");
            sb.AppendLine("Alias: " + AggregateRoot.Description + ";");
            sb.AppendLine("Members:");
            foreach (var aggregateEntity in AggregateEntities)
            {
                sb.AppendLine(aggregateEntity.Description + ";");
            }

            return sb.ToString();
        }
    }
}
