USE [Subzz_Users]
GO
/****** Object:  StoredProcedure [Users].[GetAvailability]    Script Date: 6/18/2019 9:36:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Shahzada Ghazanfar>
-- Create date: <03/30/2019>
-- Description:	<Unavailability Calendar>
-- =============================================

-- Exec [Users].[GetAvailability] 

ALTER PROCEDURE [Users].[GetAvailability] 

@UserId		VARCHAR(10) = NULL,
@StartDate	DATETIME	= NULL,
@EndDate	DATETIME	= NULL

AS
BEGIN	

	IF(@StartDate IS NOT NULL AND @EndDate IS NOT NULL)
	BEGIN
		SELECT @StartDate = CONVERT(char(10), @StartDate, 126), @EndDate = CONVERT(char(10), @EndDate, 126)
	END

	SELECT a.[AvailabilityId]
		  ,a.[UserId]
		  ,a.[AvailabilityStatusId]
		  ,st.Title as Title -- a.[Title]
		  ,st.[Name]	As AvailabilityStatusName
		  ,st.[Title]	As AvailabilityStatusTitle
		  ,st.[ContentBackgroundColor] As AvailabilityContentBackgroundColor
		  ,st.[IconCss] As AvailabilityIconCss
		  ,a.[StartDate]
		  ,a.[EndDate]
		  ,a.[StartTime]
		  ,a.[EndTime]
		  ,a.[IsAllDayOut]
		  ,a.[IsRepeat]
		  ,a.[RepeatType]
		  ,a.[RepeatValue]
		  ,a.[RepeatOnWeekDays]
		  ,a.[IsEndsNever]
		  ,a.[EndsOnAfterNumberOfOccurrance]
		  ,a.[EndsOnUntilDate]
		  ,a.[Notes]
		  ,a.[CreatedOn]
		  ,a.[CreatedBy]
		  ,a.[ModifiedOn]
		  ,a.[ModifiedBy]
		  ,a.[IsArchived]
		  ,a.[ArchivedOn]
		  ,a.[ArchivedBy]
	  FROM [Subzz_Users].[Users].[Availability] a WITH(NOLOCK)
	  INNER JOIN [Subzz_Users].[Users].[AvailabilityStatus] st ON st.[AvailabilityStatusId] = a.[AvailabilityStatusId]
	  WHERE (ISNULL(@UserId, '') = '' OR a.UserId = @UserId)
	  AND (ISNULL(a.[IsArchived], 0) = 0)
	  AND ((ISNULL(@StartDate, '') = '' AND ISNULL(@EndDate, '') = '') OR (a.StartDate BETWEEN  @StartDate AND @EndDate) OR (a.EndDate BETWEEN @StartDate AND @EndDate)) 

END  
------------------
  UPDATE [Subzz_Users].[Users].[AvailabilityStatus] SET ContentBackgroundColor = '#d20f0f' WHERE AvailabilityStatusId = 1
  UPDATE [Subzz_Users].[Users].[AvailabilityStatus] SET ContentBackgroundColor = '#0ea8ea' WHERE AvailabilityStatusId = 2
  UPDATE [Subzz_Users].[Users].[AvailabilityStatus] SET ContentBackgroundColor = '#0ea8ea' WHERE AvailabilityStatusId = 3
------------------

USE [Subzz_Users]
GO
/****** Object:  StoredProcedure [Users].[GetSubstituteAvailabilities]    Script Date: 6/18/2019 9:41:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Shahzada Ghazanfar>
-- Create date: <04/11/2019>
-- Description:	<Substitute Availabilities>
-- =============================================

-- Exec [Users].[GetSubstituteAvailabilities] '6/17/2019 3:54:46 AM', 1, null

ALTER PROCEDURE [Users].[GetSubstituteAvailabilities] 
@StartDate				DATETIME 	= NULL,
@AvailabilityStatusId 	INT 		= -1,
@UserId 				VARCHAR(50) = NULL
AS
BEGIN	


DECLARE @COLS AS NVARCHAR(MAX),
    	@QUERY  AS NVARCHAR(MAX),
		@EndDate    DATETIME = NULL
		
IF(ISNULL(@StartDate, '') = '')
BEGIN
	SET @StartDate = GETDATE()
END
ELSE 

SELECT @StartDate = dateadd(day, 1-datepart(dw, @StartDate), CONVERT(date, @StartDate)),
	   @EndDate = dateadd(day, 7-datepart(dw, @StartDate), CONVERT(date, @StartDate))

DECLARE @Availability 		TABLE 
(
	AvailabilityId 			INT,
	StartDate 				DATETIME,
	StartTime 				VARCHAR(100),
	EndDate 				DATETIME,
	EndTime 				VARCHAR(100),
	UserId 					VARCHAR(100),
	FirstName				VARCHAR(100),
	LastName				VARCHAR(100),
	Gender					VARCHAR(100),
	ProfilePicUrl			VARCHAR(max),
	Phone					VARCHAR(100),
	Email					VARCHAR(100),
	Speciality				VARCHAR(100),
	Available				VARCHAR(100),
	AvailabilityStatusName	VARCHAR(100),
	AvailabilityStatusId 	INT,
	AvailabilityStatusTitle VARCHAR(500),
	AvailabilityContentBackgroundColor VARCHAR(100),
	AvailabilityIconCss VARCHAR(100)
)
	
INSERT INTO @Availability (AvailabilityId, StartDate, StartTime, EndDate, EndTime, UserId, FirstName, LastName, Gender, ProfilePicUrl, Phone, Email, Speciality, Available, AvailabilityStatusId, AvailabilityStatusName, AvailabilityStatusTitle)
SELECT ROW_NUMBER() OVER (ORDER BY UserId), d, '12:00 AM', d, '12:00 AM', UserId, FirstName, LastName, Gender, ProfilePicUrl, Phone, Email, Speciality, Available, 0, 'AVAILABLE', 'Available'
FROM
(
  SELECT
       d = DATEADD(DAY, rn - 1, @StartDate)
	  ,u.User_Id as UserId
	  ,u.FirstName
	  ,u.LastName
	  ,u.Gender
	  ,u.ProfilePicUrl
	  ,u.Phone
	  ,u.Email
	  ,'' AS Speciality
	  ,'' As Available
  FROM 
  (
      SELECT TOP (DATEDIFF(DAY, @StartDate, @EndDate)) 
          rn = ROW_NUMBER() OVER (ORDER BY s1.[object_id])
      FROM
          sys.all_objects AS s1
      CROSS JOIN
          sys.all_objects AS s2
      ORDER BY
          s1.[object_id]
  ) AS x
  CROSS JOIN [Subzz_Users].[Users].[Users] u WITH(NOLOCK) 
	WHERE u.[UserRole_Id] = 4 --Substitute 
	AND (ISNULL(@UserId, '') = '' OR u.[User_Id] = @UserId)
) AS y;



UPDATE a 
SET
a.AvailabilityId = av.[AvailabilityId]
,a.AvailabilityStatusId = av.[AvailabilityStatusId]
,AvailabilityStatusName = st.[Name]
,AvailabilityStatusTitle = st.[Title]
			  ,AvailabilityContentBackgroundColor = st.[ContentBackgroundColor]
			  ,AvailabilityIconCss = st.[IconCss]
FROM @Availability AS a
INNER JOIN [Subzz_Users].[Users].[Availability] av ON av.[UserId] = a.[UserId] AND CAST(av.[StartDate] as DATE) = CAST(a.[StartDate] as DATE)
INNER JOIN [Subzz_Users].[Users].[AvailabilityStatus] st ON st.[AvailabilityStatusId] = av.[AvailabilityStatusId]

SELECT * FROM @Availability a
WHERE (ISNULL(@AvailabilityStatusId, 0) = 0 OR a.AvailabilityStatusId = @AvailabilityStatusId)
ORDER BY a.UserId, a.StartDate

END