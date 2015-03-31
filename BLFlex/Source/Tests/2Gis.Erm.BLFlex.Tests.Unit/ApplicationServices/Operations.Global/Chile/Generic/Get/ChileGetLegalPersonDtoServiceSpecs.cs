using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Security.API.UserContext;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonDtoServiceSpecs
    {
        public abstract class ChileGetLegalPersonDtoServiceContext
        {
            protected static ChileGetLegalPersonDtoService ChileGetLegalPersonDtoService;
            protected static IUserContext UserContext;
            protected static IClientReadModel ClientReadModel;
            protected static ILegalPersonReadModel LegalPersonReadModel;
            protected static IChileLegalPersonReadModel ChileLegalPersonReadModel;
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
                    ChileLegalPersonReadModel = Mock.Of<IChileLegalPersonReadModel>();
                    UserContext = Mock.Of<IUserContext>(x => x.Identity == new NullUserIdentity());

                    ChileGetLegalPersonDtoService = new ChileGetLegalPersonDtoService(ClientReadModel, LegalPersonReadModel, ChileLegalPersonReadModel, UserContext, null);
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
            protected static LegalPerson LegalPerson;
            protected static ChileLegalPersonPart ChileLegalPersonPart;

            const string TestOperationsKind = "TestOperationsKind";
            static readonly EntityReference TestCommune = new EntityReference(4, "TestCommune");

            Establish context = () =>
                {
                    EntityId = 1; // C Id != 0 мы получим Dto из хранилища

                    ChileLegalPersonPart = new ChileLegalPersonPart { OperationsKind = TestOperationsKind };
                    LegalPerson = new LegalPerson { Id = EntityId, Parts = new[] { ChileLegalPersonPart }, Timestamp = new byte[0] }; // Timestamp != null показывает, что LegalPerson не новая сущность

                    Mock.Get(LegalPersonReadModel).Setup(x => x.GetLegalPerson(EntityId)).Returns(LegalPerson);
                    Mock.Get(ChileLegalPersonReadModel).Setup(x => x.GetCommuneReference(EntityId)).Returns(TestCommune);
                };

            It should_be_ChileLegalPersonDomainEntityDto = () => Result.Should().BeOfType<ChileLegalPersonDomainEntityDto>();

            It should_have_expected_Commune = () => ((ChileLegalPersonDomainEntityDto)Result).CommuneRef.Should().Be(TestCommune);
            It should_have_expected_OperationsKind = () => ((ChileLegalPersonDomainEntityDto)Result).OperationsKind.Should().Be(TestOperationsKind);
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(ChileGetLegalPersonDtoService))]
        class When_requested_new_entity : ChileGetLegalPersonDtoServiceContext
        {
            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
            };

            It should_be_ChileLegalPersonDomainEntityDto = () => Result.Should().BeOfType<ChileLegalPersonDomainEntityDto>();
        }
    }
}
