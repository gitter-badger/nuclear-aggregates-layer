using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.HotClient;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.MsCrm
{
    // TODO {all, 04.02.2014}: рефакторинг с конвертацией в OperationService, SRP и т.п., при этом учесть фактическое пересечение по используемым выборкам с *OrderProcessing* функционалом - нужно обобщить, в том числе и на уровне ReadModel
    [UseCase(Duration = UseCaseDuration.Long)]
    public class CreateHotClientHandler : RequestHandler<CreateHotClientRequest, CreateHotClientResponse>
    {
        private const long TelesaleCategoryGroupId = 1;
        private const long DefaultCategoryRate = 1;

        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly ICrmTaskFactory _crmTaskFactory;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IBindCrmTaskToHotClientRequestAggregateService _bindCrmTaskToHotClientRequestAggregateService;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IUserReadModel _userReadModel;

        public CreateHotClientHandler(IBranchOfficeReadModel branchOfficeReadModel,
                                      ICrmTaskFactory crmTaskFactory,
                                      IFirmReadModel firmReadModel,
                                      IBindCrmTaskToHotClientRequestAggregateService bindCrmTaskToHotClientRequestAggregateService,
                                      IMsCrmSettings msCrmSettings,
                                      IUserReadModel userReadModel)
        {
            _branchOfficeReadModel = branchOfficeReadModel;
            _crmTaskFactory = crmTaskFactory;
            _firmReadModel = firmReadModel;
            _bindCrmTaskToHotClientRequestAggregateService = bindCrmTaskToHotClientRequestAggregateService;
            _msCrmSettings = msCrmSettings;
            _userReadModel = userReadModel;
        }

        /// <summary>
        /// Cоздаёт задачу для определённого пользователя в MS Dynamics, соответвующую указаному запросу HotClientRequest.
        /// </summary>
        /// <exception cref="BusinessLogicException">
        /// Когда сущность не найдена, не определён исполнитель зазачи или MS Dynamics не
        /// создал для нас задачу
        /// </exception>
        /// <exception cref="ErmCommunicationException">При ошибках, связанных со взаимодействием с MS Dynamics</exception>
        /// <returns>Success = true, если интеграция включена, false иначе.</returns>
        protected override CreateHotClientResponse Handle(CreateHotClientRequest request)
        {
            var hotClientRequest = _firmReadModel.GetHotClientRequest(request.Id);
            if (hotClientRequest == null)
            {
                throw new BusinessLogicException(BLResources.EntityNotFound);
            }

            if (_msCrmSettings.EnableReplication)
            {
                CreateTask(hotClientRequest);
                return new CreateHotClientResponse { Success = true };
            }

            return new CreateHotClientResponse { Success = false };
        }

        private void CreateTask(HotClientRequest requestEntity)
        {
            var dto = CreateTaskDto(requestEntity);
            Guid taskId;

            try
            {
                var taskOwner = ApplyOwnerSearchStrategies(requestEntity.Id, dto.Strategies);
                taskId = _crmTaskFactory.CreateTask(taskOwner, dto.HotClientDto, dto.Regarding);
            }
            catch (WebException ex)
            {
                throw new ErmCommunicationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
            }

            if (taskId != Guid.Empty)
            {
                _bindCrmTaskToHotClientRequestAggregateService.BindWithCrmTask(requestEntity, taskId);
            }
            else
            {
                throw new BusinessLogicException("При создании задачи MS Dynamics вернул пустой идентификатор");
            }
        }

        private UserDto ApplyOwnerSearchStrategies(long hotClientRequestId, IEnumerable<StrategyDto> strategies)
        {
            var crmUser = strategies.Select(strategy => strategy.Execute()).FirstOrDefault(userDto => userDto != null);
            if (crmUser != null)
            {
                return crmUser;
            }

            var message = string.Format("При создании горячего клиента по зацепке с id - {0} не нашли, на кого повесить задачу. Поиск производился: {1}",
                                        hotClientRequestId,
                                        string.Join(", ", strategies.Select(tuple => tuple.Message)));
            throw new BusinessLogicException(string.Format(message));
        }

        private TaskCreationDto CreateTaskDto(HotClientRequest requestEntity)
        {
            var hotClientDto = new HotClientRequestDto
                {
                    Id = requestEntity.Id,
                    ContactName = requestEntity.ContactName,
                    ContactPhone = requestEntity.ContactPhone,
                    CreationDate = requestEntity.CreationDate,
                    Description = requestEntity.Description,
                };

            if (requestEntity.CardCode != null)
            {
                var firmCategoryGroups = _firmReadModel.GetFirmAddressCategoryGroups(requestEntity.CardCode.Value);

                var mostExpensiveCategoryGroup =
                    firmCategoryGroups.OrderByDescending(categoryGroup => categoryGroup != null ? categoryGroup.GroupRate : DefaultCategoryRate)
                                      .FirstOrDefault();

                var isTelesaleTask = mostExpensiveCategoryGroup != null && mostExpensiveCategoryGroup.Id == TelesaleCategoryGroupId;

                var hotClientFirmAndClientInfo = _firmReadModel.GetFirmAndClientByFirmAddress(requestEntity.CardCode.Value);

                if (hotClientFirmAndClientInfo != null && hotClientFirmAndClientInfo.Client != null)
                {
                    return CreateForClient(hotClientDto, hotClientFirmAndClientInfo.Client, hotClientFirmAndClientInfo.Firm.OrganizationUnitId, isTelesaleTask);
                }

                if (hotClientFirmAndClientInfo != null && hotClientFirmAndClientInfo.Firm != null)
                {
                    return CreateForFirm(hotClientDto, hotClientFirmAndClientInfo.Firm, isTelesaleTask);
                }

                throw new BusinessLogicException(
                    string.Format("Зацепка на горячего клиента с id {0} не обработана. CardCode {1} не привел ни к фирме, ни к клиенту.",
                                  requestEntity.Id,
                                  requestEntity.CardCode.Value));
            }

            if (requestEntity.BranchCode != null)
            {
                return CreateForBranch(hotClientDto, requestEntity.BranchCode.Value);
            }

            throw new BusinessLogicException(
                string.Format("Зацепка на горячего клиента с id {0} не обработана. У зацепки не заполнено ни BranchCode, ни CardCode", requestEntity.Id));
        }

        private TaskCreationDto CreateForBranch(HotClientRequestDto hotClientDto, long branchCode)
        {
            return new TaskCreationDto
                {
                    HotClientDto = hotClientDto,
                    Regarding = null,
                    Strategies = new List<StrategyDto> { ForProjectDirector(branchCode) },
                };
        }

        private TaskCreationDto CreateForClient(HotClientRequestDto hotClientDto, Client client, long firmOrganizationUnitId, bool isTelesaleTask)
        {
            return new TaskCreationDto
                {
                    HotClientDto = hotClientDto,
                    Regarding = new RegardingObject
                        {
                            EntityName = EntityName.account.ToString(),
                            ReplicationCode = client.ReplicationCode,
                        },
                    Strategies = isTelesaleTask
                                     ? new List<StrategyDto>
                                         {
                                             ForOrganizationUnitTelesales(firmOrganizationUnitId),
                                             ForOrganizationUnitDirector(firmOrganizationUnitId),
                                         }
                                     : new List<StrategyDto>
                                         {
                                             ForClientOwner(client.OwnerCode),
                                             ForOrganizationUnitDirector(firmOrganizationUnitId),
                                         },
                };
        }

        private TaskCreationDto CreateForFirm(HotClientRequestDto hotClientDto, Firm firm, bool isTelesaleTask)
        {
            return new TaskCreationDto
                {
                    HotClientDto = hotClientDto,
                    Regarding = new RegardingObject
                        {
                            EntityName = "dg_firm",
                            ReplicationCode = firm.ReplicationCode
                        },
                    Strategies = isTelesaleTask
                                     ? new List<StrategyDto>
                                         {
                                             ForOrganizationUnitTelesales(firm.OrganizationUnitId),
                                             ForOrganizationUnitDirector(firm.OrganizationUnitId),
                                         }
                                     : new List<StrategyDto>
                                         {
                                             ForFirmOwner(firm.OwnerCode),
                                             ForOrganizationUnitDirector(firm.OrganizationUnitId),
                                         },
                };
        }

        private UserDto FindOrganizationUnitFranchiseeTelesaleManager(long organizationUnit)
        {
            var telemarketingUser = _userReadModel.FindAnyUserWithPrivelege(new[] { organizationUnit },
                                                                            FunctionalPrivilegeName.HotClientTelemarketingProcessingFranchisee);
            return GetUserDto(telemarketingUser, user => user != null);
        }

        private UserDto FindOrganizationUnitBranchTelesaleManager(long organizationUnit)
        {
            var telemarketingUser = _userReadModel.FindAnyUserWithPrivelege(new[] { organizationUnit },
                                                                            FunctionalPrivilegeName.HotClientTelemarketingProcessingBranch);
            return GetUserDto(telemarketingUser, user => user != null);
        }

        private UserDto FindOrganizationUnitDirector(long organizationUnit)
        {
            var directorUser = _userReadModel.FindAnyUserWithPrivelege(new[] { organizationUnit }, FunctionalPrivilegeName.HotClientProcessing);
            return GetUserDto(directorUser, user => user != null);
        }

        private UserDto FindProjectDirector(long projectCode)
        {
            var organizationUnitIds = _branchOfficeReadModel.GetProjectOrganizationUnitIds(projectCode);
            var director = _userReadModel.FindAnyUserWithPrivelege(organizationUnitIds, FunctionalPrivilegeName.HotClientProcessing);
            return GetUserDto(director, user => user != null);
        }

        private UserDto FindOwner(long ownerCode)
        {
            var owner = _userReadModel.GetUser(ownerCode);
            return GetUserDto(owner, user => user != null && !user.IsServiceUser);
        }

        private UserDto GetUserDto(User user, Func<User, bool> acceptanceCriteria)
        {
            try
            {
                return acceptanceCriteria(user)
                           ? new UserDto { Id = user.Id, Account = user.Account }
                           : null;
            }
            catch (WebException ex)
            {
                throw new ErmCommunicationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
            }
        }

        private StrategyDto ForClientOwner(long userId)
        {
            return new StrategyDto("по куратору клиента User.Id = {0}", userId, FindOwner);
        }

        private StrategyDto ForFirmOwner(long userId)
        {
            return new StrategyDto("по куратору фирмы User.Id = {0}", userId, FindOwner);
        }

        private StrategyDto ForOrganizationUnitDirector(long organizationUnitId)
        {
            return new StrategyDto("по привилегии \"Обработка теплых клиентов\" в OrganizationUnit.Id = {0}", organizationUnitId, FindOrganizationUnitDirector);
        }

        private StrategyDto ForOrganizationUnitTelesales(long organizationUnitId)
        {
            var contributionType = _branchOfficeReadModel.GetOrganizationUnitContributionType(organizationUnitId);
            switch (contributionType)
            {
                case ContributionTypeEnum.Branch:
                    return new StrategyDto("по телепродажам (филиал) OrganizationUnit.Id = {0}", organizationUnitId, FindOrganizationUnitBranchTelesaleManager);
                case ContributionTypeEnum.Franchisees:
                    return new StrategyDto("по телепродажам (франчайзи) OrganizationUnit.Id = {0}",
                                           organizationUnitId,
                                           FindOrganizationUnitFranchiseeTelesaleManager);
                default:
                    throw new InvalidEnumArgumentException("organizationUnitId", (int)contributionType, typeof(ContributionTypeEnum));
            }
        }

        private StrategyDto ForProjectDirector(long projectCode)
        {
            return new StrategyDto("по привилегии \"Обработка теплых клиентов\" в Project.Code = {0}", projectCode, FindProjectDirector);
        }

        private sealed class StrategyDto
        {
            private readonly Func<long, UserDto> _strategy;
            private readonly string _strategyDescriptionTemplate;
            private readonly long _strategyParameter;

            public StrategyDto(string strategyDescriptionTemplate, long strategyParameter, Func<long, UserDto> strategy)
            {
                _strategyDescriptionTemplate = strategyDescriptionTemplate;
                _strategyParameter = strategyParameter;
                _strategy = strategy;
            }

            public string Message
            {
                get { return string.Format(_strategyDescriptionTemplate, _strategyParameter); }
            }

            public UserDto Execute()
            {
                return _strategy.Invoke(_strategyParameter);
            }
        }

        private sealed class TaskCreationDto
        {
            public RegardingObject Regarding { get; set; }
            public IEnumerable<StrategyDto> Strategies { get; set; }
            public HotClientRequestDto HotClientDto { get; set; }
        }
    }
}