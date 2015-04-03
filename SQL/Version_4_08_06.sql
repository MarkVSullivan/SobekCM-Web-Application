-- THIS GOES FROM 4_08_04 to 4_08_06

-- Return the hierarchies for all (non-institutional) aggregations
-- starting with the 'ALL' aggregation
-- Version 3.05 - Added check to exclude DELETED aggregations
ALTER PROCEDURE [dbo].[SobekCM_Get_Collection_Hierarchies]
as
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the aggregation id for ALL
	declare @aggregationid int;
	select @aggregationid=AggregationID from SobekCM_Item_Aggregation where Code='ALL';

	-- Create the temporary table
	create table #TEMP_CHILDREN_BUILDER (AggregationID int, Code varchar(20), ParentCode varchar(20), Name nvarchar(255), [Type] varchar(50), ShortName nvarchar(100), isActive bit, Hidden bit, Parent_Name nvarchar(255), Parent_ShortName nvarchar(100), HierarchyLevel int );
	
	-- Drive down through the children in the item aggregation hierarchy (first level below)
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, ParentCode='', C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, '', '', -1
	from SobekCM_Item_Aggregation AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( P.AggregationID = @aggregationid )
	  and ( C.Deleted = 'false' )
	  and ( C.Type not like 'Institution%' );
	
	-- Now, try to find any children to this ( second level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -2
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -1 )
	  and ( C.Deleted = 'false' );

	-- Now, try to find any children to this ( third level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -3
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -2 )
	  and ( C.Deleted = 'false' );

	-- Now, try to find any children to this ( fourth level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -4
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -3 ) 
	  and ( C.Deleted = 'false' );
	
	-- Now, try to find any children to this ( fifth level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -5
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -4 )
	  and ( C.Deleted = 'false' );

	-- Return all the COLLECTION children
	select Code, ParentCode, [Name], [ShortName], [Type], HierarchyLevel, isActive, Hidden, Parent_Name, Parent_ShortName
	from #TEMP_CHILDREN_BUILDER
	order by HierarchyLevel DESC, Name;

	-- Clear the temp table
	truncate table #TEMP_CHILDREN_BUILDER;

		-- Drive down through the children in the item aggregation hierarchy (first level below)
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, ParentCode='', C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, '', '', -1
	from SobekCM_Item_Aggregation AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( P.AggregationID = @aggregationid )
	  and ( C.Deleted = 'false' )
	  and ( C.Type like 'Institution%' );
	
	-- Now, try to find any children to this ( second level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -2
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -1 )
	  and ( C.Deleted = 'false' );

	-- Now, try to find any children to this ( third level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -3
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -2 )
	  and ( C.Deleted = 'false' );

	-- Now, try to find any children to this ( fourth level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -4
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -3 ) 
	  and ( C.Deleted = 'false' );
	
	-- Now, try to find any children to this ( fifth level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -5
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -4 )
	  and ( C.Deleted = 'false' );
	  	  
	-- Return all the INSTITUTION children
	select Code, ParentCode, [Name], [ShortName], [Type], HierarchyLevel, isActive, Hidden, Parent_Name, Parent_ShortName
	from #TEMP_CHILDREN_BUILDER
	order by HierarchyLevel DESC, Name;
	
	-- drop the temporary tables
	drop table #TEMP_CHILDREN_BUILDER;

end;
GO


-- Gets all of the information about a single item aggregation
ALTER PROCEDURE [dbo].[SobekCM_Get_Item_Aggregation]
	@code varchar(20)
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Create the temporary table
	create table #TEMP_CHILDREN_BUILDER (AggregationID int, Code varchar(20), ParentCode varchar(20), Name varchar(255), [Type] varchar(50), ShortName varchar(100), isActive bit, Hidden bit, HierarchyLevel int );
	
	-- Get the aggregation id
	declare @aggregationid int
	set @aggregationid = coalesce((select AggregationID from SobekCM_Item_Aggregation AS C where C.Code = @code and Deleted=0), -1 );
	
	-- Return information about this aggregation
	select AggregationID, Code, [Name], coalesce(ShortName,[Name]) AS ShortName, [Type], isActive, Hidden, HasNewItems,
	   ContactEmail, DefaultInterface, [Description], Map_Display, Map_Search, OAI_Flag, OAI_Metadata, DisplayOptions, LastItemAdded, 
	   Can_Browse_Items, Items_Can_Be_Described, External_Link, T.ThematicHeadingID, LanguageVariants, ThemeName
	from SobekCM_Item_Aggregation AS C left outer join
	     SobekCM_Thematic_Heading as T on C.ThematicHeadingID=T.ThematicHeadingID 
	where C.AggregationID = @aggregationid;

	-- Drive down through the children in the item aggregation hierarchy (first level below)
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, HierarchyLevel )
	select C.AggregationID, C.Code, ParentCode=@code, C.[Name], C.[Type], coalesce(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, -1
	from SobekCM_Item_Aggregation AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( P.AggregationID = @aggregationid )
	  and ( C.Deleted = 'false' );
	
	-- Now, try to find any children to this ( second level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], coalesce(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, -2
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
			SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
			SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -1 )
	  and ( C.Deleted = 'false' );

	-- Now, try to find any children to this ( third level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], coalesce(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, -3
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
			SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
			SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -2 )
	  and ( C.Deleted = 'false' );

	-- Now, try to find any children to this ( fourth level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], coalesce(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, -4
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
			SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
			SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -3 )
	  and ( C.Deleted = 'false' );

	-- Return all the children
	select Code, ParentCode, [Name], [ShortName], [Type], HierarchyLevel, isActive, Hidden
	from #TEMP_CHILDREN_BUILDER
	order by HierarchyLevel, Code ASC;
	
	-- drop the temporary tables
	drop table #TEMP_CHILDREN_BUILDER;

	-- Return all the metadata ids for metadata types which have values (if not robot)
	select T.MetadataTypeID, T.canFacetBrowse, T.DisplayTerm, T.SobekCode, T.SolrCode
	into #TEMP_METADATA
	from SobekCM_Metadata_Types T
	where ( LEN(T.SobekCode) > 0 )
	  and exists ( select * from SobekCM_Item_Aggregation_Metadata_Link L where L.AggregationID=@aggregationid and L.MetadataTypeID=T.MetadataTypeID and L.Metadata_Count > 0 );

	if (( select count(*) from #TEMP_METADATA ) > 0 )
	begin
		select * from #TEMP_METADATA order by DisplayTerm ASC;
	end
	else
	begin
		select MetadataTypeID, canFacetBrowse, DisplayTerm, SobekCode, SolrCode
		from SobekCM_Metadata_Types 
		where DefaultAdvancedSearch = 'true'
		order by DisplayTerm ASC;
	end;
		
	-- Return all the metadata ids for metadata types which have values (if not robot)
	select L.MetadataTypeID, T.canFacetBrowse, T.DisplayTerm, T.SobekCode, T.SolrCode
	from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Types T
	where (  L.AggregationID = @aggregationid)
	  and ( L.MetadataTypeID = T.MetadataTypeID )
	  and ( L.Metadata_Count > 0 )
	  and ( LEN(T.SobekCode) > 0 )
	group by L.MetadataTypeID, DisplayTerm, T.canFacetBrowse, T.SobekCode, T.SolrCode
	order by DisplayTerm ASC;
		
	-- Return all the parents (if not robot)
	select Code, [Name], [ShortName], [Type], isActive
	from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy H
	where A.AggregationID = H.ParentID 
	  and H.ChildID = @aggregationid
	  and A.Deleted = 'false';
end;
GO



ALTER TABLE [SobekCM_Item_Aggregation] DROP CONSTRAINT [DF_SobekCM_Item_Aggregation_DefaultInterface];
GO

ALTER TABLE [SobekCM_Item_Aggregation] ADD CONSTRAINT [DF_SobekCM_Item_Aggregation_DefaultInterface] DEFAULT N'' FOR [DefaultInterface];
GO

-- Fix some standard translations (somewhat temporary)
update SobekCM_Metadata_Translation
set English='Citation', French='Citation', Spanish='Cita'
where English='CITATION';
GO

update SobekCM_Metadata_Translation
set English='Page Image', French='Image de Page', Spanish='Imagen de la Página'
where English='PAGE IMAGE';
GO

update SobekCM_Metadata_Translation
set English='Page Text', French='Texte de Page', Spanish='Texto de la Página'
where English='PAGE TEXT';
GO

update SobekCM_Metadata_Translation
set English='Zoomable', French='Zoomable', Spanish='Imagen Ampliable'
where English='ZOOMABLE';
GO

update SobekCM_Metadata_Translation
set English='Features', French='Fonctions', Spanish='Edificios'
where English='FEATURES';
GO

update SobekCM_Metadata_Translation
set English='Streets', French='Rue', Spanish='Calles'
where English='STREETS';
GO

update SobekCM_Metadata_Translation
set English='Thumbnails', French='Miniatures', Spanish='Miniaturas'
where English='THUMBNAILS';
GO

update SobekCM_Metadata_Translation
set English='Downloads', French='Téléchargements', Spanish='Descargas'
where English='DOWNLOADS';
GO

update SobekCM_Metadata_Translation
set English='All Issues', French='Éditions', Spanish='Ediciones'
where English='ALL ISSUES';
GO

update SobekCM_Metadata_Translation
set English='All Volumes', French='Volumes', Spanish='Volumenes'
where English='ALL VOLUMES';
GO

update SobekCM_Metadata_Translation
set English='Related Maps', French='Cartes y Afférentes', Spanish='Mapas Relacionados'
where English='RELATED MAPS';
GO

update SobekCM_Metadata_Translation
set English='Map Search', French='Carte', Spanish='Mapas de Búsqueda'
where English='MAP SEARCH';
GO

update SobekCM_Metadata_Translation
set English='Advanced Search', French='Avancé', Spanish='Búsqueda Avanzada'
where English='ADVANCED SEARCH';
GO

update SobekCM_Metadata_Translation
set English='Text Search', French='Recherche texte', Spanish='Búsqueda de Texto'
where English='TEXT SEARCH';
GO



-- Ensure the stored procedure exists
IF object_id('mySobek_Permissions_Report') IS NULL EXEC ('create procedure dbo.mySobek_Permissions_Report as select 1;');
GO

ALTER PROCEDURE mySobek_Permissions_Report as
begin

	-- Return the top-level permissions (non-aggregation specific)
	select '' as GroupName, U.UserID, UserName, EmailAddress, FirstName, LastName, Nickname, DateCreated, LastActivity, isActive, 
		Can_Submit_Items, 
		case when e.UserID is null then 'false' else 'true' end as Can_Edit_All_Items,
		Internal_User, Can_Delete_All_Items, IsPortalAdmin, IsSystemAdmin, IsHostAdmin
	from mySobek_User as U left outer join
		 mySobek_User_Editable_Link as E on E.UserID = U.UserID and E.EditableID = 1 
	where      ( Can_Submit_Items = 'true' )
			or ( IsSystemAdmin = 'true' )
			or ( IsPortalAdmin = 'true' )
			or ( Can_Delete_All_Items = 'true' )
			or ( IsHostAdmin = 'true' )
			or ( Internal_User = 'true' )
	union
	select G.GroupName, U.UserID, UserName, EmailAddress, FirstName, LastName, Nickname, DateCreated, LastActivity, isActive, 
		G.Can_Submit_Items, 
		case when e.UserGroupID is null then 'false' else 'true' end as Can_Edit_All_Items,
		G.Internal_User, G.Can_Delete_All_Items, G.IsPortalAdmin, G.IsSystemAdmin, 'false'
	from mySobek_User as U inner join
		 mySobek_User_Group_Link as L on U.UserID = L.UserID inner join
		 mySobek_User_Group as G on G.UserGroupID = L.UserGroupID left outer join
		 mySobek_User_Group_Editable_Link as E on E.UserGroupID = G.UserGroupID and E.EditableID = 1 
	where      ( G.Can_Submit_Items = 'true' )
			or ( G.IsSystemAdmin = 'true' )
			or ( G.IsPortalAdmin = 'true' )
			or ( G.Can_Delete_All_Items = 'true' )
			or ( G.Internal_User = 'true' )
	order by LastName ASC, FirstName ASC, GroupName ASC;

	-- Create a temporary table to hold all the user-aggregations links
	create table #tmpAggrPermissions (
		UserID int primary key,
		UserPermissioned varchar(2000),
		GroupPermissioned varchar(2000)
	);

	-- Return the aggregation-specific permissions (at user level unioned with group level)
	insert into #tmpAggrPermissions (UserID)
	select UserID
	from mySobek_User_Edit_Aggregation as P inner join
		 SobekCM_Item_Aggregation A on A.AggregationID=P.AggregationID
	where ( P.CanEditMetadata='true' ) 
	   or ( P.CanEditBehaviors='true' )
	   or ( P.CanPerformQc='true' )
	   or ( P.CanUploadFiles='true' )
	   or ( P.CanChangeVisibility='true' )
	   or ( P.IsCurator='true' )
	   or ( P.IsAdmin='true' )
	group by UserID
	union
	select UserID
	from mySobek_User_Group_Link as L inner join
		 mySobek_User_Group as G on G.UserGroupID = L.UserGroupID inner join
		 mySobek_User_Group_Edit_Aggregation as P on P.UserGroupID = P.UserGroupID inner join
		 SobekCM_Item_Aggregation A on A.AggregationID=P.AggregationID
	where ( P.CanEditMetadata='true' ) 
	   or ( P.CanEditBehaviors='true' )
	   or ( P.CanPerformQc='true' )
	   or ( P.CanUploadFiles='true' )
	   or ( P.CanChangeVisibility='true' )
	   or ( P.IsCurator='true' )
	   or ( P.IsAdmin='true' )
	group by UserID;

	-- Create the cursor to go through the users
	declare UserCursor CURSOR
	LOCAL STATIC FORWARD_ONLY READ_ONLY
	for select UserID from #tmpAggrPermissions;

	-- Open the user cursor
	open UserCursor;

	-- Variable for the cursor loops
	declare @UserID int;
	declare @Code varchar(20);
	declare @UserPermissions varchar(2000);
	declare @GroupPermissions varchar(2000);

	-- Fetch first userid
	fetch next from UserCursor into @UserId;

	-- Step through all users
	While ( @@FETCH_STATUS = 0 )
	begin
		-- Clear the permissions variables
		set @UserPermissions = '';
		set @GroupPermissions = '';

		-- Create the cursor aggregation permissions at the user level	
		declare UserPermissionedCursor CURSOR
		LOCAL STATIC FORWARD_ONLY READ_ONLY
		FOR
		select A.Code
		from mySobek_User_Edit_Aggregation as P inner join
			 SobekCM_Item_Aggregation A on A.AggregationID=P.AggregationID
		where ( P.UserID=@UserID )
		  and (    ( P.CanEditMetadata='true' ) 
		        or ( P.CanEditBehaviors='true' )
		        or ( P.CanPerformQc='true' )
		        or ( P.CanUploadFiles='true' )
		        or ( P.CanChangeVisibility='true' )
		        or ( P.IsCurator='true' )
		        or ( P.IsAdmin='true' ))
		order by A.Code;
	    
		-- Open the user-level aggregation permissions cursor
		open UserPermissionedCursor;

		-- Fetch first user-level aggregation permissions
		fetch next from UserPermissionedCursor into @Code;

		-- Step through each aggregation-level permissioned
		while ( @@FETCH_STATUS = 0 )
		begin
			set @UserPermissions = @UserPermissions + @Code + ', ';

			-- Fetch next user-level aggregation permissions
			fetch next from UserPermissionedCursor into @Code;
		end;

		CLOSE UserPermissionedCursor;
		DEALLOCATE UserPermissionedCursor;

		-- Create the cursor aggregation permissions at the group level	
		declare GroupPermissionedCursor CURSOR
		LOCAL STATIC FORWARD_ONLY READ_ONLY
		FOR
		select A.Code
		from mySobek_User_Group_Link as L inner join
			 mySobek_User_Group as G on G.UserGroupID = L.UserGroupID inner join
			 mySobek_User_Group_Edit_Aggregation as P on P.UserGroupID = P.UserGroupID inner join
			 SobekCM_Item_Aggregation A on A.AggregationID=P.AggregationID
		where ( L.UserID=@UserID )
		  and (    ( P.CanEditMetadata='true' ) 
		        or ( P.CanEditBehaviors='true' )
		        or ( P.CanPerformQc='true' )
		        or ( P.CanUploadFiles='true' )
		        or ( P.CanChangeVisibility='true' )
		        or ( P.IsCurator='true' )
		        or ( P.IsAdmin='true' ))
		group by A.Code
		order by A.Code;
	    
		-- Open the group-level aggregation permissions cursor
		open GroupPermissionedCursor;

		-- Fetch first group-level aggregation permissions
		fetch next from GroupPermissionedCursor into @Code;

		-- Step through each aggregation-level permissioned
		while ( @@FETCH_STATUS = 0 )
		begin
			set @GroupPermissions = @GroupPermissions + @Code + ', ';

			-- Fetch next group-level aggregation permissions
			fetch next from GroupPermissionedCursor into @Code;
		end;

		CLOSE GroupPermissionedCursor;
		DEALLOCATE GroupPermissionedCursor;

		-- Now, update this row
		update #tmpAggrPermissions
		set UserPermissioned=@UserPermissions, GroupPermissioned=@GroupPermissions
		where UserID=@UserId;

		-- Fetch next userid
		fetch next from UserCursor into @UserId;
	end;

	CLOSE UserCursor;
	DEALLOCATE UserCursor;

	-- Return the list of users linked to aggregations, either by group or individually
	select U.UserID, UserName, EmailAddress, FirstName, LastName, Nickname, DateCreated, LastActivity, isActive, T.UserPermissioned, T.GroupPermissioned
	from #tmpAggrPermissions T, mySobek_User U
	where T.UserID=U.UserID
	order by LastName ASC, FirstName ASC;

	-- Get the list of all aggregations that have special links
	with aggregations_permissioned as
	(
		select distinct AggregationID 
		from mySobek_User_Edit_Aggregation
		union
		select distinct AggregationID 
		from mySobek_User_Group_Edit_Aggregation
	)
	select A.Code, A.Name
	from SobekCM_Item_Aggregation A, aggregations_permissioned P
	where A.AggregationID = P.AggregationID
	order by A.Code;

end;
GO

GRANT EXECUTE ON mySobek_Permissions_Report to sobek_user;
GO

-- Ensure the stored procedure exists
IF object_id('mySobek_Permissions_Report_Aggregation') IS NULL EXEC ('create procedure dbo.mySobek_Permissions_Report_Aggregation as select 1;');
GO

ALTER PROCEDURE mySobek_Permissions_Report_Aggregation
	@Code varchar(20)
as
begin

	-- Get the aggregation id
	declare @aggrId int;
	set @aggrId=-1;
	if ( exists ( select 1 from SobekCM_Item_Aggregation where Code=@Code ))
	begin
		set @aggrId = ( select AggregationID from SobekCM_Item_Aggregation where Code=@Code );
	end;

	-- Can the unioned permissions
	select GroupDefined='false', GroupName='', U.UserID, UserName, EmailAddress, FirstName, LastName, Nickname, DateCreated, LastActivity, isActive, 
		   P.CanSelect, P.CanEditItems, P.IsAdmin AS IsAggregationAdmin, P.IsCurator AS IsCollectionManager, P.CanEditMetadata, P.CanEditBehaviors, P.CanPerformQc, P.CanUploadFiles, P.CanChangeVisibility, P.CanDelete
	from mySobek_User U, mySobek_User_Edit_Aggregation P
	where ( U.UserID=P.UserID )
	  and ( P.AggregationID=@aggrId )
	union
	select GroupDefined='true', GroupName=G.GroupName, U.UserID, UserName, EmailAddress, FirstName, LastName, Nickname, DateCreated, LastActivity, isActive, 
		   P.CanSelect, P.CanEditItems, P.IsAdmin AS IsAggregationAdmin, P.IsCurator AS IsCollectionManager, P.CanEditMetadata, P.CanEditBehaviors, P.CanPerformQc, P.CanUploadFiles, P.CanChangeVisibility, P.CanDelete
	from mySobek_User U, mySobek_User_Group_Link L, mySobek_User_Group G, mySobek_User_Group_Edit_Aggregation P
	where ( U.UserID=L.UserID )
	  and ( L.UserGroupID=G.UserGroupID )
	  and ( G.UserGroupID=P.UserGroupID )
	  and ( P.AggregationID=@aggrId )
	order by LastName ASC, FirstName ASC;
end;
go

GRANT EXECUTE ON mySobek_Permissions_Report_Aggregation to sobek_user;
GO


-- Ensure the stored procedure exists
IF object_id('SobekCM_Aggregation_Change_Log') IS NULL EXEC ('create procedure dbo.SobekCM_Aggregation_Change_Log as select 1;');
GO

ALTER PROCEDURE SobekCM_Aggregation_Change_Log
	@Code varchar(20)
as
begin

	-- Get the aggregation id
	declare @aggrId int;
	set @aggrId=-1;
	if ( exists ( select 1 from SobekCM_Item_Aggregation where Code=@Code ))
	begin
		set @aggrId = ( select AggregationID from SobekCM_Item_Aggregation where Code=@Code );
	end;

	-- Get the history
	select Milestone, MilestoneDate, MilestoneUser
	from SobekCM_Item_Aggregation_Milestones 
	where AggregationID = @aggrId
	order by MilestoneDate ASC, AggregationMilestoneID ASC;
end;
go

GRANT EXECUTE ON SobekCM_Aggregation_Change_Log to sobek_user;
GO



-- Stored procedure to save the basic item aggregation information
-- Version 3.05 - Added check to exclude DELETED aggregations
ALTER PROCEDURE [dbo].[SobekCM_Save_Item_Aggregation]
	@aggregationid int,
	@code varchar(20),
	@name varchar(255),
	@shortname varchar(100),
	@description varchar(1000),
	@thematicHeadingId int,
	@type varchar(50),
	@isactive bit,
	@hidden bit,
	@display_options varchar(10),
	@map_search tinyint,
	@map_display tinyint,
	@oai_flag bit,
	@oai_metadata nvarchar(2000),
	@contactemail varchar(255),
	@defaultinterface varchar(10),
	@externallink nvarchar(255),
	@parentid int,
	@username varchar(100),
	@languageVariants varchar(500),
	@newaggregationid int output
AS
begin transaction
   -- If the aggregation id is less than 1 then this is for a new aggregation
   if ((@aggregationid  < 1 ) and (( select COUNT(*) from SobekCM_Item_Aggregation where Code=@code ) = 0 ))
   begin

       print 'into insertion';

       -- Insert a new row
       insert into SobekCM_Item_Aggregation(Code, [Name], Shortname, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, OAI_Metadata, ContactEmail, HasNewItems, DefaultInterface, External_Link, DateAdded, LanguageVariants )
       values(@code, @name, @shortname, @description, @thematicHeadingId, @type, @isActive, @hidden, @display_options, @map_search, @map_display, @oai_flag, @oai_metadata, @contactemail, 'false', @defaultinterface, @externallink, GETDATE(), @languageVariants );

       -- Get the primary key
       set @newaggregationid = @@identity;
       
		-- insert the CREATED milestone
		insert into [SobekCM_Item_Aggregation_Milestones] ( AggregationID, Milestone, MilestoneDate, MilestoneUser )
		values ( @newaggregationid, 'Created', getdate(), @username );
   end
   else
   begin

	  -- Add special code to indicate if this aggregation was undeleted
	  if ( exists ( select 1 from SobekCM_Item_Aggregation where Code=@Code and Deleted='true'))
	  begin
		declare @deletedid int;
		set @deletedid = ( select aggregationid from SobekCM_Item_Aggregation where Code=@Code );

		-- insert the UNDELETED milestone
		insert into [SobekCM_Item_Aggregation_Milestones] ( AggregationID, Milestone, MilestoneDate, MilestoneUser )
		values ( @deletedid, 'Created (undeleted as previously existed)', getdate(), @username );
	  end;

      -- Update the existing row
      update SobekCM_Item_Aggregation
      set  
		Code = @code,
		[Name] = @name,
		ShortName = @shortname,
		[Description] = @description,
		ThematicHeadingID = @thematicHeadingID,
		[Type] = @type,
		isActive = @isactive,
		Hidden = @hidden,
		DisplayOptions = @display_options,
		Map_Search = @map_search,
		Map_Display = @map_display,
		OAI_Flag = @oai_flag,
		OAI_Metadata = @oai_metadata,
		ContactEmail = @contactemail,
		DefaultInterface = @defaultinterface,
		External_Link = @externallink,
		Deleted = 'false',
		DeleteDate = null,
		LanguageVariants = @languageVariants
      where AggregationID = @aggregationid or Code = @code;

	  print 'code is ' + @code;

      -- Set the return value to the existing id
      set @newaggregationid = ( select aggregationid from SobekCM_Item_Aggregation where Code=@Code );

	  print 'new aggregation id is ' + cast(@newaggregationid as varchar(10));
     
   end;

	-- Was a parent id provided
	if ( isnull(@parentid, -1 ) > 0 )
	begin
		-- Now, see if the link to the parent exists
		if (( select count(*) from SobekCM_Item_Aggregation_Hierarchy H where H.ParentID = @parentid and H.ChildID = @newaggregationid ) < 1 )
		begin			
			insert into SobekCM_Item_Aggregation_Hierarchy ( ParentID, ChildID )
			values ( @parentid, @newaggregationid );
		end;
	end;

commit transaction;
GO

/****** Object:  StoredProcedure [dbo].[SobekCM_Add_Item_Aggregation_Milestone]    Script Date: 12/20/2013 05:43:36 ******/
ALTER PROCEDURE [dbo].[SobekCM_Add_Item_Aggregation_Milestone]
	@AggregationCode varchar(20),
	@Milestone nvarchar(150),
	@MilestoneUser nvarchar(max)
AS
begin transaction

	-- get the aggregation id
	declare @aggregationid int;
	set @aggregationid = coalesce( (select AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode), -1);
	
	if ( @aggregationid > 0 )
	begin
		
		-- only enter one of these per day
		if ( (select count(*) from [SobekCM_Item_Aggregation_Milestones] where ( AggregationID = @aggregationid ) and ( MilestoneUser=@MilestoneUser ) and ( Milestone=@Milestone) and ( CONVERT(VARCHAR(10), MilestoneDate, 102) = CONVERT(VARCHAR(10), getdate(), 102) )) = 0 )
		--if ( (select count(*) from [SobekCM_Item_Aggregation_Milestones] where ( AggregationID = @aggregationid ) and ( MilestoneUser=@MilestoneUser ) and ( Milestone=@Milestone) and ( MilestoneDate=getdate())) = 0 )
		begin
			-- Just add this new milestone then
			insert into [SobekCM_Item_Aggregation_Milestones] ( AggregationID, Milestone, MilestoneDate, MilestoneUser )
			values ( @aggregationid, @Milestone, getdate(), @MilestoneUser );
		end;	
	end;

commit transaction;
GO

alter table SobekCM_Item_Aggregation_Milestones
alter column Milestone varchar(max) not null;
GO


if (( select count(*) from SobekCM_Database_Version ) = 0 )
begin
	insert into SobekCM_Database_Version ( Major_Version, Minor_Version, Release_Phase )
	values ( 4, 8, '6' );
end
else
begin
	update SobekCM_Database_Version
	set Major_Version=4, Minor_Version=8, Release_Phase='6';
end;
GO


