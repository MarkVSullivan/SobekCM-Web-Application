#region Using directives

using System.IO;
using System.Security.Cryptography;

#endregion

namespace SobekCM.Resource_Object.Divisions
{
    /// <summary>
    /// FileSha1 is an object used to convert a file to a sha1 Checksum string.  
    /// The name of this file is passed in during the construction of this 
    /// object, or by using the property FileName.  Then the Checksum is retrieved
    /// by using the Checksum property.<br />
    /// <br />
    /// </summary>
    /// <remarks>  Object created by Chris Nicolich for University of Florida's Digital Library Center.  </remarks>
    /// <SPAN class="lang">[C#]</SPAN> 
    public class FileSHA1
    {
        // Private Data Members
        private bool errorFlag;
        private string fileName;
        private string hashResult;

        /// <summary>Constuctor for a new instance of this object</summary>
        /// <param name="fileName"></param>
        public FileSHA1(string fileName)
        {
            this.fileName = fileName;
            computeChecksum();
        }

        /// <summary>Constuctor for a new instance of this object</summary>
        public FileSHA1()
        {
            fileName = "";
            hashResult = "";
        }

        /// <summary> gets and sets the file name for this object</summary>
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                computeChecksum();
            }
        }

        /// <summary> gets the sha1 check sum for this object</summary>
        public string Checksum
        {
            get { return hashResult; }
        }

        /// <summary>Gets the error message for calculating the check sum of this object</summary>
        public bool Error
        {
            get { return errorFlag; }
        }

        /// <summary> Private method used to calculate the sha1 check summ</summary>
        private void computeChecksum()
        {
            try
            {
                FileStream hashFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                byte[] result = sha1.ComputeHash(hashFile);

                hashFile.Close();

                string buffer = "";
                foreach (byte thisByte in result)
                {
                    if (thisByte < 16)
                    {
                        buffer += "0" + thisByte.ToString("x");
                    }
                    else
                    {
                        buffer += thisByte.ToString("x");
                    }
                }
                hashResult = buffer;

                errorFlag = false;
            }
            catch
            {
                hashResult = "ERROR";
                errorFlag = true;
            }
        }
    }
}

