-- Add internal notes to the user table
alter table mySobek_User add InternalNotes nvarchar(500) null;
GO


-- Add columns to the user group table for the group to be a default 
-- depending on the authentication method
alter table mySobek_User_Group add IsSobekDefault bit not null default('false');
alter table mySobek_User_Group add IsShibbolethDefault bit not null default('false');
alter table mySobek_User_Group add IsLdapDefault bit not null default('false');
GO

-- Rename UFID to ShibbID ( last remnant of UFDC-only original design)
sp_RENAME 'mySobek_User.UFID' , 'ShibbID', 'COLUMN';
GO

/****** Object:  StoredProcedure [dbo].[mySobek_Save_User2]    Script Date: 12/20/2013 05:43:36 ******/
-- Saves a user
ALTER PROCEDURE [dbo].[mySobek_Save_User]
	@userid int,
	@shibbid char(8),
	@username nvarchar(100),
	@password nvarchar(100),
	@emailaddress nvarchar(100),
	@firstname nvarchar(100),
	@lastname nvarchar(100),
	@cansubmititems bit,
	@nickname nvarchar(100),
	@organization nvarchar(250),
	@college nvarchar(250),
	@department nvarchar(250),
	@unit nvarchar(250),
	@rights nvarchar(1000),
	@sendemail bit,
	@language nvarchar(50),
	@default_template varchar(50),
	@default_metadata varchar(50),
	@organization_code varchar(15),
	@receivestatsemail bit,
	@scanningtechnician bit,
	@processingtechnician bit,
	@internalnotes nvarchar(500),
	@authentication varchar(20)
	
AS
BEGIN

	if ( @userid < 0 )
	begin

		-- Add this into the user table first
		insert into mySobek_User ( ShibbID, UserName, [Password], EmailAddress, LastName, FirstName, DateCreated, LastActivity, isActive,  Note_Length, Can_Make_Folders_Public, isTemporary_Password, Can_Submit_Items, NickName, Organization, College, Department, Unit, Default_Rights, sendEmailOnSubmission, UI_Language, Internal_User, OrganizationCode, Receive_Stats_Emails, Include_Tracking_Standard_Forms, ScanningTechnician, ProcessingTechnician, InternalNotes )
		values ( @shibbid, @username, @password, @emailaddress, @lastname, @firstname, getdate(), getDate(), 'true', 1000, 'true', 'false', @cansubmititems, @nickname, @organization, @college, @department, @unit, @rights, @sendemail, @language, 'false', @organization_code, @receivestatsemail, 'false', @scanningtechnician, @processingtechnician, @internalnotes );

		-- Get the user is
		declare @newuserid int;
		set @newuserid = @@identity;
		
		-- This is a brand new user, so we must set the default groups, according to
		-- the authentication method
		-- Authentticated used the built-in Sobek authentication
		if (( @authentication='sobek' ) and (( select count(*) from mySobek_user_Group where IsSobekDefault = 'true' ) > 0 ))
		begin
			-- insert any groups set as default for this
			insert into mySobek_User_Group_Link ( UserID, UserGroupID )
			select @newuserid, UserGroupID
			from mySobek_User_Group where IsSobekDefault='true';
		end;
		
		-- Authenticated using Shibboleth authentication
		if (( @authentication='shibboleth' ) and (( select count(*) from mySobek_user_Group where IsShibbolethDefault = 'true' ) > 0 ))
		begin
			-- insert any groups set as default for this
			insert into mySobek_User_Group_Link ( UserID, UserGroupID )
			select @newuserid, UserGroupID
			from mySobek_User_Group where IsShibbolethDefault='true';
		end;
		
		-- Authenticated using Ldap authentication
		if (( @authentication='ldap' ) and (( select count(*) from mySobek_user_Group where IsLdapDefault = 'true' ) > 0 ))
		begin
			-- insert any groups set as default for this
			insert into mySobek_User_Group_Link ( UserID, UserGroupID )
			select @newuserid, UserGroupID
			from mySobek_User_Group where IsLdapDefault='true';
		end;
	end
	else
	begin

		-- Update this user
		update mySobek_User
		set ShibbID = @shibbid, UserName = @username, EmailAddress=@emailAddress,
			Firstname = @firstname, Lastname = @lastname, Can_Submit_Items = @cansubmititems,
			NickName = @nickname, Organization=@organization, College=@college, Department=@department,
			Unit=@unit, Default_Rights=@rights, sendEmailOnSubmission = @sendemail, UI_Language=@language,
			OrganizationCode=@organization_code, Receive_Stats_Emails=@receivestatsemail,
			ScanningTechnician=@scanningtechnician, ProcessingTechnician=@processingtechnician,
			InternalNotes=@internalnotes
		where UserID = @userid;

		-- Set the default template
		if ( len( @default_template ) > 0 )
		begin
			-- Get the template id
			declare @templateid int;
			select @templateid = TemplateID from mySobek_Template where TemplateCode=@default_template;

			-- Clear the current default template
			update mySobek_User_Template_Link set DefaultTemplate = 'false' where UserID=@userid;

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_Template_Link where UserID=@userid and TemplateID=@templateid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_Template_Link set DefaultTemplate = 'true' where UserID=@userid and TemplateID=@templateid;
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate ) values ( @userid, @templateid, 'true' );
			end;
		end;

		-- Set the default metadata
		if ( len( @default_metadata ) > 0 )
		begin
			-- Get the project id
			declare @projectid int;
			select @projectid = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@default_metadata;

			-- Clear the current default project
			update mySobek_User_DefaultMetadata_Link set CurrentlySelected = 'false' where UserID=@userid;

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_DefaultMetadata_Link where UserID=@userid and DefaultMetadataID=@projectid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_DefaultMetadata_Link set CurrentlySelected = 'true' where UserID=@userid and DefaultMetadataID=@projectid;
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected ) values ( @userid, @projectid, 'true' );
			end;
		end;
	end;
END;
GO

/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_By_UserID]    Script Date: 12/20/2013 05:43:35 ******/
ALTER PROCEDURE [dbo].[mySobek_Get_User_By_UserID]
	@userid int
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the basic user information
	select UserID, ShibbID=coalesce(ShibbID,''), UserName=coalesce(UserName,''), EmailAddress=coalesce(EmailAddress,''), 
	  FirstName=coalesce(FirstName,''), LastName=coalesce(LastName,''), Note_Length, 
	  Can_Make_Folders_Public, isTemporary_Password, sendEmailOnSubmission, Can_Submit_Items, 
	  NickName=coalesce(NickName,''), Organization=coalesce(Organization, ''), College=coalesce(College,''),
	  Department=coalesce(Department,''), Unit=coalesce(Unit,''), Rights=coalesce(Default_Rights,''), Language=coalesce(UI_Language, ''), 
	  Internal_User, OrganizationCode, EditTemplate, EditTemplateMarc, IsSystemAdmin, IsPortalAdmin, Include_Tracking_Standard_Forms,
	  Descriptions=( select COUNT(*) from mySobek_User_Description_Tags T where T.UserID=U.UserID),
	  Receive_Stats_Emails, Has_Item_Stats, Can_Delete_All_Items, ScanningTechnician, ProcessingTechnician, InternalNotes=coalesce(InternalNotes,'')
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
	
	-- Get the default metadata
	select P.MetadataCode, P.MetadataName, GroupDefined='false', CurrentlySelected
	from mySobek_DefaultMetadata P, mySobek_User_DefaultMetadata_Link L
	where ( L.UserID = @userid ) and ( L.DefaultMetadataID = P.DefaultMetadataID )
	union
	select P.MetadataCode, P.MetadataName, GroupDefined='true', 'false'
	from mySobek_DefaultMetadata P, mySobek_User_Group_DefaultMetadata_Link PL, mySobek_User_Group_Link GL
	where ( GL.UserID = @userid ) and ( GL.UserGroupID = PL.UserGroupID ) and ( PL.DefaultMetadataID = P.DefaultMetadataID )
	order by CurrentlySelected DESC, MetadataCode ASC;

	-- Get the bib id's of items submitted
	select distinct( G.BibID )
	from mySobek_User_Folder F, mySobek_User_Item B, SobekCM_Item I, SobekCM_Item_Group G
	where ( F.UserID = @userid ) and ( B.UserFolderID = F.UserFolderID ) and ( F.FolderName = 'Submitted Items' ) and ( B.ItemID = I.ItemID ) and ( I.GroupID = G.GroupID );

	-- Get the regular expression for editable items
	select R.EditableRegex, GroupDefined='false', CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete
	from mySobek_Editable_Regex R, mySobek_User_Editable_Link L
	where ( L.UserID = @userid ) and ( L.EditableID = R.EditableID )
	union
	select R.EditableRegex, GroupDefined='true', CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete
	from mySobek_Editable_Regex R, mySobek_User_Group_Editable_Link L, mySobek_User_Group_Link GL
	where ( GL.UserID = @userid ) and ( GL.UserGroupID = L.UserGroupID ) and ( L.EditableID = R.EditableID );

	-- Get the list of aggregations associated with this user
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsAdmin AS IsAggregationAdmin, L.OnHomePage, L.IsCurator AS IsCollectionManager, GroupDefined='false', CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete
	from SobekCM_Item_Aggregation A, mySobek_User_Edit_Aggregation L
	where  ( L.AggregationID = A.AggregationID ) and ( L.UserID = @userid )
	union
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsAdmin AS IsAggregationAdmin, OnHomePage = 'false', L.IsCurator AS IsCollectionManager, GroupDefined='true', CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete
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
	  
	-- Get the user settings
	select * from mySobek_User_Settings where UserID=@userid;
	  
	-- Update the user table to include this as the last activity
	update mySobek_User
	set LastActivity = getdate()
	where UserID=@userid;
END;
GO


/****** Object:  StoredProcedure [dbo].[mySobek_Save_User_Group]    Script Date: 12/20/2013 05:43:36 ******/
-- Saves information about a single user group
ALTER PROCEDURE [dbo].[mySobek_Save_User_Group]
	@usergroupid int,
	@groupname nvarchar(150),
	@groupdescription nvarchar(1000),
	@can_submit_items bit,
	@is_internal bit,
	@can_edit_all bit,
	@is_system_admin bit,
	@is_portal_admin bit,
	@include_tracking_standard_forms bit,
	@clear_metadata_templates bit,
	@clear_aggregation_links bit,
	@clear_editable_links bit,
	@is_sobek_default bit,
	@is_shibboleth_default bit,
	@is_ldap_default bit,
	@new_usergroupid int output
AS 
begin
	
	-- Is there a user group id provided
	if ( @usergroupid < 0 )
	begin
		-- Insert as a new user group
		insert into mySobek_User_Group ( GroupName, GroupDescription, Can_Submit_Items, Internal_User, IsSystemAdmin, IsPortalAdmin, Include_Tracking_Standard_Forms, IsSobekDefault, IsShibbolethDefault, IsLdapDefault  )
		values ( @groupname, @groupdescription, @can_submit_items, @is_internal, @is_system_admin, @is_portal_admin, @include_tracking_standard_forms, @is_sobek_default, @is_shibboleth_default, @is_ldap_default );
		
		-- Return the new primary key
		set @new_usergroupid = @@IDENTITY;	
	end
	else
	begin
		-- Update, if it exists
		update mySobek_User_Group
		set GroupName = @groupname, GroupDescription = @groupdescription, Can_Submit_Items = @can_submit_items, Internal_User=@is_internal, IsSystemAdmin=@is_system_admin, IsPortalAdmin=@is_portal_admin, Include_Tracking_Standard_Forms=@include_tracking_standard_forms, 
			IsSobekDefault=@is_sobek_default, IsShibbolethDefault=@is_shibboleth_default, IsLdapDefault=@is_ldap_default
		where UserGroupID = @usergroupid;
	
	end;
	
	-- Check the flag to edit all items
	if ( @can_edit_all = 'true' )
	begin	
		if ( ( select count(*) from mySobek_User_Group_Editable_Link where EditableID=1 and UserGroupID=@usergroupid ) = 0 )
		begin
			-- Add the link to the ALL EDITABLE
			insert into mySobek_User_Group_Editable_Link ( UserGroupID, EditableID )
			values ( @usergroupid, 1 );
		end
	end
	else
	begin
		-- Delete the link to all
		delete from mySobek_User_Group_Editable_Link where EditableID = 1 and UserGroupID=@usergroupid;
	end;
	
		-- Clear the projects/templates
	if ( @clear_metadata_templates = 'true' )
	begin
		delete from mySobek_User_Group_DefaultMetadata_Link where UserGroupID=@usergroupid;
		delete from mySobek_User_Group_Template_Link where UserGroupID=@usergroupid;
	end;

	-- Clear the aggregations link
	if ( @clear_aggregation_links = 'true' )
	begin
		delete from mySobek_User_Group_Edit_Aggregation where UserGroupID=@usergroupid;
	end;
	
	-- Clear the editable link
	if ( @clear_editable_links = 'true' )
	begin
		delete from mySobek_User_Group_Editable_Link where UserGroupID=@usergroupid;
	end;

end;
GO

CREATE PROCEDURE [dbo].[mySobek_Get_User_By_ShibbID]
	@shibbid char(8)
AS
BEGIN  

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Look for the user by Shibboleth ID.  Does one exist?
	if (( select COUNT(*) from mySobek_User where ShibbID=@shibbid and isActive = 'true' ) = 1 )
	begin
		-- Get the userid for this user
		declare @userid int;
		select @userid = UserID from mySobek_User where ShibbID=@shibbid and isActive = 'true';  
  
		-- Stored procedure used to return standard data across all user fetch stored procedures
		exec mySobek_Get_User_By_UserID @userid; 
	end;
END;
GO

GRANT EXECUTE ON [dbo].[mySobek_Get_User_By_ShibbID] to sobek_user;
GO

DROP PROCEDURE [dbo].[mySobek_Get_User_By_UFID];
GO

UPDATE mySobek_User set ProcessingTechnician='false' where ProcessingTechnician is null;
UPDATE mySobek_User set ScanningTechnician='false' where ScanningTechnician is null;
GO

ALTER TABLE mySobek_User ALTER COLUMN ProcessingTechnician bit not null;
ALTER TABLE mySobek_User ALTER COLUMN ScanningTechnician bit not null;
GO



