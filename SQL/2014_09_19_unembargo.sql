

ALTER TABLE SobekCM_Builder_Incoming_Folders ADD ModuleConfig varchar(max) null;
GO



-- Procedure looks for items to unembargo, and then sends emails out and unembargos them
CREATE PROCEDURE dbo.Admin_Unembargo_Items_Past_Embargo_Date 
	@subject_line varchar(500),
	@email_message varchar(max),
	@send_email bit
AS
BEGIN

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #EmailPrep ( EmailAddress varchar(100) primary key, ItemList nvarchar(max));


	-- Get the items that need to be processed
	select I.ItemID, G.BibID, I.VID, CONVERT(nvarchar(10), T.EmbargoEnd, 102) as EmbargoEnd, substring(M.Title,4,1000) as Title, substring(M.Creator, 4, 1000) as Author
	into #Unembargo_Items
	from SobekCM_Item I, Tracking_Item T, SobekCM_Metadata_Basic_Search_Table M, SobekCM_Item_Group G
	where ( I.ItemID=T.ItemID )
	  and (( I.IP_Restriction_Mask <> 0 ) or ( I.Dark = 'true' ))
	  and ( T.EmbargoEnd < getdate() )
	  and ( M.ItemID = I.ItemID )
	  and ( I.GroupID = G.GroupID );


	-- Variables to hold the iteration's data for the main item cursor
	declare @itemid int;
	declare @bibid varchar(10);
	declare @vid varchar(5);
	declare @embargoend varchar(10);
	declare @title varchar(1000);
	declare @creator varchar(1000);

	-- Variables to hold the inner iteration data
	declare @contactemail varchar(255);

	-- Create the cursor to step through each item to un-embargo
	declare item_cursor cursor for
	select ItemID, BibID, VID, Title, Author, EmbargoEnd
	from #Unembargo_Items
	order by Title;

	open item_cursor;

	-- Get the first item to unembargo
	fetch next from item_cursor 
	into @itemid, @bibid, @vid, @title, @creator, @embargoend;

	-- Loop through them all
	while ( @@FETCH_STATUS = 0 )
	begin
	
		-- Now, step through all aggregations linked to this item
		declare aggregation_cursor cursor for
		select distinct( A.ContactEmail )
		from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L
		where ( L.ItemID= @itemid )
		  and ( L.impliedLink = 'false' )
		  and ( A.AggregationID = L.AggregationID )
		  and ( len(A.ContactEmail) > 0 );

		open aggregation_cursor;

		-- Get the first aggregation email 
		fetch next from aggregation_cursor
		into @contactemail;

		-- Loop through all distinct email addresses for this item 
		while ( @@FETCH_STATUS = 0 )
		begin

			-- Does this email already exist?
			if ( exists ( select 1 from #EmailPrep where EmailAddress=@contactemail ))
			begin
				update #EmailPrep
				set ItemList=ItemList + '<br /><br /><i>' + @title + '</i>, by ' + @creator + ' ( ' + @bibid + ':' + @vid + ' ) - ' + @embargoend
				where EmailAddress=@contactemail;

			end
			else
			begin
				insert into #EmailPrep ( EmailAddress, ItemList )
				values ( @contactemail, '<i>' + @title + '</i>, by ' + @creator + ' ( ' + @bibid + ':' + @vid + ' ) - ' + @embargoend);
			end;

			-- Get the next aggregation email 
			fetch next from aggregation_cursor
			into @contactemail;
		end;
		close aggregation_cursor;
		deallocate aggregation_cursor;
   
		-- Get the next item to un-embargo
		fetch next from item_cursor 
		into @itemid, @bibid, @vid, @title, @creator, @embargoend;
	end;
	close item_cursor;
	deallocate item_cursor;
	
	-- Actually mark the items as unembargoed next
	update SobekCM_Item
	set Dark='false', IP_Restriction_Mask=0, AdditionalWorkNeeded='true'
	where exists ( select * from #Unembargo_Items T where T.ItemID=SobekCM_Item.ItemID );

	-- Also add a workflow progress for this
	insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, DateStarted )
	select ItemID, 34, getdate(), 'Builder Service', 'Automatically unembargoed ( original unembargo date of ' + @embargoend + ' )', getdate()
	from #Unembargo_Items;

	-- Send emails via database email?
	if ( @send_email = 'true' )
	begin

		-- Prepare to send emails
		declare @emailaddress varchar(255);
		declare @itemlist varchar(max);
		declare @emailbody varchar(max);

		-- Now, create the cursor to send the email
		declare email_cursor cursor for
		select EmailAddress, ItemList
		from #EmailPrep;

		open email_cursor;

		-- Get the first email to send
		fetch next from email_cursor 
		into @emailaddress, @itemlist;

		-- Loop through them all
		while ( @@FETCH_STATUS = 0 )
		begin

			-- Set the email body
			set @emailbody = REPLACE(@email_message, '{0}', @itemlist);
	
			-- Send this email
			exec [SobekCM_Send_Email] @emailaddress, @subject_line, @emailbody, 'true', 'false', -1, -1;
		
			-- Get the next email to send
			fetch next from email_cursor 
			into @emailaddress, @itemlist;
		end;
		close email_cursor;
		deallocate email_cursor;
	end;

	-- Return the list of items unembargoed
	select * from #Unembargo_Items;

	-- Return the email information as well
	select * from #EmailPrep;

	-- Drop the temporary tables
	drop table #Unembargo_Items;
	drop table #EmailPrep;

END;
GO

grant execute on dbo.Admin_Unembargo_Items_Past_Embargo_Date to sobek_user;
grant execute on dbo.Admin_Unembargo_Items_Past_Embargo_Date to sobek_builder;
GO




-- Gets the list of all system-wide settings from the database, including the full list of all
-- metadata search fields, possible workflows, and all disposition data
ALTER PROCEDURE [dbo].[SobekCM_Get_Settings]
AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get all the standard SobekCM settings
	select Setting_Key, Setting_Value
	from SobekCM_Settings;

	-- Return all the builder folders
	select IncomingFolderId, NetworkFolder, ErrorFolder, ProcessingFolder, Perform_Checksum_Validation, Archive_TIFF, Archive_All_Files,
		   Allow_Deletes, Allow_Folders_No_Metadata, Allow_Metadata_Updates, FolderName, Can_Move_To_Content_Folder, BibID_Roots_Restrictions,
		   ModuleConfig
	from SobekCM_Builder_Incoming_Folders F;

	-- Return all the metadata search fields
	select MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse
	from SobekCM_Metadata_Types
	order by DisplayTerm;

	-- Return all the possible workflow types
	select WorkFlowID, WorkFlowName, WorkFlowNotes, Start_Event_Number, End_Event_Number, Start_And_End_Event_Number, Start_Event_Desc, End_Event_Desc
	from Tracking_WorkFlow;

	-- Return all the possible disposition options
	select DispositionID, DispositionFuture, DispositionPast, DispositionNotes
	from Tracking_Disposition_Type;

end;
GO


-- Gets a list of items and groups which exist within this instance
CREATE PROCEDURE [dbo].[SobekCM_Item_List]
	@include_private bit
as
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;

	-- Return the item group / item information in one large table
	select G.BibID, I.VID, IP_Restriction_Mask, I.Title, G.[Type], I.Dark
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.GroupID = G.GroupID )
	  and ( G.Deleted = CONVERT(bit,0) )
	  and ( I.Deleted = CONVERT(bit,0) )
	  and ( I.IP_Restriction_Mask >= @lower_mask )
	order by BibID, VID;

end;
GO

GRANT EXECUTE ON SobekCM_Item_List to sobek_builder;
GRANT EXECUTE ON SobekCM_Item_List to sobek_user;
GO



DROP PROCEDURE SobekCM_Item_List_Web;
DROP PROCEDURE SobekCM_Get_Builder_Settings;
GO

/****** Object:  StoredProcedure [dbo].[Tracking_Add_Workflow_By_ItemID]    Script Date: 12/20/2013 05:43:38 ******/
ALTER PROCEDURE [dbo].[Tracking_Add_Workflow_By_ItemID]
	@itemid int,
	@user varchar(50),
	@progressnote varchar(1000),
	@workflow varchar(100),
	@storagelocation varchar(255)
AS
begin transaction
	    
	-- continue if an itemid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Get the workflow id
		declare @workflowid int;
		if ( ( select COUNT(*) from Tracking_WorkFlow where ( WorkFlowName=@workflow)) > 0 )
		begin
			-- Get the existing ID for this workflow
			select @workflowid = workflowid from Tracking_WorkFlow where WorkFlowName=@workflow;
		end
		else
		begin 
			-- Create the workflow for this
			insert into Tracking_WorkFlow ( WorkFlowName, WorkFlowNotes )
			values ( @workflow, 'Added ' + CONVERT(VARCHAR(10), GETDATE(), 101) + ' by ' + @user );
			
			-- Get this ID
			set @workflowid = SCOPE_IDENTITY();
		end;
	
		-- Just add this new progress then
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
		values ( @itemid, @workflowid, GETDATE(), @user, @progressnote, @storagelocation );
	end;
commit transaction;
GO

ALTER PROCEDURE [dbo].[Tracking_Add_Workflow]
	@bibid varchar(10),
	@vid varchar(5),
	@user varchar(50),
	@progressnote varchar(1000),
	@workflow varchar(100),
	@storagelocation varchar(255)
AS
begin transaction

	-- Get the volume id
	declare @itemid int
	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid);
	    
	-- continue if an itemid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Get the workflow id
		declare @workflowid int;
		if ( ( select COUNT(*) from Tracking_WorkFlow where ( WorkFlowName=@workflow)) > 0 )
		begin
			-- Get the existing ID for this workflow
			select @workflowid = workflowid from Tracking_WorkFlow where WorkFlowName=@workflow;
		end
		else
		begin 
			-- Create the workflow for this
			insert into Tracking_WorkFlow ( WorkFlowName, WorkFlowNotes )
			values ( @workflow, 'Added ' + CONVERT(VARCHAR(10), GETDATE(), 101) + ' by ' + @user );
			
			-- Get this ID
			set @workflowid = SCOPE_IDENTITY();
		end;
	
		-- Just add this new progress then
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
		values ( @itemid, @workflowid, GETDATE(), @user, @progressnote, @storagelocation );
	end;
commit transaction;
GO

