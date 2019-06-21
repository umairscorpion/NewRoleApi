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

USE [Subzz_Users]
GO
/****** Object:  StoredProcedure [Users].[DashboardSummary]    Script Date: 6/13/2019 12:37:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--EXEC DashboardSummary 'U000000025', 2019
CREATE PROCEDURE [Users].[DashboardSummary]
@UserId VARCHAR(15),
@Year INT
AS
BEGIN
DECLARE @LocationId VARCHAR(15) 
DECLARE @Role INT = NULL
DECLARE @CurrentDate date
DECLARE @PreviousDate date
SET @CurrentDate = (SELECT CONVERT(VARCHAR(10), getdate(), 120))
SET @PreviousDate = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -1),120))
DECLARE @PreviousDateMinusOne date
DECLARE @PreviousDateMinusTwo date
DECLARE @PreviousDateMinusThree date
DECLARE @PreviousDateMinusFour date
DECLARE @PreviousDateMinusFive date
DECLARE @PreviousDateMinusSix date
DECLARE @PreviousDateMinusSeven date
DECLARE @PreviousDateMinusEight date
DECLARE @PreviousDateMinusNine date
DECLARE @PreviousDateMinusTen date
DECLARE @Week1Day VARCHAR(10) = CONVERT(VARCHAR(10), DATEADD(WEEK, DATEDIFF(WEEK, 0, @CurrentDate), 0), 111)
DECLARE @Week2Day VARCHAR(10) = CONVERT(VARCHAR(10), DATEADD(DAY, 1, @Week1Day), 111)
DECLARE @Week3Day VARCHAR(10) = CONVERT(VARCHAR(10), DATEADD(DAY, 2, @Week1Day), 111)
DECLARE @Week4Day VARCHAR(10) = CONVERT(VARCHAR(10), DATEADD(DAY, 3, @Week1Day), 111)
DECLARE @Week5Day VARCHAR(10) = CONVERT(VARCHAR(10), DATEADD(DAY, 4, @Week1Day), 111)

SET @PreviousDateMinusOne = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -1),120))  
SET @PreviousDateMinusTwo = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -2),120))  
SET @PreviousDateMinusThree = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -3),120))  
SET @PreviousDateMinusFour = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -4),120))  
SET @PreviousDateMinusFive = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -5),120))  
SET @PreviousDateMinusSix = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -6),120))  
SET @PreviousDateMinusSeven = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -7),120))  
SET @PreviousDateMinusEight = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -8),120))  
SET @PreviousDateMinusNine = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -9),120))  
SET @PreviousDateMinusTen = (SELECT CONVERT(VARCHAR(10),DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), -10),120))  

SET @Role = (SELECT [Role_Id] FROM [Subzz_Users].[Users].[UserRole] UR 
INNER JOIN [Subzz_Users].[Users].[Users] USR on UR.User_Id = USR.User_Id
WHERE USR.User_Id =  @UserId)


IF(@Role = 5) --FOR SUPER ADMIN
BEGIN
SELECT
--Absence Summary
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 1 THEN 1 END) AS January,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 2 THEN 1 END) AS February,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 3 THEN 1 END) AS March,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 4 THEN 1 END) AS April,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 5 THEN 1 END) AS May,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 6 THEN 1 END) AS June,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 7 THEN 1 END) AS July,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 8THEN 1 END) AS August,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 9 THEN 1 END) AS September,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 10 THEN 1 END) AS October,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 11 THEN 1 END) AS November,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 12 THEN 1 END) AS December,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) THEN 1 END) AS TotalAbsences,

--Absence Reasons
COUNT(CASE WHEN LeaveType_Id = 6 THEN 1 END) AS PersonalLeave,
COUNT(CASE WHEN LeaveType_Id = 2 THEN 1 END) AS IllnessSelf,
COUNT(CASE WHEN LeaveType_Id in (1,3,4) THEN 1 END) AS Other,
COUNT(CASE WHEN LeaveType_Id = 6 THEN 1 END) AS PD,

--Dashboard Top Counters
COUNT(CASE WHEN AbsenceStatus_Id in (1,2) AND StartDate = @CurrentDate  THEN 1 END) AS TotalCount,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @CurrentDate THEN 1 END) AS Filled,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @CurrentDate THEN 1 END) AS Unfilled,
COUNT(CASE WHEN IsSubstituteRequired = 0 AND StartDate = @CurrentDate THEN 1 END) AS NoSubRequired,
COUNT(CASE WHEN AbsenceStatus_Id in (1,2) AND StartDate = @PreviousDate THEN 1 END) AS TotalPrevious,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDate THEN 1 END) AS FilledPrevious,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDate THEN 1 END) AS UnfilledPrevious,
COUNT(CASE WHEN IsSubstituteRequired = 0 AND StartDate = @PreviousDate THEN 1 END) AS NoSubRequiredPrevious,

--Ten Day Trend
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusOne THEN 1 END) AS FilledPreviousMinusOne,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTwo THEN 1 END) AS FilledPreviousMinusTwo,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusThree THEN 1 END) AS FilledPreviousMinusThree,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFour THEN 1 END) AS FilledPreviousMinusFour,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFive THEN 1 END) AS FilledPreviousMinusFive,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSix THEN 1 END) AS FilledPreviousMinusSix,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSeven THEN 1 END) AS FilledPreviousMinusSeven,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusEight THEN 1 END) AS FilledPreviousMinusEight,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusNine THEN 1 END) AS FilledPreviousMinusNine,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTen THEN 1 END) AS FilledPreviousMinusTen,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusOne THEN 1 END) AS UnfilledPreviousMinusOne,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTwo THEN 1 END) AS UnfilledPreviousMinusTwo,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusThree THEN 1 END) AS UnfilledPreviousMinusThree,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFour THEN 1 END) AS UnfilledPreviousMinusFour,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFive THEN 1 END) AS UnfilledPreviousMinusFive,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSix THEN 1 END) AS UnfilledPreviousMinusSix,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSeven THEN 1 END) AS UnfilledPreviousMinusSeven,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusEight THEN 1 END) AS UnfilledPreviousMinusEight,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusNine THEN 1 END) AS UnfilledPreviousMinusNine,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTen THEN 1 END) AS UnfilledPreviousMinusTen,

--Fill Rate(Total YTD)
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) THEN 1 END) * 100.0/ sum(count(CASE WHEN AbsenceStatus_Id in (1,2,3)  THEN 1 END)) over () as TotalFilled,
COUNT(CASE WHEN AbsenceStatus_Id = 1 THEN 1 END) * 100.0/ sum(count(CASE WHEN AbsenceStatus_Id in (1,2,3)  THEN 1 END)) over () as TotalUnfilled,

--Absences By Week
COUNT(CASE WHEN (StartDate = @Week1Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayMonday,
COUNT(CASE WHEN (StartDate = @Week2Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayTuesday,
COUNT(CASE WHEN (StartDate = @Week3Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayWednesday,
COUNT(CASE WHEN (StartDate = @Week4Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayThursday,
COUNT(CASE WHEN (StartDate = @Week5Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayFriday

from [Subzz_Leaves].[Absence].[Absence]

--GET TOP TEACHERS
SELECT TOP 10 Usr.FirstName,
Usr.LastName,
COUNT(Ab.Absence_Id) AS TotalAbsence
FROM
Users.Users Usr
INNER JOIN Users.UserRole Ur On Ur.User_Id = Usr.User_Id
INNER JOIN Subzz_Leaves.Absence.Absence Ab ON Ab.User_Id = Usr.User_Id
WHERE Ur.Role_Id = 3
GROUP BY Usr.FirstName,Usr.LastName
ORDER BY TotalAbsence DESC

--GET ABSENCES BY SUBJECT
Select TS.Title,
Count(TS.Title) As Total
 from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join Subzz_Users.Users.Teacher T on T.User_Id = A.User_Id
Inner Join Subzz_Lookups.Lookup.TeacherSpeciality TS on TS.TeacherSpeciality_Id = T.TeacherSpeciality_Id
Group By TS.Title

--GET ABSENCES BY GRADE LEVEL
Select TL.Title,
Count(TL.Title) As Total
from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join Subzz_Users.Users.Teacher T on T.User_Id = A.User_Id
Inner Join [Subzz_Lookups].[Lookup].[TeachingLevel] TL on Tl.TeachingLevel_Id = T.TeacherLevel_Id
Group By TL.Title

--Top 4 Absence Reasons
Select LT.Name,
Count(LT.Name) As Total
from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join [Subzz_Leaves].[Leaves].[LeaveType] LT on LT.LeaveType_Id = A.LeaveType_Id
Group By LT.Name

END


IF(@Role = 1) --GET ABSENCE SUMMARY FOR DISTRICT ADMIN
BEGIN
SET @LocationId = (SELECT [Location_Id] FROM [Subzz_Users].[Users].[UserLocation] L 
				 INNER JOIN [Subzz_Users].[Users].[Users] U on L.User_Id = U.User_Id WHERE U.User_Id = @UserId)
SELECT
--Absence Summary
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 1 THEN 1 END) AS January,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 2 THEN 1 END) AS February,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 3 THEN 1 END) AS March,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 4 THEN 1 END) AS April,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 5 THEN 1 END) AS May,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 6 THEN 1 END) AS June,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 7 THEN 1 END) AS July,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 8THEN 1 END) AS August,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 9 THEN 1 END) AS September,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 10 THEN 1 END) AS October,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 11 THEN 1 END) AS November,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 12 THEN 1 END) AS December,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) THEN 1 END) AS TotalAbsences,
--Absence Reason
COUNT(CASE WHEN LeaveType_Id = 6 THEN 1 END) AS PersonalLeave,
COUNT(CASE WHEN LeaveType_Id = 2 THEN 1 END) AS IllnessSelf,
COUNT(CASE WHEN LeaveType_Id in (1,3,4) THEN 1 END) AS Other,
COUNT(CASE WHEN LeaveType_Id = 6 THEN 1 END) AS PD,
--Dashboard Top Counters
COUNT(CASE WHEN AbsenceStatus_Id in (1,2) AND StartDate = @CurrentDate  THEN 1 END) AS TotalCount,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @CurrentDate THEN 1 END) AS Filled,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @CurrentDate THEN 1 END) AS Unfilled,
COUNT(CASE WHEN IsSubstituteRequired = 0 AND StartDate = @CurrentDate THEN 1 END) AS NoSubRequired,
COUNT(CASE WHEN AbsenceStatus_Id in (1,2) AND StartDate = @PreviousDate THEN 1 END) AS TotalPrevious,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDate THEN 1 END) AS FilledPrevious,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDate THEN 1 END) AS UnfilledPrevious,
COUNT(CASE WHEN IsSubstituteRequired = 0 AND StartDate = @PreviousDate THEN 1 END) AS NoSubRequiredPrevious,
--Ten Day Trend
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusOne THEN 1 END) AS FilledPreviousMinusOne,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTwo THEN 1 END) AS FilledPreviousMinusTwo,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusThree THEN 1 END) AS FilledPreviousMinusThree,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFour THEN 1 END) AS FilledPreviousMinusFour,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFive THEN 1 END) AS FilledPreviousMinusFive,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSix THEN 1 END) AS FilledPreviousMinusSix,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSeven THEN 1 END) AS FilledPreviousMinusSeven,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusEight THEN 1 END) AS FilledPreviousMinusEight,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusNine THEN 1 END) AS FilledPreviousMinusNine,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTen THEN 1 END) AS FilledPreviousMinusTen,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusOne THEN 1 END) AS UnfilledPreviousMinusOne,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTwo THEN 1 END) AS UnfilledPreviousMinusTwo,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusThree THEN 1 END) AS UnfilledPreviousMinusThree,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFour THEN 1 END) AS UnfilledPreviousMinusFour,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFive THEN 1 END) AS UnfilledPreviousMinusFive,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSix THEN 1 END) AS UnfilledPreviousMinusSix,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSeven THEN 1 END) AS UnfilledPreviousMinusSeven,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusEight THEN 1 END) AS UnfilledPreviousMinusEight,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusNine THEN 1 END) AS UnfilledPreviousMinusNine,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTen THEN 1 END) AS UnfilledPreviousMinusTen,
--Fill Rate(Total YTD)
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) THEN 1 END) * 100.0/ sum(count(CASE WHEN AbsenceStatus_Id in (1,2,3)  THEN 1 END)) over () as TotalFilled,
COUNT(CASE WHEN AbsenceStatus_Id = 1 THEN 1 END) * 100.0/ sum(count(CASE WHEN AbsenceStatus_Id in (1,2,3)  THEN 1 END)) over () as TotalUnfilled,
--Absences By Week
COUNT(CASE WHEN (StartDate = @Week1Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayMonday,
COUNT(CASE WHEN (StartDate = @Week2Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayTuesday,
COUNT(CASE WHEN (StartDate = @Week3Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayWednesday,
COUNT(CASE WHEN (StartDate = @Week4Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayThursday,
COUNT(CASE WHEN (StartDate = @Week5Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayFriday

from [Subzz_Leaves].[Absence].[Absence] Ab
WHERE Ab.District_Id = @LocationId AND YEAR(StartDate) = @Year

--Top Ten Teachers
SELECT TOP 10 Usr.FirstName,
Usr.LastName,
COUNT(Ab.Absence_Id) AS TotalAbsence
FROM
Users.Users Usr
INNER JOIN Users.UserRole Ur On Ur.User_Id = Usr.User_Id
INNER JOIN Subzz_Leaves.Absence.Absence Ab ON Ab.User_Id = Usr.User_Id
WHERE Ab.District_Id = @LocationId AND Ur.Role_Id = 3
GROUP BY Usr.FirstName,Usr.LastName
ORDER BY TotalAbsence DESC

--Absences by Subject
Select TS.Title,
Count(TS.Title) As Total
 from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join Subzz_Users.Users.Teacher T on T.User_Id = A.User_Id
Inner Join Subzz_Lookups.Lookup.TeacherSpeciality TS on TS.TeacherSpeciality_Id = T.TeacherSpeciality_Id
WHERE A.District_Id = @LocationId 
Group By TS.Title

--Absences By Grade Level
Select TL.Title,
Count(TL.Title) As Total
from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join Subzz_Users.Users.Teacher T on T.User_Id = A.User_Id
Inner Join [Subzz_Lookups].[Lookup].[TeachingLevel] TL on Tl.TeachingLevel_Id = T.TeacherLevel_Id
WHERE A.District_Id = @LocationId 
Group By TL.Title

--Top 4 Absence Reasons
Select LT.Name,
Count(LT.Name) As Total
from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join [Subzz_Leaves].[Leaves].[LeaveType] LT on LT.LeaveType_Id = A.LeaveType_Id
WHERE A.District_Id = @LocationId 
Group By LT.Name

END

IF( @Role = 2) --GET ABSENCE SUMMARY FOR SCHOOL ADMIN
BEGIN
SET @LocationId = (SELECT [Location_Id] FROM [Subzz_Users].[Users].[UserLocation] L 
				 INNER JOIN [Subzz_Users].[Users].[Users] U on L.User_Id = U.User_Id
				 INNER JOIN [Subzz_Locations].[Location].[Organization] O on O.Organization_Id=L.Location_Id
				 WHERE U.User_Id = @UserId AND ISNULL(L.IsPrimary, 0) = 1)
SELECT
--Absence Summary
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 1 THEN 1 END) AS January,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 2 THEN 1 END) AS February,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 3 THEN 1 END) AS March,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 4 THEN 1 END) AS April,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 5 THEN 1 END) AS May,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 6 THEN 1 END) AS June,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 7 THEN 1 END) AS July,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 8THEN 1 END) AS August,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 9 THEN 1 END) AS September,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 10 THEN 1 END) AS October,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 11 THEN 1 END) AS November,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) and MONTH(StartDate) = 12 THEN 1 END) AS December,
COUNT(CASE WHEN (YEAR(StartDate) = @Year or YEAR(EndDate) = @Year) THEN 1 END) AS TotalAbsences,
--Absence Reasons
COUNT(CASE WHEN LeaveType_Id = 6 THEN 1 END) AS PersonalLeave,
COUNT(CASE WHEN LeaveType_Id = 2 THEN 1 END) AS IllnessSelf,
COUNT(CASE WHEN LeaveType_Id in (1,3,4) THEN 1 END) AS Other,
COUNT(CASE WHEN LeaveType_Id = 6 THEN 1 END) AS PD,
--Dashboard Top Counter
COUNT(CASE WHEN AbsenceStatus_Id in (1,2) AND StartDate = @CurrentDate  THEN 1 END) AS TotalCount,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @CurrentDate THEN 1 END) AS Filled,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @CurrentDate THEN 1 END) AS Unfilled,
COUNT(CASE WHEN IsSubstituteRequired = 0 AND StartDate = @CurrentDate THEN 1 END) AS NoSubRequired,
COUNT(CASE WHEN AbsenceStatus_Id in (1,2) AND StartDate = @PreviousDate THEN 1 END) AS TotalPrevious,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDate THEN 1 END) AS FilledPrevious,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDate THEN 1 END) AS UnfilledPrevious,
COUNT(CASE WHEN IsSubstituteRequired = 0 AND StartDate = @PreviousDate THEN 1 END) AS NoSubRequiredPrevious,
--Ten Day Trend
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusOne THEN 1 END) AS FilledPreviousMinusOne,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTwo THEN 1 END) AS FilledPreviousMinusTwo,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusThree THEN 1 END) AS FilledPreviousMinusThree,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFour THEN 1 END) AS FilledPreviousMinusFour,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFive THEN 1 END) AS FilledPreviousMinusFive,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSix THEN 1 END) AS FilledPreviousMinusSix,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSeven THEN 1 END) AS FilledPreviousMinusSeven,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusEight THEN 1 END) AS FilledPreviousMinusEight,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusNine THEN 1 END) AS FilledPreviousMinusNine,
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTen THEN 1 END) AS FilledPreviousMinusTen,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusOne THEN 1 END) AS UnfilledPreviousMinusOne,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTwo THEN 1 END) AS UnfilledPreviousMinusTwo,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusThree THEN 1 END) AS UnfilledPreviousMinusThree,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFour THEN 1 END) AS UnfilledPreviousMinusFour,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusFive THEN 1 END) AS UnfilledPreviousMinusFive,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSix THEN 1 END) AS UnfilledPreviousMinusSix,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusSeven THEN 1 END) AS UnfilledPreviousMinusSeven,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusEight THEN 1 END) AS UnfilledPreviousMinusEight,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusNine THEN 1 END) AS UnfilledPreviousMinusNine,
COUNT(CASE WHEN AbsenceStatus_Id = 1 AND IsSubstituteRequired = 1 AND StartDate = @PreviousDateMinusTen THEN 1 END) AS UnfilledPreviousMinusTen,
--Fill Rate(Total YTD)
COUNT(CASE WHEN AbsenceStatus_Id in (2,3) THEN 1 END) * 100.0/ sum(count(CASE WHEN AbsenceStatus_Id in (1,2,3)  THEN 1 END)) over () as TotalFilled,
COUNT(CASE WHEN AbsenceStatus_Id = 1 THEN 1 END) * 100.0/ sum(count(CASE WHEN AbsenceStatus_Id in (1,2,3)  THEN 1 END)) over () as TotalUnfilled,
--Absences By Week
COUNT(CASE WHEN (StartDate = @Week1Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayMonday,
COUNT(CASE WHEN (StartDate = @Week2Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayTuesday,
COUNT(CASE WHEN (StartDate = @Week3Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayWednesday,
COUNT(CASE WHEN (StartDate = @Week4Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayThursday,
COUNT(CASE WHEN (StartDate = @Week5Day AND AbsenceStatus_Id in (1,2,3,4,6,7,8)) THEN 1 END) AS WeekDayFriday
from [Subzz_Leaves].[Absence].[Absence] Ab
WHERE Ab.School_Id = @LocationId AND YEAR(StartDate) = @Year

--Top Ten Tachers
SELECT TOP 10 Usr.FirstName,
Usr.LastName,
COUNT(Ab.Absence_Id) AS TotalAbsence
FROM
Users.Users Usr
INNER JOIN Users.UserRole Ur On Ur.User_Id = Usr.User_Id
INNER JOIN Subzz_Leaves.Absence.Absence Ab ON Ab.User_Id = Usr.User_Id
WHERE Ab.School_Id = @LocationId AND Ur.Role_Id = 3
GROUP BY Usr.FirstName,Usr.LastName
ORDER BY TotalAbsence DESC

--Absences By Subjects
Select TS.Title,
Count(TS.Title) As Total
 from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join Subzz_Users.Users.Teacher T on T.User_Id = A.User_Id
Inner Join Subzz_Lookups.Lookup.TeacherSpeciality TS on TS.TeacherSpeciality_Id = T.TeacherSpeciality_Id
WHERE A.School_Id = @LocationId
Group By TS.Title

--Absences By Grade Level
Select TL.Title,
Count(TL.Title) As Total
from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join Subzz_Users.Users.Teacher T on T.User_Id = A.User_Id
Inner Join [Subzz_Lookups].[Lookup].[TeachingLevel] TL on Tl.TeachingLevel_Id = T.TeacherLevel_Id
WHERE A.School_Id = @LocationId
Group By TL.Title

--Top 4 Absence Reasons
Select LT.Name,
Count(LT.Name) As Total
from [Subzz_Leaves].[Absence].[Absence] A
Inner Join Subzz_Users.Users.Users U on U.User_Id = A.User_Id
Inner Join [Subzz_Leaves].[Leaves].[LeaveType] LT on LT.LeaveType_Id = A.LeaveType_Id
WHERE A.School_Id = @LocationId 
Group By LT.Name

END
END
GO



