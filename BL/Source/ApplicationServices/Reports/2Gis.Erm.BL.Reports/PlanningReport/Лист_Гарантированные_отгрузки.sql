--DECLARE
--	@IssueDate date = '20130901'
--	, @City int = 1

SELECT
	[Куратор] = mgrs.DisplayName
	, [Фирма] = f.Name
	, [№ Бланк-заказа] = o.Number
	, [К оплате (план)] = o.PayablePlan
	, [Начало размещения] = o.BeginDistributionDate
	, [Окончание размещения] = o.EndDistributionDatePlan
	, [На срок] = o.ReleaseCountPlan
	, [К оплате(план)/срок] = ot.AmountToWithdraw

FROM
	Billing.Orders o with(nolock)
	JOIN Billing.BranchOfficeOrganizationUnits AS boou WITH (NOLOCK) ON boou.Id = o.BranchOfficeOrganizationUnitId
	JOIN Billing.OrderReleaseTotals ot with(nolock) ON
		ot.OrderId = o.Id
	LEFT JOIN Security.Users mgrs with(nolock) ON
		o.OwnerCode = mgrs.Id
	LEFT JOIN BusinessDirectory.Firms f with(nolock) ON
		o.FirmId = f.Id
	LEFT JOIN Billing.Clients c with(nolock) ON
		c.id = f.ClientId
WHERE
	boou.OrganizationUnitId = @City
	AND o.CreatedOn < @IssueDate
	AND o.BeginDistributionDate = DATEADD(m, 1, @IssueDate)
	AND ot.ReleaseBeginDate <= DATEADD(m, 1, @IssueDate)
	AND ot.ReleaseEndDate > DATEADD(m, 1, @IssueDate)
	AND o.IsDeleted = 0
	AND o.PayablePlan > 0
	AND o.WorkflowStepId = 5
	AND ((@IsAdvertisingAgency = 1 AND c.IsAdvertisingAgency = 1) OR @IsAdvertisingAgency = 0)
ORDER BY
	mgrs.DisplayName
	, f.Name