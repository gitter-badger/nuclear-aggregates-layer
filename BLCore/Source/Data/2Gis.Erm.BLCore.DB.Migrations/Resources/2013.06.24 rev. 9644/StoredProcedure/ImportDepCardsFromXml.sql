-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
CREATE PROCEDURE [Integration].[ImportDepCardsFromXml]
    @XmlCardTbl Integration.CardsTableType READONLY,
	@XmlContactDtos  Integration.ContactDTOsTableType READONLY,
	@ModifiedBy bigint = NULL
AS 
BEGIN

SET XACT_ABORT ON;

BEGIN TRY 

BEGIN TRAN

MERGE Integration.DepCards AS CurrentValues
USING @XmlCardTbl AS NewValues 
ON (CurrentValues.Code = NewValues.CardCode) 
WHEN MATCHED THEN 
UPDATE SET CurrentValues.IsHiddenOrArchived = NewValues.IsCardHiddenOrArchived 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Code, IsHiddenOrArchived) 
VALUES (NewValues.CardCode, NewValues.IsCardHiddenOrArchived);

MERGE BusinessDirectory.FirmContacts AS CurrentValues
USING @XmlContactDtos AS NewValues
ON CurrentValues.CardId = NewValues.CardCode AND CurrentValues.SortingPosition = NewValues.SortingPosition
WHEN MATCHED THEN 
UPDATE SET CurrentValues.ContactType = NewValues.ContactTypeId,
CurrentValues.Contact = NewValues.Contact,
CurrentValues.ModifiedBy = @ModifiedBy,
CurrentValues.ModifiedOn = GETUTCDATE()
WHEN NOT MATCHED BY TARGET THEN 
INSERT (CardId, Contact, ContactType, CreatedBy, CreatedOn, SortingPosition)
VALUES (NewValues.CardCode, NewValues.Contact, NewValues.ContactTypeId, @ModifiedBy, GETUTCDATE(), NewValues.SortingPosition);

DELETE FROM BusinessDirectory.FirmContacts WHERE Id in 
(
SELECT currentContacts.Id FROM BusinessDirectory.FirmContacts as currentContacts left join @XmlContactDtos as NewValues ON  NewValues.CardCode = currentContacts.CardId AND NewValues.SortingPosition = currentContacts.SortingPosition
WHERE currentContacts.CardId in (SELECT DISTINCT CardCode FROM @XmlCardTbl) AND NewValues.CardCode is NULL
)

COMMIT TRAN

END TRY
BEGIN CATCH
	IF (XACT_STATE() != 0)
		ROLLBACK TRAN

	DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT
	SELECT @ErrorMessage = ERROR_MESSAGE(),	@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
END



