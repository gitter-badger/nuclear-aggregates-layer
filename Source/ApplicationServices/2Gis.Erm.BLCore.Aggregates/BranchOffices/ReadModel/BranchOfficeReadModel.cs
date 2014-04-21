using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel
{
    // FIXME {all, 07.04.2014}: к именно текущему состоянию данного типа вопросов не очень много, но предлагаю пока взять timeout на дальнейшее масштабирование практики переопределения readmodel в зависимости от business model через наследование
    // т.к. у нас в приложении в перспективе будет на уровне метаданных полная информация в какой бизнесмодели какие свойства к каким сущностям подцепленны, возможно не придется этим заниматься на уровне императивного кода в readmodel
    // Сам подход partable (расширяемых, возможно лучше было использовать что-то вроде extensibility) сущностей был ориентирован на то, чтобы минимизировать необходимость в создании Chile***Service_Model и т.п. 
    // Итого - до согласования подхода работы с расширяемыми сущностями (EAV и т.п.) пока подход с abstract классом и business model specific подкласами заморожен.
    public abstract class BranchOfficeReadModel : IBranchOfficeReadModel
    {
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;

        protected BranchOfficeReadModel(IFinder finder, ISecureFinder secureFinder)
        {
            _finder = finder;
            _secureFinder = secureFinder;
        }

        public virtual BranchOffice GetBranchOffice(long branchOfficeId)
        {
            return _finder.Find(Specs.Find.ById<BranchOffice>(branchOfficeId)).Single();
        }

        public virtual BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
            {
            return _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId)).Single();
        }

        public virtual BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(string syncCode1C)
        {
            return _finder.Find(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.BySyncCode1C(syncCode1C)).Single();
        }

        // FIXME {all, 07.04.2014}: какое-то слишком абстрактное название дляметодов - readmodel это набор методов, являющихся wrapper над спецификациями, разной толщины - но все они usecase специфичны. Т.о. либо метод должен быть более конкретным, либо тип в которомон находиться более абстрактным
        public virtual IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(BranchOffice branchOffice)
        {
            return Enumerable.Empty<BusinessEntityInstanceDto>();
            }

        // FIXME {all, 07.04.2014}: какое-то слишком абстрактное название дляметодов - readmodel это набор методов, являющихся wrapper над спецификациями, разной толщины - но все они usecase специфичны. Т.о. либо метод должен быть более конкретным, либо тип в которомон находиться более абстрактным
        public virtual IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return Enumerable.Empty<BusinessEntityInstanceDto>();
        }

        public T GetBranchOfficeDto<T>(long entityId)
            where T : BranchOfficeDomainEntityDto, new()
        {
            return _secureFinder.Find<BranchOffice>(x => x.Id == entityId)
                                .Select(entity => new T
                                    {
                                        Id = entity.Id,
                                        DgppId = entity.DgppId,
                                        Name = entity.Name,
                                        Inn = entity.Inn,
                                        Ic = entity.Ic,
                                        Annotation = entity.Annotation,
                                        BargainTypeRef = new EntityReference { Id = entity.BargainTypeId, Name = entity.BargainType.Name },
                                        ContributionTypeRef = new EntityReference { Id = entity.ContributionTypeId, Name = entity.ContributionType.Name },
                                        LegalAddress = entity.LegalAddress,
                                        UsnNotificationText = entity.UsnNotificationText,
                                        Timestamp = entity.Timestamp,
                                        CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                        CreatedOn = entity.CreatedOn,
                                        IsActive = entity.IsActive,
                                        IsDeleted = entity.IsDeleted,
                                        ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                        ModifiedOn = entity.ModifiedOn
                                    })
                                .Single();
        }

        public T GetBranchOfficeOrganizationUnitDto<T>(long entityId)
            where T : BranchOfficeOrganizationUnitDomainEntityDto, new()
        {
            return _secureFinder.Find<BranchOfficeOrganizationUnit>(x => x.Id == entityId)
                                .Select(entity => new T
                                    {
                                        Id = entity.Id,
                                        OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = entity.OrganizationUnit.Name },
                                        BranchOfficeRef = new EntityReference { Id = entity.BranchOfficeId, Name = entity.BranchOffice.Name },
                                        ChiefNameInGenitive = entity.ChiefNameInGenitive,
                                        ChiefNameInNominative = entity.ChiefNameInNominative,
                                        Registered = entity.Registered,
                                        IsPrimary = entity.IsPrimary,
                                        IsPrimaryForRegionalSales = entity.IsPrimaryForRegionalSales,
                                        OperatesOnTheBasisInGenitive = entity.OperatesOnTheBasisInGenitive,
                                        Kpp = entity.Kpp,
                                        PhoneNumber = entity.PhoneNumber,
                                        Email = entity.Email,
                                        PositionInGenitive = entity.PositionInGenitive,
                                        PositionInNominative = entity.PositionInNominative,
                                        ShortLegalName = entity.ShortLegalName,
                                        ActualAddress = entity.ActualAddress,
                                        PostalAddress = entity.PostalAddress,
                                        BranchOfficeAddlId = entity.BranchOffice.Id,
                                        BranchOfficeAddlIc = entity.BranchOffice.Ic,
                                        BranchOfficeAddlInn = entity.BranchOffice.Inn,
                                        BranchOfficeAddlLegalAddress = entity.BranchOffice.LegalAddress,
                                        BranchOfficeAddlName = entity.BranchOffice.Name,
                                        PaymentEssentialElements = entity.PaymentEssentialElements,
                                        SyncCode1C = entity.SyncCode1C,
                                        RegistrationCertificate = entity.RegistrationCertificate,
                                        OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                        CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                        CreatedOn = entity.CreatedOn,
                                        IsActive = entity.IsActive,
                                        IsDeleted = entity.IsDeleted,
                                        ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                        ModifiedOn = entity.ModifiedOn,
                                        Timestamp = entity.Timestamp
                                    })
                                .Single();
        }

        public IEnumerable<long> GetProjectOrganizationUnitIds(long projectCode)
        {
            var organizationUnitIds = _finder.Find<Project>(project => project.Code == projectCode && project.OrganizationUnitId.HasValue)
                                            .Select(project => project.OrganizationUnitId.Value)
                                            .ToArray();
            return organizationUnitIds;
        }

        public ContributionTypeEnum GetOrganizationUnitContributionType(long organizationUnitId)
        {
            var type = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                              .SelectMany(unit => unit.BranchOfficeOrganizationUnits)
                              .Where(Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>())
                              .Where(boou => boou.IsPrimary)
                              .Select(boou => boou.BranchOffice)
                              .Select(branchOffice => branchOffice.ContributionTypeId.Value)
                              .Single();

            return (ContributionTypeEnum)type;
        }
    }
}