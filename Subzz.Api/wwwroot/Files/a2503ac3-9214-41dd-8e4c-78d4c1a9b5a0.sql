USE [Subzz_Leaves]
GO
/****** Object:  StoredProcedure [Job].[AcceptJob]    Script Date: 7/4/2019 10:24:44 PM ******/
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

	SELECT @StartTime= StartTime, @EndTime = EndTime, @StartDate= StartDate, @EndDate = EndDate FROM Absence.Absence a 
	WHERE a.Absence_Id =  @AbsenceId

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
					AV.UserId = @UserId AND AV.IsArchived = 0
				AND	((CAST(@StartDate as DATE) between CAST(AV.StartDate as DATE)And CAST(AV.EndDate as DATE)) 
				OR	(CAST(@EndDate AS DATE) between CAST(AV.StartDate as DATE)	 And CAST(AV.EndDate AS DATE))) 
				AND ((CAST(@StartTime as TIME) > CAST(AV.StartTime AS TIME)		 And CAST(@StartTime as TIME) < CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@EndTime AS TIME) > CAST(AV.StartTime AS TIME)		 And CAST(@EndTime AS TIME) < CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@StartTime as TIME) <= CAST(AV.StartTime as TIME)		 And CAST(@EndTime AS TIME) >= CAST(AV.EndTime AS TIME))
				OR	(CAST(@StartTime as TIME) >= CAST(AV.StartTime as TIME)		 And CAST(@EndTime AS TIME) <= CAST(AV.EndTime AS TIME)) 
				OR	(CAST(@StartTime as TIME) < CAST(AV.StartTime as TIME)		 And CAST(@EndTime AS TIME) > CAST(AV.EndTime AS TIME))
				OR	(CAST(AV.StartTime as TIME) = CAST(AV.EndTime AS TIME)))) > 0 )
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
				OR	(CAST(ABSS.EndDate AS DATE) between CAST(@StartDate as DATE) AND CAST(@EndDate AS DATE))) 
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
					SET		[AbsenceStatus_Id] = 2, [AcceptedDate] = GETDATE(), Substitute_Id = @UserId	 
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