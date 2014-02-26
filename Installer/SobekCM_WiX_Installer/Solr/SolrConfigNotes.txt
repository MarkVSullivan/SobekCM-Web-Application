SobekCM Solr

This folder contains the files to get the Solr configuration working for SObekCM.  Solr is utilized for full-text searching of the documents within this digital repository.

Requirements:  You must have both Java and Apache Tomcat installed to utilize the Solr indexer.


Description of the subfolders, and instructions, follow below:

-------------

Subfolder DOCUMENTS

This will contain the full text index of all the documents in the digital repository.  This is utilized when the return from a search will be a pointer to the complete matching document (including snippets).  Each document in this index contains the complete text of all the pages of the document.

ACTION: None necessary

-------------

Subfolder PAGES

This will contain the full text index of each individual page within all the documents in the digital repository.  This is utilized for doing a search within a single document, when the return from the search will be a pointer to the matching page(s) in the document.  Each individual page will be indexed individually here.

ACTION: None necessary

-------------

Subfolder SOLR_EXECUTABLE

This contains version 4.6 of Solr.  This WAR file should be copied to the home directory of the Apache Tomcat servlet server. ...
