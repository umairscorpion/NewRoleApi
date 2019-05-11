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
@TeacherLevel int,
@Speciality nvarchar(50),
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
@PayRate nvarchar(500) = null,
@HourLimit INT = null,
@Password nvarchar(50) = null
AS
Begin Try
	if(@DistrictId = 0)
	Begin
	Select @DistrictId = District_Id from Subzz_Locations.Location.Organization where Organization_Id = @OrganizationId
	End
	declare @InsertedRecord table ( InsertedId varchar(10) )
	Insert Into Users.Users ([FirstName],[LastName],  [Gender], [IsCertified],[UserRole_Id], [UserType_Id], [Email], [Password], [Phone],
	[IsActive],[IsDeleted],[IsSubscribedSMS],[IsSubscribedEmail],[ProfilePicUrl],[District_Id])
	output inserted.User_Id into @InsertedRecord
	values (@FirstName, @LastName, @Gender, @IsCertified, @RoleId,@UserTypeId, @Email, @Password, @PhoneNumber, 1, @Isdeleted, @IsSubscribedSMS
	,@IsSubscribedEmail, @ProfilePicture,@DistrictId)

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


