

alter table mySobek_User add Can_Delete_All_Items bit not null default('false');
GO
alter table mySobek_User_Group add Can_Delete_All_Items bit not null default('false');
GO
alter table mySobek_User_Group_Edit_Aggregation add IsAdmin bit not null default('false');
GO
alter table mySobek_User_Edit_Aggregation add IsAdmin bit not null default('false');
GO


ALTER PROCEDURE [dbo].[mySobek_Get_User_By_UserID]
	@userid int
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the basic user information
	select UserID, UFID=isnull(UFID,''), UserName=isnull(UserName,''), EmailAddress=isnull(EmailAddress,''), 
	  FirstName=isnull(FirstName,''), LastName=isnull(LastName,''), Note_Length, 
	  Can_Make_Folders_Public, isTemporary_Password, sendEmailOnSubmission, Can_Submit_Items, 
	  NickName=isnull(NickName,''), Organization=isnull(Organization, ''), College=isnull(College,''),
	  Department=isnull(Department,''), Unit=isnull(Unit,''), Rights=isnull(Default_Rights,''), Language=isnull(UI_Language, ''), 
	  Internal_User, OrganizationCode, EditTemplate, EditTemplateMarc, IsSystemAdmin, IsPortalAdmin, Include_Tracking_Standard_Forms,
	  Descriptions=( select COUNT(*) from mySobek_User_Description_Tags T where T.UserID=U.UserID),
	  Receive_Stats_Emails, Has_Item_Stats, Can_Delete_All_Items	  
	from mySobek_User U
	where ( UserID = @userid ) and ( isActive = 'true' );

	-- Get the templates
	select T.TemplateCode, T.TemplateName, GroupDefined='false', DefaultTemplate
	from mySobek_Template T, mySobek_User_Template_Link L
	where ( L.UserID = @userid ) and ( L.TemplateID = T.TemplateID )
	union
	select T.TemplateCode, T.TemplateName, GroupDefined='true', 'false'
	from mySobek_Template T, mySobek_User_Group_Template_Link TL, mySobek_User_Group_Link GL
	where ( GL.UserID = @userid ) and ( GL.UserGroupID = TL.UserGroupID ) and ( TL.TemplateID = T.TemplateID )
	order by DefaultTemplate DESC, TemplateCode ASC;
	
	-- Get the projects
	select P.ProjectCode, P.ProjectName, GroupDefined='false', DefaultProject
	from mySobek_Project P, mySobek_User_Project_Link L
	where ( L.UserID = @userid ) and ( L.ProjectID = P.ProjectID )
	union
	select P.ProjectCode, P.ProjectName, GroupDefined='true', 'false'
	from mySobek_Project P, mySobek_User_Group_Project_Link PL, mySobek_User_Group_Link GL
	where ( GL.UserID = @userid ) and ( GL.UserGroupID = PL.UserGroupID ) and ( PL.ProjectID = P.ProjectID )
	order by DefaultProject DESC, ProjectCode ASC;

	-- Get the bib id's of items submitted
	select distinct( G.BibID )
	from mySobek_User_Folder F, mySobek_User_Item B, SobekCM_Item I, SobekCM_Item_Group G
	where ( F.UserID = @userid ) and ( B.UserFolderID = F.UserFolderID ) and ( F.FolderName = 'Submitted Items' ) and ( B.ItemID = I.ItemID ) and ( I.GroupID = G.GroupID );

	-- Get the regular expression for editable items
	select R.EditableRegex, GroupDefined='false'
	from mySobek_Editable_Regex R, mySobek_User_Editable_Link L
	where ( L.UserID = @userid ) and ( L.EditableID = R.EditableID )
	union
	select R.EditableRegex, GroupDefined='true'
	from mySobek_Editable_Regex R, mySobek_User_Group_Editable_Link L, mySobek_User_Group_Link GL
	where ( GL.UserID = @userid ) and ( GL.UserGroupID = L.UserGroupID ) and ( L.EditableID = R.EditableID );

	-- Get the list of aggregations associated with this user
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsAdmin AS IsAggregationAdmin, L.OnHomePage, L.IsCurator AS IsCollectionManager, GroupDefined='false'
	from SobekCM_Item_Aggregation A, mySobek_User_Edit_Aggregation L
	where  ( L.AggregationID = A.AggregationID ) and ( L.UserID = @userid )
	union
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsAdmin AS IsAggregationAdmin, OnHomePage = 'false', L.IsCurator AS IsCollectionManager, GroupDefined='true'
	from SobekCM_Item_Aggregation A, mySobek_User_Group_Edit_Aggregation L, mySobek_User_Group_Link GL
	where  ( L.AggregationID = A.AggregationID ) and ( GL.UserID = @userid ) and ( GL.UserGroupID = L.UserGroupID );

	-- Return the names of all the folders
	select F.FolderName, F.UserFolderID, ParentFolderID=isnull(F.ParentFolderID,-1), isPublic
	from mySobek_User_Folder F
	where ( F.UserID=@userid );

	-- Get the list of all items associated with a user folder (other than submitted items)
	select G.BibID, I.VID
	from mySobek_User_Folder F, mySobek_User_Item B, SobekCM_Item I, SobekCM_Item_Group G
	where ( F.UserID = @userid ) and ( B.UserFolderID = F.UserFolderID ) and ( F.FolderName != 'Submitted Items' ) and ( B.ItemID = I.ItemID ) and ( I.GroupID = G.GroupID );
	
	-- Get the list of all user groups associated with this user
	select G.GroupName, Can_Submit_Items, Internal_User, IsSystemAdmin, IsPortalAdmin, Include_Tracking_Standard_Forms 
	from mySobek_User_Group G, mySobek_User_Group_Link L
	where ( G.UserGroupID = L.UserGroupID )
	  and ( L.UserID = @userid );
	  
	-- Update the user table to include this as the last activity
	update mySobek_User
	set LastActivity = getdate()
	where UserID=@userid;
END;
GO


-- Procedure allows an admin to edit permissions flags for this user
ALTER PROCEDURE [dbo].[mySobek_Update_UFDC_User]
	@userid int,
	@can_submit bit,
	@is_internal bit,
	@can_edit_all bit,
	@can_delete_all bit,
	@is_portal_admin bit,
	@is_system_admin bit,
	@include_tracking_standard_forms bit,
	@edit_template varchar(20),
	@edit_template_marc varchar(20),
	@clear_projects_templates bit,
	@clear_aggregation_links bit,
	@clear_user_groups bit
AS
begin transaction

	-- Update the simple table values
	update mySobek_User
	set Can_Submit_Items=@can_submit, Internal_User=@is_internal, 
		IsPortalAdmin=@is_portal_admin, IsSystemAdmin=@is_system_admin, 
		Include_Tracking_Standard_Forms=@include_tracking_standard_forms, 
		EditTemplate=@edit_template, Can_Delete_All_Items = @can_delete_all,
		EditTemplateMarc=@edit_template_marc
	where UserID=@userid;

	-- Check the flag to edit all items
	if ( @can_edit_all = 'true' )
	begin	
		if ( ( select count(*) from mySobek_User_Editable_Link where EditableID=1 and UserID=@userid ) = 0 )
		begin
			-- Add the link to the ALL EDITABLE
			insert into mySobek_User_Editable_Link ( UserID, EditableID )
			values ( @userid, 1 );
		end;
	end
	else
	begin
		-- Delete the link to all
		delete from mySobek_User_Editable_Link where EditableID = 1 and UserID=@userid;
	end;

	-- Clear the projects/templates
	if ( @clear_projects_templates = 'true' )
	begin
		delete from mySobek_User_Project_Link where UserID=@userid;
		delete from mySobek_User_Template_Link where UserID=@userid;
	end;

	-- Clear the projects/templates
	if ( @clear_aggregation_links = 'true' )
	begin
		delete from mySobek_User_Edit_Aggregation where UserID=@userid;
	end;
	
	-- Clear the user groups
	if ( @clear_user_groups = 'true' )
	begin
		delete from mySobek_User_Group_Link where UserID=@userid;
	end;

commit transaction;
GO


-- Procedure to add links between a user and item aggregations
ALTER PROCEDURE [dbo].[mySobek_Add_User_Aggregations_Link]
	@userid int,
	@aggregationcode1 varchar(20),
	@canselect1 bit,
	@canedit1 bit,
	@iscurator1 bit,
	@onhomepage1 bit,
	@isadmin1 bit,
	@aggregationcode2 varchar(20),
	@canselect2 bit,
	@canedit2 bit,
	@iscurator2 bit,	
	@onhomepage2 bit,
	@isadmin2 bit,
	@aggregationcode3 varchar(20),
	@canselect3 bit,
	@canedit3 bit,
	@iscurator3 bit,
	@onhomepage3 bit,
	@isadmin3 bit
AS
BEGIN
	
	-- Add the first aggregation
	if (( len(@aggregationcode1) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode1 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation1_id int
		select @aggregation1_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode1

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditItems, IsCurator, OnHomePage, IsAdmin )
		values ( @userid, @aggregation1_id, @canselect1, @canedit1, @iscurator1, @onhomepage1, @isadmin1 )
	end

	-- Add the second aggregation
	if (( len(@aggregationcode2) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation2_id int
		select @aggregation2_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode2

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditItems, IsCurator, OnHomePage, IsAdmin )
		values ( @userid, @aggregation2_id, @canselect2, @canedit2, @iscurator2, @onhomepage2, @isadmin2 )
	end

	-- Add the third aggregation
	if (( len(@aggregationcode3) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation3_id int
		select @aggregation3_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode3

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditItems, IsCurator, OnHomePage, IsAdmin )
		values ( @userid, @aggregation3_id, @canselect3, @canedit3, @iscurator3, @onhomepage3, @isadmin3 )
	end
END
GO