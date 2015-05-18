using System.Runtime.Serialization;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    // TODO: ExtendedInfo хранить и передавать с js как Array
    [DataContract]
    public sealed class SearchListModel
    {
        [DataMember]
        public string FilterInput { get; set; }

        [DataMember]
        public IEntityType ParentEntityName { get; set; }

        [DataMember]
        public long? ParentEntityId { get; set; }

        [DataMember]
        public string ExtendedInfo { get; set; }

        [DataMember]
        public string NameLocaleResourceId { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public int Limit { get; set; }

        [DataMember]
        public string Sort { get; set; }
    }
}