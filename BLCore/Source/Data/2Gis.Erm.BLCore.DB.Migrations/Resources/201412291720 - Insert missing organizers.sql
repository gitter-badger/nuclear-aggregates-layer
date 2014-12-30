		--добавляет в базу MSCRM записи организоторов, которые не были добавлены
		MERGE INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase] as [Current]
		USING (
		SELECT 
			NEWID() as [ActivityPartyId],
			[a].[ReplicationCode] as [ActivityId],
			7 as [ParticipationTypeMask],
			8 as [PartyObjectTypeCode], 
			[crmOwners].[SystemUserId] as [PartyId], 
			[owners].[DisplayName] as [PartyIdName]
		FROM [Activity].[AppointmentBase] a 
		JOIN [Security].[Users] [owners] ON [owners].[Id] = [a].[OwnerCode]
					LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
		WHERE not exists (SELECT * FROM [Activity].[AppointmentReferences] r WHERE [r].[AppointmentId] = [a].[Id] and [Reference] = 2)
		) as [Modified]
		ON [Current].[ActivityId]=[Modified].[ActivityId] and [Current].[ParticipationTypeMask]=7
		WHEN NOT MATCHED BY TARGET THEN
			INSERT ([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
			VALUES ([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
			;

		--создаем не созданные записи организаторов в ERM
		MERGE INTO [Activity].[AppointmentReferences] as [Current]
		USING (
		SELECT 
	
			[a].[id] as [AppointmentId],
			2 as [Reference],
			53 as [ReferencedType], 	
			[a].[OwnerCode] as [ReferencedObjectId]
		FROM [Activity].[AppointmentBase] a 
		) as [Modified]
		ON [Current].[AppointmentId]=[Modified].[AppointmentId] and [Current].[Reference]=2
		WHEN NOT MATCHED BY TARGET THEN
			INSERT ([AppointmentId],[Reference],[ReferencedType],[ReferencedObjectId])
			VALUES ([AppointmentId],[Reference],[ReferencedType],[ReferencedObjectId])
			;
