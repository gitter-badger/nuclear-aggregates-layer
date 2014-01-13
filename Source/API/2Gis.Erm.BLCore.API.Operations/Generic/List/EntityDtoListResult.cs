using System.Collections;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    [DataContract]
    public sealed class EntityDtoListResult<TEntity, TEntityListDto> : ListResult, IDataListResult
        where TEntity : IEntityKey 
        where TEntityListDto : IListItemEntityDto<TEntity>
    {
        private EntityName _entityType = typeof(TEntity).AsEntityName();

        public override ListResultType ResultType
        {
            get
            {
                return ListResultType.Dto;
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
        public TEntityListDto[] Data { get; set; }

        [DataMember]
        public EntityName EntityType
        {
            get
            {
                return _entityType;
            }

            private set
            {
                _entityType = value;
            }
        }
    }
}