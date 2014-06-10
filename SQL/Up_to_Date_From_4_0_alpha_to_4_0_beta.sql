-- SCRIPT TO ADD MORE SPECIFICITY TO THE USER PERMISSIONS AND
-- ALSO TO ADD PROCEDURE FOR SAVING USER SETTINGS VALUES TO DB



IF ( NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='mySobek_DefaultMetadata' ))
BEGIN
	/****** Object:  Table [dbo].[mySobek_DefaultMetadata]    Script Date: 12/20/2013 05:43:28 ******/
	CREATE TABLE [dbo].[mySobek_DefaultMetadata](
		[DefaultMetadataID] [int] IDENTITY(1,1) NOT NULL,
		[MetadataName] [varchar](100) NOT NULL,
		[MetadataCode] [varchar](20) NOT NULL,
	 CONSTRAINT [PK_mySobek_DefaultMetadata] PRIMARY KEY CLUSTERED 
	(
		[DefaultMetadataID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END;
GO

IF ( NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='mySobek_User_DefaultMetadata_Link' ))
BEGIN
	/****** Object:  Table [dbo].[mySobek_User_DefaultMetadata_Link]    Script Date: 12/20/2013 05:43:29 ******/
	CREATE TABLE [dbo].[mySobek_User_DefaultMetadata_Link](
		[UserID] [int] NOT NULL,
		[DefaultMetadataID] [int] NOT NULL,
		[CurrentlySelected] [bit] NOT NULL,
	 CONSTRAINT [PK_mySobek_User_DefaultMetadata_Link] PRIMARY KEY CLUSTERED 
	(
		[UserID] ASC,
		[DefaultMetadataID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY];
	
	/****** Object:  ForeignKey [FK_sobek_user_Project_Link_UFDC_Project]    Script Date: 12/20/2013 05:43:29 ******/
	ALTER TABLE [dbo].[mySobek_User_DefaultMetadata_Link]  WITH CHECK ADD  CONSTRAINT [FK_sobek_user_DefaultMetadata_Link] FOREIGN KEY([DefaultMetadataID])
	REFERENCES [dbo].[mySobek_DefaultMetadata] ([DefaultMetadataID]);

	ALTER TABLE [dbo].[mySobek_User_DefaultMetadata_Link] CHECK CONSTRAINT [FK_sobek_user_DefaultMetadata_Link];

	/****** Object:  ForeignKey [FK_sobek_user_Project_Link_sobek_user]    Script Date: 12/20/2013 05:43:29 ******/
	ALTER TABLE [dbo].[mySobek_User_DefaultMetadata_Link]  WITH CHECK ADD  CONSTRAINT [FK_mysobek_user_DefaultMetadata_Link_sobek_user] FOREIGN KEY([UserID])
	REFERENCES [dbo].[mySobek_User] ([UserID]);

	ALTER TABLE [dbo].[mySobek_User_DefaultMetadata_Link] CHECK CONSTRAINT [FK_mysobek_user_DefaultMetadata_Link_sobek_user];
END;
GO

IF ( NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='mySobek_User_Group_DefaultMetadata_Link' ))
BEGIN
	/****** Object:  Table [dbo].[mySobek_User_Group_DefaultMetadata_Link]    Script Date: 12/20/2013 05:43:29 ******/
	CREATE TABLE [dbo].[mySobek_User_Group_DefaultMetadata_Link](
		[UserGroupID] [int] NOT NULL,
		[DefaultMetadataID] [int] NOT NULL,
	 CONSTRAINT [PK_sobek_user_Group_DefaultMetadata_Link] PRIMARY KEY CLUSTERED 
	(
		[UserGroupID] ASC,
		[DefaultMetadataID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY];
	
	/****** Object:  ForeignKey [FK_sobek_user_Group_Project_Link_UFDC_Project]    Script Date: 12/20/2013 05:43:29 ******/
	ALTER TABLE [dbo].[mySobek_User_Group_DefaultMetadata_Link]  WITH CHECK ADD  CONSTRAINT [FK_sobek_user_Group_DefaultMetadata_Link] FOREIGN KEY([DefaultMetadataID])
	REFERENCES [dbo].[mySobek_DefaultMetadata] ([DefaultMetadataID]);

	ALTER TABLE [dbo].[mySobek_User_Group_DefaultMetadata_Link] CHECK CONSTRAINT [FK_sobek_user_Group_DefaultMetadata_Link];

	/****** Object:  ForeignKey [FK_sobek_user_Group_DefaultMetadata_Link_sobek_user]    Script Date: 12/20/2013 05:43:29 ******/
	ALTER TABLE [dbo].[mySobek_User_Group_DefaultMetadata_Link]  WITH CHECK ADD  CONSTRAINT [FK_sobek_user_Group_DefaultMetadata_Link_sobek_user] FOREIGN KEY([UserGroupID])
	REFERENCES [dbo].[mySobek_User_Group] ([UserGroupID]);

	ALTER TABLE [dbo].[mySobek_User_Group_DefaultMetadata_Link] CHECK CONSTRAINT [FK_sobek_user_Group_DefaultMetadata_Link_sobek_user];

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


/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_Group]    Script Date: 12/20/2013 05:43:35 ******/
-- Get information about a single user group, by user group id
ALTER PROCEDURE [dbo].[mySobek_Get_User_Group]
	@usergroupid int
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the basic user group information
	select *
	from mySobek_User_Group G
	where ( G.UserGroupID = @usergroupid );

	-- Get the templates
	select T.TemplateCode, T.TemplateName
	from mySobek_Template T, mySobek_User_Group_Template_Link TL
	where ( TL.UserGroupID = @usergroupid ) and ( TL.TemplateID = T.TemplateID );

	-- Get the default metadata
	select P.MetadataCode, P.MetadataName
	from mySobek_DefaultMetadata P, mySobek_User_Group_DefaultMetadata_Link PL
	where ( PL.UserGroupID = @usergroupid ) and ( PL.DefaultMetadataID = P.DefaultMetadataID );

	-- Get the regular expression for editable items
	select R.EditableRegex
	from mySobek_Editable_Regex R, mySobek_User_Group_Editable_Link L
	where ( L.UserGroupID = @usergroupid ) and ( L.EditableID = R.EditableID );

	-- Get the list of aggregations associated with this user
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsCurator
	from SobekCM_Item_Aggregation A, mySobek_User_Group_Edit_Aggregation L
	where  ( L.AggregationID = A.AggregationID ) and ( L.UserGroupID = @usergroupid );

	-- Get the list of all user's linked to this user group
	select U.UserID, U.UserName, U.EmailAddress, U.FirstName, U.NickName, U.LastName
	from mySobek_User U, mySobek_User_Group_Link L
	where ( L.UserGroupID = @usergroupid )
	  and ( L.UserID = U.UserID );
END;
GO


/****** Object:  StoredProcedure [dbo].[mySobek_Save_User2]    Script Date: 12/20/2013 05:43:36 ******/
-- Saves a user
ALTER PROCEDURE [dbo].[mySobek_Save_User]
	@userid int,
	@ufid char(8),
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
	@receivestatsemail bit
AS
BEGIN

	if ( @userid < 0 )
	begin

		-- Add this into the user table first
		insert into mySobek_User ( UFID, UserName, [Password], EmailAddress, LastName, FirstName, DateCreated, LastActivity, isActive,  Note_Length, Can_Make_Folders_Public, isTemporary_Password, Can_Submit_Items, NickName, Organization, College, Department, Unit, Default_Rights, sendEmailOnSubmission, UI_Language, Internal_User, OrganizationCode, Receive_Stats_Emails )
		values ( @ufid, @username, @password, @emailaddress, @lastname, @firstname, getdate(), getDate(), 'true', 1000, 'true', 'false', @cansubmititems, @nickname, @organization, @college, @department, @unit, @rights, @sendemail, @language, 'false', @organization_code, @receivestatsemail );

		-- Get the user is
		declare @newuserid int;
		set @newuserid = @@identity;

	end
	else
	begin

		-- Update this user
		update mySobek_User
		set UFID = @ufid, UserName = @username, EmailAddress=@emailAddress,
			Firstname = @firstname, Lastname = @lastname, Can_Submit_Items = @cansubmititems,
			NickName = @nickname, Organization=@organization, College=@college, Department=@department,
			Unit=@unit, Default_Rights=@rights, sendEmailOnSubmission = @sendemail, UI_Language=@language,
			OrganizationCode=@organization_code, Receive_Stats_Emails=@receivestatsemail
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
			update mySobek_User_DefaultMetadata_Link set CurrentlySelected = 'false' where UserID=@userid

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_DefaultMetadata_Link where UserID=@userid and DefaultMetadataID=@projectid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_DefaultMetadata_Link set CurrentlySelected = 'true' where UserID=@userid and DefaultMetadataID=@projectid
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected ) values ( @userid, @projectid, 'true' )				
			end
		end
	end	
END
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
	@new_usergroupid int output
AS 
begin
	
	-- Is there a user group id provided
	if ( @usergroupid < 0 )
	begin
		-- Insert as a new user group
		insert into mySobek_User_Group ( GroupName, GroupDescription, Can_Submit_Items, Internal_User, IsSystemAdmin, IsPortalAdmin, Include_Tracking_Standard_Forms  )
		values ( @groupname, @groupdescription, @can_submit_items, @is_internal, @is_system_admin, @is_portal_admin, @include_tracking_standard_forms );
		
		-- Return the new primary key
		set @new_usergroupid = @@IDENTITY;	
	end
	else
	begin
		-- Update, if it exists
		update mySobek_User_Group
		set GroupName = @groupname, GroupDescription = @groupdescription, Can_Submit_Items = @can_submit_items, Internal_User=@is_internal, IsSystemAdmin=@is_system_admin, IsPortalAdmin=@is_portal_admin, Include_Tracking_Standard_Forms=@include_tracking_standard_forms
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

-- Ensure the stored procedure exists
IF object_id('mySobek_Add_User_Group_Metadata_Link') IS NULL EXEC ('create procedure dbo.mySobek_Add_User_Group_Metadata_Link as select 1;');
GO

/****** Object:  StoredProcedure [dbo].[[mySobek_Add_User_Group_Metadata_Link]]    Script Date: 12/20/2013 05:43:35 ******/
-- Add a link between a user and a set of default metadata 
ALTER PROCEDURE [dbo].[mySobek_Add_User_Group_Metadata_Link]
	@usergroupid int,
	@metadata1 varchar(20),
	@metadata2 varchar(20),
	@metadata3 varchar(20),
	@metadata4 varchar(20),
	@metadata5 varchar(20)
AS
begin

	-- Add the first default metadata
	if (( len(@metadata1) > 0 ) and ( (select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata1 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata1_id int;
		select @metadata1_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata1;

		-- Add this one as a default
		insert into mySobek_User_Group_DefaultMetadata_Link ( UserGroupID, DefaultMetadataID )
		values ( @usergroupid, @metadata1_id );
	end;

	-- Add the second default metadata
	if (( len(@metadata2) > 0 ) and ( (select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata2_id int;
		select @metadata2_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata2;

		-- Add this one as a default
		insert into mySobek_User_Group_DefaultMetadata_Link ( UserGroupID, DefaultMetadataID )
		values ( @usergroupid, @metadata2_id );
	end;

	-- Add the third detault metadata
	if (( len(@metadata3) > 0 ) and ( (select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata3_id int;
		select @metadata3_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata3;

		-- Add this one as a default
		insert into mySobek_User_Group_DefaultMetadata_Link ( UserGroupID, DefaultMetadataID )
		values ( @usergroupid, @metadata3_id );
	end;

	-- Add the fourth default metadata
	if (( len(@metadata4) > 0 ) and ( (select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata4 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata4_id int;
		select @metadata4_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata4;

		-- Add this one as a default
		insert into mySobek_User_Group_DefaultMetadata_Link ( UserGroupID, DefaultMetadataID )
		values ( @usergroupid, @metadata4_id );
	end;

	-- Add the fifth default metadata
	if (( len(@metadata5) > 0 ) and ( (select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata5 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata5_id int;
		select @metadata5_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata5;

		-- Add this one as a default
		insert into mySobek_User_Group_DefaultMetadata_Link ( UserGroupID, DefaultMetadataID )
		values ( @usergroupid, @metadata5_id );
	end;
end;
GO

-- Ensure the stored procedure exists
IF object_id('mySobek_Add_User_DefaultMetadata_Link') IS NULL EXEC ('create procedure dbo.mySobek_Add_User_DefaultMetadata_Link as select 1;');
GO


/****** Object:  StoredProcedure [dbo].[[mySobek_Add_User_DefaultMetadata_Link]]    Script Date: 12/20/2013 05:43:35 ******/
-- Add a link between a user and default metadata 
ALTER PROCEDURE [dbo].[mySobek_Add_User_DefaultMetadata_Link]
	@userid int,
	@metadata_default varchar(20),
	@metadata2 varchar(20),
	@metadata3 varchar(20),
	@metadata4 varchar(20),
	@metadata5 varchar(20)
AS
begin

	-- Add the default default metadata
	if (( len(@metadata_default) > 0 ) and ( (select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata_default ) = 1 ))
	begin
		-- Clear any previous default
		update mySobek_User_DefaultMetadata_Link set CurrentlySelected='false' where UserID = @userid;

		-- Get the id for this one
		declare @metadata_default_id int;
		select @metadata_default_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata_default;

		-- Add this one as a default
		insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected )
		values ( @userid, @metadata_default_id, 'true' );
	end;

	-- Add the second default metadata
	if (( len(@metadata2) > 0 ) and ((select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata2_id int;
		select @metadata2_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata2;

		-- Add this one
		insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected )
		values ( @userid, @metadata2_id, 'false' );
	end;

	-- Add the third default metadata
	if (( len(@metadata3) > 0 ) and ((select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata3_id int;
		select @metadata3_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata3;

		-- Add this one
		insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected )
		values ( @userid, @metadata3_id, 'false' );
	end;

	-- Add the fourth default metadata
	if (( len(@metadata4) > 0 ) and ((select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata4 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata4_id int;
		select @metadata4_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata4;

		-- Add this one
		insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected )
		values ( @userid, @metadata4_id, 'false' );
	end;

	-- Add the fifth default metadata
	if (( len(@metadata5) > 0 ) and ((select count(*) from mySobek_DefaultMetadata where MetadataCode = @metadata5 ) = 1 ))
	begin
		-- Get the id for this one
		declare @metadata5_id int;
		select @metadata5_id = DefaultMetadataID from mySobek_DefaultMetadata where MetadataCode=@metadata5;

		-- Add this one
		insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected )
		values ( @userid, @metadata5_id, 'false' );
	end;
end;
GO

-- Ensure the stored procedure exists
IF object_id('mySobek_Save_DefaultMetadata') IS NULL EXEC ('create procedure dbo.mySobek_Save_DefaultMetadata as select 1;');
GO

/****** Object:  StoredProcedure [dbo].[[mySobek_Save_DefaultMetadata]]    Script Date: 12/20/2013 05:43:36 ******/
-- Add a new default metadata set to this database
ALTER PROCEDURE [dbo].[mySobek_Save_DefaultMetadata]
	@metadata_code varchar(20),
	@metadata_name varchar(100)
AS
BEGIN
	
	-- Does this project already exist?
	if (( select count(*) from mySobek_DefaultMetadata where MetadataCode=@metadata_code ) > 0 )
	begin
		-- Update the existing default metadata
		update mySobek_DefaultMetadata
		set MetadataName = @metadata_name
		where MetadataCode = @metadata_code;
	end
	else
	begin
		-- Add a new set
		insert into mySobek_DefaultMetadata ( MetadataName, MetadataCode )
		values ( @metadata_name, @metadata_code );
	end;
END;
GO

-- Ensure the stored procedure exists
IF object_id('mySobek_Get_All_Template_DefaultMetadatas') IS NULL EXEC ('create procedure dbo.mySobek_Get_All_Template_DefaultMetadatas as select 1;');
GO

/****** Object:  StoredProcedure [dbo].[[mySobek_Get_All_Projects_DefaultMetadatas]]    Script Date: 12/20/2013 05:43:35 ******/
-- Get the list of all templates and default metadata sets 
ALTER PROCEDURE [dbo].[mySobek_Get_All_Templates_DefaultMetadatas]
AS
BEGIN
	
	select * 
	from mySobek_DefaultMetadata
	order by MetadataCode;

	select * 
	from mySobek_Template
	order by TemplateCode;

END;
GO

-- Drop the old procedures
DROP PROCEDURE [dbo].[mySobek_Save_User2];
DROP PROCEDURE [dbo].[mySobek_Add_User_Group_Projects_Link];
DROP PROCEDURE [dbo].[mySobek_Add_User_Projects_Link];
DROP PROCEDURE [dbo].[mySobek_Save_Project];
DROP PROCEDURE [dbo].[mySobek_Get_All_Projects_Templates];
GO

-- Grant permissions to new procedures
GRANT EXECUTE ON [dbo].[mySobek_Add_User_Group_Metadata_Link] TO sobek_user;
GRANT EXECUTE ON [dbo].[mySobek_Add_User_DefaultMetadata_Link] TO sobek_user;
GRANT EXECUTE ON [dbo].[mySobek_Save_DefaultMetadata] TO sobek_user;
GRANT EXECUTE ON [dbo].[mySobek_Get_All_Templates_DefaultMetadatas] TO sobek_user;
GO

-- Copy existing from PROJECT to the DEFAULT METADATA table
if (( select count(*) from dbo.mySobek_DefaultMetadata ) = 0 )
begin
	SET IDENTITY_INSERT [dbo].mySobek_DefaultMetadata ON;
	insert into mySobek_DefaultMetadata ( DefaultMetadataID, MetadataName, MetadataCode )
	select * from mySobek_Project;
	SET IDENTITY_INSERT [dbo].mySobek_DefaultMetadata OFF;
	
	-- Copy existing user/project links
	insert into mySobek_User_DefaultMetadata_Link ( UserID, DefaultMetadataID, CurrentlySelected )
	select UserID, ProjectID, DefaultProject from mySobek_User_Project_Link;
end;
GO


alter table mySobek_User_Edit_Aggregation add CanEditMetadata bit not null default('false');
alter table mySobek_User_Edit_Aggregation add CanEditBehaviors bit not null default('false');
alter table mySobek_User_Edit_Aggregation add CanPerformQc bit not null default('false');
alter table mySobek_User_Edit_Aggregation add CanUploadFiles bit not null default('false');
alter table mySobek_User_Edit_Aggregation add CanChangeVisibility bit not null default('false');
alter table mySobek_User_Edit_Aggregation add CanDelete bit not null default('false');
GO

alter table mySobek_User_Group_Edit_Aggregation add CanEditMetadata bit not null default('false');
alter table mySobek_User_Group_Edit_Aggregation add CanEditBehaviors bit not null default('false');
alter table mySobek_User_Group_Edit_Aggregation add CanPerformQc bit not null default('false');
alter table mySobek_User_Group_Edit_Aggregation add CanUploadFiles bit not null default('false');
alter table mySobek_User_Group_Edit_Aggregation add CanChangeVisibility bit not null default('false');
alter table mySobek_User_Group_Edit_Aggregation add CanDelete bit not null default('false');
GO

alter table mySobek_User_Editable_Link add CanEditMetadata bit not null default('false');
alter table mySobek_User_Editable_Link add CanEditBehaviors bit not null default('false');
alter table mySobek_User_Editable_Link add CanPerformQc bit not null default('false');
alter table mySobek_User_Editable_Link add CanUploadFiles bit not null default('false');
alter table mySobek_User_Editable_Link add CanChangeVisibility bit not null default('false');
alter table mySobek_User_Editable_Link add CanDelete bit not null default('false');
GO

alter table mySobek_User_Group_Editable_Link add CanEditMetadata bit not null default('false');
alter table mySobek_User_Group_Editable_Link add CanEditBehaviors bit not null default('false');
alter table mySobek_User_Group_Editable_Link add CanPerformQc bit not null default('false');
alter table mySobek_User_Group_Editable_Link add CanUploadFiles bit not null default('false');
alter table mySobek_User_Group_Editable_Link add CanChangeVisibility bit not null default('false');
alter table mySobek_User_Group_Editable_Link add CanDelete bit not null default('false');
GO

update mySobek_User_Edit_Aggregation
set CanEditMetadata=CanEditItems, CanEditBehaviors=CanEditItems, CanPerformQc=CanEditItems,
    CanUploadFiles=CanEditItems, CanChangeVisibility=CanEditItems, CanDelete=CanEditItems;
GO

update mySobek_User_Group_Edit_Aggregation
set CanEditMetadata=CanEditItems, CanEditBehaviors=CanEditItems, CanPerformQc=CanEditItems,
    CanUploadFiles=CanEditItems, CanChangeVisibility=CanEditItems, CanDelete=CanEditItems;
GO

update mySobek_User_Editable_Link
set CanEditMetadata='true', CanEditBehaviors='true', CanPerformQc='true',
    CanUploadFiles='true', CanChangeVisibility='true', CanDelete='true';
GO

update mySobek_User_Group_Editable_Link
set CanEditMetadata='true', CanEditBehaviors='true', CanPerformQc='true',
    CanUploadFiles='true', CanChangeVisibility='true', CanDelete='true';
GO

-- Procedure to add links between a user and item aggregations
ALTER PROCEDURE [dbo].[mySobek_Add_User_Aggregations_Link]
	@UserID int,
	@AggregationCode1 varchar(20),
	@canSelect1 bit,
	@canEditMetadata1 bit,
	@canEditBehaviors1 bit,
	@canPerformQc1 bit,
	@canUploadFiles1 bit,
	@canChangeVisibility1 bit,
	@canDelete1 bit,
	@isCurator1 bit,
	@onHomePage1 bit,
	@isAdmin1 bit,
	@AggregationCode2 varchar(20),
	@canSelect2 bit,
	@canEditMetadata2 bit,
	@canEditBehaviors2 bit,
	@canPerformQc2 bit,
	@canUploadFiles2 bit,
	@canChangeVisibility2 bit,
	@canDelete2 bit,
	@isCurator2 bit,
	@onHomePage2 bit,
	@isAdmin2 bit,
	@AggregationCode3 varchar(20),
	@canSelect3 bit,
	@canEditMetadata3 bit,
	@canEditBehaviors3 bit,
	@canPerformQc3 bit,
	@canUploadFiles3 bit,
	@canChangeVisibility3 bit,
	@canDelete3 bit,
	@isCurator3 bit,
	@onHomePage3 bit,
	@isAdmin3 bit
AS
BEGIN
	
	-- Add the first aggregation
	if (( len(@AggregationCode1) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode1 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation1_Id int;
		select @Aggregation1_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode1;

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, OnHomePage, IsAdmin, CanEditItems )
		values ( @UserID, @Aggregation1_Id, @canSelect1, @canEditMetadata1, @canEditBehaviors1, @canPerformQc1, @canUploadFiles1, @canChangeVisibility1, @canDelete1, @isCurator1, @onHomePage1, @isAdmin1, @canEditMetadata1 );
	end;
	
	-- Add the second aggregation
	if (( len(@AggregationCode2) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation2_Id int;
		select @Aggregation2_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode2;

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, OnHomePage, IsAdmin, CanEditItems )
		values ( @UserID, @Aggregation2_Id, @canSelect2, @canEditMetadata2, @canEditBehaviors2, @canPerformQc2, @canUploadFiles2, @canChangeVisibility2, @canDelete2, @isCurator2, @onHomePage2, @isAdmin2, @canEditMetadata2 );
	end;

	-- Add the third aggregation
	if (( len(@AggregationCode3) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation3_Id int;
		select @Aggregation3_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode3;

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, OnHomePage, IsAdmin, CanEditItems )
		values ( @UserID, @Aggregation3_Id, @canSelect3, @canEditMetadata3, @canEditBehaviors3, @canPerformQc3, @canUploadFiles3, @canChangeVisibility3, @canDelete3, @isCurator3, @onHomePage3, @isAdmin3, @canEditMetadata3 );
	end;
END;
GO

-- Procedure to add links between a user group and item aggregations
ALTER PROCEDURE [dbo].[mySobek_Add_User_Group_Aggregations_Link]
	@UserGroupID int,
	@AggregationCode1 varchar(20),
	@canSelect1 bit,
	@canEditMetadata1 bit,
	@canEditBehaviors1 bit,
	@canPerformQc1 bit,
	@canUploadFiles1 bit,
	@canChangeVisibility1 bit,
	@canDelete1 bit,
	@isCurator1 bit,
	@AggregationCode2 varchar(20),
	@canSelect2 bit,
	@canEditMetadata2 bit,
	@canEditBehaviors2 bit,
	@canPerformQc2 bit,
	@canUploadFiles2 bit,
	@canChangeVisibility2 bit,
	@canDelete2 bit,
	@isCurator2 bit,
	@AggregationCode3 varchar(20),
	@canSelect3 bit,
	@canEditMetadata3 bit,
	@canEditBehaviors3 bit,
	@canPerformQc3 bit,
	@canUploadFiles3 bit,
	@canChangeVisibility3 bit,
	@canDelete3 bit,
	@isCurator3 bit
AS
BEGIN
	
	-- Add the first aggregation
	if (( len(@AggregationCode1) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode1 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation1_Id int;
		select @Aggregation1_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode1;

		-- Add this one
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, CanEditItems )
		values ( @UserGroupID, @Aggregation1_Id, @canSelect1, @canEditMetadata1, @canEditBehaviors1, @canPerformQc1, @canUploadFiles1, @canChangeVisibility1, @canDelete1, @isCurator1, @canEditMetadata1 );
	end;
	
	-- Add the second aggregation
	if (( len(@AggregationCode2) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation2_Id int;
		select @Aggregation2_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode2;

		-- Add this one
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, CanEditItems )
		values ( @UserGroupID, @Aggregation2_Id, @canSelect2, @canEditMetadata2, @canEditBehaviors2, @canPerformQc2, @canUploadFiles2, @canChangeVisibility2, @canDelete2, @isCurator2, @canEditMetadata2 );
	end;
	
	-- Add the third aggregation
	if (( len(@AggregationCode3) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation3_Id int;
		select @Aggregation3_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode3;

		-- Add this one
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, CanEditItems )
		values ( @UserGroupID, @Aggregation3_Id, @canSelect3, @canEditMetadata3, @canEditBehaviors3, @canPerformQc3, @canUploadFiles3, @canChangeVisibility3, @canDelete3, @isCurator3, @canEditMetadata3 );
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


/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_Group]    Script Date: 12/20/2013 05:43:35 ******/
-- Get information about a single user group, by user group id
ALTER PROCEDURE [dbo].[mySobek_Get_User_Group]
	@usergroupid int
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the basic user group information
	select *
	from mySobek_User_Group G
	where ( G.UserGroupID = @usergroupid );

	-- Get the templates
	select T.TemplateCode, T.TemplateName
	from mySobek_Template T, mySobek_User_Group_Template_Link TL
	where ( TL.UserGroupID = @usergroupid ) and ( TL.TemplateID = T.TemplateID );

	-- Get the default metadata
	select P.MetadataCode, P.MetadataName
	from mySobek_DefaultMetadata P, mySobek_User_Group_DefaultMetadata_Link PL
	where ( PL.UserGroupID = @usergroupid ) and ( PL.DefaultMetadataID = P.DefaultMetadataID );

	-- Get the regular expression for editable items
	select R.EditableRegex, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete
	from mySobek_Editable_Regex R, mySobek_User_Group_Editable_Link L
	where ( L.UserGroupID = @usergroupid ) and ( L.EditableID = R.EditableID );

	-- Get the list of aggregations associated with this user
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsCurator, L.CanEditMetadata, L.CanEditBehaviors, L.CanPerformQc, L.CanUploadFiles, L.CanChangeVisibility, L.CanDelete
	from SobekCM_Item_Aggregation A, mySobek_User_Group_Edit_Aggregation L
	where  ( L.AggregationID = A.AggregationID ) and ( L.UserGroupID = @usergroupid );

	-- Get the list of all user's linked to this user group
	select U.UserID, U.UserName, U.EmailAddress, U.FirstName, U.NickName, U.LastName
	from mySobek_User U, mySobek_User_Group_Link L
	where ( L.UserGroupID = @usergroupid )
	  and ( L.UserID = U.UserID );
END;
GO

-- Ensure the stored procedure exists
IF object_id('SobekCM_Set_User_Setting_Value') IS NULL EXEC ('create procedure dbo.SobekCM_Set_User_Setting_Value as select 1;');
GO

-- Sets a single user setting value, by key.  Adds a new one if this
-- is a new setting key, otherwise updates the existing value.
ALTER PROCEDURE [dbo].[SobekCM_Set_User_Setting_Value]
	@UserID int,
	@Setting_Key varchar(255),
	@Setting_Value varchar(max)
AS
BEGIN

	-- Does this setting exist?
	if ( ( select COUNT(*) from mySobek_User_Settings where Setting_Key=@Setting_Key and UserID=@UserID ) > 0 )
	begin
		-- Just update existing then
		update mySobek_User_Settings set Setting_Value=@Setting_Value where Setting_Key = @Setting_Key and UserID=@UserID;
	end
	else
	begin
		-- insert a new settting key/value pair
		insert into mySobek_User_Settings( UserID, Setting_Key, Setting_Value )
		values ( @UserID, @Setting_Key, @Setting_Value );
	end;	
END;
GO

GRANT EXECUTE ON [dbo].[SobekCM_Set_User_Setting_Value] to sobek_user;
GO


-- Procedure allows an admin to edit permissions flags for this user
ALTER PROCEDURE [dbo].[mySobek_Update_User]
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
            delete from mySobek_User_DefaultMetadata_Link where UserID=@userid;
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
	@autoAssignUsers bit,
	@new_usergroupid int output
AS 
begin
	
	-- Is there a user group id provided
	if ( @usergroupid < 0 )
	begin
		-- Insert as a new user group
		insert into mySobek_User_Group ( GroupName, GroupDescription, Can_Submit_Items, Internal_User, IsSystemAdmin, IsPortalAdmin, Include_Tracking_Standard_Forms, autoAssignUsers  )
		values ( @groupname, @groupdescription, @can_submit_items, @is_internal, @is_system_admin, @is_portal_admin, @include_tracking_standard_forms, @autoAssignUsers );
		
		-- Return the new primary key
		set @new_usergroupid = @@IDENTITY;	
	end
	else
	begin
		-- Update, if it exists
		update mySobek_User_Group
		set GroupName = @groupname, GroupDescription = @groupdescription, Can_Submit_Items = @can_submit_items, Internal_User=@is_internal, IsSystemAdmin=@is_system_admin, IsPortalAdmin=@is_portal_admin, Include_Tracking_Standard_Forms=@include_tracking_standard_forms, autoAssignUsers=@autoAssignUsers
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


-- Ensure the stored procedure exists
IF object_id('mySobek_Get_All_Template_DefaultMetadatas') IS NULL EXEC ('create procedure dbo.mySobek_Get_All_Template_DefaultMetadatas as select 1;');
GO

/****** Object:  StoredProcedure [dbo].[[mySobek_Get_All_Projects_DefaultMetadatas]]    Script Date: 12/20/2013 05:43:35 ******/
-- Get the list of all templates and default metadata sets 
ALTER PROCEDURE [dbo].[mySobek_Get_All_Template_DefaultMetadatas]
AS
BEGIN
	
	select * 
	from mySobek_DefaultMetadata
	order by MetadataCode;

	select * 
	from mySobek_Template
	order by TemplateCode;

END;
GO


GRANT EXECUTE ON [dbo].[mySobek_Get_All_Templates_DefaultMetadatas] TO sobek_user;
GO


/****** Object:  StoredProcedure [dbo].[mySobek_Save_User2]    Script Date: 12/20/2013 05:43:36 ******/
-- Saves a user
ALTER PROCEDURE [dbo].[mySobek_Save_User]
	@userid int,
	@ufid char(8),
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
	@receivestatsemail bit
AS
BEGIN

	if ( @userid < 0 )
	begin

		-- Add this into the user table first
		insert into mySobek_User ( UFID, UserName, [Password], EmailAddress, LastName, FirstName, DateCreated, LastActivity, isActive,  Note_Length, Can_Make_Folders_Public, isTemporary_Password, Can_Submit_Items, NickName, Organization, College, Department, Unit, Default_Rights, sendEmailOnSubmission, UI_Language, Internal_User, OrganizationCode, Receive_Stats_Emails, Include_Tracking_Standard_Forms )
		values ( @ufid, @username, @password, @emailaddress, @lastname, @firstname, getdate(), getDate(), 'true', 1000, 'true', 'false', @cansubmititems, @nickname, @organization, @college, @department, @unit, @rights, @sendemail, @language, 'false', @organization_code, @receivestatsemail, 'false' );

		-- Get the user is
		declare @newuserid int;
		set @newuserid = @@identity;

	end
	else
	begin

		-- Update this user
		update mySobek_User
		set UFID = @ufid, UserName = @username, EmailAddress=@emailAddress,
			Firstname = @firstname, Lastname = @lastname, Can_Submit_Items = @cansubmititems,
			NickName = @nickname, Organization=@organization, College=@college, Department=@department,
			Unit=@unit, Default_Rights=@rights, sendEmailOnSubmission = @sendemail, UI_Language=@language,
			OrganizationCode=@organization_code, Receive_Stats_Emails=@receivestatsemail
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

-- Ensure the stored procedure exists
IF object_id('Tracking_Get_All_Entries_By_User') IS NULL EXEC ('create procedure dbo.Tracking_Get_All_Entries_By_User as select 1;');
GO

--Stored procedure for getting all the tracking workflow entries by user
--entered through the tracking sheet
ALTER PROCEDURE [dbo].[Tracking_Get_All_Entries_By_User]
	@username nvarchar(50)
	
AS
BEGIN

	
		select P.ItemID,P.ProgressID, W.WorkFlowName, W.Start_Event_Desc, W.End_Event_Desc, W.Start_Event_Number, W.End_Event_Number, W.Start_And_End_Event_Number,
		       P.DateStarted, P.DateCompleted, P.RelatedEquipment, P.WorkPerformedBy, P.WorkingFilePath, P.ProgressNote
		from Tracking_Progress P, Tracking_Workflow W
		where P.WorkFlowID = W.WorkFlowID
		and P.WorkPerformedBy = @username;


END;
GO

-- Ensure the stored procedure exists
IF object_id('Tracking_Add_New_Workflow') IS NULL EXEC ('create procedure dbo.Tracking_Add_New_Workflow as select 1;');
GO

ALTER PROCEDURE [dbo].[Tracking_Add_New_Workflow]
	@itemid int,
	@user varchar(50),
	@dateStarted DateTime,
	@dateCompleted DateTime,
	@relatedEquipment varchar(1000),
	@EventNumber int,
	@workflow_entry_id int output
AS
begin transaction
	
	begin
		-- Get the workflow id
		declare @workflowid int
		
		-- Get the existing ID for this workflow
			
	    set @workflowid = coalesce((select WorkFlowID from Tracking_Workflow where Start_Event_Number = @EventNumber or End_Event_Number = @EventNumber ), -1);
	
		-- Add this new workflow entry 
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateStarted, DateCompleted, WorkPerformedBy, RelatedEquipment )
		values ( @itemid, @workflowid, @dateStarted, @dateStarted, @user, @relatedEquipment );
		
		set @workflow_entry_id=@@IDENTITY;
	end
commit transaction
GO


ALTER PROCEDURE [dbo].[Tracking_Add_New_Workflow]
	@itemid int,
	@user varchar(50),
	@dateStarted DateTime,
	@dateCompleted DateTime,
	@relatedEquipment varchar(1000),
	@EventNumber int,
	@StartEventNumber int,
	@EndEventNumber int,
	@Start_End_Event int,
	@workflow_entry_id int output
AS
begin transaction
	
	begin
		-- Get the workflow id
		declare @workflowid int
		
		-- Get the matching ID for this workflow
			
	    set @workflowid = coalesce((select WorkFlowID from Tracking_Workflow where Start_Event_Number = @EventNumber or End_Event_Number = @EventNumber ), -1);
	
		-- Add this new workflow entry 
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateStarted, DateCompleted, WorkPerformedBy, RelatedEquipment, Start_Event_Number, End_Event_Number, Start_And_End_Event_Number)
		values ( @itemid, @workflowid, @dateStarted, @dateCompleted, @user, @relatedEquipment, @StartEventNumber, @EndEventNumber, @Start_End_Event );
		
		set @workflow_entry_id=@@IDENTITY;
	end
commit transaction
GO

-- Ensure the stored procedure exists
IF object_id('Tracking_Update_Workflow') IS NULL EXEC ('create procedure dbo.Tracking_Update_Workflow as select 1;');
GO


ALTER PROCEDURE [dbo].[Tracking_Update_Workflow]
	@itemid int,
	@user varchar(50),
	@dateStarted DateTime,
	@dateCompleted DateTime,
	@relatedEquipment varchar(1000),
	@EventNumber int,
	@StartEventNumber int,
	@EndEventNumber int,
	@workflow_entry_id int 
AS
	
	begin
		-- Get the workflow id
		declare @workflowid int
		
		-- Get the existing ID for this workflow
			
	    set @workflowid = coalesce((select WorkFlowID from Tracking_Workflow where Start_Event_Number = @EventNumber or End_Event_Number = @EventNumber ), -1);
	
		-- Update this workflow entry 
		Update Tracking_Progress
		set DateStarted=@dateStarted, 
		    DateCompleted=@dateCompleted,
		    RelatedEquipment=@relatedEquipment,
		    Start_Event_Number=@StartEventNumber,
		    End_Event_Number = @EndEventNumber,
		    WorkFlowID = @workflowid,
		    WorkPerformedBy = @user
		where @@IDENTITY=@workflow_entry_id AND ItemID=@itemid;
		 

	end
GO


-- Ensure the stored procedure exists
IF object_id('Tracking_Delete_Workflow') IS NULL EXEC ('create procedure dbo.Tracking_Delete_Workflow as select 1;');
GO

/* Stored procedure to delete a workflow entry */
ALTER PROCEDURE [dbo].[Tracking_Delete_Workflow]
	@workflow_entry_id int 
AS	
	begin	
	 
		-- Delete this workflow entry 
		delete from Tracking_Progress
		where ProgressID=@workflow_entry_id;	 

	end;
GO


ALTER PROCEDURE [dbo].[SobekCM_Get_Last_Open_Workflow_By_ItemID]
	@ItemID int,
	@EventNumber int
AS
BEGIN

	-- Get the workflow id
	declare @workflowid int;
	set @workflowid = coalesce((select WorkFlowID from Tracking_Workflow where Start_Event_Number = @EventNumber or End_Event_Number = @EventNumber ), -1);
	
	-- If there is a match continue
	if ( @workflowid > 0 )
	begin
	
		select P.ProgressID, W.WorkFlowName, W.Start_Event_Desc, W.End_Event_Desc, W.Start_Event_Number, W.End_Event_Number, W.Start_And_End_Event_Number,
		       P.DateStarted, P.DateCompleted, P.RelatedEquipment, P.WorkPerformedBy, P.WorkingFilePath, P.ProgressNote
		from Tracking_Progress P, Tracking_Workflow W
		where ItemID = @ItemID
		  and P.WorkFlowID = @workflowid
		  and P.WorkFlowID = W.WorkFlowID
		  and ( DateCompleted is null );
		  
	
	end;
END;
GO


ALTER PROCEDURE [dbo].[Tracking_Update_Workflow]
	@itemid int,
	@user varchar(50),
	@dateStarted DateTime,
	@dateCompleted DateTime,
	@relatedEquipment varchar(1000),
	@EventNumber int,
	@StartEventNumber int,
	@EndEventNumber int,
	@workflow_entry_id int 
AS
	
	begin
		-- Get the workflow id
		declare @workflowid int
		
		-- Get the existing ID for this workflow
			
	    set @workflowid = coalesce((select WorkFlowID from Tracking_Workflow where Start_Event_Number = @EventNumber or End_Event_Number = @EventNumber ), -1);
	
		-- Update this workflow entry 
		Update Tracking_Progress
		set DateStarted=@dateStarted, 
		    DateCompleted=@dateCompleted,
		    RelatedEquipment=@relatedEquipment,
		    Start_Event_Number=@StartEventNumber,
		    End_Event_Number = @EndEventNumber,
		    WorkFlowID = @workflowid,
		    WorkPerformedBy = @user
		where ProgressID=@workflow_entry_id;
		 

	end
GO


ALTER PROCEDURE [dbo].[SobekCM_Get_Last_Open_Workflow_By_ItemID]
	@ItemID int,
	@EventNumber int
AS
BEGIN

	-- Get the workflow id
	declare @workflowid int;
	set @workflowid = coalesce((select WorkFlowID from Tracking_Workflow where Start_Event_Number = @EventNumber or End_Event_Number = @EventNumber ), -1);
	
	-- If there is a match continue
	if ( @workflowid > 0 )
	begin
	
		select P.ItemID,P.ProgressID, W.WorkFlowName, W.Start_Event_Desc, W.End_Event_Desc, W.Start_Event_Number, W.End_Event_Number, W.Start_And_End_Event_Number,
		       P.DateStarted, P.DateCompleted, P.RelatedEquipment, P.WorkPerformedBy, P.WorkingFilePath, P.ProgressNote
		from Tracking_Progress P, Tracking_Workflow W
		where ItemID = @ItemID
		  and P.WorkFlowID = @workflowid
		  and P.WorkFlowID = W.WorkFlowID
		  and ( DateCompleted is null );
		  
	
	end;
END;
GO

GRANT EXECUTE ON Tracking_Add_New_Workflow to sobek_user;
GRANT EXECUTE ON Tracking_Update_Workflow to sobek_user;
GRANT EXECUTE ON Tracking_Delete_Workflow to sobek_user;
GRANT EXECUTE ON Tracking_Get_Scanners_List to sobek_user;
GRANT EXECUTE ON Tracking_Get_All_Entries_By_User to sobek_user;
GO


-- Ensure the stored procedure exists
IF object_id('mySobek_Get_All_Projects_DefaultMetadatas') IS NULL EXEC ('create procedure dbo.mySobek_Get_All_Projects_DefaultMetadatas as select 1;');
GO

/****** Object:  StoredProcedure [dbo].[[mySobek_Get_All_Projects_DefaultMetadatas]]    Script Date: 12/20/2013 05:43:35 ******/
-- Get the list of all templates and default metadata sets 
ALTER PROCEDURE [dbo].[mySobek_Get_All_Projects_DefaultMetadatas]
AS
BEGIN
	
	select * 
	from mySobek_DefaultMetadata
	order by MetadataCode;

	select * 
	from mySobek_Template
	order by TemplateCode;

END;
GO

GRANT EXECUTE ON mySobek_Get_All_Projects_DefaultMetadatas TO sobek_user;
GO

if (( select count(*) from SobekCM_Database_Version ) = 0 )
begin
	insert into SobekCM_Database_Version ( Major_Version, Minor_Version, Release_Phase )
	values ( 4, 1, '' );
end
else
begin
	update SobekCM_Database_Version
	set Major_Version=4, Minor_Version=0, Release_Phase='beta';
end;
GO

