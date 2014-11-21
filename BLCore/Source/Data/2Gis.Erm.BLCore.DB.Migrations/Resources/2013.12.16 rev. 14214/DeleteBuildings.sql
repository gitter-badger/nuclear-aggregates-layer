-- changes
--   16.12.2013, a.rechkalov: создана хранимка, поскольку через EF невозможно работать с объектом Building (нет Id)
CREATE PROCEDURE [Integration].[DeleteBuildings]
       @codesXml [xml]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @docHandle INT
	EXEC sp_xml_preparedocument @docHandle OUTPUT, @codesXml

	DECLARE @xmlCodes TABLE (Code BIGINT NOT NULL)
	INSERT INTO @xmlCodes SELECT Code FROM OPENXML(@docHandle, N'/root/code', 1) WITH (Code BIGINT '.')

	update [Integration].[Buildings] set IsDeleted = 1 where Code in (select Code from @xmlCodes)
END
