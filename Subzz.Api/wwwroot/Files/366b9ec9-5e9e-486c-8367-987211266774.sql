USE [Subzz_Leaves]
GO
ALTER TABLE [Absence].[Absence]
ADD AbsenceResendCounter INT;

GO
Alter table [Absence].[Absence]
Add AcceptedVia nvarchar(50)

GO
USE [Subzz_Users]
GO
INSERT INTO [Users].[Resource] (Name, StateMachine, Css, ResourceType_Id, ParentResource_Id, IsAdminPortal)
VALUES ('School Files', '/schoolFiles', 'schoolFiles', 2, -1, 0); 

GO
INSERT INTO [Users].[RolePrivilege] (RoleId, PrivilegeId, ResourceId)
VALUES	(1, 3, 35),
		(2, 3, 35); 
		
GO		
SET IDENTITY_INSERT [Users].[FileType] on
insert into [Users].[FileType] (FileType_Id, FileType_Name)
values (6, 'School Files');

GO
CREATE TABLE [Users].[Announcement](
	[AnnouncementId] [int] IDENTITY(1,1) NOT NULL,
	[RecipientId] [varchar](50) NULL,
	[District_Id] [int] NULL,
	[School_Id] [varchar] (5) NULL,
	[Title] [nvarchar](500) NULL,
	[Message] [nvarchar](max) NULL,
	[ScheduleAnnouncement] [bit] NULL,
	[ShowOn] [bit] NULL,
	[HideOn] [bit] NULL,
	[ShowOnDate] [date] NULL,
	[HideOnDate] [date] NULL,
	[ShowOnTime] [time](7) NULL,
	[HideOnTime] [time](7) NULL,
	[IsDeleted] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL, 
)

GO
/****** Object:  StoredProcedure [Users].[CreateAnnouncement]    Script Date: 8/6/2019 5:31:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		@Muhammad Adnan Ali
-- Create date: @08/06/2019
-- Description:	@Create Announcement
-- =============================================

-- Exec [Users].[CreateAnnouncement] 

Create PROCEDURE [Users].[CreateAnnouncement]
	@AnnouncementId			INT,
	@Recipients				INT,
	@DistrictId				INT,
	@OrganizationId			VARCHAR(5),
	@Title					NVARCHAR(500),
	@Message				NVARCHAR(MAX),
	@ScheduleAnnouncement	BIT,
	@ShowOn					BIT,
	@HideOn					BIT,
	@ShowOnDate				DATE,
	@HideOnDate				DATE,
	@ShowOnTime				TIME,
	@HideOnTime				TIME
AS
BEGIN
	INSERT INTO [Users].[Announcement]
	(
	   [RecipientId]
      ,[District_Id]
	  ,[School_Id]
      ,[Title]
      ,[Message]
      ,[ScheduleAnnouncement]
      ,[ShowOn]
      ,[HideOn]
      ,[ShowOnDate]
      ,[HideOnDate]
      ,[ShowOnTime]
      ,[HideOnTime]
      ,[IsDeleted]
      ,[CreatedDate]
	)
	VALUES
	(
		@Recipients,
		@DistrictId,
		@OrganizationId,
		@Title,
		@Message,
		@ScheduleAnnouncement,
		@ShowOn,
		@HideOn,
		@ShowOnDate,
		@HideOnDate,
		@ShowOnTime,
		@HideOnTime,
		0,
		GETDATE()
	)
	SELECT SCOPE_IDENTITY() AS ReturnText	
		
END 

GO
/****** Object:  StoredProcedure [Users].[GetAnnouncement]    Script Date: 8/8/2019 12:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Adnan Ali>
-- Create date: <08/06/2019>
-- Description:	<To Get Announcement>
-- =============================================
CREATE PROCEDURE [Users].[GetAnnouncement] 

AS
BEGIN

	SELECT 
	   [AnnouncementId]
      ,[RecipientId]
      ,[District_Id]
	  ,[School_Id]
      ,[Title]
      ,[Message]
      ,[ScheduleAnnouncement]
      ,[ShowOn]
      ,[HideOn]
      ,[ShowOnDate]
      ,[HideOnDate]
      ,[ShowOnTime]
      ,[HideOnTime]
      ,[IsDeleted]
      ,[CreatedDate]
      ,[ModifiedDate]
	FROM [Subzz_Users].[Users].[Announcement]

END	