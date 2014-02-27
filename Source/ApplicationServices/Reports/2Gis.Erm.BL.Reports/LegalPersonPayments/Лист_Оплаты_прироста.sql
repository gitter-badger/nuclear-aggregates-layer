SELECT
	[Название] = N''
	, [Фирма] = f.Name
	, [ЮЛ клиента] = lp.LegalName
	, [ЮЛ исполнителя] = boou.ShortLegalName
	, [Город назначения] = dou.Name
	, [Город источник] = sou.Name
	, [Номер БЗ] = o.Number
	, [К оплате (факт)] = CAST(CASE WHEN o.ReleaseCountPlan = 0 THEN 0 ELSE o.PayablePlan / o.ReleaseCountPlan * o.ReleaseCountFact END AS MONEY)
	, [К оплате (факт)/на срок (факт)] = oc.AmountPlanPeriod
	, [Срок размещения (факт)] = o.ReleaseCountFact
	, [Начало размещения] = CAST(o.BeginDistributionDate AS DATE)
	, [Окончание размещения (факт)] = CAST(o.EndDistributionDateFact AS DATE)
	, [Сумма расторжения] = oc.[Break]
	, [Статус БЗ] =
					CASE o.WorkflowStepId
						WHEN 1 THEN N'На оформлении'
						WHEN 2 THEN N'На утверждении'
						WHEN 3 THEN N'Отклонено'
						WHEN 4 THEN N'На расторжении'
						WHEN 5 THEN N'Одобрен'
						WHEN 6 THEN N'Архив'
					END
	, [ФИО МПП] = u.DisplayName
	, [Тип заказа] = 
					CASE
						WHEN oc.Category = 2 THEN N'Новая'
						WHEN oc.Category = 3 THEN N'Продление'
						WHEN oc.Category IN (4,5,6) THEN N'Прочая'
					END
	, [Оплаченный прирост без учета расторжений] = oc.Growth
	, [Оплаченный прирост с учетом расторжений] = oc.GrowthBreak
FROM 
	##PaymentGrowth oc
	JOIN Billing.Orders o ON
		o.Id = oc.OrderID
	JOIN Billing.LegalPersons lp ON
		o.LegalPersonId = lp.Id
	JOIN Billing.BranchOfficeOrganizationUnits boou ON
		o.BranchOfficeOrganizationUnitId = boou.Id
	JOIN BusinessDirectory.Firms f ON
		o.FirmId = f.Id
	JOIN Billing.OrganizationUnits sou ON
		o.SourceOrganizationUnitId = sou.Id
	JOIN Billing.OrganizationUnits dou ON
		o.DestOrganizationUnitId = dou.Id
	JOIN Security.Users u ON
		u.Id = o.OwnerCode
ORDER BY u.DisplayName, lp.LegalName