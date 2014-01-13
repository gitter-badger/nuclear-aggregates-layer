using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Import
{
    public sealed class ImportFlowRubricsHandler : RequestHandler<ImportFlowRubricsRequest, EmptyResponse>
    {
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IClientProxyFactory _clientProxyFactory;

        private readonly ICommonLog _logger;
        private readonly ICategoryService _categoryService;

        public ImportFlowRubricsHandler(
            ICommonLog logger,
            ICategoryService categoryService,
            IIntegrationSettings integrationSettings,
            IClientProxyFactory clientProxyFactory)
        {
            _logger = logger;
            _categoryService = categoryService;
            _integrationSettings = integrationSettings;
            _clientProxyFactory = clientProxyFactory;
        }

        protected override EmptyResponse Handle(ImportFlowRubricsRequest request)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>("NetTcpBinding_IBrokerApiReceiver");

            clientProxy.Execute(brokerApiReceiver =>
            {
                brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, "flowRubrics");

                try
                {
                    while (true)
                    {
                        var package = brokerApiReceiver.ReceivePackage();
                        if (package == null)
                        {
                            _logger.InfoEx("Импорт рубрик - шина пустая");
                            break;
                        }

                        _logger.InfoFormatEx("Импорт рубрик - загружено {0} объектов из шины", package.Length);
                        if (package.Length != 0)
                        {
                            ProcessPackageInTransaction(package, request.BasicLanguage, request.ReserveLanguage);
                        }

                        brokerApiReceiver.Acknowledge();
                    }
                }
                finally
                {
                    brokerApiReceiver.EndReceiving();                    
                }
            });

            return Response.Empty;
        }

        private static CategoryImportServiceBusDto ReadCategory(string xml, string basicLanguage, string reserveLanguage)
        {
            var document = XDocument.Parse(xml);
            var element = document.Root;

            if (element == null)
            {
                throw new NotificationException("Unexpected empty xml");
            }

            if (element.Name != "Rubric")
            {
                throw new NotificationException(string.Format("Unexpected element '{0}', expected '{1}'", element.Name, "Rubric"));
            }

            var category = new CategoryImportServiceBusDto();

            var code = element.Attribute("Code");
            if (code == null || string.IsNullOrEmpty(code.Value))
            {
                throw new NotificationException();
            }

            category.Id = long.Parse(code.Value);

            var isDeleted = element.Attribute("IsDeleted");
            category.IsDeleted = isDeleted != null && !string.IsNullOrEmpty(isDeleted.Value) && bool.Parse(isDeleted.Value);

            if (category.IsDeleted)
            {
                return category;
            }

            category.ParentId = (long?)element.Attribute("ParentCode");
            category.Level = (int)element.Attribute("Level");

            var isHidden = element.Attribute("IsHidden");
            category.IsHidden = isHidden != null && bool.Parse(isHidden.Value);

            var name =
                element
                    .Elements("Names")
                    .Elements("Name")
                    .FirstOrDefault(x => string.Equals(basicLanguage, x.Attribute("Lang").Value, StringComparison.InvariantCultureIgnoreCase)) ??
                element
                    .Elements("Names")
                    .Elements("Name")
                    .FirstOrDefault(x => string.Equals(reserveLanguage, x.Attribute("Lang").Value, StringComparison.InvariantCultureIgnoreCase)) ??
                element
                    .Elements("Names")
                    .Elements("Name")
                    .FirstOrDefault();

            if (name == null)
            {
                throw new BusinessLogicException(BLResources.RubricDoesntContainNameTemplate);
            }

            category.Name = name.Attribute("Value").Value;
            category.Comment = string.Join("; ",
                                           element
                                               .Elements("Groups")
                                               .Elements("Group")
                                               .Elements("Comments")
                                               .Elements("Comment")
                                               .Where(
                                                   x => string.Equals(basicLanguage, x.Attribute("Lang").Value, StringComparison.InvariantCultureIgnoreCase))
                                               .Select(ValueAttribute));

            if (string.IsNullOrWhiteSpace(category.Comment))
            {
                category.Comment = string.Join("; ",
                                               element
                                                   .Elements("Groups")
                                                   .Elements("Group")
                                                   .Elements("Comments")
                                                   .Elements("Comment")
                                                   .Where(
                                                       x =>
                                                       string.Equals(reserveLanguage, x.Attribute("Lang").Value, StringComparison.InvariantCultureIgnoreCase))
                                                   .Select(ValueAttribute));
            }

            category.OrganizationUnitsDgppIds = element
                .Elements("Groups")
                .Elements("Group")
                .Elements("Branches")
                .Elements("Branch")
                .Select(CodeAttribute)
                .ToArray();

            return category;
        }

        private static string ValueAttribute(XElement element)
        {
            // Атрибут обязательный по xsd, если его не будет, то пусть лучше упадет.
            var value = element.Attribute("Value");
            return value.Value; 
        }

        private static int CodeAttribute(XElement element)
        {
            // Атрибут обязательный по xsd, если его не будет, то пусть лучше упадет.
            var code = element.Attribute("Code");
            return int.Parse(code.Value);
        }

        private void ProcessPackageInTransaction(IEnumerable<string> dataObjects, string basicLanguage, string reserveLanguage)
        {
            var categories = dataObjects.Select(x => ReadCategory(x, basicLanguage, reserveLanguage)).ToArray();
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                ProcessPackage(categories);
                transaction.Complete();
            }
        }

        private void ProcessPackage(CategoryImportServiceBusDto[] categories)
        {
            var context = new CategoryImportContext();
            foreach (var dto in categories)
            {
                Validate(dto);
            }

            ImportCategories(categories, context);
            ImportOrganizationUnitLinks(categories, context);
        }

        private void ImportCategories(IEnumerable<CategoryImportServiceBusDto> categories, CategoryImportContext context)
        {
            // рубрики импортируются слоями, начиная с самого верхнего, 
            // так как дочерние рубрики могут ссылаться на те, которые ранее не существовали и пришли только в этом пакете.
            var categoriesByLevel = categories.GroupBy(dto => dto.Level).OrderBy(group => group.Key); // todo: продумать кейс с удалением рубрики второго уровня (что должно происходить, если есть подчинённые рубрики третьего уровня?)
            foreach (var level in categoriesByLevel)
            {
                _categoryService.ImportCategoryLevel(level, context);
            }
        }

        private void ImportOrganizationUnitLinks(IEnumerable<CategoryImportServiceBusDto> categories, CategoryImportContext context)
        {
            // реально подразделения указываются только для рубрик третьего уровня,
            // а для всех родительских они вычисляются объединением всех дочерних.
            _categoryService.ImportOrganizationUnits(categories.Where(cat => cat.Level == 3), context);

            foreach (var level in new[] { 2, 1 })
            {
                _categoryService.FixAffectedCategories(level, context);
            }
        }

        private void Validate(CategoryImportServiceBusDto category)
        {
            // Don't need to validate deleted categories
            if (category.IsDeleted)
            {
                return;
            }

            if (category.Level < 1 || category.Level > 3)
            {
                throw CreateBusinessException(
                    "Ошибка при импорте рубрики с Id={0}: уровень рубрики {1} находится за пределами допустимого диапазона (1, 2, 3).",
                    category.Id,
                    category.Level);
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw CreateBusinessException("Ошибка при импорте рубрики с Id={0}: название рубрики не должно быть пустым.", category.Id);
            }

            if (category.Level > 1 && category.ParentId == null)
            {
                throw CreateBusinessException("Ошибка при импорте рубрики с Id={0}: только рубрики первого уровня могут иметь ParentId=null",
                                              category.Id);
            }
        }

        private BusinessLogicException CreateBusinessException(string message, params object[] parameters)
        {
            return new BusinessLogicException(string.Format(message, parameters));
        }
    }
}
