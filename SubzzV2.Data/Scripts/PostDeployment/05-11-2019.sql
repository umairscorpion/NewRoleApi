USE [Subzz_Users]
GO

/****** Object:  StoredProcedure [Users].[VerifyUser]    Script Date: 5/11/2019 9:41:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Exec [Users].[VerifyUser] 'U000000037', 'johny@lever.com'

CREATE PROCEDURE [Users].[VerifyUser]
@UserId nvarchar(50),
@Email nvarchar(500)
AS
BEGIN

	IF(EXISTS(
				SELECT	* 
				FROM	[Subzz_Users].[Users].[Users] u
				WHERE	(ISNULL(@UserId, '') = '' OR u.[User_Id] <> @UserId)
				AND		u.Email = @Email 
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

/****** Object:  StoredProcedure [Users].[InsertUser]    Script Date: 5/11/2019 9:41:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Exec [Users].[InsertUser]
ALTER PROCEDURE [Users].[InsertUser]
@FirstName nvarchar(50),
@LastName nvarchar(50),
@UserTypeId nvarchar(50),
@RoleId nvarchar(50),
@Gender nvarchar(10),
@IsCertified BIT,
@DistrictId int,
@OrganizationId varchar(5),
@Email nvarchar(500),
@PhoneNumber nvarchar(500), 
@IsSubscribedSMS BIT,
@IsSubscribedEmail BIT,
@Isdeleted BIT,
@ProfilePicture nvarchar(max),
@Speciality nvarchar(50),
@TeacherLevel int,
@PayRate nvarchar(10) = "10",
@HourLimit int = 0,
@Password nvarchar(50) = null

AS
Begin Try
	if(@DistrictId = 0)
	Begin
	Select @DistrictId = District_Id from Subzz_Locations.Location.Organization where Organization_Id = @OrganizationId
	End
	declare @InsertedRecord table ( InsertedId varchar(10) )
	Insert Into Users.Users ([FirstName],[LastName],  [Gender], [IsCertified],[UserRole_Id], [UserType_Id], [Email], [Password], [Phone],
	[IsActive],[IsDeleted],[IsSubscribedSMS],[IsSubscribedEmail],[ProfilePicUrl],[District_Id], [PayRate])
	output inserted.User_Id into @InsertedRecord
	values (@FirstName, @LastName, @Gender, @IsCertified, @RoleId,@UserTypeId, @Email, @Password, @PhoneNumber, 1, @Isdeleted, @IsSubscribedSMS
	,@IsSubscribedEmail, @ProfilePicture,@DistrictId, @PayRate)

	--ADD USER ROLE
	Insert into Users.UserRole (User_Id,Role_Id) select InsertedId, @RoleId from @InsertedRecord
	--ADD USER DISTRICT
	Insert into Users.UserLocation ([User_Id], [Location_Id], [UserLevel_Id]) 
	select InsertedId, @DistrictId, case when LEN(@OrganizationId) = 5 then 3 else 1 end from @InsertedRecord
	--ADD USER ORGANIZATION IF ITS ORGGANIZATION ID EXISTS
	If(LEN(@OrganizationId) = 5)
	Begin
	Insert into Users.UserLocation ([User_Id], [Location_Id], [UserLevel_Id]) 
	select InsertedId, @OrganizationId, case when LEN(@OrganizationId) = 5 then 3 else 1 end from @InsertedRecord
	End
	
	
	--If User is Substitute i.e UserType = 4
	If (@RoleId = 4)
	Begin
	Insert Into Users.SubstituteCategory ([UserType_Id], [User_Id], [IsNotify], [ModifyDate]) 	
		Select users.UserType.UserType_Id, ( Select InsertedId from @InsertedRecord ), 1, GetDate()  from users.UserType
	End
	--If User  Teacher
	If (@UserTypeId = 1)
	Begin
	Insert Into Users.Teacher ([Speciality],[TeacherLevel_Id], [User_Id]) 	
		select @Speciality, @TeacherLevel, InsertedId from @InsertedRecord
	End
 End Try
 BEGIN CATCH
END CATCH
GO

SET IDENTITY_INSERT [Subzz_Users].[Users].[UserType] ON 
  Insert Into [Subzz_Users].[Users].[UserType] ([UserType_Id]
      ,[Title]) values (1, 'Teacher')
  SET IDENTITY_INSERT [Subzz_Users].[Users].[UserType] OFF
 GO

 USE [Subzz_Users]
GO
/****** Object:  StoredProcedure [Users].[uspGetUserDetail]    Script Date: 5/14/2019 5:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,Taimoor Ali>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
-- Exec [Users].[uspGetUserDetail] 'U000000094'
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
	   TEA.[Speciality] as Speciality, TEA.[TeacherLevel_Id] as TeachingLevel, u.PayRate as PayRate
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
	   TEA.[Speciality] as Speciality, TEA.[TeacherLevel_Id] as TeachingLevel, u.PayRate as PayRate
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
	SELECT u.User_Id as UserId,u.Firstname as FirstName ,u.LastName as LastName , u.Firstname + ' ' + u.LastName as UserName,
	 u.UserType_Id as UserTypeId, USRTYPE.Title as UserTypeDescription, USROLE.Role_Id as RoleId , ROL.Name as UserRoleDesciption,
	  u.Email as Email, u.Phone as PhoneNumber, u.ProfilePicUrl as ProfilePicture, u.[Gender] as Gender, u.IsActive as IsActive,
	  u.[IsCertified] as IsCertified,u.[IsSubscribedEmail] as IsSubscribedEmail, u.[IsSubscribedSMS] as IsSubscribedSMS, u.District_Id as DistrictId, ORG.Organization_Id as OrganizationId, @UserLevel as UserLevel,
	  TEA.[Speciality] as Speciality, TEA.[TeacherLevel_Id] as TeachingLevel, u.PayRate as PayRate
	from Subzz_Users.Users.Users u 
	INNER JOIN Users.UserRole USROLE ON u.User_Id = UsRole.User_Id
	INNER JOIN Users.[Role] ROL ON ROL.Role_Id =  UsRole.Role_Id
	INNER JOIN Users.UserType USRTYPE ON u.UserType_Id = USRTYPE.UserType_Id
	INNER JOIN Users.UserLocation UL ON u.User_Id = UL.User_Id
	LEFT JOIN Users.Teacher TEA ON TEA.User_Id = u.User_Id
	left JOIN Subzz_Locations.Location.Organization ORG ON ORG.Organization_Id = UL.Location_Id
	where u.User_Id= @UserId And ORG.Organization_Id is not null
	End
	else
	Begin
	--IF SUPER ADMIN
	SELECT u.User_Id as UserId,u.Firstname as FirstName ,u.LastName as LastName , u.Firstname + ' ' + u.LastName as UserName,
	 u.UserType_Id as UserTypeId, USRTYPE.Title as UserTypeDescription, USROLE.Role_Id as RoleId , ROL.Name as UserRoleDesciption,
	  u.Email as Email, u.Phone as PhoneNumber, u.ProfilePicUrl as ProfilePicture, u.[Gender] as Gender, u.IsActive as IsActive,
	  u.[IsCertified] as IsCertified,u.[IsSubscribedEmail] as IsSubscribedEmail, u.[IsSubscribedSMS] as IsSubscribedSMS, u.District_Id as DistrictId, '-1' as OrganizationId, @UserLevel as UserLevel 
	from Subzz_Users.Users.Users u 
	INNER JOIN Users.UserRole USROLE ON u.User_Id = UsRole.User_Id
	INNER JOIN Users.[Role] ROL ON ROL.Role_Id =  UsRole.Role_Id
	INNER JOIN Users.UserType USRTYPE ON u.UserType_Id = USRTYPE.UserType_Id
	where u.User_Id= @UserId
	End
	
END

Exec sp_rename 'Users.Teacher.Speciality', 'TeacherSpeciality_Id', 'COLUMN';
Go
USE [Subzz_Users]
ALTER TABLE Users.Teacher
ALTER COLUMN TeacherSpeciality_Id int;
Go

Update Users.Teacher
Set TeacherSpeciality_Id = 1
GO

ALTER TABLE Users.UserLocation
ADD IsPrimary Bit;
Go

Create PROCEDURE [Users].[sp_insertSecondarySchools]
	@UserId varchar(10),
	@LocationId nvarchar(10),
	@UserLevel int,
	@IsPrimary Bit

AS
Begin Try
	INSERT INTO Users.UserLocation (User_Id, Location_Id, UserLevel_Id, IsPrimary)
		Values(@UserId, @LocationId, @UserLevel, @IsPrimary)
 End Try
 BEGIN CATCH
END CATCH
Go

Create PROCEDURE [Users].[sp_getUserSecondarySchools] 
	@UserId varchar(10)
AS
BEGIN
	Select * from users.UserLocation UL
	Inner Join Subzz_Locations.Location.Organization ORG On ORG.Organization_Id = UL.Location_Id
	where UL.User_Id = @UserId And ISNULL(Ul.Isprimary, 0) = 0
END
GO

ALTER PROCEDURE [Users].[GetUsers]
@userId varchar(10),
@userRole int, --To show which type of users you want to get if it is 4 than substitutes.
@districtId int,
@organizationId varchar(5)

AS
Begin Try
Declare @loginedUserRole varchar(10)
Select @loginedUserRole = (Select UR.Role_Id from Users.Users USR Inner Join Users.UserRole UR on USR.User_Id = UR.User_Id where UR.User_Id = @userId)
If (@loginedUserRole = 5) -- Super Admin
Begin
	If (@userRole = 4)
	Begin
	Select USR.User_Id as UserId,  USR.[FirstName], USR.[LastName], USR.[Gender], USR.[IsCertified], POS.[Title] as UserTypeDescription,
	 USR.[UserRole_Id] as RoleId, USR.[Email], USR.[Password], USR.[Phone] as PhoneNumber, DIS.Name as DistrictName, UR.Name as RoleName,
	USR.[IsActive], USR.[IsDeleted], USR.[IsSubscribedSMS], USR.[IsSubscribedEmail], USR.[ProfilePicUrl] as ProfilePicture,
	 USR.[UserType_Id] as UserTypeId, UTE.TeacherSpeciality_Id  as SpecialityTypeId,
	USR.[District_Id] as DistrictId, USR.PayRate as PayRate
	 from Users.Users USR
	LEFT JOIN Users.Teacher UTE on UTE.User_Id = USR.User_Id
	INNER JOIN Users.Position POS on POS.Position_Id = USR.UserType_Id
	INNER JOIN Users.UserRole UUR on UUR.User_Id = USR.User_Id
	INNER JOIN Users.Role UR on UR.Role_Id = UUR.Role_Id
	INNER JOIN Users.UserLocation UL on UL.User_Id = USR.User_Id
	INNER JOIN Subzz_Locations.Location.Districts DIS on DIS.District_Id = UL.Location_Id
 where  uur.Role_Id = @userRole 
 End
 else
	Begin
	Select USR.User_Id as UserId,  USR.[FirstName], USR.[LastName], USR.[Gender], USR.[IsCertified], 
	USR.[UserRole_Id] as RoleId, USR.[Email], USR.[Password], USR.[Phone] as PhoneNumber, ORG.Name as OrganizationName, UR.Name as RoleName,
	USR.[IsActive], USR.[IsDeleted], USR.[IsSubscribedSMS], USR.[IsSubscribedEmail], USR.[ProfilePicUrl] as ProfilePicture, USR.[UserType_Id] as UserTypeId,
	 USR.[District_Id] as DistrictId, UTE.TeacherSpeciality_Id  as SpecialityTypeId
	from Users.Users USR
	LEFT JOIN Users.Teacher UTE on UTE.User_Id = USR.User_Id
	INNER JOIN Users.UserRole UUR on UUR.User_Id = USR.User_Id
	INNER JOIN Users.Role UR on UR.Role_Id = UUR.Role_Id
	INNER JOIN Subzz_Locations.Location.Districts DIS on DIS.District_Id = USR.District_Id
	INNER JOIN Users.UserLocation UL on UL.User_Id = USR.User_Id
	Left JOIN Subzz_Locations.Location.Organization ORG on ORG.Organization_Id = UL.Location_Id
 where  uur.Role_Id NOT IN (4)
 End
 End
 else
 Begin
	If (@userRole = 4)
	Begin
	Select USR.User_Id as UserId,  USR.[FirstName], USR.[LastName], USR.[Gender], USR.[IsCertified], POS.[Title] as UserTypeDescription,
	 USR.[UserRole_Id] as RoleId, USR.[Email], USR.[Password], USR.[Phone] as PhoneNumber, DIS.Name as DistrictName, UR.Name as RoleName,
	USR.[IsActive], USR.[IsDeleted], USR.[IsSubscribedSMS], USR.[IsSubscribedEmail], USR.[ProfilePicUrl] as ProfilePicture,
	 USR.[UserType_Id] as UserTypeId, USR.[District_Id] as DistrictId, UTE.TeacherSpeciality_Id  as SpecialityTypeId, USR.PayRate as PayRate
	from Users.Users USR
	LEFT JOIN Users.Teacher UTE on UTE.User_Id = USR.User_Id
	INNER JOIN Users.Position POS on POS.Position_Id = USR.UserType_Id
	INNER JOIN Users.UserRole UUR on UUR.User_Id = USR.User_Id
	INNER JOIN Users.Role UR on UR.Role_Id = UUR.Role_Id
	INNER JOIN Users.UserLocation UL on UL.User_Id = USR.User_Id
	INNER JOIN Subzz_Locations.Location.Districts DIS on DIS.District_Id = UL.Location_Id
 where  uur.Role_Id = 4 And USR.District_Id = 19
 End
 else
	Begin
	IF(LEN(@organizationId) = 5)
	Begin
	Select USR.User_Id as UserId,  USR.[FirstName], USR.[LastName], USR.[Gender], USR.[IsCertified],
	 USR.[UserRole_Id] as RoleId, USR.[Email], USR.[Password], USR.[Phone] as PhoneNumber, ORG.Name as OrganizationName, UR.Name as RoleName,
	USR.[IsActive], USR.[IsDeleted], USR.[IsSubscribedSMS], USR.[IsSubscribedEmail], USR.[ProfilePicUrl] as ProfilePicture, USR.[UserType_Id] as UserTypeId,
	 USR.[District_Id] as DistrictId, UTE.TeacherSpeciality_Id  as SpecialityTypeId
	 from Users.Users USR
	 LEFT JOIN Users.Teacher UTE on UTE.User_Id = USR.User_Id
	INNER JOIN Users.UserRole UUR on UUR.User_Id = USR.User_Id
	INNER JOIN Users.Role UR on UR.Role_Id = UUR.Role_Id
	INNER JOIN Users.UserLocation ULOC on ULOC.User_Id = USR.User_Id
	INNER JOIN Subzz_Locations.Location.Organization ORG on ORG.Organization_Id = ULOC.Location_Id
	where  uur.Role_Id IN (2,3) And ORG.Organization_Id = @organizationId And ULOC.Isprimary = 1 And ULOC.UserLevel_Id = 3
	End
	Else
	Begin
	Select USR.User_Id as UserId,  USR.[FirstName], USR.[LastName], USR.[Gender], USR.[IsCertified],
	 USR.[UserRole_Id] as RoleId, USR.[Email], USR.[Password], USR.[Phone] as PhoneNumber, ORG.Name as OrganizationName, '' as DistrictName, UR.Name as RoleName,
	USR.[IsActive], USR.[IsDeleted], USR.[IsSubscribedSMS], USR.[IsSubscribedEmail], USR.[ProfilePicUrl] as ProfilePicture, USR.[UserType_Id] as UserTypeId,
	 USR.[District_Id] as DistrictId, UTE.TeacherSpeciality_Id  as SpecialityTypeId
	 from Users.Users USR
	 LEFT JOIN Users.Teacher UTE on UTE.User_Id = USR.User_Id
	INNER JOIN Users.UserRole UUR on UUR.User_Id = USR.User_Id
	INNER JOIN Users.Role UR on UR.Role_Id = UUR.Role_Id
	INNER JOIN Users.UserLocation ULOC on ULOC.User_Id = USR.User_Id
	INNER JOIN Subzz_Locations.Location.Organization ORG on ORG.Organization_Id = ULOC.Location_Id
	where  uur.Role_Id IN (2,3) And USR.District_Id = @districtId And ULOC.UserLevel_Id = 3 And ULOC.Isprimary = 1
	UNION ALL
	Select USR.User_Id as UserId,  USR.[FirstName], USR.[LastName], USR.[Gender], USR.[IsCertified],
	 USR.[UserRole_Id] as RoleId, USR.[Email], USR.[Password], USR.[Phone] as PhoneNumber, '' as OrganizationName, DIS.Name as DistrictName, UR.Name as RoleName,
	USR.[IsActive], USR.[IsDeleted], USR.[IsSubscribedSMS], USR.[IsSubscribedEmail], USR.[ProfilePicUrl] as ProfilePicture, USR.[UserType_Id] as UserTypeId,
	 USR.[District_Id] as DistrictId, UTE.TeacherSpeciality_Id  as SpecialityTypeId
	 from Users.Users USR
	 LEFT JOIN Users.Teacher UTE on UTE.User_Id = USR.User_Id
	INNER JOIN Users.UserRole UUR on UUR.User_Id = USR.User_Id
	INNER JOIN Users.Role UR on UR.Role_Id = UUR.Role_Id
	INNER JOIN Users.UserLocation ULOC on ULOC.User_Id = USR.User_Id
	INNER JOIN Subzz_Locations.Location.Districts DIS on Cast(DIS.District_Id as varchar(10)) = ULOC.Location_Id
	where  uur.Role_Id IN (1,3) And DIS.District_Id = @districtId And ULOC.UserLevel_Id = 1
	End
 End
 End
 End Try
 BEGIN CATCH
END CATCH
Go

Create PROCEDURE [Users].[sp_getUserSecondarySchools] 
	@UserId varchar(10)
AS
BEGIN
	Select ORG.Organization_Id as SchoolId
	from users.UserLocation UL
	Inner Join Subzz_Locations.Location.Organization ORG On ORG.Organization_Id = UL.Location_Id
	where UL.User_Id = @UserId 
END
GO





