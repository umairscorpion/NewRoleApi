USE [Subzz_Users]
GO
/****** Object:  StoredProcedure [Users].[GetAvailabilityById]    Script Date: 7/2/2019 5:36:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Adnan Ali
-- Create date: 07/01/2019
-- Description:	GetAvailabilityById
-- =============================================

-- Exec [Users].[GetAvailability]

Create PROCEDURE [Users].[GetAvailabilityById]

@AvailabilityId	INT

AS
BEGIN
	
	SELECT [AvailabilityId],
		   [Title],
	       [UserId],
	       [AvailabilityStatusId],
	       [StartDate],
	       [EndDate],
	       [StartTime],
	       [EndTime],
	       [IsAllDayOut],
	       [IsRepeat],
	       [RepeatType],
	       [RepeatValue],
	       [RepeatOnWeekDays],
	       [IsEndsNever],
	       [EndsOnAfterNumberOfOccurrance],
	       [EndsOnUntilDate],
		   [Notes],
	       [CreatedOn],
	       [CreatedBy],
	       [ModifiedOn],
	       [ModifiedBy],
	       [IsArchived],
	       [ArchivedOn],
	       [ArchivedBy]
	FROM   [Subzz_Users].[Users].[Availability]
	WHERE  [AvailabilityId] = @AvailabilityId
END  

GO
/****** Object:  StoredProcedure [Users].[GetAvailability]    Script Date: 7/2/2019 5:34:59 PM ******/
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
		  ,a.[Title]
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

GO
/****** Object:  StoredProcedure [Users].[InsertAvailability]    Script Date: 7/2/2019 5:34:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		@Shahzada Ghazanfar
-- Create date: @03/30/2019
-- Description:	@Unavailability Calendar
-- =============================================

-- Exec [Users].[InsertAvailability] 

ALTER PROCEDURE [Users].[InsertAvailability]
	@UserId					VARCHAR(10),
	@AvailabilityStatusId	INT,
	@Title					VARCHAR(1500),
	@StartDate				VARCHAR(50),
	@EndDate				VARCHAR(50),
	@StartTime				VARCHAR(20),
	@EndTime				VARCHAR(20),
	@IsAllDayOut			BIT,
	@IsRepeat				BIT,
	@RepeatType				VARCHAR(20),
	@RepeatValue			INT,
	@RepeatOnWeekDays		VARCHAR(300),
	@IsEndsNever			BIT,
	@EndsOnAfterNumberOfOccurrance INT,
	@EndsOnUntilDate		VARCHAR(50),
	@Notes					VARCHAR(3000),
	@CreatedBy				VARCHAR(10)
AS
BEGIN
	DECLARE @AvailabilityId INT = 0
	
	INSERT INTO [Users].[Availability]
	  (
	    [UserId],
	    [AvailabilityStatusId],
		[Title],
	    [StartDate],
	    [EndDate],
	    [StartTime],
	    [EndTime],
	    [IsAllDayOut],
	    [IsRepeat],
	    [RepeatType],
	    [RepeatValue],
	    [RepeatOnWeekDays],
	    [IsEndsNever],
	    [EndsOnAfterNumberOfOccurrance],
	    [EndsOnUntilDate],
	    [CreatedOn],
	    [CreatedBy],
		[IsArchived]
	  )
	VALUES
	  (
	    @UserId,
	    @AvailabilityStatusId,
		@Title,
	    @StartDate,
	    @EndDate,
	    @StartTime,
	    @EndTime,
	    @IsAllDayOut,
	    @IsRepeat,
	    @RepeatType,
	    @RepeatValue,
	    @RepeatOnWeekDays,
	    @IsEndsNever,
	    @EndsOnAfterNumberOfOccurrance,
	    @EndsOnUntilDate,
	    GETDATE(),
	    @CreatedBy,
		0
	  )
	
	SET @AvailabilityId = SCOPE_IDENTITY()
	
	SELECT [AvailabilityId],
	       [UserId],
	       [AvailabilityStatusId],
		   [Title],
	       [StartDate],
	       [EndDate],
	       [StartTime],
	       [EndTime],
	       [IsAllDayOut],
	       [IsRepeat],
	       [RepeatType],
	       [RepeatValue],
	       [RepeatOnWeekDays],
	       [IsEndsNever],
	       [EndsOnAfterNumberOfOccurrance],
	       [EndsOnUntilDate],
	       [CreatedOn],
	       [CreatedBy],
	       [ModifiedOn],
	       [ModifiedBy],
	       [IsArchived],
	       [ArchivedOn],
	       [ArchivedBy]
	FROM   [Subzz_Users].[Users].[Availability]
	WHERE  [AvailabilityId] = @AvailabilityId
END  

GO
/****** Object:  StoredProcedure [Users].[sp_insertPosition]    Script Date: 7/2/2019 5:34:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [Users].[sp_insertPosition] 
	 @Id int,
	 @Title nvarchar(100), 
	 @IsVisible Bit,
	 @DistrictId int,
	 @CreatedDate datetime 

AS
BEGIN
	Insert Into Users.Position
		(
		 [Title]
		,[IsVisible]
		,[DistrictId]
		,CreatedDate
		) 
	Values 
		(
		@Title,
		@IsVisible,
		@DistrictId,
		@CreatedDate 
		)

	Set @Id = SCOPE_IDENTITY()
	Select
		 [Position_Id] AS Id
		,[Title] AS Title
		,[IsVisible] AS IsVisible
		,[DistrictId] AS DistrictId
		,CreatedDate AS CreatedDate
	From	Users.Position
	WHERE	[Position_Id] = @Id
END

GO
/****** Object:  StoredProcedure [Users].[GetSubstituteAvailabilities]    Script Date: 7/2/2019 5:37:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Shahzada Ghazanfar>
-- Create date: <04/11/2019>
-- Description:	<Substitute Availabilities>
-- =============================================

-- Exec [Users].[GetSubstituteAvailabilities] '7/1/2019',0,null,19

ALTER PROCEDURE [Users].[GetSubstituteAvailabilities] 
@StartDate				DATETIME 	= NULL,
@AvailabilityStatusId 	INT 		= -1,
@UserId 				VARCHAR(50) = NULL,
@DistrictId				INT
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

SELECT @StartDate = dateadd(day, 1-datepart(dw, @StartDate), CONVERT(date, @StartDate))
	,@EndDate = dateadd(day, 7-datepart(dw, @StartDate), CONVERT(date, @StartDate))

--	SELECT 	dateadd(day, 1-datepart(dw, @StartDate), CONVERT(date, @StartDate)) AS WeekStartDate,
--			dateadd(day, 5-datepart(dw, @StartDate), CONVERT(date, @StartDate)) AS WeekEndDate

--SELECT @StartDate, @EndDate
	
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
	AND (ISNULL(@DistrictId, '') = '' OR u.District_Id = @DistrictId)
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
		INNER JOIN [Subzz_Users].[Users].[Availability] av ON av.[UserId] = a.[UserId] AND 
		((CAST(a.[StartDate] as DATE) BETWEEN CAST(av.[StartDate] as DATE) And CAST(av.[EndDate] as DATE)) OR 
		(CAST(a.[EndDate] as DATE) BETWEEN CAST(av.[StartDate] as DATE) And CAST(av.[EndDate] as DATE)))
		INNER JOIN [Subzz_Users].[Users].[AvailabilityStatus] st ON st.[AvailabilityStatusId] = av.[AvailabilityStatusId]
	WHERE av.[IsArchived] = 0

	SELECT * FROM @Availability a

	ORDER BY a.UserId, a.StartDate

END

GO
/****** Object:  StoredProcedure [Users].[GetSubstituteAvailabilitiesSummary]    Script Date: 7/2/2019 5:38:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Shahzada Ghazanfar>
-- Create date: <04/11/2019>
-- Description:	<Substitute Availabilities>
-- =============================================

-- Exec [Users].[GetSubstituteAvailabilitiesSummary] 

ALTER PROCEDURE [Users].[GetSubstituteAvailabilitiesSummary] 

@DistrictId		VARCHAR(10) = NULL,
@OrganizationId VARCHAR(10) = NULL

AS
BEGIN	

	DECLARE @COLS AS NVARCHAR(MAX),
    		@QUERY  AS NVARCHAR(MAX),
			@StartDate DATETIME = GETDATE(),
			@EndDate    DATETIME = NULL
		
	IF(ISNULL(@StartDate, '') = '')
	BEGIN
		SET @StartDate = GETDATE()
	END
	ELSE 

	SELECT @StartDate = dateadd(day, 1-datepart(dw, @StartDate), CONVERT(date, @StartDate))
		,@EndDate = dateadd(day, 7-datepart(dw, @StartDate), CONVERT(date, @StartDate))

	
	DECLARE @Availability 		TABLE 
	(
		Title	varchar(300),
		StartDate Datetime,
		EndDate datetime,
		Number	int
	)
		INSERT INTO @Availability VALUES
		('Pending Requests', null, null, (select count(*) FROM [Subzz_Leaves].[Absence].[Absence] a WHERE  
		(ISNULL(@DistrictId, 0) = 0 OR a.District_Id = @DistrictId)
		AND (ISNULL(@OrganizationId, '') = '' OR a.School_Id = @OrganizationId)
		AND a.IsApproved = 0
		AND (cast(a.CreatedDate as date) BETWEEN cast(GetDate() as date) AND cast(GetDate() as date)))),

		('Employees Off Today', GetDate(), GetDate(), 0),
		('Employees Off This Week', @StartDate, @EndDate, 0)

	SELECT @StartDate = DATEADD(day, 1, @EndDate)
		,@EndDate = dateadd(day, 7-datepart(dw, @StartDate), CONVERT(date, @StartDate))

		INSERT INTO @Availability VALUES
		('Employees Off Next Week', @StartDate, @EndDate, 0)

	
	UPDATE a 
	SET
	a.Number = ISNULL(cnt.Number, 0)
	FROM @Availability AS a
	OUTER APPLY (
					SELECT COUNT(DISTINCT av.[UserId]) as Number
					FROM 
					[Subzz_Users].[Users].[Availability] av
					
					Left JOIN [Subzz_Users].[Users].[UserLocation] absence on absence.[User_Id] = av.[UserId]

					WHERE CAST(av.[StartDate] as DATE) >= CAST(a.[StartDate] as DATE) AND CAST(av.[StartDate] as DATE) <= CAST(a.[EndDate] as DATE)
					AND (ISNULL(@DistrictId, 0) = 0 OR absence.[Location_Id] = @DistrictId)
					AND (ISNULL(@OrganizationId, '') = '' OR absence.[Location_Id] = @OrganizationId) 				

				) cnt
	WHERE a.[StartDate] IS NOT NULL

	SELECT * FROM @Availability a

END

