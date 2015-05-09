
-- Esure the SobekCM_Set_Additional_Work_Needed exists
IF object_id('SobekCM_Random_Item') IS NULL EXEC ('create procedure dbo.SobekCM_Set_Additional_Work_Needed as select 1;');
GO

-- Set the flag to have the builder reprocss this item
alter procedure SobekCM_Set_Additional_Work_Needed
	@bibid varchar(10),
	@vid varchar(5) = null
as
begin transaction

	-- Find the GroupID
	declare @groupid int;
	if ( exists ( select 1 from SobekCM_Item_Group where BibID=@bibid ))
	begin
		
		set @groupid = ( select GroupID from SobekCM_Item_Group where BibID=@bibid );

		-- Was a VID provided?
		if ( @vid is null )
		begin
			-- Mark all VIDs of the BibID to be reprocessed
			update SobekCM_Item set AdditionalWorkNeeded='true'
			where GroupID = @groupid;
		end
		else
		begin
			-- update the specific vid
			update SobekCM_Item set AdditionalWorkNeeded='true'
			where GroupID = @groupid
			  and VID = @vid;		
		end;
	end;

commit transaction;
GO

-- Ensure the module exists in the stanard set for adding ALL Images
if ( not exists ( select 1 from SobekCM_Builder_Module where Class='SobekCM.Builder_Library.Modules.Items.AttachImagesAllModule' ))
begin
	-- Ensure the more selective module is there
	if ( exists ( select 1 from SobekCM_Builder_Module where Class='SobekCM.Builder_Library.Modules.Items.AddNewImagesAndViewsModule' and ModuleSetID=3 ))
	begin
		-- Add the new module
		insert into SobekCM_Builder_Module ( ModuleSetID, ModuleDesc, Class, [Enabled], [Order] )
		select 3, 'Attach ALL the images in the resource folder to the item', 'SobekCM.Builder_Library.Modules.Items.AttachImagesAllModule', 'true', [Order]
		from SobekCM_Builder_Module 
		where Class='SobekCM.Builder_Library.Modules.Items.AddNewImagesAndViewsModule' 
		and ModuleSetID=3;

		-- Disable the old, more selective, module
		update SobekCM_Builder_Module
		set [Enabled]='false'
		where Class='SobekCM.Builder_Library.Modules.Items.AddNewImagesAndViewsModule' 
		  and ModuleSetID=3;
	end;
end;
GO
