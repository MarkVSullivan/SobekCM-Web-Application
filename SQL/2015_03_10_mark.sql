
if ( not exists ( select * from SobekCM_External_Record_Type where ExtRecordType = 'ACCESSION' ))
begin
	insert into SobekCM_External_Record_Type ( ExtRecordType, repeatableTypeFlag )
	values ( 'ACCESSION', 'true' );
end;
GO
