USE [Subzz_Users]
GO
/****** Object:  StoredProcedure [Users].[GetTimeClockData]    Script Date: 7/2/2019 3:11:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [Users].[GetTimeClockData]
	@UserId NVARCHAR(10),
	@StartDate date,
	@EndDate date,
	@IsDaysSelected INT
AS
BEGIN
IF(@IsDaysSelected = 1)
BEGIN
	SELECT
       TC.[ActivityTime]
      ,TC.[Activity] AS Activity
      ,TC.[UpdatedOn] AS ClockInDate
      ,TC.[BreakTime]
      ,TC.[ReturnFromBreakTime]
      ,TC.[ParentId]
      ,TC.[UserId]
	  --,TC.[ClockInTime] AS ClockInTime
	  ,cast([ClockInTime] as time) AS ClockInTime
	  ,cast([ClockOutTime] as time) AS ClockOutTime
      --,TC.[ClockOutTime] AS ClockOutTime
	  ,TC.[TotalBreakTime] AS TotalBreakTime
	  ,TC.[Status] AS Status
	  ,U.FirstName AS FirstName
	  ,U.LastName AS LastName
	  ,DATEDIFF(MINUTE,[ClockInTime] , [ClockOutTime]) as  TotalMinutes
	  ,DATEDIFF(HOUR, [ClockInTime], [ClockOutTime]) as TotalHours
	  ,Cast(TC.[TotalBreakTime] / 60 as Varchar) + ' Hrs ' +Cast(TC.[TotalBreakTime] % 60 as Varchar) + ' Min' AS HoursAndMinutes
  FROM [Subzz_Users].[Users].[TimeClock1122] TC
  INNER JOIN [Users].[Users] U ON TC.UserId = U.User_Id
	  WHERE [UserId] = @UserId 
	  AND (TC.[Activity] = 'Clock In' OR TC.[Activity] = 'Clock Out')
	  AND TC.[UpdatedOn] BETWEEN  @StartDate AND @EndDate
	  ORDER BY TC.[UpdatedOn] DESC, TC.[ActivityTime] DESC
END
ELSE
BEGIN
	SELECT
       TC.[ActivityTime]
      ,TC.[Activity] AS Activity
      ,TC.[UpdatedOn] AS ClockInDate
      ,TC.[BreakTime]
      ,TC.[ReturnFromBreakTime]
      ,TC.[ParentId]
      ,TC.[UserId]
	  --,TC.[ClockInTime] AS ClockInTime
   --   ,TC.[ClockOutTime] AS ClockOutTime
	  ,cast([ClockInTime] as time) AS ClockInTime
	  ,cast([ClockOutTime] as time) AS ClockOutTime
	  ,TC.[TotalBreakTime] AS TotalBreakTime
	  ,TC.[Status] AS Status
	  ,U.FirstName AS FirstName
	  ,U.LastName AS LastName
	  ,DATEDIFF(MINUTE, [ClockInTime], [ClockOutTime]) as  TotalMinutes
	  ,DATEDIFF(HOUR, [ClockInTime], [ClockOutTime]) as TotalHours
	  ,Cast(TC.[TotalBreakTime] / 60 as Varchar) + ' Hrs ' +Cast(TC.[TotalBreakTime] % 60 as Varchar) + ' Min' AS HoursAndMinutes
  FROM [Subzz_Users].[Users].[TimeClock1122] TC
  INNER JOIN [Users].[Users] U ON TC.UserId = U.User_Id
	  WHERE [UserId] = @UserId AND (TC.[Activity] = 'Clock Out' OR TC.[Activity] = 'Clock In')
	  ORDER BY TC.[UpdatedOn] DESC, TC.[ActivityTime] DESC
END
END