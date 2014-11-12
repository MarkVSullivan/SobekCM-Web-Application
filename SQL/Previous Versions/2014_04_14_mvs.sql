

if ( ( select count(*) from SobekCM_Settings where Setting_Key='Spreadsheet Library License') = 0 )
begin
	insert into SobekCM_Settings ( Setting_Key, Setting_Value )
	values ( 'Spreadsheet Library License', 'rptfH59FQbdh/xnbn2HROqPjiaMPmz3L');
end;
GO

update SobekCM_Settings set Setting_Value='rptfH59FQbdh/xnbn2HROqPjiaMPmz3L' where Setting_Key='Spreadsheet Library License';
GO

