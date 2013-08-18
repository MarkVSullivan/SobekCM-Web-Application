-- RENAME CONSTRAINT ON SobekCM_Item_Group from PK_SobekCM_Bib --> PK_SobekCM_Item_Group
-- RENAME CONSTRAINT ON SobekCM_Item_Group_Statistics from PK_SobekCM_Item_Group_Stats --> PK_SobekCM_Item_Group_Statistics

-- AM I STILL USING THE MIME_TYPES TABLE?

-- DO NOT INDEX SOBEKCM_SOURCE_LINE OR TEMPTABLE NEXT TIME  (Huh?)

-- How is the SobekCM_External_Record_Type table used?  Has ALEPH, LTUF, and LTQF pre-populated

-- Need to rename '[mySobek_Update_UFDC_User]' procedure



ALTER TABLE SobekCM_Item_Aggregation add [Deleted] [bit] NOT NULL DEFAULT('false');
GO

ALTER TABLE SobekCM_Web_Skin ALTER COLUMN BaseWebSkin varchar(20) null;
GO

ALTER TABLE [dbo].[Tivoli_File_Log] ADD [SHA1_Checksum] [nvarchar](250) NOT NULL DEFAULT('');
GO

ALTER TABLE [dbo].[Tivoli_File_Log] ADD [Original_FileName] [varchar](100) NOT NULL DEFAULT('');
GO

ALTER TABLE [dbo].[Tivoli_File_Request] ADD [RequestFailed] [bit] NOT NULL DEFAULT('false');
GO

ALTER TABLE [dbo].[Tivoli_File_Request] ADD [ReplyEmailSubject] [nvarchar](250) NULL;
GO

ALTER TABLE SobekCM_Database_Version ADD Release_Phase varchar(10) null default('');
GO
 
ALTER TABLE mySobek_User ADD qcProfile nvarchar(50) not null default('');
GO
 
ALTER TABLE SobekCM_Item ADD metadataProfile nvarchar(50) not null default('');
GO
 
ALTER TABLE mySobek_User_Group ADD autoAssignUsers bit not null default('false');
GO


CREATE TABLE [dbo].[Tracking_Item](
	[ItemID] [int] NOT NULL,
	[Original_AccessCode] [varchar](25) NULL,
	[EmbargoEnd] [date] NULL,
	[Original_EmbargoEnd] [date] NULL,
	[UMI] [varchar](20) NULL,
 CONSTRAINT [PK_Tracking_Item] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Tracking_Item]  WITH CHECK ADD  CONSTRAINT [FK_Tracking_Item_SobekCM_Item] FOREIGN KEY([ItemID])
REFERENCES [dbo].[SobekCM_Item] ([ItemID])
GO

ALTER TABLE [dbo].[Tracking_Item] CHECK CONSTRAINT [FK_Tracking_Item_SobekCM_Item]
GO


-- New TEI item viewer type 
insert into SObekCM_Item_Viewer_Types (ViewType) values ( 'TEI' );
  
-- Add new standard search fields to the search table
alter table SobekCM_Metadata_Basic_Search_Table add Interviewee nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add Interviewer nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add Temporal_Year nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add ETD_Committee nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add ETD_Degree nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add ETD_Degree_Discipline nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add ETD_Degree_Grantor nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add ETD_Degree_Level nvarchar(max) not null default('');
GO

-- Update some metadata types which did not have a Sobek code
update SobekCM_Metadata_Types set SobekCode='DE' where MetadataTypeID=32;
update SobekCM_Metadata_Types set SobekCode='DD' where MetadataTypeID=33;
update SobekCM_Metadata_Types set SobekCode='MI' where MetadataTypeID=34;
update SobekCM_Metadata_Types set SobekCode='FC' where MetadataTypeID=35;
update SobekCM_Metadata_Types set SobekCode='AB' where MetadataTypeID=37;
update SobekCM_Metadata_Types set SobekCode='ED' where MetadataTypeID=38;
update SobekCM_Metadata_Types set SobekCode='TC' where MetadataTypeID=39;
update SobekCM_Metadata_Types set SobekCode='ZK' where MetadataTypeID=40;
update SobekCM_Metadata_Types set SobekCode='ZP' where MetadataTypeID=41;
update SobekCM_Metadata_Types set SobekCode='ZC' where MetadataTypeID=42;
update SobekCM_Metadata_Types set SobekCode='ZO' where MetadataTypeID=43;
update SobekCM_Metadata_Types set SobekCode='ZF' where MetadataTypeID=44;
update SobekCM_Metadata_Types set SobekCode='ZG' where MetadataTypeID=45;
update SobekCM_Metadata_Types set SobekCode='ZS' where MetadataTypeID=46;
update SobekCM_Metadata_Types set SobekCode='ZN' where MetadataTypeID=47;
update SobekCM_Metadata_Types set SobekCode='ZI' where MetadataTypeID=48;
update SobekCM_Metadata_Types set SobekCode='ZA' where MetadataTypeID=49;
GO

-- Make the solr column longer and add a flag for if this is a customizable field
ALTER TABLE SobekCM_Metadata_Types ALTER COLUMN SolrCode varchar(100) null;
ALTER TABLE SobekCM_Metadata_Types ADD CustomField bit not null default('false');
GO

--  Add the last standard metadata types 
SET IDENTITY_INSERT SobekCM_Metadata_Types ON;
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 56, 'ETD Committee', 'EC', 'etd committee', 'ETD Committee', 'ETD Committee' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 57, 'ETD Degree', 'ED', 'etd degree', 'ETD Degree', 'ETD Degree' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 58, 'ETD Degree Discipline', 'EI', 'etd degree discipline', 'ETD Degree Discipline', 'ETD Degree Discipline' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 59, 'ETD Degree Grantor', 'EG', 'etd degree grantor', 'ETD Degree Grantor', 'ETD Degree Grantor' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 60, 'ETD Degree Level', 'EL', 'etd degree level', 'ETD Degree Level', 'ETD Degree Level' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 61, 'Temporal Year', 'DY', 'temporal year', 'Temporal Year', 'Temporal Year' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 62, 'Interviewee', 'OI', 'interviewee', 'Intervewiee', 'Intervewiee' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm) values ( 63, 'Interviewer', 'OV', 'interviewer', 'Intervewer', 'Intervewer' );
GO

-- Now, add the user customizable fields into the search table
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined01 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined02 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined03 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined04 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined05 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined06 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined07 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined08 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined09 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined10 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined11 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined12 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined13 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined14 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined15 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined16 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined17 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined18 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined19 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined20 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined21 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined22 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined23 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined24 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined25 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined26 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined27 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined28 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined29 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined30 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined31 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined32 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined33 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined34 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined35 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined36 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined37 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined38 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined39 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined40 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined41 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined42 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined43 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined44 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined45 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined46 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined47 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined48 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined49 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined50 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined51 nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add UserDefined52 nvarchar(max) not null default('');
GO

-- Insert customizable fields into the metadata types table
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 64, 'UserDefined01', 'UA', 'userdefined01', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 65, 'UserDefined02', 'UB', 'userdefined02', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 66, 'UserDefined03', 'UC', 'userdefined03', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 67, 'UserDefined04', 'UD', 'userdefined04', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 68, 'UserDefined05', 'UE', 'userdefined05', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 69, 'UserDefined06', 'UF', 'userdefined06', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 70, 'UserDefined07', 'UG', 'userdefined07', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 71, 'UserDefined08', 'UH', 'userdefined08', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 72, 'UserDefined09', 'UI', 'userdefined09', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 73, 'UserDefined10', 'UJ', 'userdefined10', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 74, 'UserDefined11', 'UK', 'userdefined11', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 75, 'UserDefined12', 'UL', 'userdefined12', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 76, 'UserDefined13', 'UM', 'userdefined13', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 77, 'UserDefined14', 'UN', 'userdefined14', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 78, 'UserDefined15', 'UO', 'userdefined15', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 79, 'UserDefined16', 'UP', 'userdefined16', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 80, 'UserDefined17', 'UQ', 'userdefined17', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 81, 'UserDefined18', 'UR', 'userdefined18', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 82, 'UserDefined19', 'US', 'userdefined19', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 83, 'UserDefined20', 'UT', 'userdefined20', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 84, 'UserDefined21', 'UU', 'userdefined21', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 85, 'UserDefined22', 'UV', 'userdefined22', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 86, 'UserDefined23', 'UW', 'userdefined23', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 87, 'UserDefined24', 'UX', 'userdefined24', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 88, 'UserDefined25', 'UY', 'userdefined25', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 89, 'UserDefined26', 'UZ', 'userdefined26', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 90, 'UserDefined27', 'VA', 'userdefined27', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 91, 'UserDefined28', 'VB', 'userdefined28', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 92, 'UserDefined29', 'VC', 'userdefined29', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 93, 'UserDefined30', 'VD', 'userdefined30', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 94, 'UserDefined31', 'VE', 'userdefined31', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 95, 'UserDefined32', 'VF', 'userdefined32', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 96, 'UserDefined33', 'VG', 'userdefined33', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 97, 'UserDefined34', 'VH', 'userdefined34', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 98, 'UserDefined35', 'VI', 'userdefined35', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 99, 'UserDefined36', 'VJ', 'userdefined36', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 100, 'UserDefined37', 'VK', 'userdefined37', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 101, 'UserDefined38', 'VL', 'userdefined38', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 102, 'UserDefined39', 'VM', 'userdefined39', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 103, 'UserDefined40', 'VN', 'userdefined40', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 104, 'UserDefined41', 'VO', 'userdefined41', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 105, 'UserDefined42', 'VP', 'userdefined42', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 106, 'UserDefined43', 'VQ', 'userdefined43', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 107, 'UserDefined44', 'VR', 'userdefined44', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 108, 'UserDefined45', 'VS', 'userdefined45', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 109, 'UserDefined46', 'VT', 'userdefined46', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 110, 'UserDefined47', 'VU', 'userdefined47', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 111, 'UserDefined48', 'VV', 'userdefined48', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 112, 'UserDefined49', 'VW', 'userdefined49', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 113, 'UserDefined50', 'VX', 'userdefined50', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 114, 'UserDefined51', 'VY', 'userdefined51', 'Undefined', 'Undefined', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField) values ( 115, 'UserDefined52', 'VZ', 'userdefined52', 'Undefined', 'Undefined', 'true' );
GO

-- Add the new fields to the full text index
ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( Interviewee, Interviewer, Temporal_Year, ETD_Committee, ETD_Degree, ETD_Degree_Discipline, ETD_Degree_Grantor, ETD_Degree_Level );
ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( UserDefined01, UserDefined02, UserDefined03, UserDefined04, UserDefined05, UserDefined06, UserDefined07, UserDefined08, UserDefined09, UserDefined10 );
ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( UserDefined11, UserDefined12, UserDefined13, UserDefined14, UserDefined15, UserDefined16, UserDefined17, UserDefined18, UserDefined19, UserDefined20 );
ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( UserDefined21, UserDefined22, UserDefined23, UserDefined24, UserDefined25, UserDefined26, UserDefined27, UserDefined28, UserDefined29, UserDefined30 );
ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( UserDefined31, UserDefined32, UserDefined33, UserDefined34, UserDefined35, UserDefined36, UserDefined37, UserDefined38, UserDefined39, UserDefined40 );
ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( UserDefined41, UserDefined42, UserDefined43, UserDefined44, UserDefined45, UserDefined46, UserDefined47, UserDefined48, UserDefined49, UserDefined50 );
ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( UserDefined51, UserDefined52 );
GO

/****** Object:  Index [IX_SobekCM_Metadata_Types]    Script Date: 03/03/2013 06:26:33 ******/
CREATE NONCLUSTERED INDEX [IX_SobekCM_Metadata_Types] ON [dbo].[SobekCM_Metadata_Types] 
(
	[MetadataName] ASC,
	[CustomField] ASC
)
INCLUDE ( [MetadataTypeID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE SobekCM_Metadata_Types ADD canFacetBrowse bit default('true') not null;
GO

UPDATE SobekCM_Metadata_Types 
set FacetTerm = Replace(FacetTerm, 'ZT ', 'Taxonomic '), DisplayTerm=Replace(DisplayTerm, 'ZT ', 'Taxonomic ');
GO

UPDATE SobekCM_Metadata_Types
set canFacetBrowse='false'
where MetadataTypeID=18 or MetadataTypeID=19 or MetadataTypeID=17 
  or MetadataTypeID=23 or MetadataTypeID=32 or MetadataTypeID=35
  or MetadataTypeID=37 or MetadataTypeID=39;
GO

SET IDENTITY_INSERT SobekCM_Metadata_Types OFF;
GO


/****** Object:  Table [dbo].[SobekCM_Mime_Types]    Script Date: 03/21/2013 12:04:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SobekCM_Mime_Types](
	[MimeTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Extension] [varchar](20) NOT NULL,
	[MimeType] [varchar](100) NOT NULL,
	[isBlocked] [bit] NOT NULL,
	[shouldForward] [bit] NOT NULL,
 CONSTRAINT [PK_SobekCM_Mime_Types] PRIMARY KEY CLUSTERED 
(
	[MimeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[SobekCM_Mime_Types] ON
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (1, N'.avi', N'video/x-msvideo', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (2, N'.bmp', N'image/bmp', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (3, N'.csv', N'application/octet-stream', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (4, N'.doc', N'application/msword', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (5, N'.docx', N'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (6, N'.dtd', N'text/xml', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (7, N'.fla', N'application/octet-stream', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (8, N'.gif', N'image/gif', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (9, N'.gtar', N'application/x-gtar', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (10, N'.gz', N'application/x-gzip', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (11, N'.htm', N'text/html', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (12, N'.html', N'text/html', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (13, N'.ico', N'image/x-icon', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (14, N'.jpeg', N'image/jpeg', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (15, N'.jpg', N'image/jpeg', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (16, N'.js', N'application/x-javascript', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (17, N'.mov', N'video/quicktime', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (18, N'.movie', N'video/x-sgi-movie', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (19, N'.mp2', N'video/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (20, N'.mp3', N'audio/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (21, N'.mpa', N'video/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (22, N'.mpe', N'video/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (23, N'.mpeg', N'video/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (24, N'.mpg', N'video/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (25, N'.mpp', N'application/vnd.ms-project', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (26, N'.mpv2', N'video/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (27, N'.msi', N'application/octet-stream', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (28, N'.pdf', N'application/pdf', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (29, N'.pgm', N'image/x-portable-graymap', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (30, N'.png', N'image/png', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (31, N'.ppt', N'application/vnd.ms-powerpoint', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (32, N'.pptx', N'application/vnd.openxmlformats-officedocument.presentationml.presentation', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (33, N'.ra', N'audio/x-pn-realaudio', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (34, N'.ram', N'audio/x-pn-realaudio', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (35, N'.rm', N'application/vnd.rn-realmedia', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (36, N'.sgml', N'text/sgml', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (37, N'.swf', N'application/x-shockwave-flash', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (38, N'.tar', N'application/x-tar', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (39, N'.tif', N'image/tiff', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (40, N'.tiff', N'image/tiff', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (41, N'.txt', N'text/plain', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (42, N'.vsd', N'application/vnd.visio', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (43, N'.wav', N'audio/wav', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (44, N'.wm', N'video/x-ms-wm', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (45, N'.wma', N'audio/x-ms-wma', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (46, N'.wmv', N'video/x-ms-wmv', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (47, N'.xls', N'application/vnd.ms-excel', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (48, N'.xlsx', N'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (49, N'.xml', N'text/xml', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (50, N'.xsd', N'text/xml', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (51, N'.xsf', N'text/xml', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (52, N'.xsl', N'text/xml', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (53, N'.xslt', N'text/xml', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (54, N'.zip', N'application/x-zip-compressed', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (55, N'.jp2', N'image/jp2', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (56, N'.ogg', N'application/ogg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (57, N'.mp4', N'video/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (58, N'.ogm', N'application/ogg', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (59, N'.m4a', N'audio/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (60, N'.m4v', N'video/mpeg', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (61, N'.sql', N'text/plain', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (62, N'.mkv', N'video/x-matroksa', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (63, N'.webm', N'video/webm', 0, 1)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (64, N'.mxf', N'application/mxf', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (65, N'.mets', N'text/xml', 0, 0)
INSERT [dbo].[SobekCM_Mime_Types] ([MimeTypeID], [Extension], [MimeType], [isBlocked], [shouldForward]) VALUES (66, N'.archive', N'archive', 1, 0)
SET IDENTITY_INSERT [dbo].[SobekCM_Mime_Types] OFF

-- Add tracking for email sent by a userid
ALTER TABLE SobekCM_Email_Log ADD UserID int null;

/****** Object:  Index [IX_Email_Log]    Script Date: 03/21/2013 12:22:12 ******/
CREATE NONCLUSTERED INDEX [IX_Email_Log] ON [dbo].[SobekCM_Email_Log] 
(
	[Sent_Date] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


-- Set the new database version
UPDATE SobekCM_Database_Version
SET Major_Version = 3, Minor_Version=20, Release_Phase='BETA';

-- UP TO HERE RUN AT UNA (MARK)




-- Add the non-searchable display-only fields to the Metadata types
SET IDENTITY_INSERT SobekCM_Metadata_Types ON;
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 116, 'Publisher.Display', '', '', 'Publisher', 'Publisher', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 117, 'Spatial Coverage.Display', '', '', 'Spatial Coverage', 'Subject: Geographic Area', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 118, 'Measurements', '', '', 'Measurements', 'Measurements', 'false', 'false' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 119, 'Subjects.Display', '', '', 'Subjects', 'Subjects', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 120, 'Aggregations', '', '', 'Aggregations', 'Aggregations', 'false', 'true' );
SET IDENTITY_INSERT SobekCM_Metadata_Types OFF;
GO

-- Add the display only columns to the metadata basic search table as well
alter table SobekCM_Metadata_Basic_Search_Table add [Publisher.Display] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [Spatial_Coverage.Display] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add Measurements nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [Subjects.Display] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [Aggregations] nvarchar(max) not null default('');
GO



-- Add the new learning object metadata fields to the Metadata types
SET IDENTITY_INSERT SobekCM_Metadata_Types ON;
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 121, 'LOM_Aggregation', 'LB', '', 'Aggregation (LOM)', 'Aggregation (LOM)', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 122, 'LOM_Context', 'LC', '', 'Context', 'Context', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 123, 'LOM_Classification', 'LL', '', 'Classification', 'Classification', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 124, 'LOM_Difficulty', 'LD', '', 'Difficulty', 'Difficulty', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 125, 'LOM_Intended_End_User', 'LU', '', 'Intended End User', 'Intended End User', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 126, 'LOM_Interactivity_Level', 'LI', '', 'Interactivity Level', 'Interactivity Level', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 127, 'LOM_Interactivity_Type', 'LJ', '', 'Interactivity Type', 'Interactivity Type', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 128, 'LOM_Status', 'LS', '', 'Status', 'Status', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 129, 'LOM_Requirement', 'LR', '', 'Requirements', 'Requirements', 'false', 'true' );
insert into SobekCM_Metadata_Types ( MetadataTypeID, MetadataName, SobekCode, SolrCode, DisplayTerm, FacetTerm, CustomField, canFacetBrowse) values ( 130, 'LOM_AgeRange', 'LG', '', 'Typical Age Range', 'Typical Age Range', 'false', 'true' );
SET IDENTITY_INSERT SobekCM_Metadata_Types OFF;
GO

-- Add the new learning object metadata fields to the metadata basic search table as well
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Aggregation] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Context] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Classification] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Difficulty] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Intended_End_User] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Interactivity_Level] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Interactivity_Type] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Status] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_Requirement] nvarchar(max) not null default('');
alter table SobekCM_Metadata_Basic_Search_Table add [LOM_AgeRange] nvarchar(max) not null default('');
GO

-- Add the new fields to the full text index
ALTER FULLTEXT INDEX ON SobekCM_Metadata_Basic_Search_Table ADD ( LOM_Aggregation, LOM_Context, LOM_Classification, LOM_Difficulty, LOM_Intended_End_User, LOM_Interactivity_Level, LOM_Interactivity_Type, LOM_Status, LOM_Requirement, LOM_AgeRange );
GO

-- Add tracking for email sent by a userid
ALTER TABLE SobekCM_Email_Log ADD UserID int null;

/****** Object:  Index [IX_Email_Log]    Script Date: 03/21/2013 12:22:12 ******/
CREATE NONCLUSTERED INDEX [IX_Email_Log] ON [dbo].[SobekCM_Email_Log] 
(
	[Sent_Date] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


-- Add generic OTHER relationship
insert into mySobek_User_Item_Link_Relationship ( RelationshipLabel, Include_In_Results) values ( 'Other', 1 );

-- Update the SobekCM_Settings table
alter table SobekCM_Settings alter column Setting_Value varchar(max) not null;

-- Add setting for uploading file downloads
insert into SobekCM_Settings ( Setting_Key, Setting_Value )
values ( 'Upload File Types', '.aif,.aifc,.aiff,.au,.avi,.bz2,.c,.c++,.css,.dbf,.ddl,.doc,.docx,.dtd,.dvi,.flac,.gz,.htm,.html,.java,.jps,.js,.m4p,.mid,.midi,.mp2,.mp3,.mpg,.odp,.ogg,.pdf,.pgm,.ppt,.pptx,.ps,.ra,.ram,.rar,.rm,.rtf,.sgml,.swf,.sxi,.tbz2,.tgz,.wav,.wave,.wma,.wmv,.xls,.xlsx,.xml,.zip' );
GO

-- Add setting for uploading file downloads
insert into SobekCM_Settings ( Setting_Key, Setting_Value )
values ( 'Upload Image Types', '.txt,.tif,.jpg,.jp2,.pro' );
GO

-- Correct issue with the solr code for all fields
update SobekCM_Metadata_Types set SolrCode = 'allfields' where MetadataName='Full Citation';

-- Add the workflow for the new QC app
insert into Tracking_WorkFlow(WorkFlowID, WorkFlowName, WorkFlowNotes ) values ( 45, 'Updated Pages/Divisions', 'Using the online QC tool, updated the page names, divisions, page order, etc..' );

-- Add some additional (mostly blank) settings
insert into SobekCM_Settings values ('Aware JPEG2000 Server','');
insert into SobekCM_Settings values ('Builder Seconds Between Polls', '60');
insert into SobekCM_Settings values ('Convert Office Files to PDF', 'FALSE');
insert into SobekCM_Settings values ('Djatoka JPEG2000 Server', '');
insert into SobekCM_Settings values ('Djatoka Proxy Caching Seconds','');
insert into SobekCM_Settings values ('Include Partners On System Home', 'FALSE');
insert into SobekCM_Settings values ('Include TreeView On System Home', 'TRUE');
insert into SobekCM_Settings values ('JPEG2000 Server Type', '');
insert into SobekCM_Settings values ('Main Builder Input Folder', '');
insert into SobekCM_Settings values ('Privacy Email Address', '');
insert into SobekCM_Settings values ('SobekCM Web Server IP', '');
insert into SobekCM_Settings values ('System Default Language', 'English');
insert into SobekCM_Settings values ('Web Output Caching Minutes', '1');
GO


-- DROP ALL EXISTING STORED PROCEDURES
DECLARE @procName varchar(500);
declare cur cursor
for select [name] from sys.objects where type='p' and is_ms_shipped='false';

open cur;
fetch next from cur into @procName;
while @@fetch_status = 0
begin
	exec ('drop procedure ' + @procName );
	fetch next from cur into @procName;
end;
close cur;
deallocate cur;
GO


/****** Object:  StoredProcedure [dbo].[Tivoli_Complete_File_Request]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure completes a tivoli file retrieval request
CREATE PROCEDURE [dbo].[Tivoli_Complete_File_Request]
	@tivolirequestid int,
	@email_body nvarchar(max),
	@email_subject nvarchar(250),
	@isFailure bit
AS
begin

	-- Update this request to reflect completed
	update Tivoli_File_Request
	set Completed = 'true', CompleteDate = GETDATE(), ReplyEmailSubject=@email_subject, RequestFailed=@isFailure
	where TivoliRequestID = @tivolirequestid;
	
	-- Get the email address
	declare @email varchar(100);
	select @email = EmailAddress from Tivoli_File_Request where TivoliRequestId=@tivolirequestid;
	
	-- Send the email
	EXEC msdb.dbo.sp_send_dbmail
		@profile_name= 'sobek noreply profile',
		@recipients = @email,
		@body = @email_body,
		@subject = @email_subject; 

end
GO
/****** Object:  StoredProcedure [dbo].[Tivoli_Request_File]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure logs a tivoli file retrieval request
CREATE PROCEDURE [dbo].[Tivoli_Request_File]
	@folder varchar(250),
	@filename varchar(100),
	@username varchar(100),
	@emailaddress varchar(100),
	@requestnote nvarchar(1500)
AS
begin
	insert into Tivoli_File_Request( Folder, [FileName], UserName, EmailAddress, RequestNote, RequestDate, Completed )
	values ( @folder, @filename, @username, @emailaddress, @requestnote, GETDATE(), 'false' )
end
GO
/****** Object:  StoredProcedure [dbo].[Tivoli_Outstanding_File_Requests]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure returns all pending tivoli file requests
CREATE PROCEDURE [dbo].[Tivoli_Outstanding_File_Requests]
AS
begin

	select * from Tivoli_File_Request where Completed = 'false'

end
GO
/****** Object:  StoredProcedure [dbo].[Tivoli_Get_File_By_Bib_VID]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to pull the stored files by bib id and vid
CREATE PROCEDURE [dbo].[Tivoli_Get_File_By_Bib_VID]
	@bibid char(10),
	@vid char(5)
AS
BEGIN
	
	select * 
	from Tivoli_File_Log 
	where BibID = @bibid and VID = @vid

END
GO
/****** Object:  StoredProcedure [dbo].[Tivoli_Add_File_Archive_Log]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sullivan, Mark
-- Create date: October 7, 2009
-- Description:	Stores the file archive log
-- =============================================
CREATE PROCEDURE [dbo].[Tivoli_Add_File_Archive_Log]
	@BibID char(10),
	@VID char(5),
	@Folder varchar(250),
	@FileName varchar(100),
	@Size bigint,
	@LastWriteDate datetime,
	@ItemID int,
	@Original_FileName varchar(100),
	@SHA1_Checksum nvarchar(250)
AS
BEGIN
	insert into Tivoli_File_Log ( BibID, VID, Folder, [FileName], [Size], LastWriteDate, ArchiveDate, ItemID, Original_FileName, SHA1_Checksum, ArchiveMonth, ArchiveYear )
	values ( @BibID, @VID, @Folder, @FileName, @Size, @LastWriteDate, getdate(), @ItemID, @Original_FileName, @SHA1_Checksum, DATEPART(month, getdate()), DATEPART(YEAR, GETDATE()))
END
GO
/****** Object:  StoredProcedure [dbo].[TEMP_Get_Group_Info_By_BibID]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- TEMPORARY STORED PROCEDURE
CREATE PROCEDURE [dbo].[TEMP_Get_Group_Info_By_BibID]
	@bibid varchar(10)
AS
BEGIN
	
	select * from SobekCM_Item_Group where BibID = @bibid

END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Update_Item_Group]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to change some basic information about an item group
CREATE PROCEDURE [dbo].[SobekCM_Update_Item_Group]
	@BibID varchar(10),
	@GroupTitle nvarchar(500),
	@SortTitle varchar(500),
	@GroupThumbnail varchar(500),
	@PrimaryIdentifierType nvarchar(50),
	@PrimaryIdentifier nvarchar(100)	
AS
begin

	update SobekCM_Item_Group
	set GroupTitle = @GroupTitle, SortTitle = @SortTitle, GroupThumbnail=@GroupThumbnail,
	    Primary_Identifier_Type=@PrimaryIdentifierType, Primary_Identifier=@PrimaryIdentifier
	where BibID = @BibID;

end;
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Get_All_Possible_Workflows]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Get_All_Possible_Workflows]
AS
begin

	select * from Tracking_WorkFlow

end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Get_All_Possible_Disposition_Types]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Get_All_Possible_Disposition_Types]
AS
begin

	select * from Tracking_Disposition_Type

end
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Manager_GroupID_From_BibID]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns the group id for a single provided bib id
CREATE PROCEDURE [dbo].[SobekCM_Manager_GroupID_From_BibID]
      @bibid varchar(10)
AS
BEGIN
      select GroupID 
      from SobekCM_Item_Group
      where BibID = @bibid;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Manager_Get_Thematic_Headings]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Stored procedure for pulling the list of thematic headings
CREATE PROCEDURE [dbo].[SobekCM_Manager_Get_Thematic_Headings] 
AS
BEGIN
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Return all the thematic heading information
	select * 
	from SobekCM_Thematic_Heading
	order by ThemeOrder;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Log_Email]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Log an email which was sent through a different method.  This does not
-- cause a database mail to be sent, just logs an email which was sent
CREATE PROCEDURE [dbo].[SobekCM_Log_Email] 
	@sender varchar(250),
	@recipients_list varchar(500),
	@subject_line varchar(250),
	@email_body nvarchar(max),
	@html_format bit,
	@contact_us bit,
	@replytoemailid int
AS
begin

	-- Log this email
	insert into SobekCM_Email_Log( Sender, Receipt_List, Subject_Line, Email_Body, Sent_Date, HTML_Format, Contact_Us, ReplyToEmailID )
	values ( @sender, @recipients_list, @subject_line, @email_body, GETDATE(), @html_format, @contact_us, @replytoemailid );
	
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Link_Aggregation_Thematic_Heading]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Link an existing item aggregation to an existing thematic heading, so the
-- item aggregation will appear on the home page under this thematic heading
CREATE PROCEDURE [dbo].[SobekCM_Link_Aggregation_Thematic_Heading]
	@ThematicHeadingID int,
	@Code nvarchar(20)
AS
BEGIN
	-- Update existing row
	update SobekCM_Item_Aggregation
	set ThematicHeadingID=@ThematicHeadingID
	where Code=@Code;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Icon]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Save_Icon]
	@iconid int,
	@icon_name varchar(255),
	@icon_url varchar(255),
	@link varchar(255), 
	@height int,
	@title varchar(255),
	@new_iconid int output
as
begin transaction	

	-- Does an icon with this icon name (code) exists?
    if ((select count(*) from SobekCM_Icon where icon_name = @icon_name) = 0 )
    begin     
		-- None existed, so insert a new one 
		insert into SobekCM_Icon(icon_name,icon_url, link, height, title )
		values(@icon_name, @icon_url, @link, @height, @title )
		select @new_iconid = @@identity
    end
	else
	begin
		-- Update the existing row
		update SobekCM_Icon
		set icon_url = @icon_url, link = @link, height = @height, title = @title
		where icon_name = @icon_name
   		
		-- Return this icon id
		select @new_iconid = IconID
		from SobekCM_Icon
		where icon_name = @icon_name
   end  
  
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Group]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves all the main data about a group of items in UFDC
-- Written by Mark Sullivan (September 2006, Modified October 2011 )
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Group]
	@BibID varchar(10),
	@GroupTitle nvarchar(500),
	@SortTitle varchar(255),
	@Type varchar(50),
	@File_Location varchar(100),
	@OCLC_Number bigint,
	@ALEPH_Number int,
	@Group_Thumbnail varchar(500),
	@Large_Format bit,
	@Track_By_Month bit,
	@Never_Overlay_Record bit,
	@Update_Existing bit,
	@PrimaryIdentifierType nvarchar(50),
	@PrimaryIdentifier nvarchar(100),
	@GroupID int output,
	@New_BibID varchar(10) output,
	@New_Group bit output
AS
begin transaction

	-- Set the return BibID value first
	set @New_BibID = @BibID;
	set @New_Group = 'false';

	-- If this group does not exists (BibID) insert, else update
	if (( select count(*) from SobekCM_Item_Group  where ( BibID = @BibID ))  < 1 )
	begin	
		-- Verify the BibID is a complete bibid, otherwise find the next one
		if ( LEN(@bibid) < 10 )
		begin
			declare @next_bibid_number int;

			-- Find the next bibid number
			select @next_bibid_number = isnull(CAST(REPLACE(MAX(BibID), @bibid, '') as int) + 1,-1)
			from SobekCM_Item_Group
			where BibID like @bibid + '%';
			
			-- If no matches to this BibID, just start at 0000001
			if ( @next_bibid_number < 0 )
			begin
				select @New_BibID = @bibid + RIGHT('00000001', 10-LEN(@bibid));
			end
			else
			begin
				select @New_BibID = @bibid + RIGHT('00000000' + (CAST( @next_bibid_number as varchar(10))), 10-LEN(@bibid));
			end;
		end;
		
		-- Compute the file location if needed
		if ( LEN(@File_Location) = 0 )
		begin
			set @File_Location = SUBSTRING(@New_BibID,1 ,2 ) + '\' + SUBSTRING(@New_BibID,3,2) + '\' + SUBSTRING(@New_BibID,5,2) + '\' + SUBSTRING(@New_BibID,7,2) + '\' + SUBSTRING(@New_BibID,9,2);
		end;
		
		-- Add the values to the main SobekCM_Item table first
		insert into SobekCM_Item_Group ( BibID, GroupTitle, Deleted, [Type], SortTitle, ItemCount, File_Location, GroupCreateDate, OCLC_Number, ALEPH_Number, GroupThumbnail, Track_By_Month, Large_Format, Never_Overlay_Record, Primary_Identifier_Type, Primary_Identifier )
		values ( @New_BibID, @GroupTitle, 0, @Type, @SortTitle, 0, @File_Location, getdate(), @OCLC_Number, @ALEPH_Number, @Group_Thumbnail, @Track_By_Month, @Large_Format, @Never_Overlay_Record, @PrimaryIdentifierType, @PrimaryIdentifier );

		-- Get the item id identifier for this row
		set @GroupID = @@identity;
		set @New_Group = 'true';
	end
	else
	begin

		-- This already existed, so just return the existing group id
		select @GroupID = GroupID
		from SobekCM_Item_Group
		where BibID = @BibID;

		-- If we are supposed to update it, do this
		if ( @Update_Existing = 'true' )
		begin

			update SobekCM_Item_Group
			set GroupTitle=@GroupTitle, [Type]=@Type, SortTitle=@SortTitle, OCLC_Number=@OCLC_Number, ALEPH_Number=@ALEPH_Number, GroupThumbnail=@Group_Thumbnail, Track_By_Month = @Track_By_Month, Large_Format=@Large_Format, Never_Overlay_Record = @Never_Overlay_Record, Primary_Identifier_Type=@PrimaryIdentifierType, Primary_Identifier=@PrimaryIdentifier
			where BibID = @BibID;

		end;
		
		set @New_Group = 'false';
	end;

commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Set_Setting_Value]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Sets a single system-wide setting value, by key.  Adds a new one if this
-- is a new setting key, otherwise updates the existing value.
CREATE PROCEDURE [dbo].[SobekCM_Set_Setting_Value]
	@Setting_Key varchar(100),
	@Setting_Value varchar(255)
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
/****** Object:  StoredProcedure [dbo].[SobekCM_Send_Email]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Sends an email via database mail and additionally logs that the email was sent
CREATE PROCEDURE [dbo].[SobekCM_Send_Email] 
	@recipients_list varchar(250),
	@subject_line varchar(500),
	@email_body nvarchar(max),
	@html_format bit,
	@contact_us bit,
	@replytoemailid int,
	@userid int
AS
begin transaction

	if (( @subject_line <> 'Unlable to perform search - hierarchyObject = null' ) and ( @subject_line <> 'Unknown error occurred' ))
	begin

	if (( @userid < 0 ) or (( select count(*) from SobekCM_Email_Log where UserID = @userid and Sent_Date > DateAdd( DAY, -1, GETDATE())) < 20 ))
	begin
		-- Log this email
		insert into SobekCM_Email_Log( Sender, Receipt_List, Subject_Line, Email_Body, Sent_Date, HTML_Format, Contact_Us, ReplyToEmailId, UserID )
		values ( 'sobekcm noreply profile', @recipients_list, @subject_line, @email_body, GETDATE(), @html_format, @contact_us, @replytoemailid, @userid );
		
		-- Send the email
		if ( @html_format = 'true' )
		begin
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name= 'sobekcm noreply profile',
				@recipients = @recipients_list,
				@body = @email_body,
				@subject = @subject_line,
				@body_format = 'html';
		end
		else
		begin
				EXEC msdb.dbo.sp_send_dbmail
				@profile_name= 'sobekcm noreply profile',
				@recipients = @recipients_list,
				@body = @email_body,
				@subject = @subject_line;
		end;
	end;
	
	end;
	
commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Add_User_Aggregations_Link]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to add links between a user and item aggregations
CREATE PROCEDURE [dbo].[mySobek_Add_User_Aggregations_Link]
	@userid int,
	@aggregationcode1 varchar(20),
	@canselect1 bit,
	@canedit1 bit,
	@iscurator1 bit,
	@onhomepage1 bit,
	@aggregationcode2 varchar(20),
	@canselect2 bit,
	@canedit2 bit,
	@iscurator2 bit,	
	@onhomepage2 bit,
	@aggregationcode3 varchar(20),
	@canselect3 bit,
	@canedit3 bit,
	@iscurator3 bit,
	@onhomepage3 bit
AS
BEGIN
	
	-- Add the first aggregation
	if (( len(@aggregationcode1) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode1 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation1_id int
		select @aggregation1_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode1

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditItems, IsCurator, OnHomePage )
		values ( @userid, @aggregation1_id, @canselect1, @canedit1, @iscurator1, @onhomepage1 )
	end

	-- Add the second aggregation
	if (( len(@aggregationcode2) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation2_id int
		select @aggregation2_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode2

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditItems, IsCurator, OnHomePage )
		values ( @userid, @aggregation2_id, @canselect2, @canedit2, @iscurator2, @onhomepage2 )
	end

	-- Add the third aggregation
	if (( len(@aggregationcode3) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation3_id int
		select @aggregation3_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode3

		-- Add this one
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditItems, IsCurator, OnHomePage )
		values ( @userid, @aggregation3_id, @canselect3, @canedit3, @iscurator3, @onhomepage3 )
	end
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Change_Password]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Change a user's password
CREATE PROCEDURE [dbo].[mySobek_Change_Password]
	@username nvarchar(100),
	@current_password nvarchar(100),
	@new_password nvarchar(100),
	@isTemporaryPassword bit,
	@password_changed bit output
AS
BEGIN
	
	if ( ( select count(*) from mySobek_User where username=@username and [password]=@current_password ) > 0 )
	begin

		update mySobek_User 
		set isTemporary_Password=@isTemporaryPassword, [Password]=@new_password
		where ( UserName = @username ) and ( [Password]=@current_password )

		set @password_changed = 'true';

	end
	else
	begin
		
		set @password_changed = 'false';

	end

END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_All_Users]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the list of all users
CREATE PROCEDURE [dbo].[mySobek_Get_All_Users] AS
BEGIN
	
	select UserID, LastName + ', ' + FirstName AS [Full_Name], UserName, EmailAddress
	from mySobek_User 
	order by Full_Name
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_All_User_Groups]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[mySobek_Get_All_User_Groups]
AS
begin
	select * from mySobek_USer_Group
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_All_Projects_Templates]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of all templates and projects 
CREATE PROCEDURE [dbo].[mySobek_Get_All_Projects_Templates]
AS
BEGIN
	
	select * 
	from mySobek_Project
	order by ProjectCode

	select * 
	from mySobek_Template
	order by TemplateCode

END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Delete_User_Group]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete a user group
CREATE PROCEDURE [dbo].[mySobek_Delete_User_Group]
	@usergroupid int
AS
begin
	delete from mySobek_User_Group
	where UserGroupID = @usergroupid
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_Item_Link_Relationships]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[mySobek_Get_User_Item_Link_Relationships]
AS
begin
	select * from mySobek_User_Item_Link_Relationship
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_New_UFID_User]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Adds a new UFID user
CREATE PROCEDURE [dbo].[mySobek_New_UFID_User]
	@ufid char(8),
	@emailaddress varchar(100),
	@firstname varchar(100),
	@lastname varchar(100)
AS
BEGIN
	
	-- Add this into the user table first
	insert into mySobek_User ( UFID, UserName, EmailAddress, LastName, FirstName, DateCreated, LastActivity, isActive,  Note_Length, Can_Make_Folders_Public, isTemporary_Password, sendEmailOnSubmission, Can_Submit_Items )
	values ( @ufid, @ufid, @emailaddress, @lastname, @firstname, getdate(), getDate(), 'true', 1000, 'true', 'false', 'true', 'true' )

	-- Get the user is
	declare @userid int
	set @userid = @@identity

	-- Add the IR template and IR 'project' as defaults for this user
	

END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Set_Receive_Stats_Email_Flag]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[mySobek_Set_Receive_Stats_Email_Flag]
	@userid int,
	@newflag bit
AS
begin

	update mySobek_User
	set Receive_Stats_Emails=@newflag	

end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Set_Aggregation_Home_Page_Flag]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Set aggregation home page flag 
CREATE PROCEDURE [dbo].[mySobek_Set_Aggregation_Home_Page_Flag]
	@userid int,
	@aggregationid int,
	@onhomepage bit
AS
begin transaction

	-- Check to see if this aggregation is already tied to this user
	if ( ( select count(*) from mySobek_User_Edit_Aggregation where UserID=@userid and AggregationID=@aggregationid ) > 0 )
	begin
		-- update existing link
		update mySobek_User_Edit_Aggregation
		set OnHomePage=@onhomepage
		where UserID =  @userid and AggregationID = @aggregationid

		-- delete any links that have nothing flagged
		delete from mySobek_User_Edit_Aggregation
		where CanSelect='false' and CanEditItems='false' and IsCurator='false' and OnHomePage='false'
	end
	else
	begin
		-- Insert new link with no permissions
		insert into mySobek_User_Edit_Aggregation ( UserID, AggregationID, CanSelect, CanEditItems, IsCurator, OnHomePage )
		values ( @userid, @aggregationid, 'false', 'false', 'false', @onhomepage )
	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Web_Skins]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Stored procedure to get all the web skin information 
CREATE PROCEDURE [dbo].[SobekCM_Get_Web_Skins] AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	select WebSkinCode, OverrideHeaderFooter=isnull(OverrideHeaderFooter,'false'), 
		OverrideBanner=isnull(OverrideBanner, 'false'), BannerLink=isnull(BannerLink,''),
		BaseInterface=isnull(BaseWebSkin,''), Notes=isnull(Notes,''), Build_On_Launch,
		SuppressTopNavigation
	from SobekCM_Web_Skin
	order by WebSkinCode;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Translation]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the translations for the headings used in the SobekCM system
CREATE PROCEDURE [dbo].[SobekCM_Get_Translation] AS
begin
	
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	select * from SobekCM_Metadata_Translation;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Statistics_Dates]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the year/month pairing for which this system appears to have 
-- some usage statistics recorded.  This is for the drop-down select 
-- boxes when viewing the usage statistics online
CREATE PROCEDURE [dbo].[SobekCM_Get_Statistics_Dates]
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get the distinct years and months
	select [Year], [Month]
	from SobekCM_Statistics
	group by [Year], [Month];

END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Settings]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the list of all system-wide settings from the database
CREATE PROCEDURE [dbo].[SobekCM_Get_Settings]
	@include_items bit
AS
begin

	-- Get all the standard SobekCM settings
	select *
	from SobekCM_Settings;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Search_Stop_Words]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of stop words which are skipped during searching.  This is
-- used to ensure stop words are not passed into the database searching
-- mechanism, which will cause it to fail in certain searches
CREATE PROCEDURE [dbo].[SobekCM_Get_Search_Stop_Words]
AS
BEGIN
	-- Return all the stored stop words
	select *
	from SobekCM_Search_Stop_Words;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Email_List]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns the list of emails from the email logging system.  
-- @Include_All_Types - if TRUE, all emails returned, otherwise just the 'Contact Us' emails
CREATE PROCEDURE [dbo].[SobekCM_Get_Email_List]
	@Include_All_Types bit,
	@Top100_Only bit
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Should ALL types of emails be returned, or just the 'Contact Us' emails?
	if ( @Include_All_Types = 'true' )
	begin
		-- Should only the top 100 be returned?
		if ( @Top100_Only = 'true' )
		begin
			-- Return the top 100 emails of any type
			select top 100 EmailID, Sender, Receipt_List, Subject_Line, Sent_Date, SUBSTRING(Email_Body,0,500) as Preview, HTML_Format, Contact_Us, isnull(ReplyToEmailID, -1) as ReplyToEmailID 
			from SobekCM_Email_Log
			order by Sent_Date DESC;
		end
		else
		begin
			-- Return all emails of any type
			select EmailID, Sender, Receipt_List, Subject_Line, Sent_Date, SUBSTRING(Email_Body,0,500) as Preview, HTML_Format, Contact_Us, isnull(ReplyToEmailID, -1) as ReplyToEmailID 
			from SobekCM_Email_Log
			order by Sent_Date DESC;
		end;
	end
	else
	begin
		-- Should only the top 100 be returned?
		if ( @Top100_Only = 'true' )
		begin
			-- Return the top 100 'contact us' emails
			select top 100 EmailID, Sender, Receipt_List, Subject_Line, Sent_Date, SUBSTRING(Email_Body,0,500) as Preview, HTML_Format, Contact_Us, isnull(ReplyToEmailID, -1) as ReplyToEmailID 
			from SobekCM_Email_Log
			where Contact_Us = 'true'
			order by Sent_Date DESC;
		end
		else
		begin
			-- Return all 'contact us' emails
			select EmailID, Sender, Receipt_List, Subject_Line, Sent_Date, SUBSTRING(Email_Body,0,500) as Preview, HTML_Format, Contact_Us, isnull(ReplyToEmailID, -1) as ReplyToEmailID 
			from SobekCM_Email_Log
			where Contact_Us = 'true'
			order by Sent_Date DESC;
		end;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Email]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets an email from the email logging system, by the primary key for the Email.
-- This also includes any responses to this original email
CREATE PROCEDURE [dbo].[SobekCM_Get_Email]
	@EmailID int
AS
begin
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get the original email
	select * from SobekCM_Email_Log where EmailID=@EmailID;
	
	-- Get any responses to this email	
	select * from SobekCM_Email_Log where ReplyToEmailID=@EmailID;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Codes]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the lists of all item aggregation codes
CREATE PROCEDURE [dbo].[SobekCM_Get_Codes]
AS
begin
	
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get the first aggregations
	SELECT Code, [Type], Name, ShortName=isnull(ShortName, Name), isActive, Hidden, AggregationID, [Description]=isnull([Description],''), ThematicHeadingID=isnull(ThematicHeadingID, -1 ), External_URL=ISNULL(External_Link,''), DateAdded
	FROM SobekCM_Item_Aggregation AS P
	WHERE Deleted = 'false';	
	
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Edit_IP_Range]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Edit basic information about an ip restriction range, or add a new range
CREATE PROCEDURE [dbo].[SobekCM_Edit_IP_Range]
	@rangeid int,
	@title nvarchar(150),
	@notes nvarchar(2000),
	@not_valid_statement nvarchar(max)
AS
begin

	-- Does this range id exist?
	if ( @rangeid in (select IP_RangeID from SobekCM_IP_Restriction_Range ))
	begin
		-- Range id existed, so update the existing IP range
		update SobekCM_IP_Restriction_Range
		set Title=@title, Notes=@notes, Not_Valid_Statement=@not_valid_statement
		where IP_RangeID = @rangeid;
	end
	else
	begin
		-- New range id, so add this IP range
		insert into SobekCM_IP_Restriction_Range ( Title, Notes, Not_Valid_Statement )
		values ( @title, @notes, @not_valid_statement );	
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Build_Error_Logs]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the build errors between two dates.  Due to the date comparison, the
-- second date should really be midnight on the NEXT day.  So, if you want all
-- the build errors between 1/1/2000 and 1/2/2000, the datetimes you should use
-- would be '1/1/2000' and '1/3/2000'.
CREATE PROCEDURE [dbo].[SobekCM_Get_Build_Error_Logs]
	@firstdate datetime,
	@seconddate datetime
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Return the errors within the range, which were not cleared
	select BibID, VID, METS_Type=isnull(METS_Type,''), ErrorDescription=isnull(ErrorDescription,''), [Date]
	from SobekCM_Item_Error_Log
	where ( len(isnull(ClearedBy,'')) = 0 ) 
	  and ( ClearedDate is null )
      and ( [DATE] >= @firstdate )
	  and ( [Date] < @seconddate )
	order by [Date] DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Edit_Thematic_Heading]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Edits the order and name for an existing themathic heading, or adds a new heading
-- if the provided thematic heading id is not valid
CREATE PROCEDURE [dbo].[SobekCM_Edit_Thematic_Heading]
	@ThematicHeadingID int,
	@ThemeOrder int,
	@ThemeName nvarchar(100),
	@NewID int output
AS
BEGIN

	-- Is this a new theme?  Does the thematic heading id exist?
	if ( @ThematicHeadingID in ( select ThematicHeadingID from SobekCM_Thematic_Heading ))
	begin	
		-- Yes, exists.. so update existing thematic heading
		update SobekCM_Thematic_Heading
		set ThemeOrder = @ThemeOrder, ThemeName = @ThemeName
		where ThematicHeadingID = @ThematicHeadingID
		
		-- Just return the same id
		set @NewID = @ThematicHeadingID;
	end
	else
	begin
		-- No, it doesn't exist, so insert a new thematic heading
		insert into SobekCM_Thematic_Heading ( ThemeOrder, ThemeName )
		values ( @ThemeOrder, @ThemeName );
		
		-- Save the new id
		set @NewID = @@Identity;
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Save_Template]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a new template to this database
CREATE PROCEDURE [dbo].[mySobek_Save_Template]
	@template_code varchar(20),
	@template_name varchar(100)
AS
BEGIN
	
	-- Does this template already exist?
	if (( select count(*) from mySobek_Template where TemplateCode=@template_code ) > 0 )
	begin
		-- Update the existing template
		update mySobek_Template
		set TemplateName = @template_name
		where TemplateCode = @template_code
	end
	else
	begin
		-- Add a new template
		insert into mySobek_Template ( TemplateName, TemplateCode )
		values ( @template_name, @template_code )
	end
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Save_Project]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a new project to this database
CREATE PROCEDURE [dbo].[mySobek_Save_Project]
	@project_code varchar(20),
	@project_name varchar(100)
AS
BEGIN
	
	-- Does this project already exist?
	if (( select count(*) from mySobek_Project where ProjectCode=@project_code ) > 0 )
	begin
		-- Update the existing project
		update mySobek_Project
		set ProjectName = @project_name
		where ProjectCode = @project_code
	end
	else
	begin
		-- Add a new project
		insert into mySobek_Project ( ProjectName, ProjectCode )
		values ( @project_name, @project_code )
	end
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Reset_User_Password]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Reset a user's password
CREATE PROCEDURE [dbo].[mySobek_Reset_User_Password]
	@userid int,
	@password varchar(100),
	@is_temporary bit
AS
BEGIN
	
	update mySobek_User
	set [Password]=@password, isTemporary_Password=@is_temporary
	where UserID = @userid

END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Add_Item_Error_Log]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add an error into the item error log (usually added by the builder during processing
CREATE PROCEDURE [dbo].[SobekCM_Add_Item_Error_Log]
	@BibID varchar(10),
	@VID varchar(5),
	@METS_Type varchar(20),
	@ErrorDescription varchar(1000)
AS
BEGIN
	-- This is it's own unlinked table since the error may have involved not being able to
	-- save the item to the database, in which case there will be no item to link
	-- the error to.
	insert into SobekCM_Item_Error_Log ( BibID, VID, ErrorDescription, METS_Type, Date )
	values ( @BibID, @VID, @ErrorDescription, @METS_Type, getdate());
	
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Builder_Incoming_Folder_Edit]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a new incoming folder for the builder/bulk loader, or edit
-- an existing incoming folder (by incoming folder id)
CREATE PROCEDURE [dbo].[SobekCM_Builder_Incoming_Folder_Edit]
	@IncomingFolderId int,
	@NetworkFolder varchar(255),
	@ErrorFolder varchar(255),
	@ProcessingFolder varchar(255),
	@Perform_Checksum_Validation bit,
	@Archive_TIFF bit,
	@Archive_All_Files bit,
	@Allow_Deletes bit,
	@Allow_Folders_No_Metadata bit,
	@Contains_Institutional_Folders bit,
	@FolderName nvarchar(150),
	@NewID int output
AS 
BEGIN

	-- Is this a new incoming folder?
	if (( select COUNT(*) from SobekCM_Builder_Incoming_Folders where IncomingFolderId=@IncomingFolderId ) = 0 )
	begin	
		-- Insert new incoming folder
		insert into SobekCM_Builder_Incoming_Folders ( NetworkFolder, ErrorFolder, ProcessingFolder, Perform_Checksum_Validation, Archive_TIFF, Archive_All_Files, Allow_Deletes, Allow_Folders_No_Metadata, Contains_Institutional_Folders, FolderName, Allow_Metadata_Updates )
		values ( @NetworkFolder, @ErrorFolder, @ProcessingFolder, @Perform_Checksum_Validation, @Archive_TIFF, @Archive_All_Files, @Allow_Deletes, @Allow_Folders_No_Metadata, @Contains_Institutional_Folders, @FolderName, 'true' );
		
		-- Save the new id
		set @NewID = @@Identity;
	end
	else
	begin
		-- update existing incoming folder
		update SobekCM_Builder_Incoming_Folders
		set NetworkFolder=@NetworkFolder, ErrorFolder=@ErrorFolder, ProcessingFolder=@ProcessingFolder, Perform_Checksum_Validation=@Perform_Checksum_Validation, Archive_TIFF=@Archive_TIFF, Archive_All_Files=@Archive_All_Files, Allow_Deletes=@Allow_Deletes, Allow_Folders_No_Metadata=@Allow_Folders_No_Metadata, Contains_Institutional_Folders=@Contains_Institutional_Folders, FolderName=@FolderName
		where IncomingFolderId = @IncomingFolderId;
		
		-- Just return the same id
		set @NewID = @IncomingFolderId;	
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Builder_Incoming_Folder_Delete]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Deletes an incoming folder from the builder settings 
CREATE PROCEDURE [dbo].[SobekCM_Builder_Incoming_Folder_Delete]
	@IncomingFolderId int
AS 
BEGIN
	delete from SobekCM_Builder_Incoming_Folders 
	where IncomingFolderId=@IncomingFolderId;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Add_Web_Skin]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to add a new web skin, or edit an existing web skin
CREATE PROCEDURE [dbo].[SobekCM_Add_Web_Skin]
	@webskincode varchar(20),
	@basewebskin varchar(20),
	@overridebanner bit,
	@overrideheaderfooter bit,
	@bannerlink varchar(255),
	@notes varchar(250),
	@build_on_launch bit,
	@suppress_top_nav bit	
AS
BEGIN
	-- Does a web skin with this code already exist?
	if (( select count(*) from SobekCM_Web_Skin where WebSkinCode = @webskincode ) = 0 )
	begin
		-- No?  Add a new one
		insert into SobekCM_Web_Skin ( WebSkinCode, OverrideHeaderFooter, OverrideBanner, BaseWebSkin, BannerLink, Notes, Build_On_Launch, SuppressTopNavigation )
		values ( @webskincode, @overrideheaderfooter, @overridebanner, @basewebskin, @bannerlink, @notes, @build_on_launch, @suppress_top_nav );

	end
	else
	begin
		-- Yes? Update the existing web skin with the same code
		update SobekCM_Web_Skin
		set OverrideHeaderFooter=@overrideheaderfooter, OverrideBanner=@overridebanner, BaseWebSkin=@basewebskin, BannerLink=@bannerlink, Notes=@notes, Build_On_Launch=@build_on_launch, SuppressTopNavigation=@suppress_top_nav
		where WebSkinCode = @webskincode;
	
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Clear_Item_Error_Log]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Marks items from the item error log as cleared, by date.  This does not actually
-- clear the item error completely, just marks the error as cleared so the history 
-- of the error log is maintained
CREATE PROCEDURE [dbo].[SobekCM_Clear_Item_Error_Log]
	@BibID varchar(10),
	@VID varchar(5),
	@ClearedBy varchar(100)
AS
BEGIN

	update SobekCM_Item_Error_Log
	set ClearedBy = @ClearedBy, ClearedDate=getdate()
	where BibID=@BibID and VID=@VID;
	
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Delete_Thematic_Heading]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete a single thematic heading, and unlink any aggregations currently
-- appearing under this thematic heading on the main library home page
CREATE PROCEDURE [dbo].[SobekCM_Delete_Thematic_Heading]
	@ThematicHeadingID int
AS
BEGIN

	-- Remove anything linked to this one
	update SobekCM_Item_Aggregation
	set ThematicHeadingID = -1 where ThematicHeadingID=@ThematicHeadingID;
	
	--Remove this from the list of thematic headings
	delete from SobekCM_Thematic_Heading
	where ThematicHeadingID=@ThematicHeadingID;
END;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_UserName_Exists]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Checks to see if the username or email exists
CREATE PROCEDURE [dbo].[mySobek_UserName_Exists]
	@username nvarchar(100),
	@email varchar(100),
	@username_exists bit output,
	@email_exists bit output
AS
BEGIN

	-- Check if username exists
	if ( ( select count(*) from mySobek_User where UserName = @username ) = 0 )
	begin
		set @username_exists = 'false';
	end
	else
	begin
		set @username_exists = 'true';
	end	

	-- Check if email exists
	if ( ( select count(*) from mySobek_User where EmailAddress = @email ) = 0 )
	begin
		set @email_exists = 'false';
	end
	else
	begin
		set @email_exists = 'true';
	end	

END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Icon_List]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns the list of all icons used by the SobekCM web app
CREATE PROCEDURE [dbo].[SobekCM_Icon_List]
as
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return all the icon information, in any sort order
	select Icon_Name, Icon_URL, Link=isnull(Link,''), Title=isnull(Title,'')
	from SobekCM_Icon;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Mime_Types]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Get_Mime_Types]
AS
BEGIN
	select Extension, MimeType, isBlocked, shouldForward, MimeTypeID
	from SobekCM_Mime_Types;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Metadata_Fields]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return the list of all metadata searchable fields
CREATE PROCEDURE [dbo].[SobekCM_Get_Metadata_Fields]
AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Just return all the values, but sort by display term
	select * 
	from SobekCM_Metadata_Types
	order by DisplayTerm;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Item_Aggregation_Aliases]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the list of all item aggregation aliases and what they forward to
CREATE PROCEDURE [dbo].[SobekCM_Get_Item_Aggregation_Aliases]
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Return all teh alias information
	select A.AggregationAliasID, A.AggregationAlias, C.Code
	from SobekCM_Item_Aggregation C, SobekCM_Item_Aggregation_Alias A
	where A.AggregationID = C.AggregationID
	order by AggregationAlias;
	
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_IP_Restriction_Range]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return details on an IP restriction range, including all of the individual IPs included
CREATE PROCEDURE [dbo].[SobekCM_Get_IP_Restriction_Range]
	@ip_rangeid int
AS
BEGIN
	
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get all information (includes notes this time)
	select *
	from SobekCM_IP_Restriction_Range
	where IP_RangeID = @ip_rangeid;

	-- Get all associated single ip ranges
	select IP_SingleID, StartIP, ISNULL(EndIP,'') as EndIP, ISNULL(Notes,'') as Notes
	from SobekCM_IP_Restriction_Single
	where IP_RangeID = @ip_rangeid
	order by StartIP ASC;

END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Items_Needing_Aditional_Work]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the list of itesm currently flagged for needing additional work.
-- This is used by the builder to determine what needs post-processing.
CREATE procedure [dbo].[SobekCM_Get_Items_Needing_Aditional_Work]
as
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return the bibid, vid, and primary key to the items which are flagged
	select G.BibID, I.VID, I.ItemID
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.GroupID = G.GroupID )
	  and ( I.AdditionalWorkNeeded = 'true' )
	order by BibID, VID;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_OAI_Data_Item]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns the OAI data for a single item from the oai source tables
CREATE PROCEDURE [dbo].[SobekCM_Get_OAI_Data_Item]
	@bibid varchar(10),
	@data_code varchar(20)
AS
begin
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Select the matching rows
	select G.GroupID, BibID, O.OAI_Data, O.OAI_Date
	from SobekCM_Item_Group G, SobekCM_Item_Group_OAI O
	where G.BibID = @bibid
	  and G.GroupID=O.GroupID	
	  and O.Data_Code = @data_code;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_OAI_Data_Codes]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the distinct data codes present in the database for OAI (such as 'oai_dc')
CREATE PROCEDURE [dbo].[SobekCM_Get_OAI_Data_Codes]
AS
BEGIN
	-- Dirty read here won't hurt anything
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return distinct codes used in the OAI table
	select distinct(Data_Code)
	from SobekCM_Item_Group_OAI;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Delete_Single_IP]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delate a single IP, from a larger IP restriction range
CREATE PROCEDURE [dbo].[SobekCM_Delete_Single_IP]
	@ip_singleid int
AS
BEGIN
	
	delete from SobekCM_IP_Restriction_Single where IP_SingleID=@ip_singleid;

END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Delete_Portal]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete an entire URL Portal, by URL portal ID
CREATE PROCEDURE [dbo].[SobekCM_Delete_Portal]
	@PortalID int
AS
BEGIN

	-- Remove anything linked to this one
	delete from SobekCM_Portal_Item_Aggregation_Link where PortalID=@PortalID;
	delete from SobekCM_Portal_Web_Skin_Link where PortalID=@PortalID;
	delete from SobekCM_Portal_URL_Statistics where PortalID=@PortalID;
	delete from SobekCM_Portal_URL where PortalID = @PortalID;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Delete_Item_Aggregation_Alias]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete a single item aggregation alias (or forwarding) by alias
CREATE PROCEDURE [dbo].[SobekCM_Delete_Item_Aggregation_Alias]
	@alias varchar(50)
AS
BEGIN
	delete from SobekCM_Item_Aggregation_Alias
	where AggregationAlias = @alias;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Clear_External_Record_Numbers]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- This procedure clears all external record numbers from an existing item
CREATE PROCEDURE [dbo].[SobekCM_Clear_External_Record_Numbers]
	@groupID int
AS
begin

	-- Remove any links
	delete from SobekCM_Item_Group_External_Record where GroupID = @groupid;
	
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Check_For_Record_Existence]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns basic information for the importer to verify if a record currently exists, 
-- by checkig BibID, VID, OCLC_Number, or ALEPH number
CREATE PROCEDURE [dbo].[SobekCM_Check_For_Record_Existence]
	@BibID varchar(10),
	@VID varchar(5),
	@OCLC_Number bigint,
	@Local_Cat_Number int
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Check to see if a VID was provided
	if ( LEN(@VID) > 0 )
	begin	
		select G.BibID, I.VID, G.OCLC_Number, G.ALEPH_Number, G.GroupTitle, I.Title, I.Author, I.Publisher
		from SobekCM_Item_Group G, SobekCM_Item I
		where G.GroupID = I.GroupID 
		and ((( G.BibID = @BibID ) and ( I.VID = @VID )) or ( G.OCLC_Number = @OCLC_Number ) or ( G.ALEPH_Number = @Local_Cat_Number ));	
	end
	else
	begin
		select G.BibID, I.VID, G.OCLC_Number, G.ALEPH_Number, G.GroupTitle, I.Title, I.Author, I.Publisher
		from SobekCM_Item_Group G, SobekCM_Item I
		where G.GroupID = I.GroupID 
		and (( G.BibID = @BibID ) or ( G.OCLC_Number = @OCLC_Number ) or ( G.ALEPH_Number = @Local_Cat_Number ));	
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Add_OAI_PMH_Data]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add some OAI-PMH data to an item group.  Included will be the data (usually in XML format)
-- and the OAI-PMH code for that data type.  The XML information is saved as nvarchar, rather
-- than XML, since this data is never sub-queried.  It is just returned while serving OAI.
CREATE PROCEDURE [dbo].[SobekCM_Add_OAI_PMH_Data]
	@groupid int,
	@data_code nvarchar(20),
	@oai_data nvarchar(max),
	@overlay_existing bit
AS
begin

	-- Does this already exists?
	if (( select COUNT(*) from SobekCM_Item_Group_OAI where GroupID=@groupid ) = 0 )
	begin
		insert into SobekCM_Item_Group_OAI ( GroupID, OAI_Data, OAI_Date, Data_Code )
		values ( @groupid, @oai_data, GETDATE(), @data_code );
	end
	else
	begin
		if (( @overlay_existing = 'true' ) or (( select ItemCount from SobekCM_Item_Group where GroupID=@groupid ) <= 1 ))
		begin
			update SobekCM_Item_Group_OAI
			set OAI_Data=@oai_data, OAI_Date=GETDATE(), Data_Code=@data_code
			where GroupID=@groupid and Locked='false';
		end;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Add_External_Record_Number]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- This procedure adds a new external record number to an existing item
CREATE PROCEDURE [dbo].[SobekCM_Add_External_Record_Number]
	@groupID int,
	@extRecordValue varchar(50),
	@extRecordType varchar(25)
AS
begin transaction

	-- declare two variables that may be needed for this
	declare @extRecordTypeID int;
	declare @extRecordLinkID int;

	-- Look for an existing record type
	select @extRecordTypeID = isnull(extRecordTypeID, -1)
	from SobekCM_External_Record_Type
	where (extRecordType = @extRecordType);

	-- Was this a new record type
	if ( isnull( @extRecordTypeID, -1 ) < 0 )
	begin
		-- Add this new record type
		insert into SobekCM_External_Record_Type ( ExtRecordType, repeatableTypeFlag )
		values ( @extRecordType, 1 );

		-- Save this new id
		set @extRecordTypeID = @@identity;
	end;

	-- The linkID parameter is less than zero; query the database 
	-- to see if one exists for this record type.		
	select @extRecordLinkID = isnull( extRecordLinkID, -1 )
	from [SobekCM_Item_Group_External_Record]
	where (GroupID = @groupID )
	  and ( ExtRecordTypeID = @extRecordTypeID )
	  and ( ExtRecordValue = @extRecordValue );

	if (isnull( @extRecordLinkID, -1 ) < 0)
	begin	
		-- Check to see if this record type is singular type (nonrepeatable)
		if (( select count(*) from SobekCM_External_Record_Type where ExtRecordTypeID = @extRecordTypeID and repeatableTypeFlag = 'False' ) > 0 )
		begin
			-- Look for an existing singular record for this item group
			if (( select count(*) from SobekCM_Item_Group_External_Record 
				where ( ExtRecordTypeID = @extRecordTypeID ) and ( GroupID = @groupID )) > 0 )
			begin
				-- Get the link id
				select @extRecordLinkID = extRecordLinkID
				from SobekCM_Item_Group_External_Record 
				where ( ExtRecordTypeID = @extRecordTypeID ) and ( GroupID = @groupID );

				--Update existing link
				update SobekCM_Item_Group_External_Record
				set extRecordValue = @extRecordValue 
				where (extRecordLinkID = @extRecordLinkID);
			end
			else
			begin
				-- No existing record for this singular record type, so just insert
				insert into SobekCM_Item_Group_External_Record ( groupid, extRecordTypeID, extRecordValue)
				values ( @groupID, @extRecordTypeID, @extRecordValue );
			end;
		end
		else
		begin
			-- Non-singular record type value, so just insert if it doesn't exist
			if (( select COUNT(*) from SobekCM_Item_Group_External_Record where GroupID=@groupID and ExtRecordTypeID=@extRecordTypeID and ExtRecordValue = @extRecordValue ) = 0 )
			begin
				insert into SobekCM_Item_Group_External_Record ( groupid, extRecordTypeID, extRecordValue)
				values ( @groupID, @extRecordTypeID, @extRecordValue );
			end;
		end;
	end;
	
commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Edit_Single_IP]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Edits a single ip point within an entire IP restriction set of ranges, or
-- else adds a new ip point, if the provided ip_singleid is zero or less
CREATE PROCEDURE [dbo].[SobekCM_Edit_Single_IP]
	@ip_singleid int,
	@ip_rangeid int,
	@startip char(15),
	@endip char(15),
	@notes nvarchar(100),
	@new_ip_singleid int output
AS
BEGIN
	
	-- Was a primary key provided?
	if ( @ip_singleid > 0 )
	begin
	
		-- Update existing if there was one
		update SobekCM_IP_Restriction_Single
		set StartIP = @startip, EndIP = @endip, Notes=@notes
		where IP_SingleID = @ip_singleid;
		
		-- Return the existing ID
		set @new_ip_singleid = @ip_singleid;
	
	end
	else
	begin
	
		-- Insert new
		insert into SobekCM_IP_Restriction_Single ( IP_RangeID, StartIP, EndIP, Notes )
		values ( @ip_rangeid, @startip, @endip, @notes );
		
		-- Return the new primary key
		set @new_ip_singleid = @@identity;
	
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Edit_Portal]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to edit an existing URL portal or saving a new URL portal
CREATE PROCEDURE [dbo].[SobekCM_Edit_Portal]
	@PortalID int,
	@Base_URL nvarchar(150),
	@isActive bit,
	@isDefault bit,
	@Abbreviation nvarchar(10),
	@Name nvarchar(250),
	@Default_Aggregation nvarchar(20),
	@Base_PURL nvarchar(150),
	@Default_Web_Skin nvarchar(20),
	@NewID int output
AS
BEGIN TRANSACTION

	-- Is this a new portal?
	if (( select COUNT(*) from SobekCM_Portal_URL where PortalID=@PortalID ) = 0 )
	begin	
		-- Insert new portal
		insert into SobekCM_Portal_URL ( Abbreviation, isActive, isDefault, Name, Base_URL, Base_PURL )
		values ( @Abbreviation, @isActive, @isDefault, @Name, @Base_URL, @Base_PURL );
		
		-- Save the new id
		set @NewID = @@Identity;	
	end
	else
	begin
		-- update existing portal
		update SobekCM_Portal_URL
		set Abbreviation=@Abbreviation, isActive=@isActive, isDefault=@isDefault, Name=@Name, Base_URL=@Base_URL, Base_PURL=@Base_PURL
		where PortalID = @PortalID;
		
		-- Just return the same id
		set @NewID = @PortalID;	
	end;
	
	-- Clear any default aggregations and web skins
	delete from SobekCM_Portal_Item_Aggregation_Link where PortalID=@NewID;
	delete from SobekCM_Portal_Web_Skin_Link where PortalID=@NewID;

	-- Add the default aggregation, if one is chosen
	if ( LEN(isnull(@Default_Aggregation, '')) > 0 )
	begin
		-- Does this aggregation exists
		if (( select COUNT(*) from SobekCM_Item_Aggregation where Code=@Default_Aggregation ) = 1 )
		begin
			declare @aggrid int;
			select @aggrid=AggregationID from SobekCM_Item_Aggregation where Code=@Default_Aggregation;
			
			insert into SobekCM_Portal_Item_Aggregation_Link ( PortalID, AggregationID, isDefault )
			values ( @NewID, @aggrid, 'true' );		
		end;	
	end;	
	
	-- Add the web skin, if one is chosen
	if ( LEN(isnull(@Default_Web_Skin, '')) > 0 )
	begin
		-- Does this aggregation exists
		if (( select COUNT(*) from SobekCM_Web_Skin where WebSkinCode=@Default_Web_Skin ) = 1 )
		begin
			declare @skinid int;
			select @skinid=WebSkinID from SobekCM_Web_Skin where WebSkinCode=@Default_Web_Skin;
			
			insert into SobekCM_Portal_Web_Skin_Link ( PortalID, WebSkinID, isDefault )
			values ( @NewID, @skinid, 'true' );		
		end;	
	end;	
COMMIT TRANSACTION;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_BibID_VID_From_ItemID]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Allows a lookup of the BibID/VID for an item from the database's primary key.
-- This is used for legacy URLs which may reference the item by itemid.
CREATE PROCEDURE [dbo].[SobekCM_Get_BibID_VID_From_ItemID]
	@itemid int
as
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return the item group / item information exactly like it is returned in the Item list brief procedure
	select G.BibID, I.VID
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.GroupID = G.GroupID )
	  and ( G.Deleted = CONVERT(bit,0) )
	  and ( I.Deleted = CONVERT(bit,0) )
	  and ( I.ItemID = @itemid );

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_All_Regions]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns the entire GeoCore database of locations
CREATE PROCEDURE [dbo].[SobekCM_Get_All_Regions] AS
begin

	select GeoAuthCode, RegionName, RegionTypeName
	from Auth_GeoRegion R, Auth_GeoRegion_Type T
	where R.RegionTypeID = T.RegionTypeID
	order by GeoAuthCode;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_All_Portals]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get all of the portal information for this digital library
CREATE PROCEDURE [dbo].[SobekCM_Get_All_Portals]
	@activeonly bit
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	if ( @activeonly = 'true' )
	begin
	
		select *
		from SobekCM_Portal_URL
		where isActive = 'true';
	
		select P.PortalID, A.Code
		from SobekCM_Portal_URL P, SobekCM_Portal_Item_Aggregation_Link AL, SobekCM_Item_Aggregation A
		where ( P.PortalID = AL.PortalID )
		  and ( AL.AggregationID = A.AggregationID )
		  and ( P.isActive = 'true' );
		  
		select P.PortalID, W.WebSkinCode
		from SobekCM_Portal_URL P, SobekCM_Portal_Web_Skin_Link WL, SobekCM_Web_Skin W
		where ( P.PortalID = WL.PortalID )
		  and ( WL.WebSkinID = W.WebSkinID )
		  and ( P.isActive = 'true' );
	end
	else
	begin
	
		select *
		from SobekCM_Portal_URL;
	
		select P.PortalID, A.Code
		from SobekCM_Portal_URL P, SobekCM_Portal_Item_Aggregation_Link AL, SobekCM_Item_Aggregation A
		where ( P.PortalID = AL.PortalID )
		  and ( AL.AggregationID = A.AggregationID );
		  
		select P.PortalID, W.WebSkinCode
		from SobekCM_Portal_URL P, SobekCM_Portal_Web_Skin_Link WL, SobekCM_Web_Skin W
		where ( P.PortalID = WL.PortalID )
		  and ( WL.WebSkinID = W.WebSkinID );
	
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_All_IP_Restrictions]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Get_All_IP_Restrictions]
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get all the IP information
	select R.Title, R.IP_RangeID, R.Not_Valid_Statement, isnull(S.StartIP,'') as StartIP, isnull(S.EndIP,'') as EndIP
	from SobekCM_IP_Restriction_Range AS R LEFT JOIN 
	     SobekCM_IP_Restriction_Single AS S ON R.IP_RangeID = S.IP_RangeID
	order by IP_RangeID ASC;

END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_All_Groups_First_VID]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Get_All_Groups_First_VID]
AS
begin
  
	  with temp as (
		select G.BibID, G.GroupID, I.VID, 
    		row_number() over (partition by G.BibID order by I.VID) as rownum
		from SobekCM_Item_Group G, SobekCM_Item I
		where G.GroupID=I.GroupID
	)
	select BibID, GroupID, VID from temp where rownum = 1;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Edit_Aggregation_Details]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Edit_Aggregation_Details]
	@AggregationID int,
	@Code varchar(20),
	@Name varchar(255),
	@ShortName varchar(100),
	@Description varchar(1000),
	@ThematicHeadingID int,
	@Type varchar(50),
	@isActive bit,
	@Hidden bit,
	@DisplayOptions varchar(10),
	@Map_Search tinyint,
	@Map_Display tinyint,
	@OAI_Flag bit,
	@OAI_Metadata nvarchar(2000),
	@ContactEmail nvarchar(255),
	@HasNewItems bit,
	@Default_Skin varchar(50),
	@Items_Can_Be_Described tinyint,
	@LastItemAdded date,
	@External_Link nvarchar(255),
	@Can_Browse_Items bit,
	@Include_In_Collection_Facet bit,
	@ParentID int,
	@NewID int output
AS
BEGIN
	-- Is this a new aggregation?
	if (( select COUNT(*) from SobekCM_Item_Aggregation where AggregationID=@AggregationID ) = 0 )
	begin	
		-- Insert new portal
		insert into SobekCM_Item_Aggregation ( Code, Name, ShortName, [Description], ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, OAI_Metadata, ContactEmail, HasNewItems, DefaultInterface, Items_Can_Be_Described, LastItemAdded, External_Link, Can_Browse_Items, DateAdded, Include_In_Collection_Facet )
		values ( @Code, @Name, @ShortName, @Description, @ThematicHeadingID, @Type, @isActive, @Hidden, @DisplayOptions, @Map_Search, @Map_Display, @OAI_Flag, @OAI_Metadata, @ContactEmail, @HasNewItems, @Default_Skin, @Items_Can_Be_Described, @LastItemAdded, @External_Link, @Can_Browse_Items, GETDATE(), @Include_In_Collection_Facet );
		
		-- Save the new id
		set @NewID = @@Identity;
		
		-- Was a parent id provided for this new aggreation?
		if ( isnull(@ParentID, -1 ) > 0 )
		begin		
			insert into SobekCM_Item_Aggregation_Hierarchy ( ParentID, ChildID )
			values ( @parentid, @NewID )
		end;
	end
	else
	begin
		-- update the existing information
		update SobekCM_Item_Aggregation
		set Code=@Code, Name=@Name, ShortName=@ShortName, [Description]=@Description, 
			ThematicHeadingID=@ThematicHeadingID, [Type]=@Type, isActive=@isActive, Hidden=@Hidden, 
			DisplayOptions=@DisplayOptions, Map_Search=@Map_Search, Map_Display=@Map_Display, 
			OAI_Flag=@OAI_Flag, OAI_Metadata=@OAI_Metadata, ContactEmail=@ContactEmail, 
			HasNewItems=@HasNewItems, DefaultInterface=@Default_Skin, 
			Items_Can_Be_Described=@Items_Can_Be_Described, LastItemAdded=@LastItemAdded, 
			External_Link=@External_Link, Can_Browse_Items=@Can_Browse_Items, 
			Include_In_Collection_Facet = @Include_In_Collection_Facet
		where AggregationID=@AggregationID;
		
		-- Just return the same id
		set @NewID = @AggregationID;
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Delete_Web_Skin]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to delete a web skin, and unlink any items or web portals which
-- were linked to this web skin
CREATE PROCEDURE [dbo].[SobekCM_Delete_Web_Skin]
	@webskincode varchar(20)
AS
BEGIN
	-- Only continue if the web skin code exists
	if (( select count(*) from SobekCM_Web_Skin where WebSkinCode = @webskincode ) > 0 )
	begin	
	
		-- Get the web skin code
		declare @webskinid int;
		select @webskinid=WebSkinID from SobekCM_Web_Skin where WebSkinCode=@webskincode;
		
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
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Clear_Web_Skin_Portal_Link]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Clears the links between a URL portal and all associated web skins.
-- This will cause the URL portal to disply any requested web skin.  
-- This also clears the link to a default web skin under that portal.
CREATE PROCEDURE [dbo].[SobekCM_Clear_Web_Skin_Portal_Link]
	@PortalID int
AS
BEGIN
	delete from SobekCM_Portal_Web_Skin_Link where PortalID=@PortalID;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Builder_Settings]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the general system-wide settings and then also returns information
-- for the builder, such as the incoming folders and possibly the item list
CREATE PROCEDURE [dbo].[SobekCM_Get_Builder_Settings]
	@include_items bit
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get all the standard SobekCM settings
	select *
	from SobekCM_Settings;
	
	-- Get the list of all incoming FTP folder
	select * 
	from SobekCM_Builder_Incoming_Folders F;
	
	-- Did the user also want the item list?
	if ( @include_items = 'true' )
	begin
		-- Get information about each BibID/VID
		select BibID, ItemID, VID, G.Large_Format, G.[Type], I.Dark
		from SobekCM_Item_Group G, SobekCM_Item I
		where G.GroupID = I.GroupID;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Collection_Hierarchies]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return the hierarchies for all (non-institutional) aggregations
-- starting with the 'ALL' aggregation
-- Version 3.05 - Added check to exclude DELETED aggregations
CREATE PROCEDURE [dbo].[SobekCM_Get_Collection_Hierarchies]
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
	  and ( C.Type not like '%Institution%' )
	  and ( C.Deleted = 'false' );
	
	-- Now, try to find any children to this ( second level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -2
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -1 )
	  and ( C.Type not like '%Institution%' )
	  and ( C.Deleted = 'false' );

	-- Now, try to find any children to this ( third level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -3
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -2 )
	  and ( C.Type not like '%Institution%' )
	  and ( C.Deleted = 'false' );

	-- Now, try to find any children to this ( fourth level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -4
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -3 )
	  and ( C.Type not like '%Institution%' )	  
	  and ( C.Deleted = 'false' );
	
	-- Now, try to find any children to this ( fifth level below )
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, Parent_Name, Parent_ShortName, HierarchyLevel )
	select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, P.Name, P.ShortName, -5
	from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( HierarchyLevel = -4 )
	  and ( C.Type not like '%Institution%' )
	  and ( C.Deleted = 'false' );

	-- Return all the children
	select Code, ParentCode, [Name], [ShortName], [Type], HierarchyLevel, isActive, Hidden, Parent_Name, Parent_ShortName
	from #TEMP_CHILDREN_BUILDER
	order by HierarchyLevel DESC;
	
	-- drop the temporary tables
	drop table #TEMP_CHILDREN_BUILDER;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_ItemID]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure returns the item id as a single row given the bibid and vid.
-- This also doubles as a quick way to check if a certain item exists in
-- the database and is employed by some of the workflow tools
CREATE PROCEDURE [dbo].[SobekCM_Get_ItemID]
	@bibid varchar(10),
	@vid varchar(5)
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return the item id as a single row relation
	select ItemID
	from SobekCM_Item I, SobekCM_Item_Group G
	where I.GroupID=G.GroupID 
	  and I.VID=@vid
	  and G.BibID=@bibid;

end;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Update_UFDC_User]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure allows an admin to edit permissions flags for this user
CREATE PROCEDURE [dbo].[mySobek_Update_UFDC_User]
	@userid int,
	@can_submit bit,
	@is_internal bit,
	@can_edit_all bit,
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
		EditTemplate=@edit_template,
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
/****** Object:  StoredProcedure [dbo].[mySobek_Save_User2]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves a user
CREATE PROCEDURE [dbo].[mySobek_Save_User2]
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
	@default_project varchar(50),
	@organization_code varchar(15),
	@receivestatsemail bit
AS
BEGIN

	if ( @userid < 0 )
	begin

		-- Add this into the user table first
		insert into mySobek_User ( UFID, UserName, [Password], EmailAddress, LastName, FirstName, DateCreated, LastActivity, isActive,  Note_Length, Can_Make_Folders_Public, isTemporary_Password, Can_Submit_Items, NickName, Organization, College, Department, Unit, Default_Rights, sendEmailOnSubmission, UI_Language, Internal_User, OrganizationCode, Receive_Stats_Emails )
		values ( @ufid, @username, @password, @emailaddress, @lastname, @firstname, getdate(), getDate(), 'true', 1000, 'true', 'false', @cansubmititems, @nickname, @organization, @college, @department, @unit, @rights, @sendemail, @language, 'false', @organization_code, @receivestatsemail )

		-- Get the user is
		declare @newuserid int
		set @newuserid = @@identity

		-- Add the IR template and IR 'project' as defaults for this user
		insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject ) values ( @newuserid, 1, 'true' )

		insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate ) values ( @newuserid, 1, 'true' )

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
		where UserID = @userid

		-- Set the default template
		if ( len( @default_template ) > 0 )
		begin
			-- Get the template id
			declare @templateid int
			select @templateid = TemplateID from mySobek_Template where TemplateCode=@default_template

			-- Clear the current default template
			update mySobek_User_Template_Link set DefaultTemplate = 'false' where UserID=@userid

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_Template_Link where UserID=@userid and TemplateID=@templateid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_Template_Link set DefaultTemplate = 'true' where UserID=@userid and TemplateID=@templateid
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate ) values ( @userid, @templateid, 'true' )				
			end
		end		

		-- Set the default project
		if ( len( @default_project ) > 0 )
		begin
			-- Get the project id
			declare @projectid int
			select @projectid = ProjectID from mySobek_Project where ProjectCode=@default_project

			-- Clear the current default project
			update mySobek_User_Project_Link set DefaultProject = 'false' where UserID=@userid

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_Project_Link where UserID=@userid and ProjectID=@projectid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_Project_Link set DefaultProject = 'true' where UserID=@userid and ProjectID=@projectid
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject ) values ( @userid, @projectid, 'true' )				
			end
		end
	end	
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Save_User_Search]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a sarch to the user's list of saved searches
CREATE PROCEDURE [dbo].[mySobek_Save_User_Search]
	@userid int,
	@searchurl nvarchar(500),
	@searchdescription nvarchar(500),
	@itemorder int,
	@usernotes nvarchar(2000),
	@new_usersearchid int output
AS
begin

	-- See if this already exists
	if (( select count(*) from mySobek_User_Search where UserID=@userid and SearchURL=@searchurl ) > 0 )
	begin
		-- update existing
		update mySobek_User_Search
		set ItemOrder=@itemorder, UserNotes=@usernotes
		where UserID=@userid and SearchURL=@searchurl

		-- Just set this to -1, since nothing new was added
		set @new_usersearchid = -1
	end
	else
	begin
		-- Add a new search
		insert into mySobek_User_Search( UserID, SearchURL, SearchDescription, ItemOrder, UserNotes, DateAdded )
		values ( @userid, @searchurl, @searchdescription, @itemorder, @usernotes, getdate())

		-- Return the new identifier
		set @new_usersearchid = @@identity
	end
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Save_User_Group]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves information about a single user group
CREATE PROCEDURE [dbo].[mySobek_Save_User_Group]
	@usergroupid int,
	@groupname nvarchar(150),
	@groupdescription nvarchar(1000),
	@can_submit_items bit,
	@is_internal bit,
	@can_edit_all bit,
	@is_system_admin bit,
	@is_portal_admin bit,
	@include_tracking_standard_forms bit,
	@clear_projects_templates bit,
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
	if ( @clear_projects_templates = 'true' )
	begin
		delete from mySobek_User_Group_Project_Link where UserGroupID=@usergroupid;
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
/****** Object:  StoredProcedure [dbo].[mySobek_Save_User]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves a user
CREATE PROCEDURE [dbo].[mySobek_Save_User]
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
	@default_project varchar(50),
	@organization_code varchar(15)
AS
BEGIN

	if ( @userid < 0 )
	begin

		-- Add this into the user table first
		insert into mySobek_User ( UFID, UserName, [Password], EmailAddress, LastName, FirstName, DateCreated, LastActivity, isActive,  Note_Length, Can_Make_Folders_Public, isTemporary_Password, Can_Submit_Items, NickName, Organization, College, Department, Unit, Default_Rights, sendEmailOnSubmission, UI_Language, Internal_User, OrganizationCode )
		values ( @ufid, @username, @password, @emailaddress, @lastname, @firstname, getdate(), getDate(), 'true', 1000, 'true', 'false', @cansubmititems, @nickname, @organization, @college, @department, @unit, @rights, @sendemail, @language, 'false', @organization_code )

		-- Get the user is
		declare @newuserid int
		set @newuserid = @@identity

		-- Add the IR template and IR 'project' as defaults for this user
		insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject ) values ( @newuserid, 1, 'true' )

		insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate ) values ( @newuserid, 1, 'true' )

	end
	else
	begin

		-- Update this user
		update mySobek_User
		set UFID = @ufid, UserName = @username, EmailAddress=@emailAddress,
			Firstname = @firstname, Lastname = @lastname, Can_Submit_Items = @cansubmititems,
			NickName = @nickname, Organization=@organization, College=@college, Department=@department,
			Unit=@unit, Default_Rights=@rights, sendEmailOnSubmission = @sendemail, UI_Language=@language,
			OrganizationCode=@organization_code
		where UserID = @userid

		-- Set the default template
		if ( len( @default_template ) > 0 )
		begin
			-- Get the template id
			declare @templateid int
			select @templateid = TemplateID from mySobek_Template where TemplateCode=@default_template

			-- Clear the current default template
			update mySobek_User_Template_Link set DefaultTemplate = 'false' where UserID=@userid

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_Template_Link where UserID=@userid and TemplateID=@templateid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_Template_Link set DefaultTemplate = 'true' where UserID=@userid and TemplateID=@templateid
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate ) values ( @userid, @templateid, 'true' )				
			end
		end		

		-- Set the default project
		if ( len( @default_project ) > 0 )
		begin
			-- Get the project id
			declare @projectid int
			select @projectid = ProjectID from mySobek_Project where ProjectCode=@default_project

			-- Clear the current default project
			update mySobek_User_Project_Link set DefaultProject = 'false' where UserID=@userid

			-- Does this link already exist?
			if (( select count(*) from mySobek_User_Project_Link where UserID=@userid and ProjectID=@projectid ) > 0 )
			begin
				-- Update the link
				update mySobek_User_Project_Link set DefaultProject = 'true' where UserID=@userid and ProjectID=@projectid
			end
			else
			begin
				-- Just add this link
				insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject ) values ( @userid, @projectid, 'true' )				
			end
		end
	end	
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Link_User_To_User_Group]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[mySobek_Link_User_To_User_Group]
	@userid int,
	@usergoupid int
AS
begin

	if (( select COUNT(*) from mySobek_User_Group_Link where UserID=@userid and UserGroupID = @usergoupid ) = 0 )
	begin
	
		insert into mySobek_User_Group_Link ( UserGroupID, UserID )
		values ( @usergoupid, @userid )
	
	end

end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Link_User_To_Item]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Links a user to an item for editing purposes
CREATE PROCEDURE [dbo].[mySobek_Link_User_To_Item]
	@userid int,
	@groupid char(10)
AS
BEGIN
	
	if (( select COUNT(*) from mySobek_User_Bib_Link where UserID=@userid and GroupID = @groupid ) = 0 )
	begin
		insert into mySobek_User_Bib_Link ( UserID, GroupID )
		values ( @userid, @groupid )
	end

END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_Searches]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get list of saved searches
CREATE PROCEDURE [dbo].[mySobek_Get_User_Searches]
	@userid int
AS
BEGIN
	select * 
	from mySobek_User_Search
	where UserID = @userid
	order by ItemOrder, DateAdded
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_Group]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get information about a single user group, by user group id
CREATE PROCEDURE [dbo].[mySobek_Get_User_Group]
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

	-- Get the projects
	select P.ProjectCode, P.ProjectName
	from mySobek_Project P, mySobek_User_Group_Project_Link PL
	where ( PL.UserGroupID = @usergroupid ) and ( PL.ProjectID = P.ProjectID );

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
/****** Object:  StoredProcedure [dbo].[mySobek_Edit_User_Folder]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Edit a user's folder information
CREATE PROCEDURE [dbo].[mySobek_Edit_User_Folder]
	@userfolderid int,
	@userid int,
	@parentfolderid int,
	@foldername nvarchar(255),
	@is_public bit,
	@description nvarchar(4000),
	@new_folder_id int out
AS
begin transaction

	-- Does this reference an existing folder?
	if ( @userfolderid > 0 )
	begin
		-- Update the existing folder
		update mySobek_User_Folder 
		set FolderName=@foldername, isPublic=@is_public, FolderDescription=@description
		where UserFolderID=@userfolderid and UserID=@userid

		-- Return the old folder id
		set @new_folder_id = @userfolderid
	end
	else
	begin
		-- Add this as a new folder
		if ( @parentfolderid < 0 )
		begin
			-- Insert this as a new folder, without a parent
			insert into mySobek_User_Folder( UserID, FolderName, isPublic, FolderDescription )
			values ( @userid, @foldername, @is_public, @description )
		end
		else
		begin
			-- Insert this as a new folder, with a parent
			insert into mySobek_User_Folder( UserID, FolderName, isPublic, FolderDescription, ParentFolderID )
			values ( @userid, @foldername, @is_public, @description, @parentfolderid )
		end	

		-- Return the new folder id
		set @new_folder_id = @@identity

	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Delete_User_Search]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete a saved search
CREATE PROCEDURE [dbo].[mySobek_Delete_User_Search]
	@usersearchid int
AS
BEGIN
	delete from mySobek_User_Search
	where UserSearchID=@usersearchid
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Delete_User_Folder]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete a user's folder
CREATE PROCEDURE [dbo].[mySobek_Delete_User_Folder]
	@userfolderid int,
	@userid int
AS
begin transaction
	
	-- Only continue if the folder exists and is tagged to this user
	if ((select count(*) from mySobek_User_Folder where userfolderid=@userfolderid and userid=@userid) > 0 )
	begin
		
		-- Only continue if there are no subfolders
		if (( select count(*) from mySobek_User_Folder where ParentFolderID = @userfolderid ) <= 0 )
		begin
			DELETE FROM mySobek_User_Folder
			where UserID=@userid and UserFolderID=@userfolderid
		end
	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Delete_Template]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to delete a project completely.
-- Added in Version 3.05
CREATE PROCEDURE [dbo].[mySobek_Delete_Template]
	@template_code varchar(20)
AS
BEGIN

	-- Only continue if the template code exists
	if (( select count(*) from mySobek_Template where TemplateCode = @template_code ) > 0 )
	begin		
		-- Get the project id
		declare @templateid int;
		select @templateid=TemplateID from mySobek_Template where TemplateCode = @template_code;
		
		-- Remove links to user groups
		delete from mySobek_User_Group_Template_Link
		where TemplateID = @templateid;
		
		-- Remove links to users
		delete from mySobek_User_Template_Link
		where TemplateID = @templateid;
		
		-- Delete this template
		delete from mySobek_Template
		where TemplateID=@templateid;

	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Delete_Project]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to delete a project completely.
-- Added in Version 3.05
CREATE PROCEDURE [dbo].[mySobek_Delete_Project]
	@project_code varchar(20)
AS
BEGIN

	-- Only continue if the project code exists
	if (( select count(*) from mySobek_Project where ProjectCode = @project_code ) > 0 )
	begin		
		-- Get the project id
		declare @projid int;
		select @projid=ProjectID from mySobek_Project where ProjectCode = @project_code;
		
		-- Remove links to user groups
		delete from mySobek_User_Group_Project_Link
		where ProjectID = @projid;
		
		-- Remove links to users
		delete from mySobek_User_Project_Link
		where ProjectID = @projid;
		
		-- Delete this project
		delete from mySobek_Project
		where ProjectID=@projid;

	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_Folder_Information]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get all the information about a folder, by folder id
CREATE PROCEDURE [dbo].[mySobek_Get_Folder_Information]
	@folderid int
AS
BEGIN
	select UserFolderID, FolderName, isPublic, FolderDescription, U.UserID, FirstName, LastName, NickName, EmailAddress
	from mySobek_User_Folder F, mySobek_User U
	where ( UserFolderID=@folderid )
	  and ( U.UserID = F.UserID )
END
GO
/****** Object:  StoredProcedure [dbo].[Importer_Load_Lookup_Tables]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- This procedure calls other procedures to load data into the 
-- various look up tables used in the application.
CREATE PROCEDURE [dbo].[Importer_Load_Lookup_Tables]
 AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the list of items with identifiers for match checking
	select G.GroupID, ItemID, BibID, VID, G.ALEPH_Number, G.OCLC_Number, GroupTitle, Title
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( G.GroupID = I.GroupID );
	
	-- Get all the institutions and other aggregations
	select *
	from SobekCM_Item_Aggregation A
	order by Code;

	-- Get all the wordmarks
	select * 
	from SobekCM_Icon
    order by Icon_Name;
	
end;
GO
/****** Object:  StoredProcedure [dbo].[FDA_Get_Reports_By_Package]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Sullivan, Mark
-- Create date: October 1, 2007
-- Description:	Get all the information about a single report
-- ==========================================================
CREATE PROCEDURE [dbo].[FDA_Get_Reports_By_Package]
	@Package varchar(50),
	@IEID varchar(50)
AS
BEGIN

	-- If the IEID is given, use that
	if ( len( @IEID ) > 0 )
	begin

		-- Get the report data first
		select FdaReportID, Package=isnull(Package,''), IEID=isnull(IEID,''), FdaReportType, CONVERT(CHAR(10), Report_Date, 102) as Report_Date, Account=isnull(Account,''), 
			Project = isnull(Project,''), Warnings=isnull(Warnings,0), [Message]=isnull([Message],''), Database_Date 
		from FDA_Report R, FDA_Report_Type T
		where ( R.IEID = @IEID )
		  and ( T.FdaReportTypeID = R.FdaReportTypeID )

	end
	else -- Use Package ID
	begin
	
		-- Get the report data first
		select R.FdaReportID, Package=isnull(Package,''), IEID=isnull(IEID,''), FdaReportType, CONVERT(CHAR(10), Report_Date, 102) as Report_Date, Account=isnull(Account,''), 
			Project = isnull(Project,''), Warnings=isnull(Warnings,0), [Message]=isnull([Message],''), Database_Date 
		from FDA_Report R, FDA_Report_Type T
		where ( R.Package = @Package )
		  and ( T.FdaReportTypeID = R.FdaReportTypeID )

	end
END
GO
/****** Object:  StoredProcedure [dbo].[FDA_Get_Report_By_ID]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Sullivan, Mark
-- Create date: October 1, 2007
-- Description:	Get all the information about a single report
-- ==========================================================
CREATE PROCEDURE [dbo].[FDA_Get_Report_By_ID]
	@FdaReportID int
AS
BEGIN
	
	-- Get the report data first
	select Package=isnull(Package,''), IEID=isnull(IEID,''), FdaReportType, Report_Date, Account=isnull(Account,''), 
		Project = isnull(Project,''), Warnings=isnull(Warnings,0), [Message]=isnull([Message],''), Database_Date 
	from FDA_Report R, FDA_Report_Type T
	where ( R.FdaReportID = @FdaReportID )
	  and ( T.FdaReportTypeID = R.FdaReportTypeID )

END
GO
/****** Object:  StoredProcedure [dbo].[FDA_All_Reports_By_Date]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Sullivan, Mark
-- Create date: October 3, 2007
-- Description:	Retrieve all the FDA reports from the database
-- ==========================================================
CREATE PROCEDURE [dbo].[FDA_All_Reports_By_Date]
	@begindate datetime,
	@enddate datetime
AS
BEGIN
	
	-- Return all the reports
	select R.FdaReportID, Data_Index = -1, R.Package, IEID, FdaReportType, CONVERT(CHAR(10), Report_Date, 102) as ReportDate, Warnings, 
		[Message], FTP_Date = isnull( CONVERT(CHAR(10), L.FtpDate, 102), '')
	from FDA_Report R inner join
		 FDA_Report_Type AS T on ( R.FdaReportTypeID = T.FdaReportTypeID ) left outer join
		 FDA_FTP_Log AS L on ( R.Package = L.Package )
	where ( Report_Date >= @begindate ) and ( Report_Date <= @enddate) 
	order by R.Package

END
GO
/****** Object:  StoredProcedure [dbo].[FDA_All_Reports]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Sullivan, Mark
-- Create date: October 3, 2007
-- Description:	Retrieve all the FDA reports from the database
-- ==========================================================
CREATE PROCEDURE [dbo].[FDA_All_Reports]
AS
BEGIN
	
	-- Return all the reports
	select R.FdaReportID, Data_Index = -1, R.Package, IEID, FdaReportType, CONVERT(VARCHAR(10), Report_Date, 102) as ReportDate, Warnings, 
		[Message], FTP_Date = isnull( CONVERT(VARCHAR(10), L.FtpDate, 102), '')
	from FDA_Report R inner join
		FDA_Report_Type AS T on ( R.FdaReportTypeID = T.FdaReportTypeID ) left outer join
		FDA_FTP_Log AS L on ( R.Package = L.Package )
	order by R.Package

END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Add_User_Templates_Link]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a link between a user and a template 
CREATE PROCEDURE [dbo].[mySobek_Add_User_Templates_Link]
	@userid int,
	@template_default varchar(20),
	@template2 varchar(20),
	@template3 varchar(20),
	@template4 varchar(20),
	@template5 varchar(20)
AS
begin

	-- Add the default template
	if (( len(@template_default) > 0 ) and ( (select count(*) from mySobek_Template where TemplateCode = @template_default ) = 1 ))
	begin
		-- Clear any previous default
		update mySobek_User_Template_Link set DefaultTemplate='false' where UserID = @userid

		-- Get the id for this one
		declare @template_default_id int
		select @template_default_id = TemplateID from mySobek_Template where TemplateCode=@template_default

		-- Add this one as a default
		insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate )
		values ( @userid, @template_default_id, 'true' )
	end

	-- Add the second template
	if (( len(@template2) > 0 ) and ((select count(*) from mySobek_Template where TemplateCode = @template2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template2_id int
		select @template2_id = TemplateID from mySobek_Template where TemplateCode=@template2

		-- Add this one
		insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate )
		values ( @userid, @template2_id, 'false' )
	end

	-- Add the third template
	if (( len(@template3) > 0 ) and ((select count(*) from mySobek_Template where TemplateCode = @template3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template3_id int
		select @template3_id = TemplateID from mySobek_Template where TemplateCode=@template3

		-- Add this one
		insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate )
		values ( @userid, @template3_id, 'false' )
	end

	-- Add the fourth template
	if (( len(@template4) > 0 ) and ((select count(*) from mySobek_Template where TemplateCode = @template4 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template4_id int
		select @template4_id = TemplateID from mySobek_Template where TemplateCode=@template4

		-- Add this one
		insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate )
		values ( @userid, @template4_id, 'false' )
	end

	-- Add the fifth template
	if (( len(@template5) > 0 ) and ((select count(*) from mySobek_Template where TemplateCode = @template5 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template5_id int
		select @template5_id = TemplateID from mySobek_Template where TemplateCode=@template5

		-- Add this one
		insert into mySobek_User_Template_Link ( UserID, TemplateID, DefaultTemplate )
		values ( @userid, @template5_id, 'false' )
	end
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Add_User_Projects_Link]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a link between a user and a project 
CREATE PROCEDURE [dbo].[mySobek_Add_User_Projects_Link]
	@userid int,
	@project_default varchar(20),
	@project2 varchar(20),
	@project3 varchar(20),
	@project4 varchar(20),
	@project5 varchar(20)
AS
begin

	-- Add the default project
	if (( len(@project_default) > 0 ) and ( (select count(*) from mySobek_Project where ProjectCode = @project_default ) = 1 ))
	begin
		-- Clear any previous default
		update mySobek_User_Project_Link set DefaultProject='false' where UserID = @userid

		-- Get the id for this one
		declare @project_default_id int
		select @project_default_id = ProjectID from mySobek_Project where ProjectCode=@project_default

		-- Add this one as a default
		insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject )
		values ( @userid, @project_default_id, 'true' )
	end

	-- Add the second project
	if (( len(@project2) > 0 ) and ((select count(*) from mySobek_Project where ProjectCode = @project2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project2_id int
		select @project2_id = ProjectID from mySobek_Project where ProjectCode=@project2

		-- Add this one
		insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject )
		values ( @userid, @project2_id, 'false' )
	end

	-- Add the third project
	if (( len(@project3) > 0 ) and ((select count(*) from mySobek_Project where ProjectCode = @project3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project3_id int
		select @project3_id = ProjectID from mySobek_Project where ProjectCode=@project3

		-- Add this one
		insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject )
		values ( @userid, @project3_id, 'false' )
	end

	-- Add the fourth project
	if (( len(@project4) > 0 ) and ((select count(*) from mySobek_Project where ProjectCode = @project4 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project4_id int
		select @project4_id = ProjectID from mySobek_Project where ProjectCode=@project4

		-- Add this one
		insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject )
		values ( @userid, @project4_id, 'false' )
	end

	-- Add the fifth project
	if (( len(@project5) > 0 ) and ((select count(*) from mySobek_Project where ProjectCode = @project5 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project5_id int
		select @project5_id = ProjectID from mySobek_Project where ProjectCode=@project5

		-- Add this one
		insert into mySobek_User_Project_Link ( UserID, ProjectID, DefaultProject )
		values ( @userid, @project5_id, 'false' )
	end
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Add_User_Group_Templates_Link]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a link between a user and a template 
CREATE PROCEDURE [dbo].[mySobek_Add_User_Group_Templates_Link]
	@usergroupid int,
	@template1 varchar(20),
	@template2 varchar(20),
	@template3 varchar(20),
	@template4 varchar(20),
	@template5 varchar(20)
AS
begin

	-- Add the default template
	if (( len(@template1) > 0 ) and ( (select count(*) from mySobek_Template where TemplateCode = @template1 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template1_id int
		select @template1_id = TemplateID from mySobek_Template where TemplateCode=@template1

		-- Add this one as a default
		insert into mySobek_User_Group_Template_Link ( UserGroupID, TemplateID )
		values ( @usergroupid, @template1_id )
	end

	-- Add the second template
	if (( len(@template2) > 0 ) and ((select count(*) from mySobek_Template where TemplateCode = @template2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template2_id int
		select @template2_id = TemplateID from mySobek_Template where TemplateCode=@template2

		-- Add this one
		insert into mySobek_User_Group_Template_Link ( UserGroupID, TemplateID )
		values ( @usergroupid, @template2_id )
	end

	-- Add the third template
	if (( len(@template3) > 0 ) and ((select count(*) from mySobek_Template where TemplateCode = @template3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template3_id int
		select @template3_id = TemplateID from mySobek_Template where TemplateCode=@template3

		-- Add this one
		insert into mySobek_User_Group_Template_Link ( UserGroupID, TemplateID )
		values ( @usergroupid, @template3_id )
	end

	-- Add the fourth template
	if (( len(@template4) > 0 ) and ((select count(*) from mySobek_Template where TemplateCode = @template4 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template4_id int
		select @template4_id = TemplateID from mySobek_Template where TemplateCode=@template4

		-- Add this one
		insert into mySobek_User_Group_Template_Link ( UserGroupID, TemplateID )
		values ( @usergroupid, @template4_id )
	end

	-- Add the fifth template
	if (( len(@template5) > 0 ) and ((select count(*) from mySobek_Template where TemplateCode = @template5 ) = 1 ))
	begin
		-- Get the id for this one
		declare @template5_id int
		select @template5_id = TemplateID from mySobek_Template where TemplateCode=@template5

		-- Add this one
		insert into mySobek_User_Group_Template_Link ( UserGroupID, TemplateID )
		values ( @usergroupid, @template5_id )
	end
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Add_User_Group_Projects_Link]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a link between a user and a project 
CREATE PROCEDURE [dbo].[mySobek_Add_User_Group_Projects_Link]
	@usergroupid int,
	@project1 varchar(20),
	@project2 varchar(20),
	@project3 varchar(20),
	@project4 varchar(20),
	@project5 varchar(20)
AS
begin

	-- Add the default project
	if (( len(@project1) > 0 ) and ( (select count(*) from mySobek_Project where ProjectCode = @project1 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project1_id int
		select @project1_id = ProjectID from mySobek_Project where ProjectCode=@project1

		-- Add this one as a default
		insert into mySobek_User_Group_Project_Link ( UserGroupID, ProjectID )
		values ( @usergroupid, @project1_id )
	end

	-- Add the second project
	if (( len(@project2) > 0 ) and ((select count(*) from mySobek_Project where ProjectCode = @project2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project2_id int
		select @project2_id = ProjectID from mySobek_Project where ProjectCode=@project2

		-- Add this one
		insert into mySobek_User_Group_Project_Link ( UserGroupID, ProjectID )
		values ( @usergroupid, @project2_id )
	end

	-- Add the third project
	if (( len(@project3) > 0 ) and ((select count(*) from mySobek_Project where ProjectCode = @project3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project3_id int
		select @project3_id = ProjectID from mySobek_Project where ProjectCode=@project3

		-- Add this one
		insert into mySobek_User_Group_Project_Link ( UserGroupID, ProjectID )
		values ( @usergroupid, @project3_id )
	end

	-- Add the fourth project
	if (( len(@project4) > 0 ) and ((select count(*) from mySobek_Project where ProjectCode = @project4 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project4_id int
		select @project4_id = ProjectID from mySobek_Project where ProjectCode=@project4

		-- Add this one
		insert into mySobek_User_Group_Project_Link ( UserGroupID, ProjectID )
		values ( @usergroupid, @project4_id )
	end

	-- Add the fifth project
	if (( len(@project5) > 0 ) and ((select count(*) from mySobek_Project where ProjectCode = @project5 ) = 1 ))
	begin
		-- Get the id for this one
		declare @project5_id int
		select @project5_id = ProjectID from mySobek_Project where ProjectCode=@project5

		-- Add this one
		insert into mySobek_User_Group_Project_Link ( UserGroupID, ProjectID )
		values ( @usergroupid, @project5_id )
	end
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Add_User_Group_Aggregations_Link]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to add links between a user group and item aggregations
CREATE PROCEDURE [dbo].[mySobek_Add_User_Group_Aggregations_Link]
	@usergroupid int,
	@aggregationcode1 varchar(20),
	@canselect1 bit,
	@canedit1 bit,
	@iscurator1 bit,
	@aggregationcode2 varchar(20),
	@canselect2 bit,
	@canedit2 bit,
	@iscurator2 bit,	
	@aggregationcode3 varchar(20),
	@canselect3 bit,
	@canedit3 bit,
	@iscurator3 bit
AS
BEGIN
	
	-- Add the first aggregation
	if (( len(@aggregationcode1) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode1 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation1_id int
		select @aggregation1_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode1

		-- Add this one
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditItems, IsCurator )
		values ( @usergroupid, @aggregation1_id, @canselect1, @canedit1, @iscurator1 )
	end

	-- Add the second aggregation
	if (( len(@aggregationcode2) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode2 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation2_id int
		select @aggregation2_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode2

		-- Add this one
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditItems, IsCurator )
		values ( @usergroupid, @aggregation2_id, @canselect2, @canedit2, @iscurator2 )
	end

	-- Add the third aggregation
	if (( len(@aggregationcode3) > 0 ) and ((select count(*) from SobekCM_Item_Aggregation where Code=@aggregationcode3 ) = 1 ))
	begin
		-- Get the id for this one
		declare @aggregation3_id int
		select @aggregation3_id = AggregationID from SobekCM_Item_Aggregation where Code=@aggregationcode3

		-- Add this one
		insert into mySobek_User_Group_Edit_Aggregation ( UserGroupID, AggregationID, CanSelect, CanEditItems, IsCurator )
		values ( @usergroupid, @aggregation3_id, @canselect3, @canedit3, @iscurator3 )
	end
END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Serial_Hierarchy]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Adds the link between the item and the group and also adds the serial hierarchy
-- Stored procedure written by Mark Sullivan ( September 2006 )
CREATE PROCEDURE [dbo].[SobekCM_Save_Serial_Hierarchy]
	@GroupID int,
	@ItemID int,
	@Level1_Text varchar(255),
	@Level1_Index int,
	@Level2_Text varchar(255),
	@Level2_Index int,
	@Level3_Text varchar(255),
	@Level3_Index int,
	@Level4_Text varchar(255),
	@Level4_Index int,
	@Level5_Text varchar(255),
	@Level5_Index int,
	@SerialHierarchy varchar(500)
AS
begin transaction

	update SobekCM_Item
	set Level1_Text = @Level1_Text, Level1_Index = @Level1_Index, 
		Level2_Text = @Level2_Text, Level2_Index = @Level2_Index,
		Level3_Text = @Level3_Text, Level3_Index = @Level3_Index, 
		Level4_Text = @Level4_Text, Level4_Index = @Level4_Index,
		Level5_Text = @Level5_Text, Level5_Index = @Level5_Index
	where ItemID=@ItemID

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_VRACore_Extensions]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Save the metadata from the VRACore metadata extension
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_VRACore_Extensions]
	@ItemID int,
	@Material_Display nvarchar(1000),
	@Measurement_Display nvarchar(1000), 
	@StylePeriod_Display nvarchar(1000), 
	@Technique_Display nvarchar(1000)
AS
BEGIN
	-- Update existing row
	update SobekCM_Item
	set Material_Display=@Material_Display, Measurement_Display=@Measurement_Display,
	    StylePeriod_Display=@StylePeriod_Display, Technique_Display=@Technique_Display
	where ItemID=@ItemID;
	
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Set_Main_Thumbnail]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Set the main thumbnail for an individual item 
CREATE PROCEDURE [dbo].[SobekCM_Set_Main_Thumbnail]
	@bibid varchar(10),
	@vid varchar(5),
	@mainthumb varchar(100)
AS
BEGIN

	-- Get the item id
	declare @itemid int;
	set @itemid = ISNULL(( select ItemID from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID=@bibid and I.VID=@vid ), -1 );
	
	-- Set the main thumb
	if ( @itemid > 0 )
	begin
		update SobekCM_Item set MainThumbnail=@mainthumb where ItemID=@itemid;
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Set_Item_Comments]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure sets the internal comments for an item
CREATE PROCEDURE [dbo].[SobekCM_Set_Item_Comments]
	@itemid int,
	@newcomments nvarchar(1000)
AS
begin

	-- Update the item table
	update SobekCM_Item
	set Internal_Comments=@newcomments
	where ItemID = @itemid;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Aggregation_Hierarchy_Link]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Stored procedure to link between item aggregations
-- Written by Ying, modified by Mark ( 2/2007 )
-- This stored procedure is used by the Customization Manager
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Aggregation_Hierarchy_Link]
	@collectionid int,
	@groupid int
AS
begin transaction

   -- Verify there is not already a link here
   declare  @thiscount int
   select @thiscount = count(*)
   from SobekCM_Item_Aggregation_Hierarchy
   where (ParentID = @groupid) and (ChildID = @collectionid)

   -- if there was no link, add one
   if (@thiscount  <= 0)
   begin 
       insert into SobekCM_Item_Aggregation_Hierarchy(ParentID, ChildID)
       values(@groupid, @collectionid)
   end
  
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Aggregation_Alias]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure either adds a forwarding or edits an existing forward
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Aggregation_Alias]
	@alias varchar(50),
	@aggregation_code varchar(20)	
AS
BEGIN
	
	-- Get the aggregation id from the aggregation code
	if (( select count(*) from SobekCM_Item_Aggregation where Code=@aggregation_code and Deleted='false' ) = 1 )
	begin
		-- Get the aggregation id
		declare @aggregationid int;
		select @aggregationid = AggregationID from SobekCM_Item_Aggregation where Code=@aggregation_code;

		-- Does this alias already exist?
		if (( select count(*) from SobekCM_Item_Aggregation_Alias where AggregationAlias=@alias ) > 0 )
		begin
			-- Update existing
			update SobekCM_Item_Aggregation_Alias
			set AggregationID = @aggregationID
			where AggregationAlias = @alias;
		end
		else
		begin
			-- Not existing, so add new one
			insert into SobekCM_Item_Aggregation_Alias ( AggregationAlias, AggregationID )
			values ( @alias, @aggregationid );
		end;
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Aggregation]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Stored procedure to save the basic item aggregation information
-- Version 3.05 - Added check to exclude DELETED aggregations
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Aggregation]
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
		Description = @description,
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
		Deleted = 'false'
      where AggregationID = @aggregationid or Code = @code;

      -- Set the return value to the existing id
      set @newaggregationid = @aggregationid;
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
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Group_Web_Skins]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves all the web skin data about a group of items in UFDC
-- Written by Mark Sullivan (September 2006, Modified August 2010 )
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Group_Web_Skins]
	@GroupID int,
	@Primary_WebSkin varchar(20),
	@Alt_WebSkin1 varchar(20),
	@Alt_WebSkin2 varchar(20),
	@Alt_WebSkin3 varchar(20),
	@Alt_WebSkin4 varchar(20),
	@Alt_WebSkin5 varchar(20),
	@Alt_WebSkin6 varchar(20),
	@Alt_WebSkin7 varchar(20),
	@Alt_WebSkin8 varchar(20),
	@Alt_WebSkin9 varchar(20)	
AS
begin transaction

	-- Clear existing web skins
	delete from SobekCM_Item_Group_Web_Skin_Link 
	where GroupID = @GroupID

	-- Add the first web skin to this object  (this requires the web skins have been pre-established )
	declare @InterfaceID int
	if ( len( isnull( @Primary_WebSkin, '' )) > 0 ) 
	begin
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Primary_WebSkin

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 1 )
		end
	end

	-- Add the second web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin1, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin1

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 2 )
		end
	end

	-- Add the third web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin2, '' )) > 0 ) 
	begin		
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin2

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 3 )
		end
	end

	-- Add the fourth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin3, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin3

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 4 )
		end
	end
	
	-- Add the fifth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin4, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin4

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 5 )
		end
	end

	-- Add the sixth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin5, '' )) > 0 ) 
	begin		
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin5

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 6 )
		end
	end

	-- Add the seventh web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin6, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin6

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 7 )
		end
	end

-- Add the eight web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin7, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin7

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 8 )
		end
	end

	-- Add the ninth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin8, '' )) > 0 ) 
	begin		
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin8

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 9 )
		end
	end

	-- Add the tenth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin9, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin9

		-- Ensure this web skin exists
		if ( ISNULL(@InterfaceID,-1) > 0 )
		begin		
			-- Tie this item to this interface
			insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
			values ( @GroupID, @InterfaceID, 10 )
		end
	end		
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Group_Behaviors]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves the behavior information about an item group in this library
-- Written by Mark Sullivan 
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Group_Behaviors]
	@GroupID int,
	@GroupTitle nvarchar(500),
	@SortTitle varchar(500),
	@GroupThumbnail varchar(500),
	@Large_Format bit,
	@Primary_WebSkin varchar(20),
	@Alt_WebSkin1 varchar(20),
	@Alt_WebSkin2 varchar(20),
	@Alt_WebSkin3 varchar(20),
	@Alt_WebSkin4 varchar(20),
	@Alt_WebSkin5 varchar(20),
	@Alt_WebSkin6 varchar(20),
	@Alt_WebSkin7 varchar(20),
	@Alt_WebSkin8 varchar(20),
	@Alt_WebSkin9 varchar(20),
	@Viewer1_TypeID int,
	@Viewer1_Label nvarchar(50),
	@Viewer1_Attribute nvarchar(250),
	@Viewer2_TypeID int,
	@Viewer2_Label nvarchar(50),
	@Viewer2_Attribute nvarchar(250),
	@Viewer3_TypeID int,
	@Viewer3_Label nvarchar(50),
	@Viewer3_Attribute nvarchar(250),
	@Viewer4_TypeID int,
	@Viewer4_Label nvarchar(50),
	@Viewer4_Attribute nvarchar(250),
	@Viewer5_TypeID int,
	@Viewer5_Label nvarchar(50),
	@Viewer5_Attribute nvarchar(250),
	@Viewer6_TypeID int,
	@Viewer6_Label nvarchar(50),
	@Viewer6_Attribute nvarchar(250)
AS
begin transaction

	-- Update the basic information about this item group
	update SobekCM_Item_Group
	set GroupTitle = @GroupTitle, SortTitle = @SortTitle, GroupThumbnail=@GroupThumbnail, Large_Format=@Large_Format
	where GroupID=@GroupID
	
	-- Clear existing web skins
	delete from SobekCM_Item_Group_Web_Skin_Link 
	where GroupID = @GroupID

	-- Add the first web skin to this object  (this requires the web skins have been pre-established )
	declare @InterfaceID int
	if ( len( isnull( @Primary_WebSkin, '' )) > 0 ) 
	begin
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Primary_WebSkin

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 1 )
	end

	-- Add the second web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin1, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin1

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 2 )
	end

	-- Add the third web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin2, '' )) > 0 ) 
	begin		
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin2

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 3 )
	end

	-- Add the fourth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin3, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin3

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 4 )
	end
	
	-- Add the fifth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin4, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin4

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 2 )
	end

	-- Add the sixth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin5, '' )) > 0 ) 
	begin		
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin5

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 3 )
	end

	-- Add the seventh web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin6, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin6

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 4 )
	end

-- Add the eight web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin7, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin7

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 2 )
	end

	-- Add the ninth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin8, '' )) > 0 ) 
	begin		
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin8

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 3 )
	end

	-- Add the tenth web skin to this object  (this requires the web skins have been pre-established )
	if ( len( isnull( @Alt_WebSkin9, '' )) > 0 ) 
	begin	
		-- Get the Interface ID for this interface
		select @InterfaceID = WebSkinID from SobekCM_Web_Skin where WebSkinCode = @Alt_WebSkin9

		-- Tie this item to this interface
		insert into SobekCM_Item_Group_Web_Skin_Link ( GroupID, WebSkinID, [Sequence] )
		values ( @GroupID, @InterfaceID, 4 )
	end	
	
	-- Clear the links to all existing viewers
	delete from SobekCM_Item_Group_Viewers where GroupID=@GroupID
	
	-- Add the first viewer information
	if ( @Viewer1_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Group_Viewers ( GroupID, ItemGroupViewTypeID, Attribute, Label )
		values ( @GroupID, @Viewer1_TypeID, @Viewer1_Attribute, @Viewer1_Label )
	end
	
	-- Add the second viewer information
	if ( @Viewer2_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Group_Viewers ( GroupID, ItemGroupViewTypeID, Attribute, Label )
		values ( @GroupID, @Viewer2_TypeID, @Viewer2_Attribute, @Viewer2_Label )
	end
	
	-- Add the third viewer information
	if ( @Viewer3_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Group_Viewers ( GroupID, ItemGroupViewTypeID, Attribute, Label )
		values ( @GroupID, @Viewer3_TypeID, @Viewer3_Attribute, @Viewer3_Label )
	end
	
	-- Add the fourth viewer information
	if ( @Viewer4_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Group_Viewers ( GroupID, ItemGroupViewTypeID, Attribute, Label )
		values ( @GroupID, @Viewer4_TypeID, @Viewer4_Attribute, @Viewer4_Label )
	end
	
	-- Add the fifth viewer information
	if ( @Viewer5_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Group_Viewers ( GroupID, ItemGroupViewTypeID, Attribute, Label )
		values ( @GroupID, @Viewer5_TypeID, @Viewer5_Attribute, @Viewer5_Label )
	end
	
	-- Add the first viewer information
	if ( @Viewer6_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Group_Viewers ( GroupID, ItemGroupViewTypeID, Attribute, Label )
		values ( @GroupID, @Viewer6_TypeID, @Viewer6_Attribute, @Viewer6_Label )
	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Manager_Newspapers_Without_Serial_Info]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns basic information about newspaper titles which lack some
-- serial information and should be processed somehow.
-- This is generally used from the SMaRT tool right now, but will likely
-- be added to the web interface shortly.
CREATE PROCEDURE [dbo].[SobekCM_Manager_Newspapers_Without_Serial_Info]
as
begin

	-- Return the bibid's and group title for titles which have items lacking
	-- serial information
	select distinct(G.BibID), G.GroupTitle
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.GroupID = G.GroupID )
	  and ( G.Type like '%Newspaper%' )
      and (( len( isnull(Level1_Text,'')) = 0 ) or ( Level1_Index <= 0 ) or ( LEN( ISNULL( Level2_Text,'')) = 0 )or ( Level2_Index <= 0 ))
      and ( G.ItemCount > 1 );
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Multiple_Volumes]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Stored procedure returns the information about all the items within a single 
-- title or item/group
CREATE PROCEDURE [dbo].[SobekCM_Get_Multiple_Volumes] 
	@bibid varchar(10)
AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return the individual volumes
	select I.ItemID, Title, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), I.MainThumbnail, I.VID, I.IP_Restriction_Mask
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( G.GroupID = I.GroupID )
	  and ( G.BibID = @bibid )
	  and ( I.Deleted = 'false' )
	  and ( G.Deleted = 'false' )
	order by Level1_Index ASC, Level2_Index ASC, Level3_Index ASC, Level4_Index ASC, Level5_Index ASC, Title ASC

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Page_Item_Count_History]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return the item and page count added each month
CREATE PROCEDURE [dbo].[SobekCM_Page_Item_Count_History]
as
begin

select ItemID, YEAR(Milestone_OnlineComplete) as [Year], MONTH(Milestone_OnlineComplete) as [Month], [PageCount]
into #TEMP1
from SobekCM_Item I
where I.Deleted = 'false'

select [Year], [MONTH], Total = SUM( [PageCount] ), ItemCount = COUNT(*)
from #TEMP1
group by [Year], [Month]
order by [Year], [Month]

drop table #TEMP1

end
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Item_List_Web]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets a very abbreviated list of items and groups which exist for the web application
-- This limits the values returned for speed and memory optimization
CREATE PROCEDURE [dbo].[SobekCM_Item_List_Web]
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
	select G.BibID, I.VID, IP_Restriction_Mask, I.Title
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.GroupID = G.GroupID )
	  and ( G.Deleted = CONVERT(bit,0) )
	  and ( I.Deleted = CONVERT(bit,0) )
	  and ( I.IP_Restriction_Mask >= @lower_mask )
	order by BibID, VID;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Item_List_Brief2]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns the list of all items within the library with some very basic information.
-- This is primarily utilized by the builder to step through all items in the library
-- and build the marc files, or links for the sitemap
CREATE PROCEDURE [dbo].[SobekCM_Item_List_Brief2]
	@include_private bit
as
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	if ( @include_private = 'true' )
	begin
		-- Return the item group / item information in one large table
		select G.BibID, I.VID, G.GroupTitle, 
			isnull(I.Level1_Text, '') as Level1_Text, isnull( I.Level1_Index, 0 ) as Level1_Index, 
			isnull(I.Level2_Text, '') as Level2_Text, isnull( I.Level2_Index, 0 ) as Level2_Index, 
			isnull(I.Level3_Text, '') as Level3_Text, isnull( I.Level3_Index, 0 ) as Level3_Index, 
			PubDate=isnull(I.PubDate,''), SortDate=isnull( I.SortDate,-1), MainThumbnail=G.File_Location + '/' + VID + '/' + isnull( I.MainThumbnail,''), 
			I.Title, Author=isnull(I.Author,''), IP_Restriction_Mask, G.OCLC_Number, G.ALEPH_Number, I.LastSaved, I.AggregationCodes, G.Large_Format
		from SobekCM_Item I, SobekCM_Item_Group G
		where ( I.GroupID = G.GroupID )
		  and ( G.Deleted = CONVERT(bit,0) )
		  and ( I.Deleted = CONVERT(bit,0) );

		-- Get the items that are really multiple (having more than one volume)
		select G.BibID, G.GroupID, VID_COUNT = ItemCount, G.GroupTitle, G.[Type], G.File_Location, SortTitle=isnull(G.SortTitle, G.GroupTitle), G.OCLC_Number, G.ALEPH_Number
		from SobekCM_Item_Group G;
	end
	else
	begin
			-- Return the item group / item information in one large table
		select G.BibID, I.VID, G.GroupTitle, 
			isnull(I.Level1_Text, '') as Level1_Text, isnull( I.Level1_Index, 0 ) as Level1_Index, 
			isnull(I.Level2_Text, '') as Level2_Text, isnull( I.Level2_Index, 0 ) as Level2_Index, 
			isnull(I.Level3_Text, '') as Level3_Text, isnull( I.Level3_Index, 0 ) as Level3_Index, 
			PubDate=isnull(I.PubDate,''), SortDate=isnull( I.SortDate,-1), MainThumbnail=G.File_Location + '/' + VID + '/' + isnull( I.MainThumbnail,''), 
			I.Title, Author=isnull(I.Author,''), IP_Restriction_Mask, G.OCLC_Number, G.ALEPH_Number, I.LastSaved, I.AggregationCodes, G.Large_Format
		from SobekCM_Item I, SobekCM_Item_Group G
		where ( I.GroupID = G.GroupID )
		  and ( G.Deleted = CONVERT(bit,0) )
		  and ( I.Deleted = CONVERT(bit,0) )
		  and ( I.IP_Restriction_Mask >= 0 );
		  
		-- Get the list of groups which have at least one item non-private
		select distinct(GroupID) as GroupID
		into #TEMP1
		from SobekCM_Item I
		where ( I.Deleted = CONVERT(bit,0) )
		  and ( I.IP_Restriction_Mask >= 0 ); 
		  
		-- Get the items that are really multiple (having more than one volume)
		select G.BibID, G.GroupID, VID_COUNT = ItemCount, G.GroupTitle, G.[Type], G.File_Location, SortTitle=isnull(G.SortTitle, G.GroupTitle), G.OCLC_Number, G.ALEPH_Number
		from SobekCM_Item_Group G, #TEMP1 T
		where T.GroupID = G.GroupID and G.Deleted = CONVERT(bit,0);
		
		-- Drop the temporary table
		drop table #TEMP1;
		
		-- Get list of items / groups which are private
		select G.BibID, I.VID
		from SobekCM_Item I, SobekCM_Item_Group G
		where ( I.GroupID = G.GroupID )
		  and ( G.Deleted = CONVERT(bit,0) )
		  and ( I.Deleted = CONVERT(bit,0) )
		  and ( I.IP_Restriction_Mask < 0 );
	end;	
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Items_By_OCLC]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of items by OCLC number
CREATE PROCEDURE [dbo].[SobekCM_Items_By_OCLC] 
	@oclc_number bigint
AS
BEGIN
	
	-- Return the item informaiton
	select BibID, VID, SortDate, Spatial_KML, fk_TitleID = I.GroupID, Title 
	from SobekCM_Item I, SobekCM_Item_Group G
	where I.GroupID = G.GroupID 
	  and G.OCLC_Number = @oclc_number
	order by BibID ASC, VID ASC

	-- Return the title information
	select G.BibID, G.SortTitle, TitleID=G.GroupID, [Rank]=-1
	from SobekCM_Item_Group G
	where  G.OCLC_Number = @oclc_number
	order by BibID ASC
	
END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Items_By_ALEPH]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of items by ALEPH number
CREATE PROCEDURE [dbo].[SobekCM_Items_By_ALEPH] 
	@aleph_number int
AS
BEGIN
	
	-- Return the item informaiton
	select BibID, VID, SortDate, Spatial_KML, fk_TitleID = I.GroupID, Title 
	from SobekCM_Item I, SobekCM_Item_Group G
	where I.GroupID = G.GroupID 
	  and G.ALEPH_Number = @aleph_number
	order by BibID ASC, VID ASC

	-- Return the title information
	select G.BibID, G.SortTitle, TitleID=G.GroupID, [Rank]=-1
	from SobekCM_Item_Group G
	where  G.ALEPH_Number = @aleph_number
	order by BibID ASC
	
END
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Image_Processing_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Marks this volume image processing complete
-- Written by Mark Sullivan 
CREATE PROCEDURE [dbo].[Tracking_Image_Processing_Complete]
	@bibid varchar(10),
	@vid varchar(5),
	@user varchar(255),
	@storagelocation varchar(255),
	@date datetime
AS
begin transaction

	-- Get the volume id
	declare @itemid int
	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid)

	-- continue if a volumeid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Update the milestones
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, @date),
		    Milestone_ImageProcessing = ISNULL(Milestone_ImageProcessing, @date)
		where ItemID=@itemid
	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Get_Multiple_Volumes]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Stored procedure returns the information about all the items within a single 
-- title or item/group
-- Written by Mark Sullivan ( November 2006 )
CREATE PROCEDURE [dbo].[Tracking_Get_Multiple_Volumes] 
	@bibid varchar(10)
AS
begin transaction

		-- Return the individual volumes
		select I.ItemID, Title, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), I.MainThumbnail, I.VID, I.IP_Restriction_Mask, I.Author, I.Publisher, I.AggregationCodes, I.Tracking_Box, I.Born_Digital, I.Material_Received_Date, I.Material_Recd_Date_Estimated, I.Disposition_Advice, I.Disposition_Advice_Notes, I.Disposition_Type, I.Disposition_Date, I.Disposition_Notes, PubDate, SortDate, I.SortTitle, I.Last_MileStone, I.Remotely_Archived, I.Locally_Archived
		from SobekCM_Item I, SobekCM_Item_Group G
		where ( G.GroupID = I.GroupID )
		  and ( G.BibID = @bibid )
		  and ( I.Deleted = 'false' )
		  and ( G.Deleted = 'false' )
		order by Level1_Index ASC, Level2_Index ASC, Level3_Index ASC, Level4_Index ASC, Level5_Index ASC, I.SortTitle ASC;

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Items_Pending_Online_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Items_Pending_Online_Complete]
AS
begin

	select  G.BibID, I.VID, I.Milestone_QualityControl, I.Dark, I.IP_Restriction_Mask, I.[PageCount], I.Title, G.[Type], I.AggregationCodes
	from SobekCM_Item as I inner join
		 SobekCM_Item_Group as G on I.GroupID=G.GroupID 
	where Milestone_QualityControl is not null
	 and Milestone_OnlineComplete is null
	 and Dark = 'false'
	and Milestone_QualityControl > '1-1-2011'
	order by Milestone_QualityControl DESC
end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Items_By_OCLC]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of items by OCLC number
-- This is identical to SobekCM_Items_By_OCLC, EXCEPT:
--    (1) Title, Author, IP restriction flag, Thumbnail, and Publisher returned with item list
--    (2) Group title, thumbnail, type, OCLC, and ALEPH returned with title list
CREATE PROCEDURE [dbo].[Tracking_Items_By_OCLC] 
	@oclc_number bigint
AS
BEGIN
	
	-- Return the item informaiton
	select BibID, VID, SortDate, Spatial_KML, fk_TitleID = I.GroupID, Title, Author, Publisher, IP_Restriction_Mask, MainThumbnail as Thumbnail
	from SobekCM_Item I, SobekCM_Item_Group G
	where I.GroupID = G.GroupID 
	  and G.OCLC_Number = @oclc_number
	order by BibID ASC, VID ASC

	-- Return the title information
	select G.BibID, G.SortTitle, TitleID=G.GroupID, [Rank]=-1, G.GroupTitle, G.GroupThumbnail, G.[Type], G.ALEPH_Number, G.OCLC_Number
	from SobekCM_Item_Group G
	where  G.OCLC_Number = @oclc_number
	order by BibID ASC
	
END
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Items_By_ALEPH]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of items by ALEPH number
-- This is identical to SobekCM_Items_By_ALEPH, EXCEPT:
--    (1) Title, Author, IP restriction flag, Thumbnail, and Publisher returned with item list
--    (2) Group title, thumbnail, type, OCLC, and ALEPH returned with title list
CREATE PROCEDURE [dbo].[Tracking_Items_By_ALEPH] 
	@aleph_number int
AS
BEGIN
	
	-- Return the item informaiton
	select BibID, VID, SortDate, Spatial_KML, fk_TitleID = I.GroupID, Title, Author, Publisher, IP_Restriction_Mask, MainThumbnail as Thumbnail
	from SobekCM_Item I, SobekCM_Item_Group G
	where I.GroupID = G.GroupID 
	  and G.ALEPH_Number = @aleph_number
	order by BibID ASC, VID ASC

	-- Return the title information
	select G.BibID, G.SortTitle, TitleID=G.GroupID, [Rank]=-1, G.GroupTitle, G.GroupThumbnail, G.[Type], G.ALEPH_Number, G.OCLC_Number
	from SobekCM_Item_Group G
	where  G.ALEPH_Number = @aleph_number
	order by BibID ASC
	
END
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Digital_Acquisition_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Marks this volume image processing complete
-- Written by Mark Sullivan 
CREATE PROCEDURE [dbo].[Tracking_Digital_Acquisition_Complete]
	@bibid varchar(10),
	@vid varchar(5),
	@user varchar(255),
	@storagelocation varchar(255),
	@date datetime
AS
begin transaction

	-- Get the volume id
	declare @itemid int
	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid)

	-- continue if a volumeid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Update the milestones
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, @date)
		where ItemID=@itemid
	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Box_List]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Box_List]
AS
begin
	select distinct(Tracking_Box) 
	from SobekCM_Item
	order by Tracking_Box
end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Born_Digital_Item_Count]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure returns the count of items added between two arbitraty months
-- and includes born digital numbers
CREATE PROCEDURE [dbo].[Tracking_Born_Digital_Item_Count]
	@startdate date,
	@enddate date
AS
BEGIN

	-- Create the table for the statistics
	create table #TEMP_RESULTS ( RowName varchar(20), Titles_Impacted int, Items_Completed int, Pages_Completed int, Files_Completed int )

	-- First collect the raw born digital
	select I.GroupID, I.ItemID, I.[PageCount], I.FileCount
	into #TEMP_BORNDIGITAL
	from SobekCM_Item I
	where ( I.Milestone_OnlineComplete >= @startdate ) 
	  and ( I.Milestone_OnlineComplete < @enddate )
	  and ( Born_Digital = 'true' )
	  
	-- Add the born digital stats into the table
	insert into #TEMP_RESULTS
	select 'BORN_DIGITAL', COUNT(distinct(GroupID)), COUNT(*), SUM([PageCount]), SUM([FileCount])
	from #TEMP_BORNDIGITAL

	-- Drop the born digital table
	drop table #TEMP_BORNDIGITAL

	-- Next collect the total information
	select I.GroupID, I.ItemID, I.[PageCount], I.FileCount
	into #TEMP_RAWDATA
	from SobekCM_Item I
	where ( I.Milestone_OnlineComplete >= @startdate ) and ( I.Milestone_OnlineComplete < @enddate )

	-- Add the born digital stats into the table
	insert into #TEMP_RESULTS
	select 'TOTAL', COUNT(distinct(GroupID)), COUNT(*), SUM([PageCount]), SUM([FileCount])
	from #TEMP_RAWDATA

	-- Return all results
	select * from #TEMP_RESULTS

	-- Drop the temporary tables
	drop table #TEMP_RAWDATA
	drop table #TEMP_RESULTS
END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Web_Skin_Portal_Add]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a link between a url portal and a web skin, also indicating
-- if this addition is the default web skin
CREATE PROCEDURE [dbo].[SobekCM_Web_Skin_Portal_Add]
	@PortalID int,
	@Skin_Code nvarchar(20),
	@isDefault bit
AS
BEGIN
	-- Ensure the web skin exists
	if ( @Skin_Code in ( select WebSkinCode from SobekCM_Web_Skin ))
	begin
		-- Get the web skin primary key
		declare @skinid int;
		set @skinid=(select top 1 WebSkinCode from SobekCM_Web_Skin where WebSkinCode=@Skin_Code);
		
		-- Does this link already exist?
		if exists ( select * from SobekCM_Portal_Web_Skin_Link where WebSkinID=@skinid and PortalID=@PortalID )
		begin
			-- Just update the default value
			update SobekCM_Portal_Web_Skin_Link
			set isDefault=@isDefault
			where WebSkinID=@skinid and PortalID=@PortalID;
		end
		else
		begin
			-- Insert a new link between this portal and web skin
			insert into SobekCM_Portal_Web_Skin_Link ( PortalID, WebSkinID, isDefault )
			values ( @PortalID, @skinid, @isDefault );
		end;		
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Update_Additional_Work_Needed_Flag]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Updates the 'additional work needed' flag for an item, which tells
-- the builder that it should be post-processed.
CREATE procedure [dbo].[SobekCM_Update_Additional_Work_Needed_Flag] 
	@itemid int,
	@newflag bit
as
begin
	update SobekCM_Item set AdditionalWorkNeeded=@newflag where ItemID=@itemid;
end;
GO
/****** Object:  StoredProcedure [dbo].[Tivoli_Admin_Update]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure can be run weekly or monthly and performs some basic cleanup functions
-- on the Tivoli information.  First, it ensures that any files that were backed up
-- by BibID/VID before being in the system are now correctly linked to the SobekCM_Item
-- table.  Second, it updates the Tivoli size for each item that has had files backed
-- up to Tivoli since the last time this was run.
CREATE PROCEDURE [dbo].[Tivoli_Admin_Update]
AS
BEGIN

	-- First, ensure all tivoli file logs are linked to bibid/vid
	update Tivoli_File_Log
	set ItemID=( select ItemID from SobekCM_Item_Group G, SobekCM_Item I where I.GroupID=G.GroupID and I.VID=Tivoli_File_Log.VID and G.BibID=Tivoli_File_Log.BibID )
	where exists ( select ItemID from SobekCM_Item_Group G, SobekCM_Item I where I.GroupID=G.GroupID and I.VID=Tivoli_File_Log.VID and G.BibID=Tivoli_File_Log.BibID )
	  and ItemID <= 0;
	  
	-- Determine the most recent date that Tivoli was performed
	declare @lastrun datetime;
	select @lastrun = MAX(TivoliSize_Calculated) from SobekCM_Item;
	
	-- Get the itemids that have been affected in Tivoli since then
	select distinct(ItemID)
	into #TEMP_ITEMS
	from Tivoli_File_Log 
	where ArchiveDate >= @lastrun
	  and ItemID > 0;
	  
	-- Using the same update date should make it easier in the future
	declare @today datetime;
	set @today=GETDATE();
  
	-- Now, update the size tivoli'd for the items
	update SobekCM_Item 
	set TivoliSize_MB=isnull((select SUM(Size/1024) from Tivoli_File_Log L where L.ItemID=SobekCM_Item.ItemID ), 0 ),
	    TivoliSize_Calculated = @today
	where ItemID in ( select * from #TEMP_ITEMS );

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Update_Item_Online_Statistics]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure updates the information about page count, file count, and disk 
-- size for the online files.
CREATE PROCEDURE [dbo].[SobekCM_Update_Item_Online_Statistics]
	@bibid varchar(10),
	@vid varchar(5),
	@pagecount int,
	@filecount int,
	@disksize_mb bigint
AS
begin
	-- Get the item id
	declare @itemid int
	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid);

	-- Now, update the item row
	update SobekCM_Item
	set [PageCount]=@pagecount, FileCount=@filecount, DiskSize_MB=@disksize_mb
	where ItemID=@itemid;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Statistics_Lookup_Tables]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns the lookup tables for assembling the statistics information
CREATE PROCEDURE [dbo].[SobekCM_Statistics_Lookup_Tables]
AS
BEGIN
	
	-- No need to perform any locks here.
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Return the item id
	select I.ItemID, G.BibID, I.VID
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.GroupID = G.GroupID );

	-- Return the group id
	select G.GroupID, G.BibID
	from SobekCM_Item_Group G;

	-- Return the aggregation ids
	select S.AggregationID, S.Code, S.[Type]
	from SobekCM_Item_Aggregation S;
	
	-- Return the portal ids
	select P.PortalID, P.Base_URL, P.Abbreviation, P.isDefault
	from SobekCM_Portal_URL P
	where P.isActive = 'true';

END;
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Update_Disposition_Advice]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Update_Disposition_Advice]
	@Disposition_Advice int,
	@Disposition_Advice_Notes varchar(150),
	@ItemID int
AS
begin

	-- Update the item row
	update SobekCM_Item set Disposition_Advice = @Disposition_Advice, Disposition_Advice_Notes = @Disposition_Advice_Notes where ItemID = @ItemID

end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Update_Physical_Milestones]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Update_Physical_Milestones] 
	@bibid varchar(10),
	@vid varchar(5),
	@tracking_box varchar(25),
	@born_digital bit,
	@material_received_date datetime,
	@material_recd_date_estimated bit,
	@disposition_advice int,
	@disposition_date datetime,
	@disposition_type int
AS
begin

	-- Get the item id
	declare @itemid int
	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid)

	-- Now, update the item row
	update SobekCM_Item
	set Tracking_BOx=@tracking_box, Born_Digital=@born_digital, Material_Received_Date=@material_received_date,
		Disposition_Advice=@disposition_advice, Disposition_Date=@disposition_date,
		Disposition_Type=@disposition_type, Material_Recd_Date_Estimated = @material_recd_date_estimated
	where ItemID=@itemid

end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Update_Digitization_Milestones]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Update_Digitization_Milestones]
	@ItemID int,
	@Last_Milestone int,
	@Milestone_Date date
AS
begin

	-- Digital Acquisition milestone
	if ( @Last_Milestone = 1 )
	begin
		-- Move along to the Post-acquisition processing
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, @Milestone_Date),
		    Milestone_ImageProcessing = null,
		    Milestone_QualityControl = null,
		    Milestone_OnlineComplete = null,
		    Last_MileStone=1
		where ItemID=@itemid	
	end
	
	-- Post acquisition processing
	if ( @Last_Milestone = 2 )
	begin
		-- Move along to the Post-acquisition processing
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, @Milestone_Date),
		    Milestone_ImageProcessing = ISNULL(Milestone_ImageProcessing, @Milestone_Date),
		    Milestone_QualityControl = null,
		    Milestone_OnlineComplete = null,
		    Last_MileStone=2
		where ItemID=@itemid		
	end
	
	-- Quality control complete
	if ( @Last_Milestone = 3 )
	begin
		-- Move along to the QC Complete milestone
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, @Milestone_Date),
		    Milestone_ImageProcessing = ISNULL(Milestone_ImageProcessing, @Milestone_Date),
		    Milestone_QualityControl = ISNULL(Milestone_QualityControl, @Milestone_Date),
		    Milestone_OnlineComplete = null,
		    Last_MileStone=3
		where ItemID=@itemid	
	end
	
	-- Digitization complete
	if ( @Last_Milestone = 4 )
	begin	
		-- Move along to the COMPLETED milestone
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, @Milestone_Date),
		    Milestone_ImageProcessing = ISNULL(Milestone_ImageProcessing, @Milestone_Date),
		    Milestone_QualityControl = ISNULL(Milestone_QualityControl, @Milestone_Date),
		    Milestone_OnlineComplete = ISNULL(Milestone_OnlineComplete, @Milestone_Date),
		    Last_MileStone=4
		where ItemID=@itemid	
	end
end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Update_Born_Digital]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Update_Born_Digital]
	@Born_Digital bit,
	@ItemID int
AS
begin

	-- Update the item row
	update SobekCM_Item 
	set Born_Digital = @Born_Digital 
	where ItemID = @ItemID

end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Submit_QC_Log]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Submit a log about QCing a volume
-- Written by Mark Sullivan ( March 2006 )
CREATE PROCEDURE [dbo].[Tracking_Submit_QC_Log]
	@bibid varchar(10),
	@vid varchar(5),
	@notes varchar(255),
	@scanqc varchar(100),
	@qcstatusid int,
	@volumeerrortypeid int,
	@storagelocation varchar(50)
AS
begin transaction

	-- Get the volume id
	declare @itemid int

	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid)

	-- Add REJECTS to the process table
	if ( @qcstatusid = 3 )
	begin
			-- Add this new progress 
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, ProgressNote, WorkPerformedBy, WorkingFilePath )
			values ( @itemid, 31,  getdate(), @notes, @scanqc, @storagelocation )
	end

	-- Add PRELIMINARIES to the process table
	if ( @qcstatusid = 5 )
	begin
			-- Add this new progress 
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, ProgressNote, WorkPerformedBy, WorkingFilePath )
			values ( @itemid, 41,  getdate(), @notes, @scanqc, @storagelocation )
	end
	
	-- Add ACCEPTS and also close some milestones
	if ( @qcstatusid = 4 )
	begin
		-- Add this new progress 
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, ProgressNote, WorkPerformedBy, WorkingFilePath )
		values ( @itemid, 5, getdate(), @notes, @scanqc, @storagelocation )
		
		-- Update the QC milestones
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, getdate()),
		    Milestone_ImageProcessing = ISNULL(Milestone_ImageProcessing, getdate()),
		    Milestone_QualityControl = ISNULL(Milestone_QualityControl, getdate()),
		    Last_MileStone = 3
		where ItemID=@itemid
		
		-- If the item is public, update the last milestone as well
		if ( ( select COUNT(*) from SobekCM_Item where ItemID=@itemid and (( Dark = 'true' ) or ( IP_Restriction_Mask >= 0 ))) > 0 )
		begin		
			-- Move along to the COMPLETED milestone
			update SobekCM_Item
			set Milestone_OnlineComplete = ISNULL(Milestone_OnlineComplete, getdate()),
				Last_MileStone=4
			where ItemID=@itemid		
		end		
	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Submit_Online_Page_Division]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Submit a log about QCing a volume
-- Written by Mark Sullivan ( July 2013 )
CREATE PROCEDURE [dbo].[Tracking_Submit_Online_Page_Division]
	@itemid int,
	@notes varchar(255),
	@onlineuser varchar(100),
	@mainthumbnail varchar(100),
	@mainjpeg varchar(100),
	@pagecount int,
	@filecount int,
	@disksize_mb bigint
AS
begin transaction

	
		-- Add this new progress 
		if (( select count(*)
		      from Tracking_Progress T
		      where ( T.ItemID=@itemid ) and ( ProgressNote=@notes ) and ( WorkPerformedBy=@onlineuser ) 
		        and ( CONVERT(varchar(10), getdate(), 10) = CONVERT(varchar(10), DateCompleted, 10))) = 0 )
		begin
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, ProgressNote, WorkPerformedBy, WorkingFilePath )
			values ( @itemid, 45, getdate(), @notes, @onlineuser, '' );
		end;
		
		-- Update the QC milestones
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, getdate()),
		    Milestone_ImageProcessing = ISNULL(Milestone_ImageProcessing, getdate()),
		    Milestone_QualityControl = ISNULL(Milestone_QualityControl, getdate())
		where ItemID=@itemid;
		
		-- update last mileston
		update SobekCM_Item
		set Last_Milestone = 3
		where ItemID = @itemid and Last_Milestone < 3;
		
		-- If the item is public, update the last milestone as well
		if ( ( select COUNT(*) from SobekCM_Item where ItemID=@itemid and (( Dark = 'true' ) or ( IP_Restriction_Mask >= 0 ))) > 0 )
		begin		
			-- Move along to the COMPLETED milestone
			update SobekCM_Item
			set Milestone_OnlineComplete = ISNULL(Milestone_OnlineComplete, getdate()),
				Last_MileStone=4
			where ItemID=@itemid		
		end;		

		--Update the item table
		update SobekCM_Item set [PageCount]=@pagecount, MainThumbnail = @mainthumbnail, MainJPEG = @mainjpeg, FileCount = @filecount, DiskSize_MB = @disksize_mb
		where ItemID = @itemid;
		
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Metadata_Search]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Perform metadata search for tracking application
-- This is identical to SobekCM_Metadata_Search, EXCEPT:
--    (1) Title, Author, IP restriction flag, Thumbnail, and Publisher returned with item list
--    (2) Group title, thumbnail, type, OCLC, and ALEPH returned with title list
--    (3) No facets returned and both PRIVATE and PUBLIC items are returned in list
--    (4) All tracking information ( milestones, tracking box, etc..) returned in item list
CREATE PROCEDURE [dbo].[Tracking_Metadata_Search]
	@term1 nvarchar(255),
	@field1 int,
	@link2 int,
	@term2 nvarchar(255),
	@field2 int,
	@link3 int,
	@term3 nvarchar(255),
	@field3 int,
	@link4 int,
	@term4 nvarchar(255),
	@field4 int,
	@link5 int,
	@term5 nvarchar(255),
	@field5 int,
	@link6 int,
	@term6 nvarchar(255),
	@field6 int,
	@link7 int,
	@term7 nvarchar(255),
	@field7 int,
	@link8 int,
	@term8 nvarchar(255),
	@field8 int,
	@link9 int,
	@term9 nvarchar(255),
	@field9 int,
	@link10 int,
	@term10 nvarchar(255),
	@field10 int,
	@aggregationcode varchar(20)
AS
BEGIN

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
	
	-- Field#'s indicate which metadata field (if any).  These are numbers from the 
	-- SobekCM_Metadata_Types table.  A field# of -1, means all fields are included.
	
	-- Link#'s indicate if this is an AND-joiner ( intersect ) or an OR-joiner ( union )
	-- 0 = AND, 1 = OR, 2 = AND NOT
	
	-- Examples of using this procedure are:
	-- exec SobekCM_Metadata_Search 'haiti',1,0,'kesse',4,0,'',0
	-- This searches for materials which have haiti in the title AND kesse in the creator
	
	-- exec SobekCM_Metadata_Search 'haiti',1,1,'kesse',-1,0,'',0
	-- This searches for materials which have haiti in the title OR kesse anywhere
	
	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #TEMPZERO ( ItemID int );
	create table #TEMP1 ( ItemID int primary key, Hit_Count int );
	create table #TEMP_ITEMS ( BibID varchar(10), VID varchar(5), SortDate bigint, Spatial_KML varchar(4000), fk_TitleID int, Title nvarchar(500), IP_Restriction_Mask smallint, ItemID int, Hit_Count int, Author nvarchar(1000), Publisher nvarchar(1000), Thumbnail varchar(100), Internal_Comments nvarchar(1000), Dark bit, Last_MileStone int, Milestone_DigitalAcquisition datetime, Milestone_ImageProcessing datetime, Milestone_QualityControl datetime, Milestone_OnlineComplete datetime, Born_Digital bit, Material_Received_Date datetime, Disposition_Advice int, Disposition_Date datetime, Disposition_Type int, Locally_Archived bit, Remotely_Archived bit, Tracking_Box varchar(25), AggregationCodes varchar(100), Level1_Text nvarchar(255), Level1_Index int, Level2_Text nvarchar(255), Level2_Index int, Level3_Text nvarchar(255), Level3_Index int, Level4_Text nvarchar(255), Level4_Index int, Level5_Text nvarchar(255), Level5_Index int, SortTitle nvarchar(500), PubDate nvarchar(100));
	create table #TEMP_TITLES ( fk_TitleID int, Item_Hit_Count int );
	
	-- Do not need to maintain row counts
	Set NoCount ON;
	    
	-- declare both the sql query and the parameter definitions
	declare @SQLQuery AS nvarchar(max);
    declare @ParamDefinition AS NVarchar(2000);
    
      -- Add the first term
    set @SQLQuery = 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm1 )) and ( L.MetadataID=M.MetadataID )';
    
    -- Was this search term field specific?
    if ( @field1 > 0 )
		set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field1 as varchar(2)) + ')';

	-- Add the second term, if there is one
	if (( LEN( ISNULL(@term2,'')) > 0 ) and (( @link2 = 0 ) or ( @link2 = 1 ) or ( @link2 = 2 )))
	begin	
		-- Was this an AND?
		if ( @link2 = 0 )
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link2 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link2 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm2 )) and ( L.MetadataID=M.MetadataID )';
				
		-- Was this search term field specific?
		if ( @field2 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field2 as varchar(2)) + ')';		
	end;    
	
	-- Add the third term, if there is one
	if (( LEN( ISNULL(@term3,'')) > 0 ) and (( @link3 = 0 ) or ( @link3 = 1 ) or ( @link3 = 2 )))
	begin	
		-- Was this an AND?
		if ( @link3 = 0 )
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link3 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link3 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm3 )) and ( L.MetadataID=M.MetadataID )';
				
		-- Was this search term field specific?
		if ( @field3 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field3 as varchar(2)) + ')';
	end;
	
	-- Add the fourth term, if there is one
	if (( LEN( ISNULL(@term4,'')) > 0 ) and (( @link4 = 0 ) or ( @link4 = 1 ) or ( @link4 = 2 )))
	begin	
		-- Was this an AND?
		if ( @link4 = 0 )
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link4 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link4 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm4 )) and ( L.MetadataID=M.MetadataID )';
		
		-- Was this search term field specific?
		if ( @field4 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field4 as varchar(2)) + ')';
	end;
	
	-- Add the fifth term, if there is one
	if (( LEN( ISNULL(@term5,'')) > 0 ) and (( @link5 = 0 ) or ( @link5 = 1 ) or ( @link5 = 2 )))
	begin	
		-- Was this an AND?
		if ( @link5 = 0 )
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link5 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link5 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm5 )) and ( L.MetadataID=M.MetadataID )';
		
		-- Was this search term field specific?
		if ( @field5 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field5 as varchar(2)) + ')';
	end;
	
	-- Add the sixth term, if there is one
	if (( LEN( ISNULL(@term6,'')) > 0 ) and (( @link6 = 0 ) or ( @link6 = 1 ) or ( @link6 = 2 )))
	begin	
		-- Was this an AND?
		if ( @link6 = 0 )
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link6 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link6 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm6 )) and ( L.MetadataID=M.MetadataID )';		
		
		-- Was this search term field specific?
		if ( @field6 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field6 as varchar(2)) + ')';
	end;
	
	-- Add the seventh term, if there is one
	if (( LEN( ISNULL(@term7,'')) > 0 ) and (( @link7 = 0 ) or ( @link7 = 1 ) or ( @link7 = 2 )))
	begin	
		-- Was this an AND?
		if ( @link7 = 0 )
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link7 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link7 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm7 )) and ( L.MetadataID=M.MetadataID )';		
		
		-- Was this search term field specific?
		if ( @field7 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field7 as varchar(2)) + ')';
	end;
	
	-- Add the eighth term, if there is one
	if (( LEN( ISNULL(@term8,'')) > 0 ) and (( @link8 = 0 ) or ( @link8 = 1 ) or ( @link8 = 2 )))
	begin	
		-- Was this an AND?
		if ( @link8 = 0 )
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link8 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link8 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm8 )) and ( L.MetadataID=M.MetadataID )';
		
		-- Was this search term field specific?
		if ( @field8 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field8 as varchar(2)) + ')';
	end;
	
	-- Add the ninth term, if there is one
	if (( LEN( ISNULL(@term9,'')) > 0 ) and (( @link9 = 0 ) or ( @link9 = 1 ) or ( @link9 = 2 )))
	begin	
		-- Was this an AND?
		if (( @link9 = 0 ) or ( @link9 = 2 ))
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link9 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link9 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm9 )) and ( L.MetadataID=M.MetadataID )';
		
		-- Was this search term field specific?
		if ( @field9 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field9 as varchar(2)) + ')';
	end;
	
	-- Add the tenth term, if there is one
	if (( LEN( ISNULL(@term10,'')) > 0 ) and (( @link10 = 0 ) or ( @link10 = 1 ) or ( @link10 = 2 )))
	begin	
		-- Was this an AND?
		if (( @link10 = 0 ) or ( @link10 = 2 ))
			set @SQLQuery = @SQLQuery + ' intersect ';
		
		-- Was this an OR?
		if ( @link10 = 1 )
			set @SQLQuery = @SQLQuery + ' union ';
			
		-- Was this an AND NOT?
		if ( @link10 = 2 )
			set @SQLQuery = @SQLQuery + ' except ';
		
		-- Add the term next
		set @SQLQuery = @SQLQuery + 'select distinct(itemid) from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L where ( contains( MetadataValue, @innerterm10 )) and ( L.MetadataID=M.MetadataID )';
		
		-- Was this search term field specific?
		if ( @field10 > 0 )
			set @SQLQuery = @SQLQuery + ' and ( MetadataTypeID = ' + CAST( @field10 as varchar(2)) + ')';
	end;
	
	-- Add the recompile option
	set @SQLQuery = @SQLQuery + ' option (RECOMPILE)';
	
	-- Set the parameter definition
	set @ParamDefinition = ' @innerterm1 nvarchar(255), @innerterm2 nvarchar(255), @innerterm3 nvarchar(255), @innerterm4 nvarchar(255), @innerterm5 nvarchar(255), @innerterm6 nvarchar(255), @innerterm7 nvarchar(255), @innerterm8 nvarchar(255), @innerterm9 nvarchar(255), @innerterm10 nvarchar(255)';
	
	-- Execute this stored procedure
	insert #TEMPZERO execute sp_Executesql @SQLQuery, @ParamDefinition, @term1, @term2, @term3, @term4, @term5, @term6, @term7, @term8, @term9, @term10;
	
	-- Select the distinct item id's and sort by relevance
	insert into #TEMP1 ( ItemID, Hit_Count )
	select distinct(ItemID), Hit_Count = COUNT(*)
	from #TEMPZERO
	group by ItemID
	order by Hit_Count DESC;

	-- Drop the big temporary table
	drop table #TEMPZERO;
	
	-- Was an aggregation included?
	if ( LEN(ISNULL( @aggregationcode,'' )) > 0 )
	begin	
		-- Look for matching the provided aggregation
		insert into #TEMP_ITEMS ( BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, IP_Restriction_Mask, ItemID, Hit_Count, Author, Publisher, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, Disposition_Advice, Disposition_Date, Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, SortTitle, PubDate )
		select G.BibID, I.VID, isnull( I.SortDate,-1), isnull(Spatial_KML,''), I.GroupID, I.Title, IP_Restriction_Mask, I.ItemID, Hit_Count, I.Author, I.Publisher, I.MainThumbnail, I.Internal_Comments, I.Dark, I.Last_MileStone, I.Milestone_DigitalAcquisition, I.Milestone_ImageProcessing, I.Milestone_QualityControl, I.Milestone_OnlineComplete, I.Born_Digital, I.Material_Received_Date, I.Disposition_Advice, I.Disposition_Date, I.Disposition_Type, I.Locally_Archived, I.Remotely_Archived, I.Tracking_Box, I.AggregationCodes, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), SortTitle=ISNULL(I.SortTitle, upper(Title)), I.PubDate
		from #TEMP1 T1, SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Aggregation C, SobekCM_Item_Group G
		where ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = C.AggregationID )
		  and ( C.Code = @aggregationcode )
		  and ( I.Deleted = CONVERT(bit,0) )
		  and ( I.GroupID = G.GroupID ) 
		  and ( T1.ItemID = I.ItemID );
	end
	else
	begin	
		-- Pull the information about item
		insert into #TEMP_ITEMS ( BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, IP_Restriction_Mask, ItemID, Hit_Count, Author, Publisher, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, Disposition_Advice, Disposition_Date, Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, SortTitle, PubDate )
		select G.BibID, I.VID, isnull( I.SortDate,-1), isnull(Spatial_KML,''), I.GroupID, Title, IP_Restriction_Mask, I.ItemID, Hit_Count, I.Author, I.Publisher, I.MainThumbnail, I.Internal_Comments, I.Dark, I.Last_MileStone, I.Milestone_DigitalAcquisition, I.Milestone_ImageProcessing, I.Milestone_QualityControl, I.Milestone_OnlineComplete, I.Born_Digital, I.Material_Received_Date, I.Disposition_Advice, I.Disposition_Date, I.Disposition_Type, I.Locally_Archived, I.Remotely_Archived, I.Tracking_Box, I.AggregationCodes, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), SortTitle=ISNULL(I.SortTitle, upper(Title)), I.PubDate
		from #TEMP1 T1, SobekCM_Item I, SobekCM_Item_Group G
		where ( I.Deleted = CONVERT(bit,0) )
		  and ( I.GroupID = G.GroupID ) 
		  and ( T1.ItemID = I.ItemID );
	end;
		  
	-- Select the title information
	insert into #TEMP_TITLES ( fk_TitleID, Item_Hit_Count )
	select fk_TitleID, Item_Hit_Count=(SUM(Hit_COunt)/COUNT(*))
	from #TEMP_ITEMS I
	group by fk_TitleID
	order by Item_Hit_Count DESC;

	-- Return the item informaiton
	select BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, Author, Publisher, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, IP_Restriction_Mask, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, isnull(DAT.DispositionFuture,'') AS Disposition_Advice, Disposition_Date, isnull(DT.DispositionPast,'') AS Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, ItemID, SortTitle, PubDate
	from #TEMP_ITEMS AS T left outer join
		 Tracking_Disposition_Type AS DAT ON T.Disposition_Advice=DAT.DispositionID left outer join
		 Tracking_Disposition_Type AS DT ON T.Disposition_Type=DT.DispositionID;

	-- Return the title information
	select G.BibID, G.SortTitle, TitleID=G.GroupID, [Rank]=-1, G.GroupTitle, G.GroupThumbnail, G.[Type], G.ALEPH_Number, G.OCLC_Number, isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
	from SobekCM_Item_Group G, #TEMP_TITLES I
	where  G.GroupID = I.fk_TitleID
	order by Item_Hit_Count DESC;
	
	select Query = @SQLQuery;
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	drop table #TEMP_TITLES;
	drop table #TEMP1;
	
	Set NoCount OFF;
			
	If @@ERROR <> 0 GoTo ErrorHandler      
		Return(0);
  
ErrorHandler:
    Return(@@ERROR);
	
END;
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Metadata_Exact_Search]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Perform metadata search for tracking application
-- This is identical to SobekCM_Metadata_Exact_Search, EXCEPT:
--    (1) Title, Author, IP restriction flag, Thumbnail, and Publisher returned with item list
--    (2) Group title, thumbnail, type, OCLC, and ALEPH returned with title list
--    (3) No facets returned and both PRIVATE and PUBLIC items are returned in list
--    (4) All tracking information ( milestones, tracking box, etc..) returned in item list
CREATE PROCEDURE [dbo].[Tracking_Metadata_Exact_Search] 
	@term1 nvarchar(255),
	@field1 int,
	@aggregationcode varchar(20)
AS
BEGIN

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
	
	-- This search routine is optimized for not specifying any particular fields and
	-- for all the links to either be AND ( @link=0 ) or OR ( @link=1 )
		
	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #TEMP_ITEMS ( BibID varchar(10), VID varchar(5), SortDate bigint, Spatial_KML varchar(4000), fk_TitleID int, Title nvarchar(500), IP_Restriction_Mask smallint, ItemID int, Hit_Count int, Author nvarchar(1000), Publisher nvarchar(1000), Thumbnail varchar(100), Internal_Comments nvarchar(1000), Dark bit, Last_MileStone int, Milestone_DigitalAcquisition datetime, Milestone_ImageProcessing datetime, Milestone_QualityControl datetime, Milestone_OnlineComplete datetime, Born_Digital bit, Material_Received_Date datetime, Disposition_Advice int, Disposition_Date datetime, Disposition_Type int, Locally_Archived bit, Remotely_Archived bit, Tracking_Box varchar(25), AggregationCodes varchar(100), Level1_Text nvarchar(255), Level1_Index int, Level2_Text nvarchar(255), Level2_Index int, Level3_Text nvarchar(255), Level3_Index int, Level4_Text nvarchar(255), Level4_Index int, Level5_Text nvarchar(255), Level5_Index int, SortTitle nvarchar(500), PubDate nvarchar(100));
	create table #TEMP_TITLES ( fk_TitleID int, Item_Hit_Count int );
	
	-- Do not need to maintain row counts
	Set NoCount ON;
	
	-- Was an aggregation included?
	if ( LEN(ISNULL( @aggregationcode,'' )) > 0 )
	begin		  

		insert into #TEMP_ITEMS ( BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, IP_Restriction_Mask, ItemID, Hit_Count, Author, Publisher, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, Disposition_Advice, Disposition_Date, Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, SortTitle, PubDate )
		select G.BibID, I.VID, SortDate=isnull( I.SortDate,-1), Spatial_KML=isnull(Spatial_KML,''), fk_TitleID = I.GroupID, Title=I.Title, IP_Restriction_Mask, I.ItemID, -1, I.Author, I.Publisher, I.MainThumbnail, I.Internal_Comments, I.Dark, I.Last_MileStone, I.Milestone_DigitalAcquisition, I.Milestone_ImageProcessing, I.Milestone_QualityControl, I.Milestone_OnlineComplete, I.Born_Digital, I.Material_Received_Date, I.Disposition_Advice, I.Disposition_Date, I.Disposition_Type, I.Locally_Archived, I.Remotely_Archived, I.Tracking_Box, I.AggregationCodes, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), SortTitle=ISNULL(I.SortTitle, upper(Title)), I.PubDate
		from SobekCM_Item AS I inner join
			 SobekCM_Item_Group AS G ON I.GroupID = G.GroupID inner join
			 SobekCM_Item_Aggregation_Item_Link AS CL ON CL.ItemID = I.ItemID inner join
			 SobekCM_Item_Aggregation AS C ON C.AggregationID = CL.AggregationID inner join
			 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
			 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = SUBSTRING(@term1, 1, 100) and M.MetadataValue = @term1
	    where I.Deleted = CONVERT(bit,0)
	      and C.Code = @aggregationcode;
	end
	else
	begin	

		insert into #TEMP_ITEMS ( BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, IP_Restriction_Mask, ItemID, Hit_Count, Author, Publisher, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, Disposition_Advice, Disposition_Date, Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, SortTitle, PubDate )
		select G.BibID, I.VID, SortDate=isnull( I.SortDate,-1), Spatial_KML=isnull(Spatial_KML,''), fk_TitleID = I.GroupID, Title=I.Title, IP_Restriction_Mask, I.ItemID, -1, I.Author, I.Publisher, I.MainThumbnail, I.Internal_Comments, I.Dark, I.Last_MileStone, I.Milestone_DigitalAcquisition, I.Milestone_ImageProcessing, I.Milestone_QualityControl, I.Milestone_OnlineComplete, I.Born_Digital, I.Material_Received_Date, I.Disposition_Advice, I.Disposition_Date, I.Disposition_Type, I.Locally_Archived, I.Remotely_Archived, I.Tracking_Box, I.AggregationCodes, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), SortTitle=ISNULL(I.SortTitle, upper(Title)), I.PubDate
		from SobekCM_Item AS I inner join
			 SobekCM_Item_Group AS G ON I.GroupID = G.GroupID inner join
			 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
			 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = SUBSTRING(@term1, 1, 100) and M.MetadataValue = @term1
	    where I.Deleted = CONVERT(bit,0);
		  
	end

	-- Select the title information
	insert into #TEMP_TITLES ( fk_TitleID, Item_Hit_Count )
	select fk_TitleID, Item_Hit_Count=(SUM(Hit_COunt)/COUNT(*))
	from #TEMP_ITEMS I
	group by fk_TitleID
	order by Item_Hit_Count DESC;

	-- Return the item informaiton
	select BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, Author, Publisher, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, IP_Restriction_Mask, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, isnull(DAT.DispositionFuture,'') AS Disposition_Advice, Disposition_Date, isnull(DT.DispositionPast,'') AS Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, ItemID, SortTitle, PubDate
	from #TEMP_ITEMS AS T left outer join
		 Tracking_Disposition_Type AS DAT ON T.Disposition_Advice=DAT.DispositionID left outer join
		 Tracking_Disposition_Type AS DT ON T.Disposition_Type=DT.DispositionID;

	-- Return the title information
	select G.BibID, G.SortTitle, TitleID=G.GroupID, [Rank]=-1, G.GroupTitle, G.GroupThumbnail, G.[Type], G.ALEPH_Number, G.OCLC_Number, isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
	from SobekCM_Item_Group G, #TEMP_TITLES I
	where  G.GroupID = I.fk_TitleID
	order by Item_Hit_Count DESC;
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	drop table #TEMP_TITLES;
			
    Set NoCount OFF;
	
END;
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Metadata_Basic_Search]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Perform metadata search for tracking application
-- This is identical to SobekCM_Metadata_Basic_Search, EXCEPT:
--    (1) Title, Author, IP restriction flag, Thumbnail, and Publisher returned with item list
--    (2) Group title, thumbnail, type, OCLC, and ALEPH returned with title list
--    (3) No facets returned and both PRIVATE and PUBLIC items are returned in list
--    (4) All tracking information ( milestones, tracking box, etc..) returned in item list
CREATE PROCEDURE [dbo].[Tracking_Metadata_Basic_Search] 
	@searchcondition nvarchar(255),
	@aggregationcode varchar(20)
AS
BEGIN

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
	
	-- This search routine is optimized for not specifying any particular fields and
	-- for all the links to either be AND ( @link=0 ) or OR ( @link=1 )
		
	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #TEMP_ITEMS ( BibID varchar(10), VID varchar(5), SortDate bigint, Spatial_KML varchar(4000), fk_TitleID int, Title nvarchar(500), IP_Restriction_Mask smallint, ItemID int, Hit_Count int, Author nvarchar(1000), Publisher nvarchar(1000), Thumbnail varchar(100), Internal_Comments nvarchar(1000), Dark bit, Last_MileStone int, Milestone_DigitalAcquisition datetime, Milestone_ImageProcessing datetime, Milestone_QualityControl datetime, Milestone_OnlineComplete datetime, Born_Digital bit, Material_Received_Date datetime, Disposition_Advice int, Disposition_Date datetime, Disposition_Type int, Locally_Archived bit, Remotely_Archived bit, Tracking_Box varchar(25), AggregationCodes varchar(100), Level1_Text nvarchar(255), Level1_Index int, Level2_Text nvarchar(255), Level2_Index int, Level3_Text nvarchar(255), Level3_Index int, Level4_Text nvarchar(255), Level4_Index int, Level5_Text nvarchar(255), Level5_Index int, SortTitle nvarchar(500), PubDate nvarchar(100));
	create table #TEMP_TITLES ( fk_TitleID int, Item_Hit_Count int );
	
	-- Do not need to maintain row counts
	Set NoCount ON;
	
	-- Was an aggregation included?
	if ( LEN(ISNULL( @aggregationcode,'' )) > 0 )
	begin		  

		insert into #TEMP_ITEMS ( BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, IP_Restriction_Mask, ItemID, Hit_Count, Author, Publisher, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, Disposition_Advice, Disposition_Date, Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, SortTitle, PubDate)
		select G.BibID, I.VID, SortDate=isnull( I.SortDate,-1), Spatial_KML=isnull(Spatial_KML,''), fk_TitleID = I.GroupID, Title=I.Title, IP_Restriction_Mask, I.ItemID, KEY_TBL.RANK, I.Author, I.Publisher, I.MainThumbnail, I.Internal_Comments, I.Dark, I.Last_MileStone, I.Milestone_DigitalAcquisition, I.Milestone_ImageProcessing, I.Milestone_QualityControl, I.Milestone_OnlineComplete, I.Born_Digital, I.Material_Received_Date, I.Disposition_Advice, I.Disposition_Date, I.Disposition_Type, I.Locally_Archived, I.Remotely_Archived, I.Tracking_Box, I.AggregationCodes, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), SortTitle=ISNULL(I.SortTitle, upper(Title)), I.PubDate
		from SobekCM_Item AS I inner join
			 SobekCM_Item_Group AS G ON I.GroupID = G.GroupID inner join
			 SobekCM_Item_Aggregation_Item_Link AS CL ON CL.ItemID = I.ItemID inner join
			 SobekCM_Item_Aggregation AS C ON C.AggregationID = CL.AggregationID inner join
			 CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
	    where I.Deleted = CONVERT(bit,0)
	      and C.Code = @aggregationcode;
	end
	else
	begin	

		insert into #TEMP_ITEMS ( BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, IP_Restriction_Mask, ItemID, Hit_Count, Author, Publisher, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, Disposition_Advice, Disposition_Date, Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, SortTitle, PubDate)
		select G.BibID, I.VID, SortDate=isnull( I.SortDate,-1), Spatial_KML=isnull(Spatial_KML,''), fk_TitleID = I.GroupID, Title=I.Title, IP_Restriction_Mask, I.ItemID, KEY_TBL.RANK, I.Author, I.Publisher, I.MainThumbnail, I.Internal_Comments, I.Dark, I.Last_MileStone, I.Milestone_DigitalAcquisition, I.Milestone_ImageProcessing, I.Milestone_QualityControl, I.Milestone_OnlineComplete, I.Born_Digital, I.Material_Received_Date, I.Disposition_Advice, I.Disposition_Date, I.Disposition_Type, I.Locally_Archived, I.Remotely_Archived, I.Tracking_Box, I.AggregationCodes, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), SortTitle=ISNULL(I.SortTitle, upper(Title)), I.PubDate
		from SobekCM_Item AS I inner join
			 SobekCM_Item_Group AS G ON I.GroupID = G.GroupID inner join
			 CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
	    where I.Deleted = CONVERT(bit,0);		  
	end;

	-- Select the title information
	insert into #TEMP_TITLES ( fk_TitleID, Item_Hit_Count )
	select fk_TitleID, Item_Hit_Count=(SUM(Hit_COunt)/COUNT(*))
	from #TEMP_ITEMS I
	group by fk_TitleID;
	
	-- Return the item informaiton
	select BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, Author, Publisher, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, IP_Restriction_Mask, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, isnull(DAT.DispositionFuture,'') AS Disposition_Advice, Disposition_Date, isnull(DT.DispositionPast,'') AS Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, ItemID, SortTitle, PubDate
	from #TEMP_ITEMS AS T left outer join
		 Tracking_Disposition_Type AS DAT ON T.Disposition_Advice=DAT.DispositionID left outer join
		 Tracking_Disposition_Type AS DT ON T.Disposition_Type=DT.DispositionID;

	-- Return the title information
	select G.BibID, G.SortTitle, TitleID=G.GroupID, [Rank]=-1, G.GroupTitle, G.GroupThumbnail, G.[Type], G.ALEPH_Number, G.OCLC_Number, isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
	from SobekCM_Item_Group G, #TEMP_TITLES I
	where  G.GroupID = I.fk_TitleID
	order by Item_Hit_Count DESC;
		
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	drop table #TEMP_TITLES;
			
    Set NoCount OFF;
	
END;
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Update_Material_Received]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Update_Material_Received]
	@Material_Received_Date datetime,
	@Material_Recd_Date_Estimated bit,
	@ItemID int,
	@User varchar(255),
	@ProgressNote varchar(1000)
AS
begin

	-- Update the item row
	update SobekCM_Item 
	set Material_Received_Date=@Material_Received_Date, Material_Recd_Date_Estimated=@Material_Recd_Date_Estimated
	where ItemID = @ItemID
	
	-- If this is not a widely innacurate estimate, add this as a worklog entry as well
	if ( @Material_Recd_Date_Estimated = 'false' )
	begin
		-- Add into worklog history
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
		values ( @ItemID, 42, @Material_Received_Date, @User, @ProgressNote, '' )	
	end
end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Update_List]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure pulls the bibs and vids updated on SobekCM since the provided date. 
CREATE PROCEDURE [dbo].[Tracking_Update_List]
      @sinceDate varchar(10)
as
begin
      select BibID, VID, DateCompleted, WorkFlowName, WorkPerformedBy
      from Tracking_WorkFlow W, Tracking_Progress P, SobekCM_Item_Group G, SobekCM_Item I
      where ( W.WorkFlowID = P.WorkFlowID )
        and ( P.ItemID = I.ItemID ) 
        and ( I.GroupID = G.GroupID )
        and (( W.WorkFlowID = 29 ) or ( W.WorkFlowID = 30 ) or ( W.WorkFlowID = 34 ) or ( W.WorkFlowID = 35 ) or ( W.WorkFlowID = 36 ) or ( W.WorkFlowID=40 ) or (W.WorkFlowID=44))
        and ( DateCompleted > @sinceDate )
      order by DateCompleted DESC
end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Update_Disposition]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Update_Disposition]
	@Disposition_Date datetime,
	@Disposition_Type int,
	@Disposition_Notes varchar(150),
	@ItemID int,
	@UserName varchar(50)
AS
begin

	-- Update the item row
	update SobekCM_Item set Disposition_Date = @Disposition_Date, Disposition_Type = @Disposition_Type, Disposition_Notes=@Disposition_Notes where ItemID = @ItemID
	
	-- Add as a workflow
	insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
	values ( @itemid, 43, @Disposition_Date, @UserName, @Disposition_Notes + ' (ADDED ' + CONVERT(VARCHAR(10), GETDATE(), 101) + ')', '' )	
end
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Online_Submit_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Marks this volume as having been submitted online
CREATE PROCEDURE [dbo].[Tracking_Online_Submit_Complete]
	@itemid int,
	@user varchar(50),
	@usernotes varchar(1000)
AS
begin transaction

	-- Just add this new progress then
	insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
	values ( @itemid, 29, getdate(), @user, @usernotes, '' )

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Online_Edit_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Marks this volume as having been edited online
CREATE PROCEDURE [dbo].[Tracking_Online_Edit_Complete]
	@itemid int,
	@user varchar(50),
	@usernotes varchar(1000)
AS
begin transaction

	-- Just add this new progress then
	insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
	values ( @itemid, 30, getdate(), @user, @usernotes, '' )

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Stats_Get_Users_Linked_To_Items]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of all users that have items which may have statistics
CREATE PROCEDURE [dbo].[SobekCM_Stats_Get_Users_Linked_To_Items] AS
begin
	-- No need to perform any locks here.
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	select U.FirstName, U.LastName, U.NickName, U.UserName, U.UserID, U.EmailAddress
	from mySobek_User U
	where ( Receive_Stats_Emails = 'true' )
	   and exists ( select * from mySobek_User_Item_Link L, mySobek_User_Item_Link_Relationship R where L.UserID=U.UserID and L.RelationshipID=R.RelationshipID and R.Include_In_Results = 'true' );
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Stats_Get_User_Linked_Items_Stats]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of items linked to this user, along with usage for that month
CREATE PROCEDURE [dbo].[SobekCM_Stats_Get_User_Linked_Items_Stats]
	@userid int,
	@month int,
	@year int
AS
begin
	-- No need to perform any locks here.
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	select L.ItemID, L.RelationshipID, R.RelationshipLabel, I.Title, G.BibID, I.VID, I.CreateDate, I.Total_Hits, I.Total_Sessions, ISNULL(S2.Hits,0) as Month_Hits, ISNULL(S2.[Sessions],0) as Month_Sessions
	from mySobek_User_Item_Link_Relationship AS R join
		 mySobek_User_Item_Link AS L ON ( L.RelationshipID=R.RelationshipID ) join
		 SobekCM_Item AS I ON ( L.ItemID=I.ItemID ) join
		 SobekCM_Item_Group AS G ON ( G.GroupID=I.GroupID) left join
		 SobekCM_Item_Statistics AS S2 ON ( S2.ItemID=L.ItemID and S2.[Month]=@month and S2.[Year]=@year)		  
	where ( L.UserID=@userid ) and ( R.Include_In_Results = 'true' );
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Stats_Get_User_Linked_Items]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the list of all items linked to a user for statistics reporting purposes
CREATE PROCEDURE [dbo].[SobekCM_Stats_Get_User_Linked_Items]
	@userid int
AS
begin
	-- No need to perform any locks here.
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	select L.ItemID, R.RelationshipID, R.RelationshipLabel, G.BibID, I.VID, G.GroupTitle, I.Title
	from mySobek_User_Item_Link L, mySobek_User_Item_Link_Relationship R, SobekCM_Item I, SobekCM_Item_Group G
	where ( L.ItemID = I.ItemID )
	  and ( L.RelationshipID = R.RelationshipID )
	  and ( I.GroupID = G.GroupID )
	  and ( R.Include_In_Results = 'true' );
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Statistics_Item_Group]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure returns all the usage stats on a title, both at the title
-- level, and on each individual item (summed up) under this title
CREATE PROCEDURE [dbo].[SobekCM_Statistics_Item_Group]
	@bibid varchar(10)
AS
begin

	-- No need to perform any locks here.
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the item group stats
	WITH Group_CTE ( [Year], [Month], Group_Hits, Group_Sessions )
	AS
	(
		select [Year], [Month], Hits as Group_Hits, [Sessions] as Group_Sessions
		from SobekCM_Item_Group_Statistics GRS, SobekCM_Item_Group G
		where G.BibID=@bibid
		  and G.GroupID = GRS.GroupID
	),
	Items_CTE ( [Year], [Month], Item_Hits, JPEG_Views, Zoomable_Views, Citation_Views, Thumbnail_Views, Text_Search_Views, Flash_Views, Google_Map_Views, Download_Views )
	AS
	(
		-- Get the summed item stats
		select [Year], [Month], SUM(Hits) as Item_Hits, SUM(JPEG_Views) as JPEG_Views, SUM(Zoomable_Views) as Zoomable_Views, 
				SUM(Citation_Views) as Citation_Views, SUM(Thumbnail_Views) as Thumbnail_Views, SUM(Text_Search_Views) as Text_Search_Views, 
				SUM( Flash_Views ) as Flash_Views, SUM( Google_Map_Views) as Google_Map_Views, SUM(Download_Views) as Download_Views
		from SobekCM_Item_Statistics ITS, SobekCM_Item_Group G, SobekCM_Item I
		where G.BibID=@bibid
		  and G.GroupID = I.GroupID
		  and I.ItemID = ITS.ItemID
		group by [Year], [Month]
	)
	  
	-- Return a merged table
	select I.[Year], I.[Month], Total_Hits=ISNULL(Group_Hits,0) + Item_Hits, isnull(Group_Hits,0) as Group_Hits, Item_Hits, JPEG_Views, Zoomable_Views, Citation_Views, Thumbnail_Views, Text_Search_Views, Flash_Views, Google_Map_Views, Download_Views
	from Items_CTE AS I left join
		 Group_CTE as G on G.[Year]=I.[Year] and G.[Month]=I.[Month]	     
	order by I.[Year], I.[Month];

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Statistics_By_Date_Range]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Statistics_By_Date_Range]
	@year1 smallint,
	@month1 smallint,
	@year2 smallint,
	@month2 smallint
AS
BEGIN

-- Sum the collection level statistics for this period
select S.AggregationID, sum( Hits ) as Hits, sum( [Sessions] ) as [Sessions], 
	sum( Home_Page_Views) as Home_Page_Views, sum ( Browse_Views ) as Browse_Views,
	sum(Advanced_Search_Views) as Advanced_Search_Views, sum ( Search_Results_Views ) as Search_Results_Views,
	sum( Title_Hits ) as Title_Hits, sum ( Item_Hits ) as Item_Hits,
	sum( Item_JPEG_Views ) as Item_JPEG_Views, sum ( Item_Zoomable_Views ) as Item_Zoomable_Views,
	sum ( Item_Citation_Views ) as Item_Citation_Views, sum ( Item_Thumbnail_Views ) as Item_Thumbnail_Views,
	sum ( Item_Text_Search_Views ) as Item_Text_Search_Views, sum ( Item_Flash_Views ) as Item_Flash_Views,
	sum ( Item_Google_Map_Views) as Item_Google_Map_Views, sum( Item_Download_Views ) as item_Download_Views,
	sum ( Item_Static_Views) as Item_Static_Views, sum ( Title_Count ) as Title_Count, sum ( Item_Count) as Item_Count
into #TEMP_AGGREGATION_STATS
from SobekCM_Item_Aggregation_Statistics S
where ((( @year1 < @year2 ) and ((( [Month] >= @month1 ) and ( [Year] = @year1 ))
  or (( [Year] > @year1 ) and ( [Year] < @year2 ))
  or (( [Month] <= @month2 ) and ( [Year] = @year2 ))))
  or (( @year1 = @year2 ) and ( [Year] = @year1 ) and ( [Month] >= @month1 ) and ( [Month] <= @month2 )))
group by S.AggregationID
order by S.AggregationID

-- Start to build the return set of values
select Row_Type=C.TEMP_OldType, group_code = Code, coll_code = '                    ', subcollect_code = '                    ', [Name],
	isnull( Hits, 0 ) as Hits, isnull( [Sessions], 0 ) as [Sessions], 
	isnull( Home_Page_Views, 0) as Home_Page_Views, isnull ( Browse_Views, 0 ) as Browse_Views,
	isnull(Advanced_Search_Views, 0 ) as Advanced_Search_Views, isnull ( Search_Results_Views, 0 ) as Search_Results_Views,
	isnull( Title_Hits, 0 ) as Title_Hits, isnull ( Item_Hits, 0 ) as Item_Hits,
	isnull( Item_JPEG_Views, 0 ) as Item_JPEG_Views, isnull ( Item_Zoomable_Views, 0 ) as Item_Zoomable_Views,
	isnull ( Item_Citation_Views, 0 ) as Item_Citation_Views, isnull ( Item_Thumbnail_Views, 0 ) as Item_Thumbnail_Views,
	isnull ( Item_Text_Search_Views, 0 ) as Item_Text_Search_Views, isnull ( Item_Flash_Views, 0 ) as Item_Flash_Views,
	isnull ( Item_Google_Map_Views, 0 ) as Item_Google_Map_Views, isnull( Item_Download_Views, 0 ) as item_Download_Views,
	isnull ( Item_Static_Views, 0 ) as Item_Static_Views, isnull ( Title_Count, 0 ) as Title_Count, isnull ( Item_Count, 0) as Item_Count,
	C.AggregationID
into #TEMP_BUILDER
from SobekCM_Item_Aggregation AS C LEFT OUTER JOIN
	#TEMP_AGGREGATION_STATS AS S ON ( C.AggregationID = S.AggregationID )
where ( C.Hidden = 'false' )
  and ( C.isActive = 'true' )


-- Build the hierarchy
update #TEMP_BUILDER
SET coll_code = group_code, group_code = (select TOP 1 Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_BUILDER.AggregationID),
AggregationID = (select TOP 1 A.AggregationID from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_BUILDER.AggregationID)
where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_BUILDER.AggregationID)
 
-- Build the hierarchy
update #TEMP_BUILDER
SET subcollect_code = coll_code, coll_code = group_code, group_code = (select TOP 1 Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_BUILDER.AggregationID)
where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_BUILDER.AggregationID)

-- Pull all the statistical data by item
select sum( Hits ) as Item_Hits,
	sum( JPEG_Views ) as Item_JPEG_Views, sum ( Zoomable_Views ) as Item_Zoomable_Views,
	sum ( Citation_Views ) as Item_Citation_Views, sum ( Thumbnail_Views ) as Item_Thumbnail_Views,
	sum ( Text_Search_Views ) as Item_Text_Search_Views, sum ( Flash_Views ) as Item_Flash_Views,
	sum ( Google_Map_Views) as Item_Google_Map_Views, sum( Download_Views ) as item_Download_Views,
	sum ( Static_Views) as Item_Static_Views
into #TEMP_ITEM_STATS
from SobekCM_Item_Statistics
where ((( @year1 < @year2 ) and ((( [Month] >= @month1 ) and ( [Year] = @year1 ))
  or (( [Year] > @year1 ) and ( [Year] < @year2 ))
  or (( [Month] <= @month2 ) and ( [Year] = @year2 ))))
  or (( @year1 = @year2 ) and ( [Year] = @year1 ) and ( [Month] >= @month1 ) and ( [Month] <= @month2 )))

-- Pull all the statistical data by group
select sum( Hits ) as Title_Hits
into #TEMP_GROUP_STATS
from SobekCM_Item_Group_Statistics
where ((( @year1 < @year2 ) and ((( [Month] >= @month1 ) and ( [Year] = @year1 ))
  or (( [Year] > @year1 ) and ( [Year] < @year2 ))
  or (( [Month] <= @month2 ) and ( [Year] = @year2 ))))
  or (( @year1 = @year2 ) and ( [Year] = @year1 ) and ( [Month] >= @month1 ) and ( [Month] <= @month2 )))

-- Pull all the statistical overall data (could be multiple if we have two URLs)
select sum( Hits ) as Hits, sum( [Sessions] ) as Sessions
into #TEMP_URL_STATS
from SobekCM_Statistics
where ((( @year1 < @year2 ) and ((( [Month] >= @month1 ) and ( [Year] = @year1 ))
  or (( [Year] > @year1 ) and ( [Year] < @year2 ))
  or (( [Month] <= @month2 ) and ( [Year] = @year2 ))))
  or (( @year1 = @year2 ) and ( [Year] = @year1 ) and ( [Month] >= @month1 ) and ( [Month] <= @month2 )))

-- Insert the total number
insert into #TEMP_BUILDER
select Row_Type='z', group_code = '                   ', coll_code = '                    ', subcollect_code = '                    ', 'ALL OF UFDC', 
A.Hits, A.Sessions,0,0,0,0, G.Title_Hits,
Item_Hits, Item_JPEG_Views, Item_Zoomable_Views, Item_Citation_Views, Item_Thumbnail_Views,
Item_Text_Search_Views, Item_Flash_Views, Item_Google_Map_Views, Item_Download_Views, Item_Static_Views, -1, -1, -1
from #TEMP_ITEM_STATS I, #TEMP_GROUP_STATS G, #TEMP_URL_STATS A

-- The list of ALL collection groups should probably be marked 'z'
update #TEMP_BUILDER
set Row_Type = 'z', group_code = ''
where group_code='ALL'

-- Return the final table
select *
from #TEMP_BUILDER
order by group_code, coll_code, subcollect_code


drop table #TEMP_AGGREGATION_STATS
drop table #TEMP_ITEM_STATS
drop table #TEMP_GROUP_STATS
drop table #TEMP_URL_STATS
drop table #TEMP_BUILDER

END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Statistics_Aggregation_Titles]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns most often hit titles and items for an aggregation
CREATE PROCEDURE [dbo].[SobekCM_Statistics_Aggregation_Titles]
	@code varchar(20)
AS
BEGIN

	-- No need to perform any locks here.
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Is this for the whole library, or just one aggregation?
	if (( @code != 'all' ) and ( LEN(@code) > 0 ))
	begin
		-- Get the aggregation id
		declare @aggregationid int;
		set @aggregationid = ISNULL( (select AggregationID from SobekCM_Item_Aggregation where Code=@code), -1 );
		
		-- Return top 100 items
		select top 100 G.BibID, I.VID, G.GroupTitle, I.Total_Hits
		from SobekCM_Item I, SobekCM_Item_Group G, SobekCM_Item_Aggregation_Item_Link L
		where ( I.GroupID = G.GroupID )
		  and ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = @aggregationid )
		order by I.Total_Hits DESC
		
		-- Get the top 100 titles with the most hits
		select top 100 BibID, GroupTitle, SUM(I.Total_Hits) as Title_Hits
		from SobekCM_Item I, SobekCM_Item_Group G, SobekCM_Item_Aggregation_Item_Link L
		where ( I.GroupID = G.GroupID )
		  and ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = @aggregationid )
		group by BibID, GroupTitle
		order by Title_Hits DESC
	end
	else
	begin
		-- Return top 100 items, library-wide
		select top 100 G.BibID, I.VID, G.GroupTitle, I.Total_Hits
		from SobekCM_Item I, SobekCM_Item_Group G
		where ( I.GroupID = G.GroupID )
		order by I.Total_Hits DESC
		
		-- Get the top 100 titles with the most hits, library-wide
		select top 100 BibID, GroupTitle, SUM(I.Total_Hits) as Title_Hits
		from SobekCM_Item I, SobekCM_Item_Group G
		where ( I.GroupID = G.GroupID )
		group by BibID, GroupTitle
		order by Title_Hits DESC	
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Statistics_Aggregate]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Aggregates the item and title statistics to the subcollection, collection
-- and institutional level for a given month and year
CREATE PROCEDURE [dbo].[SobekCM_Statistics_Aggregate]
	@statyear int,
	@statmonth int,
	@message varchar(1000) output
AS
begin

	-- No need to perform any locks here.  We will define a transaction later
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Should only do this aggregation for each year month ONCE.
	if not exists ( select * from SobekCM_Statistics where [Year]=@statyear and [Month]=@statmonth )
	begin
		set @message='No row for this year/month is present in the SobekCM_Statistics table.  Add usage stats before trying to aggregate this month.';
		print @message;
		return;
	end;

	-- Has this been aggregated before?
	if exists ( select * from SobekCM_Statistics where Aggregate_Statistics_Complete='true' and [Year]=@statyear and [Month]=@statmonth )
	begin
		set @message='Statistics for this month have already been aggregated.  You cannot aggregate the same year/month twice without introducing errors.';
		print @message;
		return;
	end;	

	-- Get items statistics and aggregation id
	select AggregationID, Hits, JPEG_Views, Zoomable_Views, Citation_Views, Thumbnail_Views, Text_Search_Views, Flash_Views, Google_Map_Views, Download_Views, Static_Views
	into #TEMP_ITEM_AGGREGATION
	from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Statistics S
	where ( S.ItemID = I.ItemID )
	  and ( I.ItemID = L.ItemID )
	  and ( S.[Year] = @statyear )
	  and ( S.[Month] = @statmonth )
	order by AggregationID;

	-- Aggregate these statistics
	select distinct(AggregationID), sum( Hits) as Item_Hits, sum(JPEG_Views) as JPEG_Views, sum(Zoomable_Views) as Zoomable_Views, 
	  sum ( Citation_Views) as Citation_Views, sum( Thumbnail_Views ) as Thumbnail_Views, sum( Text_Search_Views) as Text_Search_Views, sum (Flash_Views) as Flash_Views,
	  sum(Google_Map_Views) as Google_Map_Views, sum(Download_Views) as Download_Views, sum(Static_Views) as Static_Views
	into #TEMP_AGGREGATION_STATS
	from #TEMP_ITEM_AGGREGATION
	Group by AggregationID;	

	-- Get group statistics and collection id
	select AggregationID, Distincter = cast(AggregationID as varchar(10)) + '_' + cast(S.GroupID as varchar(10)), S.Hits
	into #TEMP_TITLE_AGGREGATION
	from SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item I, SobekCM_Item_Group_Statistics S
	where ( I.ItemID = CL.ItemID )
	  and ( I.GroupID = S.GroupID )
	  and ( S.[Year] = @statyear )
	  and ( S.[Month] = @statmonth )
	order by Distincter;

	-- Get the distinct hits by group
	select distinct(Distincter), AggregationID, Hits
	into #TEMP_TITLE_AGGREGATION_DISTINCT
	from #TEMP_TITLE_AGGREGATION
	Group by Distincter, AggregationID, Hits;

	-- Aggregate these statistics
	select distinct(AggregationID), sum( Hits) as Title_Hits
	into #TEMP_AGGREGATION_STATS2
	from #TEMP_TITLE_AGGREGATION_DISTINCT
	Group by AggregationID;
	
	-- Now update the tables within a transaction
	begin transaction
		
		-- Add these stats to the collection list
		update SobekCM_Item_Aggregation_Statistics
		set Item_Hits = (select Item_Hits from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_JPEG_Views = (select JPEG_VIews from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_Zoomable_Views = (select Zoomable_Views from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_Citation_Views = (select Citation_Views from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_Thumbnail_Views = (select Thumbnail_Views from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_Text_Search_Views = (select Text_Search_Views from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_Flash_Views = (select Flash_Views from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_Google_Map_Views = (select Google_Map_Views from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_Download_Views = (select Download_Views from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID ),
			Item_Static_Views = (select Static_Views from #TEMP_AGGREGATION_STATS where #TEMP_AGGREGATION_STATS.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID )
		where [Year]=@statyear and [Month] = @statmonth;
		
		-- If there is no matching row though, insert this
		insert into SobekCM_Item_Aggregation_Statistics ( AggregationID, [Year],    [Month],     Hits, [Sessions], Home_Page_Views, Browse_Views, Advanced_Search_Views, Search_Results_Views, Title_Hits, Item_Hits, Item_JPEG_Views, Item_Zoomable_Views, Item_Citation_Views, Item_Thumbnail_Views, Item_Text_Search_Views, Item_Flash_Views, Item_Google_Map_Views, Item_Download_Views, Item_Static_Views )
		select                                            AggregationID, @statyear, @statmonth,  0,    0,          0,               0,            0,                     0,                    0,          Item_Hits, JPEG_Views,      Zoomable_Views,      Citation_Views,      Thumbnail_Views,      Text_Search_Views,      Flash_Views,      Google_Map_Views,      Download_Views,      Static_Views
		from #TEMP_AGGREGATION_STATS
		where not exists ( select * from SobekCM_Item_Aggregation_Statistics S where S.AggregationID = #TEMP_AGGREGATION_STATS.AggregationID and S.[Year] = @statyear and S.[Month] = @statmonth );

		-- Add these stats to the collection list
		update SobekCM_Item_Aggregation_Statistics
		set Title_Hits = (select Title_Hits from #TEMP_AGGREGATION_STATS2 where #TEMP_AGGREGATION_STATS2.AggregationID = SobekCM_Item_Aggregation_Statistics.AggregationID )
		where [Year]=@statyear and [Month] = @statmonth;

		-- Update the total hits at collection level
		update SobekCM_Item_Aggregation_Statistics
		set Hits = isnull( Hits + Title_Hits + Item_Hits, 0)
		where [Year] = @statyear and [Month] = @statmonth;

		-- Update the total number of hits on the items	
		UPDATE SobekCM_Item
		set Total_Hits = isnull(( select SUM(Hits) from SobekCM_Item_Statistics S where S.ItemID=SobekCM_Item.ItemID ), 0),
			Total_Sessions = isnull(( select SUM([Sessions]) from SobekCM_Item_Statistics S where S.ItemID=SobekCM_Item.ItemID ), 0);
			
		-- Update the top level that these have been aggregated
		UPDATE SobekCM_Statistics
		SET Aggregate_Statistics_Complete='true'
		where [Year]=@statyear and [Month]=@statmonth;
		
	commit transaction;

	-- Update which users are linked to items with statistics
	update mySobek_User
	set Has_Item_Stats='true'
	where exists ( select * 
				   from mySobek_User_Item_Link L, mySobek_User_Item_Link_Relationship R, SobekCM_Item_Statistics S
				   where L.UserID=mySObek_User.UserID
				     and L.RelationshipID=R.RelationshipID 
				     and R.Include_In_Results = 'true' 
				     and L.ItemID=S.ItemID
				     and S.Hits > 0 );
				   
	-- drop the temporary tables
	drop table #TEMP_ITEM_AGGREGATION;
	drop table #TEMP_AGGREGATION_STATS;
	drop table #TEMP_TITLE_AGGREGATION;
	drop table #TEMP_TITLE_AGGREGATION_DISTINCT;
	drop table #TEMP_AGGREGATION_STATS2;
end;
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Add_Workflow_Once_Per_Day]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Add_Workflow_Once_Per_Day]
	@bibid varchar(10),
	@vid varchar(5),
	@user varchar(50),
	@progressnote varchar(1000),
	@workflowid int,
	@storagelocation varchar(255)
AS
begin transaction

	-- Get the volume id
	declare @itemid int

	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid)

	-- continue if an itemid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Does a progress already exist for this which is not completed?
		if ( (select count(*) from Tracking_Progress where ( ItemID = @itemid ) and ( WorkFlowID = @workflowid ) and ( DateCompleted is null )) > 0 )
		begin
			-- If this is to mark it complete, alter existing progress
			update Tracking_Progress
			set DateCompleted = getdate(), WorkPerformedBy = @user, WorkingFilePath=@storagelocation, ProgressNote = @progressnote
			where ( ItemID = @itemid ) and ( WorkFlowID = @workflowid ) and ( DateCompleted is null )
		end
		else
		begin
			-- only enter one of these per day
			if ( (select count(*) from Tracking_Progress where ( ItemID = @itemid ) and ( WorkFlowID=@workflowid ) and ( isnull( CONVERT(VARCHAR(10), DateCompleted, 102), '') = CONVERT(VARCHAR(10), getdate(), 102) )) = 0 )
			begin
				-- Just add this new progress then
				insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
				values ( @itemid, @workflowid, getdate(), @user, @progressnote, @storagelocation )
			end
		end
	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Add_Workflow_By_ItemID]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Add_Workflow_By_ItemID]
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
		declare @workflowid int
		if ( ( select COUNT(*) from Tracking_WorkFlow where ( WorkFlowName=@workflow)) > 0 )
		begin
			-- Get the existing ID for this workflow
			select @workflowid = workflowid from Tracking_WorkFlow where WorkFlowName=@workflow
		end
		else
		begin 
			-- Create the workflow for this
			insert into Tracking_WorkFlow ( WorkFlowName, WorkFlowNotes )
			values ( @workflow, 'Added ' + CONVERT(VARCHAR(10), GETDATE(), 101) + ' by ' + @user )
			
			-- Get this ID
			set @workflow = @@IDENTITY
		end
	
		-- Just add this new progress then
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
		values ( @itemid, @workflowid, GETDATE(), @user, @progressnote, @storagelocation )
	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Add_Workflow]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Add_Workflow]
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
	    and ( VID = @vid)
	    
	-- continue if an itemid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Get the workflow id
		declare @workflowid int
		if ( ( select COUNT(*) from Tracking_WorkFlow where ( WorkFlowName=@workflow)) > 0 )
		begin
			-- Get the existing ID for this workflow
			select @workflowid = workflowid from Tracking_WorkFlow where WorkFlowName=@workflow
		end
		else
		begin 
			-- Create the workflow for this
			insert into Tracking_WorkFlow ( WorkFlowName, WorkFlowNotes )
			values ( @workflow, 'Added ' + CONVERT(VARCHAR(10), GETDATE(), 101) + ' by ' + @user )
			
			-- Get this ID
			set @workflow = @@IDENTITY
		end
	
		-- Just add this new progress then
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
		values ( @itemid, @workflowid, GETDATE(), @user, @progressnote, @storagelocation )
	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Add_Past_Workflow_By_ItemID]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Add_Past_Workflow_By_ItemID]
	@itemid int,
	@user varchar(50),
	@progressnote varchar(1000),
	@workflow varchar(100),
	@storagelocation varchar(255),
	@date datetime
AS
begin transaction
	    
	-- continue if an itemid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Get the workflow id
		declare @workflowid int
		if ( ( select COUNT(*) from Tracking_WorkFlow where ( WorkFlowName=@workflow)) > 0 )
		begin
			-- Get the existing ID for this workflow
			select @workflowid = workflowid from Tracking_WorkFlow where WorkFlowName=@workflow
		end
		else
		begin 
			-- Create the workflow for this
			insert into Tracking_WorkFlow ( WorkFlowName, WorkFlowNotes )
			values ( @workflow, 'Added ' + CONVERT(VARCHAR(10), GETDATE(), 101) + ' by ' + @user )
			
			-- Get this ID
			set @workflow = @@IDENTITY
		end
	
		-- Just add this new progress then
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
		values ( @itemid, @workflowid, @date, @user, @progressnote, @storagelocation )
	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Add_Past_Workflow]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Add_Past_Workflow]
	@bibid varchar(10),
	@vid varchar(5),
	@user varchar(50),
	@progressnote varchar(1000),
	@workflow varchar(100),
	@storagelocation varchar(255),
	@date datetime
AS
begin transaction

	-- Get the volume id
	declare @itemid int
	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid)
	    
	-- continue if an itemid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Get the workflow id
		declare @workflowid int
		if ( ( select COUNT(*) from Tracking_WorkFlow where ( WorkFlowName=@workflow)) > 0 )
		begin
			-- Get the existing ID for this workflow
			select @workflowid = workflowid from Tracking_WorkFlow where WorkFlowName=@workflow
		end
		else
		begin 
			-- Create the workflow for this
			insert into Tracking_WorkFlow ( WorkFlowName, WorkFlowNotes )
			values ( @workflow, 'Added ' + CONVERT(VARCHAR(10), GETDATE(), 101) + ' by ' + @user )
			
			-- Get this ID
			set @workflow = @@IDENTITY
		end
	
			-- Make sure a previously entered workflow does not already exist for this date
		if ((select count(*) from Tracking_Progress where ( ItemID = @itemid ) and ( WorkFlowID=@workflowid ) and ( isnull( CONVERT(VARCHAR(10), DateCompleted, 102), '') = CONVERT(VARCHAR(10), @date, 102) )) = 0 )
		begin
			-- Just add this new progress then
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
			values ( @itemid, @workflowid, @date, @user, @progressnote, @storagelocation )
		end
	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Item_Milestone_Report]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Item_Milestone_Report]
	@aggregation_code varchar(20)
AS
BEGIN

	if ( LEN( ISNULL( @aggregation_code,'')) = 0 )
	begin
      select CASE Last_MileStone
                        WHEN 0 THEN 'NO WORK COMPLETED'
                        WHEN 1 THEN 'SCANNED'
                        WHEN 2 THEN 'PROCESSED'
                        WHEN 3 THEN 'QC PERFORMED'
                        WHEN 4 THEN 'ONLINE COMPLETE'
                        ELSE 'DATABASE ERROR'
                  END AS MileStone, title_count=count(distinct(GroupID)), item_count=count(*), page_count = SUM([PageCount]), file_count=SUM(FileCount), Last_MileStone
      from SobekCM_Item
      group by Last_MileStone
      union
      select 'TOTAL', title_count=count(distinct(GroupID)), item_count=count(*), page_count = SUM([PageCount]), file_count=SUM(FileCount), -1
      from SobekCM_Item
      order by Last_MileStone DESC
    end
    else
    begin
    
		declare @aggregationid int
		set @aggregationid = (select top 1 AggregationID from SobekCM_Item_Aggregation where Code=@aggregation_code)
		
		if ( ISNULL(@aggregationid,-1) > 0 )
		begin
		      select CASE Last_MileStone
                        WHEN 0 THEN 'NO WORK COMPLETED'
                        WHEN 1 THEN 'SCANNED'
                        WHEN 2 THEN 'PROCESSED'
                        WHEN 3 THEN 'QC PERFORMED'
                        WHEN 4 THEN 'ONLINE COMPLETE'
                        ELSE 'DATABASE ERROR'
                  END AS MileStone, title_count=count(distinct(GroupID)), item_count=count(*), page_count = SUM([PageCount]), file_count=SUM(FileCount), Last_MileStone
			  from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L
			  where ( I.ItemID = L.ItemID ) and ( L.AggregationID = @aggregationid )
			  group by Last_MileStone
			  union
			  select 'TOTAL', title_count=count(distinct(GroupID)), item_count=count(*), page_count = SUM([PageCount]), file_count=SUM(FileCount), -1
			  from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L
			  where ( I.ItemID = L.ItemID ) and ( L.AggregationID = @aggregationid )
			  order by Last_MileStone DESC
		end   
    end
END
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Get_History_Archives]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Get_History_Archives]
	@itemid int
AS
BEGIN	
		-- The first return table is the CD information for this volume
		select CD_Number = ArchiveNumber, File_Range = FileRangeOnCD, Images,  [Size]= ltrim(rtrim(convert(varchar(20),imagesize_kb)))+'KB',  Date_Burned = convert(varchar(10),DateSorted,101) 
		from Tracking_Archive_Item_Link L, Tracking_ArchiveMedia M
		where (L.ItemID = @itemid )
		  and (L.ArchiveMediaID = M.ArchiveMediaID )
		order by ArchiveNumber ASC

		-- The second return table is the progress information for this volume
		select P.WorkFlowID, [Workflow Name]=WorkFlowName, [Completed Date]=isnull(CONVERT(CHAR(10), DateCompleted, 102),''), WorkPerformedBy=isnull(WorkPerformedBy, ''), WorkingFilePath=isnull(WorkingFilePath,''), Note=isnull(ProgressNote,'')
		from Tracking_Progress P, Tracking_Workflow W
		where (P.workflowid = W.workflowid)
		  and (P.ItemID = @itemid )
		order by DateCompleted ASC		
		
		-- The third return table is all the files from TIVOLI
		select BibID, VID, Folder, [FileName], Size, LastWriteDate, ArchiveDate
		from Tivoli_File_Log
		where ItemID = @itemid	
		
		-- The fourth return table has the item information (used by SMaRT app)
		select * from SobekCM_Item where ItemID=@itemid
		
		-- The fifth return table has the aggregations this item is linked to
		select A.Code, A.Name, A.ShortName, A.[Type], L.impliedLink, A.Hidden, A.isActive
		from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
		where ( L.ItemID = @ItemID )
		  and ( A.AggregationID = L.AggregationID )
			
END
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Get_Aggregation_Privates]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return the browse of all PRIVATE or DARK items for a single aggregation
CREATE PROCEDURE [dbo].[Tracking_Get_Aggregation_Privates]
	@code varchar(20),
	@pagesize int, 
	@pagenumber int,
	@sort int,	
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@total_items int output,
	@total_titles int output
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	declare @TEMP_ITEMS table ( ItemID int, fk_TitleID int, LastActivityDate datetime, LastActivityType varchar(100), LastMilestone_Date datetime, LastMilestone int );	
	declare @TEMP_TITLES table ( BibID varchar(10), fk_TitleID int, GroupTitle nvarchar(1000), LastActivityDate datetime, LastMilestone_Date datetime, RowNumber int);
	
	-- Do not need to maintain row counts
	Set NoCount ON
	
	-- Determine the start and end rows
	declare @rowstart int; 
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@code );
	  
	-- Populate the entire temporary item list	
	insert into @TEMP_ITEMS ( ItemID, fk_TitleID, LastMilestone, LastMilestone_Date )
	select I.ItemID, I.GroupID, I.Last_MileStone, 
		CASE I.Last_MileStone 
			WHEN 1 THEN I.Milestone_DigitalAcquisition
			WHEN 2 THEN I.Milestone_ImageProcessing
			WHEN 3 THEN I.Milestone_QualityControl
			WHEN 4 THEN I.Milestone_OnlineComplete
			ELSE I.CreateDate
		END									
	from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
	where ( CL.ItemID = I.ItemID )
	  and ( CL.AggregationID = @aggregationid )
	  and ( I.Deleted = 'false' )
	  and (( I.IP_Restriction_Mask < 0 ) or ( I.Dark = 'true' ));
		
	-- Using common table expressions, add the latest activity and activity type
	with CTE AS (
		select P.ItemID, DateCompleted, WorkFlowName,
		   Rnum=ROW_NUMBER() OVER ( PARTITION BY P.ItemID ORDER BY DateCompleted DESC )
		from Tracking_Progress P, @TEMP_ITEMS T, Tracking_WorkFlow W
		where P.ItemID=T.ItemID and P.WorkFlowID = W.WorkFlowID)
	update I
	set LastActivityDate=cte.DateCompleted, LastActivityType=cte.WorkFlowName
	from @TEMP_ITEMS I INNER JOIN CTE ON CTE.ItemID=I.ItemID and Rnum=1;
	
	-- Set the total counts
	select @total_items=COUNT(ItemID), @total_titles=COUNT(distinct fk_TitleID)
	from @TEMP_ITEMS;
		  
	-- Now, calculate the actual ending row, based on the ration, page information,
	-- and the lookahead factor		
	-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
	if (( @total_items > 0 ) and ( @total_titles > 0 ))
	begin
		declare @computed_value int;
		select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
		
		-- Compute the minimum value.  This cannot be less than @minpagelookahead.
		declare @floored_value int;
		select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
		
		-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
		declare @actual_pages int;
		select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead));

		-- Set the final row again then
		set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1;  
	end;
	
	-- SORT ORDERS
	-- 0 = BibID/VID
	-- 1 = Title/VID
	-- 2 = Last Activity Date (most recent first)
	-- 3 = Last Milestone Date (most recent first)
	-- 4 = Last Activity Date (oldest first)
	-- 5 = Last Milestone Date (oldest forst)
	if (( @sort != 4 ) and ( @sort != 5 ))
	begin
		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
		 (	select fk_TitleID, MAX(I.LastActivityDate) as MaxActivityDate, MAX(I.LastMilestone_Date) as MaxMilestoneDate,
				ROW_NUMBER() OVER (order by case when @sort=0 THEN G.BibID end,
											case when @sort=1 THEN G.SortTitle end,
											case when @sort=2 THEN MAX(I.LastActivityDate) end DESC,
											case when @sort=3 THEN MAX(I.LastMilestone_Date) end DESC) as RowNumber
				from @TEMP_ITEMS I, SobekCM_Item_Group G
				where ( I.fk_TitleID = G.GroupID )
				group by fk_TitleID, G.BibID, G.SortTitle )
			  
		-- Insert the correct rows into the temp title table	
		insert into @TEMP_TITLES ( BibID, fk_TitleID, GroupTitle, LastActivityDate, LastMilestone_Date, RowNumber )
		select G.BibID, S.fk_TitleID, G.GroupTitle, S.MaxActivityDate, S.MaxMilestoneDate, RowNumber
		from TITLES_SELECT S, SobekCM_Item_Group G
		where S.fk_TitleID = G.GroupID
		  and RowNumber >= @rowstart
		  and RowNumber <= @rowend;
	end
	else
	begin
		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
		 (	select fk_TitleID, MIN(I.LastActivityDate) as MaxActivityDate, MIN(I.LastMilestone_Date) as MaxMilestoneDate,
				ROW_NUMBER() OVER (order by case when @sort=4 THEN MIN(I.LastActivityDate) end ASC,
											case when @sort=5 THEN MIN(I.LastMilestone_Date) end ASC) as RowNumber
				from @TEMP_ITEMS I, SobekCM_Item_Group G
				where ( I.fk_TitleID = G.GroupID )
				group by fk_TitleID, G.BibID, G.SortTitle )
			  
		-- Insert the correct rows into the temp title table	
		insert into @TEMP_TITLES ( BibID, fk_TitleID, GroupTitle, LastActivityDate, LastMilestone_Date, RowNumber )
		select G.BibID, S.fk_TitleID, G.GroupTitle, S.MaxActivityDate, S.MaxMilestoneDate, RowNumber
		from TITLES_SELECT S, SobekCM_Item_Group G
		where S.fk_TitleID = G.GroupID
		  and RowNumber >= @rowstart
		  and RowNumber <= @rowend;
	end;
	
	-- Return the title information
	select RowNumber, G.BibID, G.GroupTitle, G.[Type], G.ALEPH_Number, G.OCLC_Number, T.LastActivityDate, T.LastMilestone_Date, G.ItemCount, isnull(G.Primary_Identifier_Type, '') as Primary_Identifier_Type, isnull(G.Primary_Identifier,'') as Primary_Identifier
	from @TEMP_TITLES T, SobekCM_Item_Group G
	where T.fk_TitleID = G.GroupID
	order by RowNumber;
	
	-- Return the item informaiton
	select T.RowNumber, VID, Title, isnull(Internal_Comments,'') as Internal_Comments, isnull(PubDate,'') as PubDate, Locally_Archived, Remotely_Archived, AggregationCodes, I.LastActivityDate, I.LastActivityType, I.LastMilestone, I.LastMilestone_Date, Born_Digital, Material_Received_Date, isnull(DAT.DispositionFuture,'') AS Disposition_Advice, Disposition_Date, isnull(DT.DispositionPast,'') AS Disposition_Type, Tracking_Box
	from @TEMP_ITEMS AS I inner join
		 @TEMP_TITLES AS T ON I.fk_TitleID=T.fk_TitleID inner join
		 SobekCM_Item AS I2 ON I.ItemID = I2.ItemID left outer join
		 Tracking_Disposition_Type AS DAT ON I2.Disposition_Advice=DAT.DispositionID left outer join
		 Tracking_Disposition_Type AS DT ON I2.Disposition_Type=DT.DispositionID
	order by T.RowNumber ASC, case when @sort=0 THEN VID end,
							case when @sort=1 THEN VID end,
							case when @sort=2 THEN I.LastActivityDate end DESC,
							case when @sort=3 THEN I.LastMilestone_Date end DESC,
							case when @sort=4 THEN I.LastActivityDate end ASC,
							case when @sort=5 THEN I.LastMilestone_Date end ASC;		 
			
    Set NoCount OFF;

end;
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Get_Aggregation_Browse]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return a single browse for a collection or group
-- This is identical to SobekCM_Get_Aggregation_Browse, EXCEPT:
--    (1) Title, Author, IP restriction flag, Thumbnail, and Publisher returned with item list
--    (2) Group title, thumbnail, type, OCLC, and ALEPH returned with title list
--    (3) No facets returned and both PRIVATE and PUBLIC items are returned in list
--    (4) Always returns all the items, not just NEW items
--    (5) All tracking information ( milestones, tracking box, etc..) returned in item list
CREATE PROCEDURE [dbo].[Tracking_Get_Aggregation_Browse]
	@code varchar(20)
AS
begin

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #TEMP_ITEMS ( BibID varchar(10), VID varchar(5), SortDate bigint, Spatial_KML varchar(4000), fk_TitleID int, Title nvarchar(500), IP_Restriction_Mask smallint, ItemID int, Hit_Count int, Author nvarchar(1000), Publisher nvarchar(1000), Thumbnail varchar(100), Internal_Comments nvarchar(1000), Dark bit, Last_MileStone int, Milestone_DigitalAcquisition datetime, Milestone_ImageProcessing datetime, Milestone_QualityControl datetime, Milestone_OnlineComplete datetime, Born_Digital bit, Material_Received_Date datetime, Disposition_Advice int, Disposition_Date datetime, Disposition_Type int, Locally_Archived bit, Remotely_Archived bit, Tracking_Box varchar(25), AggregationCodes varchar(100), Level1_Text nvarchar(255), Level1_Index int, Level2_Text nvarchar(255), Level2_Index int, Level3_Text nvarchar(255), Level3_Index int, Level4_Text nvarchar(255), Level4_Index int, Level5_Text nvarchar(255), Level5_Index int, SortTitle nvarchar(500), PubDate nvarchar(100))
	create table #TEMP_TITLES ( fk_TitleID int );

	-- Populate the temporary item list	
	insert into #TEMP_ITEMS ( BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, IP_Restriction_Mask, ItemID, Author, Publisher, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, Disposition_Advice, Disposition_Date, Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, SortTitle, PubDate )
	select G.BibID, I.VID, isnull( I.SortDate,-1), isnull(Spatial_KML,''), I.GroupID, I.Title, I.IP_Restriction_Mask, I.ItemID, I.Author, I.Publisher, I.MainThumbnail, I.Internal_Comments, I.Dark, I.Last_MileStone, I.Milestone_DigitalAcquisition, I.Milestone_ImageProcessing, I.Milestone_QualityControl, I.Milestone_OnlineComplete, I.Born_Digital, I.Material_Received_Date, I.Disposition_Advice, I.Disposition_Date, I.Disposition_Type, I.Locally_Archived, I.Remotely_Archived, I.Tracking_Box, I.AggregationCodes, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), SortTitle=ISNULL(I.SortTitle, UPPER(Title)), I.PubDate
	from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Aggregation C, SobekCM_Item_Group G
	where ( CL.ItemID = I.ItemID )
	  and ( CL.AggregationID = C.AggregationID )
	  and ( C.Code = @code )
	  and ( I.Deleted = CONVERT(bit,0) )
	  and ( I.GroupID = G.GroupID );
		
	-- Select the title information
	insert into #TEMP_TITLES ( fk_TitleID )
	select distinct(fk_TitleID)
	from #TEMP_ITEMS I
	group by fk_TitleID;

	-- Return the item informaiton
	select BibID, VID, SortDate, Spatial_KML, fk_TitleID, Title, Author, Publisher, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, IP_Restriction_Mask, Thumbnail, Internal_Comments, Dark, Last_MileStone, Milestone_DigitalAcquisition, Milestone_ImageProcessing, Milestone_QualityControl, Milestone_OnlineComplete, Born_Digital, Material_Received_Date, isnull(DAT.DispositionFuture,'') AS Disposition_Advice, Disposition_Date, isnull(DT.DispositionPast,'') AS Disposition_Type, Locally_Archived, Remotely_Archived, Tracking_Box, AggregationCodes, ItemID, SortTitle, PubDate
	from #TEMP_ITEMS AS T left outer join
		 Tracking_Disposition_Type AS DAT ON T.Disposition_Advice=DAT.DispositionID left outer join
		 Tracking_Disposition_Type AS DT ON T.Disposition_Type=DT.DispositionID;

	-- Return the title information
	select G.BibID, G.SortTitle, TitleID=G.GroupID, [Rank]=-1, G.GroupTitle, G.GroupThumbnail, G.[Type], G.ALEPH_Number, G.OCLC_Number, isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
	from SobekCM_Item_Group G, #TEMP_TITLES I
	where  G.GroupID = I.fk_TitleID;
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	drop table #TEMP_TITLES;
			
    Set NoCount OFF;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Link_User_To_Item]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Links an item to an item with a given relationship.  This is used for reporting
-- usage statistics to individuals, etc..
CREATE PROCEDURE [dbo].[SobekCM_Link_User_To_Item]
	@ItemID int,
	@UserID int,
	@RelationshipID int,
	@Change_Existing bit
AS
BEGIN

	-- Only continue if there is currently no link between the user and the item
	if (( select COUNT(*) from mySobek_User_Item_Link L where L.UserID=@UserID and L.ItemID=@ItemID ) = 0 )
	begin	
		-- Add this link
		insert into mySobek_User_Item_Link ( UserID, ItemID, RelationshipID )
		values ( @UserID, @ItemID, @RelationshipID );
	end
	else if ( @Change_Existing = 'true' )
	begin
	
		-- Change the existing
		update mySobek_User_Item_Link
		set RelationshipID=@RelationshipID
		where UserID=@UserID and ItemID=@ItemID;
	
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Mass_Update_Item_Aggregation_Link]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a link to the item aggregation (and all parents) to all the items
-- within a particular item group
CREATE PROCEDURE [dbo].[SobekCM_Mass_Update_Item_Aggregation_Link]
	@groupid int,
	@code varchar(20)
AS
begin

	-- Only continue if the code exists
	if ( len( isnull( @code,'')) > 0 )
	begin
		-- Ensure this aggregation code exists
		if ( @code in ( select Code from SobekCM_Item_Aggregation ))
		begin
			-- Get the ID for this aggregation code
			declare @AggregationID int;
			select @AggregationID = AggregationID from SobekCM_Item_Aggregation where Code = @code;
			
			-- For any existing links, make sure this does not say implied, since this was explicitly connected
			update SobekCM_Item_Aggregation_Item_Link
			set impliedLink = 'false'
			where ( AggregationID = @AggregationID ) 
			  and exists ( select * from SobekCM_Item I where I.GroupID=@GroupID and I.ItemID=SobekCM_Item_Aggregation_Item_Link.ItemID );

			-- Tie this item to this primary collection, if not present
			insert into SobekCM_Item_Aggregation_Item_Link ( AggregationID, ItemID, impliedLink )
			select @AggregationID, I.ItemID, 'false' 
			from SobekCM_Item I 
			where I.GroupID = @groupid
			  and not exists ( select * from SobekCM_Item_Aggregation_Item_Link L where L.ItemID = I.ItemID and L.AggregationID = @AggregationID );
			
			-- Update the last item added date time
			update SobekCM_Item_Aggregation
			set LastItemAdded = ( select top 1 Milestone_OnlineComplete from SobekCM_Item where GroupID=@groupid and Milestone_OnlineComplete is not null order by Milestone_OnlineComplete DESC )
			where AggregationID = @AggregationID
			  and LastItemAdded < ( select top 1 Milestone_OnlineComplete from SobekCM_Item where GroupID=@groupid and Milestone_OnlineComplete is not null order by Milestone_OnlineComplete DESC )
			  and exists ( select Milestone_OnlineComplete from SobekCM_Item where GroupID=@groupid and Milestone_OnlineComplete is not null );

			-- Select parent codes
			select P.Code, P.AggregationID, Hierarchy=1
			into #TEMP_PARENTS
			from SobekCM_Item_Aggregation C, SobekCM_Item_Aggregation P, SobekCM_Item_Aggregation_Hierarchy H
			where ( C.AggregationID = H.ChildID )
			  and ( P.AggregationID = H.ParentID )
			  and ( C.Code = @code )
			  and ( H.Search_Parent_Only = 'false' );

			-- Select the grandparent codes
			insert into #TEMP_PARENTS ( Code, AggregationID, Hierarchy)
			select P.Code, P.AggregationID, 2 
			from #TEMP_PARENTS C, SobekCM_Item_Aggregation P, SobekCM_Item_Aggregation_Hierarchy H
			where ( C.AggregationID = H.ChildID )
			  and ( P.AggregationID = H.ParentID )
			  and ( H.Search_Parent_Only = 'false' );

			-- Select the grand-grandparent codes
			insert into #TEMP_PARENTS ( Code, AggregationID, Hierarchy)
			select P.Code, P.AggregationID, 3
			from #TEMP_PARENTS C, SobekCM_Item_Aggregation P, SobekCM_Item_Aggregation_Hierarchy H
			where ( C.AggregationID = H.ChildID )
			  and ( P.AggregationID = H.ParentID )
			  and ( C.Hierarchy = 2 )
			  and ( H.Search_Parent_Only = 'false' );

			-- Select the grand-grand-grandparent codes
			insert into #TEMP_PARENTS ( Code, AggregationID, Hierarchy)
			select P.Code, P.AggregationID, 4
			from #TEMP_PARENTS C, SobekCM_Item_Aggregation P, SobekCM_Item_Aggregation_Hierarchy H
			where ( C.AggregationID = H.ChildID )
			  and ( P.AggregationID = H.ParentID )
			  and ( C.Hierarchy = 3 )
			  and ( H.Search_Parent_Only = 'false' );

			-- Insert the link anywhere it does not exist
			insert into SobekCM_Item_Aggregation_Item_Link ( AggregationID, ItemID, impliedLink )
			select AggregationID, I.ItemID, 'true'
			from #TEMP_PARENTS P, SobekCM_Item I
			where I.GroupID=@groupid 
			  and not exists ( select * 
								from SobekCM_Item_Aggregation_Item_Link L
								where ( P.AggregationID = L.AggregationID )
								  and ( L.ItemID = I.ItemID ));
								  
			-- Also update the last item added date
			update SobekCM_Item_Aggregation
			set LastItemAdded = ( select top 1 Milestone_OnlineComplete from SobekCM_Item where GroupID=@groupid and Milestone_OnlineComplete is not null order by Milestone_OnlineComplete DESC )
			where exists ( select * from #TEMP_PARENTS T where T.AggregationID=SobekCM_Item_Aggregation.AggregationID )
			  and LastItemAdded < ( select top 1 Milestone_OnlineComplete from SobekCM_Item where GroupID=@groupid and Milestone_OnlineComplete is not null order by Milestone_OnlineComplete DESC )
			  and exists ( select Milestone_OnlineComplete from SobekCM_Item where GroupID=@groupid and Milestone_OnlineComplete is not null );

			-- drop the temporary table
			drop table #TEMP_PARENTS;
		end;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_MarcXML_Test_Feed]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Pulls the list of items for MARC XML Automation during
-- load of records to test mango
CREATE PROCEDURE [dbo].[SobekCM_MarcXML_Test_Feed]
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get the list of each BibID and the first VID
	with temp as (
		select G.BibID, G.GroupID, I.VID, I.ItemID, CreateDate, File_Location,
    		row_number() over (partition by G.GroupID order by I.VID) as rownum
		from SobekCM_Item_Group G, SobekCM_Item I
		where ( G.GroupID=I.GroupID )
		  and ( I.Deleted = 'false' )
		  and ( I.IP_Restriction_Mask = 0 )
		  and ( G.Deleted = 'false' )
		  and ( G.Include_In_MarcXML_Test_Feed = 'true' )
	)
	select BibID, GroupID, VID, ItemID, CreateDate, File_Location
	into #ONE_VID_PER_BIB
	from temp 
	where rownum = 1;
	
	-- Get the list of all public items which are marked to include in 
	-- the marc xml production feed
	select BibID, VID, CreateDate, CollectionCode=C.Code, File_Location
	from #ONE_VID_PER_BIB I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Aggregation C
	where ( CL.ItemID = I.ItemID )
	  and ( CL.AggregationID = C.AggregationID )
	  and ( CL.impliedLink = 'false' )
	order by BibID;
	  
	-- Drop the temporary table we are completed with
	drop table #ONE_VID_PER_BIB;


END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_MarcXML_Production_Feed]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Pulls the list of items for MARC XML Automation during
-- load of records to production mango
CREATE PROCEDURE [dbo].[SobekCM_MarcXML_Production_Feed]
AS
BEGIN
	
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get the list of each BibID and the first VID
	with temp as (
		select G.BibID, G.GroupID, I.VID, I.ItemID, CreateDate, File_Location,
    		row_number() over (partition by G.GroupID order by I.VID) as rownum
		from SobekCM_Item_Group G, SobekCM_Item I
		where ( G.GroupID=I.GroupID )
		  and ( I.Deleted = 'false' )
		  and ( I.IP_Restriction_Mask = 0 )
		  and ( G.Deleted = 'false' )
		  and ( G.Include_In_MarcXML_Prod_Feed = 'true' )
	)
	select BibID, GroupID, VID, ItemID, CreateDate, File_Location
	into #ONE_VID_PER_BIB
	from temp 
	where rownum = 1;
	
	-- Get the list of all public items which are marked to include in 
	-- the marc xml production feed
	select BibID, VID, CreateDate, CollectionCode=C.Code, File_Location
	from #ONE_VID_PER_BIB I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Aggregation C
	where ( CL.ItemID = I.ItemID )
	  and ( CL.AggregationID = C.AggregationID )
	  and ( CL.impliedLink = 'false' )
	order by BibID;
	  
	-- Drop the temporary table we are completed with
	drop table #ONE_VID_PER_BIB;


END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Metadata_Save_Single]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Save a single piece of metadata to the database
-- This is the mapping from metadata name to metadata typeid
CREATE PROCEDURE [dbo].[SobekCM_Metadata_Save_Single]
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
/****** Object:  StoredProcedure [dbo].[SobekCM_Online_Archived_Space]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Determine the size of the online and archived spaces for the whole
-- system, a single item aggregation, or the intersection between two
-- aggregations.
-- Both include_online and include_archive args function as follows:
--   1 = provide complete sum
--   2 = break into year/month (can take a good bit of server cpu)
-- For the TIVOLI data to be up to date, you may need to run the
-- Tivoli_Admin_Update stored procedure first
CREATE PROCEDURE [dbo].[SobekCM_Online_Archived_Space]
	@code1 varchar(20),
	@code2 varchar(20),
	@include_online smallint,
	@include_archive smallint
AS
begin

	-- No need to perform any locks here, especially given the possible
	-- length of this query
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- If there are two provided codes, show the union of the two codes
	if ( LEN( ISNULL ( @code2, '' )) > 0 )
	begin
	
		-- Get the amount online by year/month/item for the intersect between these two aggregations
		select I.CreateYear, I.CreateMonth, I.DiskSize_MB AS DiskSize, I.ItemID, TivoliSize_MB
		into #TEMP_ITEMS_ONLINE
		from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
		where ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = A.AggregationID )
		  and ( A.Code = @code1 )
		  and ( CreateYear > 0 )
		  and ( CreateMonth > 0 )
		intersect
		select I2.CreateYear, I2.CreateMonth, I2.DiskSize_MB AS DiskSize, I2.ItemID, TivoliSize_MB
		from SobekCM_Item I2, SobekCM_Item_Aggregation_Item_Link L2, SobekCM_Item_Aggregation A2
		where ( I2.ItemID = L2.ItemID )
		  and ( L2.AggregationID = A2.AggregationID )
		  and ( A2.Code = @code2 )
		  and ( CreateYear > 0 )
		  and ( CreateMonth > 0 );
		  
	
						 
		-- If the online flag is ONE, just return total size
		if ( @include_online = 1 )
		begin
			-- Get the total online 
			select CAST((SUM(DiskSize))/(1024*1024) as varchar(15)) + ' GB'
			from #TEMP_ITEMS_ONLINE;
		end;
			 
		-- If the online flag is TWO, return by month/year		 
		if ( @include_online = 2 )
		begin
			-- Get the total online by year/month
			select CreateYear, CreateMonth, SUM(DiskSize)
			from #TEMP_ITEMS_ONLINE
			group by CreateYear, CreateMonth
			order by CreateYear, CreateMonth;
		end;
		
		-- If the archive flag is ONE, just return total size
		if ( @include_archive = 1 )
		begin
			-- Get the total tivolid
			select CAST((SUM(TivoliSize_MB))/(1024*1024) as varchar(15)) + ' GB'
			from #TEMP_ITEMS_ONLINE;
		end;
		
		-- If the archive flag is TWO, return by month/year
		if ( @include_archive = 2 )
		begin
			-- Get the archived amount by year/month/item for this aggregation
			select ArchiveYear, ArchiveMonth, SUM(Size)/(1024*1024) AS DiskSize
			from #TEMP_ITEMS_ONLINE T, Tivoli_File_Log A
			where ( A.ItemID=T.ItemID )
			group by ArchiveYear, ArchiveMonth;
		end;
		
		-- drop the temporary tables
		drop table #TEMP_ITEMS_ONLINE;
	
	end
	else
	begin	
	
		-- Is this for ALL items?
		if (( LEN(@code1 ) > 0 ) and ( @code1 != 'all' ))
		begin
			-- If the online flag is ONE, just return total size
			if ( @include_online = 1 )
			begin
				-- Get the total online 
				select CAST((SUM(DiskSize_MB))/(1024*1024) as varchar(15)) + ' GB'
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
				where ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Code = @code1 );
			end;
				 
			-- If the online flag is TWO, return by month/year		 
			if ( @include_online = 2 )
			begin
				-- Get the total online by year/month
				select I.CreateYear, I.CreateMonth, SUM(I.DiskSize_MB) AS DiskSize
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
				where ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Code = @code1 )
				  and ( CreateYear > 0 )
				  and ( CreateMonth > 0 )
				group by I.CreateYear, I.CreateMonth
				order by I.CreateYear, I.CreateMonth;
			end;
			
			-- If the archive flag is ONE, just return total size
			if ( @include_archive = 1 )
			begin
				-- Get the TOTAL archived amount for this aggregation
				select CAST((SUM(TivoliSize_MB))/(1024*1024) as varchar(15)) + ' GB'
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
				where ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Code = @code1 );
			end;

			-- If the archive flag is TWO, return by month/year
			if ( @include_archive = 2 )
			begin
				-- Get the total archived by year/month for this aggregation
				select T.ArchiveYear, T.ArchiveMonth, SUM(T.Size)/(1024*1024) AS DiskSize
				from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A, Tivoli_File_Log T
				where ( T.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Code = @code1 )
				group by T.ArchiveYear, T.ArchiveMonth
				order by T.ArchiveYear, T.ArchiveMonth;
			end;
		end
		else
		begin -- Just return the COMPLETE sums
			
			-- If the online flag is ONE, just return total size
			if ( @include_online = 1 )
			begin
				-- Get the total online 
				select CAST((SUM(DiskSize_MB))/(1024*1024) as varchar(15)) + ' GB'
				from SobekCM_Item I
			end;
				 
			-- If the online flag is TWO, return by month/year		 
			if ( @include_online = 2 )
			begin
				-- Get the total online by year/month
				select I.CreateYear, I.CreateMonth, SUM(I.DiskSize_MB) AS DiskSize
				from SobekCM_Item I
				where ( CreateYear > 0 )
				  and ( CreateMonth > 0 )
				group by I.CreateYear, I.CreateMonth
				order by I.CreateYear, I.CreateMonth;
			end;
				  
			-- Get the TOTAL archived amount for this system
			select CAST((SUM(TivoliSize_MB))/(1024*1024) as varchar(15)) + ' GB'
			from SobekCM_Item I
		end;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Recreate_All_Implied_Links]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- This recalculates all the implied links between items and item aggregations.
-- Implied links occur when an item is explicitly linked to a child aggregation.
-- Then, implied links can be put in the database that link the item with the
-- all of the aggregation hierarchies above the item aggregation it is explicitly
-- linked to....
-- Whenever an existing hierarchy is drastically changed, this should be executed.
CREATE PROCEDURE [dbo].[SobekCM_Recreate_All_Implied_Links]
AS
begin

	-- Delete all existing implied links first
	delete from SobekCM_Item_Aggregation_Item_Link
	where impliedLink = 'true';

	-- Add back the first implied links
	insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
	select distinct L.ItemID, H.ParentID, 'true'
	from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A
	where ( L.AggregationID = H.ChildID )
	  and ( L.AggregationID = A.AggregationID )
	  and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

	-- Add back the second level of implied links
	insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
	select distinct L.ItemID, H.ParentID, 'true'
	from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A
	where ( L.AggregationID = H.ChildID )
	  and ( L.ImpliedLink = 'true' )
	  and ( L.AggregationID = A.AggregationID )
	  and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

	-- Add back the third level of implied links
	insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
	select distinct L.ItemID, H.ParentID, 'true'
	from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A
	where ( L.AggregationID = H.ChildID )
	  and ( L.ImpliedLink = 'true' )
	  and ( L.AggregationID = A.AggregationID )
	  and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

	-- Add back the fourth level of implied links
	insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
	select distinct L.ItemID, H.ParentID, 'true'
	from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A
	where ( L.AggregationID = H.ChildID )
	  and ( L.ImpliedLink = 'true' )
	  and ( L.AggregationID = A.AggregationID )
	  and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );

	-- Add back the fifth level of implied links
	insert into SobekCM_Item_Aggregation_Item_Link (ItemID, AggregationID, impliedLink )
	select distinct L.ItemID, H.ParentID, 'true'
	from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation_Hierarchy H, SobekCM_Item_Aggregation A
	where ( L.AggregationID = H.ChildID )
	  and ( L.ImpliedLink = 'true' )
	  and ( L.AggregationID = A.AggregationID )
	  and not exists ( select * from SobekCM_Item_Aggregation_Item_Link T where T.ItemID = L.ItemID and T.AggregationID = H.ParentID );
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Footprint]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure links an item to a region
-- Written by Mark Sullivan ( August 2007 )
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Footprint]
	@ItemID int,
	@point_latitude float,
	@point_longitude float,
	@rect_latitude_A float,
	@rect_longitude_A float,
	@rect_latitude_B float,
	@rect_longitude_B float
AS
begin transaction

	if ( ( 	select count(*)
		from SobekCM_Item_Footprint
		where ( ItemID = @itemid ) and ( Point_Latitude = @point_latitude ) and ( Point_Longitude = @point_longitude )
		    and ( Rect_Latitude_A = @rect_latitude_a ) and ( Rect_Longitude_A = @rect_longitude_a ) 
		    and ( Rect_Latitude_B = @rect_latitude_b ) and ( Rect_Longitude_B = @rect_longitude_b )) = 0 )
	begin

		insert into SobekCM_Item_Footprint( ItemID, Point_Latitude, Point_Longitude, Rect_Latitude_A, Rect_Longitude_A, Rect_Latitude_B, Rect_Longitude_B )
		values ( @itemid, @point_latitude, @point_longitude, @rect_latitude_a, @rect_longitude_a, @rect_latitude_b, @rect_longitude_b )

	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Simple_Item_List]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Retrive the very simple list of items to save in XML format or to step through
-- and add to the solr/lucene index, etc..  
CREATE PROCEDURE [dbo].[SobekCM_Simple_Item_List]
	@collection_code varchar(10)
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	if ( len( isnull( @collection_code, '' )) = 0 )
	begin

		select G.BibID, I.VID, I.Title, I.CreateDate, Resource_Link = File_Location
		from SobekCM_Item_Group G, SobekCM_Item I
		where ( G.GroupID = I.GroupID )
		  and ( I.IP_Restriction_Mask = 0 )
		  and ( G.Deleted = CONVERT(bit,0) )
	      and ( I.Deleted = CONVERT(bit,0) );

	end
	else
	begin

		select G.BibID, I.VID, I.Title, I.CreateDate, Resource_Link = File_Location
		from SobekCM_Item_Group G, SobekCM_Item I, SobekCM_Item_Aggregation C, SobekCM_Item_Aggregation_Item_Link CL
		where ( G.GroupID = I.GroupID )
		  and ( I.IP_Restriction_Mask = 0 )
		  and ( G.Deleted = CONVERT(bit,0) )
	      and ( I.Deleted = CONVERT(bit,0) )
		  and ( I.ItemID = CL.ItemID )
		  and ( CL.AggregationID = C.AggregationID )
		  and ( Code = @collection_code );
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Set_IP_Restriction_Mask]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Set the IP restriction mask on a single item, by a single user, and
-- add a progress note that this was done
CREATE PROCEDURE [dbo].[SobekCM_Set_IP_Restriction_Mask]
	@itemid int,
	@newipmask int,
	@user varchar(50),
	@progressnote varchar(1000)
AS
begin transaction

	-- Update the item table
	update SobekCM_Item
	set IP_Restriction_Mask=@newipmask
	where ItemID=@itemid;
	
	-- Update the workhistory and possibly milestones
	if ( @newipmask < 0 )
	begin
		-- Add a worklog for this making the item PRIVATE
		insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote )
		values ( @itemid, 35, GETDATE(), @user, @progressnote );
	end
	else
	begin
		if ( @newipmask = 0 )
		begin
			-- Add a worklog for this making the item PUBLIC
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote )
			values ( @itemid, 34, GETDATE(), @user, @progressnote )		;
			
			-- Set the aggregations linked to this item's LastItemAdded date
			update SobekCM_Item_Aggregation
			set LastItemAdded = GETDATE()
			where exists ( select * from SobekCM_Item_Aggregation_Item_Link L where L.ItemID=@itemid and L.AggregationID = SobekCM_Item_Aggregation.AggregationID );	
		end
		else
		begin
			-- Add a worklog for this making the item RESTRICTED
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote )
			values ( @itemid, 36, GETDATE(), @user, @progressnote );
		end;
		
		-- Move along to the COMPLETED milestone
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, getdate()),
		    Milestone_ImageProcessing = ISNULL(Milestone_ImageProcessing, getdate()),
		    Milestone_QualityControl = ISNULL(Milestone_QualityControl, getdate()),
		    Milestone_OnlineComplete = ISNULL(Milestone_OnlineComplete, getdate()),
		    Last_MileStone=4
		where ItemID=@itemid;
	end;

commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Views]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves the behavior information about an item in this library
-- Written by Mark Sullivan 
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Views]
	@BibID varchar(10),
	@VID varchar(5),
	@Viewer1_TypeID int,
	@Viewer1_Label nvarchar(50),
	@Viewer1_Attribute nvarchar(250),
	@Viewer2_TypeID int,
	@Viewer2_Label nvarchar(50),
	@Viewer2_Attribute nvarchar(250),
	@Viewer3_TypeID int,
	@Viewer3_Label nvarchar(50),
	@Viewer3_Attribute nvarchar(250)
AS
begin transaction

	-- Get the volume id
	declare @itemid int

	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid)

	-- continue if a volumeid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
	
		-- Add the first viewer information
		if (( @Viewer1_TypeID > 0 ) and ( ( select COUNT(*) from SobekCM_Item_Viewers where ItemViewTypeID=@Viewer1_TypeID and ItemID=@itemid ) = 0 ))
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer1_TypeID, @Viewer1_Attribute, @Viewer1_Label )
		end
		
		-- Add the second viewer information
		if (( @Viewer2_TypeID > 0 ) and ( ( select COUNT(*) from SobekCM_Item_Viewers where ItemViewTypeID=@Viewer2_TypeID and ItemID=@itemid ) = 0 ))
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer2_TypeID, @Viewer2_Attribute, @Viewer2_Label )
		end
		
		-- Add the third viewer information
		if (( @Viewer3_TypeID > 0 ) and ( ( select COUNT(*) from SobekCM_Item_Viewers where ItemViewTypeID=@Viewer3_TypeID and ItemID=@itemid ) = 0 ))
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer3_TypeID, @Viewer3_Attribute, @Viewer3_Label )
		end	
	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Item_Aggregation_Link]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a link to the item aggregation (and all parents)
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Item_Aggregation_Link]
	@itemid int,
	@code varchar(20)
AS
begin

	-- Only continue if the code exists
	if ( len( isnull( @code,'')) > 0 )
	begin
		if (( select count(*) from SobekCM_Item_Aggregation where Code=@code and Deleted='false' ) = 1 )
		begin
			-- Get the ID for this aggregation code
			declare @AggregationID int;
			select @AggregationID = AggregationID from SobekCM_Item_Aggregation where Code = @code;

			-- Make sure the link does not already exist (two collection codes match)
			if (( select count(*) from SobekCM_Item_Aggregation_Item_Link where AggregationID = @AggregationID and ItemID = @ItemID ) = 0 )
			begin
				-- Tie this item to this primary collection
				insert into SobekCM_Item_Aggregation_Item_Link ( AggregationID, ItemID, impliedLink )
				values (  @AggregationID, @ItemID, 'false' );
			end
			else
			begin
				-- Make sure this does not say implied, since this was explicitly connected
				update SobekCM_Item_Aggregation_Item_Link
				set impliedLink = 'false'
				where ( AggregationID = @AggregationID ) and ( ItemID = @ItemID );
			end;
			
			-- Update the last create date time
			update SobekCM_Item_Aggregation
			set LastItemAdded = ( select CreateDate from SobekCM_Item where ItemID=@itemid )
			where AggregationID = @AggregationID
			  and LastItemAdded < ( select CreateDate from SobekCM_Item where ItemID=@itemid );

			-- Select parent codes
			select P.Code, P.AggregationID, Hierarchy=1
			into #TEMP_PARENTS
			from SobekCM_Item_Aggregation C, SobekCM_Item_Aggregation P, SobekCM_Item_Aggregation_Hierarchy H
			where ( C.AggregationID = H.ChildID )
			  and ( P.AggregationID = H.ParentID )
			  and ( C.Code = @code )
			  and ( H.Search_Parent_Only = 'false' );

			-- Select the grandparent codes
			insert into #TEMP_PARENTS ( Code, AggregationID, Hierarchy)
			select P.Code, P.AggregationID, 2 
			from #TEMP_PARENTS C, SobekCM_Item_Aggregation P, SobekCM_Item_Aggregation_Hierarchy H
			where ( C.AggregationID = H.ChildID )
			  and ( P.AggregationID = H.ParentID )
			  and ( H.Search_Parent_Only = 'false' );

			-- Select the grand-grandparent codes
			insert into #TEMP_PARENTS ( Code, AggregationID, Hierarchy)
			select P.Code, P.AggregationID, 3
			from #TEMP_PARENTS C, SobekCM_Item_Aggregation P, SobekCM_Item_Aggregation_Hierarchy H
			where ( C.AggregationID = H.ChildID )
			  and ( P.AggregationID = H.ParentID )
			  and ( C.Hierarchy = 2 )
			  and ( H.Search_Parent_Only = 'false' );

			-- Select the grand-grand-grandparent codes
			insert into #TEMP_PARENTS ( Code, AggregationID, Hierarchy)
			select P.Code, P.AggregationID, 4
			from #TEMP_PARENTS C, SobekCM_Item_Aggregation P, SobekCM_Item_Aggregation_Hierarchy H
			where ( C.AggregationID = H.ChildID )
			  and ( P.AggregationID = H.ParentID )
			  and ( C.Hierarchy = 3 )
			  and ( H.Search_Parent_Only = 'false' );

			-- Insert the link anywhere it does not exist
			insert into SobekCM_Item_Aggregation_Item_Link ( AggregationID, ItemID, impliedLink )
			select AggregationID, @itemid, 'true'
			from #TEMP_PARENTS P
			where not exists ( select * 
								from SobekCM_Item_Aggregation_Item_Link L
								where ( P.AggregationID = L.AggregationID )
								  and ( L.ItemID = @itemID ));
								  
			-- Also update the last create date
			update SobekCM_Item_Aggregation
			set LastItemAdded = ( select CreateDate from SobekCM_Item where ItemID=@itemid )
			where exists ( select * from #TEMP_PARENTS T where T.AggregationID=SobekCM_Item_Aggregation.AggregationID )
			  and LastItemAdded < ( select CreateDate from SobekCM_Item where ItemID=@itemid );

			-- drop the temporary table
			drop table #TEMP_PARENTS;
		end;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Metadata_Clear2]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Clears all the metadata for a single resource, while possibly leaving some 
-- values which are non-mets values, such as tracking boxes, user descriptions, etc..
CREATE PROCEDURE [dbo].[SobekCM_Metadata_Clear2]
	@itemid int,
	@clear_non_mets_values bit
AS
BEGIN
	-- Should ALL be cleared, or just non-mets stuff?
	if ( @clear_non_mets_values = 'true' )
	begin
		-- Just delete EVERY link between the metadata and this item
		delete from SobekCM_Metadata_Unique_Link 
		where ItemID = @itemid;
	end
	else
	begin
		-- Delete only specific types of metadata, not tickler or user descriptions
		delete from SobekCM_Metadata_Unique_Link 
		where ( ItemID = @itemid )
		  and not exists ( select * from SobekCM_Metadata_Unique_Search_Table S where S.MetadataID = SobekCM_Metadata_Unique_Link.MetadataID and ( MetadataTypeID = 20 or MetadataTypeID = 32 ));
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[FDA_Report_Save]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Sullivan, Mark
-- Create date: October 1, 2007
-- Description:	Save information about a single FDA Report 
-- ==========================================================
CREATE PROCEDURE [dbo].[FDA_Report_Save]
	@Package varchar(50),
	@IEID varchar(50),
	@FdaReportType varchar(50),
	@Report_Date datetime,
	@Account varchar(50),
	@Project varchar(50),
	@Warnings int,
	@Message varchar(1000),
	@BibID varchar(10),
	@VID varchar(5),
	@FdaReportID int output
AS
begin transaction

	-- Find the volume id
	declare @itemid int
	set @itemid = -1

	-- Add as a progress as well
	if ( ( select count(*) from SobekCM_Item I, SobekCM_Item_Group G where ( I.VID = @VID ) and ( I.GroupID = G.GroupID ) and ( G.BibID= @BibID)) > 0 )
	begin
		-- Select the volume id
		select @itemid = ItemID from SobekCM_Item I, SobekCM_Item_Group G where ( I.VID = @VID ) and ( I.GroupID = G.GroupID ) and ( G.BibID= @BibID)

		-- Mark as complete if report type is ''INGEST''
		if ( @FdaReportType = 'INGEST' )
		begin
			-- Add this new progress then
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, ProgressNote )
			values ( @itemID, 23, isnull(@Report_Date, getdate()), @IEID )
			
			-- Update the FDA remote archive flag
			update SobekCM_Item set Remotely_Archived = 'true' where ItemID=@itemid
		end

		-- Mark as complete if report type is ''INGEST''
		if ( @FdaReportType = 'ERROR' )
		begin
			-- Add this new progress then
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, ProgressNote )
			values ( @itemID, 22, isnull(@Report_Date, getdate()), @Message )
		end
	end
	
	-- Find the FDA Report Type ID, or insert it
	declare @reporttypeid int

	-- Does this type already exists
	if ( ( select count(*) from FDA_Report_Type where FdaReportType=@FdaReportType ) = 1 )
	begin
		-- Get the existing primary key
		select @reporttypeid = FdaReportTypeID
		from FDA_Report_Type
		where FdaReportType = @FdaReportType
	end
	else
	begin

		-- Insert a new row
		insert into FDA_Report_Type ( FdaReportType )
		values ( @FdaReportType )

		-- Get the new primary key
		set @reporttypeid = @@identity
	end

	-- Does this report already exist?
	if ( ( select count(*) from FDA_Report where (Package = @Package) and ( FdaReportTypeID = @reporttypeid ) and ( Report_Date = @Report_Date )) > 0 )
	begin

		-- Get the existing fda report id
		select @FdaReportID = FdaReportID 
		from FDA_Report 
		where (Package = @Package) 
		  and ( FdaReportTypeID = @reporttypeid ) 
		  and ( Report_Date = @Report_Date )

	end
	else
	begin

		-- Insert this report information (new report)
		insert into FDA_Report ( Package, IEID, FdaReportTypeID, Report_Date, Account, Project, Warnings, [Message], Database_Date, ItemID )
		values ( @Package, @IEID, @reporttypeid, @Report_Date, @Account, @Project, @Warnings, @Message, getDate(), @itemid )
	
		-- Return the new primary key
		set @FdaReportID = @@identity

	end
commit transaction
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Add_Item_To_User_Folder]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add an item to the user's folder
CREATE PROCEDURE [dbo].[mySobek_Add_Item_To_User_Folder]
	@userid int,
	@foldername varchar(255),
	@bibid varchar(10),
	@vid varchar(5),
	@itemorder int,
	@usernotes nvarchar(2000)
AS
begin

	-- Is there a match for this bib id /vid?
	if (( select COUNT(*) from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID = @bibid and I.VID = @vid ) = 1 )
	begin
		-- Get the item id
		declare @itemid int
		select @itemid = ItemID from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID = @bibid and I.VID = @vid
	
		-- First, see if this user already has a folder named this
		declare @userfolderid int
		if (( select count(*) from mySobek_User_Folder where UserID=@userid and FolderName=@foldername) > 0 )
		begin
			-- Get the existing folder id
			select @userfolderid = UserFolderID from mySobek_User_Folder where UserID=@userid and FolderName=@foldername
		end
		else
		begin
			-- Add this folder
			insert into mySobek_User_Folder ( UserID, FolderName, isPublic )
			values ( @userid, @foldername, 'false' )

			-- Get the new id
			select @userfolderid = @@identity
		end	

		-- Now, see if the item is already linked to the folder
		if (( select count(*) from mySobek_User_Item where ItemID=@itemid and UserFolderID=@userfolderid ) > 0 )
		begin
			-- Just update the existing link then
			update mySobek_User_Item
			set ItemOrder = @itemorder, UserNotes=@usernotes
			where UserFolderID = @userfolderid and ItemID=@itemid
		end
		else
		begin
			-- Add a new link then
			insert into mySobek_User_Item( UserFolderID, ItemID, ItemOrder, UserNotes, DateAdded )
			values ( @userfolderid, @itemid, @itemorder, @usernotes, getdate() )
		end
	end
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Add_Description_Tag]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Add a user tag
CREATE PROCEDURE [dbo].[mySobek_Add_Description_Tag] 
	@UserID int,
	@TagID int,
	@ItemID int,
	@Description nvarchar(2000),
	@new_TagID int output
AS
begin

	set @new_TagID = -1;

	if ( ISNULL(@TagID, -1 ) > 0 )
	begin
		update mySobek_User_Description_Tags
		set Description_Tag = @Description, Date_Modified = GETDATE()
		where TagID=@TagID and UserID=@UserID
		
		set @new_TagID = @TagID;	
	end
	else
	begin
		-- Can have up to five comments on a single item 
		if (( select COUNT(*) from mySobek_User_Description_Tags where UserID=@UserID and ItemID=@ItemID ) < 5)
		begin
			insert into mySobek_User_Description_Tags( UserID, ItemID, Description_Tag, Date_Modified )
			values ( @UserID, @ItemID, @Description, GETDATE() )	
			
			set @new_TagID = @@identity
		end
	end
end
GO
/****** Object:  StoredProcedure [dbo].[Admin_Update_Single_Cached_Aggregation_Metadata_Links]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Updates the links between an aggregation and metadata, as well as the number
-- of titles and items which are present within an aggregation
CREATE PROCEDURE [dbo].[Admin_Update_Single_Cached_Aggregation_Metadata_Links]
	@code varchar(20)
AS
BEGIN 

	-- No need to perform any locks here.  This should generally be run when no
	-- other changes are occurring, and even if some small changes are occurring, 
	-- they are likely to be lost in the sea of item metadata within the library
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Determine the aggregation ID for this collection
	declare @collid int;
	select @collid=( select ISNULL(AggregationID, -1) from SobekCM_Item_Aggregation where Code=@code );

	-- Delete old links
	delete from SobekCM_Item_Aggregation_Metadata_Link
	where AggregationID=@collid;
	
	-- Get the new list of metadata links to aggregations (through items-aggregations)
	select AggregationID, L.MetadataID, COUNT(*) AS Metadata_Count, MetadataTypeID 
	into #TEMP_LIST
	from SobekCM_Item_Aggregation_Item_Link A, SobekCM_Item I, SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table M
	where ( A.ItemID = I.ItemID )
	  and ( I.ItemID = L.ItemID )
	  and ( L.MetadataID = M.MetadataID )
	  and ( I.IP_Restriction_Mask >= 0 )
	  and ( A.AggregationID = @collid )
	group by AggregationID, L.MetadataID, MetadataTypeID;

	-- Insert these values into the cached links table
	insert into SobekCM_Item_Aggregation_Metadata_Link ( AggregationID, MetadataID, Metadata_Count, MetadataTypeID, OrderNum )
	select AggregationID, MetadataID, Metadata_Count, MetadataTypeID,
	ROW_NUMBER() OVER (PARTITION BY AggregationID ORDER BY Metadata_Count DESC )
	from #TEMP_LIST;
	
	-- Drop the temporary table
	drop table #TEMP_LIST;
		
	-- Now, update the number of items linked to each aggregation
	update SobekCM_Item_Aggregation
	set Current_Item_Count = ( select count(*) 
					   from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I
					   where L.ItemID = I.ItemID 
					     and L.AggregationID=@collid
					     and I.IP_Restriction_Mask >= 0 )
	where AggregationID=@collid;
	
	-- Now, update the number of titles linked to each aggregation
	update SobekCM_Item_Aggregation
	set Current_Title_Count = ( select count(distinct(GroupID)) 
					   from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I
					   where L.ItemID = I.ItemID 
					     and L.AggregationID=@collid
					     and I.IP_Restriction_Mask >= 0 )
	where AggregationID=@collid;

END;
GO
/****** Object:  StoredProcedure [dbo].[Admin_Update_Cached_Aggregation_Metadata_Links]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Admin_Update_Cached_Aggregation_Metadata_Links]
AS
BEGIN 

	-- No need to perform any locks here.  This should generally be run when no
	-- other changes are occurring, and even if some small changes are occurring, 
	-- they are likely to be lost in the sea of item metadata within the library
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Determine the aggregation ID for ALL
	declare @allid int;
	select @allid=( select ISNULL(AggregationID, -1) from SobekCM_Item_Aggregation where Code='all' );

	-- Delete old links
	delete from SobekCM_Item_Aggregation_Metadata_Link;
	
	-- Get the new list of metadata links to aggregations (through items-aggregations)
	select AggregationID, L.MetadataID, COUNT(*) AS Metadata_Count, MetadataTypeID 
	into #TEMP_LIST
	from SobekCM_Item_Aggregation_Item_Link A, SobekCM_Item I, SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table M
	where ( A.ItemID = I.ItemID )
	  and ( I.ItemID = L.ItemID )
	  and ( L.MetadataID = M.MetadataID )
	  and ( I.IP_Restriction_Mask >= 0 )
	  and ( A.AggregationID != @allid )
	group by AggregationID, L.MetadataID, MetadataTypeID;

	-- Insert these values into the cached links table
	insert into SobekCM_Item_Aggregation_Metadata_Link ( AggregationID, MetadataID, Metadata_Count, MetadataTypeID, OrderNum )
	select AggregationID, MetadataID, Metadata_Count, MetadataTypeID,
	ROW_NUMBER() OVER (PARTITION BY AggregationID ORDER BY Metadata_Count DESC )
	from #TEMP_LIST;
	
	-- Drop the temporary table
	drop table #TEMP_LIST;
	
	-- Now get the list of ALL public items metadata for insertion
	select L.MetadataID, COUNT(*) AS Metadata_Count, MetadataTypeID 
	into #TEMP_LIST2
	from SobekCM_Item I, SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table M
	where ( I.ItemID = L.ItemID )
	  and ( L.MetadataID = M.MetadataID )
	  and ( I.IP_Restriction_Mask >= 0 )
	group by L.MetadataID, MetadataTypeID;
	
	-- Insert these values into the cached links table
	insert into SobekCM_Item_Aggregation_Metadata_Link ( AggregationID, MetadataID, Metadata_Count, MetadataTypeID, OrderNum )
	select @allid, MetadataID, Metadata_Count, MetadataTypeID,
	ROW_NUMBER() OVER (ORDER BY Metadata_Count DESC )
	from #TEMP_LIST2;
	
	-- Drop the temporary table
	drop table #TEMP_LIST2;
	
	-- Clear the item/title count
	UPDATE SobekCM_Item_Aggregation SET Current_Item_Count=0, Current_Title_Count=0;
	
	-- Now, update the number of items linked to each aggregation
	WITH Aggregation_Item_CTE AS 
		(	select AggregationID, COUNT(*) as Item_Count	
			from SobekCM_Item_Aggregation_Item_Link as L inner join
				 SobekCM_Item as I on L.ItemID = I.ItemID
			where ( I.IP_Restriction_Mask >= 0 )
			group by AggregationID )
	UPDATE SobekCM_Item_Aggregation
	SET Current_Item_Count=(select Item_Count from Aggregation_Item_CTE C where C.AggregationID=SobekCM_Item_Aggregation.AggregationID )
	where AggregationID in ( select AggregationID from Aggregation_Item_CTE );
	
	-- Now, update the number of titles linked to each aggregation
	WITH Aggregation_Title_CTE AS 
		(	select AggregationID, COUNT(distinct(GroupID)) as Title_Count	
			from SobekCM_Item_Aggregation_Item_Link as L inner join
				 SobekCM_Item as I on L.ItemID = I.ItemID
			where ( I.IP_Restriction_Mask >= 0 )
			group by AggregationID )
	UPDATE SobekCM_Item_Aggregation
	SET Current_Title_Count=(select Title_Count from Aggregation_Title_CTE C where C.AggregationID=SobekCM_Item_Aggregation.AggregationID )
	where AggregationID in ( select AggregationID from Aggregation_Title_CTE );

END;
GO
/****** Object:  StoredProcedure [dbo].[Admin_Update_All_AggregationCodes_Values]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Admin_Update_All_AggregationCodes_Values]
AS
begin
	declare @itemid int

	declare itemcursor cursor read_only
	for select ItemID from SobekCM_Item

	open itemcursor

	fetch next from itemcursor into @itemid

	while @@fetch_status = 0
	begin
		-- Prepare to step through each metadata value and build the full citation
		declare @singlecode nvarchar(max)

		declare @allCodes nvarchar(max)
		set @allCodes=''
			
		declare codecursor cursor read_only
		for select Code 
			from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L
			where L.ItemID=@itemid and L.AggregationID=A.AggregationID and L.ImpliedLink='false' and A.Type not like 'INSTITU%'

		open codecursor

		fetch next from codecursor into @singlecode

		while @@fetch_status = 0
		begin

			set @allCodes = @allCodes + ' ' + @singlecode
			fetch next from codecursor into @singlecode

		end

		close codecursor
		deallocate codecursor

		-- Insert the newly created full citation for this item
		update SobekCM_Item 
		set AggregationCodes = @allCodes
		where ItemID=@itemid
		
	fetch next from itemcursor into @itemid
	end

	close itemcursor
	deallocate itemcursor
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_Public_Folder_Browse]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get a browse of all items in a public folder
CREATE PROCEDURE [dbo].[mySobek_Get_Public_Folder_Browse]
	@folderid int,
	@pagesize int, 
	@pagenumber int,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output
AS
BEGIN

	-- Declare the temporary tables
	create table #TEMP_ITEMS ( ItemID int, fk_TitleID int, ItemOrder int, SortDate bigint, UserNotes nvarchar(2000))
	create table #TEMP_PAGED_ITEMS ( ItemID int, RowNumber int, UserNotes nvarchar(2000) );

	-- Make sure this is a public folder
	set @folderid = ( select ISNULL(UserFolderID, -1) from mySobek_User_Folder F where F.UserFolderID=@folderid and F.isPublic = 'true' )
	-- Determine the start and end rows
	declare @rowstart int 
	declare @rowend int 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Get the list of items in the folder
	insert into #TEMP_ITEMS ( ItemID, fk_TitleID, ItemOrder, SortDate, UserNotes )
	select I.ItemID, I.GroupID, A.ItemOrder, isnull( I.SortDate,-1), ISNULL(A.UserNotes,'' )
	from mySobek_User_Item A, SobekCM_Item I
	where ( I.ItemID = A.ItemID )
	  and ( A.UserFolderID = @folderid );
	
	-- Items in a users folder always appear individually, rather than being aggregated into
	-- titles.  This is to allow individual actions to be done against them and for each 
	-- individual user notes to appear correctly
		
	-- Create saved select across items for row numbers
	with ITEMS_SELECT AS
	 (	select I.ItemID, UserNotes,
			ROW_NUMBER() OVER (order by ItemOrder ASC) as RowNumber
			from #TEMP_ITEMS I
			group by I.ItemID, ItemOrder, UserNotes )
				  
	-- Insert the correct rows into the temp item table	
	insert into #TEMP_PAGED_ITEMS ( ItemID, UserNotes, RowNumber )
	select ItemID, UserNotes, RowNumber
	from ITEMS_SELECT
	where RowNumber >= @rowstart
	  and RowNumber <= @rowend
	  
	-- Return the title information for this page
	select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type]
	from #TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
	where ( T.ItemID = I.ItemID )
	  and ( I.GroupID = G.GroupID )
	order by RowNumber ASC
	
	-- Return the item information for this page
	select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, T.UserNotes
	from SobekCM_Item I, #TEMP_PAGED_ITEMS T
	where ( T.ItemID = I.ItemID )
	order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index
	  				
	-- drop the temporary paged table
	drop table #TEMP_PAGED_ITEMS	

	-- Return the total counts ( since items and titles always equal, return same number for both)
	select @total_items=COUNT(*), @total_titles=COUNT(*)
	from #TEMP_ITEMS 
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin	
		-- Build the aggregation list
		select distinct(A.Code), A.ShortName, Metadata_Count=Count(*)
		from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
		where ( T.ItemID = I.ItemID )
		  and ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = A.AggregationID )
		group by A.Code, A.ShortName
		order by Metadata_Count DESC, ShortName ASC	
		
		-- Return the FIRST facet
		if ( @facettype1 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype1 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the SECOND facet
		if ( @facettype2 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype2 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the THIRD facet
		if ( @facettype3 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype3 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end		
		
		-- Return the FOURTH facet
		if ( @facettype4 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype4 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the FIFTH facet
		if ( @facettype5 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype5 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the SIXTH facet
		if ( @facettype6 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype6 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the SEVENTH facet
		if ( @facettype7 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype7 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the EIGHTH facet
		if ( @facettype8 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype8 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
	end
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS
	
END
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_Folder_Search_Information]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get overall information about folders and searches for this user
CREATE PROCEDURE [dbo].[mySobek_Get_Folder_Search_Information]
	@userid int
AS
BEGIN
	-- Return the names of all the folders first
	select F.UserFolderID, ParentFolderID=isnull(F.ParentFolderID,-1), F.FolderName, F.isPublic, Item_Count=(select count(*) from mySobek_User_Item I where I.UserFolderID=F.UserFolderID )
	from mySobek_User_Folder F
	where UserID=@userid

	-- Return the number of searches
	select Search_Count=count(*)
	from mySobek_User_Search
	where UserID=@userid
END
GO
/****** Object:  StoredProcedure [dbo].[Builder_Get_Minimum_Item_Information]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Builder_Get_Minimum_Item_Information]
	@bibid varchar(10),
	@vid varchar(5)
AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the item id and mainthumbnail
	select I.ItemID, I.MainThumbnail, I.IP_Restriction_Mask, I.Born_Digital, G.ItemCount, I.Dark
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.VID = @vid )
	  and ( G.BibID = @bibid )
	  and ( I.GroupID = G.GroupID );

	-- Get the links to the aggregations
	select A.Code, A.Name
	from SobekCM_Item I, SobekCM_Item_Group G, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
	where ( I.VID = @vid )
	  and ( G.BibID = @bibid )
	  and ( I.GroupID = G.GroupID )
	  and ( I.ItemID = L.ItemID )
	  and ( L.AggregationID = A.AggregationID );

end;
GO
/****** Object:  StoredProcedure [dbo].[Builder_Get_Item_Aggregation_Thumbnail]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Builder_Get_Item_Aggregation_Thumbnail]
	@bibid varchar(10),
	@vid varchar(5)
AS
begin

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get the item id and mainthumbnail
	select I.ItemID, I.MainThumbnail, I.IP_Restriction_Mask, I.Born_Digital
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.VID = @vid )
	  and ( G.BibID = @bibid )
	  and ( I.GroupID = G.GroupID );

	-- Get the links to the aggregations
	select A.Code
	from SobekCM_Item I, SobekCM_Item_Group G, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
	where ( I.VID = @vid )
	  and ( G.BibID = @bibid )
	  and ( I.GroupID = G.GroupID )
	  and ( I.ItemID = L.ItemID )
	  and ( L.AggregationID = A.AggregationID );

end;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Delete_Item_From_User_Folder]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete an item from the user's folder
CREATE PROCEDURE [dbo].[mySobek_Delete_Item_From_User_Folder]
	@userid int,
	@foldername varchar(255),
	@bibid varchar(10),
	@vid varchar(5)
AS
begin
	
	-- Is there a match for this bib id /vid?
	if (( select COUNT(*) from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID = @bibid and I.VID = @vid ) = 1 )
	begin
		-- Get the item id
		declare @itemid int
		select @itemid = ItemID from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID = @bibid and I.VID = @vid
	
		-- First, get the user folder id for this
		if (( select count(*) from mySobek_User_Folder where UserID=@userid and FolderName=@foldername) > 0 )
		begin
			-- Get the existing folder id
			declare @userfolderid int
			select @userfolderid = UserFolderID from mySobek_User_Folder where UserID=@userid and FolderName=@foldername

			-- Now, delete this item
			delete from mySobek_User_Item where UserFolderID=@userfolderid and ItemID=@itemid
		end
	end
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Delete_Item_From_All_User_Folders]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete an item from the user's folder
CREATE PROCEDURE [dbo].[mySobek_Delete_Item_From_All_User_Folders]
	@userid int,
	@bibid varchar(10),
	@vid varchar(5)
AS
begin
	
		-- Is there a match for this bib id /vid?
	if (( select COUNT(*) from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID = @bibid and I.VID = @vid ) = 1 )
	begin
		-- Get the item id
		declare @itemid int
		select @itemid = ItemID from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID = @bibid and I.VID = @vid
	
		-- Delete this item from this users folder
		delete from mySobek_User_Item
		where ( ItemID=@itemid ) 
		  and exists (	select FolderName 
						from mySobek_User_Folder F 
						where F.UserID=@userid 
						  and F.UserFolderID=mySobek_User_Item.UserFolderID
						  and FolderName != 'Submitted Items' )
	end
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Delete_Description_Tag]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete a user's tag
CREATE PROCEDURE [dbo].[mySobek_Delete_Description_Tag] 
	@TagID int
AS
begin
	delete from mySobek_User_Description_Tags where TagID=@TagID
end
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_By_UserID]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[mySobek_Get_User_By_UserID]
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
	  Receive_Stats_Emails, Has_Item_Stats		  
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
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsCurator AS IsAggregationAdmin, L.OnHomePage, L.IsCurator AS IsCollectionManager, GroupDefined='false'
	from SobekCM_Item_Aggregation A, mySobek_User_Edit_Aggregation L
	where  ( L.AggregationID = A.AggregationID ) and ( L.UserID = @userid )
	union
	select A.Code, A.[Name], L.CanSelect, L.CanEditItems, L.IsCurator AS IsAggregationAdmin, OnHomePage = 'false', L.IsCurator AS IsCollectionManager, GroupDefined='true'
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
/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_Folder_Browse]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get a browse of all items in a user's folder
CREATE PROCEDURE [dbo].[mySobek_Get_User_Folder_Browse]
	@userid int,
	@foldername varchar(255),
	@pagesize int, 
	@pagenumber int,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output
AS
BEGIN

	-- Declare the temporary tables
	create table #TEMP_ITEMS ( ItemID int, fk_TitleID int, ItemOrder int, SortDate bigint, UserNotes nvarchar(2000))
	create table #TEMP_PAGED_ITEMS ( ItemID int, RowNumber int, UserNotes nvarchar(2000) );
			
	-- Get the folder id
	declare @folderid int
	set @folderid = ( select ISNULL(UserFolderID,-1) from mySobek_User_Folder where UserID=@userid and FolderName=@foldername );
	
	-- Determine the start and end rows
	declare @rowstart int 
	declare @rowend int 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Get the list of items in the folder
	insert into #TEMP_ITEMS ( ItemID, fk_TitleID, ItemOrder, SortDate, UserNotes )
	select I.ItemID, I.GroupID, A.ItemOrder, isnull( I.SortDate,-1), ISNULL(A.UserNotes,'' )
	from mySobek_User_Item A, SobekCM_Item I
	where ( I.ItemID = A.ItemID )
	  and ( A.UserFolderID = @folderid );
	
	-- Items in a users folder always appear individually, rather than being aggregated into
	-- titles.  This is to allow individual actions to be done against them and for each 
	-- individual user notes to appear correctly
		
	-- Create saved select across items for row numbers
	with ITEMS_SELECT AS
	 (	select I.ItemID, UserNotes,
			ROW_NUMBER() OVER (order by ItemOrder ASC) as RowNumber
			from #TEMP_ITEMS I
			group by I.ItemID, ItemOrder, UserNotes )
				  
	-- Insert the correct rows into the temp item table	
	insert into #TEMP_PAGED_ITEMS ( ItemID, UserNotes, RowNumber )
	select ItemID, UserNotes, RowNumber
	from ITEMS_SELECT
	where RowNumber >= @rowstart
	  and RowNumber <= @rowend
	  
	-- Return the title information for this page
	select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type]
	from #TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
	where ( T.ItemID = I.ItemID )
	  and ( I.GroupID = G.GroupID )
	order by RowNumber ASC
	
	-- Return the item information for this page
	select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, T.UserNotes
	from SobekCM_Item I, #TEMP_PAGED_ITEMS T
	where ( T.ItemID = I.ItemID )
	order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index
	  				
	-- drop the temporary paged table
	drop table #TEMP_PAGED_ITEMS	

	-- Return the total counts ( since items and titles always equal, return same number for both)
	select @total_items=COUNT(*), @total_titles=COUNT(*)
	from #TEMP_ITEMS 
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin	
		-- Build the aggregation list
		select distinct(A.Code), A.ShortName, Metadata_Count=Count(*)
		from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
		where ( T.ItemID = I.ItemID )
		  and ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = A.AggregationID )
		group by A.Code, A.ShortName
		order by Metadata_Count DESC, ShortName ASC	
		
		-- Return the FIRST facet
		if ( @facettype1 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype1 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the SECOND facet
		if ( @facettype2 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype2 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the THIRD facet
		if ( @facettype3 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype3 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end		
		
		-- Return the FOURTH facet
		if ( @facettype4 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype4 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the FIFTH facet
		if ( @facettype5 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype5 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the SIXTH facet
		if ( @facettype6 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype6 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the SEVENTH facet
		if ( @facettype7 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype7 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
		
		-- Return the EIGHTH facet
		if ( @facettype8 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from  ( select top(100) T.*
					from (	select distinct(L.MetadataID), Metadata_Count = COUNT(*)
							from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS I
							where ( L.ItemID = I.ItemID )
							  and ( L.MetadataID = U.MetadataID )
							  and ( U.MetadataTypeID = @facettype8 )
							group by L.MetadataID ) T
					 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC
		end
	end
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS
	
END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_QC_Comments]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure returns the item id as a single row given the bibid and vid.
-- This also doubles as a quick way to check if a certain item exists in
-- the database and is employed by some of the workflow tools
CREATE PROCEDURE [dbo].[SobekCM_Get_QC_Comments]
	@itemID int,
	@userName varchar(50)
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return the item id as a single row relation
	select ProgressNote
	from Tracking_Progress T 
	where T.ItemID=@itemID and T.WorkPerformedBy=@userName

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_OAI_Sets]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get the OAI set information from the database
-- This stored procedure is called from the UFDC Web
-- Written by Mark Sullivan (March, 2007)
CREATE PROCEDURE [dbo].[SobekCM_Get_OAI_Sets] AS
begin transaction

	-- Get the basic collection information that supports OAI
	select C.AggregationID, C.Code, C.[Name], C.Description, OAI_Metadata=isnull(C.OAI_Metadata, '')
	into #TEMP1
	from SobekCM_Item_Aggregation C
	where ( C.isActive = 1 )
	  and ( C.OAI_Flag = 1 )
	  and ( C.Deleted = 0 )
	order by C.Code;

	select T.Code, T.[Name], T.Description, LastItemAddedDate=MAX(I.CreateDate), T.OAI_Metadata
	from #TEMP1 T, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I
	where ( T.AggregationID = L.AggregationID )
      and ( L.ItemID = I.ItemID )
	group by Code, [Name], Description, OAI_Metadata;

	-- drop the temporary tables
	drop table #TEMP1;

commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Item_Statistics]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Pull any additional item details before showing this item
CREATE PROCEDURE [dbo].[SobekCM_Get_Item_Statistics]
	@BibID varchar(10),
	@VID varchar(5)
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Return bib level first
	select G.BibID, S.[Year], S.[Month], S.[Hits], S.[Sessions]
	from SobekCM_Item_Group_Statistics S, SobekCM_Item_Group G
	where ( G.BibID = @BibID )
	  and ( G.GroupID = S.GroupID )
	order by [Year], [Month];
	  
	-- Return the volume level next
	if ( LEN( ISNULL(@VID,'')) = 0 )
	begin
	
		-- Return all the volume info
		select I.VID, S.*
		from SobekCM_Item I, SobekCM_Item_Group G, SobekCM_Item_Statistics S
		where ( G.BibID = @BibID ) 
		  and ( G.GroupID = I.GroupID )
		  and ( I.ItemID = S.ItemID )	
		order by [Year], [Month];
	end
	else
	begin
	
		-- Return the single the volume info
		select I.VID, S.*
		from SobekCM_Item I, SobekCM_Item_Group G, SobekCM_Item_Statistics S
		where ( G.BibID = @BibID ) 
		  and ( G.GroupID = I.GroupID )
		  and ( I.ItemID = S.ItemID )	
		  and ( I.VID = @VID )
		order by [Year], [Month];
	end;			  
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Item_Details2]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Pull any additional item details before showing this item
CREATE PROCEDURE [dbo].[SobekCM_Get_Item_Details2]
	@BibID varchar(10),
	@VID varchar(5)
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Was this for one item within a group?
	if ( LEN( ISNULL(@VID,'')) > 0 )
	begin	
	
		-- Only continue if there is ONE match
		if (( select COUNT(*) from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID = @BibID and I.VID = @VID ) = 1 )
		begin
			-- Get the itemid
			declare @ItemID int;
			select @ItemID = ItemID from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and G.BibID = @BibID and I.VID = @VID;

			-- Return any descriptive tags
			select U.FirstName, U.NickName, U.LastName, G.BibID, I.VID, T.Description_Tag, T.TagID, T.Date_Modified, U.UserID, isnull([PageCount], 0) as Pages, ExposeFullTextForHarvesting
			from mySobek_User U, mySobek_User_Description_Tags T, SobekCM_Item I, SobekCM_Item_Group G
			where ( T.ItemID = @ItemID )
			  and ( I.ItemID = T.ItemID )
			  and ( I.GroupID = G.GroupID )
			  and ( T.UserID = U.UserID );
			
			-- Return the aggregation information linked to this item
			select A.Code, A.Name, A.ShortName, A.[Type], A.Map_Search, A.DisplayOptions, A.Items_Can_Be_Described, L.impliedLink, A.Hidden, A.isActive, ISNULL(A.External_Link,'') as External_Link
			from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
			where ( L.ItemID = @ItemID )
			  and ( A.AggregationID = L.AggregationID );
		  
			-- Return information about the actual item/group
			select G.BibID, I.VID, G.File_Location, G.SuppressEndeca, 'true' as [Public], I.IP_Restriction_Mask, G.GroupID, I.ItemID, I.CheckoutRequired, Total_Volumes=(select COUNT(*) from SobekCM_Item J where G.GroupID = J.GroupID ),
				isnull(I.Level1_Text, '') as Level1_Text, isnull( I.Level1_Index, 0 ) as Level1_Index, 
				isnull(I.Level2_Text, '') as Level2_Text, isnull( I.Level2_Index, 0 ) as Level2_Index, 
				isnull(I.Level3_Text, '') as Level3_Text, isnull( I.Level3_Index, 0 ) as Level3_Index,
				G.GroupTitle, I.TextSearchable, Comments=isnull(I.Internal_Comments,''), Dark, G.[Type],
				I.Title, I.Publisher, I.Author, I.Donor, I.PubDate, G.ALEPH_Number, G.OCLC_Number, I.Born_Digital, 
				I.Disposition_Advice, I.Material_Received_Date, I.Material_Recd_Date_Estimated, I.Tracking_Box, I.Disposition_Advice_Notes, 
				I.Left_To_Right, I.Disposition_Notes, G.Track_By_Month, G.Large_Format, G.Never_Overlay_Record, I.CreateDate, I.SortDate, 
				G.Primary_Identifier_Type, G.Primary_Identifier, G.[Type] as GroupType, coalesce(I.MainThumbnail,'') as MainThumbnail
			from SobekCM_Item I, SobekCM_Item_Group G
			where ( I.ItemID = @ItemID )
			  and ( G.GroupID = I.GroupID );
		  
			-- Return any ticklers associated with this item
			select MetadataValue
			from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L
			where ( L.ItemID = @ItemID ) 
			  and ( L.MetadataID = M.MetadataID )
			  and ( M.MetadataTypeID = 20 );
			
			-- Return the viewers for this item
			select T.ViewType, V.Attribute, V.Label
			from SobekCM_Item_Viewers V, SobekCM_Item_Viewer_Types T
			where ( V.ItemID = @ItemID )
			  and ( V.ItemViewTypeID = T.ItemViewTypeID );
				
			-- Return the icons for this item
			select Icon_URL, Link, Icon_Name, I.Title
			from SobekCM_Icon I, SobekCM_Item_Icons L
			where ( L.IconID = I.IconID ) 
			  and ( L.ItemID = @ItemID )
			order by Sequence;
			  
			-- Return any web skin restrictions
			select S.WebSkinCode
			from SobekCM_Item_Group_Web_Skin_Link L, SobekCM_Item I, SobekCM_Web_Skin S
			where ( L.GroupID = I.GroupID )
			  and ( L.WebSkinID = S.WebSkinID )
			  and ( I.ItemID = @ItemID )
			order by L.Sequence;
		end;		
	end
	else
	begin
		-- Return the aggregation information linked to this item
		select GroupTitle, BibID, G.[Type], G.File_Location, isnull(AGGS.Code,'') AS Code, G.GroupID, isnull(GroupThumbnail,'') as Thumbnail, G.Track_By_Month, G.Large_Format, G.Never_Overlay_Record, G.Primary_Identifier_Type, G.Primary_Identifier
		from SobekCM_Item_Group AS G LEFT JOIN
			 ( select distinct(A.Code),  G2.GroupID
			   from SobekCM_Item_Group G2, SobekCM_Item IL, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
		       where IL.ItemID=L.ItemID 
		         and A.AggregationID=L.AggregationID
		         and G2.GroupID=IL.GroupID
		         and G2.BibID=@BibID
		         and G2.Deleted='false'
		       group by A.Code, G2.GroupID ) AS AGGS ON G.GroupID=AGGS.GroupID
		where ( G.BibID = @BibID )
		  and ( G.Deleted = 'false' );

		-- Return the individual volumes
		select I.ItemID, Title, Level1_Text=isnull(Level1_Text,''), Level1_Index=isnull(Level1_Index,-1), Level2_Text=isnull(Level2_Text, ''), Level2_Index=isnull(Level2_Index, -1), Level3_Text=isnull(Level3_Text, ''), Level3_Index=isnull(Level3_Index, -1), Level4_Text=isnull(Level4_Text, ''), Level4_Index=isnull(Level4_Index, -1), Level5_Text=isnull(Level5_Text, ''), Level5_Index=isnull(Level5_Index,-1), I.MainThumbnail, I.VID, I.IP_Restriction_Mask, I.SortTitle, I.Dark
		from SobekCM_Item I, SobekCM_Item_Group G
		where ( G.GroupID = I.GroupID )
		  and ( G.BibID = @bibid )
		  and ( I.Deleted = 'false' )
		  and ( G.Deleted = 'false' )
		order by Level1_Index ASC, Level1_Text ASC, Level2_Index ASC, Level2_Text ASC, Level3_Index ASC, Level3_Text ASC, Level4_Index ASC, Level4_Text ASC, Level5_Index ASC, Level5_Text ASC, Title ASC;

		-- Get list of icon ids
		select distinct(IconID)
		into #TEMP_ICON
		from SobekCM_Item_Icons II, SobekCM_Item It, SobekCM_Item_Group G
		where ( It.GroupID = G.GroupID )
			and ( G.BibID = @bibid )
			and ( It.Deleted = 0 )
			and ( II.ItemID = It.ItemID )
		group by IconID;

		-- Return icons
		select Icon_URL, Link, Icon_Name, Title
		from SobekCM_Icon I, (	select distinct(IconID)
								from SobekCM_Item_Icons II, SobekCM_Item It, SobekCM_Item_Group G
								where ( It.GroupID = G.GroupID )
							 	  and ( G.BibID = @bibid )
								  and ( It.Deleted = 0 )
								  and ( II.ItemID = It.ItemID )
								group by IconID) AS T
		where ( T.IconID = I.IconID );
		
		-- Return any web skin restrictions
		select S.WebSkinCode
		from SobekCM_Item_Group_Web_Skin_Link L, SobekCM_Item_Group G, SobekCM_Web_Skin S
		where ( L.GroupID = G.GroupID )
		  and ( L.WebSkinID = S.WebSkinID )
		  and ( G.BibID = @BibID )
		order by L.Sequence;
		
		-- Get the distinct list of all aggregations linked to this item
		select distinct( Code )
		from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Group G, SobekCM_Item I
		where ( I.ItemID = L.ItemID )
		  and ( I.GroupID = G.GroupID )
		  and ( G.BibID = @BibID )
		  and ( L.AggregationID = A.AggregationID );		
	end;
		
	-- Get the list of related item groups
	select B.BibID, B.GroupTitle, R.Relationship_A_to_B AS Relationship
	from SobekCM_Item_Group A, SobekCM_Item_Group_Relationship R, SobekCM_Item_Group B
	where ( A.BibID = @bibid ) 
	  and ( R.GroupA = A.GroupID )
	  and ( R.GroupB = B.GroupID )
	union
	select A.BibID, A.GroupTitle, R.Relationship_B_to_A AS Relationship
	from SobekCM_Item_Group A, SobekCM_Item_Group_Relationship R, SobekCM_Item_Group B
	where ( B.BibID = @bibid ) 
	  and ( R.GroupB = B.GroupID )
	  and ( R.GroupA = A.GroupID );
		  
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Item_Brief_Info]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get some basic information about an item, which is pulled from the database before the item
-- is displayed online.  Many of these values correspond to the item group/title or how this
-- item relates to the item group and any item aggregations within the system.
CREATE PROCEDURE [dbo].[SobekCM_Get_Item_Brief_Info]
	@bibid varchar(10),
	@vid varchar(5),
	@include_aggregations bit
as
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return the item group / item information exactly like it is returned in the Item list brief procedure
	select G.BibID, I.VID, G.GroupTitle, 
			isnull(I.Level1_Text, '') as Level1_Text, isnull( I.Level1_Index, 0 ) as Level1_Index, 
			isnull(I.Level2_Text, '') as Level2_Text, isnull( I.Level2_Index, 0 ) as Level2_Index, 
			isnull(I.Level3_Text, '') as Level3_Text, isnull( I.Level3_Index, 0 ) as Level3_Index, 
			PubDate=isnull(I.PubDate,''), SortDate=isnull( I.SortDate,-1), MainThumbnail=G.File_Location + '/' + VID + '/' + isnull( I.MainThumbnail,''), 
			I.Title, Author=isnull(I.Author,''), IP_Restriction_Mask, G.OCLC_Number, G.ALEPH_Number, MainThumbnailFile=ISNULL(I.MainThumbnail,''), MainJPEGFile=ISNULL(I.MainJPEG,'')
	from SobekCM_Item I, SobekCM_Item_Group G
	where ( I.GroupID = G.GroupID )
	  and ( G.BibID = @bibid )
	  and ( I.VID = @vid );  
	  
	-- Check to see if aggregation information should be returned
	if( @include_aggregations = 'true' )
	begin
  		-- Return the aggregation information linked to this item
		select A.Code, A.Name, A.ShortName, A.[Type], A.Map_Search, A.DisplayOptions, A.Items_Can_Be_Described, L.impliedLink, A.Hidden, A.isActive, ISNULL(A.External_Link,'') as External_Link
		from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A, SobekCM_Item I, SobekCM_Item_Group G
		where ( L.ItemID = I.ItemID )
		  and ( A.AggregationID = L.AggregationID )
		  and ( I.GroupID = G.GroupID )
	      and ( G.BibID = @bibid )
	      and ( I.VID = @vid );
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Item_Aggregation2]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets all of the information about a single item aggregation
CREATE PROCEDURE [dbo].[SobekCM_Get_Item_Aggregation2]
	@code varchar(20),
	@include_counts bit,
	@is_robot bit
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Create the temporary table
	create table #TEMP_CHILDREN_BUILDER (AggregationID int, Code varchar(20), ParentCode varchar(20), Name varchar(255), [Type] varchar(50), ShortName varchar(100), isActive bit, Hidden bit, HierarchyLevel int );
	
	-- Get the aggregation id
	declare @aggregationid int
	set @aggregationid = isnull((select AggregationID from SobekCM_Item_Aggregation AS C where C.Code = @code and Deleted=0), -1 );
	
	-- Return information about this aggregation
	select AggregationID, Code, [Name], isnull(ShortName,[Name]) AS ShortName, [Type], isActive, Hidden, HasNewItems,
	   ContactEmail, DefaultInterface, [Description], Map_Display, Map_Search, OAI_Flag, OAI_Metadata, DisplayOptions, LastItemAdded, 
	   Can_Browse_Items, Items_Can_Be_Described, External_Link, ThematicHeadingID
	from SobekCM_Item_Aggregation AS C 
	where C.AggregationID = @aggregationid;

	-- Drive down through the children in the item aggregation hierarchy (first level below)
	insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, HierarchyLevel )
	select C.AggregationID, C.Code, ParentCode=@code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, -1
	from SobekCM_Item_Aggregation AS P INNER JOIN
		 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
		 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
	where ( P.AggregationID = @aggregationid );
	
	-- If this is a robot, no need to go further
	if ( @is_robot = 'false' )
	begin

		-- Now, try to find any children to this ( second level below )
		insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, HierarchyLevel )
		select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, -2
		from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
			 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
			 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
		where ( HierarchyLevel = -1 );

		-- Now, try to find any children to this ( third level below )
		insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, HierarchyLevel )
		select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, -3
		from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
			 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
			 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
		where ( HierarchyLevel = -2 ); 

		-- Now, try to find any children to this ( fourth level below )
		insert into #TEMP_CHILDREN_BUILDER ( AggregationID, Code, ParentCode, Name, [Type], ShortName, isActive, Hidden, HierarchyLevel )
		select C.AggregationID, C.Code, P.Code, C.[Name], C.[Type], isnull(C.ShortName,C.[Name]) AS ShortName, C.isActive, C.Hidden, -4
		from #TEMP_CHILDREN_BUILDER AS P INNER JOIN
			 SobekCM_Item_Aggregation_Hierarchy AS H ON H.ParentID = P.AggregationID INNER JOIN
			 SobekCM_Item_Aggregation AS C ON H.ChildID = C.AggregationID 
		where ( HierarchyLevel = -3 );
	end;

	-- Return all the children
	select Code, ParentCode, [Name], [ShortName], [Type], HierarchyLevel, isActive, Hidden
	from #TEMP_CHILDREN_BUILDER
	order by HierarchyLevel DESC;
	
	-- drop the temporary tables
	drop table #TEMP_CHILDREN_BUILDER;
	
	-- Return all the metadata ids for metadata types which have values (if not robot)
	if ( @is_robot = 'false' )
	begin
		select L.MetadataTypeID, T.canFacetBrowse
		from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Types T
		where (  L.AggregationID = @aggregationid)
		  and ( L.MetadataTypeID = T.MetadataTypeID )
		  and ( L.Metadata_Count > 0 )
		  and ( LEN(T.SobekCode) > 0 )
		group by L.MetadataTypeID, DisplayTerm, T.canFacetBrowse
		order by DisplayTerm ASC;
	end;
	
	-- Check to see if the counts should be included
	if ( @include_counts = 'true' )
	begin
		-- Return some counts as well
		select count(distinct(I.GroupID)) as Title_Count, count(*) as Item_Count, isnull(SUM([PageCount]),0) as Page_Count
		from SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, SobekCM_Item_Aggregation A
		where ( A.Code = @code )
		  and ( A.AggregationID = L.AggregationID )
		  and ( L.ItemID = I.ItemID );
	end;
	
	-- Return all the parents (if not robot)
	if ( @is_robot = 'false' )
	begin
		select Code, [Name], [ShortName], [Type], isActive
		from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy H
		where A.AggregationID = H.ParentID 
		  and H.ChildID = @aggregationid;	
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Description_Tags_By_Aggregation]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Returns the list of any descriptive tags entered by users and
-- linked to an item aggregation.  If no code, or 'ALL', is passed in 
-- as the argument, then all descriptive tags are returned.
CREATE PROCEDURE [dbo].[SobekCM_Get_Description_Tags_By_Aggregation]
	@aggregationcode varchar(20)
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- If the code has length and is not 'ALL', return the descriptive tags for that aggregation
	if (( len( @aggregationcode) > 0 ) and ( @aggregationcode != 'ALL' ))
	begin
		-- Return tags linked to that aggregation code
		select U.FirstName, U.NickName, U.LastName, G.BibID, I.VID, T.Description_Tag, T.TagID, T.Date_Modified, U.UserID
		from mySobek_User U, mySobek_User_Description_Tags T, SobekCM_Item I, SobekCM_Item_Group G, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Aggregation A
		where ( I.ItemID = L.ItemID )
		  and ( L.AggregationID = A.AggregationID )
		  and ( A.Code = @aggregationcode )
		  and ( I.GroupID = G.GroupID )
		  and ( T.ItemID = I.ItemID )
		  and ( T.UserID = U.UserID )
		order by T.Date_Modified DESC;
	end
	else
	begin
		-- Return any descriptive tags
		select U.FirstName, U.NickName, U.LastName, G.BibID, I.VID, T.Description_Tag, T.TagID, T.Date_Modified, U.UserID
		from mySobek_User U, mySobek_User_Description_Tags T, SobekCM_Item I, SobekCM_Item_Group G
		where ( I.GroupID = G.GroupID )
		  and ( T.ItemID = I.ItemID )
		  and ( T.UserID = U.UserID )
		order by T.Date_Modified DESC;
	end;		  
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Collection_Statistics_History]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return the usage statistical information about a single item aggregation (or collection).
-- If the code is 'ALL', then the usage stats are aggregated up for all aggregations and
-- all items within this system.
CREATE PROCEDURE [dbo].[SobekCM_Get_Collection_Statistics_History]
	@code varchar(20)
AS
BEGIN
	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Should this pull all the data for ALL collections?  This is a lot more work
	-- since data is not naturally aggregated up for ALL aggregations, but rather each
	-- individual aggregation.  The web application should be caching this by writing
	-- a small file, so that this is pulled only once a day or so...
	if (( len(@code) = 0 ) or ( @code = 'ALL' ))
	begin

		-- Pull all the statistical data by item
		select [Year], [Month], sum( Hits ) as Item_Hits,
			sum( JPEG_Views ) as Item_JPEG_Views, sum ( Zoomable_Views ) as Item_Zoomable_Views,
			sum ( Citation_Views ) as Item_Citation_Views, sum ( Thumbnail_Views ) as Item_Thumbnail_Views,
			sum ( Text_Search_Views ) as Item_Text_Search_Views, sum ( Flash_Views ) as Item_Flash_Views,
			sum ( Google_Map_Views) as Item_Google_Map_Views, sum( Download_Views ) as item_Download_Views,
			sum ( Static_Views) as Item_Static_Views
		into #TEMP_ITEM_STATS
		from SobekCM_Item_Statistics
		group by [Year], [Month];

		-- Pull all the statistical data by group
		select [Year], [Month], sum( Hits ) as Title_Hits
		into #TEMP_GROUP_STATS
		from SobekCM_Item_Group_Statistics
		group by [Year], [Month];

		-- Pull the collection statistical information
		select [Year], [Month], sum( Home_Page_Views ) as Home_Page_Views,
			sum( Browse_Views ) as Browse_Views, sum ( Advanced_Search_Views ) as Advanced_Search_Views,
			sum ( Search_Results_Views ) as Search_Results_Views
		into #TEMP_HIERARCHY_STATS
		from SobekCM_Item_Aggregation_Statistics
		group by [Year], [Month];

		-- Pull all the statistical overall data (could be multiple if we have two URLs)
		select [Year], [Month], sum( Hits ) as Hits, sum( [Sessions] ) as Sessions
		into #TEMP_URL_STATS
		from SobekCM_Statistics
		group by [Year], [Month];

		-- Return the data
		select T3.[Year], T3.[Month], Hits, [Sessions], [Home_Page_Views], [Browse_Views], [Advanced_Search_Views], [Search_Results_Views], [Title_Hits], [Item_Hits], Item_JPEG_Views, Item_Zoomable_Views, Item_Citation_Views, Item_Thumbnail_Views, Item_Text_Search_Views, Item_Flash_Views, Item_Google_Map_Views, Item_Download_Views, Item_Static_Views
		from #TEMP_HIERARCHY_STATS AS T3 LEFT OUTER JOIN
			 #TEMP_ITEM_STATS AS T1 ON (( T3.[Year] = T1.[Year] ) and ( T3.[Month] = T1.[Month] )) LEFT OUTER JOIN
			 #TEMP_URL_STATS AS T2 ON (( T3.[Year] = T2.[Year] ) and ( T3.[Month] = T2.[Month] )) LEFT OUTER JOIN
			 #TEMP_GROUP_STATS AS T4 ON (( T3.[Year] = T4.[Year] ) and ( T3.[Month] = T4.[Month] ))
		order by T3.[Year], T3.[Month];

		-- Drop the temporary tables
		drop table #TEMP_ITEM_STATS;
		drop table #TEMP_GROUP_STATS;
		drop table #TEMP_URL_STATS;
		drop table #TEMP_HIERARCHY_STATS;

	end
	else
	begin

		-- Since this is for a single aggregation, simply return the data from the 
		-- aggregation statistics table
		select [Year], [Month], Hits, [Sessions], [Home_Page_Views], [Browse_Views], [Advanced_Search_Views], [Search_Results_Views], [Title_Hits], [Item_Hits], Item_JPEG_Views, Item_Zoomable_Views, Item_Citation_Views, Item_Thumbnail_Views, Item_Text_Search_Views, Item_Flash_Views, Item_Google_Map_Views, Item_Download_Views, Item_Static_Views
		from SobekCM_Item_Aggregation_Statistics S, SobekCM_Item_Aggregation C
		where ( C.Code = @code )
		  and ( C.AggregationID = S.AggregationID )
		order by [Year], [Month];

	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Coordinate_Points_By_Aggregation]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the list of all point coordinates for a single aggregation
CREATE PROCEDURE [dbo].[SobekCM_Coordinate_Points_By_Aggregation]
	@aggregation_code varchar(20)
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Return the distinct points
	select F.Point_Latitude, F.Point_Longitude, G.BibID, G.GroupTitle, G.GroupThumbnail, G.ItemCount, G.[Type]
	from SobekCM_Item_Group G, SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item_Footprint F, SobekCM_Item_Aggregation A
	where ( G.GroupID = I.GroupID )
	  and ( I.ItemID = L.ItemID  )
	  and ( L.AggregationID = A.AggregationID )
	  and ( A.Code = @aggregation_code ) 
	  and ( F.ItemID = I.ItemID )
	  and ( F.Point_Latitude is not null )
	  and ( F.Point_Longitude is not null )
	group by I.Spatial_KML, F.Point_Latitude, F.Point_Longitude, G.BibID, G.GroupTitle, G.GroupThumbnail, G.ItemCount, G.[Type]
	order by I.Spatial_KML;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Clear_Region_Link_By_Item]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Clears any link between a region and an item
CREATE PROCEDURE [dbo].[SobekCM_Clear_Region_Link_By_Item] 
	@itemid int
AS
begin

	-- Delete all lnks
	delete from SobekCM_Item_GeoRegion_Link
	where ItemID = @itemid;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Clear_Old_Item_Info]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Clears all the periphery data about an item in UFDC
CREATE PROCEDURE [dbo].[SobekCM_Clear_Old_Item_Info]
	@ItemID int
AS
begin

		-- Delete all matching links between streets and pages (if table exists)
		IF ( OBJECT_ID('Auth_Street_Page_Link') IS NOT NULL )
		BEGIN
			delete from Auth_Street_Page_Link where ItemID = @itemid;
		END;

		-- Delete all matching links between features and pages (if table exists)
		IF ( OBJECT_ID('Auth_Feature_Page_Link') IS NOT NULL )
		BEGIN
			delete from Auth_Feature_Page_Link where ItemID = @itemid;
		END;

		-- Delete all lnks to georegion (if table exists)
		IF ( OBJECT_ID('SobekCM_Item_GeoRegion_Link') IS NOT NULL )
		BEGIN
			delete from SobekCM_Item_GeoRegion_Link where ItemID = @itemid;
		END;

		-- Deletes the immediate geographic footprint (if table exists)
		IF ( OBJECT_ID('SobekCM_Item_Footprint') IS NOT NULL )
		BEGIN
			delete from SobekCM_Item_Footprint where ItemID = @ItemID;
		END;

end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_All_Groups]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the names of all the collection groups which are part of the 'SEARCH ALL' functionality.
-- Written by Mark Sullivan (September 2005), Updated ( January 2010 )
CREATE PROCEDURE [dbo].[SobekCM_Get_All_Groups]
	@metadata_count_to_use_cache int
AS
begin 

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Create the temporary table variable
	declare @TEMP_CHILDREN_BUILDER table ( AggregationID int primary key, Code varchar(20), ParentCode varchar(20), Name nvarchar(255), ShortName nvarchar(100), [Type] nvarchar(50), HierarchyLevel int, isActive bit, Hidden bit );

	-- Get the aggregation id for 'all'
	declare @aggregationid int;
	
	-- Get the aggregation id
	select @aggregationid = AggregationID
	from SobekCM_Item_Aggregation AS C 
	where ( C.Code = 'all' );
	
	-- Return information about this aggregation
	select AggregationID, Code, [Name], isnull(ShortName,[Name]) AS ShortName, [Type], isActive, Hidden, HasNewItems,
	   ContactEmail, DefaultInterface, [Description], Map_Display, Map_Search, OAI_Flag, OAI_Metadata, DisplayOptions, 
	   LastItemAdded=(select MAX(CreateDate) from SobekCM_Item), Can_Browse_Items, Items_Can_Be_Described, External_Link
	from SobekCM_Item_Aggregation AS C 
	where ( C.AggregationID=@aggregationid );
	
	-- Return every metadata term for which any data is present
	if ( ( select COUNT(*) from SobekCM_Item_Aggregation_Metadata_Link where AggregationID=@aggregationid ) > @metadata_count_to_use_cache )
	begin
		-- Just pull the cached links here
		select distinct(S.MetadataTypeID), T.canFacetBrowse, DisplayTerm
		from SobekCM_Item_Aggregation_Metadata_Link S, 
			SobekCM_Metadata_Types T
		where ( S.MetadataTypeID = T.MetadataTypeID )
		  and ( S.AggregationID = @aggregationid )
		group by S.MetadataTypeID, DisplayTerm, T.canFacetBrowse
		order by DisplayTerm ASC;	
		
		
	end
	else
	begin
		-- Just pull this from the actual metadata links then
		select distinct(S.MetadataTypeID), T.canFacetBrowse, DisplayTerm
		from SobekCM_Metadata_Unique_Search_Table S, 
			SobekCM_Metadata_Types T
		where ( S.MetadataTypeID = T.MetadataTypeID )
		group by S.MetadataTypeID, DisplayTerm, T.canFacetBrowse
		order by DisplayTerm ASC;
		
		
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_All_Browse_Paged]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return a single browse for the entire digital library
-- Written by Mark Sullivan ( September 2011 )
CREATE PROCEDURE [dbo].[SobekCM_Get_All_Browse_Paged]
	@date varchar(10),
	@include_private bit,
	@pagesize int, 
	@pagenumber int,
	@sort int,	
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@item_count_to_use_cached int,
	@total_items int output,
	@total_titles int output
AS
begin 

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
	
	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int;
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Ensure there is a date value
	select @date=ISNULL(@date,'1/1/1900');

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;
	
	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code='all' );


	-- There are essentially two major paths of execution, depending on whether this should
	-- be grouped as items within the page requested titles ( sorting by title or the standard 
	-- create date sort which still lumps them this way ) or whether each item should be
	-- returned by itself, such as sorting by individual publication dates, etc..
	
	if ( @sort < 10 )
	begin	
		-- Create temporary title table variable
		declare @TEMP_TITLES table ( TitleID int primary key, BibID varchar(10), RowNumber int );		
		
		-- Return the total counts, if requested
		select @total_items=COUNT(distinct I.ItemID), @total_titles=COUNT(distinct I.GroupID)
		from SobekCM_Item I
		where ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date );
		  
		-- Now, calculate the actual ending row, based on the ration, page information,
		-- and the lookahead factor		
		-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
		if (( @total_items > 0 ) and ( @total_titles > 0 ))
		begin
			declare @computed_value int;
			select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
			
			-- Compute the minimum value.  This cannot be less than @minpagelookahead.
			declare @floored_value int;
			select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
			
			-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
			declare @actual_pages int;
			select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead));

			-- Set the final row again then
			set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1;  
		end;	

		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
		 (	select G.GroupID, G.BibID, 
				ROW_NUMBER() OVER (order by case when @sort=1 THEN G.SortTitle end,
											case when @sort=0 THEN Max(I.CreateDate) end DESC,
											case when @sort=2 THEN BibID end ASC,
											case when @sort=3 THEN BibID end DESC) as RowNumber
				from SobekCM_Item I, SobekCM_Item_Group G
				where ( I.GroupID = G.GroupID )
				  and ( I.IP_Restriction_Mask >= @lower_mask )
				  and ( I.CreateDate >= @date )
				group by G.GroupID, G.BibID, G.SortTitle )
				  
		-- Insert the correct rows into the temp title table	
		insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
		select GroupID, BibID, RowNumber
		from TITLES_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		
		-- Return the title information for this page
		select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_TITLES T, SobekCM_Item_Group G
		where ( T.TitleID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the item information for this page
		select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, ISNULL(I.SortTitle, UPPER(title)) as SortTitle
		from SobekCM_Item I, @TEMP_TITLES T
		where ( T.TitleID = I.GroupID )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
	end
	else
	begin
		-- Create the temporary item table variable
		declare @TEMP_ITEMS table ( ItemID int primary key, RowNumber int );
				
		-- Return the total counts, if requested
		select @total_items=COUNT(distinct I.ItemID)
		from SobekCM_Item I
		where ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date );
		  
		-- Since these sorts make each item paired with a single title row,
		-- number of items and titles are equal
		set @total_titles = @total_items;
		
		-- In addition, always return the max lookahead pages
		set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
		
		-- Create saved select across items for row numbers
		with ITEMS_SELECT AS
		 (	select I.ItemID, 
				ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
											case when @sort=11 THEN isnull(SortDate,-1) end DESC) as RowNumber
				from SobekCM_Item I
				where ( I.IP_Restriction_Mask >= @lower_mask )
				  and ( I.CreateDate >= @date )
				group by I.ItemID, SortDate )
					  
		-- Insert the correct rows into the temp item table	
		insert into @TEMP_ITEMS ( ItemID, RowNumber )
		select ItemID, RowNumber
		from ITEMS_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		  
		-- Return the title information for this page
		select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
		where ( T.ItemID = I.ItemID )
		  and ( I.GroupID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the item information for this page
		select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display, '') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, ISNULL(I.SortTitle, UPPER(title)) as SortTitle
		from SobekCM_Item I, @TEMP_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
	end;
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin	
	
		-- Since this is an aggregation browse, can possibly use the cached
		-- metadata links to the aggregation for the facets.  Only do if this is
		-- over the value provided though
		if (( @total_items >= @item_count_to_use_cached ) and ( @date <= '1/1/1990' ))
		begin
		
			-- Build the aggregation list
			if ( @date > '1/1/1900' )
			begin
				-- Since this was for NEW items, will need to calculate the counts here.
				-- This takes a LONG time, so we avoid it in as many cases as possible
				with AGGREGATION_COUNTS AS 
					(	select distinct( AggregationID ), COUNT(*) as Item_Count
						from SobekCM_Item_Aggregation_Item_Link as L inner join
							 SobekCM_Item as I on L.ItemID = I.ItemID
						where ( I.IP_Restriction_Mask >= @lower_mask )
						  and ( I.CreateDate >= @date )
						group by AggregationID )			
				select top 100 A.Code, A.ShortName, Item_Count as Metadata_Count
				from SobekCM_Item_Aggregation A, AGGREGATION_COUNTS L
				where ( L.AggregationID = A.AggregationID )
				  and ( A.Include_In_Collection_Facet = 'true' )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )	
				order by Metadata_Count DESC, ShortName ASC;
			end
			else
			begin
				-- Just use the last current item count
				select top 100 A.Code, A.ShortName, Current_Item_Count as Metadata_Count
				from SobekCM_Item_Aggregation A
				where ( A.Include_In_Collection_Facet = 'true' )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( Current_Item_Count > 0 )
				order by Metadata_Count DESC, ShortName ASC;			
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype1 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype2 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype3 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype4 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype5 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype6 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype7 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype8 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		
		end
		else
		begin
		
			-- Get the list of all item ids 
			select distinct(I.ItemID) as ItemID
			into #TEMP_ITEMS_FACETS
			from SobekCM_Item I
			where ( I.IP_Restriction_Mask >= @lower_mask )
			  and ( I.CreateDate >= @date );	  
			  			  
			-- Build the aggregation list
			if ( @date > '1/1/1900' )
			begin
				-- Since this was for NEW items, will need to calculate the counts here.
				-- This takes a LONG time, so we avoid it in as many cases as possible
				select top 100 A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link CL, #TEMP_ITEMS_FACETS T
				where ( CL.ItemID = T.ItemID )
				  and ( CL.AggregationID = A.AggregationID )
				  and ( A.Include_In_Collection_Facet = 'true' )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;
			end
			else
			begin
				-- Just use the last current item count
				select top 100 A.Code, A.ShortName, Current_Item_Count as Metadata_Count
				from SobekCM_Item_Aggregation A
				where ( A.Include_In_Collection_Facet = 'true' )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( Current_Item_Count > 0 )
				order by Metadata_Count DESC, ShortName ASC;			
			end;
			

			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype1 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype2 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype3 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype4 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype5 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype6 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype7 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype8 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- drop this temporary table
			drop table #TEMP_ITEMS_FACETS;
		end;
	end;
	
	SET NOCOUNT ON;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Aggregation_Browse_Paged]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return a single browse for a collection or group
-- Written by Mark Sullivan ( December 2006 )
CREATE PROCEDURE [dbo].[SobekCM_Get_Aggregation_Browse_Paged]
	@code varchar(20),
	@date varchar(10),
	@include_private bit,
	@pagesize int, 
	@pagenumber int,
	@sort int,	
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@item_count_to_use_cached int,
	@total_items int output,
	@total_titles int output
AS
begin

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
		
	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- Ensure there is a date value
	select @date=ISNULL(@date,'1/1/1900');

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;

	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@code );

	-- There are essentially two major paths of execution, depending on whether this should
	-- be grouped as items within the page requested titles ( sorting by title or the standard 
	-- create date sort which still lumps them this way ) or whether each item should be
	-- returned by itself, such as sorting by individual publication dates, etc..
	
	if ( @sort < 10 )
	begin	
		-- Create temporary title table variale
		declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int );		
		
		-- Return the total counts, if requested
		select @total_items=COUNT(distinct I.ItemID), @total_titles=COUNT(distinct I.GroupID)
		from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
		where ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date )
		  and ( I.Dark = 'false' );
		  
		-- Now, calculate the actual ending row, based on the ration, page information,
		-- and the lookahead factor		
		-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
		if (( @total_items > 0 ) and ( @total_titles > 0 ))
		begin
			declare @computed_value int;
			select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
			
			-- Compute the minimum value.  This cannot be less than @minpagelookahead.
			declare @floored_value int;
			select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
			
			-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
			declare @actual_pages int;
			select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead));

			-- Set the final row again then
			set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1;  
		end;	

		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
		 (	select G.GroupID, G.BibID, 
				ROW_NUMBER() OVER (order by case when @sort=1 THEN G.SortTitle end,
											case when @sort=0 THEN Max(I.CreateDate) end DESC,
											case when @sort=2 THEN BibID end ASC,
											case when @sort=3 THEN BibID end DESC) as RowNumber
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Group G
				where ( CL.ItemID = I.ItemID )
				  and ( CL.AggregationID = @aggregationid )
				  and ( I.GroupID = G.GroupID )
				  and ( I.IP_Restriction_Mask >= @lower_mask )
				  and ( I.CreateDate >= @date )
				  and ( I.Dark = 'false' )
				group by G.GroupID, G.BibID, G.SortTitle )
				  
		-- Insert the correct rows into the temp title table	
		insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
		select GroupID, BibID, RowNumber
		from TITLES_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		
		-- Return the title information for this page
		select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_TITLES T, SobekCM_Item_Group G
		where ( T.TitleID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the item information for this page
		select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, ISNULL(I.SortTitle, UPPER(title)) as SortTitle
		from SobekCM_Item I, @TEMP_TITLES T, SobekCM_Item_Aggregation_Item_Link CL
		where ( T.TitleID = I.GroupID )
		  and ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
	end
	else
	begin
		-- Create the temporary item table variable
		declare @TEMP_ITEMS table ( ItemID int, RowNumber int );
				
		-- Return the total counts, if requested
		select @total_items=COUNT(distinct I.ItemID)
		from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
		where ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.CreateDate >= @date )
		  and ( I.Dark = 'false' );
		  
		-- Since these sorts make each item paired with a single title row,
		-- number of items and titles are equal
		set @total_titles = @total_items;
		
		-- In addition, always return the max lookahead pages
		set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
		
		if ( @sort < 12 )
		begin		
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN isnull(SortDate,-1) end DESC ) as RowNumber
					from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
					where ( CL.ItemID = I.ItemID )
					  and ( CL.AggregationID = @aggregationid )
					  and ( I.IP_Restriction_Mask >= @lower_mask )
					  and ( I.CreateDate >= @date )
					  and ( I.Dark = 'false' )
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
		end
		else
		begin
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=12 THEN I.CreateDate end DESC ) as RowNumber
					from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
					where ( CL.ItemID = I.ItemID )
					  and ( CL.AggregationID = @aggregationid )
					  and ( I.IP_Restriction_Mask >= @lower_mask )
					  and ( I.CreateDate >= @date )
					  and ( I.Dark = 'false' )
					group by I.ItemID, I.CreateDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
		end		
		  
		-- Return the title information for this page
		select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from @TEMP_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
		where ( T.ItemID = I.ItemID )
		  and ( I.GroupID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the item information for this page
		select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display, '') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, ISNULL(SortTitle, upper(Title)) as SortTitle
		from SobekCM_Item I, @TEMP_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
	end
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin			
	
		-- Since this is an aggregation browse, can possibly use the cached
		-- metadata links to the aggregation for the facets.  Only do if this is
		-- over the value provided though
		if (( @total_items >= @item_count_to_use_cached ) and ( @date <= '1/1/1990' ))
		begin
				
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype1 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype2 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype3 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;	
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype4 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype5 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype6 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype7 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select top(100) MetadataValue, Metadata_Count
				from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
				where ( L.AggregationID = @aggregationid ) and ( L.MetadataID = M.MetadataID ) and ( L.MetadataTypeID = @facettype8 ) and ( L.OrderNum <= 100 )
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		
		end
		else
		begin
		
			-- Get the list of all item ids 
			select distinct(I.ItemID) as ItemID
			into #TEMP_ITEMS_FACETS
			from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
			where ( CL.ItemID = I.ItemID )
			  and ( CL.AggregationID = @aggregationid )
			  and ( I.IP_Restriction_Mask >= @lower_mask )
			  and ( I.CreateDate >= @date );

			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype1 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype2 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype3 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;	
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype4 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype5 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype6 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype7 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from  ( select top(100) T.*
						from (	select L.MetadataID, Metadata_Count = COUNT(*)
								from SobekCM_Metadata_Unique_Link L, SobekCM_Metadata_Unique_Search_Table U, #TEMP_ITEMS_FACETS I
								where ( L.ItemID = I.ItemID )
								  and ( L.MetadataID = U.MetadataID )
								  and ( U.MetadataTypeID = @facettype8 )
								group by L.MetadataID ) T
						 order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- drop this temporary table
			drop table #TEMP_ITEMS_FACETS;
		end;
	end;
	
	SET NOCOUNT ON;
end;
GO
/****** Object:  StoredProcedure [dbo].[Save_Item_Tracking_Info]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Save_Item_Tracking_Info] 
	@ItemID int,
	@Original_AccessCode varchar(25),
	@EmbargoEnd date,
	@UMI varchar(20)
AS
BEGIN

	-- Only insert if it doesn't exist
	if ( exists ( select * from Tracking_Item where ItemID=@ItemID ))
	begin
		--update existing, not updating 'original_' columns
		update Tracking_Item
		set EmbargoEnd = @EmbargoEnd, UMI=@UMI;
	end
	else
	begin
		-- Insert ALL the data
		insert into Tracking_Item ( ItemID, Original_AccessCode, Original_EmbargoEnd, EmbargoEnd, UMI )
		values ( @ItemID, @Original_AccessCode, @EmbargoEnd, @EmbargoEnd, @UMI );
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[PalmmRightsMD_Save_Access_Embargo_UMI]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PalmmRightsMD_Save_Access_Embargo_UMI]
	@ItemID int,
	@Original_AccessCode varchar(25),
	@EmbargoEnd date,
	@UMI varchar(20)
AS
BEGIN

	-- Only insert if it doesn't exist
	if ( exists ( select * from Tracking_Item where ItemID=@ItemID ))
	begin
		--update existing, not updating 'original_' columns
		update Tracking_Item
		set EmbargoEnd = @EmbargoEnd, UMI=@UMI
		where ItemID=@ItemID;
	end
	else
	begin
		-- Insert ALL the data
		insert into Tracking_Item ( ItemID, Original_AccessCode, Original_EmbargoEnd, EmbargoEnd, UMI )
		values ( @ItemID, @Original_AccessCode, @EmbargoEnd, @EmbargoEnd, @UMI );
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_View_All_User_Tags]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- View all of a single user's tag
CREATE PROCEDURE [dbo].[mySobek_View_All_User_Tags]
	@UserID int
AS
begin

	if ( @UserID < 0 )
	begin
		select T.TagID, G.BibID, I.VID, T.Description_Tag, T.Date_Modified, U.UserID, U.FirstName, U.NickName, U.LastName 
		from mySobek_User_Description_Tags T, mySobek_User U, SobekCM_Item I, SobekCM_Item_Group G
		where T.UserID=U.UserID
		  and T.ItemID = I.ItemID
		  and I.GroupID = G.GroupID
	end
	else
	begin
		select T.TagID, G.BibID, I.VID, T.Description_Tag, T.Date_Modified, U.UserID, U.FirstName, U.NickName, U.LastName 
		from mySobek_User_Description_Tags T, mySobek_User U, SobekCM_Item I, SobekCM_Item_Group G
		where T.UserID=U.UserID 
		  and T.UserID=@UserID
		  and T.ItemID = I.ItemID
		  and I.GroupID = G.GroupID
	end
end
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Admin_Suggest_User_Item_Links]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure tries to match users that can submit with authors and suggests new
-- possible relationships for the users and items
CREATE PROCEDURE [dbo].[SobekCM_Admin_Suggest_User_Item_Links]
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Create a temporary table for all matching creator metadata
	create table #TEMP1 ( 	UserID int, ItemID int, FirstName nvarchar(100), LastName nvarchar(100), Email nvarchar(100), MetadataValue nvarchar(max), Title nvarchar(max));

	-- Create temporary variables to hold each author's information for searching
	declare @userid int;
	declare @firstname nvarchar(100);
	declare @lastname nvarchar(100);
	declare @nickname nvarchar(100);
	declare @email nvarchar(100);

	-- Create a cursor to go through all users that can submit to this system
	declare author_cursor CURSOR FOR 
	select UserID, '"' + FirstName + '"', '"' + LastName + '"', '"' + NickName + '"','"' + EmailAddress + '"' from mySobek_User where Can_Submit_Items='true';

	OPEN author_cursor;

	-- Get the first author
	FETCH NEXT FROM author_cursor 
	INTO @userid, @firstname, @lastname, @nickname, @email;

	-- Do while there are more authors to dor
	WHILE @@FETCH_STATUS = 0
	BEGIN

		insert into #TEMP1 ( UserID, ItemID, FirstName, LastName, Email, MetadataValue, Title )	   
		select @userid, L.itemid, @firstname, @lastname, @email, MetadataValue, I.Title
		from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L, SobekCM_Item I 
		where ( contains( MetadataValue, @lastname )) 
		  and (( contains( MetadataValue, @firstname )) or ( contains( MetadataValue, @nickname )))
		  and ( L.MetadataID=M.MetadataID ) 
		  and ( M.MetadataTypeID = 4 )
		  and ( L.ItemID = I.ItemID )
		  and ( not exists ( select * from mySobek_User_Item_Link U where U.UserID=@userid and U.ItemID=L.ItemID));

		-- Get the next author
		FETCH NEXT FROM author_cursor 
		INTO @userid, @firstname, @lastname, @nickname, @email;
	END;

	-- Done with the author cursor
	CLOSE author_cursor;
	DEALLOCATE author_cursor;

	select * from #TEMP1
	order by LastName, FirstName, UserID, ItemID;

	drop table #TEMP1;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Delete_Item_Aggregation]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure to delete an item aggregation and unlink it completely.
-- This fails if there are any child aggregations.  This does not really
-- delete the item aggregation, just marks it as DELETED and removed most
-- references.  The statistics are retained.
CREATE PROCEDURE [dbo].[SobekCM_Delete_Item_Aggregation]
	@aggrcode varchar(20),
	@message varchar(1000) output,
	@errorcode int output
AS
BEGIN
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
			delete from SobekCM_Item_Aggregation_Item_Link
			where AggregationID = @aggrid;
			
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
			set Deleted = 'true';
			
		end;
	end
	else
	begin
		-- Since there was no match, set an error code and message
		set @message = 'No matching item aggregation found';
		set @errorcode = 1;
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Delete_Item]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Deletes an item, and deletes the group if there are no additional items attached
CREATE PROCEDURE [dbo].[SobekCM_Delete_Item] 
@bibid varchar(10),
@vid varchar(5),
@as_admin bit,
@delete_message varchar(1000)
 AS
begin transaction
	-- Perform transactionally in case there is a problem deleting some of the rows
	-- so the entire delete is rolled back

   declare @itemid int;
   set @itemid = 0;

    -- first to get the itemid of the specified bibid and vid
   select @itemid = isnull(I.itemid, 0)
   from SobekCM_Item I, SobekCM_Item_Group G
   where (G.bibid = @bibid) 
       and (I.vid = @vid)
       and ( I.GroupID = G.GroupID );

   -- if there is such an itemid in the UFDC database, then delete this item and its related information
  if ( isnull(@itemid, 0 ) > 0)
  begin

	-- Delete all references to this item 
	delete from SobekCM_Metadata_Unique_Link where ItemID=@itemid;
	delete from SobekCM_Metadata_Basic_Search_Table where ItemID=@itemid;
	delete from SobekCM_Item_Footprint where ItemID=@itemid;
	delete from SobekCM_Item_Icons where ItemID=@itemid;
	delete from SobekCM_Item_Statistics where ItemID=@itemid;
	delete from SobekCM_Item_GeoRegion_Link where ItemID=@itemid;
	delete from SobekCM_Item_Aggregation_Item_Link where ItemID=@itemid;
	delete from mySobek_User_Item where ItemID=@itemid;
	delete from mySobek_User_Item_Link where ItemID=@itemid;
	delete from mySobek_User_Description_Tags where ItemID=@itemid;
	delete from SobekCM_Item_Viewers where ItemID=@itemid;

	delete from Tracking_Progress where ItemID=@itemid;
	
	if ( @as_admin = 'true' )
	begin
		delete from Tracking_Archive_Item_Link where ItemID=@itemid;
		update Tivoli_File_Log set DeleteMsg=@delete_message, ItemID = -1 where ItemID=@itemid;
	end;
	
	-- Finally, delete the item 
	delete from SobekCM_Item where ItemID=@itemid;
	
	-- Delete the item group if it is the last one existing
	if (( select count(I.ItemID) from SobekCM_Item_Group G, SobekCM_Item I where ( G.BibID = @bibid ) and ( G.GroupID = I.GroupID ) and ( I.Deleted = 0 )) < 1 )
	begin
		
		declare @groupid int;
		set @groupid = 0;	
		
		-- first to get the itemid of the specified bibid and vid
		select @groupid = isnull(G.GroupID, 0)
		from SobekCM_Item_Group G
		where (G.bibid = @bibid);
		
		-- Delete if this selected something
		if ( ISNULL(@groupid, 0 ) > 0 )
		begin		
			-- delete from the item group table	and all references
			delete from SobekCM_Item_Group_External_Record where GroupID=@groupid;
			delete from SobekCM_Item_Group_Web_Skin_Link where GroupID=@groupid;
			delete from SobekCM_Item_Group_Statistics where GroupID=@groupid;
			delete from mySobek_User_Bib_Link where GroupID=@groupid;
			delete from SobekCM_Item_Group_OAI where GroupID=@groupid;
			delete from SobekCM_Item_Group where GroupID=@groupid;
		end;
	end
	else
	begin
		-- Finally set the volume count for this group correctly
		update SobekCM_Item_Group
		set ItemCount = ( select count(*) from SobekCM_Item I where ( I.GroupID = SobekCM_Item_Group.GroupID ))	
		where ( SobekCM_Item_Group.BibID = @bibid );
	end;
  end;
   
commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Delete_Icon]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Delete an existing Wordmark, and output the number of links to that wordmark
-- If there are any items linked to that wordmark, the icon is not deleted
CREATE PROCEDURE [dbo].[SobekCM_Delete_Icon]
	@icon_code varchar(255),
	@links int output
AS
begin

	-- Get the number of links
	select @links = count(*) from SobekCM_Item_Icons L, SobekCM_Icon I where I.Icon_Name = @icon_code and I.IconID = L.IconID;

	-- If there are no links, delete this icon
	if ( @links = 0 )
	begin
		delete from SobekCM_Icon where Icon_Name = @icon_code;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Item_Count_By_Collection_By_Dates]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Item_Count_By_Collection_By_Dates]
	@date1 datetime,
	@date2 datetime
AS
BEGIN

	if ( isnull( @date2, '1/1/2000' ) = '1/1/2000' )
	begin

		-- Select the list of collections and items
		select AggregationID, I.ItemID, I.FileCount, I.PageCount, I.GroupID, Milestone_OnlineComplete
		into #TEMP1
		from  SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item I
		where ( CL.ItemID = I.ItemID )
		  and ( I.Deleted = 'false' )
		  and ( Milestone_OnlineComplete is not null )

		-- Now, pull only the items which have the right dates
		select *
		into #TEMP1_DATE1
		from #TEMP1
		where Milestone_OnlineComplete <= @date1

		-- Start to build the return set of values
		select Row_Type=C.TEMP_OldType, group_code = Code, coll_code = '                    ', subcollect_code = '                    ', [Name], C.isActive AS Active,
			title_count = ( select count(distinct(GroupID)) from #TEMP1 where #TEMP1.AggregationID = C.AggregationID ),
			item_count = ( select count(distinct(ItemID)) from #TEMP1 where #TEMP1.AggregationID = C.AggregationID ), 
			page_count = isnull(( select sum( PageCount ) from #TEMP1 where #TEMP1.AggregationID = C.AggregationID ), 0),
			title_count_date1 = ( select count(distinct(GroupID)) from #TEMP1_DATE1 where #TEMP1_DATE1.AggregationID = C.AggregationID ),
			item_count_date1 = ( select count(distinct(ItemID)) from #TEMP1_DATE1 where #TEMP1_DATE1.AggregationID = C.AggregationID ), 
			page_count_date1 = isnull(( select sum( [PageCount] ) from #TEMP1_DATE1 where #TEMP1_DATE1.AggregationID = C.AggregationID ), 0),
			cast(C.AggregationID as int) AS ID, C.Hidden
		into #TEMP_COLLECTIONS
		from SobekCM_Item_Aggregation C
		where (C.Hidden = 'false') AND ( C.Code <> 'TESTCOL' ) AND ( C.Code <> 'TESTG' )

		-- Build the hierarchy
		update #TEMP_COLLECTIONS
		SET coll_code = group_code, group_code = (select Top(1) Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID),
			ID = (select Top(1) A.AggregationID from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID)
		where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID)
		 
		-- Build the hierarchy
		update #TEMP_COLLECTIONS
		SET subcollect_code = coll_code, coll_code = group_code, group_code = (select Top(1) Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID)
		where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID)

		-- Get total item count
		declare @total_item_count int
		select @total_item_count =  ( select count(*) from SobekCM_Item where Deleted = 0 )

		-- Get total title count
		declare @total_title_count int
		select @total_title_count =  ( select count(*) from SobekCM_Item_Group where Deleted = 0 )

		-- Get total title count
		declare @total_page_count int
		select @total_page_count =  isnull(( select sum( PageCount ) from SobekCM_Item where Deleted = 0 ), 0 )

		-- get the list of items from date1
		select I.ItemID, GroupID, PageCount
		into #TEMP_DATE1
		from SobekCM_Item I
		where ( I.Deleted = 'false' )
		  and ( Milestone_OnlineComplete is not null )
		  and ( Milestone_OnlineComplete <= @date1 )

		-- Get total item count
		declare @total_item_count_date1 int
		select @total_item_count_date1 =  ( select count(distinct(ItemID)) from #TEMP_DATE1  )

		-- Get total title count
		declare @total_title_count_date1 int
		select @total_title_count_date1 =  ( select count(distinct(GroupID)) from #TEMP_DATE1  )

		-- Get total title count
		declare @total_page_count_date1 int
		select @total_page_count_date1 =  isnull(( select sum( PageCount ) from #TEMP_DATE1  ),0)

		-- Add one row with the total item count
		insert into #TEMP_COLLECTIONS
		values ('z', 'ZZZ', '', '', 'Total Count', 1, @total_title_count, @total_item_count, @total_page_count, 
			@total_title_count_date1, @total_item_count_date1, @total_page_count_date1, -1, 0 )

		-- Return the final table
		select Row_Type, Group_Code, Coll_Code, SubCollect_Code, [Name], Active, Hidden, Title_Count, Item_Count, Page_Count, Title_Count_Date1, Item_Count_Date1, Page_Count_Date1
		from #TEMP_COLLECTIONS
		order by group_code, coll_code, subcollect_code

		drop table #TEMP1
		drop table #TEMP_DATE1
		drop table #TEMP1_DATE1
		drop table #TEMP_COLLECTIONS	

	end
	else
	begin

		-- Select the list of collections and items
		select CL.AggregationID, I.ItemID, I.FileCount, I.[PageCount], I.GroupID, Milestone_OnlineComplete
		into #TEMQ1
		from  SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item I
		where ( CL.ItemID = I.ItemID )
		  and ( I.Deleted = 'false' )
		  and ( Milestone_OnlineComplete is not null )

		-- Now, pull only the items which have the right dates
		select *
		into #TEMQ1_DATE1
		from #TEMQ1
		where Milestone_OnlineComplete <= @date1

		select *
		into #TEMQ1_DATE2
		from #TEMQ1
		where Milestone_OnlineComplete <= @date2

		-- Start to build the return set of values
		select Row_Type=C.TEMP_OldType, group_code = Code, coll_code = '                    ', subcollect_code = '                    ', [Name], C.isActive AS Active,
			title_count = ( select count(distinct(GroupID)) from #TEMQ1 where #TEMQ1.AggregationID = C.AggregationID ),
			item_count = ( select count(distinct(ItemID)) from #TEMQ1 where #TEMQ1.AggregationID = C.AggregationID ), 
			page_count = isnull(( select sum( PageCount ) from #TEMQ1 where #TEMQ1.AggregationID = C.AggregationID ), 0),
			title_count_date1 = ( select count(distinct(GroupID)) from #TEMQ1_DATE1 T1 where T1.AggregationID = C.AggregationID ),
			item_count_date1 = ( select count(distinct(ItemID)) from #TEMQ1_DATE1 T1 where T1.AggregationID = C.AggregationID ), 
			page_count_date1 = isnull(( select sum( PageCount ) from #TEMQ1_DATE1 T1 where T1.AggregationID = C.AggregationID ), 0),
			title_count_date2 = ( select count(distinct(GroupID)) from #TEMQ1_DATE2 T2 where T2.AggregationID = C.AggregationID ),
			item_count_date2 = ( select count(distinct(ItemID)) from #TEMQ1_DATE2 T2 where T2.AggregationID = C.AggregationID ), 
			page_count_date2 = isnull(( select sum( PageCount ) from #TEMQ1_DATE2 T2 where T2.AggregationID = C.AggregationID ), 0),
			cast(C.AggregationID as int) AS ID, C.Hidden
		into #TEMQ_COLLECTIONS
		from SobekCM_Item_Aggregation C
		where (C.Hidden = 'false') AND ( C.Code <> 'TESTCOL' ) AND ( C.Code <> 'TESTG' )

		-- Build the hierarchy
		update #TEMQ_COLLECTIONS
		SET coll_code = group_code, group_code = (select Top(1) Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID),
			ID = (select Top(1) A.AggregationID from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID)
		where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID)
		 
		-- Build the hierarchy
		update #TEMQ_COLLECTIONS
		SET subcollect_code = coll_code, coll_code = group_code, group_code = (select Top(1) Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID)
		where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID)

		-- Get total item count
		declare @total_item_count2 int
		select @total_item_count2 =  ( select count(*) from SobekCM_Item where Deleted = 0 )

		-- Get total title count
		declare @total_title_count2 int
		select @total_title_count2 =  ( select count(*) from SobekCM_Item_Group where Deleted = 0 )

		-- Get total title count
		declare @total_page_count2 int
		select @total_page_count2 =  isnull(( select sum( PageCount ) from SobekCM_Item where Deleted = 0 ), 0)

		-- get the list of items from date1
		select I.ItemID, GroupID, PageCount
		into #TEMQ_DATE1
		from SobekCM_Item I
		where ( I.Deleted = 0 )
		  and ( Milestone_OnlineComplete <= @date1 )

		-- Get total item count
		declare @total_item_count2_date1 int
		select @total_item_count2_date1 =  ( select count(distinct(ItemID)) from #TEMQ_DATE1  )

		-- Get total title count
		declare @total_title_count2_date1 int
		select @total_title_count2_date1 =  ( select count(distinct(GroupID)) from #TEMQ_DATE1  )

		-- Get total title count
		declare @total_page_count2_date1 int
		select @total_page_count2_date1 =  isnull(( select sum( PageCount ) from #TEMQ_DATE1  ), 0)

		-- get the list of items from date2
		select I.ItemID, GroupID, [PageCount]
		into #TEMQ_DATE2
		from SobekCM_Item I
		where ( I.Deleted = 'false' )
		  and ( Milestone_OnlineComplete is not null )
		  and ( Milestone_OnlineComplete <= @date2 )

		-- Get total item count
		declare @total_item_count2_date2 int
		select @total_item_count2_date2 =  ( select count(distinct(ItemID)) from #TEMQ_DATE2  )

		-- Get total title count
		declare @total_title_count2_date2 int
		select @total_title_count2_date2 =  ( select count(distinct(GroupID)) from #TEMQ_DATE2  )

		-- Get total title count
		declare @total_page_count2_date2 int
		select @total_page_count2_date2 =  ( select sum( PageCount ) from #TEMQ_DATE2  )

		-- Add one row with the total item count
		insert into #TEMQ_COLLECTIONS
		values ('z', 'ZZZ', '', '', 'Total Count', 1, @total_title_count2, @total_item_count2, @total_page_count2, 
			@total_title_count2_date1, @total_item_count2_date1, @total_page_count2_date1, 
			@total_title_count2_date2, @total_item_count2_date2, @total_page_count2_date2, -1, 0 )

		-- Return the final table
		select Row_Type, Group_Code, Coll_Code, SubCollect_Code, [Name], Active, Hidden, Title_Count, Item_Count, Page_Count, 
			Title_Count_Date1, Item_Count_Date1, Page_Count_Date1,
			Title_Count_Date2, Item_Count_Date2, Page_Count_Date2
		from #TEMQ_COLLECTIONS
		order by group_code, coll_code, subcollect_code

		drop table #TEMQ1
		drop table #TEMQ_DATE1
		drop table #TEMQ_DATE2
		drop table #TEMQ1_DATE1
		drop table #TEMQ1_DATE2
		drop table #TEMQ_COLLECTIONS	
	end
END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Item_Count_By_Collection_By_Date_Range]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Item_Count_By_Collection_By_Date_Range]
	@date1 datetime,
	@date2 datetime
AS
BEGIN
		-- Select the list of collections and items
		select CL.AggregationID, I.ItemID, I.FileCount, I.[PageCount], I.GroupID, Milestone_OnlineComplete
		into #TEMQ1
		from  SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item I
		where ( CL.ItemID = I.ItemID )
		  and ( I.Deleted = 'false' )
		  and ( Milestone_OnlineComplete is not null )

		-- Now, pull only the items which have the right dates
		select *
		into #TEMQ1_DATE1
		from #TEMQ1
		where Milestone_OnlineComplete < @date1

		select *
		into #TEMQ1_DATE2
		from #TEMQ1
		where ( Milestone_OnlineComplete >= @date1 ) 
		  and ( Milestone_OnlineComplete < @date2 )
		  
		-- Start to build the return set of values
		select Row_Type=C.TEMP_OldType, group_code = Code, coll_code = '                    ', subcollect_code = '                    ', [Name], C.isActive AS Active,
			title_count_date1 = ( select count(distinct(GroupID)) from #TEMQ1_DATE1 T1 where T1.AggregationID = C.AggregationID ),
			item_count_date1 = ( select count(distinct(ItemID)) from #TEMQ1_DATE1 T1 where T1.AggregationID = C.AggregationID ), 
			page_count_date1 = isnull(( select sum( PageCount ) from #TEMQ1_DATE1 T1 where T1.AggregationID = C.AggregationID ), 0),
			title_count_date2 = ( select count(distinct(GroupID)) from #TEMQ1_DATE2 T2 where T2.AggregationID = C.AggregationID ),
			item_count_date2 = ( select count(distinct(ItemID)) from #TEMQ1_DATE2 T2 where T2.AggregationID = C.AggregationID ), 
			page_count_date2 = isnull(( select sum( PageCount ) from #TEMQ1_DATE2 T2 where T2.AggregationID = C.AggregationID ), 0),
			cast(C.AggregationID as int) AS ID, C.Hidden
		into #TEMQ_COLLECTIONS
		from SobekCM_Item_Aggregation C
		where (C.Hidden = 'false') AND ( C.Code <> 'TESTCOL' ) AND ( C.Code <> 'TESTG' )

		-- Build the hierarchy
		update #TEMQ_COLLECTIONS
		SET coll_code = group_code, group_code = (select Top(1) Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID),
			ID = (select Top(1) A.AggregationID from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID)
		where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID)
		 
		-- Build the hierarchy
		update #TEMQ_COLLECTIONS
		SET subcollect_code = coll_code, coll_code = group_code, group_code = (select Top(1) Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID)
		where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMQ_COLLECTIONS.ID)

		-- Get total item count
		declare @total_item_count2 int
		select @total_item_count2 =  ( select count(*) from SobekCM_Item where Deleted = 0 )

		-- Get total title count
		declare @total_title_count2 int
		select @total_title_count2 =  ( select count(*) from SobekCM_Item_Group where Deleted = 0 )

		-- Get total title count
		declare @total_page_count2 int
		select @total_page_count2 =  isnull(( select sum( PageCount ) from SobekCM_Item where Deleted = 0 ), 0)

		-- get the list of items from date1
		select I.ItemID, GroupID, PageCount
		into #TEMQ_DATE1
		from SobekCM_Item I
		where ( I.Deleted = 0 )
		  and ( CreateDate < @date1 )
		  
		-- Get total item count
		declare @total_item_count2_date1 int
		select @total_item_count2_date1 =  ( select count(distinct(ItemID)) from #TEMQ_DATE1  )

		-- Get total title count
		declare @total_title_count2_date1 int
		select @total_title_count2_date1 =  ( select count(distinct(GroupID)) from #TEMQ_DATE1  )

		-- Get total title count
		declare @total_page_count2_date1 int
		select @total_page_count2_date1 =  isnull(( select sum( [PageCount]) from #TEMQ_DATE1  ), 0)

		-- get the list of items from date2
		select I.ItemID, GroupID, PageCount
		into #TEMQ_DATE2
		from SobekCM_Item I
		where ( I.Deleted = 'false' )
		  and ( Milestone_OnlineComplete is not null )
		  and ( Milestone_OnlineComplete >= @date1 ) 
		  and ( Milestone_OnlineComplete < @date2 )

		-- Get total item count
		declare @total_item_count2_date2 int
		select @total_item_count2_date2 =  ( select count(distinct(ItemID)) from #TEMQ_DATE2  )

		-- Get total title count
		declare @total_title_count2_date2 int
		select @total_title_count2_date2 =  ( select count(distinct(GroupID)) from #TEMQ_DATE2  )

		-- Get total title count
		declare @total_page_count2_date2 int
		select @total_page_count2_date2 =  isnull(( select sum( [PageCount] ) from #TEMQ_DATE2  ), 0)

		-- Add one row with the total item count
		insert into #TEMQ_COLLECTIONS
		values ('z', 'ZZZ', '', '', 'Total Count', 1,  
			@total_title_count2_date1, @total_item_count2_date1, @total_page_count2_date1, 
			@total_title_count2_date2, @total_item_count2_date2, @total_page_count2_date2, -1, 0 )

		-- Return the final table
		select Row_Type, Group_Code, Coll_Code, SubCollect_Code, [Name], Active, Hidden,  
			Title_Count_Date1, Item_Count_Date1, Page_Count_Date1,
			Title_Count_Date2, Item_Count_Date2, Page_Count_Date2
		from #TEMQ_COLLECTIONS
		order by group_code, coll_code, subcollect_code

		drop table #TEMQ1
		drop table #TEMQ_DATE1
		drop table #TEMQ_DATE2
		drop table #TEMQ1_DATE1
		drop table #TEMQ1_DATE2
		drop table #TEMQ_COLLECTIONS	
END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Item_Count_By_Collection]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Item_Count_By_Collection]
AS
BEGIN

	-- Select the list of collections and items
	select AggregationID, I.ItemID, I.FileCount, I.[PageCount], I.GroupID
	into #TEMP1
	from  SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item I
	where ( CL.ItemID = I.ItemID )
	  and ( I.Deleted = 'false' )
	  and ( Milestone_OnlineComplete is not null )

	-- Start to build the return set of values
	select Row_Type=C.TEMP_OldType, group_code = Code, coll_code = '                    ', subcollect_code = '                    ', [Name], C.isActive AS Active,
		title_count = ( select count(distinct(GroupID)) from #TEMP1 where #TEMP1.AggregationID = C.AggregationID ),
		item_count = ( select count(distinct(ItemID)) from #TEMP1 where #TEMP1.AggregationID = C.AggregationID ), 
		page_count = isnull(( select sum( PageCount ) from #TEMP1 where #TEMP1.AggregationID = C.AggregationID ), 0),
        cast(C.AggregationID as int) AS ID, C.Hidden
	into #TEMP_COLLECTIONS
	from SobekCM_Item_Aggregation C
	where ( C.Code <> 'TESTCOL' ) AND ( C.Code <> 'TESTG' )

	-- Build the hierarchy
	update #TEMP_COLLECTIONS
	SET coll_code = group_code, group_code = (select Top(1) Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID),
		ID = (select Top(1) A.AggregationID from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID)
	where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID)
	 
	-- Build the hierarchy
	update #TEMP_COLLECTIONS
	SET subcollect_code = coll_code, coll_code = group_code, group_code = (select Top(1) Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID)
	where exists (select Code from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Hierarchy L where A.AggregationID = L.ParentID and L.ChildID = #TEMP_COLLECTIONS.ID)

	-- Get total item count
	declare @total_item_count int
	select @total_item_count =  ( select count(*) from SobekCM_Item where Deleted = 'false'  and Milestone_OnlineComplete is not null )

	-- Get total title count
	declare @total_page_count int
	select @total_page_count =  ( select sum( PageCount ) from SobekCM_Item where Deleted = 'false' and Milestone_OnlineComplete is not null)
	
	-- Get total title count
	declare @total_title_count int
	select @total_title_count =  ( select count(distinct(GroupID)) from #TEMP1 )


	-- Add one row with the total item count
	insert into #TEMP_COLLECTIONS
	values ('z', 'ZZZ', '', '', 'Total Count', 1, @total_title_count, @total_item_count, @total_page_count, -1, 0 )

	-- Return the final table
	select Row_Type, Group_Code, Coll_Code, SubCollect_Code, [Name], Active, Hidden, Title_Count, Item_Count, Page_Count
	from #TEMP_COLLECTIONS
	order by group_code, coll_code, subcollect_code

	drop table #TEMP1
	drop table #TEMP_COLLECTIONS	

END
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_OAI_Data]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Return a list of the OAI data to server through the OAI-PMH server
CREATE PROCEDURE [dbo].[SobekCM_Get_OAI_Data]
	@aggregationcode varchar(20),
	@data_code varchar(20),
	@from date,
	@until date,
	@pagesize int, 
	@pagenumber int,
	@include_data bit
AS
begin

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Do not need to maintain row counts
	SET NoCount ON;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
		
	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	
	-- Rowend is calculated normally, but then an additional item is
	-- added at the end which will be used to determine if a resumption
	-- token should be issued
	set @rowend = (@rowstart + @pagesize - 1) + 1; 
	
	-- Ensure there are date values
	if ( @from is null )
		set @from = CONVERT(date,'19000101');
	if ( @until is null )
		set @until = GETDATE();
	
	-- Is this for a single aggregation
	if (( @aggregationcode is not null ) and ( LEN(@aggregationcode) > 0 ) and ( @aggregationcode != 'all' ))
	begin	
		-- Determine the aggregationid
		declare @aggregationid int;
		set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@aggregationcode );
			  
		-- Should the actual data be returned, or just the identifiers?
		if ( @include_data='true')
		begin
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
			(	select G.GroupID, BibID,
				ROW_NUMBER() OVER (order by O.OAI_Date ASC ) as RowNumber
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Group G, SobekCM_Item_Group_OAI O
				where ( CL.ItemID = I.ItemID )
				  and ( CL.AggregationID = @aggregationid )
				  and ( I.GroupID = G.GroupID )
				  and ( G.GroupID = O.GroupID )
				  and ( G.Suppress_OAI = 'false' )
				  and ( O.OAI_Date >= @from )
				  and ( O.OAI_Date <= @until )
				  and ( O.Data_Code = @data_code )
				group by G.GroupID, G.BibID, O.OAI_Date )
				
			-- Select the matching rows
			select T.GroupID, BibID, O.OAI_Data, O.OAI_Date
			from TITLES_SELECT T, SobekCM_Item_Group_OAI O
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend
			  and T.GroupID = O.GroupID;			 
		end
		else
		begin
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
			(	select G.GroupID, BibID,
				ROW_NUMBER() OVER (order by O.OAI_Date ASC ) as RowNumber
				from SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL, SobekCM_Item_Group G, SobekCM_Item_Group_OAI O
				where ( CL.ItemID = I.ItemID )
				  and ( CL.AggregationID = @aggregationid )
				  and ( I.GroupID = G.GroupID )
				  and ( G.GroupID = O.GroupID )
				  and ( G.Suppress_OAI = 'false' )
				  and ( O.OAI_Date >= @from )
				  and ( O.OAI_Date <= @until )
				  and ( O.Data_Code = @data_code )
				group by G.GroupID, G.BibID, O.OAI_Date )
				
			-- Select the matching rows
			select T.GroupID, BibID, O.OAI_Date
			from TITLES_SELECT T, SobekCM_Item_Group_OAI O
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend
			  and T.GroupID = O.GroupID;		
		end;		  
	end
	else
	begin
				  
		-- Should the actual data be returned, or just the identifiers?
		if ( @include_data='true')
		begin
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
			(	select G.GroupID, BibID,
				ROW_NUMBER() OVER (order by O.OAI_Date ASC) as RowNumber
				from SobekCM_Item_Group G, SobekCM_Item_Group_OAI O
				where ( G.GroupID = O.GroupID )
				  and ( G.Suppress_OAI = 'false' )
				  and ( O.OAI_Date >= @from )
				  and ( O.OAI_Date <= @until )
				  and ( O.Data_Code = @data_code )
				group by G.GroupID, G.BibID, O.OAI_Date  )				
								
			-- Select the matching rows
			select T.GroupID, BibID, O.OAI_Data, O.OAI_Date
			from TITLES_SELECT T, SobekCM_Item_Group_OAI O
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend
			  and T.GroupID = O.GroupID;				 
		end
		else
		begin
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
			(	select G.GroupID, BibID,
				ROW_NUMBER() OVER (order by O.OAI_Date ASC) as RowNumber
				from SobekCM_Item_Group G, SobekCM_Item_Group_OAI O
				where ( G.GroupID = O.GroupID )
				  and ( G.Suppress_OAI = 'false' )
				  and ( O.OAI_Date >= @from )
				  and ( O.OAI_Date <= @until )
				  and ( O.Data_Code = @data_code )
				group by G.GroupID, G.BibID, O.OAI_Date  )				
								
			-- Select the matching rows
			select T.GroupID, BibID, O.OAI_Date
			from TITLES_SELECT T, SobekCM_Item_Group_OAI O
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend
			  and T.GroupID = O.GroupID;	
		end;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Metadata_Browse]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure allows the distinct metadata values in an aggregation to be viewed
CREATE PROCEDURE [dbo].[SobekCM_Get_Metadata_Browse]
	@aggregation_code varchar(20),
	@metadata_name varchar(100),
	@item_count_to_use_cached int
AS
begin
	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	-- Get the metadata type id
	declare @metadatatypeid int;
	set @metadatatypeid = ( select ISNULL(MetadataTypeID, -1) from SobekCM_Metadata_Types where MetadataName = @metadata_name );

	-- Get the aggregation id
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID, -1) from SobekCM_Item_Aggregation where Code=@aggregation_code );	

	-- Seperate code for metadata browsing ALL ITEMS, versus by aggregation
	if (( LEN(@aggregation_code) = 0 ) or ( @aggregation_code='all' ))
	begin
		-- How many items exist?
		if (( select COUNT(*) from SobekCM_Item where IP_Restriction_Mask >= 0 ) >= @item_count_to_use_cached )
		begin
			-- Now get the distinct values from the cached table
			select L.MetadataID, MetadataValue
			from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
			where ( L.AggregationID = @aggregationid )
			  and ( L.MetadataID = M.MetadataID )
			  and ( L.MetadataTypeID = @metadatatypeid )	
			order by MetadataValue;
		end
		else
		begin
			-- Now get the distinct values by actually going through the item links, but this
			-- time do it irrespective of which aggregations items are linked to 
			select distinct(S.MetadataID), MetadataValue
			from SobekCM_Metadata_Unique_Search_Table S, SobekCM_Metadata_Unique_Link X, SobekCM_Item L
			where ( S.MetadataTypeID = @metadatatypeid )
			  and ( X.ItemID = L.ItemID )
			  and ( X.MetadataID = S.MetadataID )
			  and ( L.IP_Restriction_Mask >= 0 )
			order by MetadataValue;
		end;	
	end
	else
	begin

		-- How many items are linked to this aggregation?
		if ( ( select COUNT(*) from SobekCM_Item_Aggregation_Item_Link where AggregationID = @aggregationid  ) >= @item_count_to_use_cached )
		begin
			-- Now get the distinct values from the cached table
			select L.MetadataID, MetadataValue
			from SobekCM_Item_Aggregation_Metadata_Link L, SobekCM_Metadata_Unique_Search_Table M
			where ( L.AggregationID = @aggregationid )
			  and ( L.MetadataID = M.MetadataID )
			  and ( L.MetadataTypeID = @metadatatypeid )	
			order by MetadataValue;
		end
		else
		begin
			-- Now get the distinct values by actually going through the item links
			select distinct(S.MetadataID), MetadataValue
			from SobekCM_Metadata_Unique_Search_Table S, SobekCM_Metadata_Unique_Link X, SobekCM_Item_Aggregation_Item_Link L
			where ( S.MetadataTypeID = @metadatatypeid )
			  and ( X.ItemID = L.ItemID )
			  and ( X.MetadataID = S.MetadataID )
			  and ( L.AggregationID = @aggregationid )
			order by MetadataValue;
		end;
	end;
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Get_Items_By_Coordinates]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedure returns the items by a coordinate search
CREATE PROCEDURE [dbo].[SobekCM_Get_Items_By_Coordinates]
	@lat1 float,
	@long1 float,
	@lat2 float,
	@long2 float,
	@include_private bit,
	@aggregationcode varchar(20),
	@pagesize int, 
	@pagenumber int,
	@sort int,	
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #TEMPSUBZERO ( ItemID int );
	create table #TEMPZERO ( ItemID int );
	create table #TEMP_ITEMS ( ItemID int, fk_TitleID int, SortDate bigint, Spatial_KML varchar(4000), Spatial_KML_Distance float );

	-- Is this really just a point search?
	if (( isnull(@lat2,1000) = 1000 ) or ( isnull(@long2,1000) = 1000 ) or (( @lat1=@lat2 ) and ( @long1=@long2 )))
	begin

		-- Select all matching item ids
		insert into #TEMPZERO
		select distinct(itemid) 
		from SobekCM_Item_Footprint
		where (( Point_Latitude = @lat1 ) and ( Point_Longitude = @long1 ))
		   or (((( Rect_Latitude_A >= @lat1 ) and ( Rect_Latitude_B <= @lat1 )) or (( Rect_Latitude_A <= @lat1 ) and ( Rect_Latitude_B >= @lat1)))
	        and((( Rect_Longitude_A >= @long1 ) and ( Rect_Longitude_B <= @long1 )) or (( Rect_Longitude_A <= @long1 ) and ( Rect_Longitude_B >= @long1 ))));

	end
	else
	begin

		-- Select all matching item ids by rectangle
		insert into #TEMPSUBZERO
		select distinct(itemid)
		from SobekCM_Item_Footprint
		where ((( Point_Latitude <= @lat1 ) and ( Point_Latitude >= @lat2 )) or (( Point_Latitude >= @lat1 ) and ( Point_Latitude <= @lat2 )))
		  and ((( Point_Longitude <= @long1 ) and ( Point_Longitude >= @long2 )) or (( Point_Longitude >= @long1 ) and ( Point_Longitude <= @long2 )));
		
		-- Select rectangles which OVERLAP with this rectangle
		insert into #TEMPSUBZERO
		select distinct(itemid)
		from SobekCM_Item_Footprint
		where (((( Rect_Latitude_A >= @lat1 ) and ( Rect_Latitude_A <= @lat2 )) or (( Rect_Latitude_A <= @lat1 ) and ( Rect_Latitude_A >= @lat2 )))
			or ((( Rect_Latitude_B >= @lat1 ) and ( Rect_Latitude_B <= @lat2 )) or (( Rect_Latitude_B <= @lat1 ) and ( Rect_Latitude_B >= @lat2 ))))
		  and (((( Rect_Longitude_A >= @long1 ) and ( Rect_Longitude_A <= @long2 )) or (( Rect_Longitude_A <= @long1 ) and ( Rect_Longitude_A >= @long2 )))
			or ((( Rect_Longitude_B >= @long1 ) and ( Rect_Longitude_B <= @long2 )) or (( Rect_Longitude_B <= @long1 ) and ( Rect_Longitude_B >= @long2 ))));
		
		-- Select rectangles that INCLUDE this rectangle by picking overlaps with one point
		insert into #TEMPSUBZERO
		select distinct(itemid)
		from SobekCM_Item_Footprint
		where ((( @lat1 <= Rect_Latitude_A ) and ( @lat1 >= Rect_Latitude_B )) or (( @lat1 >= Rect_Latitude_A ) and ( @lat1 <= Rect_Latitude_B )))
		  and ((( @long1 <= Rect_Longitude_A ) and ( @long1 >= Rect_Longitude_B )) or (( @long1 >= Rect_Longitude_A ) and ( @long1 <= Rect_Longitude_B )));

		-- Make sure uniqueness applies here as well
		insert into #TEMPZERO
		select distinct(itemid)
		from #TEMPSUBZERO;
	end;
	
	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;

	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@aggregationcode );

			
	-- Was an aggregation included?
	if ( LEN(ISNULL( @aggregationcode,'' )) > 0 )
	begin	
		-- Look for matching the provided aggregation
		insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate, Spatial_KML, Spatial_KML_Distance )
		select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1), Spatial_KML=isnull(Spatial_KML,''), Spatial_KML_Distance
		from #TEMPZERO T1, SobekCM_Item I, SobekCM_Item_Aggregation_Item_Link CL
		where ( CL.ItemID = I.ItemID )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.Deleted = 'false' )
		  and ( T1.ItemID = I.ItemID )
		  and ( I.IP_Restriction_Mask >= @lower_mask );
	end
	else
	begin	
		-- Look for matching the provided aggregation
		insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate, Spatial_KML, Spatial_KML_Distance )
		select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1), Spatial_KML=isnull(Spatial_KML,''), Spatial_KML_Distance
		from #TEMPZERO T1, SobekCM_Item I
		where ( I.Deleted = 'false' )
		  and ( T1.ItemID = I.ItemID )
		  and ( I.IP_Restriction_Mask >= @lower_mask );
	end;
	
	-- There are essentially THREE major paths of execution, depending on whether this should
	-- be grouped as items within the page requested titles ( sorting by title or the basic
	-- sorting by rank, which ranks this way ) or whether each item should be
	-- returned by itself, such as sorting by individual publication dates, etc..
	-- The default sort for this search is by spatial coordiantes, in which case the same 
	-- title should appear multiple times, if the items in the volume have different coordinates
	
	if ( @sort = 0 )
	begin
		-- create the temporary title table definition
		create table #TEMP_TITLES_ITEMS ( TitleID int, BibID varchar(10), RowNumber int, Spatial_KML varchar(4000), Spatial_Distance float );
		
		-- Compute the number of seperate titles/coordinates
		select fk_TitleID, (COUNT(Spatial_KML)) as assign_value
		into #TEMP1
		from #TEMP_ITEMS I
		group by fk_TitleID, Spatial_KML;
		
		-- Get the TOTAL count of spatial_kmls
		select @total_titles = isnull(SUM(assign_value), 0) from #TEMP1;
		drop table #TEMP1;
		
		-- Total items is simpler to computer
		select @total_items = COUNT(*) from #TEMP_ITEMS;	
		
		-- For now, always return the max lookahead pages
		set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
		
		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
			(	select GroupID, G.BibID, Spatial_KML, Spatial_KML_Distance,
					ROW_NUMBER() OVER (order by Spatial_KML_Distance ASC, Spatial_KML ASC) as RowNumber
				from #TEMP_ITEMS I, SobekCM_Item_Group G
				where I.fk_TitleID = G.GroupID
				group by G.GroupID, G.BibID, G.SortTitle, Spatial_KML, Spatial_KML_Distance )

		-- Insert the correct rows into the temp title table	
		insert into #TEMP_TITLES_ITEMS ( TitleID, BibID, RowNumber, Spatial_KML, Spatial_Distance )
		select GroupID, BibID, RowNumber, Spatial_KML, Spatial_KML_Distance
		from TITLES_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		  
		-- Return the title information for this page
		select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from #TEMP_TITLES_ITEMS T, SobekCM_Item_Group G
		where ( T.TitleID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the item information for this page
		select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text,  isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( I.Spatial_KML, '') as Spatial_KML
		from #TEMP_ITEMS M, #TEMP_TITLES_ITEMS T, SobekCM_Item I
		where ( M.fk_TitleID = T.TitleID )
		  and ( I.ItemID = M.ItemID )
		  and ( M.Spatial_KML = T.Spatial_KML )
		  and ( M.Spatial_KML_Distance = T.Spatial_Distance )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		
		-- drop the temporary table
		drop table #TEMP_TITLES_ITEMS;	
	end;
	
	if (( @sort < 10 ) and ( @sort > 0 ))
	begin	
		-- create the temporary title table definition
		create table #TEMP_TITLES ( TitleID int, BibID varchar(10), RowNumber int );
		
		-- Get the total counts
		select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
		from #TEMP_ITEMS; 
		
		-- Now, calculate the actual ending row, based on the ration, page information,
		-- and the lookahead factor
		
		-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
		declare @computed_value int;
		select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
		
		-- Compute the minimum value.  This cannot be less than @minpagelookahead.
		declare @floored_value int;
		select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
		
		-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
		declare @actual_pages int;
		select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead)); 

		-- Set the final row again then
		set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 		
				  
		-- Create saved select across titles for row numbers
		with TITLES_SELECT AS
			(	select GroupID, G.BibID, 
					ROW_NUMBER() OVER (order by case when @sort=1 THEN G.SortTitle end ASC,											
												case when @sort=2 THEN BibID end ASC,
											    case when @sort=3 THEN BibID end DESC) as RowNumber
				from #TEMP_ITEMS I, SobekCM_Item_Group G
				where I.fk_TitleID = G.GroupID
				group by G.GroupID, G.BibID, G.SortTitle )

		-- Insert the correct rows into the temp title table	
		insert into #TEMP_TITLES ( TitleID, BibID, RowNumber )
		select GroupID, BibID, RowNumber
		from TITLES_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
	
		-- Return the title information for this page
		select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from #TEMP_TITLES T, SobekCM_Item_Group G
		where ( T.TitleID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the item information for this page
		select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text,  isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( I.Spatial_KML, '') as Spatial_KML
		from #TEMP_ITEMS M, #TEMP_TITLES T, SobekCM_Item I
		where ( M.fk_TitleID = T.TitleID )
		  and ( I.ItemID = M.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		
		-- drop the temporary table
		drop table #TEMP_TITLES;
	end;
	
	if ( @sort >= 10 )
	begin
		-- Create the temporary item table for paging purposes
		create table #TEMP_PAGED_ITEMS ( ItemID int, RowNumber int );
		
		-- Since these sorts make each item paired with a single title row,
		-- number of items and titles are equal
		select @total_items=COUNT(*), @total_titles=COUNT(*)
		from #TEMP_ITEMS; 
		
		-- In addition, always return the max lookahead pages
		set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
		
		-- Create saved select across items for row numbers
		with ITEMS_SELECT AS
		 (	select I.ItemID, 
				ROW_NUMBER() OVER (order by case when @sort=10 THEN SortDate end ASC,
											case when @sort=11 THEN SortDate end DESC) as RowNumber
				from #TEMP_ITEMS I
				group by I.ItemID, SortDate )
					  
		-- Insert the correct rows into the temp item table	
		insert into #TEMP_PAGED_ITEMS ( ItemID, RowNumber )
		select ItemID, RowNumber
		from ITEMS_SELECT
		where RowNumber >= @rowstart
		  and RowNumber <= @rowend;
		  
		-- Return the title information for this page
		select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
		from #TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
		where ( T.ItemID = I.ItemID )
		  and ( I.GroupID = G.GroupID )
		order by RowNumber ASC;
		
		-- Return the item information for this page
		select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( I.Spatial_KML, '') as Spatial_KML
		from SobekCM_Item I, #TEMP_PAGED_ITEMS T
		where ( T.ItemID = I.ItemID )
		order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		  				
		-- drop the temporary paged table
		drop table #TEMP_PAGED_ITEMS;	
	end
	
	-- Return the facets if asked for
	if ( @include_facets = 'true' )
	begin	
		-- Only return the aggregation codes if this was a search across all collections	
		if (( LEN( isnull( @aggregationcode, '')) = 0 ) or ( @aggregationcode='all'))
		begin
			-- Build the aggregation list
			select A.Code, A.ShortName, Metadata_Count=Count(*)
			from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
			where ( T.ItemID = I.ItemID )
			  and ( I.ItemID = L.ItemID )
			  and ( L.AggregationID = A.AggregationID )
			  and ( A.Hidden = 'false' )
			  and ( A.isActive = 'true' )
			  and ( A.Include_In_Collection_Facet = 'true' )
			group by A.Code, A.ShortName
			order by Metadata_Count DESC, ShortName ASC;	
		end;	
		
		-- Return the FIRST facet
		if ( @facettype1 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype1 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SECOND facet
		if ( @facettype2 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype2 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the THIRD facet
		if ( @facettype3 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype3 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;	
		
		-- Return the FOURTH facet
		if ( @facettype4 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype4 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the FIFTH facet
		if ( @facettype5 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype5 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SIXTH facet
		if ( @facettype6 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype6 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the SEVENTH facet
		if ( @facettype7 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype7 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
		
		-- Return the EIGHTH facet
		if ( @facettype8 > 0 )
		begin
			-- Return the first 100 values
			select MetadataValue, Metadata_Count
			from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
					from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
					where ( U.ItemID = I.ItemID )
					  and ( U.MetadataTypeID = @facettype8 )
					group by U.MetadataID
					order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
			where M.MetadataID = F.MetadataID
			order by Metadata_Count DESC, MetadataValue ASC;
		end;
	end;

	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	drop table #TEMPZERO;
	drop table #TEMPSUBZERO;

END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Create_Full_Citation_Value]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Takes all the individual pieces of metadata linked to an item, and then collapses them 
-- all together into a large single value to be stored for the basic searches which do
-- not indicate anything about which field each search term should appear in.
CREATE PROCEDURE [dbo].[SobekCM_Create_Full_Citation_Value]
	@itemid int
AS
begin

	-- Delete the old tracking box from the metadata unique link, since we wil pull
	-- in the latest value from the item table first
	delete from SobekCM_Metadata_Unique_Link 
	where ItemID=@itemid and exists ( select * from SobekCM_Metadata_Unique_Search_Table T where T.MetadataID = SobekCM_Metadata_Unique_Link.MetadataID and T.MetadataTypeID=36);

	-- Copy the tracking box from the item table
	if ( (select LEN(ISNULL(Tracking_Box,'')) from SobekCM_Item where ItemID=@itemid ) > 0 )
	begin
		-- Get the tracking box from the item table
		declare @tracking_box_add nvarchar(max);
		set @tracking_box_add = ( select Tracking_Box from SobekCM_Item where ItemID=@itemid );
		
		-- Save this in the single metadata portion, in case a search is done by 'tracking box'
		exec [SobekCM_Metadata_Save_Single] @itemid, 'Tracking Box', @tracking_box_add;
	end;
	
	-- Delete the old complete citation
	delete from SobekCM_Metadata_Basic_Search_Table
	where ItemID=@itemid;

	-- Prepare to step through each metadata value and build the full citation and also
	-- each individual search value for the 
	declare @singlevalue nvarchar(max);
	declare @metadatatype int;
	declare @fullcitation nvarchar(max);
	declare @title nvarchar(max);
	declare @type nvarchar(max);
	declare @language nvarchar(max);
	declare @creator nvarchar(max);
	declare @publisher nvarchar(max);
	declare @publication_place nvarchar(max);
	declare @subject_keyword nvarchar(max);
	declare @genre nvarchar(max);
	declare @target_audience nvarchar(max);
	declare @spatial_coverage nvarchar(max);
	declare @country nvarchar(max);
	declare @state nvarchar(max);
	declare @county nvarchar(max);
	declare @city nvarchar(max);
	declare @source_institution nvarchar(max);
	declare @holding_location nvarchar(max);
	declare @identifier nvarchar(max);
	declare @notes nvarchar(max);
	declare @other_citation nvarchar(max);
	declare @tickler nvarchar(max);
	declare @donor nvarchar(max);
	declare @format nvarchar(max);
	declare @bibid nvarchar(max);
	declare @publication_date nvarchar(max);
	declare @affiliation nvarchar(max);
	declare @frequency nvarchar(max);
	declare @name_as_subject nvarchar(max);
	declare @title_as_subject nvarchar(max);
	declare @all_subjects nvarchar(max);
	declare @temporal_subject nvarchar(max);
	declare @attribution nvarchar(max);
	declare @user_description nvarchar(max);
	declare @temporal_decade nvarchar(max);
	declare @mime_type nvarchar(max);
	declare @tracking_box nvarchar(max);
	declare @abstract nvarchar(max);
	declare @edition nvarchar(max);
	declare @toc nvarchar(max);
	declare @zt_kingdom nvarchar(max);
	declare @zt_phylum nvarchar(max);
	declare @zt_class nvarchar(max);
	declare @zt_order nvarchar(max);
	declare @zt_family nvarchar(max);
	declare @zt_genus nvarchar(max);
	declare @zt_species nvarchar(max);
	declare @zt_common_name nvarchar(max);
	declare @zt_scientific_name nvarchar(max);
	declare @zt_all_taxonomy nvarchar(max);
	declare @cultural_context nvarchar(max);
	declare @inscription nvarchar(max);
	declare @material nvarchar(max);
	declare @style_period nvarchar(max);
	declare @technique nvarchar(max);
	declare @accession_number nvarchar(max);
	declare @interviewee nvarchar(max);
	declare @interviewer nvarchar(max);
	declare @temporal_year nvarchar(max);
	declare @etd_committee nvarchar(max);
	declare @etd_degree nvarchar(max);
	declare @etd_degree_discipline nvarchar(max);
	declare @etd_degree_grantor nvarchar(max);
	declare @etd_degree_level nvarchar(max);
	declare @userdefined01 nvarchar(max);
	declare @userdefined02 nvarchar(max);
	declare @userdefined03 nvarchar(max);
	declare @userdefined04 nvarchar(max);
	declare @userdefined05 nvarchar(max);
	declare @userdefined06 nvarchar(max);
	declare @userdefined07 nvarchar(max);
	declare @userdefined08 nvarchar(max);
	declare @userdefined09 nvarchar(max);
	declare @userdefined10 nvarchar(max);
	declare @userdefined11 nvarchar(max);
	declare @userdefined12 nvarchar(max);
	declare @userdefined13 nvarchar(max);
	declare @userdefined14 nvarchar(max);
	declare @userdefined15 nvarchar(max);
	declare @userdefined16 nvarchar(max);
	declare @userdefined17 nvarchar(max);
	declare @userdefined18 nvarchar(max);
	declare @userdefined19 nvarchar(max);
	declare @userdefined20 nvarchar(max);
	declare @userdefined21 nvarchar(max);
	declare @userdefined22 nvarchar(max);
	declare @userdefined23 nvarchar(max);
	declare @userdefined24 nvarchar(max);
	declare @userdefined25 nvarchar(max);
	declare @userdefined26 nvarchar(max);
	declare @userdefined27 nvarchar(max);
	declare @userdefined28 nvarchar(max);
	declare @userdefined29 nvarchar(max);
	declare @userdefined30 nvarchar(max);
	declare @userdefined31 nvarchar(max);
	declare @userdefined32 nvarchar(max);
	declare @userdefined33 nvarchar(max);
	declare @userdefined34 nvarchar(max);
	declare @userdefined35 nvarchar(max);
	declare @userdefined36 nvarchar(max);
	declare @userdefined37 nvarchar(max);
	declare @userdefined38 nvarchar(max);
	declare @userdefined39 nvarchar(max);
	declare @userdefined40 nvarchar(max);
	declare @userdefined41 nvarchar(max);
	declare @userdefined42 nvarchar(max);
	declare @userdefined43 nvarchar(max);
	declare @userdefined44 nvarchar(max);
	declare @userdefined45 nvarchar(max);
	declare @userdefined46 nvarchar(max);
	declare @userdefined47 nvarchar(max);
	declare @userdefined48 nvarchar(max);
	declare @userdefined49 nvarchar(max);
	declare @userdefined50 nvarchar(max);
	declare @userdefined51 nvarchar(max);
	declare @userdefined52 nvarchar(max);
	
	set @fullcitation='';
	set @title='';
	set @type='';
	set @language='';
	set @creator='';
	set @publisher='';
	set @publication_place='';
	set @subject_keyword='';
	set @genre='';
	set @target_audience='';
	set @spatial_coverage='';
	set @country='';
	set @state='';
	set @county='';
	set @city='';
	set @source_institution='';
	set @holding_location='';
	set @identifier='';
	set @notes='';
	set @other_citation='';
	set @tickler='';
	set @donor='';
	set @format='';
	set @bibid='';
	set @publication_date='';
	set @affiliation='';
	set @frequency='';
	set @name_as_subject='';
	set @title_as_subject='';
	set @all_subjects='';
	set @temporal_subject='';
	set @attribution='';
	set @user_description='';
	set @temporal_decade='';
	set @mime_type='';
	set @tracking_box='';
	set @abstract='';
	set @edition='';
	set @toc='';
	set @zt_kingdom='';
	set @zt_phylum='';
	set @zt_class='';
	set @zt_order='';
	set @zt_family='';
	set @zt_genus='';
	set @zt_species='';
	set @zt_common_name='';
	set @zt_scientific_name='';
	set @zt_all_taxonomy='';
	set @cultural_context='';
	set @inscription='';
	set @material='';
	set @style_period='';
	set @technique='';
	set @accession_number='';
	set @interviewee ='';
	set @interviewer ='';
	set @temporal_year ='';
	set @etd_committee ='';
	set @etd_degree ='';
	set @etd_degree_discipline ='';
	set @etd_degree_grantor ='';
	set @etd_degree_level ='';
	set @userdefined01 ='';
	set @userdefined02 ='';
	set @userdefined03 ='';
	set @userdefined04 ='';
	set @userdefined05 ='';
	set @userdefined06 ='';
	set @userdefined07 ='';
	set @userdefined08 ='';
	set @userdefined09 ='';
	set @userdefined10 ='';
	set @userdefined11 ='';
	set @userdefined12 ='';
	set @userdefined13 ='';
	set @userdefined14 ='';
	set @userdefined15 ='';
	set @userdefined16 ='';
	set @userdefined17 ='';
	set @userdefined18 ='';
	set @userdefined19 ='';
	set @userdefined20 ='';
	set @userdefined21 ='';
	set @userdefined22 ='';
	set @userdefined23 ='';
	set @userdefined24 ='';
	set @userdefined25 ='';
	set @userdefined26 ='';
	set @userdefined27 ='';
	set @userdefined28 ='';
	set @userdefined29 ='';
	set @userdefined30 ='';
	set @userdefined31 ='';
	set @userdefined32 ='';
	set @userdefined33 ='';
	set @userdefined34 ='';
	set @userdefined35 ='';
	set @userdefined36 ='';
	set @userdefined37 ='';
	set @userdefined38 ='';
	set @userdefined39 ='';
	set @userdefined40 ='';
	set @userdefined41 ='';
	set @userdefined42 ='';
	set @userdefined43 ='';
	set @userdefined44 ='';
	set @userdefined45 ='';
	set @userdefined46 ='';
	set @userdefined47 ='';
	set @userdefined48 ='';
	set @userdefined49 ='';
	set @userdefined50 ='';
	set @userdefined51 ='';
	set @userdefined52 ='';
		
	-- Use a cursor to step through all the metadata linked to this item
	declare metadatacursor cursor read_only
	for (select MetadataValue, MetadataTypeID
	    from SobekCM_Metadata_Unique_Search_Table M, SobekCM_Metadata_Unique_Link L
	    where L.ItemID=@itemid 
	      and L.MetadataID = M.MetadataID
	      and M.MetadataTypeID != 35);

	-- Open the cursor to begin stepping through all the unique metadata
	open metadatacursor;

	-- Get the first metadata value
	fetch next from metadatacursor into @singlevalue, @metadatatype;

	while @@fetch_status = 0
	begin
		-- Build the full citation by adding each single value to the full citation
		-- being built
		set @fullcitation = @fullcitation + ' | ' + @singlevalue;
		
		-- Now, build each smaller metadata value
		if ( @metadatatype = 1 ) set @title=@title + ' | ' + @singlevalue;
		if ( @metadatatype = 2 ) set @type=@type + ' | ' + @singlevalue;
		if ( @metadatatype = 3 ) set @language=@language + ' | ' + @singlevalue;
		if ( @metadatatype = 4 ) set @creator=@creator + ' | ' + @singlevalue;
		if ( @metadatatype = 5 ) set @publisher=@publisher + ' | ' + @singlevalue;
		if ( @metadatatype = 6 ) set @publication_place=@publication_place + ' | ' + @singlevalue;
		if ( @metadatatype = 7 ) set @subject_keyword=@subject_keyword + ' | ' + @singlevalue;
		if ( @metadatatype = 8 ) set @genre=@genre + ' | ' + @singlevalue;
		if ( @metadatatype = 9 ) set @target_audience=@target_audience + ' | ' + @singlevalue;
		if ( @metadatatype = 10 ) set @spatial_coverage=@spatial_coverage + ' | ' + @singlevalue;
		if ( @metadatatype = 11 ) set @country=@country + ' | ' + @singlevalue;
		if ( @metadatatype = 12 ) set @state=@state + ' | ' + @singlevalue;
		if ( @metadatatype = 13 ) set @county=@county + ' | ' + @singlevalue;
		if ( @metadatatype = 14 ) set @city=@city + ' | ' + @singlevalue;
		if ( @metadatatype = 15 ) set @source_institution=@source_institution + ' | ' + @singlevalue;
		if ( @metadatatype = 16 ) set @holding_location=@holding_location + ' | ' + @singlevalue;
		if ( @metadatatype = 17 ) set @identifier=@identifier + ' | ' + @singlevalue;
		if ( @metadatatype = 18 ) set @notes=@notes + ' | ' + @singlevalue;
		if ( @metadatatype = 19 ) set @other_citation=@other_citation + ' | ' + @singlevalue;
		if ( @metadatatype = 20 ) set @tickler=@tickler + ' | ' + @singlevalue;
		if ( @metadatatype = 21 ) set @donor=@donor + ' | ' + @singlevalue;
		if ( @metadatatype = 22 ) set @format=@format + ' | ' + @singlevalue;
		if ( @metadatatype = 23 ) set @bibid=@bibid + ' | ' + @singlevalue;
		if ( @metadatatype = 24 ) set @publication_date=@publication_date + ' | ' + @singlevalue;
		if ( @metadatatype = 25 ) set @affiliation=@affiliation + ' | ' + @singlevalue;
		if ( @metadatatype = 26 ) set @frequency=@frequency + ' | ' + @singlevalue;
		if ( @metadatatype = 27 ) set @name_as_subject=@name_as_subject + ' | ' + @singlevalue;
		if ( @metadatatype = 28 ) set @title_as_subject=@title_as_subject + ' | ' + @singlevalue;
		if ( @metadatatype = 29 ) set @all_subjects=@all_subjects + ' | ' + @singlevalue;
		if ( @metadatatype = 30 ) set @temporal_subject=@temporal_subject + ' | ' + @singlevalue;
		if ( @metadatatype = 31 ) set @attribution=@attribution + ' | ' + @singlevalue;
		if ( @metadatatype = 32 ) set @user_description=@user_description + ' | ' + @singlevalue;
		if ( @metadatatype = 33 ) set @temporal_decade=@temporal_decade + ' | ' + @singlevalue;
		if ( @metadatatype = 34 ) set @mime_type=@mime_type + ' | ' + @singlevalue;
		if ( @metadatatype = 36 ) set @tracking_box=@tracking_box + ' | ' + @singlevalue;
		if ( @metadatatype = 37 ) set @abstract=@abstract + ' | ' + @singlevalue;
		if ( @metadatatype = 38 ) set @edition=@edition + ' | ' + @singlevalue;
		if ( @metadatatype = 39 ) set @toc=@toc + ' | ' + @singlevalue;
		if ( @metadatatype = 40 ) set @zt_kingdom=@zt_kingdom + ' | ' + @singlevalue;
		if ( @metadatatype = 41 ) set @zt_phylum=@zt_phylum + ' | ' + @singlevalue;
		if ( @metadatatype = 42 ) set @zt_class=@zt_class + ' | ' + @singlevalue;
		if ( @metadatatype = 43 ) set @zt_order=@zt_order + ' | ' + @singlevalue;
		if ( @metadatatype = 44 ) set @zt_family=@zt_family + ' | ' + @singlevalue;
		if ( @metadatatype = 45 ) set @zt_genus=@zt_genus + ' | ' + @singlevalue;
		if ( @metadatatype = 46 ) set @zt_species=@zt_species + ' | ' + @singlevalue;
		if ( @metadatatype = 47 ) set @zt_common_name=@zt_common_name + ' | ' + @singlevalue;
		if ( @metadatatype = 48 ) set @zt_scientific_name=@zt_scientific_name + ' | ' + @singlevalue;
		if ( @metadatatype = 49 ) set @zt_all_taxonomy=@zt_all_taxonomy + ' | ' + @singlevalue;
		if ( @metadatatype = 50 ) set @cultural_context=@cultural_context + ' | ' + @singlevalue;
		if ( @metadatatype = 51 ) set @inscription=@inscription + ' | ' + @singlevalue;
		if ( @metadatatype = 52 ) set @material=@material + ' | ' + @singlevalue;
		if ( @metadatatype = 53 ) set @style_period=@style_period + ' | ' + @singlevalue;
		if ( @metadatatype = 54 ) set @technique=@technique + ' | ' + @singlevalue;
		if ( @metadatatype = 55 ) set @accession_number=@accession_number + ' | ' + @singlevalue;
		if ( @metadatatype = 62 ) set @interviewee = @interviewee + ' | ' + @singlevalue;
		if ( @metadatatype = 63 ) set @interviewer = @interviewer + ' | ' + @singlevalue;
		if ( @metadatatype = 61 ) set @temporal_year = @temporal_year + ' | ' + @singlevalue;
		if ( @metadatatype = 56 ) set @etd_committee = @etd_committee + ' | ' + @singlevalue;
		if ( @metadatatype = 57 ) set @etd_degree = @etd_degree + ' | ' + @singlevalue;
		if ( @metadatatype = 58 ) set @etd_degree_discipline = @etd_degree_discipline + ' | ' + @singlevalue;
		if ( @metadatatype = 59 ) set @etd_degree_grantor = @etd_degree_grantor + ' | ' + @singlevalue;
		if ( @metadatatype = 60 ) set @etd_degree_level = @etd_degree_level + ' | ' + @singlevalue;
		if ( @metadatatype = 64 ) set @userdefined01 = @userdefined01 + ' | ' + @singlevalue;
		if ( @metadatatype = 65 ) set @userdefined02 = @userdefined02 + ' | ' + @singlevalue;
		if ( @metadatatype = 66 ) set @userdefined03 = @userdefined03 + ' | ' + @singlevalue;
		if ( @metadatatype = 67 ) set @userdefined04 = @userdefined04 + ' | ' + @singlevalue;
		if ( @metadatatype = 68 ) set @userdefined05 = @userdefined05 + ' | ' + @singlevalue;
		if ( @metadatatype = 69 ) set @userdefined06 = @userdefined06 + ' | ' + @singlevalue;
		if ( @metadatatype = 70 ) set @userdefined07 = @userdefined07 + ' | ' + @singlevalue;
		if ( @metadatatype = 71 ) set @userdefined08 = @userdefined08 + ' | ' + @singlevalue;
		if ( @metadatatype = 72 ) set @userdefined09 = @userdefined09 + ' | ' + @singlevalue;
		if ( @metadatatype = 73 ) set @userdefined10 = @userdefined10 + ' | ' + @singlevalue;
		if ( @metadatatype = 74 ) set @userdefined11 = @userdefined11 + ' | ' + @singlevalue;
		if ( @metadatatype = 75 ) set @userdefined12 = @userdefined12 + ' | ' + @singlevalue;
		if ( @metadatatype = 76 ) set @userdefined13 = @userdefined13 + ' | ' + @singlevalue;
		if ( @metadatatype = 77 ) set @userdefined14 = @userdefined14 + ' | ' + @singlevalue;
		if ( @metadatatype = 78 ) set @userdefined15 = @userdefined15 + ' | ' + @singlevalue;
		if ( @metadatatype = 79 ) set @userdefined16 = @userdefined16 + ' | ' + @singlevalue;
		if ( @metadatatype = 80 ) set @userdefined17 = @userdefined17 + ' | ' + @singlevalue;
		if ( @metadatatype = 81 ) set @userdefined18 = @userdefined18 + ' | ' + @singlevalue;
		if ( @metadatatype = 82 ) set @userdefined19 = @userdefined19 + ' | ' + @singlevalue;
		if ( @metadatatype = 83 ) set @userdefined20 = @userdefined20 + ' | ' + @singlevalue;
		if ( @metadatatype = 84 ) set @userdefined21 = @userdefined21 + ' | ' + @singlevalue;
		if ( @metadatatype = 85 ) set @userdefined22 = @userdefined22 + ' | ' + @singlevalue;
		if ( @metadatatype = 86 ) set @userdefined23 = @userdefined23 + ' | ' + @singlevalue;
		if ( @metadatatype = 87 ) set @userdefined24 = @userdefined24 + ' | ' + @singlevalue;
		if ( @metadatatype = 88 ) set @userdefined25 = @userdefined25 + ' | ' + @singlevalue;
		if ( @metadatatype = 89 ) set @userdefined26 = @userdefined26 + ' | ' + @singlevalue;
		if ( @metadatatype = 90 ) set @userdefined27 = @userdefined27 + ' | ' + @singlevalue;
		if ( @metadatatype = 91 ) set @userdefined28 = @userdefined28 + ' | ' + @singlevalue;
		if ( @metadatatype = 92 ) set @userdefined29 = @userdefined29 + ' | ' + @singlevalue;
		if ( @metadatatype = 93 ) set @userdefined30 = @userdefined30 + ' | ' + @singlevalue;
		if ( @metadatatype = 94 ) set @userdefined31 = @userdefined31 + ' | ' + @singlevalue;
		if ( @metadatatype = 95 ) set @userdefined32 = @userdefined32 + ' | ' + @singlevalue;
		if ( @metadatatype = 96 ) set @userdefined33 = @userdefined33 + ' | ' + @singlevalue;
		if ( @metadatatype = 97 ) set @userdefined34 = @userdefined34 + ' | ' + @singlevalue;
		if ( @metadatatype = 98 ) set @userdefined35 = @userdefined35 + ' | ' + @singlevalue;
		if ( @metadatatype = 99 ) set @userdefined36 = @userdefined36 + ' | ' + @singlevalue;
		if ( @metadatatype = 100 ) set @userdefined37 = @userdefined37 + ' | ' + @singlevalue;
		if ( @metadatatype = 101 ) set @userdefined38 = @userdefined38 + ' | ' + @singlevalue;
		if ( @metadatatype = 102 ) set @userdefined39 = @userdefined39 + ' | ' + @singlevalue;
		if ( @metadatatype = 103 ) set @userdefined40 = @userdefined40 + ' | ' + @singlevalue;
		if ( @metadatatype = 104 ) set @userdefined41 = @userdefined41 + ' | ' + @singlevalue;
		if ( @metadatatype = 105 ) set @userdefined42 = @userdefined42 + ' | ' + @singlevalue;
		if ( @metadatatype = 106 ) set @userdefined43 = @userdefined43 + ' | ' + @singlevalue;
		if ( @metadatatype = 107 ) set @userdefined44 = @userdefined44 + ' | ' + @singlevalue;
		if ( @metadatatype = 108 ) set @userdefined45 = @userdefined45 + ' | ' + @singlevalue;
		if ( @metadatatype = 109 ) set @userdefined46 = @userdefined46 + ' | ' + @singlevalue;
		if ( @metadatatype = 110 ) set @userdefined47 = @userdefined47 + ' | ' + @singlevalue;
		if ( @metadatatype = 111 ) set @userdefined48 = @userdefined48 + ' | ' + @singlevalue;
		if ( @metadatatype = 112 ) set @userdefined49 = @userdefined49 + ' | ' + @singlevalue;
		if ( @metadatatype = 113 ) set @userdefined50 = @userdefined50 + ' | ' + @singlevalue;
		if ( @metadatatype = 114 ) set @userdefined51 = @userdefined51 + ' | ' + @singlevalue;
		if ( @metadatatype = 115 ) set @userdefined52 = @userdefined52 + ' | ' + @singlevalue;
		
		-- Get the next value
		fetch next from metadatacursor into @singlevalue, @metadatatype;

	end;

	-- Close and deallocate the cursor which was used
	close metadatacursor;
	deallocate metadatacursor;

	-- Insert the newly created full citation for this item
	insert into SobekCM_Metadata_Basic_Search_Table ( ItemID, FullCitation, Title, [Type], [Language], Creator, Publisher, Publication_Place, Subject_Keyword, Genre, Target_Audience, Spatial_Coverage, Country, [State], County, City, Source_Institution, Holding_Location, Notes, Other_Citation, Tickler, Donor, Format, BibID, Publication_Date, Affiliation, Frequency, Name_as_Subject, Title_as_Subject, All_Subjects, Temporal_Subject, Attribution, User_Description, Temporal_Decade, MIME_Type, Tracking_Box, Abstract, Edition, TOC, ZT_Kingdom, ZT_Phylum, ZT_Class, ZT_Order, ZT_Family, ZT_Genus, ZT_Species, ZT_Common_Name, ZT_Scientific_Name, ZT_All_Taxonomy, Cultural_Context, Inscription, Material, Style_Period, Technique, Accession_Number, Interviewee, Interviewer, Temporal_Year, ETD_Committee, ETD_Degree, ETD_Degree_Discipline, ETD_Degree_Grantor, ETD_Degree_Level, UserDefined01, UserDefined02, UserDefined03, UserDefined04, UserDefined05, UserDefined06, UserDefined07, UserDefined08, UserDefined09, UserDefined10, UserDefined11, UserDefined12, UserDefined13, UserDefined14, UserDefined15, UserDefined16, UserDefined17, UserDefined18, UserDefined19, UserDefined20, UserDefined21, UserDefined22, UserDefined23, UserDefined24, UserDefined25, UserDefined26, UserDefined27, UserDefined28, UserDefined29, UserDefined30, UserDefined31, UserDefined32, UserDefined33, UserDefined34, UserDefined35, UserDefined36, UserDefined37, UserDefined38, UserDefined39, UserDefined40, UserDefined41, UserDefined42, UserDefined43, UserDefined44, UserDefined45, UserDefined46, UserDefined47, UserDefined48, UserDefined49, UserDefined50, UserDefined51, UserDefined52 )
	values ( @itemid, @fullcitation + ' | ', @title , @type , @language , @creator , @publisher , @publication_place , @subject_keyword , @genre , @target_audience , @spatial_coverage , @country , @state , @county , @city , @source_institution , @holding_location , @notes , @other_citation , @tickler , @donor , @format , @bibid , @publication_date , @affiliation , @frequency , @name_as_subject , @title_as_subject , @all_subjects , @temporal_subject , @attribution , @user_description , @temporal_decade , @mime_type , @tracking_box , @abstract , @edition , @toc , @zt_kingdom , @zt_phylum , @zt_class , @zt_order , @zt_family , @zt_genus , @zt_species , @zt_common_name , @zt_scientific_name , @zt_all_taxonomy , @cultural_context , @inscription , @material , @style_period , @technique , @accession_number, @interviewee, @interviewer, @temporal_year, @etd_committee, @etd_degree, @etd_degree_discipline, @etd_degree_grantor, @etd_degree_level, @userdefined01, @userdefined02, @userdefined03, @userdefined04, @userdefined05, @userdefined06, @userdefined07, @userdefined08, @userdefined09, @userdefined10, @userdefined11, @userdefined12, @userdefined13, @userdefined14, @userdefined15, @userdefined16, @userdefined17, @userdefined18, @userdefined19, @userdefined20, @userdefined21, @userdefined22, @userdefined23, @userdefined24, @userdefined25, @userdefined26, @userdefined27, @userdefined28, @userdefined29, @userdefined30, @userdefined31, @userdefined32, @userdefined33, @userdefined34, @userdefined35, @userdefined36, @userdefined37, @userdefined38, @userdefined39, @userdefined40, @userdefined41, @userdefined42, @userdefined43, @userdefined44, @userdefined45, @userdefined46, @userdefined47, @userdefined48, @userdefined49, @userdefined50, @userdefined51, @userdefined52  );

	select @etd_degree_discipline as Discipline;
	
end;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_By_UserName_Password]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets all the user information by the username and hashed password
CREATE PROCEDURE [dbo].[mySobek_Get_User_By_UserName_Password]
	@username varchar(100),
	@password varchar(100)
AS
BEGIN

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Look for the current user by username and hashed password.  Does one exist?
	if (( select COUNT(*) from mySobek_User where UserName=@username and [Password]=@password and isActive = 'true' ) = 1 )
	begin
		-- Get the userid for this user
		declare @userid int;
		select @userid = UserID from mySobek_User where UserName=@username and [Password]=@password and isActive = 'true';
		
		-- Stored procedure used to return standard data across all user fetch stored procedures
		exec mySobek_Get_User_By_UserID @userid;
	end  -- Look for current user by email and hashed password...
	else if (( select COUNT(*) from mySobek_User where EmailAddress=@username and [Password]=@password and isActive = 'true' ) = 1 )
	begin
		-- Get the userid for this user by email and hashed password
		declare @userid2 int;
		select @userid2 = UserID from mySobek_User where EmailAddress=@username and [Password]=@password and isActive = 'true';
		
		-- Stored procedure used to return standard data across all user fetch stored procedures
		exec mySobek_Get_User_By_UserID @userid2;
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[mySobek_Get_User_By_UFID]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[mySobek_Get_User_By_UFID]
	@ufid char(8)
AS
BEGIN		

	-- No need to perform any locks here.  A slightly dirty read won't hurt much
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Look for the user by UFID (Shibboleth ID).  Does one exist?
	if (( select COUNT(*) from mySobek_User where UFID=@ufid and isActive = 'true' ) = 1 )
	begin
		-- Get the userid for this user
		declare @userid int;
		select @userid = UserID from mySobek_User where UFID=@ufid and isActive = 'true';		
		
		-- Stored procedure used to return standard data across all user fetch stored procedures
		exec mySobek_Get_User_By_UserID @userid;	
	end;
END;
GO
/****** Object:  StoredProcedure [dbo].[Edit_OCR_Progress]    Script Date: 08/18/2013 13:31:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Adds an OCR Workflow progress for a specific volume.
-- Created:		04/08/09
-- Project:		OCR Automation Project-
-- Developer:	Tom Bielicke
CREATE PROCEDURE [dbo].[Edit_OCR_Progress]
	@BibID		varchar(10),
	@VIDNumber	varchar (5)	
AS
begin

	exec Tracking_Add_Workflow_Once_Per_Day @bibid, @VIDNumber, '', '', 6, '';
	
end;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_New_Item]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves all the main data for a new item in a SobekCM library, 
-- including the serial hierarchy, behaviors, tracking, and basic item information
-- Written by Mark Sullivan ( January 2011 )
CREATE PROCEDURE [dbo].[SobekCM_Save_New_Item]
	@GroupID int,
	@VID varchar(5),
	@PageCount int,
	@FileCount int,
	@Title nvarchar(500),
	@SortTitle nvarchar(500), --NEW
	@AccessMethod int,
	@Link varchar(500),
	@CreateDate datetime,
	@PubDate nvarchar(100),
	@SortDate bigint,
	@Author nvarchar(1000),
	@Spatial_KML varchar(4000),
	@Spatial_KML_Distance float,
	@DiskSize_MB bigint,
	@Spatial_Display nvarchar(1000), 
	@Institution_Display nvarchar(1000), 
	@Edition_Display nvarchar(1000),
	@Material_Display nvarchar(1000),
	@Measurement_Display nvarchar(1000), 
	@StylePeriod_Display nvarchar(1000), 
	@Technique_Display nvarchar(1000), 
	@Subjects_Display nvarchar(1000), 
	@Donor nvarchar(250),
	@Publisher nvarchar(1000),
	@TextSearchable bit,
	@MainThumbnail varchar(100),
	@MainJPEG varchar(100),
	@IP_Restriction_Mask smallint,
	@CheckoutRequired bit,
	@AggregationCode1 varchar(20),
	@AggregationCode2 varchar(20),
	@AggregationCode3 varchar(20),
	@AggregationCode4 varchar(20),
	@AggregationCode5 varchar(20),
	@AggregationCode6 varchar(20),
	@AggregationCode7 varchar(20),
	@AggregationCode8 varchar(20),
	@HoldingCode varchar(20),
	@SourceCode varchar(20),
	@Icon1_Name varchar(50),
	@Icon2_Name varchar(50),
	@Icon3_Name varchar(50),
	@Icon4_Name varchar(50),
	@Icon5_Name varchar(50),
	@Viewer1_TypeID int,
	@Viewer1_Label nvarchar(50),
	@Viewer1_Attribute nvarchar(250),
	@Viewer2_TypeID int,
	@Viewer2_Label nvarchar(50),
	@Viewer2_Attribute nvarchar(250),
	@Viewer3_TypeID int,
	@Viewer3_Label nvarchar(50),
	@Viewer3_Attribute nvarchar(250),
	@Viewer4_TypeID int,
	@Viewer4_Label nvarchar(50),
	@Viewer4_Attribute nvarchar(250),
	@Viewer5_TypeID int,
	@Viewer5_Label nvarchar(50),
	@Viewer5_Attribute nvarchar(250),
	@Viewer6_TypeID int,
	@Viewer6_Label nvarchar(50),
	@Viewer6_Attribute nvarchar(250),
	@Level1_Text varchar(255),
	@Level1_Index int,
	@Level2_Text varchar(255),
	@Level2_Index int,
	@Level3_Text varchar(255),
	@Level3_Index int,
	@Level4_Text varchar(255),
	@Level4_Index int,
	@Level5_Text varchar(255),
	@Level5_Index int,
	@VIDSource varchar(150),
	@CopyrightIndicator smallint, 
	@Born_Digital bit,
	@Dark bit,
	@Material_Received_Date datetime,
	@Material_Recd_Date_Estimated bit,
	@Disposition_Advice int,
	@Disposition_Advice_Notes varchar(150),
	@Internal_Comments nvarchar(1000),
	@Tracking_Box varchar(25),
	@Online_Submit bit,
	@User varchar(50),
	@UserNotes varchar(1000),
	@UserID_To_Link int,
	@ItemID int output,
	@New_VID varchar(5) output
AS
begin transaction

	-- Set the return VID value and itemid first
	set @New_VID = @VID;
	set @ItemID = -1;

	-- Verify this is a new item before doing anything
	if ( (	 select count(*) from SobekCM_Item I where ( I.VID = @VID ) and ( I.GroupID = @GroupID ))  =  0 )
	begin
	
		-- Verify the VID is a complete bibid, otherwise find the next one
		if ( LEN(@VID) < 5 )
		begin
			declare @next_vid_number int;

			-- Find the next vid number
			select @next_vid_number = isnull(CAST(MAX(VID) as int) + 1,-1)
			from SobekCM_Item
			where GroupID = @GroupID;
			
			-- If no matches to this BibID, just start at 00001
			if ( @next_vid_number < 0 )
			begin
				select @New_VID = '00001'
			end
			else
			begin
				select @New_VID = RIGHT('0000' + (CAST( @next_vid_number as varchar(5))), 5);	
			end;	
		end;

		-- Add the values to the main SobekCM_Item table first
		insert into SobekCM_Item ( VID, [PageCount], FileCount, Deleted, Title, SortTitle, AccessMethod, Link, CreateDate, PubDate, SortDate, Author, Spatial_KML, Spatial_KML_Distance, GroupID, LastSaved, Donor, Publisher, TextSearchable, MainThumbnail, MainJPEG, CheckoutRequired, IP_Restriction_Mask, Level1_Text, Level1_Index, Level2_Text, Level2_Index, Level3_Text, Level3_Index, Level4_Text, Level4_Index, Level5_Text, Level5_Index, Last_MileStone, VIDSource, Born_Digital, Dark, Material_Received_Date, Material_Recd_Date_Estimated, Disposition_Advice, Internal_Comments, Tracking_Box, Disposition_Advice_Notes, Spatial_Display, Institution_Display, Edition_Display, Material_Display, Measurement_Display, StylePeriod_Display, Technique_Display, Subjects_Display )
		values (  @New_VID, @PageCount, @FileCount, 0, @Title, @SortTitle, @AccessMethod, @Link, @CreateDate, @PubDate, @SortDate, @Author, @Spatial_KML, @Spatial_KML_Distance, @GroupID, GETDATE(), @Donor, @Publisher, @TextSearchable, @MainThumbnail, @MainJPEG, @CheckoutRequired, @IP_Restriction_Mask, @Level1_Text, @Level1_Index, @Level2_Text, @Level2_Index, @Level3_Text, @Level3_Index, @Level4_Text, @Level4_Index, @Level5_Text, @Level5_Index, 0, @VIDSource, @Born_Digital, @Dark, @Material_Received_Date, @Material_Recd_Date_Estimated, @Disposition_Advice, @Internal_Comments, @Tracking_Box, @Disposition_Advice_Notes, @Spatial_Display, @Institution_Display, @Edition_Display, @Material_Display, @Measurement_Display, @StylePeriod_Display, @Technique_Display, @Subjects_Display  );
		
		-- Get the item id identifier for this row
		set @ItemID = @@identity;	
		
		-- Set the milestones to complete if this is NON-PRIVATE, NON-DARK, and BORN DIGITAL
		if (( @IP_Restriction_Mask >= 0 ) and ( @Dark = 'false' ) and ( @Born_Digital = 'true' ))
		begin
			update SobekCM_Item
			set Last_MileStone = 4, Milestone_DigitalAcquisition = CreateDate, Milestone_ImageProcessing=CreateDate, Milestone_QualityControl=CreateDate, Milestone_OnlineComplete=CreateDate 
			where ItemID=@ItemID;		
		end;
				
		-- If a size was included, set that value
		if ( @DiskSize_MB > 0 )
		begin
			update SobekCM_Item set DiskSize_MB = @DiskSize_MB where ItemID=@ItemID;
		end;

		-- Finally set the volume count for this group correctly
		update SobekCM_Item_Group
		set ItemCount = ( select count(*) from SobekCM_Item I where ( I.GroupID = @GroupID ) and ( I.Deleted = 'false' ))
		where GroupID = @GroupID;
		
		-- Add the first icon to this object  (this requires the icons have been pre-established )
		declare @IconID int;
		if ( len( isnull( @Icon1_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon1_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 1 );
			end;
		end;

		-- Add the second icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon2_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon2_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 2 );
			end;
		end;

		-- Add the third icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon3_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon3_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 3 );
			end;
		end;

		-- Add the fourth icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon4_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon4_Name;
			
			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 4 );
			end;
		end;

		-- Add the fifth icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon5_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon5_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				values ( @ItemID, @IconID, 5 );
			end;
		end;

		-- Clear all links to aggregations
		delete from SobekCM_Item_Aggregation_Item_Link where ItemID = @ItemID;

		-- Add all of the aggregations
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode1;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode2;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode3;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode4;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode5;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode6;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode7;
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode8;
		
		-- Create one string of all the aggregation codes
		declare @aggregationCodes varchar(100);
		set @aggregationCodes = rtrim(isnull(@AggregationCode1,'') + ' ' + isnull(@AggregationCode2,'') + ' ' + isnull(@AggregationCode3,'') + ' ' + isnull(@AggregationCode4,'') + ' ' + isnull(@AggregationCode5,'') + ' ' + isnull(@AggregationCode6,'') + ' ' + isnull(@AggregationCode7,'') + ' ' + isnull(@AggregationCode8,''));
	
		-- Update matching items to have the aggregation codes value
		update SobekCM_Item set AggregationCodes = @aggregationCodes where ItemID=@ItemID;

		-- Check for Holding Institution Code
		declare @AggregationID int;
		if ( len ( isnull ( @HoldingCode, '' ) ) > 0 )
		begin
			-- Does this institution already exist?
			if (( select count(*) from SobekCM_Item_Aggregation where Code = @HoldingCode ) = 0 )
			begin
				-- Add new institution
				insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
				values ( @HoldingCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
			end;
			
			-- Add the link to this holding code ( and any legitimate parent aggregations )
			exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @HoldingCode;
		end;

		-- Check for Source Institution Code
		if ( len ( isnull ( @SourceCode, '' ) ) > 0 )
		begin
			-- Does this institution already exist?
			if (( select count(*) from SobekCM_Item_Aggregation where Code = @SourceCode ) = 0 )
			begin
				-- Add new institution
				insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
				values ( @SourceCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
			end;

			-- Add the link to this holding code ( and any legitimate parent aggregations )
			exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @SourceCode;
		end;
		
		-- Clear the links to all existing viewers
		delete from SobekCM_Item_Viewers where ItemID=@ItemID;
		
		-- Add the first viewer information
		if ( @Viewer1_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer1_TypeID, @Viewer1_Attribute, @Viewer1_Label );
		end;
		
		-- Add the second viewer information
		if ( @Viewer2_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer2_TypeID, @Viewer2_Attribute, @Viewer2_Label );
		end;
		
		-- Add the third viewer information
		if ( @Viewer3_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer3_TypeID, @Viewer3_Attribute, @Viewer3_Label );
		end;
		
		-- Add the fourth viewer information
		if ( @Viewer4_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer4_TypeID, @Viewer4_Attribute, @Viewer4_Label );
		end;
		
		-- Add the fifth viewer information
		if ( @Viewer5_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer5_TypeID, @Viewer5_Attribute, @Viewer5_Label );
		end;
		
		-- Add the first viewer information
		if ( @Viewer6_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			values ( @ItemID, @Viewer6_TypeID, @Viewer6_Attribute, @Viewer6_Label );
		end;

		-- Add the workhistory for this item being loaded
		if ( @Online_Submit = 'true' )
		begin
			-- Add progress for online submission completed
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
			values ( @itemid, 29, getdate(), @user, @usernotes, '' );
		end
		else
		begin  
			-- Add progress for bulk loaded into the system through the Builder
			insert into Tracking_Progress ( ItemID, WorkFlowID, DateCompleted, WorkPerformedBy, ProgressNote, WorkingFilePath )
			values ( @itemid, 40, getdate(), @user, @usernotes, '' );	
		end;		
		
		-- Link this to the user?
		if ( @UserID_To_Link >= 1 )
		begin
			-- Link this user to the bibid, if not already linked
			if (( select COUNT(*) from mySobek_User_Bib_Link where UserID=@UserID_To_Link and GroupID = @groupid ) = 0 )
			begin
				insert into mySobek_User_Bib_Link ( UserID, GroupID )
				values ( @UserID_To_Link, @groupid );
			end;
			
			-- First, see if this user already has a folder named 'Submitted Items'
			declare @userfolderid int
			if (( select count(*) from mySobek_User_Folder where UserID=@UserID_To_Link and FolderName='Submitted Items') > 0 )
			begin
				-- Get the existing folder id
				select @userfolderid = UserFolderID from mySobek_User_Folder where UserID=@UserID_To_Link and FolderName='Submitted Items';
			end
			else
			begin
				-- Add this folder
				insert into mySobek_User_Folder ( UserID, FolderName, isPublic )
				values ( @UserID_To_Link, 'Submitted Items', 'false' );

				-- Get the new id
				select @userfolderid = @@identity;
			end;
			
			-- Add a new link then
			insert into mySobek_User_Item( UserFolderID, ItemID, ItemOrder, UserNotes, DateAdded )
			values ( @userfolderid, @itemid, 1, '', getdate() );
			
			-- Also link using the newer system, which links for statistical reporting, etc..
			-- This will likely replace the 'submitted items' folder technique from above
			insert into mySobek_User_Item_Link( UserID, ItemID, RelationshipID )
			values ( @UserID_To_Link, @ItemID, 1 );
		
		end;
	end;

commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item_Behaviors]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves the behavior information about an item in this library
-- Written by Mark Sullivan 
CREATE PROCEDURE [dbo].[SobekCM_Save_Item_Behaviors]
	@ItemID int,
	@TextSearchable bit,
	@MainThumbnail varchar(100),
	@MainJPEG varchar(100),
	@IP_Restriction_Mask smallint,
	@CheckoutRequired bit,
	@Dark_Flag bit,
	@Born_Digital bit,
	@Disposition_Advice int,
	@Disposition_Advice_Notes varchar(150),
	@Material_Received_Date datetime,
	@Material_Recd_Date_Estimated bit,
	@Tracking_Box varchar(25),
	@AggregationCode1 varchar(20),
	@AggregationCode2 varchar(20),
	@AggregationCode3 varchar(20),
	@AggregationCode4 varchar(20),
	@AggregationCode5 varchar(20),
	@AggregationCode6 varchar(20),
	@AggregationCode7 varchar(20),
	@AggregationCode8 varchar(20),
	@HoldingCode varchar(20),
	@SourceCode varchar(20),
	@Icon1_Name varchar(50),
	@Icon2_Name varchar(50),
	@Icon3_Name varchar(50),
	@Icon4_Name varchar(50),
	@Icon5_Name varchar(50),
	@Viewer1_TypeID int,
	@Viewer1_Label nvarchar(50),
	@Viewer1_Attribute nvarchar(250),
	@Viewer2_TypeID int,
	@Viewer2_Label nvarchar(50),
	@Viewer2_Attribute nvarchar(250),
	@Viewer3_TypeID int,
	@Viewer3_Label nvarchar(50),
	@Viewer3_Attribute nvarchar(250),
	@Viewer4_TypeID int,
	@Viewer4_Label nvarchar(50),
	@Viewer4_Attribute nvarchar(250),
	@Viewer5_TypeID int,
	@Viewer5_Label nvarchar(50),
	@Viewer5_Attribute nvarchar(250),
	@Viewer6_TypeID int,
	@Viewer6_Label nvarchar(50),
	@Viewer6_Attribute nvarchar(250),
	@Left_To_Right bit
AS
begin transaction

	--Update the main item
	update SobekCM_Item
	set TextSearchable = @TextSearchable, Deleted = 0, MainThumbnail=@MainThumbnail,
		MainJPEG=@MainJPEG, CheckoutRequired=@CheckoutRequired, IP_Restriction_Mask=@IP_Restriction_Mask,
		Dark=@Dark_Flag, Born_Digital=@Born_Digital, Disposition_Advice=@Disposition_Advice,
		Material_Received_Date=@Material_Received_Date, Material_Recd_Date_Estimated=@Material_Recd_Date_Estimated,
		Tracking_Box=@Tracking_Box, Disposition_Advice_Notes = @Disposition_Advice_Notes, Left_To_Right=@Left_To_Right
	where ( ItemID = @ItemID )

	-- Clear the links to all existing icons
	delete from SobekCM_Item_Icons where ItemID=@ItemID
	
	-- Add the first icon to this object  (this requires the icons have been pre-established )
	declare @IconID int
	if ( len( isnull( @Icon1_Name, '' )) > 0 ) 
	begin
		-- Get the Icon ID for this icon
		select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon1_Name

		-- Tie this item to this icon
		if ( ISNULL(@IconID,-1) > 0 )
		begin
			insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
			values ( @ItemID, @IconID, 1 )
		end
	end

	-- Add the second icon to this object  (this requires the icons have been pre-established )
	if ( len( isnull( @Icon2_Name, '' )) > 0 ) 
	begin
		-- Get the Icon ID for this icon
		select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon2_Name

		-- Tie this item to this icon
		if ( ISNULL(@IconID,-1) > 0 )
		begin
			insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
			values ( @ItemID, @IconID, 2 )
		end
	end

	-- Add the third icon to this object  (this requires the icons have been pre-established )
	if ( len( isnull( @Icon3_Name, '' )) > 0 ) 
	begin
		-- Get the Icon ID for this icon
		select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon3_Name

		-- Tie this item to this icon
		if ( ISNULL(@IconID,-1) > 0 )
		begin
			insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
			values ( @ItemID, @IconID, 3 )
		end
	end

	-- Add the fourth icon to this object  (this requires the icons have been pre-established )
	if ( len( isnull( @Icon4_Name, '' )) > 0 ) 
	begin
		-- Get the Icon ID for this icon
		select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon4_Name
		
		-- Tie this item to this icon
		if ( ISNULL(@IconID,-1) > 0 )
		begin
			insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
			values ( @ItemID, @IconID, 4 )
		end
	end

	-- Add the fifth icon to this object  (this requires the icons have been pre-established )
	if ( len( isnull( @Icon5_Name, '' )) > 0 ) 
	begin
		-- Get the Icon ID for this icon
		select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon5_Name

		-- Tie this item to this icon
		if ( ISNULL(@IconID,-1) > 0 )
		begin
			insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
			values ( @ItemID, @IconID, 5 )
		end
	end

	-- Clear all links to aggregations
	delete from SobekCM_Item_Aggregation_Item_Link where ItemID = @ItemID

	-- Add all of the aggregations
	exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode1
	exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode2
	exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode3
	exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode4
	exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode5
	exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode6
	exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode7
	exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @AggregationCode8
	
	-- Create one string of all the aggregation codes
	declare @aggregationCodes varchar(100)
	set @aggregationCodes = rtrim(isnull(@AggregationCode1,'') + ' ' + isnull(@AggregationCode2,'') + ' ' + isnull(@AggregationCode3,'') + ' ' + isnull(@AggregationCode4,'') + ' ' + isnull(@AggregationCode5,'') + ' ' + isnull(@AggregationCode6,'') + ' ' + isnull(@AggregationCode7,'') + ' ' + isnull(@AggregationCode8,''))
	
	-- Update matching items to have the aggregation codes value
	update SobekCM_Item set AggregationCodes = @aggregationCodes where ItemID=@ItemID

	-- Check for Holding Institution Code
	declare @AggregationID int
	if ( len ( isnull ( @HoldingCode, '' ) ) > 0 )
	begin
		-- Does this institution already exist?
		if (( select count(*) from SobekCM_Item_Aggregation where Code = @HoldingCode ) = 0 )
		begin
			-- Add new institution
			insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
			values ( @HoldingCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' )
		end
		
		-- Add the link to this holding code ( and any legitimate parent aggregations )
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @HoldingCode		
	end

	-- Check for Source Institution Code
	if ( len ( isnull ( @SourceCode, '' ) ) > 0 )
	begin
		-- Does this institution already exist?
		if (( select count(*) from SobekCM_Item_Aggregation where Code = @SourceCode ) = 0 )
		begin
			-- Add new institution
			insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
			values ( @SourceCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' )
		end

		-- Add the link to this holding code ( and any legitimate parent aggregations )
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @SourceCode	
	end	
	
	-- Clear the links to all existing viewers
	delete from SobekCM_Item_Viewers where ItemID=@ItemID
	
	-- Add the first viewer information
	if ( @Viewer1_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer1_TypeID, @Viewer1_Attribute, @Viewer1_Label )
	end
	
	-- Add the second viewer information
	if ( @Viewer2_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer2_TypeID, @Viewer2_Attribute, @Viewer2_Label )
	end
	
	-- Add the third viewer information
	if ( @Viewer3_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer3_TypeID, @Viewer3_Attribute, @Viewer3_Label )
	end
	
	-- Add the fourth viewer information
	if ( @Viewer4_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer4_TypeID, @Viewer4_Attribute, @Viewer4_Label )
	end
	
	-- Add the fifth viewer information
	if ( @Viewer5_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer5_TypeID, @Viewer5_Attribute, @Viewer5_Label )
	end
	
	-- Add the first viewer information
	if ( @Viewer6_TypeID > 0 )
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer6_TypeID, @Viewer6_Attribute, @Viewer6_Label )
	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Save_Item]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Saves all the main data about an item in UFDC (but not behaviors)
-- Written by Mark Sullivan ( September 2005, Edited Decemver 2010 )
CREATE PROCEDURE [dbo].[SobekCM_Save_Item]
	@GroupID int,
	@VID varchar(5),
	@PageCount int,
	@FileCount int,
	@Title nvarchar(500),
	@SortTitle nvarchar(500), --NEW
	@AccessMethod int,
	@Link varchar(500),
	@CreateDate datetime,
	@PubDate nvarchar(100),
	@SortDate bigint,
	@HoldingCode varchar(20),
	@SourceCode varchar(20),
	@Author nvarchar(1000),
	@Spatial_KML varchar(4000),
	@Spatial_KML_Distance float,
	@DiskSize_MB bigint,
	@Spatial_Display nvarchar(1000), 
	@Institution_Display nvarchar(1000), 
	@Edition_Display nvarchar(1000),
	@Material_Display nvarchar(1000),
	@Measurement_Display nvarchar(1000), 
	@StylePeriod_Display nvarchar(1000), 
	@Technique_Display nvarchar(1000), 
	@Subjects_Display nvarchar(1000), 
	@Donor nvarchar(250),
	@Publisher nvarchar(1000),
	@ItemID int output,
	@Existing bit output,
	@New_VID varchar(5) output
AS
begin transaction

	-- Set the return VID value first
	set @New_VID = @VID;

	-- If this already exists (BibID, VID) then just update
	if ( (	 select count(*) from SobekCM_Item I where ( I.VID = @VID ) and ( I.GroupID = @GroupID ) )  > 0 )
	begin
		-- Save the item id
		select @ItemID = I.ItemID
		from SobekCM_Item I
		where  ( I.VID = @VID ) and ( I.GroupID = @GroupID );

		--Update the main item
		update SobekCM_Item
		set [PageCount] = @PageCount, 
			Deleted = 0, Title=@Title, SortTitle=@SortTitle, AccessMethod=@AccessMethod, Link=@Link,
			PubDate=@PubDate, SortDate=@SortDate, FileCount=@FileCount, Author=@Author, 
			Spatial_KML=@Spatial_KML, Spatial_KML_Distance=@Spatial_KML_Distance,  
			Donor=@Donor, Publisher=@Publisher, 
			GroupID = GroupID, LastSaved=GETDATE(), Spatial_Display=@Spatial_Display, Institution_Display=@Institution_Display, 
			Edition_Display=@Edition_Display, Material_Display=@Material_Display, Measurement_Display=@Measurement_Display, 
			StylePeriod_Display=@StylePeriod_Display, Technique_Display=@Technique_Display, Subjects_Display=@Subjects_Display 
		where ( ItemID = @ItemID );

		-- Set the existing flag to true (1)
		set @Existing = 1;
	end
	else
	begin
	
		-- Verify the VID is a complete bibid, otherwise find the next one
		if ( LEN(@VID) < 5 )
		begin
			declare @next_vid_number int;

			-- Find the next vid number
			select @next_vid_number = isnull(CAST(MAX(VID) as int) + 1,-1)
			from SobekCM_Item
			where GroupID = @GroupID;
			
			-- If no matches to this BibID, just start at 00001
			if ( @next_vid_number < 0 )
			begin
				select @New_VID = '00001';
			end
			else
			begin
				select @New_VID = RIGHT('0000' + (CAST( @next_vid_number as varchar(5))), 5);	
			end;	
		end;
		
		-- Add the values to the main SobekCM_Item table first
		insert into SobekCM_Item ( VID, [PageCount], FileCount, Deleted, Title, SortTitle, AccessMethod, Link, CreateDate, PubDate, SortDate, Author, Spatial_KML, Spatial_KML_Distance, GroupID, LastSaved, Donor, Publisher, Spatial_Display, Institution_Display, Edition_Display, Material_Display, Measurement_Display, StylePeriod_Display, Technique_Display, Subjects_Display )
		values (  @New_VID, @PageCount, @FileCount, 0, @Title, @SortTitle, @AccessMethod, @Link, @CreateDate, @PubDate, @SortDate, @Author, @Spatial_KML, @Spatial_KML_Distance, @GroupID, GETDATE(), @Donor, @Publisher, @Spatial_Display, @Institution_Display, @Edition_Display, @Material_Display, @Measurement_Display, @StylePeriod_Display, @Technique_Display, @Subjects_Display  );

		-- Get the item id identifier for this row
		set @ItemID = @@identity;

		-- Set existing flag to false
		set @Existing = 0;
		
		-- Add the default views
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, 1, '', '');
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, 2, '', '');
	end;

	-- Check for Holding Institution Code
	declare @AggregationID int;
	if ( len ( isnull ( @HoldingCode, '' ) ) > 0 )
	begin
		-- Does this institution already exist?
		if (( select count(*) from SobekCM_Item_Aggregation where Code = @HoldingCode ) = 0 )
		begin
			-- Add new institution
			insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
			values ( @HoldingCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
		end;
		
		-- Add the link to this holding code ( and any legitimate parent aggregations )
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @HoldingCode;		
	end;

	-- Check for Source Institution Code
	if ( len ( isnull ( @SourceCode, '' ) ) > 0 )
	begin
		-- Does this institution already exist?
		if (( select count(*) from SobekCM_Item_Aggregation where Code = @SourceCode ) = 0 )
		begin
			-- Add new institution
			insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
			values ( @SourceCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
		end;

		-- Add the link to this holding code ( and any legitimate parent aggregations )
		exec SobekCM_Save_Item_Item_Aggregation_Link @ItemID, @SourceCode;	
	end;
	
	-- If a size was included, set that value
	if ( @DiskSize_MB > 0 )
	begin
		update SobekCM_Item set DiskSize_MB = @DiskSize_MB where ItemID=@ItemID;
	end;

	-- Finally set the volume count for this group correctly
	update SobekCM_Item_Group
	set ItemCount = ( select count(*) from SobekCM_Item I where ( I.GroupID = @GroupID ) and ( I.Deleted = 'false' ))
	where GroupID = @GroupID;

commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Metadata_Search_Paged2]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Perform metadata search 
CREATE PROCEDURE [dbo].[SobekCM_Metadata_Search_Paged2]
	@term1 nvarchar(255),
	@field1 int,
	@link2 int,
	@term2 nvarchar(255),
	@field2 int,
	@link3 int,
	@term3 nvarchar(255),
	@field3 int,
	@link4 int,
	@term4 nvarchar(255),
	@field4 int,
	@link5 int,
	@term5 nvarchar(255),
	@field5 int,
	@link6 int,
	@term6 nvarchar(255),
	@field6 int,
	@link7 int,
	@term7 nvarchar(255),
	@field7 int,
	@link8 int,
	@term8 nvarchar(255),
	@field8 int,
	@link9 int,
	@term9 nvarchar(255),
	@field9 int,
	@link10 int,
	@term10 nvarchar(255),
	@field10 int,
	@include_private bit,
	@aggregationcode varchar(20),	
	@pagesize int, 
	@pagenumber int,
	@sort int,
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output,
	@all_collections_items int output,
	@all_collections_titles int output	
AS
BEGIN
	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
	
	-- Field#'s indicate which metadata field (if any).  These are numbers from the 
	-- SobekCM_Metadata_Types table.  A field# of -1, means all fields are included.
	
	-- Link#'s indicate if this is an AND-joiner ( intersect ) or an OR-joiner ( union )
	-- 0 = AND, 1 = OR, 2 = AND NOT
	
	-- Examples of using this procedure are:
	-- exec SobekCM_Metadata_Search 'haiti',1,0,'kesse',4,0,'',0
	-- This searches for materials which have haiti in the title AND kesse in the creator
	
	-- exec SobekCM_Metadata_Search 'haiti',1,1,'kesse',-1,0,'',0
	-- This searches for materials which have haiti in the title OR kesse anywhere
	
	-- Create the temporary table variables first
	-- Create the temporary table to hold all the item id's
	create table #TEMPZERO ( ItemID int primary key );
	create table #TEMP_ITEMS ( ItemID int primary key, fk_TitleID int, Hit_Count int, SortDate bigint );
		    
	-- declare both the sql query and the parameter definitions
	declare @SQLQuery AS nvarchar(max);
	declare @rankselection AS nvarchar(1000);
    declare @ParamDefinition AS NVarchar(2000);
    
    -- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@aggregationcode );
	
    -- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;
	    
    -- Start to build the main bulk of the query   
	set @SQLQuery = '';
    
    -- Was a field listed?
    if (( @field1 > 0 ) and ( @field1 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
    begin
		-- Get the name of this column then
		declare @field1_name varchar(100);
		set @field1_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field1 );

		-- Add this search then
		set @SQLQuery = @SQLQuery + ' contains ( ' + @field1_name + ', @innerterm1 )';
	end
	else
	begin
		-- Search the full citation then
		set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm1 )';	
	end;
            
    -- Start to build the query which will do ranking over the results which match this search
    set @rankselection = @term1;

	-- Add the second term, if there is one
	if (( LEN( ISNULL(@term2,'')) > 0 ) and (( @link2 = 0 ) or ( @link2 = 1 ) or ( @link2 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link2 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link2 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link2 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field2 > 0 ) and ( @field2 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field2_name varchar(100);
			set @field2_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field2 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field2_name + ', @innerterm2 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm2 )';	
		end;			
		
		-- Build the ranking query
		if ( @link2 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term2;	
		end
	end;    
	
	-- Add the third term, if there is one
	if (( LEN( ISNULL(@term3,'')) > 0 ) and (( @link3 = 0 ) or ( @link3 = 1 ) or ( @link3 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link3 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link3 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link3 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field3 > 0 ) and ( @field3 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field3_name varchar(100);
			set @field3_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field3 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field3_name + ', @innerterm3 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm3 )';	
		end;	
		
		-- Build the ranking query
		if ( @link3 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term3;		
		end
	end;   
	
	-- Add the fourth term, if there is one
	if (( LEN( ISNULL(@term4,'')) > 0 ) and (( @link4 = 0 ) or ( @link4 = 1 ) or ( @link4 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link4 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link4 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link4 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field4 > 0 ) and ( @field4 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field4_name varchar(100);
			set @field4_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field4 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field4_name + ', @innerterm4 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm4 )';	
		end;	
			
		-- Build the ranking query
		if ( @link4 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term4;		
		end
	end;
	
	-- Add the fifth term, if there is one
	if (( LEN( ISNULL(@term5,'')) > 0 ) and (( @link5 = 0 ) or ( @link5 = 1 ) or ( @link5 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link5 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link5 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link5 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field5 > 0 ) and ( @field5 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field5_name varchar(100);
			set @field5_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field5 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field5_name + ', @innerterm5 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm5 )';	
		end;
			
		-- Build the ranking query
		if ( @link5 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term5;		
		end
	end;
	
	-- Add the sixth term, if there is one
	if (( LEN( ISNULL(@term6,'')) > 0 ) and (( @link6 = 0 ) or ( @link6 = 1 ) or ( @link6 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link6 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link6 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link6 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field6 > 0 ) and ( @field6 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field6_name varchar(100);
			set @field6_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field6 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field6_name + ', @innerterm6 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm6 )';	
		end;
		
		-- Build the ranking query
		if ( @link6 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term6;		
		end
	end; 
	
	-- Add the seventh term, if there is one
	if (( LEN( ISNULL(@term7,'')) > 0 ) and (( @link7 = 0 ) or ( @link7 = 1 ) or ( @link7 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link7 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link7 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link7 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field7 > 0 ) and ( @field7 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field7_name varchar(100);
			set @field7_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field7 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field7_name + ', @innerterm7 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm7 )';	
		end;
		
		-- Build the ranking query
		if ( @link7 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term7;		
		end
	end;
	
	-- Add the eighth term, if there is one
	if (( LEN( ISNULL(@term8,'')) > 0 ) and (( @link8 = 0 ) or ( @link8 = 1 ) or ( @link8 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link8 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link8 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link8 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field8 > 0 ) and ( @field8 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field8_name varchar(100);
			set @field8_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field8 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field8_name + ', @innerterm8 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm8 )';	
		end;
		
		-- Build the ranking query
		if ( @link8 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term8;		
		end
	end;
	
	-- Add the ninth term, if there is one
	if (( LEN( ISNULL(@term9,'')) > 0 ) and (( @link9 = 0 ) or ( @link9 = 1 ) or ( @link9 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link9 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link9 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link9 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field9 > 0 ) and ( @field9 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field9_name varchar(100);
			set @field9_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field9 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field9_name + ', @innerterm9 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm9 )';	
		end;
		
		-- Build the ranking query
		if ( @link9 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term9;		
		end
	end;
	
	-- Add the tenth term, if there is one
	if (( LEN( ISNULL(@term10,'')) > 0 ) and (( @link10 = 0 ) or ( @link10 = 1 ) or ( @link10 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link10 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link10 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link10 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field10 > 0 ) and ( @field10 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field10_name varchar(100);
			set @field10_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field10 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field10_name + ', @innerterm10 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm10 )';	
		end;
		
		-- Build the ranking query
		if ( @link10 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term10;		
		end		
	end;
	
	-- Add the recompile option
	--set @SQLQuery = @SQLQuery + ' option (RECOMPILE)';

    -- Add the first term and start to build the query which will provide the items which match the search
    declare @mainquery nvarchar(max);
    set @mainquery = 'select L.Itemid from SobekCM_Metadata_Basic_Search_Table AS L';
    
    -- Do we need to limit by aggregation id as well?
    if ( @aggregationid > 0 )
    begin
		set @mainquery = @mainquery + ' join SobekCM_Item_Aggregation_Item_Link AS A ON ( A.ItemID = L.ItemID ) and ( A.AggregationID = ' + CAST( @aggregationid as varchar(5) ) + ')';   
    end    
    
    -- Add the full text search portion here
    set @mainquery = @mainquery + ' where ' + @SQLQuery;
	
	-- Set the parameter definition
	set @ParamDefinition = ' @innerterm1 nvarchar(255), @innerterm2 nvarchar(255), @innerterm3 nvarchar(255), @innerterm4 nvarchar(255), @innerterm5 nvarchar(255), @innerterm6 nvarchar(255), @innerterm7 nvarchar(255), @innerterm8 nvarchar(255), @innerterm9 nvarchar(255), @innerterm10 nvarchar(255)';
		
	-- Execute this stored procedure
	insert #TEMPZERO execute sp_Executesql @mainquery, @ParamDefinition, @term1, @term2, @term3, @term4, @term5, @term6, @term7, @term8, @term9, @term10;
			
	-- Perform ranking against the items and insert into another temporary table 
	-- with all the possible data elements needed for applying the user's sort
	insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate, Hit_Count )
	select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1), isnull(KEY_TBL.RANK, 0 )
	from SobekCM_Item I, #TEMPZERO AS T1 join
	  CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @rankselection ) AS KEY_TBL on KEY_TBL.[KEY] = T1.ItemID
	where ( T1.ItemID = I.ItemID )
	  and ( I.Deleted = 'false' )
      and ( I.IP_Restriction_Mask >= @lower_mask )	
      and ( I.IncludeInAll = 'true' );    

	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int;
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- If there were no results at all, check the count in the entire library
	if ( ( select COUNT(*) from #TEMP_ITEMS ) = 0 )
	begin
		-- Set the items and titles correctly
		set @total_items = 0;
		set @total_titles = 0;
		
		-- If there was an aggregation id, just return the counts for the whole library
		if ( @aggregationid > 0 )	
		begin
		
			-- Truncate the table and repull the data
			truncate table #TEMPZERO;
			
			-- Query against ALL aggregations this time
			declare @allquery nvarchar(max);
			set @allquery = 'select L.Itemid from SobekCM_Metadata_Basic_Search_Table AS L where ' + @SQLQuery;
			
			-- Execute this stored procedure
			insert #TEMPZERO execute sp_Executesql @allquery, @ParamDefinition, @term1, @term2, @term3, @term4, @term5, @term6, @term7, @term8, @term9, @term10;
			
			-- Get all items in the entire library then		  
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID )
			select I.ItemID, I.GroupID
			from #TEMPZERO T1, SobekCM_Item I
			where ( T1.ItemID = I.ItemID )
			  and ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask )	
			  and ( I.IncludeInAll = 'true' );  
			  
			-- Return these counts
			select @all_collections_items=COUNT(*), @all_collections_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
		end;
		
		-- Drop the big temporary table
		drop table #TEMPZERO;
	
	end
	else
	begin	
	
		-- Drop the big temporary table
		drop table #TEMPZERO;	
		  
		-- There are essentially two major paths of execution, depending on whether this should
		-- be grouped as items within the page requested titles ( sorting by title or the basic
		-- sorting by rank, which ranks this way ) or whether each item should be
		-- returned by itself, such as sorting by individual publication dates, etc..
		
		if ( @sort < 10 )
		begin	
			-- create the temporary title table definition
			declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int );	
			
			-- Return these counts
			select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
			
			-- Now, calculate the actual ending row, based on the ration, page information,
			-- and the lookahead factor
			if (( @total_items > 0 ) and ( @total_titles > 0 ))
			begin		
				-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
				declare @computed_value int;
				select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
				
				-- Compute the minimum value.  This cannot be less than @minpagelookahead.
				declare @floored_value int;
				select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
				
				-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
				declare @actual_pages int;
				select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead));

				-- Set the final row again then
				set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 
			end;	
					  
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
				(	select GroupID, G.BibID, 
						ROW_NUMBER() OVER (order by case when @sort=0 THEN (SUM(Hit_COunt)/COUNT(*)) end DESC,
													case when @sort=1 THEN G.SortTitle end ASC,												
													case when @sort=2 THEN BibID end ASC,
													case when @sort=3 THEN BibID end DESC) as RowNumber
					from #TEMP_ITEMS I, SobekCM_Item_Group G
					where I.fk_TitleID = G.GroupID
					group by G.GroupID, G.BibID, G.SortTitle )

			-- Insert the correct rows into the temp title table	
			insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
			select GroupID, BibID, RowNumber
			from TITLES_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
		
			-- Return the title information for this page
			select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_TITLES T, SobekCM_Item_Group G
			where ( T.TitleID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the item information for this page
			select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text,  isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, ISNULL(SortTitle, upper(title)) as SortTitle
			from #TEMP_ITEMS M, @TEMP_TITLES T, SobekCM_Item I
			where ( M.fk_TitleID = T.TitleID )
			  and ( I.ItemID = M.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		end
		else
		begin
			-- Create the temporary item table for paging purposes
			declare @TEMP_PAGED_ITEMS table ( ItemID int, RowNumber int );
			
			-- Since these sorts make each item paired with a single title row,
			-- number of items and titles are equal
			select @total_items=COUNT(*), @total_titles=COUNT(*)
			from #TEMP_ITEMS; 
			
			-- In addition, always return the max lookahead pages
			set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
			
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN isnull(SortDate,-1) end DESC) as RowNumber
					from #TEMP_ITEMS I
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			  
			-- Return the title information for this page
			select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
			where ( T.ItemID = I.ItemID )
			  and ( I.GroupID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the item information for this page
			select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, ISNULL(SortTitle, upper(title)) as SortTitle
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		end;

select count(*) as NUMBER_ITEMS from #TEMP_ITEMS;

		-- Return the facets if asked for
		if ( @include_facets = 'true' )
		begin	
			if (( LEN( isnull( @aggregationcode, '')) = 0 ) or ( @aggregationcode = 'all' ))
			begin
				-- Build the aggregation list
				select A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
				where ( T.ItemID = I.ItemID )
				  and ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( A.Include_In_Collection_Facet = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;	
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype1 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype2 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype3 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype4 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype5 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype6 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype7 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype8 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		end; -- End overall FACET block
	end; -- End else statement entered if there are any results to return
	
	-- return the query string as well, for debuggins
	select Query = @mainquery, RankSelection = @rankselection;
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	
	Set NoCount OFF;
			
	If @@ERROR <> 0 GoTo ErrorHandler;
    Return(0);
  
ErrorHandler:
    Return(@@ERROR);
	
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Metadata_Search_Paged]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Perform metadata search 
CREATE PROCEDURE [dbo].[SobekCM_Metadata_Search_Paged]
	@term1 nvarchar(255),
	@field1 int,
	@link2 int,
	@term2 nvarchar(255),
	@field2 int,
	@link3 int,
	@term3 nvarchar(255),
	@field3 int,
	@link4 int,
	@term4 nvarchar(255),
	@field4 int,
	@link5 int,
	@term5 nvarchar(255),
	@field5 int,
	@link6 int,
	@term6 nvarchar(255),
	@field6 int,
	@link7 int,
	@term7 nvarchar(255),
	@field7 int,
	@link8 int,
	@term8 nvarchar(255),
	@field8 int,
	@link9 int,
	@term9 nvarchar(255),
	@field9 int,
	@link10 int,
	@term10 nvarchar(255),
	@field10 int,
	@include_private bit,
	@aggregationcode varchar(20),	
	@pagesize int, 
	@pagenumber int,
	@sort int,
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output,
	@all_collections_items int output,
	@all_collections_titles int output	
AS
BEGIN
	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;
	
	-- Field#'s indicate which metadata field (if any).  These are numbers from the 
	-- SobekCM_Metadata_Types table.  A field# of -1, means all fields are included.
	
	-- Link#'s indicate if this is an AND-joiner ( intersect ) or an OR-joiner ( union )
	-- 0 = AND, 1 = OR, 2 = AND NOT
	
	-- Examples of using this procedure are:
	-- exec SobekCM_Metadata_Search 'haiti',1,0,'kesse',4,0,'',0
	-- This searches for materials which have haiti in the title AND kesse in the creator
	
	-- exec SobekCM_Metadata_Search 'haiti',1,1,'kesse',-1,0,'',0
	-- This searches for materials which have haiti in the title OR kesse anywhere
	
	-- Create the temporary table variables first
	-- Create the temporary table to hold all the item id's
	create table #TEMPZERO ( ItemID int primary key );
	create table #TEMP_ITEMS ( ItemID int primary key, fk_TitleID int, Hit_Count int, SortDate bigint );
		    
	-- declare both the sql query and the parameter definitions
	declare @SQLQuery AS nvarchar(max);
	declare @rankselection AS nvarchar(1000);
    declare @ParamDefinition AS NVarchar(2000);
    
    -- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@aggregationcode );
	
    -- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;
	    
    -- Start to build the main bulk of the query   
	set @SQLQuery = '';
    
    -- Was a field listed?
    if (( @field1 > 0 ) and ( @field1 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
    begin
		-- Get the name of this column then
		declare @field1_name varchar(100);
		set @field1_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field1 );

		-- Add this search then
		set @SQLQuery = @SQLQuery + ' contains ( ' + @field1_name + ', @innerterm1 )';
	end
	else
	begin
		-- Search the full citation then
		set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm1 )';	
	end;
            
    -- Start to build the query which will do ranking over the results which match this search
    set @rankselection = @term1;

	-- Add the second term, if there is one
	if (( LEN( ISNULL(@term2,'')) > 0 ) and (( @link2 = 0 ) or ( @link2 = 1 ) or ( @link2 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link2 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link2 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link2 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field2 > 0 ) and ( @field2 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field2_name varchar(100);
			set @field2_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field2 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field2_name + ', @innerterm2 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm2 )';	
		end;			
		
		-- Build the ranking query
		if ( @link2 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term2;	
		end
	end;    
	
	-- Add the third term, if there is one
	if (( LEN( ISNULL(@term3,'')) > 0 ) and (( @link3 = 0 ) or ( @link3 = 1 ) or ( @link3 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link3 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link3 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link3 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field3 > 0 ) and ( @field3 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field3_name varchar(100);
			set @field3_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field3 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field3_name + ', @innerterm3 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm3 )';	
		end;	
		
		-- Build the ranking query
		if ( @link3 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term3;		
		end
	end;   
	
	-- Add the fourth term, if there is one
	if (( LEN( ISNULL(@term4,'')) > 0 ) and (( @link4 = 0 ) or ( @link4 = 1 ) or ( @link4 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link4 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link4 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link4 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field4 > 0 ) and ( @field4 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field4_name varchar(100);
			set @field4_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field4 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field4_name + ', @innerterm4 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm4 )';	
		end;	
			
		-- Build the ranking query
		if ( @link4 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term4;		
		end
	end;
	
	-- Add the fifth term, if there is one
	if (( LEN( ISNULL(@term5,'')) > 0 ) and (( @link5 = 0 ) or ( @link5 = 1 ) or ( @link5 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link5 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link5 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link5 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field5 > 0 ) and ( @field5 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field5_name varchar(100);
			set @field5_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field5 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field5_name + ', @innerterm5 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm5 )';	
		end;
			
		-- Build the ranking query
		if ( @link5 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term5;		
		end
	end;
	
	-- Add the sixth term, if there is one
	if (( LEN( ISNULL(@term6,'')) > 0 ) and (( @link6 = 0 ) or ( @link6 = 1 ) or ( @link6 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link6 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link6 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link6 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field6 > 0 ) and ( @field6 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field6_name varchar(100);
			set @field6_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field6 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field6_name + ', @innerterm6 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm6 )';	
		end;
		
		-- Build the ranking query
		if ( @link6 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term6;		
		end
	end; 
	
	-- Add the seventh term, if there is one
	if (( LEN( ISNULL(@term7,'')) > 0 ) and (( @link7 = 0 ) or ( @link7 = 1 ) or ( @link7 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link7 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link7 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link7 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field7 > 0 ) and ( @field7 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field7_name varchar(100);
			set @field7_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field7 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field7_name + ', @innerterm7 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm7 )';	
		end;
		
		-- Build the ranking query
		if ( @link7 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term7;		
		end
	end;
	
	-- Add the eighth term, if there is one
	if (( LEN( ISNULL(@term8,'')) > 0 ) and (( @link8 = 0 ) or ( @link8 = 1 ) or ( @link8 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link8 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link8 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link8 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field8 > 0 ) and ( @field8 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field8_name varchar(100);
			set @field8_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field8 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field8_name + ', @innerterm8 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm8 )';	
		end;
		
		-- Build the ranking query
		if ( @link8 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term8;		
		end
	end;
	
	-- Add the ninth term, if there is one
	if (( LEN( ISNULL(@term9,'')) > 0 ) and (( @link9 = 0 ) or ( @link9 = 1 ) or ( @link9 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link9 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link9 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link9 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field9 > 0 ) and ( @field9 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field9_name varchar(100);
			set @field9_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field9 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field9_name + ', @innerterm9 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm9 )';	
		end;
		
		-- Build the ranking query
		if ( @link9 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term9;		
		end
	end;
	
	-- Add the tenth term, if there is one
	if (( LEN( ISNULL(@term10,'')) > 0 ) and (( @link10 = 0 ) or ( @link10 = 1 ) or ( @link10 = 2 )))
	begin	
		-- Was this an AND, OR, or AND NOT?
		if ( @link10 = 0 ) set @SQLQuery = @SQLQuery + ' and';
		if ( @link10 = 1 ) set @SQLQuery = @SQLQuery + ' or';
		if ( @link10 = 2 ) set @SQLQuery = @SQLQuery + ' and not';
		
		-- Was a field listed?
		if (( @field10 > 0 ) and ( @field10 in ( select MetadataTypeID from SobekCM_Metadata_Types )))
		begin
			-- Get the name of this column then
			declare @field10_name varchar(100);
			set @field10_name = ( select REPLACE(MetadataName, ' ', '_') from SobekCM_Metadata_Types where MetadataTypeID = @field10 );

			-- Add this search then
			set @SQLQuery = @SQLQuery + ' contains ( ' + @field10_name + ', @innerterm10 )';
		end
		else
		begin
			-- Search the full citation then
			set @SQLQuery = @SQLQuery + ' contains ( FullCitation, @innerterm10 )';	
		end;
		
		-- Build the ranking query
		if ( @link10 != 2 )
		begin
			set @rankselection = @rankselection + ' or ' + @term10;		
		end		
	end;
	
	-- Add the recompile option
	--set @SQLQuery = @SQLQuery + ' option (RECOMPILE)';

    -- Add the first term and start to build the query which will provide the items which match the search
    declare @mainquery nvarchar(max);
    set @mainquery = 'select L.Itemid from SobekCM_Metadata_Basic_Search_Table AS L';
    
    -- Do we need to limit by aggregation id as well?
    if ( @aggregationid > 0 )
    begin
		set @mainquery = @mainquery + ' join SobekCM_Item_Aggregation_Item_Link AS A ON ( A.ItemID = L.ItemID ) and ( A.AggregationID = ' + CAST( @aggregationid as varchar(5) ) + ')';   
    end    
    
    -- Add the full text search portion here
    set @mainquery = @mainquery + ' where ' + @SQLQuery;
	
	-- Set the parameter definition
	set @ParamDefinition = ' @innerterm1 nvarchar(255), @innerterm2 nvarchar(255), @innerterm3 nvarchar(255), @innerterm4 nvarchar(255), @innerterm5 nvarchar(255), @innerterm6 nvarchar(255), @innerterm7 nvarchar(255), @innerterm8 nvarchar(255), @innerterm9 nvarchar(255), @innerterm10 nvarchar(255)';
		
	-- Execute this stored procedure
	insert #TEMPZERO execute sp_Executesql @mainquery, @ParamDefinition, @term1, @term2, @term3, @term4, @term5, @term6, @term7, @term8, @term9, @term10;
			
	-- Perform ranking against the items and insert into another temporary table 
	-- with all the possible data elements needed for applying the user's sort
	insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate, Hit_Count )
	select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1), isnull(KEY_TBL.RANK, 0 )
	from SobekCM_Item I, #TEMPZERO AS T1 join
	  CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @rankselection ) AS KEY_TBL on KEY_TBL.[KEY] = T1.ItemID
	where ( T1.ItemID = I.ItemID )
	  and ( I.Deleted = 'false' )
      and ( I.IP_Restriction_Mask >= @lower_mask )	
      and ( I.IncludeInAll = 'true' );    

	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int;
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 
	
	-- If there were no results at all, check the count in the entire library
	if ( ( select COUNT(*) from #TEMP_ITEMS ) = 0 )
	begin
		-- Set the items and titles correctly
		set @total_items = 0;
		set @total_titles = 0;
		
		-- If there was an aggregation id, just return the counts for the whole library
		if ( @aggregationid > 0 )	
		begin
		
			-- Truncate the table and repull the data
			truncate table #TEMPZERO;
			
			-- Query against ALL aggregations this time
			declare @allquery nvarchar(max);
			set @allquery = 'select L.Itemid from SobekCM_Metadata_Basic_Search_Table AS L where ' + @SQLQuery;
			
			-- Execute this stored procedure
			insert #TEMPZERO execute sp_Executesql @allquery, @ParamDefinition, @term1, @term2, @term3, @term4, @term5, @term6, @term7, @term8, @term9, @term10;
			
			-- Get all items in the entire library then		  
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID )
			select I.ItemID, I.GroupID
			from #TEMPZERO T1, SobekCM_Item I
			where ( T1.ItemID = I.ItemID )
			  and ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask )	
			  and ( I.IncludeInAll = 'true' );  
			  
			-- Return these counts
			select @all_collections_items=COUNT(*), @all_collections_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
		end;
		
		-- Drop the big temporary table
		drop table #TEMPZERO;
	
	end
	else
	begin	
	
		-- Drop the big temporary table
		drop table #TEMPZERO;	
		  
		-- There are essentially two major paths of execution, depending on whether this should
		-- be grouped as items within the page requested titles ( sorting by title or the basic
		-- sorting by rank, which ranks this way ) or whether each item should be
		-- returned by itself, such as sorting by individual publication dates, etc..
		
		if ( @sort < 10 )
		begin	
			-- create the temporary title table definition
			declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int );	
			
			-- Return these counts
			select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
			
			-- Now, calculate the actual ending row, based on the ration, page information,
			-- and the lookahead factor
			if (( @total_items > 0 ) and ( @total_titles > 0 ))
			begin		
				-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
				declare @computed_value int;
				select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
				
				-- Compute the minimum value.  This cannot be less than @minpagelookahead.
				declare @floored_value int;
				select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
				
				-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
				declare @actual_pages int;
				select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead));

				-- Set the final row again then
				set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 
			end;	
					  
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
				(	select GroupID, G.BibID, 
						ROW_NUMBER() OVER (order by case when @sort=0 THEN (SUM(Hit_COunt)/COUNT(*)) end DESC,
													case when @sort=1 THEN G.SortTitle end ASC,												
													case when @sort=2 THEN BibID end ASC,
													case when @sort=3 THEN BibID end DESC) as RowNumber
					from #TEMP_ITEMS I, SobekCM_Item_Group G
					where I.fk_TitleID = G.GroupID
					group by G.GroupID, G.BibID, G.SortTitle )

			-- Insert the correct rows into the temp title table	
			insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
			select GroupID, BibID, RowNumber
			from TITLES_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
		
			-- Return the title information for this page
			select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_TITLES T, SobekCM_Item_Group G
			where ( T.TitleID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the item information for this page
			select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text,  isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, ISNULL(SortTitle, upper(title)) as SortTitle
			from #TEMP_ITEMS M, @TEMP_TITLES T, SobekCM_Item I
			where ( M.fk_TitleID = T.TitleID )
			  and ( I.ItemID = M.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		end
		else
		begin
			-- Create the temporary item table for paging purposes
			declare @TEMP_PAGED_ITEMS table ( ItemID int, RowNumber int );
			
			-- Since these sorts make each item paired with a single title row,
			-- number of items and titles are equal
			select @total_items=COUNT(*), @total_titles=COUNT(*)
			from #TEMP_ITEMS; 
			
			-- In addition, always return the max lookahead pages
			set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
			
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN isnull(SortDate,-1) end DESC) as RowNumber
					from #TEMP_ITEMS I
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			  
			-- Return the title information for this page
			select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
			where ( T.ItemID = I.ItemID )
			  and ( I.GroupID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the item information for this page
			select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, ISNULL(SortTitle, upper(title)) as SortTitle
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		end;

		-- Return the facets if asked for
		if ( @include_facets = 'true' )
		begin	
			if (( LEN( isnull( @aggregationcode, '')) = 0 ) or ( @aggregationcode = 'all' ))
			begin
				-- Build the aggregation list
				select A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
				where ( T.ItemID = I.ItemID )
				  and ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( A.Include_In_Collection_Facet = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;	
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype1 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype2 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype3 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype4 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype5 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype6 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype7 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype8 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		end; -- End overall FACET block
	end; -- End else statement entered if there are any results to return
	
	-- return the query string as well, for debuggins
	select Query = @mainquery, RankSelection = @rankselection;
	
	-- drop the temporary tables
	drop table #TEMP_ITEMS;
	
	Set NoCount OFF;
			
	If @@ERROR <> 0 GoTo ErrorHandler;
    Return(0);
  
ErrorHandler:
    Return(@@ERROR);
	
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Metadata_Save]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Save multiple pieces of metadata to the database for a single item
-- This calls SobekCM_Metadata_Save_Single which includes the mapping
-- from the textual type of metadata to the correct type id
CREATE PROCEDURE [dbo].[SobekCM_Metadata_Save]
	@itemid int,
	@metadata_type1 varchar(100),
	@metadata_value1 nvarchar(max),
	@metadata_type2 varchar(100),
	@metadata_value2 nvarchar(max),
	@metadata_type3 varchar(100),
	@metadata_value3 nvarchar(max),
	@metadata_type4 varchar(100),
	@metadata_value4 nvarchar(max),
	@metadata_type5 varchar(100),
	@metadata_value5 nvarchar(max),
	@metadata_type6 varchar(100),
	@metadata_value6 nvarchar(max),
	@metadata_type7 varchar(100),
	@metadata_value7 nvarchar(max),
	@metadata_type8 varchar(100),
	@metadata_value8 nvarchar(max),
	@metadata_type9 varchar(100),
	@metadata_value9 nvarchar(max),
	@metadata_type10 varchar(100),
	@metadata_value10 nvarchar(max)
AS
BEGIN

	-- Try to add the each piece of metadata
	if ( len( isnull(@metadata_value1,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type1, @metadata_value1;
	if ( len( isnull(@metadata_value2,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type2, @metadata_value2;
	if ( len( isnull(@metadata_value3,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type3, @metadata_value3;
	if ( len( isnull(@metadata_value4,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type4, @metadata_value4;
	if ( len( isnull(@metadata_value5,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type5, @metadata_value5;
	if ( len( isnull(@metadata_value6,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type6, @metadata_value6;
	if ( len( isnull(@metadata_value7,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type7, @metadata_value7;
	if ( len( isnull(@metadata_value8,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type8, @metadata_value8;
	if ( len( isnull(@metadata_value9,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type9, @metadata_value9;
	if ( len( isnull(@metadata_value10,'')) > 0 )
		exec SobekCM_Metadata_Save_Single @itemid, @metadata_type10, @metadata_value10;

END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Metadata_Exact_Search_Paged]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Perform an EXACT match type of search against one field of metadata
CREATE PROCEDURE [dbo].[SobekCM_Metadata_Exact_Search_Paged] 
	@term1 nvarchar(512),
	@field1 int,
	@include_private bit,
	@aggregationcode varchar(20),
	@pagesize int, 
	@pagenumber int,
	@sort int,
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output,
	@all_collections_items int output,
	@all_collections_titles int output			
AS
BEGIN

	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	-- Create the temporary table variable first
	-- Create the temporary table variable to hold all the item id's
	create table #TEMP_ITEMS ( ItemID int primary key, fk_TitleID int, Hit_Count int, SortDate bigint );

	-- Determine the start and end rows
	declare @rowstart int;
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + @pagesize - 1; 

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;

	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@aggregationcode );
	
	-- Determine the first 100 of the search term
	declare @term_start varchar(100);
	set @term_start = SUBSTRING(@term1, 1, 100);
	
	-- Perform the actual metadata search differently, depending on whether an aggregation was 
	-- included to limit this search
	if ( @aggregationid > 0 )
	begin 
		insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate )
		select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1)
		from SobekCM_Item AS I inner join
			 SobekCM_Item_Aggregation_Item_Link AS CL ON CL.ItemID = I.ItemID inner join
			 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
			 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = @term_start and M.MetadataValue = @term1
	    where ( I.Deleted = 'false' )
	      and ( CL.AggregationID = @aggregationid )
	      and ( I.IP_Restriction_Mask >= @lower_mask )
	      and ( I.Dark = 'false' );
	end
	else
	begin	
		insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate )
		select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1)
		from SobekCM_Item AS I inner join
			 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
			 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = @term_start and M.MetadataValue = @term1
	    where ( I.Deleted = 'false' )
	      and ( I.IP_Restriction_Mask >= @lower_mask )	
	      and ( I.IncludeInAll = 'true' )
	      and ( I.Dark = 'false' );
	end;

	-- If there were no results at all, check the count in the entire library
	if ( ( select COUNT(*) from #TEMP_ITEMS ) = 0 )
	begin
		-- Set the items and titles correctly
		set @total_items = 0;
		set @total_titles = 0;
		
		-- If there was an aggregation id, just return the counts for the whole library
		if ( @aggregationid > 0 )	
		begin
			-- Get all items in the entire library then		  
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, SortDate )
			select I.ItemID, I.GroupID, SortDate=isnull( I.SortDate,-1)
			from SobekCM_Item AS I inner join
				 SobekCM_Metadata_Unique_Link ML on ML.ItemID = I.ItemID inner join
				 SobekCM_Metadata_Unique_Search_Table M ON M.MetadataID = ML.MetadataID and MetadataTypeID = @field1 and M.MetadataValueStart = @term_start and M.MetadataValue = @term1
			where ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask )	
			  and ( I.IncludeInAll = 'true' );	 
			  
			-- Return these counts
			select @all_collections_items=COUNT(*), @all_collections_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS; 		
		end;
	end
	else
	begin	
		-- There are essentially two major paths of execution, depending on whether this should
		-- be grouped as items within the page requested titles ( sorting by title or the basic
		-- sorting by rank, which ranks this way ) or whether each item should be
		-- returned by itself, such as sorting by individual publication dates, etc..
		
		if ( @sort < 10 )
		begin	
			-- create the temporary title table definition
			declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int);	
			
			-- Get the total counts
			select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
			
			-- Now, calculate the actual ending row, based on the ration, page information,
			-- and the lookahead factor
			if (( @total_items > 0 ) and ( @total_titles > 0 ))
			begin	
				-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
				declare @computed_value int;
				select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
				
				-- Compute the minimum value.  This cannot be less than @minpagelookahead.
				declare @floored_value int;
				select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
				
				-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
				declare @actual_pages int;
				select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead)); 

				-- Set the final row again then
				set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 
			end;
					  
			-- Create saved select across titles for row numbers for selecting correct page(s) of results
			with TITLES_SELECT AS
				(	select GroupID, G.BibID, 
						ROW_NUMBER() OVER (order by case when @sort=0 THEN G.SortTitle end ASC,	
													case when @sort=1 THEN G.SortTitle end ASC,												
													case when @sort=2 THEN BibID end ASC,
													case when @sort=3 THEN BibID end DESC) as RowNumber
					from #TEMP_ITEMS I, SobekCM_Item_Group G
					where I.fk_TitleID = G.GroupID
					group by G.GroupID, G.BibID, G.SortTitle )

			-- Insert the correct rows into the temp title table	
			insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
			select GroupID, BibID, RowNumber
			from TITLES_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			
			-- Return the title information for this page of results
			select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_TITLES T, SobekCM_Item_Group G
			where ( T.TitleID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the item information for this page of results
			select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text,  isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, isnull(I.SortTitle, upper(title)) as SortTitle
			from #TEMP_ITEMS M, @TEMP_TITLES T, SobekCM_Item I
			where ( M.fk_TitleID = T.TitleID )
			  and ( I.ItemID = M.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		end
		else
		begin
			-- Create the temporary item table variable for paging purposes
			declare @TEMP_PAGED_ITEMS table ( ItemID int, RowNumber int );
			
			-- Since these sorts make each item paired with a single title row,
			-- number of items and titles are equal
			select @total_items=COUNT(*), @total_titles=COUNT(*)
			from #TEMP_ITEMS;
			
			-- In addition, always return the max lookahead pages
			set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 
			
			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN isnull(SortDate,-1) end DESC) as RowNumber
					from #TEMP_ITEMS I
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			  
			-- Return the title information for this page
			select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
			where ( T.ItemID = I.ItemID )
			  and ( I.GroupID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the item information for this page
			select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text,  isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, isnull(I.SortTitle, upper(title)) as SortTitle
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		end
		
		-- Return the facets if asked for
		if ( @include_facets = 'true' )
		begin	
			-- Build the aggregation list
			if ( LEN( isnull( @aggregationcode, '')) = 0 )
			begin
				select A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
				where ( T.ItemID = I.ItemID )
				  and ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( A.Include_In_Collection_Facet = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;		
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype1 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype2 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype3 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype4 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype5 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype6 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype7 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype8 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		end; -- End overall FACET block
	end; -- End else statement entered if there are any results to return
	
	-- Drop the temporary table
	drop table #TEMP_ITEMS;
			
    Set NoCount OFF;	
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Metadata_Basic_Search_Paged]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SobekCM_Metadata_Basic_Search_Paged] 
	@searchcondition nvarchar(4000),
	@include_private bit,
	@aggregationcode varchar(20),
	@pagesize int, 
	@pagenumber int,
	@sort int,
	@minpagelookahead int,
	@maxpagelookahead int,
	@lookahead_factor float,
	@include_facets bit,
	@facettype1 smallint,
	@facettype2 smallint,
	@facettype3 smallint,
	@facettype4 smallint,
	@facettype5 smallint,
	@facettype6 smallint,
	@facettype7 smallint,
	@facettype8 smallint,
	@total_items int output,
	@total_titles int output,
	@all_collections_items int output,
	@all_collections_titles int output												
AS
BEGIN
	-- No need to perform any locks here, especially given the possible
	-- length of this search
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	-- Create the temporary tables first
	-- Create the temporary table to hold all the item id's
	create table #TEMP_ITEMS ( ItemID int primary key, fk_TitleID int, Hit_Count int, SortDate bigint );

	-- Determine the start and end rows
	declare @rowstart int; 
	declare @rowend int; 
	set @rowstart = (@pagesize * ( @pagenumber - 1 )) + 1;
	set @rowend = @rowstart + ( @pagesize * @minpagelookahead ) - 1; 

	-- Set value for filtering privates
	declare @lower_mask int;
	set @lower_mask = 0;
	if ( @include_private = 'true' )
	begin
		set @lower_mask = -256;
	end;

	-- Determine the aggregationid
	declare @aggregationid int;
	set @aggregationid = ( select ISNULL(AggregationID,-1) from SobekCM_Item_Aggregation where Code=@aggregationcode );
	
	-- Perform the actual metadata search differently, depending on whether an aggregation was 
	-- included to limit this search
	if ( @aggregationid > 0 )
	begin		  
		insert into #TEMP_ITEMS ( ItemID, fk_TitleID, Hit_Count, SortDate )
		select CL.ItemID, I.GroupID, KEY_TBL.RANK, SortDate
		from SobekCM_Item AS I inner join
			 SobekCM_Item_Aggregation_Item_Link AS CL ON CL.ItemID = I.ItemID inner join
			 CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
		where ( I.Deleted = 'false' )
		  and ( CL.AggregationID = @aggregationid )
		  and ( I.IP_Restriction_Mask >= @lower_mask )
		  and ( I.Dark = 'false' );
	end
	else
	begin	
		insert into #TEMP_ITEMS ( ItemID, fk_TitleID, Hit_Count, SortDate )
		select I.ItemID, I.GroupID, KEY_TBL.RANK, SortDate
		from SobekCM_Item AS I inner join
			 CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
		where ( I.Deleted = 'false' )
		  and ( I.IP_Restriction_Mask >= @lower_mask ) 
		  and ( I.IncludeInAll = 'true' )
		  and ( I.Dark = 'false' );
	end;
	
	-- If there were no results at all, check the count in the entire library
	if ( ( select COUNT(*) from #TEMP_ITEMS ) = 0 )
	begin
		-- Set the items and titles correctly
		set @total_items = 0;
		set @total_titles = 0;
		
		-- If there was an aggregation id, just return the counts for the whole library
		if ( @aggregationid > 0 )	
		begin
			-- Get all items in the entire library then
			insert into #TEMP_ITEMS ( ItemID, fk_TitleID, Hit_Count, SortDate )
			select I.ItemID, I.GroupID, KEY_TBL.RANK, SortDate
			from SobekCM_Item AS I inner join
				CONTAINSTABLE(SobekCM_Metadata_Basic_Search_Table, FullCitation, @SearchCondition ) AS KEY_TBL on KEY_TBL.[KEY] = I.ItemID
			where ( I.Deleted = 'false' )
			  and ( I.IP_Restriction_Mask >= @lower_mask ) 
			  and ( I.IncludeInAll = 'true' )
			  and ( I.Dark = 'false' );
			  
			-- Return these counts
			select @all_collections_items=COUNT(*), @all_collections_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;		
		end;
	end
	else
	begin	
		-- There are essentially two major paths of execution, depending on whether this should
		-- be grouped as items within the page requested titles ( sorting by title or the basic
		-- sorting by rank, which ranks this way ) or whether each item should be
		-- returned by itself, such as sorting by individual publication dates, etc..	
		if ( @sort < 10 )
		begin	
			-- create the temporary title table definition
			declare @TEMP_TITLES table ( TitleID int, BibID varchar(10), RowNumber int);		
			
			-- Get the total counts
			select @total_items=COUNT(*), @total_titles=COUNT(distinct fk_TitleID)
			from #TEMP_ITEMS;
			
			-- Now, calculate the actual ending row, based on the ration, page information,
			-- and the lookahead factor
			if (( @total_items > 0 ) and ( @total_titles > 0 ))
			begin	
				-- Compute equation to determine possible page value ( max - log(factor, (items/title)/2))
				declare @computed_value int;
				select @computed_value = (@maxpagelookahead - CEILING( LOG10( ((cast(@total_items as float)) / (cast(@total_titles as float)))/@lookahead_factor)));
				
				-- Compute the minimum value.  This cannot be less than @minpagelookahead.
				declare @floored_value int;
				select @floored_value = 0.5 * ((@computed_value + @minpagelookahead) + ABS(@computed_value - @minpagelookahead));
				
				-- Compute the maximum value.  This cannot be more than @maxpagelookahead.
				declare @actual_pages int;
				select @actual_pages = 0.5 * ((@floored_value + @maxpagelookahead) - ABS(@floored_value - @maxpagelookahead)); 

				-- Set the final row again then
				set @rowend = @rowstart + ( @pagesize * @actual_pages ) - 1; 
			end;
					  
			-- Create saved select across titles for row numbers
			with TITLES_SELECT AS
				(	select GroupID, G.BibID, 
						ROW_NUMBER() OVER (order by case when @sort=0 THEN (SUM(Hit_COunt)/COUNT(*)) end DESC,
													case when @sort=1 THEN G.SortTitle end ASC,												
													case when @sort=2 THEN BibID end ASC,
													case when @sort=3 THEN BibID end DESC) as RowNumber
					from #TEMP_ITEMS I, SobekCM_Item_Group G
					where I.fk_TitleID = G.GroupID
					group by G.GroupID, G.BibID, G.SortTitle )

			-- Insert the correct rows into the temp title table	
			insert into @TEMP_TITLES ( TitleID, BibID, RowNumber )
			select GroupID, BibID, RowNumber
			from TITLES_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			
			-- Return the title information for this page
			select RowNumber as TitleID, T.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_TITLES T, SobekCM_Item_Group G
			where ( T.TitleID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the item information for this page
			select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text,  isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, isnull(I.SortTitle, upper(title)) as SortTitle
			from #TEMP_ITEMS M, @TEMP_TITLES T, SobekCM_Item I
			where ( M.fk_TitleID = T.TitleID )
			  and ( I.ItemID = M.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		end
		else
		begin
			-- Create the temporary item table variable for paging purposes
			declare @TEMP_PAGED_ITEMS table ( ItemID int, RowNumber int );
			
			-- Since these sorts make each item paired with a single title row,
			-- number of items and titles are equal
			select @total_items=COUNT(*), @total_titles=COUNT(*)
			from #TEMP_ITEMS;
			
			-- In addition, always return the max lookahead pages
			set @rowend = @rowstart + ( @pagesize * @maxpagelookahead ) - 1; 

			-- Create saved select across items for row numbers
			with ITEMS_SELECT AS
			 (	select I.ItemID, 
					ROW_NUMBER() OVER (order by case when @sort=10 THEN isnull(SortDate,9223372036854775807)  end ASC,
												case when @sort=11 THEN isnull(SortDate,-1) end DESC) as RowNumber
					from #TEMP_ITEMS I
					group by I.ItemID, SortDate )
						  
			-- Insert the correct rows into the temp item table	
			insert into @TEMP_PAGED_ITEMS ( ItemID, RowNumber )
			select ItemID, RowNumber
			from ITEMS_SELECT
			where RowNumber >= @rowstart
			  and RowNumber <= @rowend;
			  
			-- Return the title information for this page
			select RowNumber as TitleID, G.BibID, G.GroupTitle, G.ALEPH_Number, G.OCLC_Number, isnull(G.GroupThumbnail,'') as GroupThumbnail, G.[Type], isnull(G.Primary_Identifier_Type,'') as Primary_Identifier_Type, isnull(G.Primary_Identifier, '') as Primary_Identifier
			from @TEMP_PAGED_ITEMS T, SobekCM_Item I, SobekCM_Item_Group G
			where ( T.ItemID = I.ItemID )
			  and ( I.GroupID = G.GroupID )
			order by RowNumber ASC;
			
			-- Return the item information for this page
			select T.RowNumber as fk_TitleID, VID, Title, IP_Restriction_Mask, isnull(I.MainThumbnail,'') as MainThumbnail, isnull(I.Level1_Index, -1) as Level1_Index, isnull(I.Level1_Text,'') as Level1_Text, isnull(I.Level2_Index, -1) as Level2_Index, isnull(I.Level2_Text,'') as Level2_Text, ISNULL(I.Level3_Index,-1) as Level3_Index, ISNULL(I.Level3_Text,'') as Level3_Text, isnull(I.PubDate,'') as PubDate, I.[PageCount], isnull(I.Link,'') as Link, isnull(Publisher,'') as Publisher, isnull(Author,'') as Author, isnull(Format,'') as Format, isnull(Donor,'') as Donor, isnull(Spatial_Display,'') as [Spatial], isnull(Edition_Display,'') as Edition, isnull(Institution_Display,'') as Institution, isnull( Material_Display, '') as Material, isnull(Measurement_Display, '') as Measurement, isnull( StylePeriod_Display, '') as StylePeriod, isnull(Technique_Display, '') as Technique, isnull(Subjects_Display, '') as Subjects, ISNULL( Spatial_KML, '') as Spatial_KML, isnull(I.SortTitle, upper(title)) as SortTitle
			from SobekCM_Item I, @TEMP_PAGED_ITEMS T
			where ( T.ItemID = I.ItemID )
			order by T.RowNumber, Level1_Index, Level2_Index, Level3_Index;
		end

		-- Return the facets if asked for
		if ( @include_facets = 'true' )
		begin	
			-- Build the aggregation list
			if ( LEN( isnull( @aggregationcode, '')) = 0 )
			begin
				select A.Code, A.ShortName, Metadata_Count=Count(*)
				from SobekCM_Item_Aggregation A, SobekCM_Item_Aggregation_Item_Link L, SobekCM_Item I, #TEMP_ITEMS T
				where ( T.ItemID = I.ItemID )
				  and ( I.ItemID = L.ItemID )
				  and ( L.AggregationID = A.AggregationID )
				  and ( A.Hidden = 'false' )
				  and ( A.isActive = 'true' )
				  and ( A.Include_In_Collection_Facet = 'true' )
				group by A.Code, A.ShortName
				order by Metadata_Count DESC, ShortName ASC;
			end;
			
			-- Return the FIRST facet
			if ( @facettype1 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype1 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SECOND facet
			if ( @facettype2 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype2 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the THIRD facet
			if ( @facettype3 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype3 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FOURTH facet
			if ( @facettype4 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype4 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the FIFTH facet
			if ( @facettype5 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype5 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SIXTH facet
			if ( @facettype6 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype6 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the SEVENTH facet
			if ( @facettype7 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype7 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
			
			-- Return the EIGHTH facet
			if ( @facettype8 > 0 )
			begin
				-- Return the first 100 values
				select MetadataValue, Metadata_Count
				from (	select top(100) U.MetadataID, Metadata_Count = COUNT(*)
						from #TEMP_ITEMS I, Metadata_Item_Link_Indexed_View U with (NOEXPAND)
						where ( U.ItemID = I.ItemID )
						  and ( U.MetadataTypeID = @facettype8 )
						group by U.MetadataID
						order by Metadata_Count DESC ) F, SobekCM_Metadata_Unique_Search_Table M
				where M.MetadataID = F.MetadataID
				order by Metadata_Count DESC, MetadataValue ASC;
			end;
		end; -- End overall FACET block
	end; -- End else statement entered if there are any results to return
	
	-- Drop the temporary table
	drop table #TEMP_ITEMS;
	
	SET NOCOUNT OFF;
END;
GO
/****** Object:  StoredProcedure [dbo].[SobekCM_Mass_Update_Item_Behaviors]    Script Date: 08/18/2013 13:31:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Modifies the item behaviors in a mass update for all items in 
-- a particular item group
CREATE PROCEDURE [dbo].[SobekCM_Mass_Update_Item_Behaviors]
	@GroupID int,
	@IP_Restriction_Mask smallint,
	@CheckoutRequired bit,
	@Dark_Flag bit,
	@Born_Digital bit,
	@AggregationCode1 varchar(20),
	@AggregationCode2 varchar(20),
	@AggregationCode3 varchar(20),
	@AggregationCode4 varchar(20),
	@AggregationCode5 varchar(20),
	@AggregationCode6 varchar(20),
	@AggregationCode7 varchar(20),
	@AggregationCode8 varchar(20),
	@HoldingCode varchar(20),
	@SourceCode varchar(20),
	@Icon1_Name varchar(50),
	@Icon2_Name varchar(50),
	@Icon3_Name varchar(50),
	@Icon4_Name varchar(50),
	@Icon5_Name varchar(50),
	@Viewer1_TypeID int,
	@Viewer1_Label nvarchar(50),
	@Viewer1_Attribute nvarchar(250),
	@Viewer2_TypeID int,
	@Viewer2_Label nvarchar(50),
	@Viewer2_Attribute nvarchar(250),
	@Viewer3_TypeID int,
	@Viewer3_Label nvarchar(50),
	@Viewer3_Attribute nvarchar(250),
	@Viewer4_TypeID int,
	@Viewer4_Label nvarchar(50),
	@Viewer4_Attribute nvarchar(250),
	@Viewer5_TypeID int,
	@Viewer5_Label nvarchar(50),
	@Viewer5_Attribute nvarchar(250),
	@Viewer6_TypeID int,
	@Viewer6_Label nvarchar(50),
	@Viewer6_Attribute nvarchar(250)
AS
begin transaction

	--Update the main item's flags if provided
	if ( @IP_Restriction_Mask is not null )
	begin
		update SobekCM_Item
		set IP_Restriction_Mask=@IP_Restriction_Mask
		where ( GroupID = @GroupID );
	end;
	
	if ( @CheckoutRequired is not null )
	begin
		update SobekCM_Item
		set CheckoutRequired=@CheckoutRequired
		where ( GroupID = @GroupID );
	end;
	
	if ( @Dark_Flag is not null )
	begin
		update SobekCM_Item
		set Dark=@Dark_Flag
		where ( GroupID = @GroupID );
	end;
	
	if ( @Born_Digital is not null )
	begin
		update SobekCM_Item
		set Born_Digital=@Born_Digital
		where ( GroupID = @GroupID );
	end;
	
	-- Only do icon stuff if the first icon has length
	if ( len( isnull( @Icon1_Name, '' )) > 0 ) 
	begin

		-- Clear the links to all existing icons
		delete from SobekCM_Item_Icons 
		where exists (  select *
						from SobekCM_Item
						where ( SobekCM_Item.GroupID=@GroupID )
						  and ( SobekCM_Item.ItemID = SobekCM_Item_Icons.ItemID ));
		
		-- Add the first icon to this object  (this requires the icons have been pre-established )
		declare @IconID int;
		if ( len( isnull( @Icon1_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon1_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				select ItemID, @IconID, 1 from SobekCM_Item I where I.GroupID=@GroupID;
			end;
		end;

		-- Add the second icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon2_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon2_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				select ItemID, @IconID, 2 from SobekCM_Item I where I.GroupID=@GroupID;
			end;
		end;

		-- Add the third icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon3_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon3_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				select ItemID, @IconID, 3 from SobekCM_Item I where I.GroupID=@GroupID;
			end;
		end;

		-- Add the fourth icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon4_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon4_Name;
			
			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				select ItemID, @IconID, 4 from SobekCM_Item I where I.GroupID=@GroupID;
			end;
		end;

		-- Add the fifth icon to this object  (this requires the icons have been pre-established )
		if ( len( isnull( @Icon5_Name, '' )) > 0 ) 
		begin
			-- Get the Icon ID for this icon
			select @IconID = IconID from SobekCM_Icon where Icon_Name = @Icon5_Name;

			-- Tie this item to this icon
			if ( ISNULL(@IconID,-1) > 0 )
			begin
				insert into SobekCM_Item_Icons ( ItemID, IconID, [Sequence] )
				select ItemID, @IconID, 5 from SobekCM_Item I where I.GroupID=@GroupID;
			end;
		end;
	end;
	
	-- Only modify the aggregation codes if they have length
	if ( LEN ( ISNULL( @AggregationCode1, '')) > 0 )
	begin
	
		-- Clear all links to aggregations
		delete from SobekCM_Item_Aggregation_Item_Link 
		where exists ( select * from SobekCM_Item I where I.GroupID=@GroupID and I.ItemID=SobekCM_Item_Aggregation_Item_Link.ItemID );

		-- Add all of the aggregations
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @AggregationCode1;
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @AggregationCode2;
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @AggregationCode3;
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @AggregationCode4;
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @AggregationCode5;
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @AggregationCode6;
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @AggregationCode7;
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @AggregationCode8;

	end;

	-- Check for Holding Institution Code
	declare @AggregationID int;
	if ( len ( isnull ( @HoldingCode, '' ) ) > 0 )
	begin
		-- Does this institution already exist?
		if (( select count(*) from SobekCM_Item_Aggregation where Code = @HoldingCode ) = 0 )
		begin
			-- Add new institution
			insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
			values ( @HoldingCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
		end;
		
		-- Add the link to this holding code ( and any legitimate parent aggregations )
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @HoldingCode;
	end;

	-- Check for Source Institution Code
	if ( len ( isnull ( @SourceCode, '' ) ) > 0 )
	begin
		-- Does this institution already exist?
		if (( select count(*) from SobekCM_Item_Aggregation where Code = @SourceCode ) = 0 )
		begin
			-- Add new institution
			insert into SobekCM_Item_Aggregation ( Code, [Name], ShortName, Description, ThematicHeadingID, [Type], isActive, Hidden, DisplayOptions, Map_Search, Map_Display, OAI_Flag, ContactEmail, HasNewItems )
			values ( @SourceCode, 'Added automatically', 'Added automatically', 'Added automatically', -1, 'Institution', 'false', 'true', '', 0, 0, 'false', '', 'false' );
		end;

		-- Add the link to this holding code ( and any legitimate parent aggregations )
		exec [SobekCM_Mass_Update_Item_Aggregation_Link] @GroupID, @SourceCode;
	end;
	
	-- Only affect the viewers if there is a provided viewer
	if ( @Viewer1_TypeID > 0 )
	begin
	
		-- Clear the links to all existing viewers
		delete from SobekCM_Item_Viewers 
		where exists ( select * from SobekCM_Item I where I.GroupID=@GroupID and I.ItemID=SobekCM_Item_Viewers.ItemID );
		
		-- Add the first viewer information
		if ( @Viewer1_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			select I.ItemID, @Viewer1_TypeID, @Viewer1_Attribute, @Viewer1_Label from SobekCM_Item I where I.GroupID=@GroupID;
		end;
		
		-- Add the second viewer information
		if ( @Viewer2_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			select I.ItemID, @Viewer2_TypeID, @Viewer2_Attribute, @Viewer2_Label from SobekCM_Item I where I.GroupID=@GroupID;
		end;
		
		-- Add the third viewer information
		if ( @Viewer3_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			select I.ItemID, @Viewer3_TypeID, @Viewer3_Attribute, @Viewer3_Label from SobekCM_Item I where I.GroupID=@GroupID;
		end;
		
		-- Add the fourth viewer information
		if ( @Viewer4_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			select I.ItemID, @Viewer4_TypeID, @Viewer4_Attribute, @Viewer4_Label from SobekCM_Item I where I.GroupID=@GroupID;
		end;
		
		-- Add the fifth viewer information
		if ( @Viewer5_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			select I.ItemID, @Viewer5_TypeID, @Viewer5_Attribute, @Viewer5_Label from SobekCM_Item I where I.GroupID=@GroupID;
		end;
		
		-- Add the first viewer information
		if ( @Viewer6_TypeID > 0 )
		begin
			-- Insert this viewer information
			insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
			select I.ItemID, @Viewer6_TypeID, @Viewer6_Attribute, @Viewer6_Label from SobekCM_Item I where I.GroupID=@GroupID;
		end;
	end;

commit transaction;
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Archive_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Marks this volume as having files archived to TIVOLI
-- This is called when an item is moved into the tivoli area by the Tivoli Processor application
-- Written by Mark Sullivan (October 2009)
CREATE PROCEDURE [dbo].[Tracking_Archive_Complete]
	@bibid varchar(10),
	@vid varchar(5),
	@user varchar(50),
	@usernotes varchar(1000)
AS
begin transaction

	-- Add this archiving to the workhistory log
	exec [Tracking_Add_Workflow_Once_Per_Day] @bibid, @vid, @user, @usernotes, 28, null
	
	-- If this item is not marked as being internally archived, mark that now
	if (( select COUNT(*) from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and I.VID = @vid and G.BibiD=@bibid ) = 1 )
	begin
		-- Get the item id
		declare @itemid int
		select @itemid=ItemID from SobekCM_Item I, SobekCM_Item_Group G where I.GroupID = G.GroupID and I.VID = @vid and G.BibiD=@bibid
		
		-- Check that this is marked as archived
		update SobekCM_Item 
		set Locally_Archived='true'
		where ItemID=@itemid
	
	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_OCR_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Adds an OCR Workflow progress for a specific volume.
CREATE PROCEDURE [dbo].[Tracking_OCR_Complete]
	@bibid varchar(10),
	@vid varchar(5)	
AS
begin transaction

	exec [Tracking_Add_Workflow_Once_Per_Day] @bibid, @vid, '', '', 6, null

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_PreQC_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Marks this volume''s image complete and qc as selected and pending
-- This is called when an item successfully passes ''Pre-QC!''
-- Written by Mark Sullivan (March 2006)
CREATE PROCEDURE [dbo].[Tracking_PreQC_Complete]
	@bibid varchar(10),
	@vid varchar(5),
	@user varchar(255),
	@storagelocation varchar(255)
AS
begin transaction

	-- Add this pre-qc to the workhistory log
	exec [Tracking_Add_Workflow_Once_Per_Day] @bibid, @vid, @user, '', 4, @storagelocation

	-- Get the volume id
	declare @itemid int

	select @itemid = ItemID
	from SobekCM_Item_Group G, SobekCM_Item I
	where ( BibID = @bibid )
	    and ( I.GroupID = G.GroupID ) 
	    and ( VID = @vid)

	-- continue if a volumeid was located
	if ( isnull( @itemid, -1 ) > 0 )
	begin
		-- Update the milestones
		update SobekCM_Item
		set Milestone_DigitalAcquisition = ISNULL(Milestone_DigitalAcquisition, getdate()),
		    Milestone_ImageProcessing = ISNULL(Milestone_ImageProcessing, getdate()),
		    Last_MileStone=2
		where ItemID=@itemid
	end

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Load_Metadata_Update_Complete]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Marks this volume as having processed a metadata update for it
-- This is called when an item successfully passes 'UFDC Loader'
-- Written by Mark Sullivan (April 2007)
CREATE PROCEDURE [dbo].[Tracking_Load_Metadata_Update_Complete]
	@bibid varchar(10),
	@vid varchar(5),
	@user varchar(50),
	@usernotes varchar(1000)
AS
begin transaction

	exec [Tracking_Add_Workflow_Once_Per_Day] @bibid, @vid, @user, @usernotes, 11, null

commit transaction
GO
/****** Object:  StoredProcedure [dbo].[Tracking_Update_Tracking_Box]    Script Date: 08/18/2013 13:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Tracking_Update_Tracking_Box]
	@Tracking_Box varchar(25),
	@ItemID int
AS
begin

	-- Update the item row
	update SobekCM_Item set Tracking_Box = @Tracking_Box where ItemID = @ItemID
	
	-- Create the full citation, which also copies this into the metadata table
	exec SobekCM_Create_Full_Citation_Value @ItemID	
end
GO
GRANT EXECUTE ON [dbo].[Admin_Update_All_AggregationCodes_Values] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Admin_Update_Cached_Aggregation_Metadata_Links] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Admin_Update_Cached_Aggregation_Metadata_Links] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Admin_Update_Cached_Aggregation_Metadata_Links] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Builder_Get_Item_Aggregation_Thumbnail] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Builder_Get_Item_Aggregation_Thumbnail] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Builder_Get_Minimum_Item_Information] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Builder_Get_Minimum_Item_Information] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Builder_Get_Minimum_Item_Information] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Edit_OCR_Progress] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[FDA_All_Reports] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[FDA_All_Reports_By_Date] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[FDA_Get_Report_By_ID] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[FDA_Get_Reports_By_Package] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[FDA_Report_Save] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[FDA_Report_Save] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Importer_Load_Lookup_Tables] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Importer_Load_Lookup_Tables] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Add_Description_Tag] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Add_Item_To_User_Folder] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Add_User_Aggregations_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Add_User_Group_Aggregations_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Add_User_Group_Projects_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Add_User_Group_Templates_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Add_User_Projects_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Add_User_Templates_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Change_Password] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Delete_Description_Tag] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Delete_Item_From_All_User_Folders] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Delete_Item_From_User_Folder] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Delete_Project] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Delete_Template] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Delete_User_Folder] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Delete_User_Group] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Delete_User_Search] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Edit_User_Folder] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_All_Projects_Templates] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_All_User_Groups] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_All_Users] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_Folder_Information] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_Folder_Search_Information] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_Public_Folder_Browse] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_User_By_UFID] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_User_By_UserID] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_User_By_UserName_Password] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_User_Folder_Browse] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_User_Group] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_User_Item_Link_Relationships] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_User_Item_Link_Relationships] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Get_User_Searches] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Link_User_To_Item] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Link_User_To_User_Group] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Reset_User_Password] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Save_Project] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Save_Template] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Save_User] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Save_User_Group] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Save_User_Search] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Save_User2] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Set_Aggregation_Home_Page_Flag] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Set_Receive_Stats_Email_Flag] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Set_Receive_Stats_Email_Flag] TO [sobek_user] AS [dbo]
GO

GRANT EXECUTE ON [dbo].[mySobek_UserName_Exists] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_View_All_User_Tags] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[PalmmRightsMD_Save_Access_Embargo_UMI] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[PalmmRightsMD_Save_Access_Embargo_UMI] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[PalmmRightsMD_Save_Access_Embargo_UMI] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Save_Item_Tracking_Info] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Save_Item_Tracking_Info] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_External_Record_Number] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_External_Record_Number] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_External_Record_Number] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_Item_Error_Log] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_Item_Error_Log] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_OAI_PMH_Data] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_OAI_PMH_Data] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_OAI_PMH_Data] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Add_Web_Skin] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Admin_Suggest_User_Item_Links] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Admin_Suggest_User_Item_Links] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Admin_Suggest_User_Item_Links] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Builder_Incoming_Folder_Delete] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Builder_Incoming_Folder_Edit] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Check_For_Record_Existence] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_External_Record_Numbers] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_External_Record_Numbers] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_External_Record_Numbers] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Item_Error_Log] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Item_Error_Log] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Old_Item_Info] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Old_Item_Info] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Old_Item_Info] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Region_Link_By_Item] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Region_Link_By_Item] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Region_Link_By_Item] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Clear_Web_Skin_Portal_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Coordinate_Points_By_Aggregation] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Create_Full_Citation_Value] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Create_Full_Citation_Value] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Create_Full_Citation_Value] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Icon] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Icon] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Item] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Item] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Item_Aggregation] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Item_Aggregation_Alias] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Item_Aggregation_Alias] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Portal] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Single_IP] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Thematic_Heading] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Delete_Web_Skin] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Edit_Aggregation_Details] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Edit_IP_Range] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Edit_Portal] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Edit_Single_IP] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Edit_Thematic_Heading] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Aggregation_Browse_Paged] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_Browse_Paged] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_Browse_Paged] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_Browse_Paged] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_Groups] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_Groups_First_VID] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_Groups_First_VID] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_IP_Restrictions] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_Portals] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_All_Regions] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_BibID_VID_From_ItemID] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_BibID_VID_From_ItemID] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Build_Error_Logs] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Builder_Settings] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Builder_Settings] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Builder_Settings] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Builder_Settings] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Codes] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Collection_Hierarchies] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Collection_Statistics_History] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Description_Tags_By_Aggregation] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Email] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Email_List] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_IP_Restriction_Range] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Item_Aggregation_Aliases] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Item_Aggregation2] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Item_Brief_Info] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Item_Details2] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Item_Statistics] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_ItemID] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Items_By_Coordinates] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Items_Needing_Aditional_Work] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Items_Needing_Aditional_Work] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Metadata_Browse] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Metadata_Browse] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Metadata_Fields] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Mime_Types] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Multiple_Volumes] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_OAI_Data] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_OAI_Data_Codes] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_OAI_Data_Item] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_OAI_Sets] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Search_Stop_Words] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Settings] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Settings] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Settings] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Statistics_Dates] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Translation] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Get_Web_Skins] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Icon_List] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Item_Count_By_Collection] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Item_Count_By_Collection_By_Date_Range] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Item_Count_By_Collection_By_Date_Range] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Item_Count_By_Collection_By_Dates] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Item_List_Brief2] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Item_List_Web] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Items_By_ALEPH] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Items_By_OCLC] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Link_Aggregation_Thematic_Heading] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Link_User_To_Item] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Link_User_To_Item] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Log_Email] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Manager_Get_Thematic_Headings] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Manager_GroupID_From_BibID] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Manager_Newspapers_Without_Serial_Info] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_MarcXML_Production_Feed] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_MarcXML_Production_Feed] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_MarcXML_Test_Feed] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_MarcXML_Test_Feed] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Mass_Update_Item_Aggregation_Link] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Mass_Update_Item_Aggregation_Link] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Mass_Update_Item_Aggregation_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Mass_Update_Item_Behaviors] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Mass_Update_Item_Behaviors] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Mass_Update_Item_Behaviors] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Basic_Search_Paged] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Clear2] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Clear2] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Clear2] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Exact_Search_Paged] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Save] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Save] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Save] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Save_Single] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Save_Single] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Metadata_Search_Paged] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Online_Archived_Space] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Page_Item_Count_History] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Recreate_All_Implied_Links] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Icon] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Icon] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation_Alias] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation_Alias] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation_Alias] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation_Hierarchy_Link] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation_Hierarchy_Link] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Aggregation_Hierarchy_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Behaviors] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Behaviors] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Behaviors] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Footprint] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Footprint] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Footprint] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group_Behaviors] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group_Behaviors] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group_Behaviors] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group_Web_Skins] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group_Web_Skins] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Group_Web_Skins] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Item_Aggregation_Link] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Item_Aggregation_Link] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Item_Aggregation_Link] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Views] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_Views] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_VRACore_Extensions] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_VRACore_Extensions] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Item_VRACore_Extensions] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_New_Item] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_New_Item] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_New_Item] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Serial_Hierarchy] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Serial_Hierarchy] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Save_Serial_Hierarchy] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Send_Email] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Send_Email] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_IP_Restriction_Mask] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_IP_Restriction_Mask] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_Item_Comments] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_Item_Comments] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_Item_Comments] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_Main_Thumbnail] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_Main_Thumbnail] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_Setting_Value] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_Setting_Value] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Set_Setting_Value] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Simple_Item_List] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Statistics_Aggregate] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Statistics_Aggregation_Titles] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Statistics_By_Date_Range] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Statistics_Item_Group] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Statistics_Lookup_Tables] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Stats_Get_User_Linked_Items] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Stats_Get_User_Linked_Items] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Stats_Get_User_Linked_Items_Stats] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Stats_Get_User_Linked_Items_Stats] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Stats_Get_Users_Linked_To_Items] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Stats_Get_Users_Linked_To_Items] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Update_Additional_Work_Needed_Flag] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Update_Additional_Work_Needed_Flag] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Update_Item_Group] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Update_Item_Group] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Update_Item_Group] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Update_Item_Online_Statistics] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Update_Item_Online_Statistics] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[SobekCM_Web_Skin_Portal_Add] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[TEMP_Get_Group_Info_By_BibID] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Add_File_Archive_Log] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Add_File_Archive_Log] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Admin_Update] TO [sobek_admin] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Admin_Update] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Complete_File_Request] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Get_File_By_Bib_VID] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Outstanding_File_Requests] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Request_File] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tivoli_Request_File] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Past_Workflow] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Past_Workflow] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Past_Workflow] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Past_Workflow_By_ItemID] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Past_Workflow_By_ItemID] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Past_Workflow_By_ItemID] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow_By_ItemID] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow_By_ItemID] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow_By_ItemID] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow_Once_Per_Day] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow_Once_Per_Day] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Add_Workflow_Once_Per_Day] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Archive_Complete] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Archive_Complete] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Born_Digital_Item_Count] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Box_List] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Box_List] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Box_List] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Digital_Acquisition_Complete] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Digital_Acquisition_Complete] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Digital_Acquisition_Complete] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Get_Aggregation_Browse] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Get_Aggregation_Privates] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Get_Aggregation_Privates] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Get_Aggregation_Privates] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Get_All_Possible_Disposition_Types] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Get_All_Possible_Workflows] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Get_History_Archives] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Get_Multiple_Volumes] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Image_Processing_Complete] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Image_Processing_Complete] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Image_Processing_Complete] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Item_Milestone_Report] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Item_Milestone_Report] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Item_Milestone_Report] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Items_By_ALEPH] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Items_By_OCLC] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Items_Pending_Online_Complete] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Load_Metadata_Update_Complete] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Load_Metadata_Update_Complete] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Metadata_Basic_Search] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Metadata_Exact_Search] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Metadata_Search] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_OCR_Complete] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_OCR_Complete] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Online_Edit_Complete] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Online_Submit_Complete] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_PreQC_Complete] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_PreQC_Complete] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Submit_Online_Page_Division] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Submit_QC_Log] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Submit_QC_Log] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Born_Digital] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Born_Digital] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Born_Digital] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Digitization_Milestones] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Digitization_Milestones] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Digitization_Milestones] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Disposition] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Disposition] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Disposition] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Disposition_Advice] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Disposition_Advice] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Disposition_Advice] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_List] TO [public] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Material_Received] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Material_Received] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Material_Received] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Physical_Milestones] TO [sobek_builder] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Physical_Milestones] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Physical_Milestones] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Tracking_Box] TO [sobek_itemeditor] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[Tracking_Update_Tracking_Box] TO [sobek_user] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[mySobek_Update_UFDC_User] TO [sobek_user] AS [dbo]
GO