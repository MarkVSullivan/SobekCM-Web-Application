


if ( not exists ( select 1 from SobekCM_Settings where Setting_Key = 'Include Result Count In Text' ))
begin
	insert into SobekCM_Settings ( Setting_Key, Setting_Value, TabPage, Heading, Hidden, Reserved, Help, Options, Dimensions )
	values ( 'Include Result Count In Text','true','General Settings','Search Settings','0',0,'When this is set to TRUE, the result count will be displayed in the search explanation text ( i.e., Your search for ... resulted in 2 results ).  Setting this to FALSE will not show the final portion in that text.','true|false','');
end;
GO

