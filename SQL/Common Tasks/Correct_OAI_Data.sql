
-- Add the missing URL to the MARC records
with marc_oai_url as
(	
  select O.ItemID, 'http://digital.wolfsonian.org/' + BibID + '/' + VID as URL
  from SobekCM_Item_OAI O, SobekCM_Item I, SobekCM_Item_Group G
  where Data_Code = 'marc21'
    and O.ItemID = I.ItemID
	and I.GroupID = G.GroupID
	and OAI_Data like '%<marc:subfield code="u"></marc:subfield>%'
)
update SobekCM_Item_OAI 
set OAI_Data = replace(OAI_Data, '<marc:subfield code="u"></marc:subfield>', '<marc:subfield code="u">' + C.URL + '</marc:subfield>')
from marc_oai_url C
where C.ItemID = SObekCM_Item_OAI.ItemID
  and Data_Code = 'marc21';

-- Change '<dc:Identifer>' to '<dc:Identifier>'
update SObekCM_Item_OAI
set OAI_Data = replace(OAI_Data, '<dc:Identifer>', '<dc:Identifier>')
where OAI_Data like '%<dc:Identifer>%'
  and Data_Code='oai_dc';

