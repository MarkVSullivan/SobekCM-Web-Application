
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE dbo.mySobek_Get_All_User_Groups AS
BEGIN

	with linked_users_cte ( UserGroupID, UserCount ) AS
	(
		select UserGroupID, count(*)
		from mySobek_User_Group_Link
		group by UserGroupID
	)
	select G.UserGroupID, GroupName, GroupDescription, coalesce(UserCount,0) as UserCount, IsSpecialGroup
	from mySobek_User_Group G 
	     left outer join linked_users_cte U on U.UserGroupID=G.UserGroupID
	order by IsSpecialGroup, GroupName;

END
GO

GRANT EXECUTE ON dbo.mySobek_Get_All_User_Groups to sobek_user;
GO

drop table mySobek_User_Item_Permissions;
go

USE [AurariaDev]
GO

/****** Object:  Table [dbo].[mySobek_User_Item_Permissions]    Script Date: 9/15/2014 6:48:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[mySobek_User_Item_Permissions](
	[UserPermissionID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[isOwner] [bit] NOT NULL,
	[canView] [bit] NULL,
	[canEditMetadata] [bit] NULL,
	[canEditBehaviors] [bit] NULL,
	[canPerformQc] [bit] NULL,
	[canUploadFiles] [bit] NULL,
	[canChangeVisibility] [bit] NULL,
	[canDelete] [bit] NULL,
	[customPermissions] [varchar](max) NULL,
 CONSTRAINT [PK_mySobek_User_Item_Permissions] PRIMARY KEY CLUSTERED 
(
	[UserPermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


ALTER TABLE [mySobek_User_Item_Permissions]
ADD CONSTRAINT fk_mySobek_User_Item_Permissions_UserID
FOREIGN KEY (UserID)
REFERENCES mySobek_User(UserID);
GO

ALTER TABLE [mySobek_User_Item_Permissions]
ADD CONSTRAINT fk_mySobek_User_Item_Permissions_ItemID
FOREIGN KEY (ItemID)
REFERENCES SobekCM_Item(ItemID);
GO


CREATE TABLE [dbo].[mySobek_User_Group_Item_Permissions](
	[UserGroupPermissionID] [int] IDENTITY(1,1) NOT NULL,
	[UserGroupID] [int] NOT NULL,
	[ItemID] [int] NULL,
	[isOwner] [bit] NOT NULL,
	[canView] [bit] NULL,
	[canEditMetadata] [bit] NULL,
	[canEditBehaviors] [bit] NULL,
	[canPerformQc] [bit] NULL,
	[canUploadFiles] [bit] NULL,
	[canChangeVisibility] [bit] NULL,
	[canDelete] [bit] NULL,
	[customPermissions] [varchar](max) NULL,
	[isDefaultPermissions] [bit] NOT NULL,
 CONSTRAINT [PK_mySobek_User_Group_Item_Permissions] PRIMARY KEY CLUSTERED 
(
	[UserGroupPermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[mySobek_User_Group_Item_Permissions] ADD  DEFAULT ('false') FOR [isDefaultPermissions]
GO


ALTER TABLE [mySobek_User_Group_Item_Permissions]
ADD CONSTRAINT fk_mySobek_User_Group_Item_Permissions_UserGroupID
FOREIGN KEY (UserGroupID)
REFERENCES mySobek_User_Group(UserGroupID);
GO

ALTER TABLE [mySobek_User_Group_Item_Permissions]
ADD CONSTRAINT fk_mySobek_User_Group_Item_Permissions_ItemID
FOREIGN KEY (ItemID)
REFERENCES SobekCM_Item(ItemID);
GO


alter table mySobek_User_Group add IsSpecialGroup bit not null default('false');
GO


SET IDENTITY_INSERT mySobek_User_Group ON;
GO

insert into mySobek_User_Group ( UserGroupID, GroupName, GroupDescription, Can_Submit_Items, Internal_User, IsSystemAdmin, IsPortalAdmin, Include_Tracking_Standard_Forms, autoAssignUsers, Can_Delete_All_Items, IsSobekDefault, IsShibbolethDefault, IsLdapDefault, IsSpecialGroup )
values ( -1, 'Everyone', 'Default everyone group within the SobekCM system', 'false', 'false', 'false', 'false', 'false', 'true', 'false', 'false', 'false', 'false', 'true' );
GO

SET IDENTITY_INSERT mySobek_User_Group OFF;
GO

ALTER PROCEDURE [dbo].[mySobek_Delete_User_Group]
	@usergroupid int
AS
begin
	delete from mySobek_User_Group
	where UserGroupID = @usergroupid
	  and isSpecialGroup = 'false';
end
GO

ALTER PROCEDURE [dbo].[mySobek_Get_All_Users] AS
BEGIN
	
	select UserID, LastName + ', ' + FirstName AS [Full_Name], UserName, EmailAddress
	from mySobek_User 
	order by Full_Name;
END;
GO

