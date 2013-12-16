using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using UFDC_Bib_Package;
using DLC.MetaTemplate.Template;

namespace DLC.MetaTemplate.Elements
{
    /// <summary>
    /// Summary description for fileChooser_Element.
    /// </summary>
    public abstract class fileChooser_Element2 : abstract_Element
    {
        protected System.Windows.Forms.ListBox fileList;
        protected string mets_directory;
        protected int lines;
        protected string searchPattern;
        protected bool show_filter_options;
        private static bool xp_theme;
        protected Hashtable file_labels;

        public fileChooser_Element2( int lines )
        {
            file_labels = new Hashtable();
            this.lines = lines;

            fileList = new ListBox();
            fileList.Sorted = true;
            fileList.MultiColumn = true;
            fileList.Location = new Point(115, 5);
            fileList.Size = new Size(200, lines * 22);
            fileList.Cursor = Cursors.Hand;
            fileList.Click +=new EventHandler(fileList_Click);
            fileList.SelectedIndexChanged += new EventHandler(fileList_SelectedIndexChanged);
            fileList.ForeColor = System.Drawing.Color.MediumBlue;
            fileList.Enter += new EventHandler(fileList_Enter);
            fileList.Leave += new EventHandler(fileList_Leave);
            this.Controls.Add(fileList);

            // Set default title to blank
            title = "no default";
            searchPattern = "*.*";
            show_filter_options = true;

            if (!DLC.Tools.Windows_Appearance_Checker.is_XP_Theme)
            {
                fileList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
        }


        public fileChooser_Element2(int lines, string defaultTitle)
        {
            file_labels = new Hashtable();
            this.lines = lines;

            fileList = new ListBox();
            fileList.Sorted = true;
            fileList.MultiColumn = true;
            fileList.Location = new Point(115, 5);
            fileList.Size = new Size(200, lines * 22);
            fileList.Cursor = Cursors.Hand;
            fileList.Click += new EventHandler(fileList_Click);
            fileList.SelectedIndexChanged += new EventHandler(fileList_SelectedIndexChanged);
            fileList.ForeColor = System.Drawing.Color.MediumBlue;
            fileList.Enter += new EventHandler(fileList_Enter);
            fileList.Leave += new EventHandler(fileList_Leave);
            this.Controls.Add(fileList);

            // Save the title
            title = defaultTitle;
            searchPattern = "*.*";
            show_filter_options = true;

            if (!DLC.Tools.Windows_Appearance_Checker.is_XP_Theme)
            {
                fileList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
        }

        void fileList_Leave(object sender, EventArgs e)
        {
            if (!read_only)
            {
                fileList.BackColor = Color.White;
            }
        }

        void fileList_Enter(object sender, EventArgs e)
        {
            if (!read_only)
            {
                fileList.BackColor = Color.Khaki;
            }
        }

        public static bool XP_Theme
        {
            set
            {
                xp_theme = value;
            }
        }

        void fileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            fileList.SelectedItem = null;
        }


        void fileList_Click(object sender, EventArgs e)
        {
            // If readonly, do nothing
            if (read_only)
                return;

            // Get the files in the diretcory
            if (mets_directory.IndexOf("http:") >= 0)
            {
                MessageBox.Show("You cannot modify this list when updating from UFDC.     ","UFDC Update", MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }

            // Does the file exists?
            if ( !Directory.Exists( mets_directory ))
            {
                MessageBox.Show("METS directory no longer exists!     ", "METS Directory Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            string[] allFiles = Directory.GetFiles(mets_directory, searchPattern );
            StringCollection possibleFiles = new StringCollection();
            foreach (string thisFile in allFiles)
            {
                string upperFile = thisFile.ToUpper();
                if ((upperFile.IndexOf(".TXT") < 0) && (upperFile.IndexOf(".PRO") < 0) && (upperFile.IndexOf(".XML") < 0) && (upperFile.IndexOf(".METS") < 0))
                {
                    possibleFiles.Add((new FileInfo(thisFile)).Name);
                }
            }

            // Also create the collection of files currently included
            StringCollection includedFiles = new StringCollection();
            foreach (string thisFile in fileList.Items)
            {
                includedFiles.Add(thisFile);
                if (!possibleFiles.Contains(thisFile))
                    possibleFiles.Add(thisFile);
            }

            // Show the selection form
            MetaTemplate.Forms.Download_Label_Form select = new MetaTemplate.Forms.Download_Label_Form(possibleFiles, includedFiles, show_filter_options, xp_theme, file_labels);
            DialogResult results = select.ShowDialog();

            // Get the new selections
            if (results == DialogResult.OK)
            {
                bool newItems = false;
                fileList.Items.Clear();
                foreach (string newFile in select.Checked_Files)
                {
                    fileList.Items.Add(newFile);
                    if (!includedFiles.Contains(newFile))
                        newItems = true;
                }

                if (!newItems)
                {
                    if (includedFiles.Count != select.Checked_Files.Length)
                        newItems = true;
                }

                if (newItems)
                {
                    OnDataChanged();
                }
            }


        }

        /// <summary> Override the OnPaint method to draw the title before the text box </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the title
            Draw_Title(e.Graphics, title);

            // Determine the y-mid-point
            int midpoint = (int)(1.5 * this.Font.SizeInPoints);

            // If this is repeatable, show the '+' to add another after this one
            Draw_Repeatable_Icon(e.Graphics, this.Width - 22, midpoint - 6);

            // Call this for the base
            OnPaint(e);
        }

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Writes the inner data into Template XML format </summary>
        protected override string Inner_Write_Data()
        {
            return String.Empty;
        }

        /// <summary> Reads the inner data from the Template XML format </summary>
        protected override void Inner_Read_Data(XmlTextReader xmlReader)
        {

        }

        /// <summary> Perform any height setting calculations specific to the 
        /// implementation of abstract_Element.  </summary>
        /// <param name="size"> Height of the font </param>
        protected override void Inner_Set_Height(float size)
        {
            // Set total height
            int size_int = (int)size;
            this.Height = lines * (size_int + 14) + 4;
        }

        /// <summary> Perform any width setting calculations specific to the 
        /// implementation of abstract_Element.  </summary>
        /// <param name="size"> Height of the font </param>
        protected override void Inner_Set_Width(int new_width)
        {
            // Set the width of the text box
            int width = new_width - title_length - 30;
            if (width > 350)
                width = 350;
            fileList.Width = width;
            
            fileList.Location = new Point(title_length + 5, fileList.Location.Y);
        }

        /// <summary> Perform any readonly functions specific to the
        /// implementation of abstract_Element. </summary>
        protected override void Inner_Set_Read_Only()
        {
            if (read_only)
            {
                
            }
            else
            {
    
            }
        }

        /// <summary> Clones this element, not copying the actual data
        /// in the fields, but all other values. </summary>
        /// <returns>Clone of this element</returns>
        public override abstract_Element Clone()
        {
            // Get the new element
            fileChooser_Element2 newElement = (fileChooser_Element2)Element_Factory.getElement(this.Type, this.Display_SubType);
            newElement.Location = this.Location;
            newElement.Language = this.Language;
            newElement.Title_Length = this.Title_Length;
            newElement.Height = this.Height;
            newElement.Font = this.Font;
            newElement.Set_Width(this.Width);
            newElement.Index = this.Index + 1;

            return newElement;
        }

        /// <summary> Gets the flag indicating this element has an entered value </summary>
        public override bool hasValue
        {
            get
            {
                return true;
            }
        }

        /// <summary> Checks the data in this element for validity. </summary>
        /// <returns> TRUE if valid, otherwise FALSE </returns>
        /// <remarks> This sets the <see cref="abstract_Element.Invalid_String" /> value. </remarks>
        public override bool isValid()
        {
            return true;
        }

        #endregion

    }
}
