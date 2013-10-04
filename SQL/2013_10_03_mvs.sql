

SET IDENTITY_INSERT Tracking_WorkFlow ON;
GO

insert into Tracking_WorkFlow ( WorkFlowID, WorkFlowName, WorkFlowNotes ) values ( 46, 'Uploaded Page Images', 'Uploaded new page images for the item' );
insert into Tracking_WorkFlow ( WorkFlowID, WorkFlowName, WorkFlowNotes ) values ( 47, 'Updated Coordinates', 'Used the online map edit feature to add or edit coordinates associated with this item');
insert into Tracking_WorkFlow ( WorkFlowID, WorkFlowName, WorkFlowNotes ) values ( 48, 'Managed Downloads', 'Managed the download files for this item');
GO

SET IDENTITY_INSERT Tracking_WorkFlow OFF;
GO


