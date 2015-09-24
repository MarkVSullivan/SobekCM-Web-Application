




-- Add a persisted, computed column to the SobekCM_Item_Group table
alter table SobekCM_Item_Group add BibIdPortion as substring(BibID,0,3) persisted;

-- Create an index on the persisted, computed columne
create index IX_SobekCM_Item_Group_BibID_Portion on SobekCM_Item_Group (BibIdPortion);

