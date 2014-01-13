using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11348, "Импорт данных OrderPositions.CategoryRate из отчётов за сентябрь-октябрь 2013")]
    public sealed class Migration11348 : TransactedMigration
    {
        private const int BatchSize = 1000; // Число строк, добавляемой в одном запросе InsertStatement. Взято с потолка.
        private const string DataForAugustResourceName = "DoubleGis.Erm.BLCore.DB.Migrations.Resources.2013.08.21 rev. 11348 - september 2013.xml";
        
        private const string CreateTable = @"
if OBJECT_ID('Billing.OrdersTemporary') is not null drop table Billing.OrdersTemporary
if OBJECT_ID('Billing.GetOrders') is not null drop function Billing.GetOrders
if OBJECT_ID('Billing.GetOrderId') is not null drop function Billing.GetOrderId

CREATE TABLE Billing.OrdersTemporary
	(
	OrderId bigint NOT NULL,
	OrderNumber nvarchar(50) NOT NULL,
	CategoryRate decimal(19, 4) NOT NULL
	)  ON [PRIMARY]
GO

CREATE FUNCTION Billing.GetOrders
(
	@orderNumber nvarchar(200),
	@categoryRate decimal(19, 4)
)
RETURNS TABLE
AS
return (
	select Id, Number, @categoryRate as CategoryRate
	from Billing.Orders
	where Number = @orderNumber 
		and IsDeleted = 0 
		and IsActive = 1
)
GO
";

        private const string UpdateStaement = @"
update [Billing].OrderPositions
set OrderPositions.CategoryRate = OrdersTemporary.CategoryRate
FROM [Billing].OrdersTemporary
	inner join [Billing].Orders on Orders.Id = OrdersTemporary.OrderId
	inner join [Billing].OrderPositions on OrderPositions.OrderId = Orders.Id
	inner join [Billing].PricePositions on PricePositions.Id = OrderPositions.PricePositionId
where PricePositions.RatePricePositions = 1
	and OrderPositions.CategoryRate <> OrdersTemporary.CategoryRate
";

        private const string DropStatement = @"
DROP TABLE Billing.OrdersTemporary
DROP FUNCTION Billing.GetOrders
";

        private const string InsertStatement = @"insert into Billing.OrdersTemporary(OrderId, OrderNumber, CategoryRate) select Id, Number, CategoryRate from Billing.GetOrders('{0}', {1})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(CreateTable);

            var dataAugust = ReadResource(DataForAugustResourceName).ToArray();

            var queryItems = new List<string>(dataAugust.Length);

            foreach (var orderCategoryRates in dataAugust)
            {
                var orderNumber = orderCategoryRates.Key;
                var categoryRate = orderCategoryRates.Value;

                if (orderNumber.Contains('\''))
                {
                    throw new Exception(string.Format("Order number {0} contains invalid characters", orderNumber));
                }

                queryItems.Add(string.Format(InsertStatement, orderNumber, categoryRate.ToString(CultureInfo.InvariantCulture)));
            }

            for (var i = 0; i < queryItems.Count; i += BatchSize)
            {
                var query = string.Join("; ", queryItems.Skip(i).Take(BatchSize));
                try
                {
                    context.Database.ExecuteNonQuery(query);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Insert failed: {0}", query), e);
                }
            }

            context.Database.ExecuteNonQuery(UpdateStaement);
            context.Database.ExecuteNonQuery(DropStatement);
        }

        private IDictionary<string, decimal> ReadResource(string resourceName)
        {
            var stream = GetType().Assembly.GetManifestResourceStream(resourceName);
            var errorMessages = new List<string>();

            try
            {
                var document = XElement.Load(stream);
                var orders = document.Elements("Order");

                var xmlItems = orders
                    .Select(order =>
                        {
                            var orderNumber = order.Attribute("Number").Value;
                            var categoryRate = order.Attribute("CategoryRate").Value;
                            return new Tuple<string, string>(orderNumber, categoryRate);
                        })
                    .ToArray();

                // TODO {a.rechkalov, 31.07.2013}: Я правильно понимаю, что можно сгруппировать выборку по Number, затем выбрать
                //                                 единственное значение CategoryRate, отличное от 1, либо (если их несколько) кинуть Exception?
                // COMMENT {a.tukaev, 2013-08-01}: Правильно, только ещё придётся ещё брать Distinct по CategoryRate, поскольку если их будет несколько одинаковых - это не ошибка

                // .ToDictionary не использую, поскольку есть обработка случаев, когда ключ уже существует в словаре
                var result = new Dictionary<string, decimal>(xmlItems.Length);
                foreach (var item in xmlItems)
                {
                    // округление до трех знаков после запятой: в excel-файлах знаке в 16-м есть погрешность, избавляемся от её влияния
                    var categoryRate = string.IsNullOrWhiteSpace(item.Item2)
                        ? 1
                        : Math.Round(decimal.Parse(item.Item2, CultureInfo.InvariantCulture), 3);
                    decimal existingCategoryRate;
                    if (!result.TryGetValue(item.Item1, out existingCategoryRate))
                    {
                        result.Add(item.Item1, categoryRate);
                    }
                    else if (categoryRate != existingCategoryRate && categoryRate == 1)
                    {
                        result[item.Item1] = existingCategoryRate;
                    }
                    else if (categoryRate != existingCategoryRate && existingCategoryRate == 1)
                    {
                        result[item.Item1] = categoryRate;
                    }
                    else if (categoryRate != existingCategoryRate)
                    {
                        var message = string.Format("Order {0} was used with other CategoryGroup value: {1} vs {2}",
                                                    item.Item1,
                                                    existingCategoryRate,
                                                    categoryRate);
                        errorMessages.Add(message);
                    }
                }

                if (errorMessages.Any())
                {
                    throw new Exception(string.Join(". ", errorMessages));
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error while processing {0}", resourceName), e);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }
    }
}
