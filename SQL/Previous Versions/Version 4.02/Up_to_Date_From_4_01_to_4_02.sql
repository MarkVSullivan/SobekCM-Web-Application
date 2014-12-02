/** Update SobekCM from 4.01 to 4.02 ***/


CREATE TABLE [dbo].[SobekCM_Item_Viewer_Priority](
	[ItemViewPriorityID] [int] IDENTITY(1,1) NOT NULL,
	[ViewType] [nvarchar](255) NOT NULL,
	[Priority] [int] NOT NULL,
 CONSTRAINT [PK_SobekCM_Item_View_Priority] PRIMARY KEY CLUSTERED 
(
	[ItemViewPriorityID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('DATASET_CODEBOOK',1);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('DATASET_REPORTS',2);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('DATASET_VIEWDATA',3);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('EAD_CONTAINER_LIST',4);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('EAD_DESCRIPTION',5);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('YOUTUBE_VIDEO',6);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('EMBEDDED_VIDEO',7);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('FLASH',8);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('JPEG',9);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('JPEG_TEXT_TWO_UP',10);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('JPEG2000',11);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('PDF',12);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('DOWNLOADS',13);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('TEXT',14);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('CITATION',15);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('FEATURES',16);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('GOOGLE_MAP',17);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('HTML',18);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('PAGE_TURNER',19);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('RELATED_IMAGES',20);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('SEARCH',21);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('SIMPLE_HTML_LINK',22);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('STREETS',23);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('TEI',24);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('TOC',25);
insert into SObekCM_Item_Viewer_Priority(ViewType, [Priority]) values ('ALL_VOLUMES',26);
GO

CREATE PROCEDURE dbo.SobekCM_Get_Viewer_Priority
AS
BEGIN
	select * 
	from SobekCM_Item_Viewer_Priority
	order by [Priority] ASC;
END;
GO

GRANT EXECUTE on dbo.SobekCM_Get_Viewer_Priority to sobek_user;
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
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsCurator, L.CanEditMetadata, L.CanEditBehaviors, L.CanPerformQc, L.CanUploadFiles, L.CanChangeVisibility, L.CanDelete, L.IsAdmin
	from SobekCM_Item_Aggregation A, mySobek_User_Group_Edit_Aggregation L
	where  ( L.AggregationID = A.AggregationID ) and ( L.UserGroupID = @usergroupid );

	-- Get the list of all user's linked to this user group
	select U.UserID, U.UserName, U.EmailAddress, U.FirstName, U.NickName, U.LastName
	from mySobek_User U, mySobek_User_Group_Link L
	where ( L.UserGroupID = @usergroupid )
	  and ( L.UserID = U.UserID );
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


-- Procedure to add links between a user group and item aggregations
-- NOTE: The OnHomePage values are NOT used, but are included to keep this
--       signature the same as the single user aggregation link procedure
--       reducing overhead for maintenance
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
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, CanEditItems, IsAdmin )
		values ( @UserGroupID, @Aggregation1_Id, @canSelect1, @canEditMetadata1, @canEditBehaviors1, @canPerformQc1, @canUploadFiles1, @canChangeVisibility1, @canDelete1, @isCurator1, @canEditMetadata1, @isAdmin1 );
	end;
	
	-- Add the second aggregation
	if (( len(@AggregationCode2) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation2_Id int;
		select @Aggregation2_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode2;

		-- Add this one
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, CanEditItems, IsAdmin )
		values ( @UserGroupID, @Aggregation2_Id, @canSelect2, @canEditMetadata2, @canEditBehaviors2, @canPerformQc2, @canUploadFiles2, @canChangeVisibility2, @canDelete2, @isCurator2, @canEditMetadata2, @isAdmin2 );
	end;
	
	-- Add the third aggregation
	if (( len(@AggregationCode3) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation3_Id int;
		select @Aggregation3_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode3;

		-- Add this one
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, CanEditItems, IsAdmin )
		values ( @UserGroupID, @Aggregation3_Id, @canSelect3, @canEditMetadata3, @canEditBehaviors3, @canPerformQc3, @canUploadFiles3, @canChangeVisibility3, @canDelete3, @isCurator3, @canEditMetadata3, @isAdmin3 );
	end;
END;
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

		-- Is this user already linked to the aggreagtion?
		if (( select count(*) from mySobek_User_Edit_Aggregation where UserID=@UserID and AggregationID=@Aggregation1_Id ) = 0 )
		begin
			-- Add this one
			insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, OnHomePage, IsAdmin, CanEditItems )
			values ( @UserID, @Aggregation1_Id, @canSelect1, @canEditMetadata1, @canEditBehaviors1, @canPerformQc1, @canUploadFiles1, @canChangeVisibility1, @canDelete1, @isCurator1, @onHomePage1, @isAdmin1, @canEditMetadata1 );
		end
		else
		begin
			-- Update the existing link
			update mySobek_User_Edit_Aggregation
			set CanSelect=@canSelect1, CanEditMetadata=@canEditMetadata1, CanEditBehaviors=@canEditBehaviors1, CanPerformQc=@canPerformQc1, CanUploadFiles=@canUploadFiles1, CanChangeVisibility=@canChangeVisibility1, CanDelete=@canDelete1, IsCurator=@isCurator1, OnHomePage=@onHomePage1, IsAdmin=@isAdmin1, CanEditItems=@canEditMetadata1
			where UserID=@UserID and AggregationID=@Aggregation1_Id;
		end;		
	end;
	
	-- Add the second aggregation
	if (( len(@AggregationCode2) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation2_Id int;
		select @Aggregation2_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode2;

		-- Is this user already linked to the aggreagtion?
		if (( select count(*) from mySobek_User_Edit_Aggregation where UserID=@UserID and AggregationID=@Aggregation2_Id ) = 0 )
		begin
			-- Add this one
			insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, OnHomePage, IsAdmin, CanEditItems )
			values ( @UserID, @Aggregation2_Id, @canSelect2, @canEditMetadata2, @canEditBehaviors2, @canPerformQc2, @canUploadFiles2, @canChangeVisibility2, @canDelete2, @isCurator2, @onHomePage2, @isAdmin2, @canEditMetadata2 );
		end
		else
		begin
			-- Update the existing link
			update mySobek_User_Edit_Aggregation
			set CanSelect=@canSelect2, CanEditMetadata=@canEditMetadata2, CanEditBehaviors=@canEditBehaviors2, CanPerformQc=@canPerformQc2, CanUploadFiles=@canUploadFiles2, CanChangeVisibility=@canChangeVisibility2, CanDelete=@canDelete2, IsCurator=@isCurator2, OnHomePage=@onHomePage2, IsAdmin=@isAdmin2, CanEditItems=@canEditMetadata2
			where UserID=@UserID and AggregationID=@Aggregation2_Id;
		end;	
	end;

	-- Add the third aggregation
	if (( len(@AggregationCode3) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@AggregationCode3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @Aggregation3_Id int;
		select @Aggregation3_Id = AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode3;

		-- Is this user already linked to the aggreagtion?
		if (( select count(*) from mySobek_User_Edit_Aggregation where UserID=@UserID and AggregationID=@Aggregation3_Id ) = 0 )
		begin
			-- Add this one
			insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, OnHomePage, IsAdmin, CanEditItems )
			values ( @UserID, @Aggregation3_Id, @canSelect3, @canEditMetadata3, @canEditBehaviors3, @canPerformQc3, @canUploadFiles3, @canChangeVisibility3, @canDelete3, @isCurator3, @onHomePage3, @isAdmin3, @canEditMetadata3 );
		end
		else
		begin
			-- Update the existing link
			update mySobek_User_Edit_Aggregation
			set CanSelect=@canSelect3, CanEditMetadata=@canEditMetadata3, CanEditBehaviors=@canEditBehaviors3, CanPerformQc=@canPerformQc3, CanUploadFiles=@canUploadFiles3, CanChangeVisibility=@canChangeVisibility3, CanDelete=@canDelete3, IsCurator=@isCurator3, OnHomePage=@onHomePage3, IsAdmin=@isAdmin3, CanEditItems=@canEditMetadata3
			where UserID=@UserID and AggregationID=@Aggregation3_Id;
		end;	
	end;
END;
GO


ALTER PROCEDURE [dbo].[mySobek_Link_User_To_User_Group]
	@userid int,
	@usergroupid int
AS
begin

	if (( select COUNT(*) from mySobek_User_Group_Link where UserID=@userid and UserGroupID = @usergroupid ) = 0 )
	begin
	
		insert into mySobek_User_Group_Link ( UserGroupID, UserID )
		values ( @usergroupid, @userid );
	
	end;

end;
GO

if (( select count(*) from SobekCM_Database_Version ) = 0 )
begin
	insert into SobekCM_Database_Version ( Major_Version, Minor_Version, Release_Phase )
	values ( 4, 2, '' );
end
else
begin
	update SobekCM_Database_Version
	set Major_Version=4, Minor_Version=2, Release_Phase='';
end;
GO