

delete from SobekCM_Settings where Setting_Key='Log Files Directory';
delete from SobekCM_Settings where Setting_Key='Log Files URL';
GO


  if ( not exists ( select 1 from SobekCM_Settings where Setting_Key = 'Show Citation For Dark Items' ))
  begin

	insert into SobekCM_Settings (Setting_Key, Setting_Value, TabPage, Heading, Hidden, Reserved, Help, Options, Dimensions )
	values ( 'Show Citation For Dark Items', 'true', 'Digital Resource Settings', 'Online Behavior', 0, 0, 'Flag indicates if the citation is displayed online for DARK items.', 'true|false', null );

  end;
  GO

