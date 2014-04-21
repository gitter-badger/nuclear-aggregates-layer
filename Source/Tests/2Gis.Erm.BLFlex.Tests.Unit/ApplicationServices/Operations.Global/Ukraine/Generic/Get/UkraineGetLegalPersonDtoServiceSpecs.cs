using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonDtoServiceSpecs
    {
        public abstract class UkraineGetLegalPersonDtoServiceContext
        {
            protected static UkraineGetLegalPersonDtoService UkraineGetLegalPersonDtoService;
            protected static IUserContext UserContext;
            protected static ILegalPersonReadModel ReadModel;
            protected static ISecureFinder SecureFinder;
            protected static IDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static EntityName ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    ReadModel = Mock.Of<ILegalPersonReadModel>();
                    UserContext = Mock.Of<IUserContext>();
                    SecureFinder = Mock.Of<ISecureFinder>();

                    UkraineGetLegalPersonDtoService = new UkraineGetLegalPersonDtoService(UserContext, ReadModel, SecureFinder);
                };

            Because of = () =>
                {
                    Result = UkraineGetLegalPersonDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonDtoService))]
        class When_requested_existing_entity : UkraineGetLegalPersonDtoServiceContext
        {
            protected static UkraineLegalPersonDomainEntityDto Dto;
            protected static LegalPerson LegalPerson;
            protected static UkraineLegalPersonPart UkraineLegalPersonPart;

            const string TestEgrpou = "TestEGRPOU";
            const TaxationType TestTaxationType = TaxationType.WithoutVat;
            const long entityId = 1;

            Establish context = () =>
                {
                    EntityId = entityId; // C Id != 0 мы получим Dto из хранилища
                    Dto = new UkraineLegalPersonDomainEntityDto();

                    UkraineLegalPersonPart = new UkraineLegalPersonPart { Egrpou = TestEgrpou, TaxationType = TestTaxationType };
                    LegalPerson = new LegalPerson { Parts = new[] { UkraineLegalPersonPart } };

                    Mock.Get(ReadModel).Setup(x => x.GetLegalPersonDto<UkraineLegalPersonDomainEntityDto>(entityId)).Returns(Dto);
                    Mock.Get(ReadModel).Setup(x => x.GetLegalPerson(EntityId)).Returns(LegalPerson);
                };

            It should_be_UkraineLegalPersonDomainEntityDto = () => Result.Should().BeOfType<UkraineLegalPersonDomainEntityDto>();

            It should_have_expected_EGRPOU = () => ((UkraineLegalPersonDomainEntityDto)Result).Egrpou.Should().Be(TestEgrpou);
            It should_have_expected_TaxationType = () => ((UkraineLegalPersonDomainEntityDto)Result).TaxationType.Should().Be(TestTaxationType);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(UkraineGetLegalPersonDtoService))]
        class When_requested_new_entity : UkraineGetLegalPersonDtoServiceContext
        {
            static IUserIdentity _userIdentity;
            const long userCode = 2;

            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
                _userIdentity = Mock.Of<IUserIdentity>(x => x.Code == userCode);
                Mock.Get(UserContext).Setup(x => x.Identity).Returns(_userIdentity);
            };

            It should_be_UkraineLegalPersonDomainEntityDto = () => Result.Should().BeOfType<UkraineLegalPersonDomainEntityDto>();
        }
    }
}
