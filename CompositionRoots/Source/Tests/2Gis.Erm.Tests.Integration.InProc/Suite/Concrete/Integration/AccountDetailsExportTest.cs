using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Integration
{
    public sealed class AccountDetailsExportTest : IIntegrationTest
    {
        private readonly IGetDebitsInfoInitialForExportOperationService _getDebitsInfoInitialForExportOperationService;

        public AccountDetailsExportTest(IGetDebitsInfoInitialForExportOperationService getDebitsInfoInitialForExportOperationService)
        {
            _getDebitsInfoInitialForExportOperationService = getDebitsInfoInitialForExportOperationService;
        }

        public ITestResult Execute()
        {
            var startDate = new DateTime(2014, 11, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            var resultNew = _getDebitsInfoInitialForExportOperationService.Get(startDate, endDate, new long[] { 1 });

            //var regionalOld = resultOld.Debits.Where(x => x.Type == DebitDto.DebitType.Regional).ToArray();
            
            //resultOld.Debits = resultOld.Debits.OrderBy(x => x.AccountCode).ThenBy(x => x.OrderCode).ToArray();
            //resultNew.Debits = resultNew.Debits.OrderBy(x => x.AccountCode).ThenBy(x => x.OrderCode).ToArray();

            //var oldXml = resultOld.ToXElement().ToString();
            //var newXml = resultNew.ToXElement().ToString();

            //var errors = CheckProperties(resultOld, resultNew, dto => dto.ClientDebitTotalAmount, dto => dto.StartDate, dto => dto.EndDate, dto => dto.OrganizationUnitCode);
            //if (errors.Any())
            //{
            //    throw new InvalidOperationException(string.Join(";", errors));
            //}

            //if (oldXml != newXml)
            //{
            //    throw new InvalidOperationException();
            //}

            return OrdinaryTestResult.As.Succeeded;
        }

        private IEnumerable<string> CheckProperties<T>(T oldData, T newData, params Expression<Func<T, object>>[] propertyExpressions)
            where T : class
        {
            var errorsList = new List<string>();
            foreach (var propertyExpression in propertyExpressions)
            {
                var propertyName = StaticReflection.GetMemberName(propertyExpression);
                var func = propertyExpression.Compile();
                var oldValue = func.Invoke(oldData);
                var newValue = func.Invoke(newData);

                if (oldValue != newValue && (oldValue == null || !oldValue.Equals(newValue)))
                {
                    errorsList.Add(string.Format("Ошибка в свойстве {0}. Значение в старом варианте: {1}. Значение в новом: {2}.",
                                                 propertyName,
                                                 oldValue,
                                                 newValue));
                }
            }

            return errorsList;
        }
    }
}
