
ALTER TABLE SobekCM_Item_Aggregation ADD DeleteDate date null;
GO

ALTER TABLE SobekCM_Settings ALTER COLUMN Setting_Key varchar(255) not null;
GO

-- Delete a system-wide setting
CREATE PROCEDURE dbo.SobekCM_Delete_Setting
	@Setting_Key varchar(255)
AS
BEGIN
	delete from SobekCM_Settings where Setting_Key = @Setting_Key;
END;
GO

GRANT EXECUTE ON dbo.SobekCM_Delete_Setting TO sobek_user;
GRANT EXECUTE ON dbo.SobekCM_Delete_Setting TO sobek_itemeditor;
GO

-- Sets a single system-wide setting value, by key.  Adds a new one if this
-- is a new setting key, otherwise updates the existing value.
ALTER PROCEDURE [dbo].[SobekCM_Set_Setting_Value]
	@Setting_Key varchar(255),
	@Setting_Value varchar(max)
AS
BEGIN

	-- Does this setting exist?
	if ( ( select COUNT(*) from SobekCM_Settings where Setting_Key = @Setting_Key ) > 0 )
	begin
		-- Just update existing then
		update SobekCM_Settings set Setting_Value=@Setting_Value where Setting_Key = @Setting_Key;
	end
	else
	begin
		-- insert a new settting key/value pair
		insert into SobekCM_Settings( Setting_Key, Setting_Value )
		values ( @Setting_Key, @Setting_Value );
	end;	
END;
GO

-- Procedure to delete an item aggregation and unlink it completely.
-- This fails if there are any child aggregations.  This does not really
-- delete the item aggregation, just marks it as DELETED and removed most
-- references.  The statistics are retained.
ALTER PROCEDURE [dbo].[SobekCM_Delete_Item_Aggregation]
	@aggrcode varchar(20),
	@message varchar(1000) output,
	@errorcode int output
AS
BEGIN TRANSACTION
	-- Do not delete 'ALL'
	if ( @aggrcode = 'ALL' )
	begin
		-- Set the message and code
		set @message = 'Cannot delete the ALL aggregation.';
		set @errorcode = 3;
		return;	
	end;
	
	-- Only continue if the web skin code exists
	if (( select count(*) from SobekCM_Item_Aggregation where Code = @aggrcode ) > 0 )
	begin	
	
		-- Get the web skin code
		declare @aggrid int;
		select @aggrid=AggregationID from SobekCM_Item_Aggregation where Code = @aggrcode;
		
		-- Are there any children aggregations to this?
		if (( select COUNT(*) from SobekCM_Item_Aggregation_Hierarchy where ParentID=@aggrid ) > 0 )
		begin
			-- Set the message and code
			set @message = 'Item aggregation still has child aggregations';
			set @errorcode = 2;
		
		end
		else
		begin	
		
			-- Set the message and error code initially
			set @message = 'Item aggregation removed';
			set @errorcode = 0;
		
			-- Delete the aggregations to users group links
			delete from mySobek_User_Group_Edit_Aggregation
			where AggregationID = @aggrid;
			
			-- Delete the aggregations to users links
			delete from mySobek_User_Edit_Aggregation
			where AggregationID = @aggrid;
			
			-- Delete links to any items
			--delete from SobekCM_Item_Aggregation_Item_Link
			--where AggregationID = @aggrid;
			
			-- Delete links to any metadata that exist
			delete from SobekCM_Item_Aggregation_Metadata_Link 
			where AggregationID = @aggrid;
			
			-- Delete from the item aggregation aliases
			delete from SobekCM_Item_Aggregation_Alias
			where AggregationID = @aggrid;
			
			-- Delete the links to portals
			delete from SobekCM_Portal_Item_Aggregation_Link
			where AggregationID = @aggrid;
	
			-- Set the deleted flag
			update SobekCM_Item_Aggregation
			set Deleted = 'true', DeleteDate=getdate()
			where AggregationID = @aggrid;
			
		end;
	end
	else
	begin
		-- Since there was no match, set an error code and message
		set @message = 'No matching item aggregation found';
		set @errorcode = 1;
	end;
COMMIT TRANSACTION;
GO


-- Procedure to delete a web skin, and unlink any items or web portals which
-- were linked to this web skin
ALTER PROCEDURE [dbo].[SobekCM_Delete_Web_Skin]
	@webskincode varchar(20),
	@force_delete bit,
	@links int output
AS
BEGIN

	-- set default links return value
	set @links = 0;
	
	-- Only continue if the web skin code exists
	if (( select count(*) from SobekCM_Web_Skin where WebSkinCode = @webskincode ) > 0 )
	begin	
	
		-- Get the web skin id, from the code
		declare @webskinid int;
		select @webskinid=WebSkinID from SobekCM_Web_Skin where WebSkinCode=@webskincode;	
	
		-- Should this force delete?
		if ( @force_delete = 'true' )
		begin	
		
			-- Delete the web skins to item group links
			delete from SobekCM_Item_Group_Web_Skin_Link 
			where WebSkinID=@webskinid;
			
			-- Delete the web skin links to URL portals
			delete from SobekCM_Portal_Web_Skin_Link 
			where WebSkinID=@webskinid;
			
			-- Remove any links to the item aggregation
			update SobekCM_Item_Aggregation
			set DefaultInterface = '' 
			where DefaultInterface = @webskincode;
			
			-- Delete the web skins themselves
			delete from SobekCM_Web_Skin
			where WebSkinID=@webskinid;		
		end
		else
		begin
			if ((( select count(*) from SobekCM_Item_Group_Web_Skin_Link where WebSkinID=@webskinid ) > 0 ) or
			    (( select count(*) from SobekCM_Portal_Web_Skin_Link where WebSkinID=@webskinid ) > 0 ) or
			    (( select count(*) from SobekCM_Item_Aggregation where DefaultInterface=@webskincode ) > 0 ))
			begin
				set @links = 1;
			end
			else
			begin
				-- Delete the web skins themselves, since no links found
				delete from SobekCM_Web_Skin
				where WebSkinID=@webskinid;					
			end;			
		end;
	end;
END;
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
	@newaggregationid int output
AS
begin transaction
   -- If the aggregation id is less than 1 then this is for a new aggregation
   if ((@aggregationid  < 1 ) and (( select COUNT(*) from SobekCM_Item_Aggregation where Code=@code ) = 0 ))
   begin
       -- Insert a new row
       insert into SobekCM_Item_Aggregation(Code, [Name], Shortname, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, OAI_Metadata, ContactEmail, HasNewItems, DefaultInterface, External_Link, DateAdded )
       values(@code, @name, @shortname, @description, @thematicHeadingId, @type, @isActive, @hidden, @display_options, @map_search, @map_display, @oai_flag, @oai_metadata, @contactemail, 'false', @defaultinterface, @externallink, GETDATE() );

       -- Get the primary key
       set @newaggregationid = @@identity;
   end
   else
   begin
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
		DeleteDate = null
      where AggregationID = @aggregationid or Code = @code;

      -- Set the return value to the existing id
      set @newaggregationid = ( select @aggregationid from SobekCM_Item_Aggregation where Code=@Code );
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