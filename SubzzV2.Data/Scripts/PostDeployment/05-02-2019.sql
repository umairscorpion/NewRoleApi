/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
USE [Subzz_Users]
IF EXISTS(SELECT 1 FROM sys.procedures WHERE object_id = OBJECT_ID(N'Users.sp_getSchoolSubList'))
  DROP TABLE [Users].[sp_getSchoolSubList]
Go
Create PROCEDURE [Users].[sp_getSchoolSubList] 
@UserId varchar(10),
@DistrictId int
AS
BEGIN
	Select SUBLIST.SchoolSubstituteList_Id as Id, USR.FirstName as SubstituteName, USR.User_Id as SubstituteId, ISNULL(SUBLIST.IsAdded, 0) as IsAdded
	from Users.Users USR
	Left JOIN Users.SchoolSubstituteList SUBLIST ON SUBLIST.District_Id = USR.District_Id
	INNER JOIN Users.UserRole UUR on UUR.User_Id = USR.User_Id
	where  uur.Role_Id = 4 And USR.District_Id = @districtId
END
Go