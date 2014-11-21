﻿using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class UserProfileDomainEntityDto
    {
        [DataMember]
        public long ProfileId { get; set; }
        [DataMember]
        public string DomainAccountName { get; set; }
        [DataMember]
        public Uri IdentityServiceUrl { get; set; }
    }
}