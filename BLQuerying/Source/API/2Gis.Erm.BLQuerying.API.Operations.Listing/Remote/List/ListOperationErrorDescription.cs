﻿using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.API.Operations.Remote;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.Remote.List
{
    [DataContract]
    public class ListOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ListOperationErrorDescription(EntityName entityName, string message)
        {
            EntityName = entityName;
            Message = message;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
