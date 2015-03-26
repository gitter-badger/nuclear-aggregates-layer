		--создаем не созданные записи организаторов в ERM
		INSERT INTO [Activity].[AppointmentReferences]([AppointmentId],[Reference],[ReferencedType],[ReferencedObjectId])
		SELECT 
			[a1].[id] as [AppointmentId],
			2 as Reference,
			53 as ReferencedType, 	
			[a1].[OwnerCode] as [ReferencedObjectId]
		FROM [Activity].[AppointmentBase] a1 
        LEFT OUTER JOIN [Activity].[AppointmentReferences] a2
        ON [a1].[Id] = [a2].[AppointmentId] and [a2].[Reference] = 2
        WHERE [a2].[AppointmentId] is NULL

		--добавляет в базу MSCRM записи организоторов, которые не были добавлены
		INSERT INTO [{0}].[dbo].[ActivityPartyBase]  ([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
		SELECT 
			NEWID() as [ActivityPartyId],
			[a].[ReplicationCode] as [ActivityId],
			7 as [ParticipationTypeMask],
			8 as [PartyObjectTypeCode], 
			[crmOwners].[SystemUserId] as [PartyId], 
			[owners].[DisplayName] as [PartyIdName]
		FROM [Activity].[AppointmentBase] a 
		JOIN [Security].[Users] [owners] ON [owners].[Id] = [a].[OwnerCode]
		LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
		LEFT OUTER JOIN [{0}].[dbo].[ActivityPartyBase] r ON ( [r].[ActivityId] = [a].[ReplicationCode] and [ParticipationTypeMask] = 7)
		INNER JOIN  [{0}].[dbo].[ActivityPointerBase] apointer ON ([apointer].[ActivityId] = [a].[ReplicationCode])
		WHERE [r].[ActivityId] is NULL