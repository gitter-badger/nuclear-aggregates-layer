-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--   29.05.2014, a.gutorov: Добавления условия при Update, чтобы заменялись только различающиеся значения.
--   18.09.2014, a.tukaev: fix ERM-4939 - поддержка отложенной репликации
ALTER PROCEDURE [Integration].[CalculateClientPromising]
    @ModifiedBy BIGINT = NULL
    WITH EXECUTE AS CALLER
AS
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ClientUpdatedIds TABLE ( Id BIGINT PRIMARY KEY )

    BEGIN TRY
        BEGIN TRANSACTION;	

        WITH    ClientStats ( Id, MaxPromisingScore )
                  AS ( SELECT   C.Id,
                                MAX(F.PromisingScore)
                       FROM     Billing.Clients AS C
                                INNER JOIN BusinessDirectory.Firms AS F ON F.ClientId = C.Id
                       GROUP BY C.Id
                     )
            UPDATE  Billing.Clients
            SET     PromisingValue = ClientStats.MaxPromisingScore,
                    ModifiedBy = @ModifiedBy,
                    ModifiedOn = GETUTCDATE()
            OUTPUT  inserted.Id
                    INTO
			@ClientUpdatedIds
            FROM    Billing.Clients
                    INNER JOIN ClientStats ON Clients.Id = ClientStats.Id
            WHERE   Clients.PromisingValue <> ClientStats.MaxPromisingScore

		-- отчет для вызывающего кода об измененных сущностях
        DECLARE @EntityName_Client INT = 200
        DECLARE @ChangeType_Updated INT = 3
        DECLARE @ChangedEntities TABLE
            (
              Id BIGINT NOT NULL,
              EntityName INT NOT NULL,
              ChangeType INT NOT NULL
            )

        INSERT  INTO @ChangedEntities
                SELECT  c.Id AS Id,
                        @EntityName_Client AS EntityName,
                        @ChangeType_Updated AS ChangeType
                FROM    @ClientUpdatedIds c

        SELECT  *
        FROM    @ChangedEntities

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF ( XACT_STATE() != 0 )
            BEGIN
                ROLLBACK TRANSACTION;
            END;

        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT  @ErrorMessage = ERROR_MESSAGE(),
                @ErrorSeverity = ERROR_SEVERITY(),
                @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;


