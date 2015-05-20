--DECLARE
--	@issuedate date = '20120301'
--	, @city int = 1

DECLARE
	@DZCheckPoint date = '20141201'

SELECT
	[Куратор] = u.DisplayName
	, [Юр.лицо заказчика] = lp.LegalName
	, [Юр.лицо исполнителя] = bou.ShortLegalName
	, [Баланс лицевого счета] = 
				--a.Balance - ISNULL(lck.Lock, 0) + CASE WHEN a.DZ1201 < 0 THEN -a.DZ1201 ELSE 0 END
				CASE
					WHEN a.Balance >= 0 THEN 0
					WHEN a.DZ1201 + a.Payments >= 0 THEN a.Balance
					WHEN a.Balance >= a.DZ1201 + a.Payments THEN 0
					ELSE a.Balance - (a.DZ1201 + a.Payments)
				END
	, [Корректировка РГ] = NULL
	, [Примечание] = NULL
	, [Последнее размещение] = ord.EndDistributionDateFact
FROM
	(
		SELECT
			a.LegalPersonId
			, a.BranchOfficeOrganizationUnitId
			, a.OwnerCode
			, [Balance] = ISNULL(SUM(ABS(ad.Amount)*(2*ot.IsPlus-1)),0)
			, [DZ1201] =
					SUM(
						CASE
							WHEN ad.OperationTypeId = 7 AND l.PeriodStartDate < @DZCheckPoint THEN ABS(ad.Amount)*(2*ot.IsPlus-1)
							WHEN ad.OperationTypeId <> 7 AND ad.TransactionDate < @DZCheckPoint THEN ABS(ad.Amount)*(2*ot.IsPlus-1)
							ELSE 0
						END
					)
			, [Payments] = 
					ISNULL(
						SUM(
							CASE 
								WHEN 
									ad.TransactionDate >= @DZCheckPoint
									AND ad.TransactionDate < @IssueDate
									AND ot.Id NOT IN (7,13,14) 
								THEN 
									ABS(ad.Amount)*(2*ot.IsPlus-1) 
								ELSE 0 
							END
						)
					, 0)

		FROM
			Billing.Accounts a with(nolock)
			LEFT JOIN Billing.AccountDetails ad with(nolock) ON
				ad.AccountId = a.Id
				AND ad.IsDeleted = 0
			LEFT JOIN Billing.Locks l with(nolock) ON
				l.DebitAccountDetailId = ad.Id
				AND l.IsDeleted = 0
				AND l.IsActive = 0
			LEFT JOIN Billing.OperationTypes ot with(nolock) ON
				ot.Id = ad.OperationTypeId
			LEFT JOIN (
				SELECT DISTINCT o.AccountId
				FROM 
					Billing.Orders o
					JOIN Billing.BranchOfficeOrganizationUnits AS boou WITH (NOLOCK) ON boou.Id = o.BranchOfficeOrganizationUnitId
				WHERE
					o.EndDistributionDateFact > @IssueDate
					AND o.IsDeleted = 0
					AND o.IsActive = 1
					AND o.WorkflowStepId NOT IN (1,2)
					AND o.PayablePlan > 0
					AND boou.OrganizationUnitId = @City
			) o ON
				o.AccountId = a.Id
		WHERE
			CASE
				WHEN ad.Id IS NULL THEN DATEADD(d, 0, 0) --это чтобы лицевые счета без операций не потерялись
				WHEN ad.OperationTypeId = 7 THEN DATEADD(n, -1, l.PeriodStartDate) -- отнимаем минуту, чтобы выполнялось строгое неравенство
				ELSE ad.TransactionDate 
			END < @IssueDate
			AND a.IsDeleted = 0
			AND o.AccountId IS NULL
		GROUP BY
			a.LegalPersonId
			, a.BranchOfficeOrganizationUnitId
			, a.OwnerCode
	) a
	LEFT JOIN(
		SELECT
			a.LegalPersonId
			, [Lock] = SUM(l.PlannedAmount)
		FROM Billing.Locks l with(nolock)
			JOIN Billing.Accounts a with(nolock) ON
				l.AccountId = a.Id
				AND a.IsDeleted = 0
		WHERE
			l.PeriodStartDate <= @IssueDate
			and l.IsDeleted = 0
			AND l.IsActive = 1
		GROUP BY
			a.LegalPersonId
	) lck ON
		lck.LegalPersonId = a.LegalPersonId
	JOIN Billing.LegalPersons lp with(nolock) ON
		a.LegalPersonId = lp.Id
	LEFT JOIN Billing.Clients c with(nolock) ON
		c.id = lp.ClientId

		AND lp.IsDeleted = 0
	JOIN Security.Users u with(nolock) ON
		u.Id = a.OwnerCode
	JOIN Billing.BranchOfficeOrganizationUnits bou with(nolock) ON
		a.BranchOfficeOrganizationUnitId = bou.Id
	LEFT JOIN (
	
			SELECT o.LegalPersonId, o.BranchOfficeOrganizationUnitId, MAX(o.EndDistributionDateFact) AS EndDistributionDateFact
			FROM Billing.Orders AS o 
			WHERE o.IsActive = 1 AND o.IsDeleted = 0 AND o.WorkflowStepId IN (4,5,6) AND CONVERT(date,o.EndDistributionDateFact)< @IssueDate
			GROUP BY o.LegalPersonId, o.BranchOfficeOrganizationUnitId
) AS ord ON bou.Id = ord.BranchOfficeOrganizationUnitId AND lp.Id = ord.LegalPersonId

WHERE
	bou.OrganizationUnitId = @City	AND ((@IsAdvertisingAgency = 1 AND c.IsAdvertisingAgency = 1) OR @IsAdvertisingAgency = 0)
	AND CASE
			WHEN a.Balance >= 0 THEN 0
			WHEN a.DZ1201 + a.Payments >= 0 THEN -a.Balance
			WHEN a.Balance >= a.DZ1201 + a.Payments THEN 0
			ELSE -( a.Balance - (a.DZ1201 + a.Payments))
		END > 0
ORDER BY 
	u.DisplayName
	, lp.LegalName