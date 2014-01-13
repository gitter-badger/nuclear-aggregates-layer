﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.HotClient;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.MsCrm;
using DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests;
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
    // TODO {d.ivanov, 25.11.2013}: должен лечь в 2Gis.Erm.BLCore.Tests.Unit\BL\Export\CreateHotClientHandlerSpecs.cs
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    public static class CreateHotClientHandlerSpecs
    {
        private const long TelesaleCategoryGroupId = 1;
        private const long OrganizationUnitId = 2;
        private const long ClientOwnerId = 3;
        private const long FirmOwnerId = 4;
        private const long OrganizationUnitDirectorId = 5;
        private const long ProjectDirectorId = 6;
        private const long TelemarketingManagerId = 7;
        private const long HotClientRequestEntityId = 8;
        private const long ProjectOrganizationUnitId = 9;

        public abstract class BasicContext
        {
            protected static CreateHotClientHandler Handler;
            protected static IMsCrmSettings AppSettings;
            protected static IFirmRepository FirmRepository;
            protected static IHotClientRequestService HotClientRequestService;
            protected static ICrmTaskFactory CrmTaskFactory;
            protected static IUserRepository UserRepository;
            protected static DataStore Data;

            protected static CreateHotClientResponse Response;
            protected static Exception Exception;
            protected static User TaskExecutor;

            Establish context = () =>
                {
                    Data = new DataStore();
                    AppSettings = SetupAppSettings();
                    UserRepository = SetupUserRepository();
                    CrmTaskFactory = SetupCrmTaskFactory();
                    HotClientRequestService = SetupHotClientRequestService();
                    FirmRepository = SetupFirmRepository();
                    
                    Handler = new CreateHotClientHandler(AppSettings, FirmRepository, HotClientRequestService, CrmTaskFactory, UserRepository);
                };

            Because of = () =>
                {
                    try
                    {
                        Response = (CreateHotClientResponse)Handler.Handle(new CreateHotClientRequest { Id = HotClientRequestEntityId });
                        Exception = null;
                    }
                    catch (Exception e)
                    {
                        Exception = e;
                    }
                };

            protected static IUserRepository SetupUserRepository()
            {
                var userRepository = Mock.Of<FakeUserRepository>();

                Mock.Get(userRepository)
                    .Setup(service => service.GetUser(Moq.It.Is<long>(id => id == ClientOwnerId)))
                    .Returns<long>(l => new User { Id = l, Account = string.Format("user_client_{0}", l), IsServiceUser = Data.IsOwnerServiceUser });

                Mock.Get(userRepository)
                    .Setup(service => service.GetUser(Moq.It.Is<long>(id => id == FirmOwnerId)))
                    .Returns<long>(l => new User { Id = l, Account = string.Format("user_firm_{0}", l), IsServiceUser = Data.IsOwnerServiceUser }); 
                
                Mock.Get(userRepository)
                    .Setup(service => service.FindAnyUserWithPrivelege(Moq.It.Is<IEnumerable<long>>(longs => longs.Contains(OrganizationUnitId)), FunctionalPrivilegeName.HotClientProcessing))
                    .Returns(new User { Id = OrganizationUnitDirectorId, Account = string.Format("user_director_{0}", OrganizationUnitDirectorId) });

                Mock.Get(userRepository)
                    .Setup(service => service.FindAnyUserWithPrivelege(Moq.It.Is<IEnumerable<long>>(longs => longs.Contains(ProjectOrganizationUnitId)), FunctionalPrivilegeName.HotClientProcessing))
                    .Returns(new User { Id = ProjectDirectorId, Account = string.Format("user_director_{0}", ProjectDirectorId) }); 
                
                Mock.Get(userRepository)
                    .Setup(service => service.FindAnyUserWithPrivelege(Moq.It.IsAny<IEnumerable<long>>(), FunctionalPrivilegeName.HotClientTelemarketingProcessing))
                    .Returns(new User { Id = TelemarketingManagerId, Account = string.Format("user_telemarketing_{0}", TelemarketingManagerId) });

                return userRepository;
            }

            protected static ICrmTaskFactory SetupCrmTaskFactory()
            {
                var crmTaskFactory = Mock.Of<ICrmTaskFactory>();

                Mock.Get(crmTaskFactory)
                    .Setup(service => service.CreateTask(Moq.It.IsAny<UserDto>(), Moq.It.IsAny<HotClientRequestDto>(), Moq.It.IsAny<RegardingObject>()))
                    .Returns<UserDto, HotClientRequestDto, RegardingObject>((crmUser, hotClient, regarding) => Guid.NewGuid());

                return crmTaskFactory;
            }

            protected static IHotClientRequestService SetupHotClientRequestService()
            {
                var hotClientRequestService = Mock.Of<IHotClientRequestService>();

                Mock.Get(hotClientRequestService)
                    .Setup(repository => repository.GetHotClientRequest(Moq.It.IsAny<long>()))
                    .Returns(() => Data.HotClientRequestEntity);

                return hotClientRequestService;
            }

            protected static IFirmRepository SetupFirmRepository()
            {
                var firmRepository = Mock.Of<IFirmRepository>();

                Mock.Get(firmRepository)
                    .Setup(repository => repository.GetFirmAddressOrganizationUnitId(Moq.It.IsAny<long>()))
                    .Returns(() => OrganizationUnitId);

                Mock.Get(firmRepository)
                    .Setup(repository => repository.GetProjectOrganizationUnitIds(Moq.It.IsAny<long>()))
                    .Returns(() => new[] { ProjectOrganizationUnitId });

                Mock.Get(firmRepository)
                    .Setup(repository => repository.GetFirmAddressCategoryGroups(Moq.It.IsAny<long>()))
                    .Returns(() => Data.CategoryGroups);

                Mock.Get(firmRepository)
                    .Setup(repository => repository.GetFirmAndClientByFirmAddress(Moq.It.IsAny<long>()))
                    .Returns(() => Data.FirmAndClientDto);

                return firmRepository;
            }

            protected static IMsCrmSettings SetupAppSettings()
            {
                var appSettings = Mock.Of<IMsCrmSettings>();

                Mock.Get(appSettings)
                    .Setup(x => x.EnableReplication)
                    .Returns(() => Data.EnableReplication);

                return appSettings;
            }

            protected sealed class DataStore
            {
                public IEnumerable<CategoryGroup> CategoryGroups;
                public FirmAndClientDto FirmAndClientDto;
                public HotClientRequest HotClientRequestEntity;
                public User ErmUser;
                public bool EnableReplication;
                public bool IsOwnerServiceUser;
                
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

                public void SetupCategoryGroupsTelesale()
                {
                    CategoryGroups = new[]
                    {
                        new CategoryGroup { Id = TelesaleCategoryGroupId, GroupRate = 0.5m },
                    };
                }

                public void SetupCategoryGroupsNotTelesale()
                {
                    CategoryGroups = new[]
                    {
                        new CategoryGroup { Id = TelesaleCategoryGroupId, GroupRate = 0.5m },
                        new CategoryGroup { Id = TelesaleCategoryGroupId + 1, GroupRate = 1m },
                    };
                }

                public void SetupCategoryGroupsEmpty()
                {
                    CategoryGroups = new CategoryGroup[0];
                }

                public void SetupFirmWithClient()
                {
                    FirmAndClientDto = new FirmAndClientDto
                    {
                        Client = new Platform.Model.Entities.Erm.Client { ReplicationCode = Guid.NewGuid(), OwnerCode = ClientOwnerId },
                        Firm = new Firm { ReplicationCode = Guid.NewGuid(), OwnerCode = FirmOwnerId, OrganizationUnitId = OrganizationUnitId },
                    };
                }

                public void SetupFirmWithoutClient()
                {
                    FirmAndClientDto = new FirmAndClientDto
                    {
                        Client = null,
                        Firm = new Firm { ReplicationCode = Guid.NewGuid(), OwnerCode = FirmOwnerId, OrganizationUnitId = OrganizationUnitId },
                    };
                }

                public void SetupFirmWithoutClientAndFirm()
                {
                    FirmAndClientDto = null;
                }

                public void SetupReplicationOn()
                {
                    EnableReplication = true;
                }

                public void SetupReplicationOff()
                {
                    EnableReplication = false;
                }
            }
        }

        [Behaviors]
        public class ReplicationSucceededBehavior : BasicContext
        {
            private It should_return_success = () =>
                {
                    Response.Should().NotBeNull();
                    Response.Success.Should().BeTrue();
                };

            It should_throw_no_exceptions = () => Exception.Should().BeNull();

            private It sould_create_one_and_only_one_task = () =>
                {
                    Mock.Get(CrmTaskFactory)
                        .Verify(factory => factory.CreateTask(Moq.It.IsAny<UserDto>(), Moq.It.IsAny<HotClientRequestDto>(), Moq.It.IsAny<RegardingObject>()),
                                Times.Once);
                };
        }

        [Behaviors]
        public class ReplicationFailedBehavior : BasicContext
        {
            It should_return_success = () =>
            {
                Response.Should().NotBeNull();
                Response.Success.Should().BeFalse();
            };

            It should_throw_no_exceptions = () => Exception.Should().BeNull();
        }

        [Behaviors]
        public class ReplicationFailedWithExceptionBehavior : BasicContext
        {
            It should_throw_exception = () => Exception.Should().NotBeNull();
        }
        
        public class InvalidHotClientEntity : BasicContext
        {
            Establish context = () =>
                {
                    Data.SetupHotClientRequestInvalidEntity();
                    Data.SetupCategoryGroupsTelesale();
                    Data.SetupFirmWithClient();
                    Data.SetupReplicationOn();
                };

            Behaves_like<ReplicationFailedWithExceptionBehavior> throws_exception;
        }

        #region Fail stories
        public class WhenHotClientRequestContainsNeitherBranchNorCardCode : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestInvalidEntity(); // проверяемое условие - объект заявки не имеет ни идентификатра адреса, ни проекта
                Data.SetupFirmWithClient();
                Data.SetupCategoryGroupsEmpty(); 
                Data.SetupReplicationOn();
                Data.IsOwnerServiceUser = false;
            };

            Behaves_like<ReplicationFailedWithExceptionBehavior> replication_must_fail_with_exception;
        }
        #endregion

        #region Success stories
        #region Тесты на определение исполнителя задачи
        public class WhenFirmHasNoCategyGroup : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.SetupCategoryGroupsEmpty(); // проверяемое условие - нет групп рубрик
                Data.SetupReplicationOn();
                Data.IsOwnerServiceUser = false;
            };

            Behaves_like<ReplicationSucceededBehavior> replication_must_secceed;

            It should_client_owner_become_task_executor = () =>
            {
                Mock.Get(CrmTaskFactory)
                    .Verify(factory => factory.CreateTask(Moq.It.Is<UserDto>(dto => dto.Id == ClientOwnerId),
                                                          Moq.It.IsAny<HotClientRequestDto>(),
                                                          Moq.It.IsAny<RegardingObject>()));
            };
        }

        public class WhenFirmHasTelemarketingCategoryGroup : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.SetupCategoryGroupsTelesale(); // проверяемое условие - фирма идёт с самыми дешевыми рубриками
                Data.SetupReplicationOn();
                Data.IsOwnerServiceUser = false;
            };

            Behaves_like<ReplicationSucceededBehavior> replication_must_secceed;

            It should_telemarketing_become_task_executor = () =>
            {
                Mock.Get(CrmTaskFactory)
                    .Verify(factory => factory.CreateTask(Moq.It.Is<UserDto>(dto => dto.Id == TelemarketingManagerId),
                                                          Moq.It.IsAny<HotClientRequestDto>(),
                                                          Moq.It.IsAny<RegardingObject>()));
            };
        }

        public class WhenFirmHasSomeDifferentCategoryGroups : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.SetupCategoryGroupsNotTelesale(); // проверяемое условие - фирма c несколькими рубриками, в том числе, самой дешёвой
                Data.SetupReplicationOn();
                Data.IsOwnerServiceUser = false;
            };

            Behaves_like<ReplicationSucceededBehavior> replication_must_secceed;

            It should_client_owner_become_task_executor = () =>
            {
                Mock.Get(CrmTaskFactory)
                    .Verify(factory => factory.CreateTask(Moq.It.Is<UserDto>(dto => dto.Id == ClientOwnerId),
                                                          Moq.It.IsAny<HotClientRequestDto>(),
                                                          Moq.It.IsAny<RegardingObject>()));
            };
        }

        public class WhenClientDoesNotExists : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithoutClient();  // проверяемое условие - фирма без клиента
                Data.SetupCategoryGroupsNotTelesale();
                Data.SetupReplicationOn();
                Data.IsOwnerServiceUser = true; // если клиента нет, то куратор фирмы - резерв
            };

            Behaves_like<ReplicationSucceededBehavior> replication_must_secceed;

            It should_organization_unit_director_become_task_executor = () =>
                {
                    Mock.Get(CrmTaskFactory)
                        .Verify(factory => factory.CreateTask(Moq.It.Is<UserDto>(dto => dto.Id == OrganizationUnitDirectorId),
                                                              Moq.It.IsAny<HotClientRequestDto>(),
                                                              Moq.It.IsAny<RegardingObject>()));
                };
        }

        public class WhenBranchCodeSpecifiedInsteadOfCardCode : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForProjectEntity(); // проверяемое условие - указан BranchCode
                Data.SetupFirmWithoutClientAndFirm();
                Data.SetupCategoryGroupsNotTelesale();
                Data.SetupReplicationOn();
                Data.IsOwnerServiceUser = true;
            };

            Behaves_like<ReplicationSucceededBehavior> replication_must_secceed;

            It should_organization_unit_director_become_task_executor = () =>
            {
                Mock.Get(CrmTaskFactory)
                    .Verify(factory => factory.CreateTask(Moq.It.Is<UserDto>(dto => dto.Id == ProjectDirectorId),
                                                          Moq.It.IsAny<HotClientRequestDto>(),
                                                          Moq.It.IsAny<RegardingObject>()));
            };
        } 
        #endregion
        #endregion

        #region Fixed problems
        public class WhenFirmHasTelemarketingAndNullCategoryGroup : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.SetupReplicationOn();
                Data.IsOwnerServiceUser = false;

                Data.CategoryGroups = new[] // проверяемое условие - фирма идёт с самыми дешевыми рубриками, плюс несколько рубрик не имеют явно указанной группы
                    {
                        new CategoryGroup { Id = TelesaleCategoryGroupId, GroupRate = 0.5m },
                        null,
                    };
            };

            Behaves_like<ReplicationSucceededBehavior> replication_must_secceed;

            It should_not_telemarketing_become_task_executor = () =>
            {
                Mock.Get(CrmTaskFactory)
                    .Verify(factory => factory.CreateTask(Moq.It.Is<UserDto>(dto => dto.Id == TelemarketingManagerId),
                                                          Moq.It.IsAny<HotClientRequestDto>(),
                                                          Moq.It.IsAny<RegardingObject>()), Times.Never);
            };
        }

        public class WhenFirmHasNullCategoryGroup : BasicContext
        {
            Establish context = () =>
            {
                Data.SetupHotClientRequestForFirmAddressEntity();
                Data.SetupFirmWithClient();
                Data.SetupReplicationOn();
                Data.IsOwnerServiceUser = false;

                Data.CategoryGroups = new CategoryGroup[] // проверяемое условие - рубрики фирмы не имеют явно указанной группы
                    {
                        null,
                    };
            };

            Behaves_like<ReplicationSucceededBehavior> replication_must_secceed;

            It should_not_telemarketing_become_task_executor = () =>
            {
                Mock.Get(CrmTaskFactory)
                    .Verify(factory => factory.CreateTask(Moq.It.Is<UserDto>(dto => dto.Id == TelemarketingManagerId),
                                                          Moq.It.IsAny<HotClientRequestDto>(),
                                                          Moq.It.IsAny<RegardingObject>()), Times.Never);
            };
        }
        #endregion
    }
}
