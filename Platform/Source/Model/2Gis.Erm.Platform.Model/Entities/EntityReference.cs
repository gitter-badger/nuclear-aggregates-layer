﻿using System.Runtime.Serialization;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    [DataContract]
    public sealed class EntityReference
    {
        public EntityReference(long? id, string name)
        {
            Id = id;
            Name = name;
        }

        public EntityReference(long? id)
        {
            Id = id;
        }

        public EntityReference()
        {
        }

        [DataMember]
        public long? Id { get; set; }
        [DataMember]
        public string Name { get; set; }

        // might be redundant
        [DataMember]
        public IEntityType EntityName { get; set; }
    }
}