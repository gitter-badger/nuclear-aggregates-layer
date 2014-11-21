using System.Runtime.Serialization;

// ReSharper disable CheckNamespace

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class BargainDomainEntityDto
    {
        // COMMENT {all, 10.07.2014}: Мне кажется, что эти поля (права доступа) не имеют отношения к сущности договора, и им не место в DomainEntityDto. 
        // С другой стороны "у нас так принято". Кто-нибудь может помочь разрешить этот вопрос?
        [DataMember]
        public bool UserCanWorkWithAdvertisingAgencies { get; set; }

        [DataMember]
        public bool IsLegalPersonChoosingDenied { get; set; }

        [DataMember]
        public bool IsBranchOfficeOrganizationUnitChoosingDenied { get; set; }

        [DataMember]
        public long ClientId { get; set; }
    }
}