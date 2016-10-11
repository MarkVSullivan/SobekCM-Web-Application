using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace SobekCM.Resource_Object.Utilities
{
    /// <summary> Class used to perform very basic validation against any XML file </summary>
    public class XmlValidator
    {
        private StringBuilder errors;
        private bool isValid;

        /// <summary> Constructor for a new instance of the XmlValidator clss </summary>
        public XmlValidator()
        {
            // Do nothing
        }

        /// <summary> Gets the error string for the last METS file </summary>
        public string Errors
        {
            get { return errors.ToString(); }
        }

        /// <summary> Validates whether XML is valid XML, without any checks against schemas </summary>
        /// <param name="XmlFile"> Full pathname for the XML file to validate </param>
        /// <returns> TRUE if the XML is valid, otherwise FALSE </returns>
        public bool IsValid(string XmlFile)
        {
            XmlReader validator = null;
            try
            {
                // Set some initial values
                isValid = true;
                errors = new StringBuilder();

                // Create the reader and validator
                XmlReaderSettings metsSettings = new XmlReaderSettings {ValidationType = ValidationType.None};
                metsSettings.ValidationEventHandler += MyValidationEventHandler;

                validator = XmlReader.Create(XmlFile, metsSettings);

                // Step through the XML file
                while (validator.Read())
                {
                    /* Just reading through, looking for problems... */
                }
                validator.Close();

                // Return the valid flag
                return isValid;
            }
            catch (Exception ee)
            {
                errors.AppendLine(ee.Message);
                return false;
            }
            finally
            {
                if (validator != null)
                    validator.Close();
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
            errors.AppendLine(args.Message);
        }
    }
}
