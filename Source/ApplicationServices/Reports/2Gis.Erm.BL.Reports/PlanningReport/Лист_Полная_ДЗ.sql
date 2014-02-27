--DECLARE
--	@IssueDate date = '20120301'
--	, @City int = 1

SELECT
	[Куратор юр.лица] = u.DisplayName
	, [Юр.лицо заказчика] = lp.LegalName
	, [Полная дебиторская задолженность] = -(ISNULL(b.Balance,0) - ISNULL(lck.Lock, 0))
FROM
	Billing.LegalPersons lp with(nolock)
	JOIN Security.Users u with(nolock) ON
		u.Id = lp.OwnerCode
			LEFT JOIN Billing.Clients c with(nolock) ON
		c.id = lp.ClientId
	JOIN Billing.Accounts a with(nolock) ON
		a.LegalPersonId = lp.Id
		AND a.IsDeleted = 0
	JOIN Billing.BranchOfficeOrganizationUnits bou with(nolock) ON
		a.BranchOfficeOrganizationUnitId = bou.Id
	LEFT JOIN(
		SELECT
			ad.AccountId
			, [Balance] = SUM(ABS(ad.Amount)*(2*ot.IsPlus-1))
		FROM
			Billing.AccountDetails ad with(nolock)
			LEFT JOIN Billing.Locks l with(nolock) ON
				l.DebitAccountDetailId = ad.Id
				AND l.IsDeleted = 0
				AND l.IsActive = 0
			JOIN Billing.OperationTypes ot with(nolock) ON
				ot.Id = ad.OperationTypeId
		WHERE
			(
				(ad.OperationTypeId <> 7 AND ad.TransactionDate < @IssueDate)
				OR (ad.OperationTypeId = 7 AND l.PeriodStartDate <= @IssueDate)
			)
			AND ad.IsDeleted = 0
		GROUP BY
			ad.AccountId
	) b ON
		b.AccountId = a.Id
	LEFT JOIN(
		SELECT
			l.AccountId
			, [Lock] = SUM(l.PlannedAmount)
		FROM Billing.Locks l with(nolock)
		WHERE
			l.PeriodStartDate <= @IssueDate
			and l.IsDeleted = 0
			AND l.IsActive = 1
		GROUP BY
			l.AccountId
	) lck ON
		lck.AccountId = a.Id
WHERE
	bou.OrganizationUnitId = @City
	AND ISNULL(b.Balance,0) - ISNULL(lck.Lock, 0) < 0
	AND lp.IsDeleted = 0	AND ((@IsAdvertisingAgency = 1 AND c.IsAdvertisingAgency = 1) OR @IsAdvertisingAgency = 0)
ORDER BY 
	u.DisplayName
	, lp.LegalName