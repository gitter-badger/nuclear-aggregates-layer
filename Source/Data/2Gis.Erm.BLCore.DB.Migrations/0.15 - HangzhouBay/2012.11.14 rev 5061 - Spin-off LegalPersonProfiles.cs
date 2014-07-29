using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5061, "Выделение сущности профиль юр. лица клиента")]
    public class Migration5061 : TransactedMigration
    {
        private const string LegalPersonProfileTable = "LegalPersonProfiles";
        private const string LegalPersonProfileSchemaName = "Billing";

        private const string LegalPersonTable = "LegalPersons";
        private const string LegalPersonSchemaName = "Billing";

        private const string OrderTable = "Orders";
        private const string OrderSchemaName = "Billing";

        private const string UpdateProfilesScript = "SET ANSI_WARNINGS OFF " +
                                                    "UPDATE Billing.LegalPersonProfiles " +
                                                    "SET " +
                                                    "IsActive = persons.IsActive, " +
                                                    "IsDeleted = persons.IsDeleted, " +
                                                    "Name = persons.LegalName, " +
                                                    "PositionInNominative = persons.PositionInNominative, " +
                                                    "PositionInGenitive =  persons.PositionInGenitive, " +
                                                    "ChiefNameInNominative = persons.ChiefNameInNominative, " +
                                                    "ChiefNameInGenitive = persons.ChiefNameInGenitive, " +
                                                    "DocumentsDeliveryAddress = COALESCE(persons.DocumentsDeliveryAddress,' '), " +
                                                    "PostAddress = persons.PostAddress, " +
                                                    "RecipientName = persons.RecipientName, " +
                                                    "DocumentsDeliveryMethod = persons.DocumentsDeliveryMethod, " +
                                                    "EmailForAccountingDocuments = persons.EmailForAccountingDocuments, " +
                                                    "AdditionalEmail = persons.AdditionalEmail, " +
                                                    "PersonResponsibleForDocuments = COALESCE(persons.PersonResponsibleForDocuments,' '), " +
                                                    "Phone = persons.Phone, " +
                                                    "PaymentEssentialElements = persons.PaymentEssentialElements, " +
                                                    "OwnerCode = persons.OwnerCode " +
                                                    "FROM Billing.LegalPersonProfiles profiles INNER JOIN Billing.LegalPersons persons ON profiles.LegalPersonId = persons.Id " +
                                                    "SET ANSI_WARNINGS ON ";

        private const string SetPriveleges = "DECLARE @LegalPersonEntityId int = 147 " +
                                             "DECLARE @UpdateProfilePrivelegeId int " +
                                             "SET @UpdateProfilePrivelegeId = (SELECT TOP 1 Id FROM [Security].[Privileges] where EntityType = 219 AND Operation = 2) " +
                                             "DECLARE @UpdateLegalPersonPrivelegeId int " +
                                             "SET @UpdateLegalPersonPrivelegeId = (SELECT TOP 1 Id FROM [Security].[Privileges] where EntityType = @LegalPersonEntityId AND Operation = 2) " +
                                             "INSERT INTO [Security].RolePrivileges(PrivilegeId, RoleId,Mask, Priority, CreatedOn, CreatedBy) " +
                                             "SELECT @UpdateProfilePrivelegeId, RoleId, Mask, 0, GETUTCDATE(), 1 FROM [Security].RolePrivileges " +
                                             "WHERE PrivilegeId = @UpdateLegalPersonPrivelegeId " +
                                             "DECLARE @ReadProfilePrivelegeId int " +
                                             "SET @ReadProfilePrivelegeId = (SELECT TOP 1 Id FROM [Security].[Privileges] where EntityType = 219 AND Operation = 1) " +
                                             "DECLARE @ReadLegalPersonPrivelegeId int " +
                                             "SET @ReadLegalPersonPrivelegeId = (SELECT TOP 1 Id FROM [Security].[Privileges] where EntityType = @LegalPersonEntityId AND Operation = 1) " +
                                             "INSERT INTO [Security].RolePrivileges(PrivilegeId, RoleId,Mask, Priority, CreatedOn, CreatedBy) " +
                                             "SELECT @ReadProfilePrivelegeId, RoleId, Mask, 0, GETUTCDATE(), 1 FROM [Security].RolePrivileges " +
                                             "WHERE PrivilegeId = @ReadLegalPersonPrivelegeId " +
                                             "DECLARE @CreatePrivelegeId int " +
                                             "SET @CreatePrivelegeId = (SELECT TOP 1 Id FROM [Security].[Privileges] where EntityType = 219 AND Operation = 32) " +
                                             "INSERT INTO [Security].RolePrivileges(PrivilegeId, RoleId,Mask, Priority, CreatedOn, CreatedBy) " +
                                             "SELECT @CreatePrivelegeId, Id, 16, 0, GETUTCDATE(), 1 FROM Security.Roles";

        private const string FillOrdersWithProfilesScript = "UPDATE Billing.Orders " +
                                                            "SET LegalPersonProfileId = profiles.Id " +
                                                            "FROM Billing.Orders orders " +
                                                            "INNER JOIN Billing.LegalPersonProfiles profiles ON orders.LegalPersonId = profiles.LegalPersonId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var currentTimeout = context.Connection.StatementTimeout;
            context.Connection.StatementTimeout = 1200;
            if (context.Database.Tables.Contains(LegalPersonProfileTable, LegalPersonProfileSchemaName)) 
                return;

            CreateLegalPersonProfilesTable(context.Database);
            FillOperatesOnBasisOf(context);
            context.Connection.ExecuteNonQuery(UpdateProfilesScript);
            context.Connection.ExecuteNonQuery(SetPriveleges);
                

            var ordersTable = context.Database.Tables[OrderTable, OrderSchemaName];
            AddLegalPersonProfileIdToOrderTable(ordersTable);
            context.Connection.ExecuteNonQuery(FillOrdersWithProfilesScript);

            var legalPersonsTable = context.Database.Tables[LegalPersonTable, LegalPersonSchemaName];
            RemoveColumnsFromLegalPersonsTable(legalPersonsTable);
            context.Connection.StatementTimeout = currentTimeout;
        }

        private static void FillOperatesOnBasisOf(IMigrationContext context)
        {
            #region Определение словарей
		            var certificateDefinitions = new List<string>
		                                             {
                                                          "свидетельсва",
                                                          "свидетельство",
                                                          "свидетльства",
                                                          "свидетельства",
                                                          "свидетнльства",
                                                          "свид",
                                                          "св-ва",
                                                          "св-во",
                                                          "свидетельста",
                                                          "свидельства",
                                                          "свидетелства",
                                                          "св-тво",
                                                          "свид-во",
                                                          "свид-ва",
                                                          "св.",
                                                          "свдетельства",
                                                          "видетельства",
                                                          "свид.",
                                                          "свиделельство",
                                                          "сведетельства",
                                                          "cвидетельства",
                                                          "ссвидетельства",
                                                          "свиддетельство",
                                                          "7свидетельства",
                                                          "свидетельсво",
                                                          "свидет-ва",
                                                          "cвид-ва",
                                                          "сидетельства",
                                                          "свидетельсво",
                                                          "свидетелиства",
                                                          "свидетельсво",
                                                          "св.о",
                                                          "свиде-ва",
                                                          "свидетедьства",
                                                          "свидительства",
                                                          "свитетельства",
                                                          "св-во:",
                                                          "свитдетельства",
                                                          "свидетельсвта",
                                                          "уссвидетельства",
                                                          "свидельство",
                                                          "свиретельства",
                                                          "свидедельство",
                                                          "cв-ва",
                                                          "свидетельствасерия",
                                                          "свидетельсвта",
                                                          "свидетельва",
                                                          "сви-ва",
                                                          "свидетальства",
                                                          "св-сва",
                                                          "свидет-во",
                                                          "свиделельства",
                                                          "сведетельсва",
                                                          "свидетельтсва",
                                                          "свиведетельства",
                                                          "свиделельства",
                                                          "свидетельчтва",
                                                          "свидетельтва",
                                                          "свитедельства",
                                                          "свидетельствава",
                                                          "свед-ва",
                                                          "свидетества",
                                                          "свидетельств",
                                                          "свидетиельства",
                                                          "свидетелсьва",
                                                          "свидетелества",
                                                          "св- ва",
                                                          "свуидетельства",
                                                          "свидетельсьво",
                                                          "свидедетельства",
                                                          "сводетельства",
                                                          "свтдетельства",
                                                          "свидетьельства",
                                                          "свидетельства22",
                                                          "сведетельство",
                                                          "сви-во",
                                                          "свидетьства"
                                                      };


            var warrantyDefinitions = new List<string>
                                          {
                                                       "доверенности",
                                                       "довереннсоти",
                                                       "доверенность",
                                                       "доверености",
                                                       "дверенности",
                                                       "доверенноти",
                                                       "доверен.",
                                                       "довер-ть",
                                                       "дов-ти",
                                                       "довереннности",
                                                       "довернности",
                                                       "довереннсти",
                                                       "доверенноси",
                                                       "ген.доверенности",
                                                       "дов.",
                                                       "довериности",
                                                       "доверинности",
                                                       "довереность"
                                                   };


            var charterDefinitions = new List<string>
                                         {
                                                      "устав",
                                                      "устава",
                                                      "устава.",
                                                      "уства"
                                                  };

            var seriesDefinitions = new List<string>
                                        {
                                                     "серия",
                                                     "сер",
                                                     "сер-я",
                                                     "серии"
                                                 };

            var numberDefinitions = new List<string>
                                        {
                                                     "№",
                                                     "номер"
                                                 };

            var digitDefinitions = new List<Char>
                                       {
                                                  '0',
                                                  '1',
                                                  '2',
                                                  '3',
                                                  '4',
                                                  '5',
                                                  '6',
                                                  '7',
                                                  '8',
                                                  '9',
                                                  '№'
                                              };

            var bargainDefinitions = new List<string>
                                          {
                                                       "договора",
                                                      "договор"
                                                   };

            var foundingDefinitions = new List<string>
                                          {
                                                       "учредительного"
                                                   };

            var months = new Dictionary<string, int>
                             {
                                 {" января ", 1},
                                 {" январь ", 1},
                                 {" февраля ", 2},
                                 {" февраль ", 2},
                                 {" марта ", 3},
                                 {" март ", 3},
                                 {" апреля ", 4},
                                 {" апрель ", 4},
                                 {" мая ", 5},
                                 {" май ", 5},
                                 {" июня ", 6},
                                 {" июнь ", 6},
                                 {" июля ", 7},
                                 {" июль ", 7},
                                 {" августа ", 8},
                                 {" август ", 8},
                                 {" сентября ", 9},
                                 {" сентябрь ", 9},
                                 {" октября ", 10},
                                 {" октябрь ", 10},
                                 {" ноября ", 11},
                                 {" ноябрь ", 11},
                                 {" декабря ", 12},
                                 {" декабрь ", 12}
                             };

            var profiles = new List<UpdateProfileInfo>();
	#endregion

            const string dateRegexpPattern = @"(0?[1-9]|[12][0-9]|3[01])[- ./](0?[1-9]|1[012])[- ./](19|20)?\d\d";

            var updateScriptBuilder = new StringBuilder();
            var reader =
                context.Connection.ExecuteReader(
                    "SELECT Id, OperatesOnTheBasisInGenitive, IsActive, IsDeleted, LegalPersonTypeEnum FROM Billing.LegalPersons");

            while (reader.Read())
            {
                #region Заполнение профиля
                var info = new UpdateProfileInfo(Convert.ToInt32(reader["Id"]), Convert.ToInt32(reader["IsActive"]), Convert.ToInt32(reader["IsDeleted"]));
                profiles.Add(info); 
                #endregion

                var currentType = OperatesOnTheBasisType.Underfined;
                var documentNumber = string.Empty;
                var documentDate = new DateTime();
                var documentEndDate = new DateTime();

                var hasDocumentError = false;
                var hasDateError = false;

                int legalPersonTypeEnum = Convert.ToInt32(reader["LegalPersonTypeEnum"]);
                var operatesOnTheBasisInGenitive = reader["OperatesOnTheBasisInGenitive"].ToString().ToLower();

                if (String.IsNullOrWhiteSpace(operatesOnTheBasisInGenitive) && legalPersonTypeEnum != 2 && info.IsActive == 1 && info.IsDeleted != 1)
                {
                    hasDateError = true;
                }
                else
                {
                    #region Определение типа документа

                    var words = operatesOnTheBasisInGenitive.Split(' ', ',', '№', '_', ':');
                    foreach (var word in words)
                    {
                        if (foundingDefinitions.Contains(word))
                        {
                            currentType = OperatesOnTheBasisType.FoundingBargain;
                            break;
                        }
                        if (bargainDefinitions.Contains(word))
                        {
                            currentType = OperatesOnTheBasisType.Bargain;
                            break;
                        }
                        if (certificateDefinitions.Contains(word))
                        {
                            currentType = OperatesOnTheBasisType.Certificate;
                            break;
                        }
                        if (charterDefinitions.Contains(word))
                        {
                            currentType = OperatesOnTheBasisType.Charter;
                            break;
                        }
                        if (warrantyDefinitions.Contains(word))
                        {
                            currentType = OperatesOnTheBasisType.Warranty;
                            break;
                        }
                    }
                    if (currentType == OperatesOnTheBasisType.Underfined)
                    {
                        hasDocumentError = true;
                    }

                    #endregion

                    #region Выделение даты

                    var dateLowerS =
                        operatesOnTheBasisInGenitive.Replace(@"""", "").Replace("«", "").Replace("»", "").Replace("  ",
                                                                                                                  " ");

                    foreach (var month in months)
                    {
                        dateLowerS = dateLowerS.Replace(month.Key, String.Format(".{0:00}.", month.Value));
                    }

                    var regex = new Regex(dateRegexpPattern);
                    var match = regex.Match(dateLowerS);

                    var dateAfterIndex = dateLowerS.IndexOf(" до ", StringComparison.Ordinal);
                    var dateCount = 0;
                    var firstDate = new DateTime();
                    var afterDate = new DateTime();

                    var operatesOnTheBasisInGenitiveForNumberParsing = dateLowerS;

                    while (match.Success)
                    {
                        dateCount++;
                        if (dateCount == 1)
                        {
                            hasDateError = !DateTime.TryParse(match.Value, out firstDate);
                        }

                        if (dateAfterIndex > -1 && match.Index > dateAfterIndex)
                        {
                            hasDateError = !DateTime.TryParse(match.Value, out afterDate);
                        }

                        operatesOnTheBasisInGenitiveForNumberParsing =
                            operatesOnTheBasisInGenitiveForNumberParsing.Replace(match.Value, "");

                        match = match.NextMatch();
                    }

                    if (!hasDocumentError)
                    {
                        switch (currentType)
                        {
                            case OperatesOnTheBasisType.FoundingBargain:
                            case OperatesOnTheBasisType.Charter:
                                break;
                            case OperatesOnTheBasisType.Certificate:
                                {
                                    if (dateCount != 1)
                                    {
                                        hasDateError = true;
                                    }

                                    if (!hasDateError)
                                    {
                                        documentDate = firstDate;
                                    }
                                    break;
                                }
                            case OperatesOnTheBasisType.Bargain:
                                {
                                    if (dateCount != 1 && dateCount != 2)
                                    {
                                        hasDateError = true;
                                    }

                                    if (!hasDateError)
                                    {
                                        documentDate = firstDate;
                                        if (dateCount == 2)
                                        {
                                            documentEndDate = afterDate;
                                        }
                                    }
                                    break;
                                }
                            case OperatesOnTheBasisType.Warranty:
                                {
                                    if (dateCount != 1 && dateCount != 2)
                                    {
                                        hasDateError = true;
                                    }

                                    if (!hasDateError)
                                    {
                                        documentDate = firstDate;
                                        if(dateCount==2)
                                        {
                                            documentEndDate = afterDate;
                                        }
                                    }
                                    break;
                                }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    #endregion

                    #region Выделение номера документа
                    var strings = operatesOnTheBasisInGenitiveForNumberParsing.Split(' ', ':', ',');
                    var isNumberParsing = false;

                    documentNumber = String.Empty;

                    foreach (var s1 in strings)
                    {
                        if (seriesDefinitions.Contains(s1))
                        {
                            isNumberParsing = false;
                        }
                        else if (numberDefinitions.Contains(s1) || s1.Contains("№"))
                        {
                            isNumberParsing = true;
                        }
                        else
                        {
                            var isDigitFound = s1.Any(ch => digitDefinitions.Contains(ch));
                            if (!isDigitFound)
                            {
                                isNumberParsing = false;
                            }
                            else if (!isNumberParsing)
                            {
                                isNumberParsing = true;
                            }
                        }

                        if (isNumberParsing)
                        {
                            documentNumber += " " + s1;
                        }
                    }

                    documentNumber = documentNumber.Trim();

                    if (String.IsNullOrWhiteSpace(documentNumber))
                    {
                        documentNumber = "Не задано";
                    }

                    #endregion
                }

                #region Составляем запрос на обновление профиля

                info.DocumentType = ((int) currentType).ToString(CultureInfo.InvariantCulture);
                if (!String.IsNullOrWhiteSpace(documentNumber))
                {
                    switch (currentType)
                    {
                        case OperatesOnTheBasisType.Certificate:
                            {
                                info.CertificateNumber = "'" + documentNumber + "'";
                                break;
                            }
                        case OperatesOnTheBasisType.Bargain:
                            {
                                info.BargainNumber = "'" + documentNumber + "'";
                                break;
                            }
                        case OperatesOnTheBasisType.Warranty:
                            {
                                info.WarrantyNumber = "'" + documentNumber + "'";
                                break;
                            }
                    }
                }

                if (!hasDateError && 
                    (documentDate != default(DateTime) || 
                        (documentEndDate != default(DateTime) && 
                            (currentType == OperatesOnTheBasisType.Warranty || currentType == OperatesOnTheBasisType.Bargain))))
                {
                    switch (currentType)
                    {
                        case OperatesOnTheBasisType.Certificate:
                            {
                                info.CertificateDate = "'" + documentDate.ToString("yyyyMMdd") + "'";
                                break;
                            }
                        case OperatesOnTheBasisType.Bargain:
                            {
                                if (documentDate != default(DateTime))
                                {
                                    info.BargainBeginDate = "'" + documentDate.ToString("yyyyMMdd") + "'";
                                }
                                if (documentEndDate != default(DateTime))
                                {
                                    info.BargainEndDate = "'" + documentEndDate.ToString("yyyyMMdd") + "'";
                                }
                                break;
                            }
                        case OperatesOnTheBasisType.Warranty:
                            {
                                if (documentDate != default(DateTime))
                                {
                                    info.WarrantyBeginDate = "'" + documentDate.ToString("yyyyMMdd") + "'";
                                }
                                if (documentEndDate != default(DateTime))
                                {
                                    info.WarrantyEndDate = "'" + documentEndDate.ToString("yyyyMMdd") + "'";
                                }
                                break;
                            }
                    }
                }

                #endregion
            }
            reader.Close();

            #region Генерация скрипта

            updateScriptBuilder.AppendLine("SET ANSI_WARNINGS OFF ");

            updateScriptBuilder.AppendLine(
                "INSERT INTO Billing.LegalPersonProfiles (Name, LegalPersonId, PositionInNominative, PositionInGenitive, ChiefNameInNominative, ChiefNameInGenitive, OperatesOnTheBasisInGenitive, CertificateNumber, CertificateDate, WarrantyNumber, WarrantyBeginDate, WarrantyEndDate, BargainNumber, BargainBeginDate, BargainEndDate, DocumentsDeliveryAddress, PostAddress, RecipientName, DocumentsDeliveryMethod, EmailForAccountingDocuments, AdditionalEmail, PersonResponsibleForDocuments, Phone, PaymentEssentialElements, IsDeleted, IsActive, OwnerCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn, IsMainProfile)");
            updateScriptBuilder.AppendLine("Values ");
            int loopLimit = profiles.Count;
            for (int i = 0; i < loopLimit;i++ )
            {
                if((i+1)%1000==0)
                {
                    updateScriptBuilder.AppendLine(
                "INSERT INTO Billing.LegalPersonProfiles (Name, LegalPersonId, PositionInNominative, PositionInGenitive, ChiefNameInNominative, ChiefNameInGenitive, OperatesOnTheBasisInGenitive, CertificateNumber, CertificateDate, WarrantyNumber, WarrantyBeginDate, WarrantyEndDate, BargainNumber, BargainBeginDate, BargainEndDate, DocumentsDeliveryAddress, PostAddress, RecipientName, DocumentsDeliveryMethod, EmailForAccountingDocuments, AdditionalEmail, PersonResponsibleForDocuments, Phone, PaymentEssentialElements, IsDeleted, IsActive, OwnerCode, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn, IsMainProfile)");
                    updateScriptBuilder.AppendLine("Values ");
                }
                updateScriptBuilder.Append(profiles[i].ToInsertEmptyValues());
                if (i != loopLimit - 1 && (i + 2) % 1000 != 0)
                {
                    updateScriptBuilder.Append(",");
                }
                updateScriptBuilder.AppendLine();
            }
            updateScriptBuilder.AppendLine("INSERT INTO Security.Privileges VALUES (219, 1), (219, 2), (219, 32)");
            updateScriptBuilder.AppendLine("SET ANSI_WARNINGS ON ");
    
            #endregion
           
            context.Connection.ExecuteNonQuery(updateScriptBuilder.ToString());
        }

        private static void CreateLegalPersonProfilesTable(Database database)
        {
            var table = new Table(database,
                                  LegalPersonProfileTable,
                                  LegalPersonProfileSchemaName);
            table.Columns.Add(new Column(table, "Id", DataType.Int)
                                  {
                                      Nullable = false,
                                      Identity = true,
                                      IdentityIncrement = 1,
                                      IdentitySeed = 1
                                  });
            table.Columns.Add(new Column(table, "Name", DataType.NVarChar(256)) {Nullable = false});
            table.Columns.Add(new Column(table, "LegalPersonId", DataType.Int) {Nullable = false});
            table.Columns.Add(new Column(table, "IsMainProfile", DataType.Bit) {Nullable = false});
            table.Columns.Add(new Column(table, "PositionInNominative", DataType.NVarChar(256)) {Nullable = true});
            table.Columns.Add(new Column(table, "PositionInGenitive", DataType.NVarChar(256)) {Nullable = true});
            table.Columns.Add(new Column(table, "ChiefNameInNominative", DataType.NVarChar(256)) {Nullable = true});
            table.Columns.Add(new Column(table, "ChiefNameInGenitive", DataType.NVarChar(256)) {Nullable = true});
            table.Columns.Add(new Column(table, "OperatesOnTheBasisInGenitive", DataType.Int) {Nullable = true});
            table.Columns.Add(new Column(table, "CertificateNumber", DataType.NVarChar(50)) {Nullable = true});
            table.Columns.Add(new Column(table, "CertificateDate", DataType.DateTime2(2)) {Nullable = true});
            table.Columns.Add(new Column(table, "WarrantyNumber", DataType.NVarChar(50)) {Nullable = true});
            table.Columns.Add(new Column(table, "WarrantyBeginDate", DataType.DateTime2(2)) { Nullable = true });
            table.Columns.Add(new Column(table, "WarrantyEndDate", DataType.DateTime2(2)) {Nullable = true});
            table.Columns.Add(new Column(table, "BargainNumber", DataType.NVarChar(50)) { Nullable = true });
            table.Columns.Add(new Column(table, "BargainBeginDate", DataType.DateTime2(2)) { Nullable = true });
            table.Columns.Add(new Column(table, "BargainEndDate", DataType.DateTime2(2)) { Nullable = true });
            table.Columns.Add(new Column(table, "DocumentsDeliveryAddress", DataType.NVarChar(512)) {Nullable = true});
            table.Columns.Add(new Column(table, "PostAddress", DataType.NVarChar(512)) {Nullable = true});
            table.Columns.Add(new Column(table, "RecipientName", DataType.NVarChar(256)) {Nullable = true});
            table.Columns.Add(new Column(table, "DocumentsDeliveryMethod", DataType.Int) {Nullable = false});
            table.Columns.Add(new Column(table, "EmailForAccountingDocuments", DataType.NVarChar(64)) {Nullable = true});
            table.Columns.Add(new Column(table, "AdditionalEmail", DataType.NVarChar(64)) {Nullable = true});
            table.Columns.Add(new Column(table, "PersonResponsibleForDocuments", DataType.NVarChar(256))
                                  {Nullable = false});
            table.Columns.Add(new Column(table, "Phone", DataType.NVarChar(50)) {Nullable = true});
            table.Columns.Add(new Column(table, "PaymentEssentialElements", DataType.NVarChar(200)) {Nullable = true});
            table.Columns.Add(new Column(table, "IsDeleted", DataType.Bit) {Nullable = false});
            table.Columns.Add(new Column(table, "IsActive", DataType.Bit) {Nullable = false});
            table.Columns.Add(new Column(table, "OwnerCode", DataType.Int) {Nullable = false});
            table.Columns.Add(new Column(table, "CreatedBy", DataType.Int) {Nullable = false});
            table.Columns.Add(new Column(table, "ModifiedBy", DataType.Int) {Nullable = true});
            table.Columns.Add(new Column(table, "CreatedOn", DataType.DateTime2(2)) {Nullable = false});
            table.Columns.Add(new Column(table, "ModifiedOn", DataType.DateTime2(2)) {Nullable = true});
            table.Columns.Add(new Column(table, "Timestamp", DataType.Timestamp) {Nullable = true});

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("LegalPersonId", new SchemaQualifiedObjectName("Billing", "LegalPersons"), "Id");
        }

        private static void AddLegalPersonProfileIdToOrderTable(Table orderTable)
        {
            if (!orderTable.Columns.Contains("LegalPersonProfileId"))
            {
                var profileIdColumn = new Column(orderTable, "LegalPersonProfileId", DataType.Int) {Nullable = true};

                orderTable.Columns.Add(profileIdColumn);
                orderTable.Alter();
                orderTable.CreateForeignKey("LegalPersonProfileId",
                                            new SchemaQualifiedObjectName("Billing", "LegalPersonProfiles"), "Id");
            }
        }

        private static void RemoveColumnsFromLegalPersonsTable(Table legalPersonsTable)
        {
            var areThereAnyChanges = false;

            if (legalPersonsTable.Columns.Contains("PositionInNominative"))
            {
                var column = legalPersonsTable.Columns["PositionInNominative"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("PositionInGenitive"))
            {
                var column = legalPersonsTable.Columns["PositionInGenitive"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("ChiefNameInNominative"))
            {
                var column = legalPersonsTable.Columns["ChiefNameInNominative"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("ChiefNameInGenitive"))
            {
                var column = legalPersonsTable.Columns["ChiefNameInGenitive"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("OperatesOnTheBasisInGenitive"))
            {
                var column = legalPersonsTable.Columns["OperatesOnTheBasisInGenitive"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("DocumentsDeliveryAddress"))
            {
                var column = legalPersonsTable.Columns["DocumentsDeliveryAddress"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("PostAddress"))
            {
                var column = legalPersonsTable.Columns["PostAddress"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("RecipientName"))
            {
                var column = legalPersonsTable.Columns["RecipientName"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("DocumentsDeliveryMethod"))
            {
                var column = legalPersonsTable.Columns["DocumentsDeliveryMethod"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("EmailForAccountingDocuments"))
            {
                var column = legalPersonsTable.Columns["EmailForAccountingDocuments"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("AdditionalEmail"))
            {
                var column = legalPersonsTable.Columns["AdditionalEmail"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("PersonResponsibleForDocuments"))
            {
                var column = legalPersonsTable.Columns["PersonResponsibleForDocuments"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("Phone"))
            {
                var column = legalPersonsTable.Columns["Phone"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if (legalPersonsTable.Columns.Contains("PaymentEssentialElements"))
            {
                var column = legalPersonsTable.Columns["PaymentEssentialElements"];
                column.Drop();
                areThereAnyChanges = true;
            }

            if(areThereAnyChanges)
            {
                legalPersonsTable.Alter();
            }
        }

        /// <summary>
        /// Действует на основании...
        /// </summary>
        public enum OperatesOnTheBasisType
        {
            Underfined = 0,
            /// <summary>
            /// Устав
            /// </summary>
            Charter = 1,
            /// <summary>
            /// Сертификат
            /// </summary>
            Certificate = 2,
            /// <summary>
            /// Доверенность
            /// </summary>
            Warranty = 3,
            /// <summary>
            /// Учредительный договор
            /// </summary>
            FoundingBargain = 4,
            /// <summary>
            /// Договор
            /// </summary>
            Bargain = 5
        }

        private class UpdateProfileInfo
        {
            public UpdateProfileInfo(int legalPersonId, int isActive, int isDeleted)
            {
                IsDeleted = isDeleted;
                IsActive = isActive;
                LegalPersonId = legalPersonId;
                DocumentType = "NULL";
                CertificateDate = "NULL";
                CertificateNumber = "NULL";
                WarrantyNumber = "NULL";
                WarrantyBeginDate = "NULL";
                WarrantyEndDate = "NULL";
                BargainBeginDate = "NULL";
                BargainEndDate = "NULL";
                BargainNumber = "NULL";
            }

            private int LegalPersonId { get; set; }
            
            public string DocumentType { private get; set; }
            public string CertificateNumber { private get; set; }
            public string CertificateDate { private get; set; }
            public string WarrantyNumber { private get; set; }
            public string WarrantyBeginDate { private get; set; }
            public string WarrantyEndDate { private get; set; }

            public string BargainNumber { private get; set; }
            public string BargainBeginDate { private get; set; }
            public string BargainEndDate { private get; set; }

            public int IsActive { get; private set; }
            public int IsDeleted { get; private set; }

            public string ToInsertEmptyValues()
            {
                return String.Format(
                    "({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31})",
                    "' '", LegalPersonId, "' '",
                    "' '", "' '", "' '", DocumentType,
                    CertificateNumber, CertificateDate, WarrantyNumber, WarrantyBeginDate, WarrantyEndDate, BargainNumber, BargainBeginDate, BargainEndDate,
                    "' '", "' '", "' '", "1",
                    "' '", "' '", "' '", "' '",
                    "' '", "0", "1", "1", "1", "Null", "GETUTCDATE()", "Null", "1");
            }
        }
    }
}
