--declare @IssueDate date = '20130901' 
--		,@City int = 1
--		,@IsAdvertisingAgency int = 0

DECLARE
	@DZCheckPoint date = '20141201'

SELECT
	[Куратор юр.лица клиента] = u.DisplayName
	, [Юр.лицо заказчика] = lp.LegalName
	, [ДЗ до 01.12.2014] = ABS([data].DZ)
	, [Оплаты с 01.12.2014] = [data].Payments
	, [Остаток ДЗ, накопленного до 01.12.2014] = CASE WHEN [data].DZ + [data].Payments >= 0 THEN 0 ELSE ABS([data].DZ + [data].Payments) END
	, [Корректировка РГ] = NULL
	, [Примечание] = NULL
FROM
	(
		SELECT
			[AccountId] = a.Id
			, [DZ] =
					ISNULL(SUM(CASE WHEN ISNULL(l.PeriodStartDate, ad.TransactionDate) < @DZCheckPoint THEN ABS(ad.Amount)*(2*ot.IsPlus-1) ELSE 0 END), 0)
			, [Payments] = 
					ISNULL(SUM(CASE WHEN ad.TransactionDate >= @DZCheckPoint AND ad.TransactionDate <= @IssueDate AND ot.Id NOT IN (7,13,14) THEN ABS(ad.Amount)*(2*ot.IsPlus-1) ELSE 0 END), 0)
		FROM
			Billing.Accounts a with(nolock)
			JOIN Billing.AccountDetails ad with(nolock) ON
				a.Id = ad.AccountId
			LEFT JOIN Billing.Locks l with(nolock) ON
				l.DebitAccountDetailId = ad.Id
				AND l.IsDeleted = 0
				AND l.IsActive = 0
			JOIN Billing.BranchOfficeOrganizationUnits bou with(nolock) ON
				a.BranchOfficeOrganizationUnitId = bou.Id
			JOIN Billing.OperationTypes ot with(nolock) ON
				ad.OperationTypeId = ot.Id
		WHERE
			bou.OrganizationUnitId = @City
			AND ad.TransactionDate < @IssueDate
			AND a.IsDeleted = 0
			AND ad.IsDeleted = 0
		GROUP BY
			a.Id
	) DATA
	JOIN Billing.Accounts AS a ON a.id = DATA.AccountId
	JOIN Billing.LegalPersons lp with(nolock) ON
		a.LegalPersonId = lp.Id
	LEFT JOIN Billing.Clients c with(nolock) ON
		c.id = lp.ClientId
	JOIN Security.Users u with(nolock) ON
		u.Id = lp.OwnerCode
WHERE
	[data].DZ < 0 		AND ((@IsAdvertisingAgency = 1 AND c.IsAdvertisingAgency = 1) OR @IsAdvertisingAgency = 0) AND lp.IsDeleted = 0
ORDER BY 
	u.DisplayName
	, lp.LegalName