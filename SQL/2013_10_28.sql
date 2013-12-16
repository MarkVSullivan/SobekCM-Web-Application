



SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SobekCM_Item_Aggregation_Milestones](
	[AggregationMilestoneID] [int] IDENTITY(1,1) NOT NULL,
	[AggregationID] [int] NOT NULL,
	[Milestone] [nvarchar](150) NOT NULL,
	[MilestoneDate] [date] NOT NULL,
	[MilestoneUser] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SobekCM_Item_Aggregation_Milestones] PRIMARY KEY CLUSTERED 
(
	[AggregationMilestoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [SobekCM_Item_Aggregation_Milestones]
ADD CONSTRAINT fk_ItemAggregationMilestones
FOREIGN KEY (AggregationID)
REFERENCES SobekCM_Item_Aggregation(AggregationID);
GO

-- Populate the create dates from the aggregation table
insert into [SobekCM_Item_Aggregation_Milestones] ( AggregationID, Milestone, MilestoneDate, MilestoneUser )
select AggregationID, 'Created', DateAdded, 'Unknown (legacy)'
from SobekCM_Item_Aggregation
where DateAdded > '2000-01-01';
GO

CREATE PROCEDURE SobekCM_Add_Item_Aggregation_Milestone
	@AggregationCode varchar(20),
	@Milestone nvarchar(150),
	@MilestoneUser nvarchar(100)
AS
begin transaction

	-- get the aggregation id
	declare @aggregationid int;
	set @aggregationid = coalesce( (select AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode), -1);
	
	if ( @aggregationid > 0 )
	begin
		
		-- only enter one of these per day
		if ( (select count(*) from [SobekCM_Item_Aggregation_Milestones] where ( AggregationID = @aggregationid ) and ( MilestoneUser=@MilestoneUser ) and ( Milestone=@Milestone) and ( CONVERT(VARCHAR(10), MilestoneDate, 102) = CONVERT(VARCHAR(10), getdate(), 102) )) = 0 )
		begin
			-- Just add this new milestone then
			insert into [SobekCM_Item_Aggregation_Milestones] ( AggregationID, Milestone, MilestoneDate, MilestoneUser )
			values ( @aggregationid, @Milestone, getdate(), @MilestoneUser );
		end;	
	end;

commit transaction;
GO

grant execute on SobekCM_Add_Item_Aggregation_Milestone to sobek_user;
GO

CREATE PROCEDURE SobekCM_Get_Item_Aggregation_Milestone
	@AggregationCode varchar(20)
AS
begin transaction

	-- get the aggregation id
	declare @aggregationid int;
	set @aggregationid = coalesce( (select AggregationID from SobekCM_Item_Aggregation where Code=@AggregationCode), -1);
	
	if ( @aggregationid > 0 )
	begin
		select Milestone, MilestoneDate, MilestoneUser
		from SobekCM_Item_Aggregation_Milestones
		order by MilestoneDate ASC;	
	end;

commit transaction;
GO

grant execute on SobekCM_Get_Item_Aggregation_Milestone to sobek_user;
GO


-- Procedure to delete an item aggregation and unlink it completely.
-- This fails if there are any child aggregations.  This does not really
-- delete the item aggregation, just marks it as DELETED and removed most
-- references.  The statistics are retained.
ALTER PROCEDURE [dbo].[SobekCM_Delete_Item_Aggregation]
	@aggrcode varchar(20),
	@isadmin bit,
	@username varchar(100),
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
		
			-- How many items are still linked to the item aggregation?
			if (( @isadmin = 'false' ) and (( select count(*) from SobekCM_Item_Aggregation_Item_Link where AggregationID=@aggrid ) > 0 ))
			begin
					-- Set the message and code
				set @message = 'Only system admins can delete aggregations with digital resources';
				set @errorcode = 4;
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
				
				-- Add the milestone
				insert into SobekCM_Item_Aggregation_Milestones ( AggregationID, Milestone, MilestoneDate, MilestoneUser )
				values ( @aggrid, 'Deleted', getdate(), @username );
			
			end;
			
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
       
		-- insert the CREATED milestone
		insert into [SobekCM_Item_Aggregation_Milestones] ( AggregationID, Milestone, MilestoneDate, MilestoneUser )
		values ( @newaggregationid, 'Created', getdate(), @username );
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
      
      -- insert the CREATED milestone
	  insert into [SobekCM_Item_Aggregation_Milestones] ( AggregationID, Milestone, MilestoneDate, MilestoneUser )
	  values ( @newaggregationid, 'Configuration edited', getdate(), @username );
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

-- Procedure allows an admin to edit permissions flags for this user
CREATE PROCEDURE [dbo].[mySobek_Update_User]
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

GRANT EXECUTE ON [dbo].[mySobek_Update_User] to sobek_user;
GO