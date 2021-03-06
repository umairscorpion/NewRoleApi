USE [Subzz_Leaves]
GO
/****** Object:  StoredProcedure [Absence].[GetAbsencesForCalendar]    Script Date: 8/24/2019 5:47:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--Exec [Absence].[GetAbsencesForCalendar] '5/16/2019 4:21:33 PM', '8/31/2019 12:00:00 AM', 'U000000020'

ALTER PROCEDURE [Absence].[GetAbsencesForCalendar] 
@StartDate		DATE,
@EndDate		DATE,
@UserId			VARCHAR(10)

AS
BEGIN	
	DECLARE @UserRole VARCHAR(10)

	SELECT	@UserRole = (SELECT [UserRole_Id] FROM [Subzz_Users].Users.Users WHERE User_Id = @UserId )

	IF(@UserRole = 4)
	BEGIN
	Select ABSE.[Absence_Id] as AbsenceId, ABSE.[StartDate] as StartDate
		  ,ABSE.[EndDate] as EndDate
		  ,ABSE.[StartTime] as StartTime
		  ,ABSE.[EndTime] as EndTime
		  ,ABSE.[Absence_Id] as AbsenceId
		  ,ABSE.[LeaveType_Id] as AbsenceReasonId
		  ,ABSE.[IsSubstituteRequired] as SubstituteRequired
		  ,ABSE.[NotesToSubstitute] as SubstituteNotes
		  ,ABSE.[PayRollNotes] as PayrollNotes
		  ,ABSE.[AcceptedDate] as AcceptedDate
		  ,ABSE.[ApprovedDate] as ApprovedDate
		  ,ABSE.[District_Id] as DistrictId
		  ,ABSE.[School_Id] as OrganizationId
		  ,ABSE.[AbsenceStatus_Id] as Status
		  ,ABSE.[AbsenceDuration_Id] as DurationType
		  ,ABSE.[AbsencePosition] as PositionId
		  ,ABSE.[IsApproved] as IsApprovalRequired
		  ,ABSE.[AnyAttachemt] as AnyAttachment
		  ,ORG.Name as OrganizationName
		  ,ORG.Address as OrganizationAddress
		  ,ABSE.[User_Id] as EmployeeId
		  ,ABSE.[CreatedBy_User_Id] as AbsenceCreatedByEmployeeId
		  ,ABSE.[Substitute_Id] as SubstituteId
		  ,ABSE.[AbsenceType_Id] as AbsenceType
		  ,ABSE.[CreatedDate] as CreatedDate
		  ,ABSSTATUS.Title as AbsenceStatusDescription
		  ,USREmp.FirstName + ' ' + USREmp.LastName as EmployeeName
		  ,USRCreatedBy.FirstName + ' ' + USRCreatedBy.LastName as CreatedByUser
		  ,USRSubstitute.FirstName + ' ' + USRSubstitute.LastName as SubstituteName
		  ,DIST.Name as AbsenceLocation
		  ,DIST.Address as DistrictAddress
		  ,LEAVE.Name as AbsenceReasonDescription
		  ,USERTYPE.Title As PositionDescription
		  ,ATACH.[FileName] as AttachedFileName
		  ,ATACH.[ContentType] as FileContentType 
		  ,ATACH.[OriginalFileName] as OriginalFileName
		  ,USREmp.ProfilePicUrl As EmployeeProfilePicUrl

		   FROM Absence.Absence ABSE

		   INNER JOIN Subzz_Users.Users.Users as USREmp on USREmp.User_Id = ABSE.User_Id
		   INNER JOIN Subzz_Users.Users.Users as USRCreatedBy on USRCreatedBy.User_Id = ABSE.CreatedBy_User_Id
		   INNER JOIN Absence.AbsenceStatus as ABSSTATUS on ABSSTATUS.AbsenceStatus_Id = ABSE.AbsenceStatus_Id
		   INNER JOIN Leaves.LeaveType as LEAVE on LEAVE.LeaveType_Id = ABSE.LeaveType_Id
		   INNER JOIN Subzz_Locations.Location.Districts as DIST on DIST.District_Id = USREmp.District_Id
		   INNER JOIN Subzz_Users.Users.UserType as USERTYPE on USERTYPE.UserType_Id = ABSE.AbsencePosition
		   Left JOIN Subzz_Users.Users.Users as USRSubstitute on USRSubstitute.User_Id = ABSE.Substitute_Id
		   Left JOIN Subzz_Locations.Location.Organization ORG on ORG.Organization_Id = ABSE.[School_Id]
		   Left JOIN Absence.Attachment as ATACH on ATACH.Absence_Id = ABSE.Absence_Id 

		   WHERE 
				ABSE.AbsenceStatus_Id = 2
			AND	(CAST(ABSE.StartDate AS DATE)	BETWEEN CAST(@StartDate AS DATE) And CAST(@EndDate AS DATE)  
			OR	CAST(ABSE.EndDate AS DATE)		BETWEEN CAST(@StartDate AS DATE) And CAST(@EndDate AS DATE)) 
			AND ABSE.Substitute_Id = @UserId 

		   ORDER BY ABSE.CreatedDate DESC
	END

	ELSE
	BEGIN
	Select ABSE.[Absence_Id] as AbsenceId, ABSE.[StartDate] as StartDate
		  ,ABSE.[EndDate] as EndDate
		  ,ABSE.[StartTime] as StartTime
		  ,ABSE.[EndTime] as EndTime
		  ,ABSE.[Absence_Id] as AbsenceId
		  ,ABSE.[LeaveType_Id] as AbsenceReasonId
		  ,ABSE.[IsSubstituteRequired] as SubstituteRequired
		  ,ABSE.[NotesToSubstitute] as SubstituteNotes
		  ,ABSE.[PayRollNotes] as PayrollNotes
		  ,ABSE.[AcceptedDate] as AcceptedDate
		  ,ABSE.[ApprovedDate] as ApprovedDate
		  ,ABSE.[District_Id] as DistrictId
		  ,ABSE.[School_Id] as OrganizationId
		  ,ABSE.[AbsenceStatus_Id] as Status
		  ,ABSE.[AbsenceDuration_Id] as DurationType
		  ,ABSE.[AbsencePosition] as PositionId
		  ,ABSE.[IsApproved] as IsApprovalRequired
		  ,ABSE.[AnyAttachemt] as AnyAttachment
		  ,ORG.Name as OrganizationName
		  ,ORG.Address as OrganizationAddress
		  ,ABSE.[User_Id] as EmployeeId
		  ,ABSE.[CreatedBy_User_Id] as AbsenceCreatedByEmployeeId
		  ,ABSE.[Substitute_Id] as SubstituteId
		  ,ABSE.[AbsenceType_Id] as AbsenceType
		  ,ABSE.[CreatedDate] as CreatedDate
		  ,ABSSTATUS.Title as AbsenceStatusDescription
		  ,USREmp.FirstName + ' ' + USREmp.LastName as EmployeeName
		  ,USRCreatedBy.FirstName + ' ' + USRCreatedBy.LastName as CreatedByUser
		  ,USRSubstitute.FirstName + ' ' + USRSubstitute.LastName as SubstituteName
		  ,DIST.Name as AbsenceLocation
		  ,DIST.Address as DistrictAddress
		  ,LEAVE.Name as AbsenceReasonDescription
		  ,USERTYPE.Title As PositionDescription
		  ,ATACH.[FileName] as AttachedFileName
		  ,ATACH.[ContentType] as FileContentType 
		  ,ATACH.[OriginalFileName] as OriginalFileName
		  ,USREmp.ProfilePicUrl As EmployeeProfilePicUrl

		   FROM Absence.Absence ABSE

		   INNER JOIN Subzz_Users.Users.Users as USREmp on USREmp.User_Id = ABSE.User_Id
		   INNER JOIN Subzz_Users.Users.Users as USRCreatedBy on USRCreatedBy.User_Id = ABSE.CreatedBy_User_Id
		   INNER JOIN Absence.AbsenceStatus as ABSSTATUS on ABSSTATUS.AbsenceStatus_Id = ABSE.AbsenceStatus_Id
		   INNER JOIN Leaves.LeaveType as LEAVE on LEAVE.LeaveType_Id = ABSE.LeaveType_Id
		   INNER JOIN Subzz_Locations.Location.Districts as DIST on DIST.District_Id = USREmp.District_Id
		   INNER JOIN Subzz_Users.Users.UserType as USERTYPE on USERTYPE.UserType_Id = ABSE.AbsencePosition
		   Left JOIN Subzz_Users.Users.Users as USRSubstitute on USRSubstitute.User_Id = ABSE.Substitute_Id
		   Left JOIN Subzz_Locations.Location.Organization ORG on ORG.Organization_Id = ABSE.[School_Id]
		   Left JOIN Absence.Attachment as ATACH on ATACH.Absence_Id = ABSE.Absence_Id 

		   WHERE 
				ABSE.AbsenceStatus_Id = 2
			AND	(CAST(ABSE.StartDate AS DATE)	BETWEEN CAST(@StartDate AS DATE) And CAST(@EndDate AS DATE)  
			OR	CAST(ABSE.EndDate AS DATE)		BETWEEN CAST(@StartDate AS DATE) And CAST(@EndDate AS DATE)) 
			AND ABSE.[User_Id] = @UserId 
			AND ABSE.AbsenceStatus_Id IN (1,2,3)

		   ORDER BY ABSE.CreatedDate DESC
	END
	
	   
END
