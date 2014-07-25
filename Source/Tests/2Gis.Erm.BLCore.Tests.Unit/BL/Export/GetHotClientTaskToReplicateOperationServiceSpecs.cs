using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.BLCore.Operations.Concrete.HotClients;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Export
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    public static class GetHotClientTaskToReplicateOperationServiceSpecs
    {
        private const long OrganizationUnitId = 2;
        private const long ClientOwnerId = 3;
        private const long FirmOwnerId = 4;
        private const long OrganizationUnitDirectorId = 5;
        private const long ProjectDirectorId = 6;
        private const long TelemarketingFranchiseeManagerId = 7;
        private const long HotClientRequestEntityId = 8;
        private const long ProjectOrganizationUnitId = 9;
        private const long TelemarketingBranchManagerId = 10;

        public abstract class BasicContext
        {
            protected static GetHotClientTaskToReplicateOperationService Service;
            protected static IMsCrmSettings AppSettings;
            protected static IBranchOfficeReadModel BranchOfficeReadModel;
            protected static IFirmReadModel FirmReadModel;
            protected static IUserReadModel UserReadModel;
            protected static DataStore Data;
            protected static HotClientTaskDto TaskDto;
            protected static Exception Exception;
            protected static User TaskExecutor;

            Establish context = () =>
                {
                    Data = new DataStore();
                    UserReadModel = SetupUserReadModel();
                    BranchOfficeReadModel = SetupBranchOfficeReadModel();
                    FirmReadModel = SetupFirmReadModel();

                    Service = new GetHotClientTaskToReplicateOperationService(UserReadModel, FirmReadModel, BranchOfficeReadModel);
                };

            Because of = () =>
                {
                    try
                    {
                        TaskDto = Service.GetHotClientTask(HotClientRequestEntityId);
                        Exception = null;
                    }
                    catch (Exception e)
                    {
                        Exception = e;
                    }
                };



            protected static IUserReadModel SetupUserReadModel()
            {
                var userReadModel = Mock.Of<IUserReadModel>();

                Mock.Get(userReadModel)
                    .Setup(x => x.GetUser(Moq.It.Is<long>(id => id == ClientOwnerId)))
                    .Returns<long>(l => new User { Id = l, Account = string.Format("user_client_{0}", l), IsServiceUser = Data.IsOwnerServiceUser });

                Mock.Get(userReadModel)
                    .Setup(x => x.GetUser(Moq.It.Is<long>(id => id == FirmOwnerId)))
                    .Returns<long>(l => new User { Id = l, Account = string.Format("user_firm_{0}", l), IsServiceUser = Data.IsOwnerServiceUser }); 
                
                Mock.Get(userReadModel)
                    .Setup(x => x.FindAnyUserWithPrivelege(Moq.It.Is<IEnumerable<long>>(longs => longs.Contains(OrganizationUnitId)), FunctionalPrivilegeName.HotClientProcessing))
                    .Returns(new User { Id = OrganizationUnitDirectorId, Account = string.Format("user_director_{0}", OrganizationUnitDirectorId) });

                Mock.Get(userReadModel)
                    .Setup(x => x.FindAnyUserWithPrivelege(Moq.It.Is<IEnumerable<long>>(longs => longs.Contains(ProjectOrganizationUnitId)), FunctionalPrivilegeName.HotClientProcessing))
                    .Returns(new User { Id = ProjectDirectorId, Account = string.Format("user_director_{0}", ProjectDirectorId) }); 
                
                Mock.Get(userReadModel)
                    .Setup(x => x.FindAnyUserWithPrivelege(Moq.It.IsAny<IEnumerable<long>>(), FunctionalPrivilegeName.HotClientTelemarketingProcessingFranchisee))
                    .Returns(new User { Id = TelemarketingFranchiseeManagerId, Account = string.Format("user_telemarketing_franchisee_{0}", TelemarketingFranchiseeManagerId) });

                Mock.Get(userReadModel)
                    .Setup(x => x.FindAnyUserWithPrivelege(Moq.It.IsAny<IEnumerable<long>>(), FunctionalPrivilegeName.HotClientTelemarketingProcessingBranch))
                    .Returns(new User { Id = TelemarketingBranchManagerId, Account = string.Format("user_telemarketing_branch_{0}", TelemarketingBranchManagerId) });
                return userReadModel;
            }

            protected static IBranchOfficeReadModel SetupBranchOfficeReadModel()
            {
                var firmRepository = Mock.Of<IBranchOfficeReadModel>();

                Mock.Get(firmRepository)
                    .Setup(repository => repository.GetOrganizationUnitContributionType(Moq.It.IsAny<long>()))
                    .Returns(() => Data.ContributionType);

                Mock.Get(firmRepository)
                    .Setup(repository => repository.GetProjectOrganizationUnitIds(Moq.It.IsAny<long>()))
                    .Returns(() => new[] { ProjectOrganizationUnitId });

                return firmRepository;
            }

            protected static IFirmReadModel SetupFirmReadModel()
            {
                var firmRepository = Mock.Of<IFirmReadModel>();

                Mock.Get(firmRepository)
                    .Setup(repository => repository.IsTelesaleFirmAddress(Moq.It.IsAny<long>()))
                    .Returns(() => Data.IsTelesale);

                FirmAndClientDto dto = Data.FirmAndClientDto;
                Mock.Get(firmRepository)
                    .Setup(repository => repository.TryGetFirmAndClientByFirmAddress(Moq.It.IsAny<long>(), out dto))
                    .Returns(true);

                Mock.Get(firmRepository)
                    .Setup(repository => repository.GetHotClientRequest(HotClientRequestEntityId))
                    .Returns(() => Data.HotClientRequestEntity);

                return firmRepository;
            }

            protected sealed class DataStore
            {
                public FirmAndClientDto FirmAndClientDto = new FirmAndClientDto();
                public HotClientRequest HotClientRequestEntity;
                public User ErmUser;
                public bool IsOwnerServiceUser;
                public bool IsTelesale;
                public ContributionTypeEnum ContributionType = ContributionTypeEnum.Branch;
                
                public void SetupHotClientRequestInvalidEntity()
                {
                    HotClientRequestEntity = new HotClientRequest
                    {
                        Id = HotClientRequestEntityId,
                        ContactName = "ContactName",
                        ContactPhone = "ContactPhone",
                        CreationDate = DateTime.Now,
                        Description = "Description",
                        CardCode = null,
                        BranchCode = null,
                    };
                }

                public void SetupHotClientRequestForFirmAddressEntity()
                {
                    HotClientRequestEntity = new HotClientRequest
                    {
                        Id = HotClientRequestEntityId,
                        ContactName = "ContactName",
                        ContactPhone = "ContactPhone",
                        CreationDate = DateTime.Now,
                        Description = "Description",
                        CardCode = 1,
                        BranchCode = null,
                    };
                }

                public void SetupHotClientRequestForProjectEntity()
                {
                    HotClientRequestEntity = new HotClientRequest
                    {
                        Id = HotClientRequestEntityId,
                        ContactName = "ContactName",
                        ContactPhone = "ContactPhone",
                        CreationDate = DateTime.Now,
                        Description = "Description",
                        CardCode = null,
                        BranchCode = 1,
                    };
                }

                public void SetupFirmWithClient()
                {
                    FirmAndClientDto.Client = new Platform.Model.Entities.Erm.Client { ReplicationCode = Guid.NewGuid(), OwnerCode = ClientOwnerId };
                    FirmAndClientDto.Firm = new Firm { ReplicationCode = Guid.NewGuid(), OwnerCode = FirmOwnerId, OrganizationUnitId = OrganizationUnitId };
                }

                public void SetupFirmWithoutClient()
                {
                    FirmAndClientDto.Client = null;
                    FirmAndClientDto.Firm = new Firm { ReplicationCode = Guid.NewGuid(), OwnerCode = FirmOwnerId, OrganizationUnitId = OrganizationUnitId };
                }

                public void SetupFirmWithoutClientAndFirm()
                {
                    FirmAndClientDto = null;
                }
            }
        }

        [Behaviors]
        public class FailedWithExceptionBehavior : BasicContext
        {
            It should_throw_exception = () => Exception.Should().NotBeNull();
        }
        
        public class InvalidHotClientEntity : BasicContext
        {
            Establish context = () =>
                {
                    Data.SetupHotClientRequestInvalidEntity();
                    Data.IsTelesale = true;
                    Data.SetupFirmWithClient();
                };

            Behaves_like<FailedWithExceptionBehavior> throws_exception;
        }

        [Behaviors]
        public class SucceededBehavior : BasicContext
        {
            private It should_return_success = () =>
            {
                TaskDto.Should().NotBeNull();
            };

            It should_throw_no_exceptions = () => Exception.Should().BeNull();
        }


        #region Fail stories
        public class WhenHotClientRequestContainsNeitherBranchNorCardCode : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestInvalidEntity(); // проверяемое условие - объект заявки не имеет ни идентификатра адреса, ни проекта
                Data.SetupFirmWithClient();
                Data.IsTelesale = false;
                Data.IsOwnerServiceUser = false;
            };

            Behaves_like<FailedWithExceptionBehavior> creation_must_fail_with_exception;
        }
        #endregion

        #region Success stories
        #region Тесты на определение исполнителя задачи
        public class WhenFirmHasTelemarketingCategoryGroup : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.IsTelesale = true; // проверяемое условие - фирма идёт с самыми дешевыми рубриками
                Data.IsOwnerServiceUser = false;
            };

            Behaves_like<SucceededBehavior> creation_must_succeed;

            It should_telemarketing_become_task_executor = () =>
            {
                TaskDto.TaskOwner.Id.Should().Be(TelemarketingBranchManagerId);
            };
        }

        public class WhenTelemarketingFirmBelongsToBranch : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.IsTelesale = true;
                Data.IsOwnerServiceUser = false;

                // проверяемое условие - отделение организации - бранч
                Data.ContributionType = ContributionTypeEnum.Branch;
            };

            Behaves_like<SucceededBehavior> creation_must_succeed;

            It should_search_for_branch_user = () =>
            {
                TaskDto.TaskOwner.Id.Should().Be(TelemarketingBranchManagerId);
            };
        }

        public class WhenTelemarketingFirmBelongsToFranchisee : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.IsTelesale = true;
                Data.IsOwnerServiceUser = false;

                // проверяемое условие - отделение организации - франчайзи
                Data.ContributionType = ContributionTypeEnum.Franchisees;
            };

            Behaves_like<SucceededBehavior> creation_must_succeed;

            It should_search_for_franchisee_user = () =>
            {
                TaskDto.TaskOwner.Id.Should().Be(TelemarketingFranchiseeManagerId);
            };
        }

        public class WhenTelemarketingFirmWithoutTelemarketingManager : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.IsTelesale = true;
                Data.IsOwnerServiceUser = false;

                Mock.Get(UserReadModel)
                    .Setup(x => x.FindAnyUserWithPrivelege(Moq.It.IsAny<IEnumerable<long>>(), FunctionalPrivilegeName.HotClientTelemarketingProcessingFranchisee))
                    .Returns((User)null);

                Mock.Get(UserReadModel)
                    .Setup(x => x.FindAnyUserWithPrivelege(Moq.It.IsAny<IEnumerable<long>>(), FunctionalPrivilegeName.HotClientTelemarketingProcessingBranch))
                    .Returns((User)null);
            };

            Behaves_like<SucceededBehavior> creation_must_succeed;

            It should_search_for_organization_unit_director = () =>
            {
                TaskDto.TaskOwner.Id.Should().Be(OrganizationUnitDirectorId);
            };
        }

        public class WhenFirmIsNotTelemarketing : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.IsTelesale = false;
                Data.IsOwnerServiceUser = false;
            };

            Behaves_like<SucceededBehavior> creation_must_succeed;

            It should_client_owner_become_task_executor = () =>
            {
                TaskDto.TaskOwner.Id.Should().Be(ClientOwnerId);
            };
        }

        public class WhenClientDoesNotExists : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithoutClient();  // проверяемое условие - фирма без клиента
                Data.IsTelesale = false;
                Data.IsOwnerServiceUser = true; // если клиента нет, то куратор фирмы - резерв
            };

            Behaves_like<SucceededBehavior> creation_must_succeed;

            It should_organization_unit_director_become_task_executor = () =>
                {
                    TaskDto.TaskOwner.Id.Should().Be(OrganizationUnitDirectorId);
                };
        }

        public class WhenBranchCodeSpecifiedInsteadOfCardCode : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForProjectEntity(); // проверяемое условие - указан BranchCode
                Data.SetupFirmWithoutClientAndFirm();
                Data.IsTelesale = false;
                Data.IsOwnerServiceUser = true;
            };

            Behaves_like<SucceededBehavior> creation_must_succeed;

            It should_organization_unit_director_become_task_executor = () =>
            {
                TaskDto.TaskOwner.Id.Should().Be(ProjectDirectorId);
            };
        } 
        #endregion
        #endregion
    }
}
