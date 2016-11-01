

  -- Add web content id to each of the web content statistics rows
  update SobekCM_WebContent_Statistics 
  set WebContentID = ( select WebContentID 
                       from SobekCM_WebContent W
                       where coalesce(W.Level1,'') = coalesce(SobekCM_WebContent_Statistics.Level1, '' )
                         and coalesce(W.Level2,'') = coalesce(SobekCM_WebContent_Statistics.Level2, '' )
                         and coalesce(W.Level3,'') = coalesce(SobekCM_WebContent_Statistics.Level3, '' )
                         and coalesce(W.Level4,'') = coalesce(SobekCM_WebContent_Statistics.Level4, '' )
                         and coalesce(W.Level5,'') = coalesce(SobekCM_WebContent_Statistics.Level5, '' )
                         and coalesce(W.Level6,'') = coalesce(SobekCM_WebContent_Statistics.Level6, '' )
                         and coalesce(W.Level7,'') = coalesce(SobekCM_WebContent_Statistics.Level7, '' )
                         and coalesce(W.Level8,'') = coalesce(SobekCM_WebContent_Statistics.Level8, '' ))
  where exists ( select WebContentID 
                       from SobekCM_WebContent W
                       where coalesce(W.Level1,'') = coalesce(SobekCM_WebContent_Statistics.Level1, '' )
                         and coalesce(W.Level2,'') = coalesce(SobekCM_WebContent_Statistics.Level2, '' )
                         and coalesce(W.Level3,'') = coalesce(SobekCM_WebContent_Statistics.Level3, '' )
                         and coalesce(W.Level4,'') = coalesce(SobekCM_WebContent_Statistics.Level4, '' )
                         and coalesce(W.Level5,'') = coalesce(SobekCM_WebContent_Statistics.Level5, '' )
                         and coalesce(W.Level6,'') = coalesce(SobekCM_WebContent_Statistics.Level6, '' )
                         and coalesce(W.Level7,'') = coalesce(SobekCM_WebContent_Statistics.Level7, '' )
                         and coalesce(W.Level8,'') = coalesce(SobekCM_WebContent_Statistics.Level8, '' ));
   
  -- Delete any that are not linked?? (may skip this part)                      
  delete from SobekCM_WebContent_Statistics
  where WebContentID is null;
                         