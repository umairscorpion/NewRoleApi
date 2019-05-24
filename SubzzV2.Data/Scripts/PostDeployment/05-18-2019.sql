
/******* TABLE ALTER SCRIPT START ********/

ALTER TABLE [Users].[Role] 
ADD SortOrder INT,
IsEditable BIT NOT NULL DEFAULT(1)

/******* TABLE ALTER SCRIPT END ********/


/******* DATA SCRIPT START ********/
UPDATE [Users].[Role] SET IsEditable = 1 WHERE [Name] = 'Super Admin'

INSERT INTO [Users].[Permissions] VALUES (1, 'ADD_SCHOOL', 'Add School', 'To give access to add school', 0)
INSERT INTO [Users].[Permissions] VALUES (1, 'ADD_EDIT_SUBSTITUTE_POSITIONS', 'Add/Edit Substitutes Positions', 'To give access to add/edit Substitutes Positions', 0)
INSERT INTO [Users].[Permissions] VALUES (1, 'ADD_EDIT_LEAVE_TYPE', 'Add/Edit Leave Type', 'To give access to add/edit leave type', 0)
INSERT INTO [Users].[Permissions] VALUES (1, 'ADD_EDIT_ALLOWANCES', 'Add/Edit Allowances', 'To give access to add/edit Allowances', 0)

/******* DATA SCRIPT END ********/


USE [Subzz_Users]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Users].[GetUsersSummary]
@DistrictId INT = NULL
AS
BEGIN

	SELECT	u.User_Id as UserId,
			u.Firstname as FirstName,
			u.LastName as LastName,
			u.Firstname + ' ' + u.LastName as UserName,
			u.Email as Email,
			USROLE.Role_Id as RoleId,
			ROL.Name as RoleName
	from Subzz_Users.Users.Users u 
	INNER JOIN Users.UserRole USROLE ON u.User_Id = UsRole.User_Id
	INNER JOIN Users.[Role] ROL ON ROL.Role_Id =  UsRole.Role_Id
	WHERE (ISNULL(@DistrictId, 0) = 0 OR u.District_Id = @DistrictId)
	AND ROL.Role_Id != 5
	ORDER BY u.Firstname + ' ' + u.LastName


END
GO

-- Exec [Users].[GetRolePermissions] 5
ALTER PROCEDURE [Users].[GetRolePermissions] 
@RoleId INT
AS
BEGIN

	DECLARE @RoleName VARCHAR(300) = NULL
	SELECT @RoleName = r.Name
	FROM [Users].[Role] r
	WHERE r.Role_Id = @RoleId

	IF(@RoleName IS NOT NULL)
	BEGIN
		-- EXISTING ROLE
		SELECT		RolePermissions.Id As Id,
					ISNULL(RolePermissions.RoleId, @RoleId) As RoleId,
					@RoleName as RoleName,
					[Permissions].[Id] As [PermissionId],
					[Permissions].[Title],
					[Permissions].[Description],
					[Permissions].[PermissionCategoryId],
					[Permissions].[DisplayName],
					IsChecked = CASE WHEN ISNULL(RolePermissions.Id, 0) > 0 OR @RoleId = 5 THEN 1 ELSE 0 END
		FROM		[Subzz_Users].[Users].[Permissions] [Permissions] 
		LEFT OUTER JOIN	 [Subzz_Users].[Users].[RolePermissions] RolePermissions
		ON			RolePermissions.[PermissionId] = [Permissions].[Id] AND RolePermissions.[RoleId] = @RoleId
	END
	ELSE
	BEGIN
		-- NEW ROLE
		SELECT		NULL As Id,
					0 As RoleId,
					'' as RoleName,
					[Permissions].[Id] As [PermissionId],
					[Permissions].[Title],
					[Permissions].[Description],
					[Permissions].[PermissionCategoryId],
					[Permissions].[DisplayName],
					IsChecked = 0
		FROM		[Subzz_Users].[Users].[Permissions] [Permissions] 
	END
END
GO

--Exec [Users].[InsertUserRole] 'Superintendent'
ALTER PROCEDURE [Users].[InsertUserRole]
 @RoleName nvarchar(100)
AS
	
	Begin Try

		declare @Inserted table ( Id varchar(10) )
		declare @SortOrder INT = -1
		SELECT @SortOrder = Max(SortOrder) FRom Users.Role 

		Insert into Users.Role (Name, SortOrder, IsEditable) 
		output inserted.Role_Id into @Inserted 
		values(@RoleName, ISNULL(@SortOrder, 0) + 1, 1)

		Select Id as RoleId from @Inserted

	End Try
 BEGIN CATCH
END CATCH
GO

ALTER PROCEDURE [Users].[GetRolesSummary]
@DistrictId INT
AS
BEGIN

	SELECT
		r.Role_Id,
		r.Name as Name,
		COUNT(ur.User_Id) as UsersCount
	FROM [Subzz_Users].[Users].[Role] r
	LEFT OUTER JOIN [Subzz_Users].[Users].UserRole ur ON r.Role_Id = ur.Role_Id
	LEFT OUTER JOIN [Subzz_Users].[Users].[Users] u on u.User_Id = ur.User_Id --AND u.District_Id = @DistrictId
	WHERE ISNULL(r.IsEditable, 0) = 1
	GROUP BY r.Role_Id, r.Name, r.SortOrder
	ORDER BY r.SortOrder, r.Name

END
GO


