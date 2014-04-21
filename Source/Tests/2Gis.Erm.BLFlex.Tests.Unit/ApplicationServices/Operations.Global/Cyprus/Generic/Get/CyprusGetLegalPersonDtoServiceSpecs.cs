using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Cyprus.Generic.Get
{
    public class CyprusGetLegalPersonDtoServiceSpecs
    {
        public abstract class CyprusGetLegalPersonDtoServiceContext
        {
            protected static CyprusGetLegalPersonDtoService CyprusGetLegalPersonDtoService;
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

                    CyprusGetLegalPersonDtoService = new CyprusGetLegalPersonDtoService(UserContext, SecureFinder, ReadModel);
                };

            Because of = () =>
                {
                    Result = CyprusGetLegalPersonDtoService.GetDomainEntityDto(EntityId, ReadOnly, ParentEntityId, ParentEntityName, ExtendedInfo);
                };
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(CyprusGetLegalPersonDtoService))]
        class When_requested_existing_entity : CyprusGetLegalPersonDtoServiceContext
        {
            protected static LegalPersonDomainEntityDto Dto;
            protected static LegalPerson LegalPerson;

            const long entityId = 1;

            Establish context = () =>
                {
                    EntityId = entityId; // C Id != 0 мы получим Dto из хранилища
                    Dto = new LegalPersonDomainEntityDto();

                    LegalPerson = new LegalPerson();

                    Mock.Get(ReadModel).Setup(x => x.GetLegalPersonDto<LegalPersonDomainEntityDto>(entityId)).Returns(Dto);
                    Mock.Get(ReadModel).Setup(x => x.GetLegalPerson(EntityId)).Returns(LegalPerson);
                };

            It should_be_LegalPersonDomainEntityDto = () => Result.Should().BeOfType<LegalPersonDomainEntityDto>();
        }

        [Tags("GetDtoServie")]
        [Subject(typeof(CyprusGetLegalPersonDtoService))]
        class When_requested_new_entity : CyprusGetLegalPersonDtoServiceContext
        {
            static IUserIdentity _userIdentity;
            const long userCode = 2;

            Establish context = () =>
            {
                EntityId = 0; // C Id = 0 мы создадим новую Dto
                _userIdentity = Mock.Of<IUserIdentity>(x => x.Code == userCode);
                Mock.Get(UserContext).Setup(x => x.Identity).Returns(_userIdentity);
            };

            It should_be_LegalPersonDomainEntityDto = () => Result.Should().BeOfType<LegalPersonDomainEntityDto>();
        }
    }
}
