USE [Subzz_Settings]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		@Shahzada Ghazanfar
-- Create date: @06/02/2019
-- Description:	@Get Version Update
-- =============================================

-- Exec [Settings].[GetLatestVersion] 

CREATE PROCEDURE [Settings].[GetLatestVersion]
	
AS
BEGIN
	
	SELECT 
	TOP 1 
		[Id]
      ,[Version]
      ,[Features]
      ,[ContentDetails]
  FROM [Subzz_Settings].[Settings].[Versions]
  ORDER BY 1 DESC 
END  

-- 
USE [Subzz_Users]
GO
CREATE TABLE [Users].[Events](
	[EventId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](10) NULL,
	[Title] [varchar](1500) NULL,
	[StartDate] [varchar](50) NULL,
	[EndDate] [varchar](50) NULL,
	[StartTime] [varchar](20) NULL,
	[EndTime] [varchar](20) NULL,
	[Notes] [varchar](3000) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [varchar](10) NULL,
	[ModifiedOn] [datetime] NULL,
	[ModifiedBy] [varchar](10) NULL,
	[IsArchived] [bit] NULL,
	[ArchivedOn] [datetime] NULL,
	[ArchivedBy] [varchar](10) NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--


USE [Subzz_Users]
GO

/****** Object:  StoredProcedure [Users].[uspGetUserDetail]    Script Date: 6/13/2019 11:26:31 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,Taimoor Ali>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
-- Exec [Users].[uspGetUserDetail] 'U000000098'
ALTER PROCEDURE [Users].[uspGetUserDetail]
@UserId varchar(10)
AS
BEGIN
	declare @UserLevel Int
	declare @UserRoleId Int
	declare @LocationId varchar(5)
	Select @UserRoleId = USROLE.Role_Id from Users.Users USR 
	INNER JOIN Users.UserRole USROLE ON USR.User_Id = USROLE.User_Id 
	where USR.User_Id = @UserId
	Select @UserLevel = UL.UserLevel_Id, @LocationId = UL.Location_Id from Users.Users USR 
		INNER JOIN Users.UserLocation UL ON USR.User_Id = UL.User_Id 
		where USR.User_Id = @UserId

	If(@UserLevel = 1 And (@UserRoleId = 1 or @UserRoleId = 3))
	Begin
	--IF District Admin
	SELECT u.User_Id as UserId,u.Firstname as FirstName ,u.LastName as LastName , u.Firstname + ' ' + u.LastName as UserName,
	 u.UserType_Id as UserTypeId, USRTYPE.Title as UserTypeDescription, USROLE.Role_Id as RoleId , ROL.Name as UserRoleDesciption,
	  u.Email as Email, u.Phone as PhoneNumber, u.ProfilePicUrl as ProfilePicture, u.[Gender] as Gender, u.IsActive as IsActive,
	  u.[IsCertified] as IsCertified,u.[IsSubscribedEmail] as IsSubscribedEmail, u.[IsSubscribedSMS] as IsSubscribedSMS, u.District_Id as DistrictId, ORG.Organization_Id as OrganizationId, @UserLevel as UserLevel,
	   TEA.TeacherSpeciality_Id as SpecialityTypeId, TEA.[TeacherLevel_Id] as TeachingLevel, u.PayRate as PayRate, u.IsViewedNewVersion
	from Subzz_Users.Users.Users u 
	INNER JOIN Users.UserRole USROLE ON u.User_Id = UsRole.User_Id
	INNER JOIN Users.[Role] ROL ON ROL.Role_Id =  UsRole.Role_Id
	INNER JOIN Users.UserType USRTYPE ON u.UserType_Id = USRTYPE.UserType_Id
	INNER JOIN Users.UserLocation UL ON u.User_Id = UL.User_Id
	LEFT JOIN Users.Teacher TEA ON TEA.User_Id = u.User_Id
	left JOIN Subzz_Locations.Location.Organization ORG ON ORG.Organization_Id = UL.Location_Id
	where u.User_Id= @UserId 
	End

	else If(@UserRoleId = 4)
	Begin
	--IF Substitute 
	SELECT u.User_Id as UserId,u.Firstname as FirstName ,u.LastName as LastName , u.Firstname + ' ' + u.LastName as UserName,
	 u.UserType_Id as UserTypeId, USRPOS.Title as UserTypeDescription, USROLE.Role_Id as RoleId , ROL.Name as UserRoleDesciption,
	  u.Email as Email, u.Phone as PhoneNumber, u.ProfilePicUrl as ProfilePicture, u.[Gender] as Gender, u.IsActive as IsActive,
	  u.[IsCertified] as IsCertified,u.[IsSubscribedEmail] as IsSubscribedEmail, u.[IsSubscribedSMS] as IsSubscribedSMS, u.District_Id as DistrictId, ORG.Organization_Id as OrganizationId, @UserLevel as UserLevel,
	   TEA.TeacherSpeciality_Id as SpecialityTypeId, TEA.[TeacherLevel_Id] as TeachingLevel, u.PayRate as PayRate, u.IsViewedNewVersion
	from Subzz_Users.Users.Users u 
	INNER JOIN Users.UserRole USROLE ON u.User_Id = UsRole.User_Id
	INNER JOIN Users.[Role] ROL ON ROL.Role_Id =  UsRole.Role_Id
	INNER JOIN Users.Position USRPOS ON u.UserType_Id = USRPOS.Position_Id
	INNER JOIN Users.UserLocation UL ON u.User_Id = UL.User_Id
	LEFT JOIN Users.Teacher TEA ON TEA.User_Id = u.User_Id
	left JOIN Subzz_Locations.Location.Organization ORG ON ORG.Organization_Id = UL.Location_Id
	where u.User_Id= @UserId 
	End
	else if (@UserLevel = 3)
	begin
	--Employee or School Admin
	SELECT  u.User_Id as UserId,u.Firstname as FirstName ,u.LastName as LastName , u.Firstname + ' ' + u.LastName as UserName,
	 u.UserType_Id as UserTypeId, USRTYPE.Title as UserTypeDescription, USROLE.Role_Id as RoleId , ROL.Name as UserRoleDesciption,
	  u.Email as Email, u.Phone as PhoneNumber, u.ProfilePicUrl as ProfilePicture, u.[Gender] as Gender, u.IsActive as IsActive,
	  u.[IsCertified] as IsCertified,u.[IsSubscribedEmail] as IsSubscribedEmail, u.[IsSubscribedSMS] as IsSubscribedSMS, u.District_Id as DistrictId, ORG.Organization_Id as OrganizationId, @UserLevel as UserLevel,
	  TEA.TeacherSpeciality_Id as SpecialityTypeId, TEA.[TeacherLevel_Id] as TeachingLevel, u.PayRate as PayRate, u.IsViewedNewVersion
	from Subzz_Users.Users.Users u 
	INNER JOIN Users.UserRole USROLE ON u.User_Id = UsRole.User_Id
	INNER JOIN Users.[Role] ROL ON ROL.Role_Id =  UsRole.Role_Id
	INNER JOIN Users.UserType USRTYPE ON u.UserType_Id = USRTYPE.UserType_Id
	INNER JOIN Users.UserLocation UL ON u.User_Id = UL.User_Id
	LEFT JOIN Users.Teacher TEA ON TEA.User_Id = u.User_Id
	left JOIN Subzz_Locations.Location.Organization ORG ON ORG.Organization_Id = UL.Location_Id
	where u.User_Id= @UserId And ORG.Organization_Id is not null And UL.IsPrimary = 1
	End
	else
	Begin
	--IF SUPER ADMIN
	SELECT u.User_Id as UserId,u.Firstname as FirstName ,u.LastName as LastName , u.Firstname + ' ' + u.LastName as UserName,
	 u.UserType_Id as UserTypeId, USRTYPE.Title as UserTypeDescription, USROLE.Role_Id as RoleId , ROL.Name as UserRoleDesciption,
	  u.Email as Email, u.Phone as PhoneNumber, u.ProfilePicUrl as ProfilePicture, u.[Gender] as Gender, u.IsActive as IsActive,
	  u.[IsCertified] as IsCertified,u.[IsSubscribedEmail] as IsSubscribedEmail, u.[IsSubscribedSMS] as IsSubscribedSMS, u.District_Id as DistrictId, '-1' as OrganizationId, @UserLevel as UserLevel 
	  ,u.IsViewedNewVersion
	from Subzz_Users.Users.Users u 
	INNER JOIN Users.UserRole USROLE ON u.User_Id = UsRole.User_Id
	INNER JOIN Users.[Role] ROL ON ROL.Role_Id =  UsRole.Role_Id
	INNER JOIN Users.UserType USRTYPE ON u.UserType_Id = USRTYPE.UserType_Id
	where u.User_Id= @UserId
	End
	
END

GO

/****** Object:  StoredProcedure [Users].[GetEvents]    Script Date: 6/13/2019 11:26:31 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		@Shahzada Ghazanfar
-- Create date: @06/01/2019
-- Description:	@Get Events
-- =============================================

-- Exec [Users].[GetEvents] 

CREATE PROCEDURE [Users].[GetEvents]
	@UserId		VARCHAR(10) = NULL,
	@StartDate	DATETIME	= NULL,
	@EndDate	DATETIME	= NULL
AS
BEGIN

	IF(@StartDate IS NOT NULL AND @EndDate IS NOT NULL)
	BEGIN
		SELECT @StartDate = CONVERT(char(10), @StartDate, 126), @EndDate = CONVERT(char(10), @EndDate, 126)
	END

	SELECT	[EventId]
			,[UserId]
			,[Title]
			,[StartDate]
			,[EndDate]
			,[StartTime]
			,[EndTime]
			,[Notes]
			,[CreatedOn]
			,[CreatedBy]
			,[ModifiedOn]
			,[ModifiedBy]
			,[IsArchived]
			,[ArchivedOn]
			,[ArchivedBy]
	FROM	[Subzz_Users].[Users].[Events]
	WHERE (ISNULL(@UserId, '') = '' OR UserId = @UserId)
	  AND (ISNULL([IsArchived], 0) = 0)
	  AND ((ISNULL(@StartDate, '') = '' AND ISNULL(@EndDate, '') = '') OR (StartDate BETWEEN  @StartDate AND @EndDate) OR (EndDate BETWEEN @StartDate AND @EndDate)) 

END  
GO

/****** Object:  StoredProcedure [Users].[InsertEvent]    Script Date: 6/13/2019 11:26:31 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		@Shahzada Ghazanfar
-- Create date: @06/01/2019
-- Description:	@Unavailability Calendar
-- =============================================

-- Exec [Users].[InsertEvent] 

CREATE PROCEDURE [Users].[InsertEvent]
	@UserId					VARCHAR(10),
	@Title					VARCHAR(1500),
	@StartDate				VARCHAR(50),
	@EndDate				VARCHAR(50),
	@StartTime				VARCHAR(20),
	@EndTime				VARCHAR(20),
	@Notes					VARCHAR(3000),
	@CreatedBy				VARCHAR(10)
AS
BEGIN
	DECLARE @EventId INT = 0
	
	INSERT INTO [Users].[Events]
    (
		[UserId],
        [Title],
	    [StartDate],
	    [EndDate],
	    [StartTime],
	    [EndTime],
		[Notes],
	    [CreatedOn],
	    [CreatedBy]
	)
     VALUES
    (
		@UserId,
        @Title,
	    @StartDate,
	    @EndDate,
	    @StartTime,
	    @EndTime,
		@Notes,
	    GETDATE(),
	    @CreatedBy
	)
	
	SET @EventId = SCOPE_IDENTITY()
	
	SELECT	[EventId]
			,[UserId]
			,[Title]
			,[StartDate]
			,[EndDate]
			,[StartTime]
			,[EndTime]
			,[Notes]
			,[CreatedOn]
			,[CreatedBy]
			,[ModifiedOn]
			,[ModifiedBy]
			,[IsArchived]
			,[ArchivedOn]
			,[ArchivedBy]
	FROM	[Subzz_Users].[Users].[Events]
	WHERE	[EventId] = @EventId
END  
GO



