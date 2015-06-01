using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared
{
    public static class LegalPersonViewModelsTestHelper
    {
        public const string TestLegalName = "TestLegalName";
        public static readonly EntityReference TestClient = new EntityReference(2, "TestClient");
        public static readonly EntityReference TestCommune = new EntityReference(3, "TestCommune");
        public const string TestInn = "TestInn";
        public const string TestVat = "TestVat";
        public const TaxationType TestTaxationType = TaxationType.WithoutVat;
        public const string TestLegalAddress = "TestLegalAddress";
        public const string TestComment = "TestComment";
        public const bool TestHasProfiles = true;
        public const long TestId = 5;
        public static readonly byte[] TestTimestamp = { 1, 2, 3, 4, 5, 6, 7, 8 };
        public const string TestEgrpou = "TestEgrpou";
        public const string TestOperationsKind = "TestOperationsKind";
        public const LegalPersonType TestLegalPersonType = LegalPersonType.Businessman;
        public const string TestPassportSeries = "TestPassportSeries";
        public const string TestPassportNumber = "TestPassportNumber";
        public const string TestPassportIssuedBy = "TestPassportIssuedBy";
        public const string TestRegistrationAddress = "TestRegistrationAddress";
        public const string TestCardNumber = "TestCardNumber";
        public const string TestIc = "TestIc";
        public const string TestKpp = "TestKpp";

        public static void FillUkraineLegalPersonDtoWithTestData(UkraineLegalPersonDomainEntityDto entityDto)
        {
            entityDto.Id = TestId;
            entityDto.LegalName = TestLegalName;
            entityDto.ClientRef = TestClient;
            entityDto.Inn = TestInn;
            entityDto.LegalAddress = TestLegalAddress;
            entityDto.Comment = TestComment;
            entityDto.HasProfiles = TestHasProfiles;
            entityDto.Timestamp = TestTimestamp;
            entityDto.LegalPersonTypeEnum = TestLegalPersonType;
            entityDto.PassportSeries = TestPassportSeries;
            entityDto.PassportNumber = TestPassportNumber;
            entityDto.PassportIssuedBy = TestPassportIssuedBy;
            entityDto.RegistrationAddress = TestRegistrationAddress;
            entityDto.TaxationType = TestTaxationType;
        }

        public static void FillChileLegalPersonDtoWithTestData(ChileLegalPersonDomainEntityDto entityDto)
        {
            entityDto.Id = TestId;
            entityDto.LegalName = TestLegalName;
            entityDto.ClientRef = TestClient;
            entityDto.Rut = TestInn;
            entityDto.LegalAddress = TestLegalAddress;
            entityDto.Comment = TestComment;
            entityDto.HasProfiles = TestHasProfiles;
            entityDto.Timestamp = TestTimestamp;
            entityDto.LegalPersonTypeEnum = TestLegalPersonType;
            entityDto.PassportSeries = TestPassportSeries;
            entityDto.PassportNumber = TestPassportNumber;
            entityDto.PassportIssuedBy = TestPassportIssuedBy;
            entityDto.RegistrationAddress = TestRegistrationAddress;
            entityDto.OperationsKind = TestOperationsKind;
            entityDto.CommuneRef = TestCommune;
        }

        public static void FillRussiaLegalPersonDtoWithTestData(LegalPersonDomainEntityDto entityDto)
        {
            FillLegalPersonDtoWithData(entityDto);
        }

        public static void FillCyprusLegalPersonDtoWithTestData(LegalPersonDomainEntityDto entityDto)
        {
            FillLegalPersonDtoWithData(entityDto);
        }

        public static void FillCzechLegalPersonDtoWithTestData(LegalPersonDomainEntityDto entityDto)
        {
            FillLegalPersonDtoWithData(entityDto);
        }

        public static void FillUkraineLegalPersonModelWithTestData(UkraineLegalPersonViewModel model)
        {
            model.Id = TestId;
            model.LegalName = TestLegalName;
            model.Client = TestClient.ToLookupField();
            model.Ipn = TestInn;
            model.TaxationType = TestTaxationType;
            model.LegalAddress = TestLegalAddress;
            model.Comment = TestComment;
            model.Timestamp = TestTimestamp;
        }

        public static void FillChileLegalPersonModelWithTestData(ChileLegalPersonViewModel model)
        {
            model.Id = TestId;
            model.LegalName = TestLegalName;
            model.Client = TestClient.ToLookupField();
            model.Rut = TestInn;
            model.LegalAddress = TestLegalAddress;
            model.Comment = TestComment;
            model.Timestamp = TestTimestamp;
            model.OperationsKind = TestOperationsKind;
            model.Commune = TestCommune.ToLookupField();
            model.LegalPersonType = TestLegalPersonType;
        }

        public static void FillCyprusLegalPersonModelWithTestData(CyprusLegalPersonViewModel model)
        {
            model.Id = TestId;
            model.LegalName = TestLegalName;
            model.VAT = TestVat;
            model.Client = TestClient.ToLookupField();
            model.LegalAddress = TestLegalAddress;
            model.Comment = TestComment;
            model.Timestamp = TestTimestamp;
            model.PassportNumber = TestPassportNumber;
            model.PassportIssuedBy = TestPassportIssuedBy;
            model.RegistrationAddress = TestRegistrationAddress;
            model.CardNumber = TestCardNumber;
        }

        public static void FillRussiaLegalPersonModelWithTestData(LegalPersonViewModel model)
        {
            model.Id = TestId;
            model.LegalName = TestLegalName;
            model.Kpp = TestKpp;
            model.Client = TestClient.ToLookupField();
            model.LegalAddress = TestLegalAddress;
            model.Comment = TestComment;
            model.Timestamp = TestTimestamp;
            model.PassportNumber = TestPassportNumber;
            model.PassportIssuedBy = TestPassportIssuedBy;
            model.RegistrationAddress = TestRegistrationAddress;
            model.PassportSeries = TestPassportSeries;
        }

        public static void FillCzechLegalPersonModelWithTestData(CzechLegalPersonViewModel model)
        {
            model.Id = TestId;
            model.LegalName = TestLegalName;
            model.Client = TestClient.ToLookupField();
            model.LegalAddress = TestLegalAddress;
            model.Comment = TestComment;
            model.Timestamp = TestTimestamp;
            model.Ic = TestIc;
            model.Inn = TestInn;
        }

        static void FillLegalPersonDtoWithData(LegalPersonDomainEntityDto entityDto)
        {
            entityDto.Id = TestId;
            entityDto.LegalName = TestLegalName;
            entityDto.ClientRef = TestClient;
            entityDto.Inn = TestInn;
            entityDto.LegalAddress = TestLegalAddress;
            entityDto.Comment = TestComment;
            entityDto.HasProfiles = TestHasProfiles;
            entityDto.Timestamp = TestTimestamp;
            entityDto.LegalPersonTypeEnum = TestLegalPersonType;
            entityDto.PassportSeries = TestPassportSeries;
            entityDto.PassportNumber = TestPassportNumber;
            entityDto.PassportIssuedBy = TestPassportIssuedBy;
            entityDto.RegistrationAddress = TestRegistrationAddress;
            
            entityDto.CardNumber = TestCardNumber;
            entityDto.VAT = TestVat;
            entityDto.Ic = TestIc;
            entityDto.Kpp = TestKpp;
        }
    }
}
