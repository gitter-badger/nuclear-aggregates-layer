using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Xml;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeLegalPersonHandler : SerializeObjectsHandler<LegalPerson, ExportFlowFinancialDataLegalEntity>
    {
        private static readonly CultureInfo RussianCultureInfo = CultureInfo.GetCultureInfo(1049);
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        
        public SerializeLegalPersonHandler(ISecurityServiceUserIdentifier securityServiceUserIdentifier, 
                                           IExportRepository<LegalPerson> exportOperationsRepository, 
                                           ITracer logger)
            : base(exportOperationsRepository, logger)
        {
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override IEnumerable<IExportableEntityDto> ProcessDtosAfterMaterialization(IEnumerable<IExportableEntityDto> entityDtos)
        {
            return entityDtos.Cast<LegalPersonDto>().Select(dto =>
            {
                {
                    var userInfo = _securityServiceUserIdentifier.GetUserInfo(dto.LegalPerson.OwnerCode);
                    dto.Curator = !string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.DisplayName : userInfo.Account;
                    dto.CuratorLogin = userInfo.Account;
                }

                foreach (var profile in dto.Profiles)
                {
                    var userInfo = _securityServiceUserIdentifier.GetUserInfo(profile.OwnerCode);
                    profile.Curator = !string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.DisplayName : userInfo.Account;
                    profile.CuratorLogin = userInfo.Account;
                }

                switch (dto.LegalEntityType)
                {
                    case LegalPersonType.LegalPerson:
                        {
                            dto.LegalEntityLegalPerson = new LegalEntityLegalPersonDto
                            {
                                TIN = dto.LegalPerson.Inn, 
                                KPP = dto.LegalPerson.Kpp, 
                                LegalAddress = dto.LegalPerson.LegalAddress
                            };
                        }

                        break;

                    case LegalPersonType.Businessman:
                        {
                            dto.LegalEntitySoleProprietor = new LegalEntitySoleProprietorDto
                            {
                                TIN = dto.LegalPerson.Inn, 
                                LegalAddress = dto.LegalPerson.LegalAddress
                            };
                        }

                        break;

                    case LegalPersonType.NaturalPerson:
                        {
                            dto.LegalEntityIndividualPerson = new LegalEntityIndividualPersonDto
                            {
                                PassportSeries = dto.LegalPerson.PassportSeries, 
                                PassportNumber = dto.LegalPerson.PassportNumber, 
                                PassportIssuedBy = dto.LegalPerson.PassportIssuedBy, 
                                Residence = dto.LegalPerson.RegistrationAddress
                            };
                        }

                        break;
                }

                return dto;
            });
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            return ((LegalPersonDto)entityDto).ToXElement();
        }

        protected override string GetError(IExportableEntityDto entityDto)
        {
            var legalPersonDto = (LegalPersonDto)entityDto;
            var activeAndNonDeleted = !legalPersonDto.IsHidden && !legalPersonDto.IsDeleted;

            if (!legalPersonDto.Profiles.Any() && activeAndNonDeleted)
            {
                return BLResources.MustMakeLegalPersonProfile;
            }

            if (string.IsNullOrEmpty(legalPersonDto.Curator) || string.IsNullOrEmpty(legalPersonDto.CuratorLogin))
            {
                return string.Format("Юр.лицо клиента с id=[{0}]. Не указан куратор", legalPersonDto.LegalPerson.Id);
            }

            if (legalPersonDto.DefaultProfileCode == null && activeAndNonDeleted)
            {
                return string.Format("Юр.лицо клиента с id=[{0}]. Отсутствует профиль по умолчанию у активного юр. лица", legalPersonDto.Code);
            }

            if (string.IsNullOrEmpty(legalPersonDto.Name))
            {
                return string.Format("Юр.лицо клиента с id=[{0}]. Не указано название", legalPersonDto.Code);
            }

            switch (legalPersonDto.LegalEntityType)
            {
                case LegalPersonType.LegalPerson:
                {
                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntityLegalPerson.TIN))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'ИНН'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntityLegalPerson.KPP))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'КПП'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntityLegalPerson.LegalAddress))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'Юридический адрес'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    break;
                }

                case LegalPersonType.Businessman:
                {
                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntitySoleProprietor.TIN))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'ИНН'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntitySoleProprietor.LegalAddress))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'Юридический адрес'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    break;
                }

                case LegalPersonType.NaturalPerson:
                {
                    uint parseResult;
                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntityIndividualPerson.PassportSeries) ||
                        !uint.TryParse(legalPersonDto.LegalEntityIndividualPerson.PassportSeries, out parseResult))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'Серия паспорта'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntityIndividualPerson.PassportNumber) ||
                        !uint.TryParse(legalPersonDto.LegalEntityIndividualPerson.PassportNumber, out parseResult))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'Номер паспорта'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntityIndividualPerson.PassportIssuedBy))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'Паспорт выдан'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    if (string.IsNullOrEmpty(legalPersonDto.LegalEntityIndividualPerson.Residence))
                    {
                        return string.Format(@"Юр.лицо клиента с id=[{0}] типа:[{1}]. Имеет недопустимое значение поля 'Адрес проживания'",
                                             legalPersonDto.LegalPerson.Id,
                                             legalPersonDto.LegalEntityType);
                    }

                    break;
                }
            }

            foreach (var profile in legalPersonDto.Profiles)
            {
                if (string.IsNullOrEmpty(profile.Name))
                {
                    return string.Format("Профиль юр.лица клиента с id=[{0}]. Не указано название", profile.Code);
                }

                if (string.IsNullOrEmpty(profile.Curator) || string.IsNullOrEmpty(profile.CuratorLogin))
                {
                    return string.Format("Профиль с id=[{0}]. Не указан куратор", profile.Code);
                }
            }

            foreach (var account in legalPersonDto.Accounts)
            {
                if (account.BargainTypeCode != 1 && account.BargainTypeCode != 2)
                {
                    return string.Format(@"Лицевой счет с id=[{0}]. Имеет недопустимое значение поля 'Тип договора'", account.Code);
                }

                if (string.IsNullOrEmpty(account.Code1C))
                {
                    return string.Format(@"Лицевой счет с id=[{0}]. Имеет недопустимое значение поля 'Код 1С лицевого счета'", account.Code);
                }

                if (string.IsNullOrEmpty(account.LegalEntityBranchCode1C))
                {
                    return string.Format(@"Лицевой счет с id=[{0}]. Имеет недопустимое значение поля 'Код 1С юридического лица отделения организации'",
                                         account.Code);
                }
            }

            return null;
        }

        protected override ISelectSpecification<LegalPerson, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<LegalPerson, IExportableEntityDto>(lp => new LegalPersonDto
                {
                    Id = lp.Id, 

                    Profiles = lp.LegalPersonProfiles.Select(lpp => new LegalEntityProfile
                        {
                            Code = lpp.Id, 
                            DirectorName = lpp.ChiefNameInNominative, 
                            DocumentDeliveryMethod = lpp.DocumentsDeliveryMethod, 
                            DocumentDeliveryAddress = lpp.DocumentsDeliveryAddress, 
                            Email = lpp.EmailForAccountingDocuments ?? lpp.AdditionalEmail, 
                            PaymentDetails = lpp.PaymentEssentialElements,
                            IsHidden = !lpp.IsActive, 
                            OperatesOnTheBasisInGenitive = lpp.OperatesOnTheBasisInGenitive,
                            OwnerCode = lpp.OwnerCode,
                            WarrantyBeginDate = lpp.WarrantyBeginDate,
                            WarrantyNumber = lpp.WarrantyNumber,
                            CertificateNumber = lpp.CertificateNumber,
                            CertificateDate = lpp.CertificateDate,
                            BargainNumber = lpp.BargainNumber,
                            BargainBeginDate = lpp.BargainBeginDate,
                            Name = lpp.Name, 
                            PersonResponsibleForDocuments = lpp.PersonResponsibleForDocuments, 
                            Phone = lpp.Phone, 
                            Position = lpp.PositionInNominative, 
                            PostAddress = lpp.PostAddress, 
                            RecipientName = lpp.RecipientName, 
                        }), 

                    Accounts = lp.Accounts.Select(a => new LegalEntityAccount
                        {
                            BargainTypeCode = a.BranchOfficeOrganizationUnit.BranchOffice.BargainTypeId ?? 0, 
                            Code = a.Id, 
                            LegalEntityBranchCode = a.BranchOfficeOrganizationUnitId,
                            Code1C = a.LegalPesonSyncCode1C, 
                            IsHidden = !a.IsActive, 
                            IsDeleted = a.IsDeleted, 
                            LegalEntityBranchCode1C = a.BranchOfficeOrganizationUnit.SyncCode1C, 
                        }), 

                    LegalPerson = new LegalPersonFieldsDto 
                    {
                        Inn = lp.Inn,
                        Kpp = lp.Kpp,
                        LegalAddress = lp.LegalAddress,
                        PassportSeries = lp.PassportSeries,
                        PassportNumber = lp.PassportNumber,
                        PassportIssuedBy = lp.PassportIssuedBy,
                        RegistrationAddress = lp.RegistrationAddress,
                        Id = lp.Id,
                        OwnerCode = lp.OwnerCode,
                    }, 
                    Code = lp.Id, 
                    Name = lp.LegalName, 
                    ShortName = lp.ShortName, 
                    DefaultProfileCode = lp.LegalPersonProfiles.Where(x => x.IsMainProfile).Select(x => (long?)x.Id).FirstOrDefault(),
                    IsHidden = !lp.IsActive, 
                    IsDeleted = lp.IsDeleted, 
                    LegalEntityType = lp.LegalPersonTypeEnum, 
                });
        }

        #region nested types

        public sealed class LegalPersonFieldsDto
        {
            public string Inn { get; set; }
            public string Kpp { get; set; }
            public string LegalAddress { get; set; }
            public string PassportSeries { get; set; }
            public string PassportNumber { get; set; }
            public string PassportIssuedBy { get; set; }
            public string RegistrationAddress { get; set; }
            public long Id { get; set; }
            public long OwnerCode { get; set; }
        }

        public sealed class LegalPersonDto : IExportableEntityDto
        {
            private const string TagName = "LegalEntity";

            public long Id { get; set; }

            public LegalPersonFieldsDto LegalPerson { get; set; }

            /// <summary>
            /// Стабильный идентификатор юридического лица клиента. Required
            /// </summary>
            public long Code { get; set; }

            /// <summary>
            /// Название юридического лица клиента. Required
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Краткое название юридического лица клиента. Optional
            /// </summary>
            public string ShortName { private get; set; }

            /// <summary>
            /// Стабильный идентификатор профиля по умолчанию (o)
            /// </summary>
            public long? DefaultProfileCode { get; set; }

            /// <summary>
            /// Куратор юридического лица клиента. Required
            /// </summary>
            public string Curator { get; set; }

            /// <summary>
            /// Логин куратора юридического лица клиента. Без указания домена. Просто, как хранится у нас в таблице пользователей (r)
            /// </summary>
            public string CuratorLogin { get; set; }

            /// <summary>
            /// Признак активности юридического лица клиента.
            /// Если атрибут пришел со значением true, то считаем, что юридическое лицо скрыто.
            /// В противном случае считаем, что юридическое лицо активно. Optional.
            /// default="false"
            /// </summary>
            public bool IsHidden { get; set; }

            /// <summary>
            /// Признак удаленности юридического лица клиента.
            /// Если атрибут пришел со значением true, то считаем, что юридическое лицо клиента удалено.
            /// В противном случае считаем, что юридическое лицо клиента не удалено. Optional.
            /// default="false"
            /// </summary>
            public bool IsDeleted { get; set; }

            public IEnumerable<LegalEntityAccount> Accounts { get; set; }
            public IEnumerable<LegalEntityProfile> Profiles { get; set; }

            public LegalPersonType LegalEntityType { get; set; }
            public LegalEntityLegalPersonDto LegalEntityLegalPerson { get; set; }
            public LegalEntitySoleProprietorDto LegalEntitySoleProprietor { get; set; }
            public LegalEntityIndividualPersonDto LegalEntityIndividualPerson { get; set; }

            public XElement ToXElement()
            {
                var xmlTree = new XElement(TagName, 
                                           this.ToXAttribute(() => Code, Code), 
                                           this.ToXAttribute(() => Name, Name), 
                                           !string.IsNullOrEmpty(ShortName) ? this.ToXAttribute(() => ShortName, ShortName) : null, 
                                           DefaultProfileCode != null ? this.ToXAttribute(() => DefaultProfileCode, DefaultProfileCode) : null,
                                           !string.IsNullOrEmpty(Curator) ? this.ToXAttribute(() => Curator, Curator) : null, 
                                           !string.IsNullOrEmpty(CuratorLogin) ? this.ToXAttribute(() => CuratorLogin, CuratorLogin) : null, 
                                           IsHidden ? this.ToXAttribute(() => IsHidden, IsHidden) : null, 
                                           IsDeleted ? this.ToXAttribute(() => IsDeleted, IsDeleted) : null, 
// ReSharper disable CoVariantArrayConversion
                                           this.ToXElement(() => Profiles, Profiles.Select(x => x.ToXElement()).ToArray()), 
                                           this.ToXElement(() => Accounts, Accounts.Select(x => x.ToXElement()).ToArray()), 
// ReSharper restore CoVariantArrayConversion
                                           GetLegalEntityXml());

                return xmlTree; 
            }

            private XElement GetLegalEntityXml()
            {
                switch (LegalEntityType)
                {
                    case LegalPersonType.LegalPerson:
                        return LegalEntityLegalPerson.ToXElement();
                    case LegalPersonType.Businessman:
                        return LegalEntitySoleProprietor.ToXElement();
                    case LegalPersonType.NaturalPerson:
                        return LegalEntityIndividualPerson.ToXElement();
                    default:
                        return null;
                }
            }
        }

        public sealed class LegalEntityLegalPersonDto
        {
            private const string TagName = "LegalPerson";

            /// <summary>
            /// ИНН. Required
            /// </summary>
            // ReSharper disable once InconsistentNaming
            public string TIN { get; set; }

            /// <summary>
            /// КПП. Required
            /// </summary>
            // ReSharper disable once InconsistentNaming
            public string KPP { get; set; }

            /// <summary>
            /// Юридический адрес. Required
            /// </summary>
            public string LegalAddress { get; set; }

            public XElement ToXElement()
            {
                return new XElement(TagName, 
                                    this.ToXAttribute(() => TIN, TIN), 
                                    this.ToXAttribute(() => KPP, KPP), 
                                    this.ToXAttribute(() => LegalAddress, LegalAddress));
            }
        }

        public sealed class LegalEntitySoleProprietorDto
        {
            private const string TagName = "SoleProprietor";

            /// <summary>
            /// ИНН. Required
            /// </summary>
            // ReSharper disable once InconsistentNaming
            public string TIN { get; set; }

            /// <summary>
            /// Юридический адрес. Required
            /// </summary>
            public string LegalAddress { get; set; }

            public XElement ToXElement()
            {
                return new XElement(TagName, 
                                    this.ToXAttribute(() => TIN, TIN), 
                                    this.ToXAttribute(() => LegalAddress, LegalAddress));
            }
        }

        public sealed class LegalEntityIndividualPersonDto
        {
            private const string TagName = "IndividualPerson";

            /// <summary>
            /// Серия паспорта. Required
            /// </summary>
            public string PassportSeries { get; set; }

            /// <summary>
            /// Номер паспорта. Required
            /// </summary>
            public string PassportNumber { get; set; }

            /// <summary>
            /// Кем выдан. Required
            /// </summary>
            public string PassportIssuedBy { get; set; }

            /// <summary>
            /// Прописка. Required
            /// </summary>
            public string Residence { get; set; }

            public XElement ToXElement()
            {
                return new XElement(TagName, 
                                    this.ToXAttribute(() => PassportSeries, PassportSeries), 
                                    this.ToXAttribute(() => PassportNumber, PassportNumber), 
                                    this.ToXAttribute(() => PassportIssuedBy, PassportIssuedBy), 
                                    this.ToXAttribute(() => Residence, Residence));
            }
        }

        public class LegalEntityAccount
        {
            private const string TagName = "Account";

            /// <summary>
            /// Стабильный идентификатор лицевого счета (r)
            /// </summary>
            public long Code { get; set; }

            /// <summary>
            /// Стабильный идентификатор юридического лица отделения организации (r)
            /// </summary>
            public long LegalEntityBranchCode { get; set; }

            /// <summary>
            /// Код 1С лицевого счета (r)
            /// </summary>
            public string Code1C { get; set; }

            /// <summary>
            /// Тип договора (r) -> int
            /// </summary>
            public long BargainTypeCode { get; set; }

            /// <summary>
            /// Код 1С юридического лица отделения организации (r)
            /// </summary>
            public string LegalEntityBranchCode1C { get; set; }

            /// <summary>
            /// Признак неактивности лицевого счета (o:false)
            /// </summary>
            public bool IsHidden { private get; set; }

            /// <summary>
            /// Признак удаленности лицевого счета (o:false)
            /// </summary>
            public bool IsDeleted { private get; set; }

            public XElement ToXElement()
            {
                return new XElement(TagName, 
                                    this.ToXAttribute(() => Code, Code),
                                    this.ToXAttribute(() => LegalEntityBranchCode, LegalEntityBranchCode), 
                                    this.ToXAttribute(() => Code1C, Code1C), 
                                    this.ToXAttribute(() => BargainTypeCode, BargainTypeCode), 
                                    this.ToXAttribute(() => LegalEntityBranchCode1C, LegalEntityBranchCode1C), 
                                    IsHidden ? this.ToXAttribute(() => IsHidden, IsHidden) : null, 
                                    IsDeleted ? this.ToXAttribute(() => IsDeleted, IsDeleted) : null);
            }
        }

        public class LegalEntityProfile
        {
            private const string TagName = "Profile";

            /// <summary>
            /// Стабильный идентификатор профила юридического лица клиента (r)
            /// </summary>
            public long Code { get; set; }

            /// <summary>
            /// Название юридического лица клиента (r)
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Электронный адрес (o)
            /// </summary>
            public string Email { private get; set; }

            /// <summary>
            /// Платежные реквизиты (o)
            /// </summary>
            public string PaymentDetails { private get; set; }

            /// <summary>
            ///  Адрес доставки документов (o)
            /// </summary>
            public string DocumentDeliveryAddress { private get; set; }

            /// <summary>
            /// Телефон получателя (o)
            /// </summary>
            public string Phone { private get; set; }

            /// <summary>
            /// Лицо, ответственное за документы (o)
            /// </summary>
            public string PersonResponsibleForDocuments { private get; set; }

            /// <summary>
            /// Почтовый адрес (o)
            /// </summary>
            public string PostAddress { private get; set; }

            /// <summary>
            /// Получатель (o)
            /// </summary>
            public string RecipientName { private get; set; }

            /// <summary>
            /// Способ доставки документов (r) -> string
            /// </summary>
            public DocumentsDeliveryMethod DocumentDeliveryMethod { private get; set; }

            /// <summary>
            /// Директор (o)
            /// </summary>
            public string DirectorName { private get; set; }

            /// <summary>
            /// Должность (o)
            /// </summary>
            public string Position { private get; set; }

            /// <summary>
            /// Признак неактивности профиля (o:false)
            /// </summary>
            public bool IsHidden { private get; set; }

            /// <summary>
            /// Логин (из AD) куратора профиля
            /// </summary>
            public string CuratorLogin { get; set; }

            /// <summary>
            /// Куратор профиля
            /// </summary>
            public string Curator { get; set; }

            public long OwnerCode { get; set; }

            #region OnBasicOf 

            public OperatesOnTheBasisType? OperatesOnTheBasisInGenitive { private get; set; }

            public string CertificateNumber { private get; set; }

            public string WarrantyNumber { private get; set; }

            public DateTime? WarrantyBeginDate { private get; set; }

            public DateTime? CertificateDate { private get; set; }

            public string BargainNumber { private get; set; }

            public DateTime? BargainBeginDate { private get; set; }

            /// <summary>
            /// Действует на основании (o) 
            /// </summary>
            /// <summary>
            /// Действует на основании. Optional
            /// </summary>
            private string OnBasicOf
            {
                get
                {
                    if (OperatesOnTheBasisInGenitive == null)
                    {
                        return string.Empty;
                    }

                    // FIXME {all, 25.10.2013}: явно используется локаль ru-RU
                    switch (OperatesOnTheBasisInGenitive)
                    {
                        case OperatesOnTheBasisType.Undefined:
                            return OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, RussianCultureInfo);

                        case OperatesOnTheBasisType.Charter:
                            return OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, RussianCultureInfo);

                        case OperatesOnTheBasisType.Certificate:
                            return OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, RussianCultureInfo) +
                                   " " +
                                   (CertificateNumber ?? string.Empty) +
                                " от " +
                                   (CertificateDate == null ? string.Empty : CertificateDate.ToString());

                        case OperatesOnTheBasisType.Warranty:
                            return OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, RussianCultureInfo) +
                                   " " + (WarrantyNumber ?? string.Empty) +
                                " от " +
                                   (WarrantyBeginDate == null ? string.Empty : WarrantyBeginDate.ToString());

                        case OperatesOnTheBasisType.Bargain:
                            return OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, RussianCultureInfo) +
                                   " " +
                                   (BargainNumber ?? string.Empty) +
                                         " от " +
                                   (BargainBeginDate == null ? string.Empty : BargainBeginDate.ToString());

                        case OperatesOnTheBasisType.FoundingBargain:
                            return OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, RussianCultureInfo);

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            #endregion

            public XElement ToXElement()
            {
                return new XElement(TagName, 
                                    this.ToXAttribute(() => Code, Code), 
                                    this.ToXAttribute(() => Name, Name), 
                                    !string.IsNullOrEmpty(Email) ? this.ToXAttribute(() => Email, Email) : null, 
                                    !string.IsNullOrEmpty(PaymentDetails) ? this.ToXAttribute(() => PaymentDetails, PaymentDetails) : null,
                                    !string.IsNullOrEmpty(DocumentDeliveryAddress) ? this.ToXAttribute(() => DocumentDeliveryAddress, DocumentDeliveryAddress) : null, 
                                    !string.IsNullOrEmpty(Phone) ? this.ToXAttribute(() => Phone, Phone) : null, 
                                    !string.IsNullOrEmpty(PersonResponsibleForDocuments) ? this.ToXAttribute(() => PersonResponsibleForDocuments, PersonResponsibleForDocuments) : null, 
                                    !string.IsNullOrEmpty(PostAddress) ? this.ToXAttribute(() => PostAddress, PostAddress) : null, 
                                    !string.IsNullOrEmpty(RecipientName) ? this.ToXAttribute(() => RecipientName, RecipientName) : null, 
                                    this.ToXAttribute(() => DocumentDeliveryMethod, DocumentDeliveryMethod.ToString()), 
                                    !string.IsNullOrEmpty(DirectorName) ? this.ToXAttribute(() => DirectorName, DirectorName) : null, 
                                    !string.IsNullOrEmpty(Position) ? this.ToXAttribute(() => Position, Position) : null, 
                                    !string.IsNullOrEmpty(OnBasicOf) ? this.ToXAttribute(() => OnBasicOf, OnBasicOf) : null, 
                                    this.ToXAttribute(() => Curator, Curator),
                                    this.ToXAttribute(() => CuratorLogin, CuratorLogin),
                                    IsHidden ? this.ToXAttribute(() => IsHidden, IsHidden) : null);
            }
        }

        #endregion
    }
}
