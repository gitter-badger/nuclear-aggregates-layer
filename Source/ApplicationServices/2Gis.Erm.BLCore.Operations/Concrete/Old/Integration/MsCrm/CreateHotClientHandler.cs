using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.HotClient;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
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

        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IUserReadModel _userReadModel;
        private readonly IFirmRepository _firmRepository;
        private readonly IHotClientRequestService _hotClientRequestService;
        private readonly ICrmTaskFactory _crmTaskFactory;

        public CreateHotClientHandler(IMsCrmSettings msCrmSettings,
                                      IUserReadModel userReadModel,
                                      IFirmRepository firmRepository,
                                      IHotClientRequestService hotClientRequestService,
                                      ICrmTaskFactory crmTaskFactory)
        {
            _msCrmSettings = msCrmSettings;
            _userReadModel = userReadModel;
            _firmRepository = firmRepository;
            _hotClientRequestService = hotClientRequestService;
            _crmTaskFactory = crmTaskFactory;
        }

        /// <summary>
        /// Cоздаёт задачу для определённого пользователя в MS Dynamics, соответвующую указаному запросу HotClientRequest.
        /// </summary>
        /// <exception cref="BusinessLogicException">Когда сущность не найдена, не определён исполнитель зазачи или MS Dynamics не создал для нас задачу</exception>
        /// <exception cref="ErmCommunicationException">При ошибках, связанных со взаимодействием с MS Dynamics</exception>
        /// <returns>Success = true, если интеграция включена, false иначе.</returns>
        protected override CreateHotClientResponse Handle(CreateHotClientRequest request)
        {
            var hotClientRequest = _hotClientRequestService.GetHotClientRequest(request.Id);
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
                _hotClientRequestService.LinkWithCrmTask(requestEntity.Id, taskId);
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
                var firmCategoryGroups = _firmRepository.GetFirmAddressCategoryGroups(requestEntity.CardCode.Value);
                
                var mostExpensiveCategoryGroup = firmCategoryGroups.OrderByDescending(categoryGroup => categoryGroup != null ? categoryGroup.GroupRate : DefaultCategoryRate)
                                                                   .FirstOrDefault();

                var isTelesaleTask = mostExpensiveCategoryGroup != null && mostExpensiveCategoryGroup.Id == TelesaleCategoryGroupId;

                var hotClientFirmAndClientInfo = _firmRepository.GetFirmAndClientByFirmAddress(requestEntity.CardCode.Value);

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
                        }
                                     : new List<StrategyDto>
                                         {
                                             ForFirmOwner(firm.OwnerCode),
                                             ForOrganizationUnitDirector(firm.OrganizationUnitId),
                                         },
                };
        }

        private UserDto FindOrganizationUnitTelesales(long organizationUnit)
        {
            var telemarketingUser = _userReadModel.FindAnyUserWithPrivelege(new[] { organizationUnit }, FunctionalPrivilegeName.HotClientTelemarketingProcessing);
            return GetUserDto(telemarketingUser, user => user != null);
        }

        private UserDto FindOrganizationUnitDirector(long organizationUnit)
        {
            var directorUser = _userReadModel.FindAnyUserWithPrivelege(new[] { organizationUnit }, FunctionalPrivilegeName.HotClientProcessing);
            return GetUserDto(directorUser, user => user != null);
        }

        private UserDto FindProjectDirector(long projectCode)
        {
            var organizationUnitIds = _firmRepository.GetProjectOrganizationUnitIds(projectCode);
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
            return new StrategyDto("по директору OrganizationUnit.Id = {0}", organizationUnitId, FindOrganizationUnitDirector);
        }

        private StrategyDto ForOrganizationUnitTelesales(long organizationUnitId)
        {
            return new StrategyDto("по телепродажам OrganizationUnit.Id = {0}", organizationUnitId, FindOrganizationUnitTelesales);
        }

        private StrategyDto ForProjectDirector(long projectCode)
        {
            return new StrategyDto("по директору Project.Code = {0}", projectCode, FindProjectDirector);
        }

        private sealed class StrategyDto
        {
            private readonly string _strategyDescriptionTemplate;
            private readonly long _strategyParameter;
            private readonly Func<long, UserDto> _strategy;

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
