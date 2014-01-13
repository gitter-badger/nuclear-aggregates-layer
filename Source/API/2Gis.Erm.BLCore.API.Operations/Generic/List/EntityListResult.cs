using System.Collections;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    [DataContract]
    public sealed class EntityListResult<TEntity> : ListResult, IDataListResult
        where TEntity : IEntityKey
    {
        private readonly EntityName _entityType = typeof(TEntity).AsEntityName();

        public override ListResultType ResultType
        {
            get
            {
                return ListResultType.Entity;
            }
            set
            {
                
            }
        }

        IEnumerable IDataListResult.Data
        {
            get { return Data; }
        }

        [DataMember]
        public TEntity[] Data { get; set; }

        [DataMember]
        public EntityName EntityType
        {
            get
            {
                return _entityType;
            }
        }
    }
}