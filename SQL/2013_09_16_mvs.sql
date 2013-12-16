


-- Add new columns in the search table
alter table SobekCM_Metadata_Basic_Search_Table add ETD_Degree_Division nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add SortDate bigint not null default(-1);

--  Add the ETD degree division to metadata table
SET IDENTITY_INSERT SobekCM_Metadata_Types ON;
GO

insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 131, 'ETD Degree Division', 'EJ', 'etd degree division', 'ETD Degree Division', 'ETD Degree Division' );
GO

ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( ETD_Degree_Division );
GO

SET IDENTITY_INSERT SobekCM_Metadata_Types OFF;
GO


/****** Object:  StoredProcedure [dbo].[SobekCM_Metadata_Save_Single]    Script Date: 09/16/2013 07:37:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Save a single piece of metadata to the database
-- This is the mapping from metadata name to metadata typeid
ALTER PROCEDURE [dbo].[SobekCM_Metadata_Save_Single]
	@itemid int,
	@metadata_type varchar(100),
	@metadata_value nvarchar(max)
AS
BEGIN

	-- Determine the id first
	declare @metadatatypeid int;
	set @metadatatypeid = (SELECT CASE @metadata_type 
			WHEN 'Title' then 1
			WHEN 'Type' then 2
			WHEN 'Language' then 3
			WHEN 'Creator' then 4
			WHEN 'Publisher' then 5
			WHEN 'Publication Place' then 6
			WHEN 'Subject Keyword' then 7
			WHEN 'Genre' then 8
			WHEN 'Target Audience' then 9
			WHEN 'Spatial Coverage' then 10
			WHEN 'Country' then 11
			WHEN 'State' then 12
			WHEN 'County' then 13
			WHEN 'City' then 14
			WHEN 'Source Institution' then 15
			WHEN 'Holding Location' then 16
			WHEN 'Identifier' then 17
			WHEN 'Notes' then 18
			WHEN 'Other Citation' then 19
			WHEN 'Undefined' then 19
			WHEN 'Tickler' then 20
			WHEN 'Donor' then 21
			WHEN 'Format' then 22
			WHEN 'BibID' then 23
			WHEN 'Publication Date' then 24
			WHEN 'Affiliation' then 25
			WHEN 'Frequency' then 26
			WHEN 'Name as Subject' then 27
			WHEN 'Title as Subject' then 28
			WHEN 'All Subjects' then 29
			WHEN 'Temporal Subject' then 30
			WHEN 'Attribution' then 31
			WHEN 'User Description' then 32
			WHEN 'Temporal Decade' then 33
			WHEN 'MIME Type' then 34
			WHEN 'Full Citation' then 35
			WHEN 'Tracking Box' then 36
			WHEN 'Abstract' then 37
			WHEN 'Edition' then 38
			WHEN 'TOC' then 39
			WHEN 'ZT Kingdom' then 40
			WHEN 'ZT Phylum' then 41
			WHEN 'ZT Class' then 42
			WHEN 'ZT Order' then 43
			WHEN 'ZT Family' then 44
			WHEN 'ZT Genus' then 45
			WHEN 'ZT Species' then 46
			WHEN 'ZT Common Name' then 47
			WHEN 'ZT Scientific Name' then 48
			WHEN 'ZT All Taxonomy' then 49
			WHEN 'Cultural Context' then 50
			WHEN 'Inscription' then 51
			WHEN 'Material' then 52
			WHEN 'Style Period' then 53
			WHEN 'Technique' then 54
			WHEN 'Accession Number' then 55
			WHEN 'ETD Committee' then 56
			WHEN 'ETD Degree' then 57
			WHEN 'ETD Degree Discipline' then 58
			WHEN 'ETD Degree Division' then 131
			WHEN 'ETD Degree Grantor' then 59
			WHEN 'ETD Degree Level' then 60
			WHEN 'Temporal Year' then 61
			WHEN 'Interviewee' then 62
			WHEN 'Interviewer' then 63		
			WHEN 'Publisher.Display' then 116
			WHEN 'Spatial Coverage.Display' then 117
			WHEN 'Measurements' then 118
			WHEN 'Subjects.Display' then 119
			WHEN 'Aggregations' then 120
		ELSE -1
		END );	
		
		
	-- If the metadata type id is -1, then this is a custom metadata field (apparently)
	if ( @metadatatypeid = -1 )
	begin
		-- Does this unique metadata field already exist?
		if ( exists ( select * from SobekCM_Metadata_Types where DisplayTerm=@metadata_type and CustomField='true' ))
		begin
			-- Already exists, so just set the metadata type id
			set @metadatatypeid=( select TOP 1 MetadataTypeID from SobekCM_Metadata_Types where DisplayTerm=@metadata_type and CustomField='true' )
		end
		else
		begin
			-- Insert this as a new custom element, if some open ones exist
			if ( exists ( select * from SobekCM_Metadata_Types where CustomField='true' and DisplayTerm='Undefined' ))
			begin
				-- Get the next open id
				set @metadatatypeid= ( select TOP 1 MetadataTypeID from SobekCM_Metadata_Types where CustomField='true' and DisplayTerm='Undefined' );
			
				-- Now, set the display name and such 
				update SobekCM_Metadata_Types
				set DisplayTerm=@metadata_type, FacetTerm=@metadata_type
				where MetadataTypeID=@metadatatypeid;
			end
			else
			begin
				-- Set to OTHER CITATION since no more custom fields to use
				set @metadatatypeid=19;
			end;
		end;	
	end;
	
	-- Look for a matching existing unique metadata entry
	declare @metadataid bigint;
	set @metadataid = isnull( (select MetadataID 
	                    from SobekCM_Metadata_Unique_Search_Table 
	                    where MetadataTypeID=@metadatatypeid 
	                      and MetadataValueStart=SUBSTRING(@metadata_value, 1, 100)
	                      and MetadataValue=@metadata_value ),-1);
	
	-- If missing, add this unique data 
	if ( @metadataid < 0 )
	begin
		-- Insert this new row
		insert into SobekCM_Metadata_Unique_Search_Table ( MetadataValue, MetadataTypeID, MetadataValueStart )
		values ( @metadata_value, @metadatatypeid, SUBSTRING(@metadata_value, 1, 100) );
	
		-- Save the new primary key for this metadata
		set @metadataid = @@IDENTITY;
	end;
	
	-- Insert the link between the metadata value and the item id
	if ( ( select COUNT(*) from SobekCM_Metadata_Unique_Link where ItemID=@itemid and MetadataID=@metadataid ) = 0 )
	begin
		insert into SobekCM_Metadata_Unique_Link ( ItemID, MetadataID )
		values ( @itemid, @metadataid );
	end;

END;
GO