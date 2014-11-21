﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.HotClients
{
    // TODO {all, 04.02.2014}: рефакторинг с конвертацией в OperationService, SRP и т.п., при этом учесть фактическое пересечение по используемым выборкам с *OrderProcessing* функционалом - нужно обобщить, в том числе и на уровне ReadModel
    public class GetHotClientTaskToReplicateOperationService : IGetHotClientTaskToReplicateOperationService
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IUserReadModel _userReadModel;

        public GetHotClientTaskToReplicateOperationService(IUserReadModel userReadModel,
                                                   IFirmReadModel firmReadModel,
                                                   IBranchOfficeReadModel branchOfficeReadModel)
        {
            _userReadModel = userReadModel;
            _firmReadModel = firmReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        public HotClientTaskDto GetHotClientTask(long hotClientRequestId)
        {
            var hotClientRequest = _firmReadModel.GetHotClientRequest(hotClientRequestId);

            var dto = CreateTaskDto(hotClientRequest);
            var taskOwner = ApplyOwnerSearchStrategies(hotClientRequest.Id, dto.Strategies);

            return new HotClientTaskDto { TaskOwner = taskOwner, HotClientDto = dto.HotClientDto, Regarding = dto.Regarding };
        }

        private UserDto ApplyOwnerSearchStrategies(long hotClientRequestId, IReadOnlyCollection<StrategyDto> strategies)
        {
            var user = strategies.Select(strategy => strategy.Execute()).FirstOrDefault(userDto => userDto != null);
            if (user != null)
            {
                return user;
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
                return CreateForCard(requestEntity.Id, requestEntity.CardCode.Value, hotClientDto);
            }

            if (requestEntity.BranchCode != null)
            {
                return CreateForBranch(hotClientDto, requestEntity.BranchCode.Value);
            }

            throw new BusinessLogicException(
                string.Format("Зацепка на горячего клиента с id {0} не обработана. У зацепки не заполнено ни BranchCode, ни CardCode", requestEntity.Id));
        }

        private TaskCreationDto CreateForCard(long requestId, long cardCode, HotClientRequestDto hotClientDto)
        {
            FirmAndClientDto dto;
            if (!_firmReadModel.TryGetFirmAndClientByFirmAddress(cardCode, out dto))
            {
                throw new BusinessLogicException(
                    string.Format("Зацепка на горячего клиента с id {0} не обработана. CardCode {1} не привел ни к фирме, ни к клиенту.",
                                  requestId,
                                  cardCode));
            }

            var isTelesaleTask = _firmReadModel.IsTelesaleFirmAddress(cardCode);
            var client = dto.Client;
            var firm = dto.Firm;
            return client != null
                       ? CreateForClient(hotClientDto, client, firm.OrganizationUnitId, isTelesaleTask)
                       : CreateForFirm(hotClientDto, firm, isTelesaleTask);
        }

        private TaskCreationDto CreateForBranch(HotClientRequestDto hotClientDto, long branchCode)
        {
            return new TaskCreationDto
                {
                    HotClientDto = hotClientDto,
                    Regarding = null,
                    Strategies = new[] { ForProjectDirector(branchCode) },
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
                                     ? new[]
                                         {
                                             ForOrganizationUnitTelesales(firmOrganizationUnitId),
                                             ForOrganizationUnitDirector(firmOrganizationUnitId)
                                         }
                                     : new[]
                                         {
                                             ForClientOwner(client.OwnerCode),
                                             ForOrganizationUnitDirector(firmOrganizationUnitId)
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
                                     ? new[]
                                         {
                                             ForOrganizationUnitTelesales(firm.OrganizationUnitId),
                                             ForOrganizationUnitDirector(firm.OrganizationUnitId)
                                         }
                                     : new[]
                                         {
                                             ForFirmOwner(firm.OwnerCode),
                                             ForOrganizationUnitDirector(firm.OrganizationUnitId)
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
            return acceptanceCriteria(user)
                       ? new UserDto { Id = user.Id, Account = user.Account }
                       : null;
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
            public IReadOnlyCollection<StrategyDto> Strategies { get; set; }
            public HotClientRequestDto HotClientDto { get; set; }
        }
    }
}