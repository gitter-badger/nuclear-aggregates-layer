SELECT
	[Куратор]										=	u.DisplayName
	, [Оплаты по ПДЗ до 01.12]					=	ISNULL(t.DZBefore1210,0)
	, [Оплаты по новым продажам]					=	ISNULL(t.PaymentNew,0)
	, [Оплаты прочие]								=	ISNULL(t.PaymentOther,0)
	, [Невыясненные оплаты]							=	ISNULL(t.PaymentUnknown,0)
	, [Оплаченный прирост без учета расторжений] 	=	ISNULL(g.Growth,0)
	, [Оплаченный прирост с учетом расторжений]		=	ISNULL(g.GrowthBreak,0)
	, [Оплаченный прирост по продлениям]			=	ISNULL(g.GrowthBreakProlong,0)
	, [Оплаченный прирост по новым]					=	ISNULL(g.GrowthBreakNew,0)
FROM
	(
		SELECT
			Curator
			, DZBefore1210 = SUM(DZBefore1210)
			, PaymentNew = SUM(PaymentNew)
			, PaymentOther = SUM(PaymentCont+PaymentProlong+PaymentOther-[Returns]+DZAfter1210)
			, PaymentUnknown = SUM(PaymentUnknown)
		FROM ##PaymentDistribution
		GROUP BY Curator
	) t
	FULL JOIN(
		SELECT
			Curator
			, Growth = SUM(Growth)
			, GrowthBreak = SUM(GrowthBreak)
			, GrowthBreakProlong = SUM(CASE WHEN Category = 3 THEN GrowthBreak ELSE 0 END)
			, GrowthBreakNew = SUM(CASE WHEN Category = 2 THEN GrowthBreak ELSE 0 END)
		FROM ##PaymentGrowth
		GROUP BY Curator
	) g ON
		t.Curator = g.Curator
	JOIN Security.Users u ON
		u.Id = ISNULL(t.Curator, g.Curator)
ORDER BY u.DisplayName