using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonDtoServiceSpecs
    {
        public abstract class UkraineGetLegalPersonDtoServiceContext
        {
            protected static UkraineGetLegalPersonDtoService UkraineGetLegalPersonDtoService;
            protected static IUserContext UserContext;
            protected static IClientReadModel ClientReadModel;
            protected static ILegalPersonReadModel LegalPersonReadModel;
            protected static ISecureFinder SecureFinder;
            protected static IDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static EntityName ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    ClientReadModel = Mock.Of<IClientReadModel>();
                    LegalPersonReadModel = Mock.Of<ILegalPersonReadModel>();
                    UserContext = Mock.Of<IUserContext>(x => x.Identity == new NullUserIdentity());
                    SecureFinder = Mock.Of<ISecureFinder>();

                    UkraineGetLegalPersonDtoService = new UkraineGetLegalPersonDtoService(UserContext, ClientReadModel, LegalPersonReadModel, null);
                };

            Because of = () =>
                {
                    Result = UkraineGetLegalPersonDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
                };
        }

        [Obsolete("Test must be actualized")]
        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonDtoService))]
        class When_requested_existing_entity : UkraineGetLegalPersonDtoServiceContext
        {
            protected static LegalPerson LegalPerson;
            protected static UkraineLegalPersonPart UkraineLegalPersonPart;

            const string TestEgrpou = "TestEGRPOU";
            const TaxationType TestTaxationType = TaxationType.WithoutVat;
            const long entityId = 1;

            Establish context = () =>
                {
                    EntityId = entityId; // C Id != 0 мы получим Dto из хранилища

                    UkraineLegalPersonPart = new UkraineLegalPersonPart { Egrpou = TestEgrpou, TaxationType = TestTaxationType };
                    LegalPerson = new LegalPerson { Parts = new[] { UkraineLegalPersonPart }, Timestamp = new byte[0] }; // Timestamp != null показывает, что LegalPerson не новая сущность

                    Mock.Get(LegalPersonReadModel).Setup(x => x.GetLegalPerson(EntityId)).Returns(LegalPerson);
                };

            It should_be_UkraineLegalPersonDomainEntityDto = () => Result.Should().BeOfType<UkraineLegalPersonDomainEntityDto>();

            It should_have_expected_EGRPOU = () => ((UkraineLegalPersonDomainEntityDto)Result).Egrpou.Should().Be(TestEgrpou);
            It should_have_expected_TaxationType = () => ((UkraineLegalPersonDomainEntityDto)Result).TaxationType.Should().Be(TestTaxationType);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonDtoService))]
        class When_requested_new_entity : UkraineGetLegalPersonDtoServiceContext
        {
            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
            };

            It should_be_UkraineLegalPersonDomainEntityDto = () => Result.Should().BeOfType<UkraineLegalPersonDomainEntityDto>();
        }
    }
}
