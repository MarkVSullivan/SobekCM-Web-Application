

update SobekCM_Settings set TabPage='Builder', Heading='Builder Settings' where Setting_Key='Builder IIS Logs Directory';
GO

update SobekCM_Settings set TabPage='System / Server Settings', Heading='System Settings' where Setting_Key='Can Submit Items Online';
GO

update SobekCM_Settings set TabPage='Deprecated', Heading='Deprecated'  where Setting_Key='Builder Add PageTurner ItemViewer';
GO

  
DROP PROCEDURE SobekCM_Get_Viewer_Priority;
GO

DROP TABLE SobekCM_Item_Viewer_Priority;
GO

if ( not exists ( select 1 from SobekCM_Settings where Setting_Key = 'Builder Last Message' ))
begin
	insert into SobekCM_Settings ( Setting_Key, Setting_Value, TabPage, Heading, Hidden, Reserved, Help, Options, Dimensions ) values ( 'Builder Last Message','','Builder','Status','0',0,'Help for Builder Last Message','','');
end
else
begin
	update SobekCM_Settings set TabPage='Builder', Heading='Status', Hidden='false', Reserved=0, Help='Help for Builder Last Message', Options='', Dimensions='' where Setting_Key='Builder Last Message';
end
GO

if ( not exists ( select 1 from SobekCM_Settings where Setting_Key = 'Builder Last Run Finished' ))
begin
	insert into SobekCM_Settings ( Setting_Key, Setting_Value, TabPage, Heading, Hidden, Reserved, Help, Options, Dimensions ) values ( 'Builder Last Run Finished','','Builder','Status','0',0,'Help for Builder Last Run Finished','','');
end
else
begin
	update SobekCM_Settings set TabPage='Builder', Heading='Status', Hidden='false', Reserved=0, Help='Help for Builder Last Run Finished', Options='', Dimensions='' where Setting_Key='Builder Last Run Finished';
end
GO

if ( not exists ( select 1 from SobekCM_Settings where Setting_Key = 'Builder Version' ))
begin
	insert into SobekCM_Settings ( Setting_Key, Setting_Value, TabPage, Heading, Hidden, Reserved, Help, Options, Dimensions ) values ( 'Builder Version','','Builder','Status','0',0,'Help for Builder Version','','');
end
else
begin
	update SobekCM_Settings set TabPage='Builder', Heading='Status', Hidden='false', Reserved=0, Help='Help for Builder Version', Options='', Dimensions='' where Setting_Key='Builder Version';
end
GO
