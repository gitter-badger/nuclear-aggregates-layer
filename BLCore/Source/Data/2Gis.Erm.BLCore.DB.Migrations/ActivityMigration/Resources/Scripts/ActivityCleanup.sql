SET NOCOUNT ON;
DECLARE @rows INT;

/*
    4201 : Appointment
    4202 : Email
    4204 : Fax
    4207 : Letter
    4210 : PhoneCall
    4212 : Task
    4214 : ServiceAppointment
    4401 : CampaignResponse
*/

-- чистим примечания
BEGIN TRANSACTION;
DELETE FROM [dbo].[AnnotationBase] WHERE [ObjectTypeCode] in (4201, 4202, 4204, 4207, 4210, 4212, 4214, 4401);
COMMIT TRANSACTION;

-- чистим связи с внешними объектами
SET @rows = 1;
WHILE @rows > 0
BEGIN
    BEGIN TRANSACTION;
    DELETE TOP (100000) FROM [dbo].[ActivityPartyBase] WHERE [ActivityId] in (SELECT ActivityId FROM [dbo].[AppointmentBase]);
    SET @rows = @@ROWCOUNT;
    COMMIT TRANSACTION;
END

SET @rows = 1;
WHILE @rows > 0
BEGIN
    BEGIN TRANSACTION;
    DELETE TOP (100000) FROM [dbo].[ActivityPartyBase] WHERE [ActivityId] in (SELECT ActivityId FROM [dbo].[EmailBase]);
    SET @rows = @@ROWCOUNT;
    COMMIT TRANSACTION;
END

SET @rows = 1;
WHILE @rows > 0
BEGIN
    BEGIN TRANSACTION;
    DELETE TOP (100000) FROM [dbo].[ActivityPartyBase] WHERE [ActivityId] in (SELECT ActivityId FROM [dbo].[FaxBase]);
    SET @rows = @@ROWCOUNT;
    COMMIT TRANSACTION;
END

SET @rows = 1;
WHILE @rows > 0
BEGIN
    BEGIN TRANSACTION;
    DELETE TOP (100000) FROM [dbo].[ActivityPartyBase] WHERE [ActivityId] in (SELECT ActivityId FROM [dbo].[LetterBase]);
    SET @rows = @@ROWCOUNT;
    COMMIT TRANSACTION;
END

SET @rows = 1;
WHILE @rows > 0
BEGIN
    BEGIN TRANSACTION;
    DELETE TOP (100000) FROM [dbo].[ActivityPartyBase] WHERE [ActivityId] in (SELECT ActivityId FROM [dbo].[PhonecallBase]);
    SET @rows = @@ROWCOUNT;
    COMMIT TRANSACTION;
END

SET @rows = 1;
WHILE @rows > 0
BEGIN
    BEGIN TRANSACTION;
    DELETE TOP (100000) FROM [dbo].[ActivityPartyBase] WHERE [ActivityId] in (SELECT ActivityId FROM [dbo].[TaskBase]);
    SET @rows = @@ROWCOUNT;
    COMMIT TRANSACTION;
END

SET @rows = 1;
WHILE @rows > 0
BEGIN
    BEGIN TRANSACTION;
    DELETE TOP (100000) FROM [dbo].[ActivityPartyBase] WHERE [ActivityId] in (SELECT ActivityId FROM [dbo].[ServiceAppointmentBase]);
    SET @rows = @@ROWCOUNT;
    COMMIT TRANSACTION;
END

SET @rows = 1;
WHILE @rows > 0
BEGIN
    BEGIN TRANSACTION;
    DELETE TOP (100000) FROM [dbo].[ActivityPartyBase] WHERE [ActivityId] in (SELECT ActivityId FROM [dbo].[CampaignResponseBase]);
    SET @rows = @@ROWCOUNT;
    COMMIT TRANSACTION;
END
                
-- удаляем декомпозированные действия
BEGIN TRANSACTION;
TRUNCATE TABLE [dbo].[AppointmentExtensionBase]
TRUNCATE TABLE [dbo].[AppointmentBase]
TRUNCATE TABLE [dbo].[EmailExtensionBase]
TRUNCATE TABLE [dbo].[EmailBase]
TRUNCATE TABLE [dbo].[FaxExtensionBase]
TRUNCATE TABLE [dbo].[FaxBase]
TRUNCATE TABLE [dbo].[LetterExtensionBase]
TRUNCATE TABLE [dbo].[LetterBase]
TRUNCATE TABLE [dbo].[PhonecallExtensionBase]
TRUNCATE TABLE [dbo].[PhonecallBase]
TRUNCATE TABLE [dbo].[TaskExtensionBase]
TRUNCATE TABLE [dbo].[TaskBase]
TRUNCATE TABLE [dbo].[ServiceAppointmentExtensionBase]
TRUNCATE TABLE [dbo].[ServiceAppointmentBase]
TRUNCATE TABLE [dbo].[CampaignResponseExtensionBase]
TRUNCATE TABLE [dbo].[CampaignResponseBase]
COMMIT TRANSACTION;

-- чистим объекты, связанные с электронной почтой
TRUNCATE TABLE [dbo].[EmailHashBase]

-- чистим вложения для emails
TRUNCATE TABLE [dbo].[ActivityMimeAttachment] 

-- чистим обобщенную таблицу
SET @rows = 1;
WHILE @rows > 0
BEGIN
  BEGIN TRANSACTION;
 
  DELETE TOP (100000)
    FROM [dbo].[ActivityPointerBase] WHERE [ActivityTypeCode] in (4201, 4202, 4204, 4207, 4210, 4212, 4214, 4401)
 
  SET @rows = @@ROWCOUNT;
 
  COMMIT TRANSACTION;
END