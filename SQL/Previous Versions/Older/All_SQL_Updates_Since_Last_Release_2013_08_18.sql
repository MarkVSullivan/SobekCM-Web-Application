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


CREATE VIEW [dbo].[Statistics_Item_Aggregation_Link_View] WITH SCHEMABINDING
AS
SELECT  AggregationID, I.ItemID, I.FileCount, I.[PageCount], I.GroupID, Milestone_OnlineComplete
FROM  dbo.SobekCM_Item_Aggregation_Item_Link CL, dbo.SobekCM_Item I
WHERE ( CL.ItemID = I.ItemID )
  and ( I.Deleted = 'false' )
  and ( Milestone_OnlineComplete is not null );
GO

/****** Object:  Index [Statistics_Item_Aggregation_Link]    Script Date: 09/27/2013 14:21:13 ******/
CREATE UNIQUE CLUSTERED INDEX [Statistics_Item_Aggregation_Link_View_IX] ON [dbo].[Statistics_Item_Aggregation_Link_View] 
(
	[AggregationID] ASC,
	[ItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY];
GO

/****** Object:  Index [Statistics_Item_Aggregation_Link_View_IX2]    Script Date: 09/27/2013 14:26:04 ******/
CREATE NONCLUSTERED INDEX [Statistics_Item_Aggregation_Link_View_IX2] ON [dbo].[Statistics_Item_Aggregation_Link_View] 
(
	[Milestone_OnlineComplete] ASC
)
INCLUDE ( [AggregationID],
[GroupID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

SET IDENTITY_INSERT SobekCM_Item_Viewer_Types ON;
GO

insert into SobekCM_Item_Viewer_Types ( ItemViewTypeID, ViewType ) values ( 11, 'Dataset Codebook');
insert into SobekCM_Item_Viewer_Types ( ItemViewTypeID, ViewType ) values ( 12, 'Dataset Reports');
insert into SobekCM_Item_Viewer_Types ( ItemViewTypeID, ViewType ) values ( 13, 'Dataset View Data');
insert into SobekCM_Item_Viewer_Types ( ItemViewTypeID, ViewType ) values ( 14, 'JPEG/Text Two Up');
GO

SET IDENTITY_INSERT SobekCM_Item_Viewer_Types OFF;
GO

SET IDENTITY_INSERT Tracking_WorkFlow ON;
GO

insert into Tracking_WorkFlow ( WorkFlowID, WorkFlowName, WorkFlowNotes ) values ( 46, 'Uploaded Page Images', 'Uploaded new page images for the item' );
insert into Tracking_WorkFlow ( WorkFlowID, WorkFlowName, WorkFlowNotes ) values ( 47, 'Updated Coordinates', 'Used the online map edit feature to add or edit coordinates associated with this item');
insert into Tracking_WorkFlow ( WorkFlowID, WorkFlowName, WorkFlowNotes ) values ( 48, 'Managed Downloads', 'Managed the download files for this item');
GO

SET IDENTITY_INSERT Tracking_WorkFlow OFF;
GO


alter table Tracking_Progress add DateStarted datetime null;
alter table Tracking_Progress add Duration int not null default(0);
alter table Tracking_Progress add RelatedEquipment nvarchar(255) null;
GO

alter table Tracking_Workflow add Start_Event_Number int null;
alter table Tracking_Workflow add End_Event_Number int null;
alter table Tracking_Workflow add Start_And_End_Event_Number int null;
alter table Tracking_Workflow add Start_Event_Desc nvarchar(100) null;
alter table Tracking_Workflow add End_Event_Desc nvarchar(100) null;
GO

-- Update the scanning, processing, and material disposition

CREATE TABLE [dbo].[Tracking_ScanningEquipment](
	[EquipmentID] [int] IDENTITY(1,1) NOT NULL,
	[ScanningEquipment] [nvarchar](255) NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[Location] [nvarchar](255) NULL,
	[EquipmentType] [nvarchar](255) NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_Tracking_ScanningEquipment] PRIMARY KEY CLUSTERED 
(
	[EquipmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

alter table mySobek_User add ScanningTechnician bit default(0);
alter table mySobek_User add ProcessingTechnician bit default(0);
GO

alter table SobekCM_Item add Complete_KML varchar(max) null;
GO

alter table SobekCM_Item_Footprint add Segment_KML varchar(max) null;
GO

-- Close all current workflows
update Tracking_Progress set DateStarted=DateCompleted, Duration=0;
GO


ALTER TABLE SobekCM_Item_Aggregation ADD DeleteDate date null;
GO

ALTER TABLE SobekCM_Settings ALTER COLUMN Setting_Key varchar(255) not null;
GO

-- Add the select statement into the Item Aggregation search table
alter table SobekCM_Item_Aggregation
add Browse_Results_Display_SQL nvarchar(max) not null default 'select S.ItemID, S.Publication_Date, S.Creator, S.[Publisher.Display], S.Format, S.Edition, S.Material, S.Measurements, S.Style_Period, S.Technique, S.[Subjects.Display], S.Source_Institution, S.Donor from SobekCM_Metadata_Basic_Search_Table S, @itemtable T where S.ItemID = T.ItemID order by T.RowNumber;';
GO

-- Add the COINs statement into the item table, to be returned 
alter table SobekCM_Item
add COinS_OpenURL varchar(max);
GO

-- Create the type of table for use in the browse/search tables
CREATE TYPE TempPagedItemsTableType AS TABLE (
ItemID int NOT NULL, 
RowNumber int NOT NULL );
GO

GRANT EXECUTE ON TYPE::TempPagedItemsTableType to sobek_user;
GO




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


alter table mySobek_User add Can_Delete_All_Items bit not null default('false');
GO
alter table mySobek_User_Group add Can_Delete_All_Items bit not null default('false');
GO
alter table mySobek_User_Group_Edit_Aggregation add IsAdmin bit not null default('false');
GO
alter table mySobek_User_Edit_Aggregation add IsAdmin bit not null default('false');
GO


update SobekCM_Metadata_Types
set canFacetBrowse='0'
where MetadataName like '%.Display';
GO

update SobekCM_Metadata_Types
set SobekCode='ET' 
where MetadataName='Edition';
GO


alter table SobekCM_Builder_Incoming_Folders add Can_Move_To_Content_Folder bit null;
alter table SobekCM_Builder_Incoming_Folders add BibID_Roots_Restrictions varchar(255) not null default('');
GO

alter table SobekCM_Builder_Incoming_Folders drop column Contains_Institutional_Folders;
GO


update SobekCM_Builder_Incoming_Folders
set Can_Move_To_Content_Folder = 'true'
GO

CREATE TABLE [dbo].[SobekCM_Builder_Log](
	[BuilderLogID] [bigint] IDENTITY(1,1) NOT NULL,
	[RelatedBuilderLogID] [bigint] NULL,
	[LogDate] [datetime] NULL,
	[BibID_VID] [varchar](16) NULL,
	[LogType] [varchar](25) NULL,
	[LogMessage] [varchar](2000) NULL,
	SuccessMessage varchar(500) null,
	METS_Type varchar(50) null,
 CONSTRAINT [PK_SobekCM_Builder_Log] PRIMARY KEY CLUSTERED 
(
	[BuilderLogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[SobekCM_Builder_Log]  WITH CHECK ADD  CONSTRAINT [FK_Self_SobekCM_Builder_Log] FOREIGN KEY([RelatedBuilderLogID])
REFERENCES [dbo].[SobekCM_Builder_Log] ([BuilderLogID])
GO



CREATE TABLE [dbo].[mySobek_User_Settings](
	[UserSettingID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Setting_Key] [nvarchar](255) NOT NULL,
	[Setting_Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_mySobek_User_Settings] PRIMARY KEY CLUSTERED 
(
	[UserSettingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO 


ALTER TABLE [dbo].[mySobek_User_Settings]  WITH CHECK ADD  CONSTRAINT [FK_User_Settings_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[mySobek_User] ([UserID])
GO


CREATE TABLE [dbo].[mySobek_User_Item_Permissions](
	[UserItemPermissionsID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[PermissionStatement] [varchar](255) NOT NULL,
 CONSTRAINT [PK_mySobek_User_Item_Permissions] PRIMARY KEY CLUSTERED 
(
	[UserItemPermissionsID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[mySobek_User_Item_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_User_Item_Permissions_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[mySobek_User] ([UserID])
GO

ALTER TABLE [dbo].[mySobek_User_Item_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_User_Item_Permissions_Item] FOREIGN KEY([ItemID])
REFERENCES [dbo].[SobekCM_Item] ([ItemID])
GO

CREATE TABLE [dbo].[SobekCM_Item_Aggregation_Settings](
	[AggregationSettingID] [bigint] IDENTITY(1,1) NOT NULL,
	[AggregationID] [int] NOT NULL,
	[Setting_Key] [nvarchar](255) NOT NULL,
	[Setting_Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SobekCM_Item_Aggregation_Settings] PRIMARY KEY CLUSTERED 
(
	[AggregationSettingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[SobekCM_Item_Aggregation_Settings]  WITH CHECK ADD  CONSTRAINT [FK_Aggregation_Settings_Aggregation] FOREIGN KEY([AggregationID])
REFERENCES [dbo].SobekCM_Item_Aggregation ([AggregationID])
GO






-- DROP ALL EXISTING STORED PROCEDURES
DECLARE @procName varchar(500);
declare cur cursor
for select [name] from sys.objects where type='p' and is_ms_shipped='false'
and (( [name] like 'Admin_%' ) or ( [name] like 'Builder_%' ) or ( [name] like 'Edit_%' ) or ( [name] like 'FDA_%' ) or ( [name] like 'Importer_%' ) or ( [name] like 'mySobek_%' )
or ( [name] like 'SobekCM_%' )or ( [name] like 'TEMP_%' ) or ( [name] like 'Tivoli_%' ) or ( [name] like 'Tracking_%' ));

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


