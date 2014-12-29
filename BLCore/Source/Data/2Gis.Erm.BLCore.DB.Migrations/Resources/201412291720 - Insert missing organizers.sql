	SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

		--добавляет в базу MSCRM записи организоторов, которые не были добавлены
		MERGE INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase] as [Current]
		USING (
		SELECT 
			NEWID() as ActivityPartyId,
			a.ReplicationCode as ActivityId,
			7 as ParticipationTypeMask,
			8 as PartyObjectTypeCode, 
			[crmOwners].[SystemUserId] as [PartyId], 
			[owners].[DisplayName] as [PartyIdName]
		FROM [Activity].[AppointmentBase] a 
		JOIN [Security].[Users] [owners] ON [owners].[Id] = [a].[OwnerCode]
					LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
		WHERE not exists (SELECT * FROM [Activity].[AppointmentReferences] r WHERE r.AppointmentId = a.Id and Reference = 2)
		) as [Modified]
		ON [Current].ActivityId=[Modified].ActivityId and [Current].ParticipationTypeMask=7
		WHEN NOT MATCHED BY TARGET THEN
			INSERT ([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
			VALUES ([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
			;

		--создаем не созданные записи организаторов в ERM
		MERGE INTO Activity.AppointmentReferences as [Current]
		USING (
		SELECT 
	
			a.id as [AppointmentId],
			2 as Reference,
			53 as ReferencedType, 	
			[owners].[Id] as [ReferencedObjectId]
		FROM Activity.AppointmentBase a 
		JOIN [Security].[Users] [owners] ON [owners].[Id] = [a].[OwnerCode]			
		) as [Modified]
		ON [Current].[AppointmentId]=[Modified].[AppointmentId] and [Current].[Reference]=2
		WHEN NOT MATCHED BY TARGET THEN
			INSERT ([AppointmentId],[Reference],[ReferencedType],[ReferencedObjectId])
			VALUES ([AppointmentId],[Reference],[ReferencedType],[ReferencedObjectId])
			;

		COMMIT TRAN;

    END TRY
    BEGIN CATCH
        IF ( XACT_STATE() != 0 )
            BEGIN
                ROLLBACK TRAN;
            END;

        DECLARE @ErrorMessage NVARCHAR(MAX),
            @ErrorSeverity INT,
            @ErrorState INT;

        SELECT  @ErrorMessage = ERROR_MESSAGE(),
                @ErrorSeverity = ERROR_SEVERITY(),
                @ErrorState = ERROR_STATE();

        RAISERROR ( @ErrorMessage , @ErrorSeverity , @ErrorState );
    END CATCH;