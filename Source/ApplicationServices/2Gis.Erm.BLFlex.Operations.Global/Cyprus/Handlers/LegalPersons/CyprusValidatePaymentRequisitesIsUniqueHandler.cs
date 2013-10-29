﻿using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.LegalPersons;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Core.RequestResponse.LegalPersons;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.Handlers.LegalPersons
{
    public sealed class CyprusValidatePaymentRequisitesIsUniqueHandler : RequestHandler<ValidatePaymentRequisitesIsUniqueRequest, EmptyResponse>, ICyprusAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ILegalPersonRepository _legalPersonRepository;

        public CyprusValidatePaymentRequisitesIsUniqueHandler(
            ILegalPersonRepository legalPersonRepository,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _legalPersonRepository = legalPersonRepository;
        }

        protected override EmptyResponse Handle(ValidatePaymentRequisitesIsUniqueRequest request)
        {
            var modelLegalPersonType = (LegalPersonType)request.Entity.LegalPersonTypeEnum;
            var tic = !string.IsNullOrEmpty(request.Entity.Inn) ? request.Entity.Inn.Trim() : null;

            switch (modelLegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    {
                        var ticDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, tic);
                        if (!ticDublicate.ActiveDublicateExists && !ticDublicate.InactiveDublicateExists && !ticDublicate.DeletedDublicateExists)
                        {
                            break;
                        }

                        // { fixme, v.sinitsyn } Вряд ли нижеследующая логика для этого пермишена имеет смысл в кипрской версии
                        var permission = GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code));

                        switch (permission)
                        {
                            case LegalPersonChangeRequisitesAccess.None:
                                {
                                    if (ticDublicate.ActiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.CyprusActiveLegalPersonWithSpecifiedTicExist);        
                                    }

                                    if (ticDublicate.InactiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.CyprusInactiveLegalPersonWithSpecifiedTicExist);
                                    }

                                    if (ticDublicate.DeletedDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.CyprusDeletedLegalPersonWithSpecifiedTicExist);
                                    } 
                                }

                                break;
                                
                            case LegalPersonChangeRequisitesAccess.Granted:
                                break;

                            case LegalPersonChangeRequisitesAccess.GrantedLimited:
                                {
                                    // В российской версии здесь находится проверка на дубль по инн и кпп
                                    if (ticDublicate.ActiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.CyprusActiveLegalPersonWithSpecifiedTicExist);
                                    }

                                    if (ticDublicate.InactiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.CyprusInactiveLegalPersonWithSpecifiedTicExist);
                                    }

                                    if (ticDublicate.DeletedDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.CyprusDeletedLegalPersonWithSpecifiedTicExist);
                                    }  
                                }

                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    break;

                case LegalPersonType.Businessman:
                    {
                        var innDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, tic);

                        if (innDublicate.ActiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.CyprusActiveBusinessmanWithSpecifiedTicExist);
                        }

                        if (innDublicate.InactiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.CyprusInactiveBusinessmanWithSpecifiedTicExist);
                        }

                        if (innDublicate.DeletedDublicateExists)
                        {
                            throw new NotificationException(BLResources.CyprusDeletedBusinessManWithSpecifiedTicExist);
                        }  
                    }

                    break;

                case LegalPersonType.NaturalPerson:
                    {
                        var passportSeries = !string.IsNullOrEmpty(request.Entity.PassportSeries) ? request.Entity.PassportSeries.Trim() : null;
                        var passportNumber = !string.IsNullOrEmpty(request.Entity.PassportNumber) ? request.Entity.PassportNumber.Trim() : null;

                        var passportNumberDublicate = _legalPersonRepository.CheckIfExistsPassportDuplicate(request.Entity.Id,
                                                                                                            passportSeries,
                                                                                                            passportNumber);

                        if (passportNumberDublicate.ActiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.ActiveLegalPersonWithSpecifiedPassportNumberAlreadyExist);
                        }

                        if (passportNumberDublicate.InactiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.InactiveLegalPersonWithSpecifiedPassportNumberAlreadyExist);
                        }

                        if (passportNumberDublicate.DeletedDublicateExists)
                        {
                            throw new NotificationException(BLResources.DeletedLegalPersonWithSpecifiedPassportNumberAlreadyExist);
                        }
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Response.Empty;
        }

        private static LegalPersonChangeRequisitesAccess GetMaxAccess(int[] accesses)
        {
            if (!accesses.Any())
            {
                return LegalPersonChangeRequisitesAccess.None;
            }

            var priorities = new[] { LegalPersonChangeRequisitesAccess.None, LegalPersonChangeRequisitesAccess.GrantedLimited, LegalPersonChangeRequisitesAccess.Granted };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (LegalPersonChangeRequisitesAccess)x)).Max();
            return priorities[maxPriority];
        }
    }
}
