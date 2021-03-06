﻿--New Stored Procedure
CREATE PROCEDURE [Users].[sp_updatePassword] 
	@UserId varchar(10),
	@Password varchar(50)
AS
BEGIN
	Update Users.Users
	Set Password = @Password
	where User_Id = @UserId
END
GO
--Delete Data from Resources
TRUNCATE TABLE Users.Resource;
Go

--Insert Resources
USE [Subzz_Users]
GO
SET IDENTITY_INSERT [Users].[Resource] ON 

INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (1, N'Dashboard', N'/home', N'dashboard', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (2, N'Manage', N'manage', N'manage', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (3, N'Absences', N'absence', N'absence', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (4, N'Time Clock ', N'timeclock', N'timeclock', 10, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (5, N'Time Tracker', N'timetracker', N'timetracker', 10, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (6, N'Reports', N'reports', N'reports', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (7, N'Organizations', N'manage/organizations', N'organizations', 3, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (8, N'Jobs', N'viewjobs', N'viewjobs', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (9, N'Districts', N'./districts', N'districts', 3, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (10, N'Schools', N'./schools', N'schools', 3, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (11, N'Schedule Absence', N'absence/createAbsence', N'createAbsence', 3, 2, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (12, N'School Sub List', N'./schoolSubList', N'schoolSubList', 10, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (13, N'Jobs', N'./myJobs', N'myJobs', 10, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (14, N'Class Cover', N'./classCover', N'classCover', 10, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (15, N'Staff', N'manage/employees', N'employees', 3, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (16, N'Substitutes', N'manage/substitutes', N'substitutes', 3, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (17, N'Leave', N'manage/leave', N'leave', 3, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (18, N'Upcoming Absences', N'absence/upcommingAbsence', N'upcommingAbsence', 3, 2, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (19, N'Past Absences', N'absence/pastAbsence', N'pastAbsence', 3, 2, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (20, N'Time Tracker', N'./abortedAbsence', N'abortedAbsence', 3, 2, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (21, N'Daily Report', N'./dailyReports', N'dailyReports', 3, 111, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (22, N'Monthly Report', N'./monthlyReports', N'monthlyReports', 3, 111, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (23, N'Payroll Report', N'./payRollReports', N'payRollReports', 3, 111, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (24, N'Class Cover Report', N'./classCoverReports', N'classCoverReports', 3, 5, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (25, N'Available Jobs', N'./availableJobs', N'availableJobs', 3, 8, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (26, N'My Jobs', N'./myJobs', N'myJobs', 3, 8, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (27, N'Past Jobs', N'./pastJobs', N'pastJobs', 3, 8, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (28, N'Profile', N'profile', N'profile', 10, 1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (29, N'Permissions', N'permissions', N'permissions', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (30, N'Roles', N'./roles', N'roles', 2, -1, NULL, NULL, 1)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (32, N'Dashboard', N'/home', N'home', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (33, N'Payroll', N'/payroll', N'payroll', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (34, N'Training Guides', N'/trainingGuide', N'trainingGuide', 2, -1, NULL, NULL, 0)
INSERT [Users].[Resource] ([Resource_Id], [Name], [StateMachine], [Css], [ResourceType_Id], [ParentResource_Id], [ShowInTopNav], [ShowInSideNav], [IsAdminPortal]) VALUES (35, N'Training Guides', N'/trainingGuide', N'trainingGuide', 2, -1, NULL, NULL, 0)
SET IDENTITY_INSERT [Users].[Resource] OFF
Go
--To Check Email Existance in case of Forgot Password
Create PROCEDURE [users].[sp_checkEmailExistance] 
	@EmailId varchar(50)
AS
BEGIN
	IF(EXISTS(
				SELECT	* 
				FROM	[Subzz_Users].[Users].[Users] u
				WHERE	u.Email = @EmailId
				))
				BEGIN
					SELECT IsExists = 1
				END
				ELSE
				BEGIN
					SELECT IsExists = 0
				END 
END
GO

--Add New Columns in Users.User Table
Alter Table Users.Users
ADD ResetPasswordKey nvarchar(100)
Alter Table Users.Users
ADD LinkValidUpto DateTime 
Go

--Drop table Absence Type
Drop Table [Subzz_Leaves].[Absence].[AbsenceType]

--Create Table Absence Type
USE [Subzz_Leaves]
GO

/****** Object:  Table [Absence].[AbsenceType]    Script Date: 5/24/2019 6:15:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Absence].[AbsenceType](
	[AbsenceType_Id] [int] NOT NULL,
	[Title] [nvarchar](50) NULL
) ON [PRIMARY]

GO

USE [Subzz_Leaves]
GO
INSERT [Absence].[AbsenceType] ([AbsenceType_Id], [Title]) VALUES (5, N'No Substitute')
INSERT [Absence].[AbsenceType] ([AbsenceType_Id], [Title]) VALUES (4, N'All Subs')
INSERT [Absence].[AbsenceType] ([AbsenceType_Id], [Title]) VALUES (1, N'My Favorites')
INSERT [Absence].[AbsenceType] ([AbsenceType_Id], [Title]) VALUES (3, N'Preferred Sub')
INSERT [Absence].[AbsenceType] ([AbsenceType_Id], [Title]) VALUES (2, N'Direct Assign')
Go

Create PROCEDURE [Users].[sp_updatePasswordResetKey] 
	@EmailId nvarchar(50),
	@ResetPassKey nvarchar(200),
	@validUpTo Datetime
AS
BEGIN
	Update Users.Users 
	Set ResetPasswordKey = @ResetPassKey,
		LinkValidUpto = @validUpTo
	Where @EmailId = @EmailId
END
Go

Create PROCEDURE [Users].[sp_updatePasswordByActivationLink] 
	@ResetPassKey nvarchar(100),
	@Email nvarchar(100),
	@Password nvarchar(100)
AS
BEGIN
	Update Users.Users 
	Set Password = @Password
	Where Email = @Email And ResetPasswordKey = @ResetPassKey
END

Go
ALTER TABLE [Subzz_Users].[Users].[Users]
ADD IsViewedNewVersion BIT DEFAULT(0) NOT NULL

----
USE [Subzz_Settings]
GO
CREATE TABLE [Settings].[Versions](
	[Id] [int] NOT NULL,
	[Version] [varchar](20) NULL,
	[Features] [varchar](500) NULL,
	[ContentDetails] [varchar](1500) NULL,
 CONSTRAINT [PK_Versions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
