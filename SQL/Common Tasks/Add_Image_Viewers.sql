

-- 1 = JPEG
-- 2 = JPEG2000
-- 8 = Related Items

-- Add JPEG viewer to all items without it
insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
select ItemID, 1, '', ''
from SobekCM_Item I
where not exists ( select 1 from SobekCM_Item_Viewers O where O.ItemID=I.ItemID and O.ItemViewTypeID=1 );
GO

-- Add JPEG2000 viewer to all items without it
insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
select ItemID, 2, '', ''
from SobekCM_Item I
where not exists ( select 1 from SobekCM_Item_Viewers O where O.ItemID=I.ItemID and O.ItemViewTypeID=2 );
GO

-- Add related images (thumbnails) to all items without it
insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
select ItemID, 8, '', ''
from SobekCM_Item I
where not exists ( select 1 from SobekCM_Item_Viewers O where O.ItemID=I.ItemID and O.ItemViewTypeID=8 );
GO


