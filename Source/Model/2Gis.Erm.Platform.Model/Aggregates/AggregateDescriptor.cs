using System;
using System.Text;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates
{
    public class AggregateDescriptor
    {
        private readonly EntityName _aggregateRoot;
        private readonly EntityName[] _aggregateEntities;
        private readonly Type _aggregateAliasType;

        public AggregateDescriptor(params EntityName[] aggregateEntities)
        {
            if (aggregateEntities == null || aggregateEntities.Length == 0)
            {
                throw new ArgumentNullException("aggregateEntities");
            }

            _aggregateRoot = aggregateEntities[0];
            _aggregateEntities = aggregateEntities;
        }

        public AggregateDescriptor(EntityName aggregateRoot, EntityName[] aggregateEntities, Type aggregateAliasType)
        {
            if (aggregateEntities == null || aggregateEntities.Length == 0)
            {
                throw new ArgumentNullException("aggregateEntities");
            }

            _aggregateRoot = aggregateRoot;
            _aggregateEntities = aggregateEntities;
            _aggregateAliasType = aggregateAliasType;
        }

        public EntityName AggregateRoot
        {
            get
            {
                return _aggregateRoot;
            }
        }

        public EntityName[] AggregateEntities
        {
            get
            {
                return _aggregateEntities;
            }
        }

        /// <summary>
        /// Тип alias enum агрегата содержащего подмножество элементов EntityName, относящихся к данному агрегату
        /// </summary>
        public Type AliasType
        {
            get
            {
                return _aggregateAliasType;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(AggregateRoot + " Aggregate;");
            sb.AppendLine("Alias: " + AliasType.Name + ";");
            sb.AppendLine("Members:");
            foreach (var aggregateEntity in AggregateEntities)
            {
                sb.AppendLine(aggregateEntity.ToString() + ";");
            }

            return sb.ToString();
        }
    }
}
