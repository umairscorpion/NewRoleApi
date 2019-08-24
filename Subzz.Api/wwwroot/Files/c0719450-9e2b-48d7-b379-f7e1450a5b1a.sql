USE [Subzz_Users]
GO
/****** Object:  StoredProcedure [Users].[InsertAvailability]    Script Date: 8/8/2019 12:25:32 PM ******/
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
	@EndsOnAfterNumberOfOccurrance INT,
	@EndsOnUntilDate		VARCHAR(50),
	@Notes					VARCHAR(3000),
	@CreatedBy				VARCHAR(10),
	@IsEndsOnDate			BIT,
	@IsEndsOnAfterNumberOfOccurrance BIT,
	@EndDateAfterNumberOfOccurrances VARCHAR(50)
AS
BEGIN
	DECLARE @Unavailable   INT	= 0
	DECLARE @ForRecurring  INT	= 0
	DECLARE @AbsenceCount1 INT	= 0
	DECLARE @AbsenceCount2 INT	= 0

	SELECT @AbsenceCount1 = COUNT(ABSS.Absence_Id) FROM [Subzz_Leaves].[Absence].[AbsenceSchedule] ABSS
	Inner Join [Subzz_Leaves].Absence.Absence A on A.Absence_Id = ABSS.Absence_Id
	WHERE 
			A.Substitute_Id = @UserId AND @AvailabilityStatusId IN (1,2)
		AND ((CAST(ABSS.StartDate as DATE) BETWEEN CAST(@StartDate as DATE) and CAST(@EndDate as DATE)) 
		OR	(CAST(ABSS.EndDate AS DATE) BETWEEN CAST(@StartDate as DATE) AND CAST(@EndDate AS DATE))) 
		AND ((CAST(@StartTime as TIME) > CAST(ABSS.StartDate AS TIME)	 AND CAST(@StartTime as TIME) < CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@EndTime AS TIME) > CAST(ABSS.StartDate AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime as TIME) <= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) >= CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime as TIME) >= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) <= CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime as TIME) < CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) > CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime as TIME) = CAST(@EndTime AS TIME))) 
		AND	A.AbsenceStatus_Id = 2
	
	SELECT @AbsenceCount2 = COUNT(ABSS.Absence_Id) FROM [Subzz_Leaves].[Absence].[AbsenceSchedule] ABSS
	Inner Join [Subzz_Leaves].Absence.Absence A on A.Absence_Id = ABSS.Absence_Id
	WHERE 
			A.Substitute_Id = @UserId  AND A.AbsenceStatus_Id = 2 AND @AvailabilityStatusId = 3
		AND  (CAST(ABSS.StartDate as DATE) BETWEEN (CAST(@StartDate as DATE)) AND (CAST(@EndsOnUntilDate as DATE))
		OR   CAST(ABSS.StartDate as DATE) BETWEEN (CAST(@StartDate as DATE)) AND (CAST(@EndDateAfterNumberOfOccurrances as DATE)))
		AND  DATEPART(WEEKDAY, ABSS.StartDate) = (@RepeatOnWeekDays + 1)
		AND ((CAST(@StartTime as TIME) > CAST(ABSS.StartDate AS TIME)	 AND CAST(@StartTime as TIME) < CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@EndTime AS TIME) > CAST(ABSS.StartDate AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime as TIME) <= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) >= CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime as TIME) >= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) <= CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime as TIME) < CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) > CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime as TIME) = CAST(@EndTime AS TIME))) 
	
	SELECT @Unavailable = COUNT(AV.AvailabilityId) FROM   [Subzz_Users].[Users].[Availability] AV
	WHERE 
			AV.UserId = @UserId AND AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (1,2)
		AND	((@AvailabilityStatusId IN (1,2) 
		AND ((CAST(@StartDate as DATE) BETWEEN CAST(AV.StartDate as DATE) AND CAST(AV.EndDate as DATE)) 
		OR	(CAST(@EndDate AS DATE) BETWEEN CAST(AV.StartDate as DATE)	 AND CAST(AV.EndDate AS DATE))))
		OR  (@AvailabilityStatusId IN (3) 
		AND ((@RepeatOnWeekDays + 1) BETWEEN DATEPART(WEEKDAY, CAST(AV.StartDate AS DATE)) AND DATEPART(WEEKDAY, CAST(AV.EndDate AS DATE)))
		AND ((CAST(AV.StartDate as DATE) BETWEEN CAST(@StartDate as DATE) AND CAST(@EndDateAfterNumberOfOccurrances as DATE)) 
		OR	(CAST(AV.EndDate AS DATE) BETWEEN CAST(@StartDate as DATE)	 AND CAST(@EndsOnUntilDate AS DATE))
		OR	(CAST(AV.StartDate AS DATE) BETWEEN CAST(@StartDate as DATE) AND CAST(@EndDateAfterNumberOfOccurrances AS DATE))
		OR	(CAST(AV.EndDate AS DATE) BETWEEN CAST(@StartDate as DATE)   AND CAST(@EndsOnUntilDate AS DATE))))) 
		AND ((CAST(@StartTime as TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@StartTime as TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime as TIME) <= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime as TIME) >= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime as TIME) < CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
		OR	(CAST(AV.StartTime as TIME) = CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime as TIME) = CAST(@EndTime AS TIME)))

	SELECT @ForRecurring = COUNT(AV.AvailabilityId) FROM   [Subzz_Users].[Users].[Availability] AV
	WHERE 
			AV.UserId = @UserId AND AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (3) 
		AND ((@AvailabilityStatusId IN (1,2) 
		AND ((AV.RepeatOnWeekDays + 1) BETWEEN DATEPART(WEEKDAY, CAST(@StartDate AS DATE)) AND DATEPART(WEEKDAY, CAST(@EndDate AS DATE))))
		OR  (@AvailabilityStatusId = 3 AND AV.RepeatOnWeekDays = @RepeatOnWeekDays))
		AND	((AV.IsEndsOnDate = 1 
		AND ((CAST(@StartDate AS DATE) BETWEEN AV.StartDate		AND AV.EndsOnUntilDate) 
		OR	(CAST(@EndDate AS DATE) BETWEEN AV.StartDate		AND AV.EndsOnUntilDate)))
		OR	(AV.IsEndsOnAfterNumberOfOccurrance = 1 
		AND ((CAST(@StartDate AS DATE) BETWEEN AV.StartDate		AND AV.EndDateAfterNumberOfOccurrances) 
		OR	(CAST(@EndDate AS DATE) BETWEEN AV.StartDate 		AND AV.EndDateAfterNumberOfOccurrances))))
		AND ((CAST(@StartTime as TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@StartTime as TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime as TIME) <= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime as TIME) >= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime as TIME) < CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
		OR	(CAST(AV.StartTime as TIME) = CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime as TIME) = CAST(@EndTime AS TIME)))

	IF(@AbsenceCount1 > 0 OR @AbsenceCount2 > 0)
	BEGIN
		SELECT 'accepted' AS ReturnText
	END
	ELSE
	BEGIN
		IF(@ForRecurring > 0 OR @Unavailable > 0)
		BEGIN
			SELECT 'unavailable' AS ReturnText
		END
		ELSE
		BEGIN
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
				[EndsOnAfterNumberOfOccurrance],
				[EndsOnUntilDate],
				[CreatedOn],
				[CreatedBy],
				[IsArchived],
				[IsEndsOnDate],
				[IsEndsOnAfterNumberOfOccurrance],
				[EndDateAfterNumberOfOccurrances]
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
				@EndsOnAfterNumberOfOccurrance,
				@EndsOnUntilDate,
				GETDATE(),
				@CreatedBy,
				0,
				@IsEndsOnDate,
				@IsEndsOnAfterNumberOfOccurrance,
				@EndDateAfterNumberOfOccurrances
			)
			SELECT SCOPE_IDENTITY() AS ReturnText		
		END
	END		
END  

GO
/****** Object:  StoredProcedure [Users].[UpdateAvailability]    Script Date: 8/8/2019 12:25:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		@Shahzada Ghazanfar
-- Create date: @03/30/2019
-- Description:	@Unavailability Calendar
-- =============================================

-- Exec [Users].[UpdateAvailability] 

ALTER PROCEDURE [Users].[UpdateAvailability]
	@AvailabilityId			INT,
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
	@EndsOnAfterNumberOfOccurrance INT,
	@EndsOnUntilDate		VARCHAR(50),
	@Notes					VARCHAR(3000),
	@ModifiedBy				VARCHAR(10),
	@IsEndsOnDate			BIT,
	@IsEndsOnAfterNumberOfOccurrance BIT,
	@EndDateAfterNumberOfOccurrances VARCHAR(50)
AS
BEGIN
	DECLARE @Unavailable	INT	= 0
	DECLARE @ForRecurring	INT	= 0
	DECLARE @AbsenceCount1	INT	= 0
	DECLARE @AbsenceCount2	INT	= 0

	SELECT @AbsenceCount1 = COUNT(ABSS.Absence_Id) FROM [Subzz_Leaves].[Absence].[AbsenceSchedule] ABSS
	Inner Join [Subzz_Leaves].Absence.Absence A on A.Absence_Id = ABSS.Absence_Id
	WHERE 
			A.Substitute_Id = @UserId AND @AvailabilityStatusId IN (1,2) AND A.AbsenceStatus_Id = 2
		AND ((CAST(ABSS.StartDate as DATE) BETWEEN CAST(@StartDate as DATE) AND CAST(@EndDate as DATE)) 
		OR	(CAST(ABSS.EndDate AS DATE) BETWEEN CAST(@StartDate as DATE) AND CAST(@EndDate AS DATE))) 
		AND ((CAST(@StartTime as TIME) > CAST(ABSS.StartDate AS TIME)	 AND CAST(@StartTime as TIME) < CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@EndTime AS TIME) > CAST(ABSS.StartDate AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime as TIME) <= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) >= CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime as TIME) >= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) <= CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime as TIME) < CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) > CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime as TIME) = CAST(@EndTime AS TIME))) 
		
	SELECT @AbsenceCount2 = COUNT(ABSS.Absence_Id) FROM [Subzz_Leaves].[Absence].[AbsenceSchedule] ABSS
	Inner Join [Subzz_Leaves].Absence.Absence A on A.Absence_Id = ABSS.Absence_Id
	WHERE 
			A.Substitute_Id = @UserId  AND A.AbsenceStatus_Id = 2 AND @AvailabilityStatusId = 3
		AND  (CAST(ABSS.StartDate as DATE) BETWEEN (CAST(@StartDate as DATE)) AND (CAST(@EndsOnUntilDate as DATE))
		OR   CAST(ABSS.StartDate as DATE) BETWEEN (CAST(@StartDate as DATE)) AND (CAST(@EndDateAfterNumberOfOccurrances as DATE)))
		AND  DATEPART(WEEKDAY, ABSS.StartDate) = (@RepeatOnWeekDays + 1)
		AND ((CAST(@StartTime as TIME) > CAST(ABSS.StartDate AS TIME)	 AND CAST(@StartTime as TIME) < CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@EndTime AS TIME) > CAST(ABSS.StartDate AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime as TIME) <= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) >= CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime as TIME) >= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) <= CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime as TIME) < CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) > CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime as TIME) = CAST(@EndTime AS TIME))) 

	SELECT @Unavailable = COUNT(AV.AvailabilityId) FROM   [Subzz_Users].[Users].[Availability] AV 
	WHERE 
			AV.UserId = @UserId AND AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (1,2) AND AV.AvailabilityId <> @AvailabilityId
		AND	((@AvailabilityStatusId IN (1,2) 
		AND ((CAST(@StartDate as DATE) BETWEEN CAST(AV.StartDate as DATE) AND CAST(AV.EndDate as DATE)) 
		OR	(CAST(@EndDate AS DATE) BETWEEN CAST(AV.StartDate as DATE)	 AND CAST(AV.EndDate AS DATE))))
		OR  (@AvailabilityStatusId IN (3) 
		AND ((@RepeatOnWeekDays + 1) BETWEEN DATEPART(WEEKDAY, CAST(AV.StartDate AS DATE)) AND DATEPART(WEEKDAY, CAST(AV.EndDate AS DATE)))
		AND ((CAST(AV.StartDate as DATE) BETWEEN CAST(@StartDate as DATE) AND CAST(@EndDateAfterNumberOfOccurrances as DATE)) 
		OR	(CAST(AV.EndDate AS DATE) BETWEEN CAST(@StartDate as DATE)	 AND CAST(@EndsOnUntilDate AS DATE))
		OR	(CAST(AV.StartDate AS DATE) BETWEEN CAST(@StartDate as DATE) AND CAST(@EndDateAfterNumberOfOccurrances AS DATE))
		OR	(CAST(AV.EndDate AS DATE) BETWEEN CAST(@StartDate as DATE)   AND CAST(@EndsOnUntilDate AS DATE))))) 
		AND ((CAST(@StartTime as TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@StartTime as TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime as TIME) <= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime as TIME) >= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime as TIME) < CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
		OR	(CAST(AV.StartTime as TIME) = CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime as TIME) = CAST(@EndTime AS TIME)))

	SELECT @ForRecurring = COUNT(AV.AvailabilityId) FROM   [Subzz_Users].[Users].[Availability] AV
	WHERE 
			AV.UserId = @UserId AND AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (3) AND AV.AvailabilityId <> @AvailabilityId
		AND ((@AvailabilityStatusId IN (1,2) 
		AND ((AV.RepeatOnWeekDays + 1) BETWEEN DATEPART(WEEKDAY, CAST(@StartDate AS DATE)) AND DATEPART(WEEKDAY, CAST(@EndDate AS DATE))))
		OR  (@AvailabilityStatusId = 3 AND AV.RepeatOnWeekDays = @RepeatOnWeekDays))
		AND	((AV.IsEndsOnDate = 1 
		AND ((CAST(@StartDate AS DATE) BETWEEN AV.StartDate		AND AV.EndsOnUntilDate) 
		OR	(CAST(@EndDate AS DATE) BETWEEN AV.StartDate		AND AV.EndsOnUntilDate)))
		OR	(AV.IsEndsOnAfterNumberOfOccurrance = 1 
		AND ((CAST(@StartDate AS DATE) BETWEEN AV.StartDate		AND AV.EndDateAfterNumberOfOccurrances) 
		OR	(CAST(@EndDate AS DATE) BETWEEN AV.StartDate 		AND AV.EndDateAfterNumberOfOccurrances))))
		AND ((CAST(@StartTime as TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@StartTime as TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime as TIME) <= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime as TIME) >= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime as TIME) < CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
		OR	(CAST(AV.StartTime as TIME) = CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime as TIME) = CAST(@EndTime AS TIME)))

	IF(@AbsenceCount1 > 0 OR @AbsenceCount2 > 0)
	BEGIN
		SELECT 'accepted' AS ReturnText
	END

	ELSE
	BEGIN
		IF(@ForRecurring > 0 OR @Unavailable > 0)
		BEGIN
			SELECT 'unavailable' AS ReturnText
		END
		ELSE
		BEGIN
			UPDATE	 [Users].[Availability]
			SET		 [UserId]				= @UserId
					,[AvailabilityStatusId] = @AvailabilityStatusId
					,[Title]				= @Title
					,[StartDate]			= @StartDate
					,[EndDate]				= @EndDate
					,[StartTime]			= @StartTime
					,[EndTime]				= @EndTime
					,[IsAllDayOut]			= @IsAllDayOut
					,[IsRepeat]				= @IsRepeat
					,[RepeatType]			= @RepeatType
					,[RepeatValue]			= @RepeatValue
					,[RepeatOnWeekDays]		= @RepeatOnWeekDays
					,[EndsOnAfterNumberOfOccurrance] = @EndsOnAfterNumberOfOccurrance
					,[EndsOnUntilDate]		= @EndsOnUntilDate
					,[Notes]				= @Notes
					,[ModifiedOn]			= GETDATE()
					,[ModifiedBy]			= @ModifiedBy
					,[IsEndsOnDate]			= @IsEndsOnDate
					,[IsEndsOnAfterNumberOfOccurrance] = @IsEndsOnAfterNumberOfOccurrance
					,[EndDateAfterNumberOfOccurrances] = @EndDateAfterNumberOfOccurrances
			WHERE	 AvailabilityId			= @AvailabilityId
			SELECT @AvailabilityId AS ReturnText
		END
	END
END  

GO
/****** Object:  StoredProcedure [Users].[DeleteAvailability]    Script Date: 8/8/2019 12:25:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		@Shahzada Ghazanfar
-- Create date: @03/30/2019
-- Description:	@Unavailability Calendar
-- =============================================

-- Exec [Users].[DeleteAvailability] 

ALTER PROCEDURE [Users].[DeleteAvailability]
	@AvailabilityId			INT,
	@ArchivedBy				VARCHAR(10)
AS
BEGIN
	
	UPDATE	 [Users].[Availability]
	SET		 [IsArchived]	= 1
			,[ArchivedOn]	= GETDATE()
			,[ArchivedBy]	= @ArchivedBy
	WHERE	 AvailabilityId	= @AvailabilityId
	
	SELECT [AvailabilityId]
	FROM   [Subzz_Users].[Users].[Availability]
	WHERE  [AvailabilityId] = @AvailabilityId
END  

GO
/****** Object:  StoredProcedure [Users].[InsertFile]    Script Date: 8/8/2019 12:27:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Adnan Ali>
-- Create date: <05/16/2019>
-- Description:	<To insert Files>
-- =============================================
ALTER PROCEDURE [Users].[InsertFile] 
@FileName				VARCHAR(500),
@OriginalFileName		VARCHAR(500),
@FileExtentione			VARCHAR(50),
@FileContentType		VARCHAR(50),
@UserId					VARCHAR(10),
@DistrictId				INT = NULL,
@OrganizationId			VARCHAR(10),
@FileType				INT = NULL

AS
BEGIN
		
	INSERT INTO [Subzz_Users].[Users].[File]
           ([FileName]
           ,[OriginalFileName]
		   ,[Date]
           ,[FileExtention]
           ,[ContentType]
		   ,[User_Id]
		   ,[District_Id]
		   ,[School_Id]
		   ,[IsFileExist]
		   ,[FileType_Id])
     VALUES
           (@FileName
		   ,@OriginalFileName
           ,GETDATE()
           ,@FileExtentione
		   ,@FileContentType
		   ,@UserId
		   ,@DistrictId
		   ,@OrganizationId
		   ,1
		   ,@FileType)

	If(@FileType = 1)
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM   [Subzz_Users].[Users].[File] 
	WHERE	[User_Id]			= @UserId
			AND	[District_Id]	= @DistrictId
			AND	[IsFileExist]	= 1
			AND [FileType_Id]	IN (1)

	ORDER BY [Date] DESC
	END

	If(@FileType IN (2,3,4))
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM   [Subzz_Users].[Users].[File] 
	WHERE		[IsFileExist]	= 1
			AND [FileType_Id]	IN (2,3,4)

	ORDER BY [Date] DESC
	END

	If(@FileType IN (6))
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM   [Subzz_Users].[Users].[File] 
	WHERE		[IsFileExist]	= 1
			AND [FileType_Id]	IN (6)

	ORDER BY [Date] DESC
	END

END

GO
/****** Object:  StoredProcedure [Users].[GetFiles]    Script Date: 8/8/2019 12:27:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Adnan Ali>
-- Create date: <05/16/2019>
-- Description:	<To Get Files>
-- =============================================
ALTER PROCEDURE [Users].[GetFiles] 
@UserId			VARCHAR(10),
@DistrictId		INT = NULL,
@FileType		VARCHAR(20)
AS
BEGIN

	DECLARE @UserRoleId INT
	DECLARE @UserLevelId INT

	SELECT @UserRoleId = USROLE.Role_Id FROM Users.Users USR 
	INNER JOIN Users.UserRole USROLE ON USR.User_Id = USROLE.User_Id 
	WHERE USR.User_Id = @UserId

	SELECT @UserLevelId = UL.UserLevel_Id FROM Users.Users USR 
	INNER JOIN Users.UserLocation UL ON USR.User_Id = UL.User_Id 
	WHERE USR.User_Id = @UserId


	If(@FileType = 'Guides')
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM	[Subzz_Users].[Users].[File] 
	WHERE	[IsFileExist]	= 1
			AND [FileType_Id]	IN (2,3,4)

	ORDER BY [Date] DESC
	END

	If(@FileType = 'Substitute Files')
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM	[Subzz_Users].[Users].[File] 
	WHERE	[User_Id]			= @UserId
			AND	[District_Id]	= @DistrictId
			AND	[IsFileExist]	= 1
			AND [FileType_Id]	IN (1)

	ORDER BY [Date] DESC
	END

	If(@FileType = 'School Files')
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM	[Subzz_Users].[Users].[File] 
	WHERE	[District_Id]	= @DistrictId
			AND	[IsFileExist]	= 1
			AND [FileType_Id]	IN (6)

	ORDER BY [Date] DESC
	END

END

GO
/****** Object:  StoredProcedure [Users].[DeleteFiles]    Script Date: 8/8/2019 12:27:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Adnan Ali>
-- Create date: <05/16/2019>
-- Description:	<To Delete Files>
-- =============================================
ALTER PROCEDURE [Users].[DeleteFiles] 
@UserId			VARCHAR(10),
@DistrictId		INT = NULL,
@FileName		VARCHAR(500),
@FileType		VARCHAR(20)

AS
BEGIN
	UPDATE	[Subzz_Users].[Users].[File] 
	SET		[IsFileExist] = 0

	WHERE	[FileName]	= @FileName

	If(@FileType = 'Guides')
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM   [Subzz_Users].[Users].[File] 
	WHERE		[IsFileExist]	= 1
			AND [FileType_Id]	IN (2,3,4)

	ORDER BY [Date] DESC
	END

	If(@FileType = 'Substitute Files')
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM   [Subzz_Users].[Users].[File] 
	WHERE	[User_Id]			= @UserId
			AND	[District_Id]	= @DistrictId
			AND	[IsFileExist]	= 1
			AND [FileType_Id]	IN (1)

	ORDER BY [Date] DESC
	END

	If(@FileType = 'School Files')
	BEGIN
	SELECT	 [FileName]			AS FileName
			,[OriginalFileName] AS OriginalFileName
			,[Date]				AS Date
			,[FileExtention]	AS FileExtention
			,[ContentType]		AS FileContentType
			,[FileType_Id]		AS FileType

	FROM	[Subzz_Users].[Users].[File] 
	WHERE	[District_Id]	= @DistrictId
			AND	[IsFileExist]	= 1
			AND [FileType_Id]	IN (6)

	ORDER BY [Date] DESC
	END

END

GO
/****** Object:  StoredProcedure [Users].[sp_getSchoolSubList]    Script Date: 8/8/2019 12:30:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--Exec [Users].[sp_getSchoolSubList] 'dfdf', 19
ALTER PROCEDURE [Users].[sp_getSchoolSubList] 
@UserId		VARCHAR(10),
@DistrictId INT

AS
BEGIN
	SELECT	USR.User_Id AS SubstituteId, SUBLIST.SchoolSubstituteList_Id AS Id, 
			USR.FirstName + ' ' + USR.LastName AS SubstituteName,
			ISNULL(SUBLIST.IsAdded, 0) AS IsAdded, SUBLIST.AddedByUserId AS AddedByUserId
			 
	FROM	Users.Users USR
			Left JOIN Users.SchoolSubstituteList SUBLIST ON SUBLIST.SubstituteId = USR.User_Id

	WHERE	USR.UserRole_Id = 4 
			AND (ISNULL(@DistrictId, 0) = 0 OR USR.District_Id = @DistrictId)
END

GO
/****** Object:  StoredProcedure [Users].[sp_getBlockedSchoolSubList]    Script Date: 8/8/2019 12:30:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--Exec [Users].[sp_getBlockedSchoolSubList] 'dfdf', 19
ALTER PROCEDURE [Users].[sp_getBlockedSchoolSubList] 
@UserId		VARCHAR(10),
@DistrictId INT

AS
BEGIN
	SELECT	USR.User_Id AS SubstituteId, SUBLIST.BlockedSchoolSubstituteList_Id AS Id, 
			USR.FirstName + ' ' + USR.LastName as SubstituteName,
			ISNULL(SUBLIST.IsAdded, 0) AS IsAdded, SUBLIST.AddedByUserId AS AddedByUserId 

	FROM	Users.Users USR
			Left JOIN Users.BlockedSchoolSubstituteList SUBLIST ON SUBLIST.SubstituteId = USR.User_Id

	WHERE	USR.UserRole_Id = 4 
			AND (ISNULL(@DistrictId, 0) = 0 OR USR.District_Id = @DistrictId)
END

GO
/****** Object:  StoredProcedure [Users].[GetAvailableSubstitutes]    Script Date: 8/8/2019 12:31:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Users].[GetAvailableSubstitutes]

@DistrictId INT,
@StartDate DATETIME,
@EndDate DATETIME,
@StartTime TIME(7),
@EndTime TIME(7)

AS

BEGIN TRY
IF OBJECT_ID('tempdb..#tempsub') is not null
	BEGIN
		DROP TABLE  #tempsub
	END

	CREATE TABLE #tempsub (Substitute_Id nvarchar(10))
	
	INSERT INTO #tempsub 

	SELECT SCHEDULE.SubstituteId AS SubstituteId FROM Subzz_Leaves.Absence.AbsenceSchedule SCHEDULE
			Inner join Users.Users USR ON USR.User_Id = SCHEDULE.SubstituteId
			Inner join Subzz_Leaves.Absence.Absence ABSC ON ABSC.Absence_Id = SCHEDULE.Absence_Id

	WHERE	((CAST(SCHEDULE.StartDate AS DATE) between @StartDate AND @EndDate) OR 
			(CAST(SCHEDULE.EndDate AS DATE) between @StartDate AND @EndDate)) AND 
			((@StartTime > ABSC.StartTime AND @StartTime < ABSC.Endtime) OR 
			(@EndTime > ABSC.StartTime AND @EndTime < ABSC.Endtime) OR 
			(@StartTime <= StartTime And @EndTime >= Endtime) OR 
			(@StartTime >= ABSC.StartTime AND @EndTime <= ABSC.Endtime)) AND 
			USR.District_Id =  @DistrictId
			
	UNION 

	SELECT AV.[UserId] AS SubstituteId FROM   [Subzz_Users].[Users].[Availability] AV
	WHERE 
				AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (1,2)
			AND	((CAST(@StartDate as DATE) BETWEEN CAST(AV.StartDate as DATE)AND CAST(AV.EndDate as DATE)) 
			OR	(CAST(@EndDate AS DATE) BETWEEN CAST(AV.StartDate as DATE)	 AND CAST(AV.EndDate AS DATE))) 
			AND ((CAST(@StartTime as TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@StartTime as TIME) < CAST(AV.EndTime AS TIME)) 
			OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
			OR	(CAST(@StartTime as TIME) <= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
			OR	(CAST(@StartTime as TIME) >= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
			OR	(CAST(@StartTime as TIME) < CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
			OR	(CAST(AV.StartTime as TIME) = CAST(AV.EndTime AS TIME)))

	UNION

	SELECT AV.[UserId] AS SubstituteId FROM   [Subzz_Users].[Users].[Availability] AV
	WHERE 
				AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (3)
			AND (AV.RepeatOnWeekDays + 1) BETWEEN DATEPART(WEEKDAY, CAST(@StartDate AS DATE)) AND DATEPART(WEEKDAY, CAST(@EndDate AS DATE))
			AND	((AV.IsEndsOnDate = 1 
			AND ((CAST(@StartDate AS DATE) BETWEEN AV.StartDate		AND AV.EndsOnUntilDate) 
			OR	(CAST(@EndDate AS DATE) BETWEEN AV.StartDate		AND AV.EndsOnUntilDate)))
			OR	(AV.IsEndsOnAfterNumberOfOccurrance = 1 
			AND ((CAST(@StartDate AS DATE) BETWEEN AV.StartDate		AND AV.EndDateAfterNumberOfOccurrances) 
			OR	(CAST(@EndDate AS DATE) BETWEEN AV.StartDate 		AND AV.EndDateAfterNumberOfOccurrances))))
			AND ((CAST(@StartTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@StartTime AS TIME) < CAST(AV.EndTime AS TIME)) 
			OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
			OR	(CAST(@StartTime AS TIME) <= CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
			OR	(CAST(@StartTime AS TIME) >= CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
			OR	(CAST(@StartTime AS TIME) < CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
			OR	(CAST(AV.StartTime AS TIME) = CAST(AV.EndTime AS TIME)))

	SELECT	usr.User_Id AS UserId, usr.[FirstName] + ' ' +  usr.[LastName] AS FirstName,
			usr.UserType_Id AS UserTypeId, usr.User_Id AS UserId, USRL.UserLevel_Id as UserLevel, 
			USRROLE.Role_Id AS RoleId, usr.District_Id AS DistrictId, usr.IsActive AS IsActive,
			usr.ProfilePicUrl AS ProfilePicture

	FROM [Users].[Users] usr
			INNER JOIN UserLocation USRL ON USRL.User_Id = usr.User_Id 
			INNER JOIN UserRole USRROLE ON USRROLE.User_Id = usr.User_Id

	WHERE	usr.District_Id = @DistrictId AND USRROLE.Role_Id = 4 And usr.IsActive = 1
			AND usr.User_Id NOT IN (SELECT  Substitute_Id FROM #tempsub where Substitute_Id IS NOT NULL)

 End Try
 BEGIN CATCH
END CATCH

GO
/****** Object:  StoredProcedure [Users].[GetAuditLog]    Script Date: 8/8/2019 12:32:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Adnan Ali>
-- Create date: <05/02/2019>
-- Description:	<To get audit log>
-- =============================================
ALTER PROCEDURE [Users].[GetAuditLog] 

@StartDate 				DATETIME = NULL,
@EndDate 				DATETIME = NULL,
@LoginUserId			VARCHAR(10) = NULL,
@SearchByEmployeeName	VARCHAR(10) = NULL,
@DistrictId				INT = NULL,
@OrganizationId			VARCHAR(10) = NULL

AS
BEGIN

	DECLARE @loginedUserLevel INT = 0
	SELECT @loginedUserLevel = (SELECT TOP 1 USR.UserLevel_Id FROM [Subzz_Users].[Users].[UserLocation] USR where USR.User_Id = @LoginUserId)
	
	SELECT	   [Id]
			  ,[OccurredOn]
			  ,uEmp.FirstName + ' ' + uEmp.LastName  AS [User]
			  ,[Event] = [ActionType] + ' ' + 
			  CASE WHEN [ActionType] = 'View'				THEN ' Job # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'Absence'			THEN ' Absence # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'District'			THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'School'				THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'Staff'				THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'Substitute'			THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'LeaveType'			THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'Allowances'			THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'ChangedPassword'	THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'PayRate'			THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'PayRateRule'		THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'Unavailability'		THEN ' # ' + [EntityId] ELSE '' END + '' +
			  CASE WHEN [EntityType] = 'Announcement'		THEN ' # ' + [EntityId] ELSE '' END

		FROM   [Users].[AuditLogs] audit
		INNER JOIN [Subzz_Users].[Users].Users uEmp ON uEmp.[User_Id] = audit.[UserId]
		WHERE	

			(@loginedUserLevel = 4 OR audit.District_Id = @DistrictId)
		AND (@loginedUserLevel IN (1, 4) OR audit.School_Id = @OrganizationId)
		AND ((ISNULL(@StartDate, '') = '' AND ISNULL(@EndDate, '') = '') OR 
			(CAST(audit.OccurredOn AS DATE) BETWEEN  CAST(@StartDate AS DATE) AND CAST(@EndDate AS DATE)) OR 
			(CAST(audit.OccurredOn AS DATE) BETWEEN CAST(@StartDate AS DATE) AND CAST(@EndDate AS DATE))) 
		AND (ISNULL(@SearchByEmployeeName, '') = '' OR LOWER(uEmp.FirstName) 
			LIKE @SearchByEmployeeName + '%' OR LOWER(uEmp.LastName) LIKE @SearchByEmployeeName + '%')

		ORDER BY audit.OccurredOn DESC

END

GO
USE [Subzz_Leaves]
GO
/****** Object:  StoredProcedure [Job].[AcceptJob]    Script Date: 8/8/2019 1:02:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[Job].[AcceptJob] 'U000000012', 2182, 'WebApp'

ALTER PROCEDURE [Job].[AcceptJob]
@SubstituteId varchar(10),
@AbsenceId bigint,
@AcceptVia nvarchar(50)

AS
BEGIN

	DECLARE @AbsenceCount int , @UserId varchar(10), @AbsenceDistrict int, @EmployeeId varchar(10) 

	DECLARE @StartTime Time(7), @EndTime Time(7), @StartDate date, @EndDate date

	DECLARE @SchStartDate datetime, @SchEndDate datetime

	SELECT @StartTime= StartTime, @EndTime = EndTime, @StartDate= StartDate, @EndDate = EndDate FROM Absence.Absence a 
	WHERE a.Absence_Id =  @AbsenceId

	SELECT @SchStartDate = StartDate, @SchEndDate = EndDate FROM Absence.AbsenceSchedule WHERE Absence_Id =  @AbsenceId

	SET @AbsenceCount = 0
	SET @AbsenceDistrict = 0

	--UserId of that user who created this Absence 
	SELECT @EmployeeId = User_Id FROM Absence.Absence WHERE Absence_Id = @AbsenceId

	--Getting District of user that created this Absence
	SELECT @AbsenceDistrict = District_Id FROM Absence.Absence WHERE Absence_Id = @AbsenceId

	--To verify that this substitute is Active
	SELECT @UserId = User_Id FROM Subzz_Users.Users.Users WHERE  User_Id = @SubstituteId And IsActive = 1

	IF LEN(@UserId) = 10
	BEGIN
		IF exists(SELECT Blocked.User_Id FROM  Subzz_Users.Users.BlockedSubstitute AS Blocked 
				  WHERE Blocked.User_Id = @EmployeeId AND Blocked.Substitute_Id = @SubstituteId)
		BEGIN
			SELECT 'Blocked' as ReturnText , 0 as Sts
		END

		IF ((SELECT AbsenceStatus_Id from Absence.Absence WHERE Absence_Id = @AbsenceId) = 2)
		BEGIN
			Select 'Accepted' as ReturnText, 0 as Sts
		END

		ELSE IF ((SELECT AbsenceStatus_Id FROM Absence.Absence WHERE Absence_Id = @AbsenceId) = 4)
		BEGIN
			SELECT 'Cancelled' as ReturnText, 0 as Sts
		END

		ELSE IF (
				(SELECT COUNT(AV.AvailabilityId) FROM   [Subzz_Users].[Users].[Availability] AV
				WHERE 
					AV.UserId = @UserId AND AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (1,2)
				AND	((CAST(@StartDate as DATE) BETWEEN CAST(AV.StartDate AS DATE)AND CAST(AV.EndDate AS DATE)) 
				OR	(CAST(@EndDate AS DATE) BETWEEN CAST(AV.StartDate AS DATE)	 AND CAST(AV.EndDate AS DATE))) 
				AND ((CAST(@StartTime as TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@StartTime AS TIME) < CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@StartTime as TIME) <= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
				OR	(CAST(@StartTime as TIME) >= CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@StartTime as TIME) < CAST(AV.StartTime as TIME)		 AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
				OR	(CAST(AV.StartTime as TIME) = CAST(AV.EndTime AS TIME)))) > 0 )
		BEGIN
			SELECT 'Unavailable' as ReturnText, 0 as Sts
		END

		ELSE IF (
				(SELECT COUNT(AV.AvailabilityId) FROM   [Subzz_Users].[Users].[Availability] AV
				WHERE 
					AV.UserId = @UserId AND AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (3)
				AND DATEPART(WEEKDAY, CAST(@SchStartDate AS DATE)) = AV.RepeatOnWeekDays + 1
				AND	((AV.IsEndsOnDate = 1 
				AND (CAST(@SchStartDate AS DATE) BETWEEN AV.StartDate AND AV.EndsOnUntilDate)) 
				OR	(AV.IsEndsOnAfterNumberOfOccurrance = 1 
				AND (CAST(@SchStartDate AS DATE) BETWEEN AV.StartDate AND AV.EndDateAfterNumberOfOccurrances))) 
				AND ((CAST(@SchStartDate AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@SchStartDate AS TIME) < CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@SchEndDate AS TIME) > CAST(AV.StartTime AS TIME)			 AND CAST(@SchEndDate AS TIME) < CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@SchStartDate AS TIME) <= CAST(AV.StartTime AS TIME)		 AND CAST(@SchEndDate AS TIME) >= CAST(AV.EndTime AS TIME))
				OR	(CAST(@SchStartDate AS TIME) >= CAST(AV.StartTime AS TIME)		 AND CAST(@SchEndDate AS TIME) <= CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@SchStartDate AS TIME) < CAST(AV.StartTime AS TIME)		 AND CAST(@SchEndDate AS TIME) > CAST(AV.EndTime AS TIME))
				OR	(CAST(AV.StartTime AS TIME) = CAST(AV.EndTime AS TIME)))) > 0 )
		BEGIN
			SELECT 'Unavailable' as ReturnText, 0 as Sts
		END

		ELSE
		BEGIN
			If EXISTS(SELECT Absence_Id FROM [Absence].[AbsenceSchedule] WHERE Absence_Id = @AbsenceId)
			BEGIN
				--Check If Substitute Already Accepted Absence within that dates and time
				SELECT @AbsenceCount = count(*) FROM [Absence].[AbsenceSchedule] ABSS
				Inner Join Absence.Absence A on A.Absence_Id = ABSS.Absence_Id
				WHERE 
					A.Substitute_Id = @UserId AND ((CAST(ABSS.StartDate as DATE) between CAST(@StartDate as DATE) and CAST(@EndDate as DATE)) 
				OR	(CAST(ABSS.EndDate AS DATE) BETWEEN CAST(@StartDate as DATE) AND CAST(@EndDate AS DATE))) 
				AND ((CAST(@StartTime as TIME) > CAST(ABSS.StartDate AS TIME)	 AND CAST(@StartTime as TIME) < CAST(ABSS.EndDate AS TIME))
				OR	(CAST(@EndTime AS TIME) > CAST(ABSS.StartDate AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(ABSS.EndDate AS TIME)) 
				OR	(CAST(@StartTime as TIME) <= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) >= CAST(ABSS.EndDate AS TIME))
				OR	(CAST(@StartTime as TIME) >= CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) <= CAST(ABSS.EndDate AS TIME)) 
				OR	(CAST(@StartTime as TIME) < CAST(ABSS.StartDate as TIME)	 AND CAST(@EndTime AS TIME) > CAST(ABSS.EndDate AS TIME))) 
				AND	A.AbsenceStatus_Id = 2
			
				-- If Count is Greater Than 0 than Its Overlapping
				IF (@AbsenceCount > 0)
				BEGIN
					SELECT 'Conflict' as ReturnText, 0 as Sts 
				END

				ELSE
				BEGIN
					--Update [szn_Absence] Table
					UPDATE	[Absence].[Absence] 
					SET		[AbsenceStatus_Id] = 2, [AcceptedDate] = GETDATE(), Substitute_Id = @UserId, [AcceptedVia] = @AcceptVia	 
					WHERE	[Absence_Id] = @AbsenceId And AbsenceStatus_Id = 1
		
					--Update [AbsenceBasicInfo] Table
					UPDATE	[Absence].[AbsenceSchedule]
					SET		AbsenceStatus_Id = 2 , SubstituteId = @UserId
					WHERE	Absence_Id = @AbsenceId And AbsenceStatus_Id = 1
					SELECT	'success' as ReturnText, 1 as Sts 
				END
			END
		END
	END

END

GO
/****** Object:  StoredProcedure [Absence].[UpdateAbsence]    Script Date: 8/8/2019 1:03:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Absence].[UpdateAbsence] 
@AbsenceID				INT,
@StartDate				DATE,
@EndDate				DATE,
@StartTime				TIME(7),
@EndTime				TIME(7),
@LeaveType_Id			INT,
@IsSubstituteRequired	BIT,
@NotesToSubstitute		NVARCHAR(500),
@AbsenceStatus_Id		INT,
@AbsenceType_Id			INT,
@AnyAttachemt			BIT,
@User_Id				VARCHAR(10),
@UpdatedBy_User_Id		VARCHAR(10),
@Substitute_Id			VARCHAR(10),
@AbsenceDuration_Id		INT
As
BEGIN

	DECLARE @IsAlreadyExist BIT = 0
	DECLARE @Absences		INT = 0
	DECLARE @Unavailability1	INT = 0
	DECLARE @Unavailability2	INT = 0

	SELECT @Unavailability1 = COUNT(AV.AvailabilityId) FROM [Subzz_Users].[Users].[Availability] AV
	WHERE 
			AV.UserId = @Substitute_Id AND AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (1,2)
		AND	((CAST(@StartDate AS DATE) BETWEEN CAST(AV.StartDate AS DATE)AND CAST(AV.EndDate AS DATE)) 
		OR	(CAST(@EndDate AS DATE) BETWEEN CAST(AV.StartDate as DATE)	AND CAST(AV.EndDate AS DATE))) 
		AND ((CAST(@StartTime AS TIME) > CAST(AV.StartTime AS TIME)		AND CAST(@StartTime as TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime AS TIME) <= CAST(AV.StartTime AS TIME)		AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime AS TIME) >= CAST(AV.StartTime AS TIME)		AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime AS TIME) < CAST(AV.StartTime AS TIME)		AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
		OR	(CAST(AV.StartTime AS TIME) = CAST(AV.EndTime AS TIME)))

	SELECT @Unavailability2 = COUNT(AV.AvailabilityId) FROM   [Subzz_Users].[Users].[Availability] AV
	WHERE 
			AV.UserId = @Substitute_Id AND AV.IsArchived = 0 AND AV.AvailabilityStatusId IN (3)
		AND (AV.RepeatOnWeekDays + 1) BETWEEN DATEPART(WEEKDAY, CAST(@StartDate AS DATE)) AND DATEPART(WEEKDAY, CAST(@EndDate AS DATE))
		AND	((AV.IsEndsOnDate = 1 
		AND ((CAST(@StartDate AS DATE) BETWEEN AV.StartDate		AND AV.EndsOnUntilDate) 
		OR	(CAST(@EndDate AS DATE) BETWEEN AV.StartDate		AND AV.EndsOnUntilDate)))
		OR	(AV.IsEndsOnAfterNumberOfOccurrance = 1 
		AND ((CAST(@StartDate AS DATE) BETWEEN AV.StartDate		AND AV.EndDateAfterNumberOfOccurrances) 
		OR	(CAST(@EndDate AS DATE) BETWEEN AV.StartDate 		AND AV.EndDateAfterNumberOfOccurrances))))
		AND ((CAST(@StartTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@StartTime AS TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime AS TIME) <= CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
		OR	(CAST(@StartTime AS TIME) >= CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
		OR	(CAST(@StartTime AS TIME) < CAST(AV.StartTime AS TIME)		 AND CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
		OR	(CAST(AV.StartTime AS TIME) = CAST(AV.EndTime AS TIME)))

	SELECT	@Absences = COUNT(*) 
	FROM	[Absence].[AbsenceSchedule] ABSS
	Inner Join Absence.Absence A on A.Absence_Id = ABSS.Absence_Id
	WHERE	A.[User_Id] = @User_Id AND A.[Absence_Id] <> @AbsenceID AND ABSS.AbsenceStatus_Id in (1,2,3)
		AND	((CAST(ABSS.StartDate AS DATE) BETWEEN CAST(@StartDate AS DATE) AND CAST(@EndDate AS DATE)) 
		OR	(CAST(ABSS.EndDate AS DATE) BETWEEN CAST(@StartDate AS DATE)	AND CAST(@EndDate AS DATE))) 
		AND	((CAST(@StartTime AS TIME) > CAST(ABSS.StartDate AS TIME)	AND CAST(@StartTime AS TIME) < CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@EndTime AS TIME) > CAST(ABSS.StartDate AS TIME)		AND CAST(@EndTime AS TIME) < CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime AS TIME) <= CAST(ABSS.StartDate AS TIME)	AND CAST(@EndTime AS TIME) >= CAST(ABSS.EndDate AS TIME))
		OR	(CAST(@StartTime AS TIME) >= CAST(ABSS.StartDate AS TIME)	AND CAST(@EndTime AS TIME) <= CAST(ABSS.EndDate AS TIME)) 
		OR	(CAST(@StartTime AS TIME) < CAST(ABSS.StartDate AS TIME)	AND CAST(@EndTime AS TIME) > CAST(ABSS.EndDate AS TIME)))

	IF(@Unavailability1 > 0 OR @Unavailability2 > 0)
	BEGIN
		SELECT 'unavailable' AS ReturnText
	END

	ELSE
	BEGIN
		IF(@Absences > 0)
		BEGIN
			SELECT 'overlap' AS ReturnText
		END

		ELSE
		BEGIN
			UPDATE [Absence].[Absence]
			SET	[StartDate] = @StartDate,
			[EndDate] = @EndDate, 
			[StartTime] = @StartTime, 
			[EndTime] = @EndTime, 
			[LeaveType_Id] = @LeaveType_Id, 
			[IsSubstituteRequired] = @IsSubstituteRequired,
			[NotesToSubstitute] = @NotesToSubstitute,
			[AbsenceStatus_Id] = @AbsenceStatus_Id,
			[AbsenceType_Id] = @AbsenceType_Id,
			[AnyAttachemt] = @AnyAttachemt,
			[UpdatedBy_User_Id] = @UpdatedBy_User_Id, 
			[Substitute_Id] = @Substitute_Id, 
			[AbsenceDuration_Id] = @AbsenceDuration_Id,
			[UpdatedDate] = GETDATE()
			WHERE [Absence_Id] = @AbsenceID

			SELECT 'success' AS ReturnText

			DELETE FROM [Absence].[AbsenceSchedule]
			WHERE [Absence_Id] = @AbsenceID
		END
	END

END

GO
/****** Object:  StoredProcedure [Absence].[UpdateAbsenceStatus]    Script Date: 8/8/2019 1:04:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Absence].[UpdateAbsenceStatus] 
@AbsenceId	INT,
@statusId	INT,
@UserId		VARCHAR(10)
AS

BEGIN
SET NOCOUNT ON;

	Update	Absence.Absence
	SET		AbsenceStatus_Id	= @statusId,
			AcceptedVia			= null
	WHERE	Absence_Id			= @AbsenceId

	UPDATE	Absence.AbsenceSchedule 
	SET		AbsenceStatus_Id	= @statusId
	WHERE	Absence_Id			= @AbsenceId

	IF(@statusId = 1)
	BEGIN
	UPDATE	Absence.Absence 
	SET		Substitute_Id	= -1
	WHERE	Absence_Id		= @AbsenceId

	UPDATE	Absence.AbsenceSchedule 
	SET		SubstituteId	= -1
	WHERE	Absence_Id		= @AbsenceId
	End

SELECT @@ROWCOUNT
END

--GO
--/****** Object:  StoredProcedure [Report].[GetDetail]    Script Date: 8/8/2019 1:05:33 PM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--ALTER PROCEDURE [Report].[GetDetail] --	@FromDate 			DATETIME = NULL,--	@ToDate 			DATETIME = NULL,--	@ConfirmationNumber VARCHAR(10) = NULL,--	@EmployeeTypeId 	INT = NULL,--	@AbsenceTypeId 		INT = NULL,--	@LocationId 		VARCHAR(10) = NULL,--	@DistrictId			INT = NULL,--	@ReasonId 			INT = NULL,--	@EmployeeName 		VARCHAR(10) = NULL,--	@Month 				INT = NULL,--	@Year 				VARCHAR(10) = NULL,--	@AbsencePosition	INT = NULL--AS--BEGIN		--	IF(@FromDate IS NOT NULL AND @ToDate IS NOT NULL)--	BEGIN--		SELECT @FromDate = CONVERT(char(10), @FromDate, 126), @ToDate = CONVERT(char(10), @ToDate, 126)--	END--	IF(@LocationId = '-1')--	BEGIN--		SET @LocationId = null--	END--	SELECT--			a.Absence_Id as AbsenceId,--			uEmp.FirstName + ' ' + uEmp.LastName  as EmployeeName,--			uEmp.FirstName  as EmployeeFirstName,--			uEmp.LastName as EmployeeLastName,--			usrType.Title AS [EmployeeTypeTitle],--			a.AbsencePosition,--			tl.Title as Grade,--			ts.Title as [Subject],--			lv.[Name] as Reason,--			a.[StartDate],--			a.[EndDate],--			Convert(varchar(20), a.[StartTime]) as StartTime,--			Convert(varchar(20), a.[EndTime]) as EndTime,--			dst.[Name] as [DistrictName],--			a.[CreatedDate] as PostedOn,--			uCrtd.[User_Id] as PostedById,--			uCrtd.FirstName + ' ' + uCrtd.LastName as PostedByName,--			a.AbsenceStatus_Id as StatusId,--			abSt.Title as StatusTitle,--			a.AcceptedDate as StatusDate,--			a.Substitute_Id as SubstituteId,--			USRSubstitute.FirstName + ' ' + USRSubstitute.LastName as SubstituteName,--			USRProfile.ProfilePicUrl As EmployeeProfilePicUrl,--			USRSubstitute.ProfilePicUrl As SubstituteProfilePicUrl,--			a.NotesToSubstitute as Notes,--			atchmnt.FileName as AttachedFileName,--			atchmnt.ContentType as FileContentType,--			atchmnt.[OriginalFileName] as OriginalFileName,--			a.AnyAttachemt as AnyAttachment,--			a.IsSubstituteRequired as SubstituteRequired,--			a.AbsenceDuration_Id as DurationType,--			org.[Name] as [SchoolName],--			a.IsApproved as [IsApproved],--			a.AbsenceType_Id as AbsenceType,--			a.AbsenceResendCounter,--			a.[ConfirmationNumber] as ConfirmationNumber,--			a.[AcceptedVia] AS AcceptedVia		--	FROM [Absence].Absence a--	INNER JOIN [Subzz_Users].[Users].Users uEmp on uEmp.[User_Id] = a.[User_Id]--	INNER JOIN [Subzz_Users].[Users].Users uCrtd on uCrtd.[User_Id] = a.CreatedBy_User_Id--	INNER JOIN [Absence].AbsenceStatus abSt on abSt.AbsenceStatus_Id = a.AbsenceStatus_Id--	INNER JOIN [Leaves].LeaveType lv on lv.LeaveType_Id = a.LeaveType_Id--	INNER JOIN [Subzz_Locations].[Location].Districts dst on dst.District_Id = a.District_Id	--	INNER JOIN [Subzz_Users].[Users].UserType usrType on usrType.UserType_Id = a.AbsencePosition--	Left JOIN [Subzz_Locations].[Location].[Organization] org on org.Organization_Id = a.School_Id--	Left JOIN [Subzz_Users].[Users].Teacher tchr on tchr.[User_Id] = uEmp.[User_Id]--	Left JOIN [Subzz_Lookups].[Lookup].TeachingLevel tl on tl.TeachingLevel_Id = tchr.TeacherLevel_Id--	Left JOIN [Subzz_Lookups].[Lookup].[TeacherSpeciality] ts on ts.[TeacherSpeciality_Id] = tchr.[TeacherSpeciality_Id]--	Left JOIN [Subzz_Users].[Users].Users USRSubstitute on USRSubstitute.[User_Id] = a.Substitute_Id--	Left JOIN [Subzz_Users].[Users].Users USRProfile on USRProfile.[User_Id] = a.[User_Id]--	Left JOIN [Absence].Attachment atchmnt on atchmnt.Absence_Id = a.Absence_Id--	WHERE--	((ISNULL(@FromDate, '') = '' AND ISNULL(@ToDate, '') = '') OR (a.StartDate BETWEEN  @FromDate AND @ToDate) OR (a.EndDate BETWEEN @FromDate AND @ToDate)) --	AND (ISNULL(@DistrictId, 0) = 0 OR a.District_Id = @DistrictId)--	AND (ISNULL(@LocationId, '') = '' OR a.School_Id		= @LocationId) --	AND (ISNULL(@ReasonId, 0) = 0 OR a.LeaveType_Id = @ReasonId)--	AND (ISNULL(@EmployeeName, '') = '' OR LOWER(uEmp.FirstName) LIKE @EmployeeName + '%' OR LOWER(uEmp.LastName) LIKE @EmployeeName + '%')--	AND (ISNULL(@AbsenceTypeId, 0) = 0 OR a.AbsenceDuration_Id = @AbsenceTypeId)--	AND (ISNULL(@ConfirmationNumber, '') = '' OR a.ConfirmationNumber = @ConfirmationNumber)--	AND (ISNULL(@Month, 0) = 0 OR Month(a.StartDate) = @Month)--	AND (ISNULL(@Year, '') = '' OR Year(a.StartDate) = @Year)--	AND (ISNULL(@AbsencePosition, 0) = 0 OR a.AbsencePosition = @AbsencePosition)--	ORDER BY a.[StartDate]--END--GO
--/****** Object:  StoredProcedure [Report].[GetSummary]    Script Date: 8/8/2019 1:05:50 PM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--ALTER PROCEDURE [Report].[GetSummary] 
--	@FromDate 			DATETIME = NULL,
--	@ToDate 			DATETIME = NULL,
--	@ConfirmationNumber	NVARCHAR(10) = NULL,
--	@EmployeeTypeId 	INT = NULL,
--	@AbsenceTypeId 		INT = NULL,
--	@LocationId 		VARCHAR(10) = NULL,
--	@DistrictId			INT = NULL,
--	@ReasonId 			INT = NULL,
--	@EmployeeName 		VARCHAR(10) = NULL,
--	@Month 				INT = NULL,
--	@Year 				VARCHAR(10) = NULL,
--	@AbsencePosition	INT = NULL
--AS
--BEGIN	
	
--	IF(@FromDate IS NOT NULL AND @ToDate IS NOT NULL)
--	BEGIN
--		SELECT @FromDate = CONVERT(char(10), @FromDate, 126), @ToDate = CONVERT(char(10), @ToDate, 126)
--	END
	
--	IF(@LocationId = '-1')
--	BEGIN
--		SET @LocationId = null
--	END

--	SELECT
--		COUNT(CASE WHEN abSt.AbsenceStatus_Id in (1,2) AND a.IsApproved = 1 THEN 1 END) AS TotalCount,
--		COUNT(CASE WHEN abSt.AbsenceStatus_Id in (2,3) AND a.IsSubstituteRequired = 1 AND a.IsApproved = 1 THEN 1 END) AS Filled,
--		COUNT(CASE WHEN abSt.AbsenceStatus_Id = 1 AND a.IsSubstituteRequired = 1 AND a.IsApproved = 1 THEN 1 END) AS Unfilled,
--		COUNT(CASE WHEN a.IsSubstituteRequired = 0 AND abSt.AbsenceStatus_Id = 1 AND a.IsApproved = 1 THEN 1 END) AS NoSubRequired
--	FROM [Absence].Absence a
--	INNER JOIN [Subzz_Users].[Users].Users uEmp on uEmp.[User_Id] = a.[User_Id]
--	INNER JOIN [Subzz_Users].[Users].Users uCrtd on uCrtd.[User_Id] = a.CreatedBy_User_Id
--	INNER JOIN [Absence].AbsenceStatus abSt on abSt.AbsenceStatus_Id = a.AbsenceStatus_Id
--	INNER JOIN [Leaves].LeaveType lv on lv.LeaveType_Id = a.LeaveType_Id
--	INNER JOIN [Subzz_Locations].[Location].Districts dst on dst.District_Id = a.District_Id
--	INNER JOIN [Subzz_Users].[Users].UserType usrType on usrType.UserType_Id = a.AbsencePosition
--	Left JOIN [Subzz_Locations].[Location].[Organization] org on org.Organization_Id = a.School_Id
--	Left JOIN [Subzz_Users].[Users].Teacher tchr on tchr.[User_Id] = uEmp.[User_Id]
--	Left JOIN [Subzz_Lookups].[Lookup].TeachingLevel TL on TL.TeachingLevel_Id = tchr.TeacherLevel_Id
--	Left JOIN [Subzz_Users].[Users].Users USRSubstitute on USRSubstitute.[User_Id] = a.Substitute_Id
--	Left JOIN [Absence].Attachment atchmnt on atchmnt.Absence_Id = a.Absence_Id    
--	WHERE  
--	((ISNULL(@FromDate, '') = '' AND ISNULL(@ToDate, '') = '') OR (a.StartDate BETWEEN  @FromDate AND @ToDate) OR (a.EndDate BETWEEN @FromDate AND @ToDate)) 
--	AND (ISNULL(@DistrictId, 0) = 0 OR a.District_Id = @DistrictId)
--	AND (ISNULL(@LocationId, '') = '' OR a.School_Id		= @LocationId)
--	AND (ISNULL(@ReasonId, 0) = 0 OR a.LeaveType_Id = @ReasonId)
--	AND (ISNULL(@EmployeeName, '') = '' OR LOWER(uEmp.FirstName) LIKE @EmployeeName + '%' OR LOWER(uEmp.LastName) LIKE @EmployeeName + '%')
--	AND (ISNULL(@AbsenceTypeId, 0) = 0 OR a.AbsenceDuration_Id = @AbsenceTypeId)
--	AND (ISNULL(@ConfirmationNumber, '') = '' OR a.ConfirmationNumber = @ConfirmationNumber)
--	AND (ISNULL(@Month, 0) = 0 OR Month(a.StartDate) = @Month)
--	AND (ISNULL(@Year, '') = '' OR Year(a.StartDate) = @Year)
--	AND (ISNULL(@AbsencePosition, 0) = 0 OR a.AbsencePosition = @AbsencePosition)
--END
