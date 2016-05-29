#region Using directives

using System;
using System.Drawing;
using System.IO;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM_Resource_Database.Builder
{
    /// <summary> Creates a file object for a page in a bibliographic package </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    public class Builder_Page_File : IComparable
    {
        private string extension = "tif";
        private int fileerrortype;
        private string fileid;
        private string filename;
        private string pageid;
        private bool present;
        private string relative_directory;
        private Builder_SplitFileName splitName;
        private Bitmap thisImage;

        #region Constructors and Constructor Helpers

        /// <summary> Constructor for a new instance of the Builder_Page_File class </summary>
        /// <param name="FileLocation">Directory within which to find the files </param>
        public Builder_Page_File(string FileLocation)
        {
            constructor2_helper(FileLocation, String.Empty, false);
        }

        /// <summary> Constructor for a new instance of the Builder_Page_File class </summary>
        /// <param name="FileLocation">Directory within which to find the files </param>
        /// <param name="SplitName">Flag indicates if the file names should be split for sorting</param>
        public Builder_Page_File(string FileLocation, bool SplitName)
        {
            constructor2_helper(FileLocation, String.Empty, SplitName);
        }

        /// <summary> Constructor for a new instance of the Builder_Page_File class </summary>
        /// <param name="FileLocation">Directory within which to find the files </param>
        /// <param name="RelativeDirectory">Relative directory of this file from the digital resource's main source directory</param>
        /// <param name="SplitName">Flag indicates if the file names should be split for sorting</param>
        public Builder_Page_File(string FileLocation, string RelativeDirectory, bool SplitName)
        {
            constructor2_helper(FileLocation, RelativeDirectory, SplitName);
        }

        private void constructor2_helper(string FileLocation, string RelativeDirParam, bool SplitName)
        {
            // Create the FileInfo object
            FileInfo fileInfo = new FileInfo(FileLocation);

            // Populate all the date from that
            fileid = String.Empty;
            if (fileInfo.Extension.Length > 1)
                extension = fileInfo.Extension.Substring(1);
            else
                extension = "";
            filename = fileInfo.Name.Replace("." + extension, "");
            fileerrortype = -1;
            pageid = String.Empty;
            present = true;
            relative_directory = RelativeDirParam;

            // split the file name for sorting purposes
            splitName = SplitName ? new Builder_SplitFileName(filename) : null;
        }

        #endregion

        #region Public Properties

        /// <summary> Gets and sets the division tree node to which this page exists </summary>
        public Division_TreeNode METS_Division { get; set; }

        /// <summary> Page object to which this page file exists </summary>
        public Page_TreeNode METS_Page { get; set; }

        /// <summary> Gets the pageid linked to this page within the item </summary>
        public string PageID
        {
            get { return pageid; }
        }

        /// <summary> Primary key to the error type which may exist for this page file </summary>
        public int ErrorID
        {
            get { return fileerrortype; }
            set { fileerrortype = value; }
        }

        /// <summary> Gets the name splitter for this page file </summary>
        internal Builder_SplitFileName Sorter
        {
            get
            {
                // Make sure there is a value to return
                if (splitName != null)
                    return splitName;
                
                throw new ApplicationException("SplitFileName not configured!\n\nChange constructor");
            }
        }

        /// <summary> Gets and sets the flag that indicates the file is present </summary>
        public bool Present
        {
            get { return present; }
            set { present = value; }
        }

        /// <summary> Full name of this page file </summary>
        public string FullName
        {
            get { return filename + "." + extension; }
        }

        /// <summary> Filename (without extension) of this page file, used to reference all the other 
        /// related page files </summary>
        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary> Full name of this page file, including the relative directory of this file from the digital resource's
        /// main source directory </summary>
        public string FullName_With_Relative_Directory
        {
            get
            {
                if (relative_directory.Length == 0)
                    return filename + "." + extension;
                
                return relative_directory + "\\" + filename + "." + extension;
            }
        }

        /// <summary> Relative directory of this file from the digital resource's main source directory </summary>
        /// <remarks> Value of an empty string means this resides in the digital resource's main source directory</remarks>
        public string Relative_Directory
        {
            get { return relative_directory; }
            set { relative_directory = value; }
        }

        /// <summary> Primary extension of this page file </summary>
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        /// <summary> Primary key for this file within this digital resource </summary>
        public string FileID
        {
            get { return fileid; }
            set { fileid = value; }
        }

        /// <summary> Grants direct access to the bitmap which has the quality control image loaded </summary>
        public Bitmap Image
        {
            get { return thisImage; }
        }

        #endregion

        #region Methods which load the image, draw the file, and clear the image

        /// <summary> Loads the quality control thumbnail into a Bitmap object </summary>
        /// <param name="Directory"> Location where the files reside </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Load_Image(string Directory)
        {
            if (File.Exists(Directory + FileName + ".QC.jpg"))
            {
                try
                {
                    thisImage = (Bitmap) System.Drawing.Image.FromFile(Directory + FileName + ".QC.jpg");
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            
            return false;
        }

        /// <summary> Clears the loaded quality control thumbnail image from the Bitmap object when complete </summary>
        public void Clear_Loaded_Image()
        {
            if (thisImage != null)
            {
                thisImage.Dispose();
            }
        }

        /// <summary> Draws the current page with the quality control background and writes any existing
        /// error on the image </summary>
        /// <param name="G"> Graphics object used to draw upon </param>
        /// <param name="BorderPen"> Pen to use for the border and for the text </param>
        /// <param name="X"> X location within the container </param>
        /// <param name="Y"> Y location within the container</param>
        /// <param name="Scale"> Scale to draw the quality control thumbnail, used to control size</param>
        public void Draw(Graphics G, Pen BorderPen, int X, int Y, float Scale)
        {
            int currWidth = (int) (Scale*thisImage.Width);
            int currHeight = (int) (Scale*thisImage.Height);

            // Draw the image first
            G.DrawImage(thisImage, X, Y, currWidth, currHeight);

            // If there was any errors, show that now
            if (fileerrortype >= 0)
            {
                // Create the necessary objects to draw this error
                Brush errorBrush = new SolidBrush(Color.Tomato);
                int fontsize = (int) (45*Scale);
                Font errorFont = new Font("Tahoma", fontsize, FontStyle.Bold);

                if ((fileerrortype == 4) || (fileerrortype == 6))
                {
                    fontsize = (int) (30*Scale);
                    errorFont = new Font("Tahoma", fontsize, FontStyle.Bold);
                }

                if (fileerrortype == 3)
                {
                    fontsize = (int) (40*Scale);
                    errorFont = new Font("Tahoma", fontsize, FontStyle.Bold);
                }

                // Draw the text of the errors
                switch (fileerrortype)
                {
                    case 2:
                        G.DrawString(" CROP", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) - (fontsize*1.5)), currWidth, fontsize*2));
                        G.DrawString("ERROR", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) + (fontsize*0.5)), currWidth, fontsize*2));
                        break;

                    case 3:
                        G.DrawString("  IMAGE", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) - (fontsize*2.5)), currWidth, fontsize*2));
                        G.DrawString("QUALITY", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) - (fontsize*0.5)), currWidth, fontsize*2));
                        G.DrawString("  ERROR", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) + (fontsize*1.5)), currWidth, fontsize*2));
                        break;

                    case 4:
                        G.DrawString("ORIENTATION", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) - (fontsize*1.5)), currWidth, fontsize*2));
                        G.DrawString("  ERROR", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) + (fontsize*0.5)), currWidth, fontsize*2));
                        break;

                    case 5:
                        G.DrawString(" SKEW", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) - (fontsize*1.5)), currWidth, fontsize*2));
                        G.DrawString("ERROR", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) + (fontsize*0.5)), currWidth, fontsize*2));
                        break;

                    case 6:
                        G.DrawString("TECHNICAL", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) - (fontsize*2.5)), currWidth, fontsize*2));
                        G.DrawString("  SPEC", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) - (fontsize*0.5)), currWidth, fontsize*2));
                        G.DrawString("  ERROR", errorFont, errorBrush, new Rectangle(X + 10, (int) (Y + (currHeight/2) + (fontsize*1.5)), currWidth, fontsize*2));
                        break;
                }

                // Dispose of the created objects
                errorBrush.Dispose();
                errorFont.Dispose();
            }

            // Draw the border around the image as well
            G.DrawRectangle(BorderPen, X, Y, currWidth, currHeight);
        }

        #endregion

        #region IComparable Members

        /// <summary> Check to see if these two files are equivalent </summary>
        /// <param name="Obj"> Object to compare to </param>
        /// <returns> Returns -1, 0, or 1, depending on how this object ranks in comparison to the testing object </returns>
        public int CompareTo(object Obj)
        {
            // Check for null
            if (Obj == null)
                return 1;

            // Perform comparison
            int result = 0;

            var obj = Obj as Builder_Page_File;
            if (obj != null)
            {
                // Same object types, so cast
                Builder_Page_File compareToObj = obj;

                // Check for comparison between all the parts
                int maximum_part_checks = Math.Min(Sorter.NameParts.Length, compareToObj.Sorter.NameParts.Length);
                int i = 0;
                while ((i < maximum_part_checks) && (result == 0))
                {
                    // Only check for roman numerals in the first section
                    if (i == 0)
                        result = CompareStrings(Sorter.NameParts[i], compareToObj.Sorter.NameParts[i], true);
                    else
                        result = CompareStrings(Sorter.NameParts[i], compareToObj.Sorter.NameParts[i], false);

                    // Increment the counter to look at the next one
                    i++;
                }

                // If there was no result, the file with more parts should follow the one with less
                if ((result == 0) && (Sorter.NameParts.Length != compareToObj.Sorter.NameParts.Length))
                {
                    if (Sorter.NameParts.Length > compareToObj.Sorter.NameParts.Length)
                    {
                        return 1;
                    }
                    
                    return -1;
                }

                return result;
            }
            
            return 0;
        }

        #endregion

        #region Private methods serve CompareTo for implementing IComparable

        /// <summary>Compare two strings</summary>
        /// <param name="str1">First string to check</param>
        /// <param name="str2">Second string to check</param>
        /// <param name="checkRoman">Check for roman numberals?</param>
        /// <returns>0 if str1 = str2; -1 if str1 &lt; str2; 1 if str1 &gt; str2</returns>
        private int CompareStrings(string str1, string str2, bool checkRoman)
        {
            // If the two strings are the same, just return 0
            if (str1.ToUpper().CompareTo(str2.ToUpper()) == 0)
                return 0;

            // Get the type for each string
            File_String_Type firstType = Builder_General_Convert_Mill.Convert_FileString_To_Type(str1, checkRoman);
            File_String_Type secondType = Builder_General_Convert_Mill.Convert_FileString_To_Type(str2, checkRoman);

            // If different types, just return order
            if (firstType > secondType) return 1;
            if (firstType < secondType) return -1;

            // Are they both roman numerals?
            if (firstType == File_String_Type.ROMAN_NUMERALS)
            {
                if (Builder_General_Convert_Mill.Convert_Roman_To_Numbers(str1) < Builder_General_Convert_Mill.Convert_Roman_To_Numbers(str2))
                    return -1;
                else
                    return 1;
            }

            // Are they both numerals?
            if (firstType == File_String_Type.NUMBERS)
            {
                if (Convert.ToInt32(str1) < Convert.ToInt32(str2))
                    return -1;
                else
                    return 1;
            }

            // They are both letter-type strings (or mixed)

            // the str1 is greater than str2, return 1
            return str1.CompareTo(str2);
        }

        #endregion

        /// <summary> Check to see if these two files have the same name </summary>
        /// <param name="CompareTo"> Object to compare to </param>
        /// <returns>TRUE if they have the same name, otherwise FALSE </returns>
        public bool Same_Name(Builder_Page_File CompareTo)
        {
            return String.Compare(FullName, CompareTo.FullName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary> Check to see if these two files are equivalent </summary>
        /// <param name="CompareTo"> Object to compare to </param>
        /// <returns> TRUE if equal, otherwise FALSE </returns>
        public bool Equals(Builder_Page_File CompareTo)
        {
            // Must have same name, first of all
            return Same_Name(CompareTo);
        }
    }
}