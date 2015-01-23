﻿using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia
{
    [DataContract]
    public class RussiaBranchOfficeOrganizationUnitDomainEntityDto : IDomainEntityDto<BranchOfficeOrganizationUnit>, IRussiaAdapted
    {
        #region Common properties

        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string ShortLegalName { get; set; }
        [DataMember]
        public EntityReference BranchOfficeRef { get; set; }
        [DataMember]
        public EntityReference OrganizationUnitRef { get; set; }
        [DataMember]
        public bool IsPrimary { get; set; }
        [DataMember]
        public bool IsPrimaryForRegionalSales { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public EntityReference OwnerRef { get; set; }
        [DataMember]
        public EntityReference CreatedByRef { get; set; }
        [DataMember]
        public EntityReference ModifiedByRef { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        [DataMember]
        public byte[] Timestamp { get; set; }
        [DataMember]
        public Uri IdentityServiceUrl { get; set; }

        #endregion

        [DataMember]
        public long BranchOfficeAddlId { get; set; }
        [DataMember]
        public string BranchOfficeAddlName { get; set; }
        [DataMember]
        public string BranchOfficeAddlLegalAddress { get; set; }
        [DataMember]
        public string BranchOfficeAddlInn { get; set; }
        [DataMember]
        public string SyncCode1C { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string PositionInNominative { get; set; }
        [DataMember]
        public string PositionInGenitive { get; set; }
        [DataMember]
        public string ChiefNameInNominative { get; set; }
        [DataMember]
        public string ChiefNameInGenitive { get; set; }
        [DataMember]
        public string RegistrationCertificate { get; set; }
        [DataMember]
        public string ActualAddress { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PostalAddress { get; set; }
        [DataMember]
        public string PaymentEssentialElements { get; set; }
        [DataMember]
        public string Kpp { get; set; }
        [DataMember]
        public string OperatesOnTheBasisInGenitive { get; set; }
    }
}