using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonDtoServiceSpecs
    {
        public abstract class ChileGetLegalPersonDtoServiceContext
        {
            protected static ChileGetLegalPersonDtoService ChileGetLegalPersonDtoService;
            protected static IUserContext UserContext;
            protected static ILegalPersonReadModel ReadModel;
            protected static IChileLegalPersonReadModel ChileReadModel;
            protected static ISecureFinder SecureFinder;
            protected static IDomainEntityDto Result;

            protected static long EntityId;
            protected static bool ReadOnly;
            protected static long? ParentEntityId;
            protected static EntityName ParentEntityName;
            protected static string ExtendedInfo;

            Establish context = () =>
                {
                    ChileReadModel = Mock.Of<IChileLegalPersonReadModel>();
                    ReadModel = Mock.Of<ILegalPersonReadModel>();
                    UserContext = Mock.Of<IUserContext>();
                    SecureFinder = Mock.Of<ISecureFinder>();

                    ChileGetLegalPersonDtoService = new ChileGetLegalPersonDtoService(SecureFinder, ReadModel, ChileReadModel, UserContext);
                };

            Because of = () =>
                {
                    Result = ChileGetLegalPersonDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonDtoService))]
        class When_requested_existing_entity : ChileGetLegalPersonDtoServiceContext
        {
            protected static ChileLegalPersonDomainEntityDto Dto;
            protected static LegalPerson LegalPerson;
            protected static ChileLegalPersonPart ChileLegalPersonPart;

            static readonly EntityReference TestCommune = new EntityReference(4, "TestCommune");
            const string TestOperationsKind = "TestOperationsKind";
            const long entityId = 1;

            Establish context = () =>
                {
                    EntityId = entityId; // C Id != 0 мы получим Dto из хранилища
                    Dto = new ChileLegalPersonDomainEntityDto();

                    ChileLegalPersonPart = new ChileLegalPersonPart { OperationsKind = TestOperationsKind };
                    LegalPerson = new LegalPerson { Parts = new[] { ChileLegalPersonPart } };

                    Mock.Get(ReadModel).Setup(x => x.GetLegalPersonDto<ChileLegalPersonDomainEntityDto>(entityId)).Returns(Dto);
                    Mock.Get(ReadModel).Setup(x => x.GetLegalPerson(EntityId)).Returns(LegalPerson);
                    Mock.Get(ChileReadModel).Setup(x => x.GetCommuneReference(EntityId)).Returns(TestCommune);
                };

            It should_be_ChileLegalPersonDomainEntityDto = () => Result.Should().BeOfType<ChileLegalPersonDomainEntityDto>();

            It should_have_expected_Commune = () => ((ChileLegalPersonDomainEntityDto)Result).CommuneRef.Should().Be(TestCommune);
            It should_have_expected_TaxationType = () => ((ChileLegalPersonDomainEntityDto)Result).OperationsKind.Should().Be(TestOperationsKind);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonDtoService))]
        class When_requested_new_entity : ChileGetLegalPersonDtoServiceContext
        {
            static IUserIdentity _userIdentity;
            const long userCode = 2;

            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
                _userIdentity = Mock.Of<IUserIdentity>(x => x.Code == userCode);
                Mock.Get(UserContext).Setup(x => x.Identity).Returns(_userIdentity);
            };

            It should_be_ChileLegalPersonDomainEntityDto = () => Result.Should().BeOfType<ChileLegalPersonDomainEntityDto>();
        }
    }
}
