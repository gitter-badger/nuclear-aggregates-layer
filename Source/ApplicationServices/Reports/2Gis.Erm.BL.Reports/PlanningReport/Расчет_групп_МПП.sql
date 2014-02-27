IF(OBJECT_ID('tempdb..##Users') IS NOT NULL)
	DROP TABLE ##Users

SELECT DISTINCT
	UserId = mpp.Id
	, [GmId] = gm.GmId
	, [IsGM] = CONVERT(BIT, CASE WHEN mpp.Id = gm.GmId THEN 1 ELSE 0 END)
INTO ##Users
FROM
	(
		SELECT DISTINCT
			[GmId] = u1.Id
			, [DepartmentId] = d1.Id
		FROM 
			Security.Users u1
			JOIN Security.UserRoles us with(nolock) ON --Добавил Гуторов Алексей 29.08.2012
				us.UserId = u1.Id  --Добавил Гуторов Алексей 29.08.2012
			--JOIN Security.Users u2 ON    --Закоментил Гуторов Алексей 29.08.2012
			--	u2.ParentId = u1.Id        --Закоментил Гуторов Алексей 29.08.2012
			--	AND u1.DepartmentId = u2.DepartmentId   --Закоментил Гуторов Алексей 29.08.2012
			JOIN Security.UserOrganizationUnits uou with(nolock) ON
				uou.UserId = u1.Id
				AND uou.OrganizationUnitId = @City
			JOIN Security.Departments d1 with(nolock) ON
				u1.DepartmentId = d1.Id -- ПЛОХО Гуторов Алексей 29.08.2012
			LEFT JOIN Security.Departments d2 with(nolock) ON
				d1.ParentId = d2.Id
				AND d2.IsDeleted = 0
		WHERE
			u1.IsActive = 1
			AND u1.IsDeleted = 0
			--AND d2.Id IS NULL -- ПЛОХО Гуторов Алексей 29.08.2012
			AND d1.IsActive = 1
			AND d1.IsDeleted = 0
			AND us.RoleId = 4 --Добавил Гуторов Алексей 29.08.2012
			AND u1.DepartmentId > 1 --Добавил Гуторов Алексей 29.08.2012
	) gm 
	JOIN Security.Users mpp with(nolock) ON
		mpp.DepartmentId = gm.DepartmentId
	LEFT JOIN Billing.LegalPersons lp with(nolock) ON
		lp.OwnerCode = mpp.Id
		AND lp.IsDeleted = 0
WHERE
	(
		mpp.Id = gm.GmId
		OR (mpp.ParentId = gm.GmId AND lp.Id IS NOT NULL)
	)
ORDER BY
	2, 3 DESC, 1