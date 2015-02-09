
create table dbo.SobekCM_Item_Alias(
	ItemAliasID int IDENTITY(1,1) NOT NULL,
	Alias varchar(50) NOT NULL,
	ItemID int NOT NULL,
	CONSTRAINT [PK_SobekCM_Item_Alias] PRIMARY KEY CLUSTERED 
(
	ItemAliasID ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].SobekCM_Item_Alias  WITH CHECK ADD  CONSTRAINT [FK_SobekCM_Item_Alias_SobekCM_Item] FOREIGN KEY([ItemID])
REFERENCES [dbo].[SobekCM_Item] ([ItemID])
GO



ALTER PROCEDURE [dbo].[mySobek_Delete_User_Group]
	@usergroupid int,
	@message int output
AS
begin transaction

	if ( exists ( select 1 from mySobek_User_Group_Link where UserGroupID=@usergroupid ))
	begin
		set @message = -1;
	end
	else if ( exists ( select 1 from mySobek_User_Group where UserGroupID=@usergroupid and isSpecialGroup = 'true' ))
	begin
		set @message = -2;
	end
	else
	begin

		delete from mySobek_User_Group_DefaultMetadata_Link where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Edit_Aggregation where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Item_Permissions where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Editable_Link where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Template_Link where UserGroupID=@usergroupid;
		delete from mySobek_User_Group where UserGroupID = @usergroupid;

		set @message = 1;
	end;

commit transaction;
GO
