#region Using directives

using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;

#endregion

namespace SobekCM.Resource_Object.Utilities
{
    /// <summary> Validates a METS file with references to the SobekCM custom schema </summary>
    /// <remarks> Written by Gus Clifton and Mark Sullivan ( 2005 ). </remarks>
    public class METS_Validator_Object
    {
        private readonly XmlSchemaSet cache;
        private StringBuilder errors;
        private bool isValid = true;

        /// <summary> Constructor for a new instance of the METS_Validator_Object class. </summary>
        /// <param name="load_all_schemas"> Flag indicates whether third-party schemas should be loaded for validation </param>
        public METS_Validator_Object(bool load_all_schemas)
        {
            // Define the new XmlSChemeCollection
            cache = new XmlSchemaSet();

            // Import the METS schema, which is the only schema needed.
            // METS schema governs the importing of other schemas.
            try
            {
                cache.Add("http://www.loc.gov/METS/", "http://www.loc.gov/standards/mets/mets.xsd");
                cache.Add("http://www.loc.gov/mods/v3", "http://www.loc.gov/mods/v3/mods-3-3.xsd");
                cache.Add("http://digital.uflib.ufl.edu/metadata/sobekcm/", "http://digital.uflib.ufl.edu/metadata/sobekcm/sobekcm.xsd");

                // With this flag set, the DAITSS XSD is read
                if (load_all_schemas)
                {
                    cache.Add("http://www.fcla.edu/dls/md/daitss/", "http://www.fcla.edu/dls/md/daitss/daitss.xsd");
                }
            }
            catch
            {

            }
        }

        /// <summary> Gets the error string for the last METS file </summary>
        public string Errors
        {
            get { return errors.ToString(); }
        }

        /// <summary> Validates a single METS XML file </summary>
        /// <param name="METS_File"> Location and name of file to validate </param>
        /// <returns> TRUE if it validates, otherwise FALSE </returns>
        /// <remarks> If this does not validate, the accumulated errors can be reached through the 'Errors' property. </remarks>
        public bool Validate_Against_Schema(string METS_File)
        {
            try
            {
                // Set some initial values
                isValid = true;
                errors = new StringBuilder();

                // Create the reader and validator
                XmlReaderSettings metsSettings = new XmlReaderSettings();
                metsSettings.Schemas.Add(cache);
                metsSettings.ValidationType = ValidationType.Schema;
                metsSettings.ValidationEventHandler += new ValidationEventHandler(MyValidationEventHandler);

                XmlReader validator = XmlReader.Create(METS_File, metsSettings);

                // Step through the XML file
                while (validator.Read())
                {
                    /* Just reading through, looking for problems... */
                }
                validator.Close();

                // Return the valid flag
                return isValid;
            }
            catch ( Exception ee )
            {
                errors.Append("Error caught during validation of METS: " + ee.Message);
                return false;
            }
        }

        /// <summary> EventHandler is called when there is an error during validation </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MyValidationEventHandler(object sender, ValidationEventArgs args)
        {
            // Set the flag
            isValid = false;

            // Add this error to the building list of errors
            errors.Append(args.Message + "\n");
        }
    }
}