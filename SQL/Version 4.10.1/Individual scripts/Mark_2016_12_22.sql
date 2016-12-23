


if ( not exists ( select 1 from SobekCM_Settings where Setting_Key = 'Ace Editor Theme' ))
begin
	insert into SobekCM_Settings ( Setting_Key, Setting_Value, TabPage, Heading, Hidden, Reserved, Help, Options, Dimensions ) 
	values ( 'Ace Editor Theme','chrome','General Settings','UI Settings','0',0,'Set the theme for the Ace editor, used for CSS and Javascript editing, as well as TEI editing, if that plug-in is enabled.','{ACE_THEMES}','');
end;
GO

