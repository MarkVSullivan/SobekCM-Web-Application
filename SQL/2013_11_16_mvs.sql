

update SobekCM_Metadata_Types
set canFacetBrowse='0'
where MetadataName like '%.Display';

update SobekCM_Metadata_Types
set SobekCode='ET' 
where MetadataName='Edition';


