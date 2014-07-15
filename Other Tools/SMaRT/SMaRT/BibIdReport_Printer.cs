#region Using directives

using System.Drawing;
using System.Drawing.Printing;

#endregion

namespace SobekCM.Management_Tool
{
	/// <summary> BibIdReport_Printer is the class which is used to print 
    /// the tracking-style report for an item group (title) or a single item (volume). </summary>
	public class BibIdReport_Printer
	{
	    private const int leftMargin = 30;
        private const int topMargin = 30;
        private const int rightMargin = 770;

	    private const string fontFace = "Aerial";
	    private readonly string aleph;
	    private readonly string author;
	    private readonly string bibID;
	    private readonly string groupTitle;
        private readonly string level1Text, level2Text, level3Text;
        private readonly string materialType;
        private readonly string oclc;
        private readonly string publisher;
        private readonly string title;
        private readonly string vid;
	    private readonly Font bibFont = new Font("Tahoma", 35F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
	    private readonly Font boldFont = new Font(fontFace, 9F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
	    private readonly Font largerFont = new Font(fontFace, 12F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
	    private PrintDocument printDocument1;
	    private readonly Font regFont = new Font(fontFace, 9F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
	    private readonly Brush textBrush = new SolidBrush(Color.Black);
	    private readonly Pen textPen = new Pen(Color.Black, 0.1F);


        /// <summary> Constructor for a new instance of the BibIdReport_Printer class </summary>
        /// <param name="printDocument1"> Print document to use to print the report for a particular item or title </param>
        /// <param name="BibID"> </param>
	    /// <param name="VID"></param>
	    /// <param name="GroupTitle"></param>
	    /// <param name="Title"></param>
	    /// <param name="Author"></param>
	    /// <param name="Publisher"></param>
	    /// <param name="MaterialType"></param>
	    /// <param name="ALEPH"></param>
	    /// <param name="OCLC"></param>
	    /// <param name="Level1_Text"></param>
	    /// <param name="Level2_Text"></param>
	    /// <param name="Level3_Text"></param>
	    public BibIdReport_Printer(PrintDocument printDocument1, string BibID, string VID, string GroupTitle, string Title, string Author, string Publisher, string MaterialType, string ALEPH, string OCLC, string Level1_Text, string Level2_Text, string Level3_Text)
		{
			// Save the parameters
			this.printDocument1 = printDocument1;
            bibID = BibID;
            vid = VID;
            groupTitle = GroupTitle;
            title = Title;
            author = Author;
            publisher = Publisher;
            materialType = MaterialType;
            aleph = ALEPH;
            oclc = OCLC;
            level1Text = Level1_Text;
            level2Text = Level2_Text;
            level3Text = Level3_Text;
        }

        /// <summary> Print the report for this title </summary>
        /// <param name="g"></param>
        public void Print_Title_Report(Graphics g )
        {
            // Draw the BIB ID very large across the top
            if (vid.Length > 0)
            {
                g.DrawString(bibID + " : " + vid, bibFont, textBrush, new Point(leftMargin, topMargin));
            }
            else
            {
                g.DrawString(bibID, bibFont, textBrush, new Point(leftMargin, topMargin));
            }

            // Draw the text for the material information section
            g.DrawString("Material Information", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 80));

            // Variables to define where to draw this block, etc..
            const int LINE_HEIGHT = 25;
            const int BLOCK_LEFT_START = leftMargin + 25;
            int block_top_start = topMargin + 110;
            int lineNum = 0;
            int volumeBuffer = 0;

            // Print the block of data for the tracking information
            if (title.Length == 0)
            {
                print_information_line(g, "Group Title:", groupTitle, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT), rightMargin, 100);
            }
            else
            {
                print_information_line(g, "Title:", title, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT), rightMargin, 100);
            }

            print_information_line(g, "Author: ", author, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT), rightMargin, 60);
            //   print_information_line(g, "Project Code(s):", thisBib.ProjectCodes.ToString(), rightMargin - 260, block_top_start + ((lineNum++) * lineHeight), rightMargin, 60);
            print_information_line(g, "Publisher:", publisher, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT), rightMargin, 100);
            //    print_information_line(g, "Holding Location:", thisBib.HoldingLocation.Display_Text, block_left_start, block_top_start + ((lineNum++) * lineHeight), rightMargin, 100);
            print_information_line(g, "OCLC: ", oclc, BLOCK_LEFT_START, block_top_start + ((lineNum) * LINE_HEIGHT), 230, 60);
            print_information_line(g, "Aleph: ", aleph, BLOCK_LEFT_START + 280, block_top_start + ((lineNum) * LINE_HEIGHT), BLOCK_LEFT_START + 430, 60);
            print_information_line(g, "Material Type:", materialType, BLOCK_LEFT_START + 450, block_top_start + ((lineNum) * LINE_HEIGHT), rightMargin, 60);

            // If there is volume information here, try to display it
            if (((level1Text.Length > 0) && (level1Text != title)) || (level2Text.Length > 0) || (level3Text.Length > 0))
            {
                // Set the new block_start
                block_top_start = topMargin + 300;
                lineNum = 0;

                // enumeration fields
                print_information_line(g, "Level 1:  ", level1Text, BLOCK_LEFT_START, block_top_start + ((lineNum) * LINE_HEIGHT), BLOCK_LEFT_START + 180, 60);
                print_information_line(g, "Level 2:  ", level2Text, BLOCK_LEFT_START + 200, block_top_start + ((lineNum) * LINE_HEIGHT), BLOCK_LEFT_START + 380, 60);
                print_information_line(g, "Level 3: ", level3Text, BLOCK_LEFT_START + 400, block_top_start + ((lineNum) * LINE_HEIGHT), BLOCK_LEFT_START + 580, 60);

                // Draw the text for Volume Information
                g.DrawString("Serial Hierarchy", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 270));
                volumeBuffer = 100;
            }

            // Draw the text for Scanning Information
            g.DrawString("Imaging Progress", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 550 + volumeBuffer));

            // Variables
            block_top_start = topMargin + 580 + volumeBuffer;
            lineNum = 0;

            // Print the block of data
            print_imaging_progress_line(g, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT));
            print_imaging_progress_line(g, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT));
            print_imaging_progress_line(g, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT));
            print_imaging_progress_line(g, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT));

            if (volumeBuffer == 0 )
            {
                print_imaging_progress_line(g, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT));
                print_imaging_progress_line(g, BLOCK_LEFT_START, block_top_start + ((lineNum++) * LINE_HEIGHT));
                print_imaging_progress_line(g, BLOCK_LEFT_START, block_top_start + ((lineNum) * LINE_HEIGHT));
            }
            else
            {
                volumeBuffer -= (3 * LINE_HEIGHT);
                if (volumeBuffer < 0)
                    volumeBuffer = 0;
            }

            // Draw the text for Additional Notes
            g.DrawString("Additional Notes", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 820 + volumeBuffer));

            // Variables
            block_top_start = topMargin + 850 + volumeBuffer;
            lineNum = 0;

            // Print the block of data
            g.DrawLine(textPen, BLOCK_LEFT_START, block_top_start + ((lineNum) * LINE_HEIGHT) + 14, rightMargin, block_top_start + ((lineNum++) * LINE_HEIGHT) + 14);
            g.DrawLine(textPen, BLOCK_LEFT_START, block_top_start + ((lineNum) * LINE_HEIGHT) + 14, rightMargin, block_top_start + ((lineNum++) * LINE_HEIGHT) + 14);
            g.DrawLine(textPen, BLOCK_LEFT_START, block_top_start + ((lineNum) * LINE_HEIGHT) + 14, rightMargin, block_top_start + ((lineNum++) * LINE_HEIGHT) + 14);
            g.DrawLine(textPen, BLOCK_LEFT_START, block_top_start + ((lineNum) * LINE_HEIGHT) + 14, rightMargin, block_top_start + ((lineNum) * LINE_HEIGHT) + 14);
        }

        //public void Print_Volume_Report(Graphics g)
        //{
        //    // Draw the BIB ID very large across the top
        //    if (volume_id == String.Empty)
        //    {
        //        g.DrawString(thisBib.BibID, bibFont, textBrush, new Point(leftMargin, topMargin));
        //    }
        //    else
        //    {
        //        g.DrawString(thisBib.BibID + " / " + volume_id, bibFont, textBrush, new Point(leftMargin, topMargin));
        //    }

        //    // Draw the text for the tracking information section
        //    g.DrawString("Tracking Information", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 80));

        //    // Variables to define where to draw this block, etc..
        //    int lineHeight = 25;
        //    int block_left_start = leftMargin + 25;
        //    int block_top_start = topMargin + 110;
        //    int lineNum = 0;
        //    int volumeBuffer = 0;

        //    // Print the block of data for the tracking information
        //    print_information_line(g, "Title:", thisBib.Title, block_left_start, block_top_start + ((lineNum++) * lineHeight), rightMargin, 100);
        //    print_information_line(g, "Author: ", thisBib.Author, block_left_start, block_top_start + ((lineNum) * lineHeight), rightMargin - 280, 60);
        //    print_information_line(g, "Project Code(s):", thisBib.ProjectCodes.ToString(), rightMargin - 260, block_top_start + ((lineNum++) * lineHeight), rightMargin, 60);
        //    print_information_line(g, "Publisher:", thisBib.Publisher.Name, block_left_start, block_top_start + ((lineNum++) * lineHeight), rightMargin, 100);
        //    print_information_line(g, "Holding Location:", thisBib.HoldingLocation.Display_Text, block_left_start, block_top_start + ((lineNum++) * lineHeight), rightMargin, 100);
        //    print_information_line(g, "Aleph: ", thisBib.AlephBibNumber, block_left_start + 280, block_top_start + ((lineNum) * lineHeight), block_left_start + 430, 60);
        //    print_information_line(g, "Material Type:", thisBib.Bibliographic_Type_String, block_left_start + 450, block_top_start + ((lineNum++) * lineHeight), rightMargin, 60);

        //    // If there is volume information here, try to display it
        //    if ((volume_id != String.Empty) && (thisBib.Volumes.Count > 0))
        //    {
        //        // Try to find this volume
        //        SingleVolume thisVolume = thisBib.Volumes[volume_id];
        //        if (thisVolume != null)
        //        {
        //            // Set the new block_start
        //            block_top_start = topMargin + 300;
        //            lineNum = 0;

        //            // show volume title, enumeration, and chronology fields

        //            // volume title
        //            print_information_line(g, "Volume Title: ", thisVolume.VolumeTitle, block_left_start, block_top_start + ((lineNum++) * lineHeight), block_left_start + 580, 100);

        //            // enumeration fields
        //            print_information_line(g, "Volume:  ", thisVolume.Enumeration1, block_left_start, block_top_start + ((lineNum) * lineHeight), block_left_start + 180, 60);
        //            print_information_line(g, "Issue:  ", thisVolume.Enumeration2, block_left_start + 200, block_top_start + ((lineNum) * lineHeight), block_left_start + 380, 60);
        //            print_information_line(g, "Part: ", thisVolume.Enumeration3, block_left_start + 400, block_top_start + ((lineNum++) * lineHeight), block_left_start + 580, 60);

        //            // chronology fields
        //            print_information_line(g, "Year:  ", thisVolume.Chronology1, block_left_start, block_top_start + ((lineNum) * lineHeight), block_left_start + 180, 60);
        //            print_information_line(g, "Month:  ", thisVolume.Chronology2, block_left_start + 200, block_top_start + ((lineNum) * lineHeight), block_left_start + 380, 60);
        //            print_information_line(g, "Day: ", thisVolume.Chronology3, block_left_start + 400, block_top_start + ((lineNum++) * lineHeight), block_left_start + 580, 60);

        //            // set the volumeBuffer value to the current "Y" point
        //            volumeBuffer = lineHeight;

        //            // If some volume information was shown, show the title
        //            if (volumeBuffer > 0)
        //            {
        //                // Draw the text for Volume Information
        //                g.DrawString("Volume Information", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 270));
        //                //volumeBuffer += 60;
        //                volumeBuffer += 100;
        //            }
        //        }
        //    }
        //    lineNum++;

        //    // Draw the text for Material Assessment
        //    g.DrawString("Material Assessment", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 270 + volumeBuffer));

        //    // Variables
        //    block_top_start = topMargin + 300 + volumeBuffer;
        //    lineNum = 0;

        //    // Print the block of data
        //    print_information_line(g, "Condition upon receipt:", "Excellent       Good       Fair       Fragile", block_left_start, block_top_start + ((lineNum++) * lineHeight), 500, 100);
        //    print_information_line(g, "Condition upon return: ", "Excellent       Good       Fair       Fragile", block_left_start, block_top_start + ((lineNum++) * lineHeight), 500, 100);
        //    print_information_line(g, "Comments:  ", " ", block_left_start, block_top_start + ((lineNum++) * lineHeight), rightMargin, 100);

        //    // Draw the text for Scanning Information
        //    g.DrawString("Image Capture Information", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 410 + volumeBuffer));

        //    // Variables
        //    block_top_start = topMargin + 440 + volumeBuffer;
        //    lineNum = 0;

        //    // Print the block of data
        //    print_information_line(g, "Color Specifications:", "RGB       256 Color       Grayscale       B&W", block_left_start, block_top_start + ((lineNum++) * lineHeight), 500, 100);
        //    print_information_line(g, "PPI for Text:", " ", block_left_start, block_top_start + ((lineNum) * lineHeight), block_left_start + 200, 100);
        //    print_information_line(g, "PPI for Maps/Plates:", " ", block_left_start + 240, block_top_start + ((lineNum++) * lineHeight), block_left_start + 510, 100);
        //    print_information_line(g, "Scanning Technology:  ", "Flatbeds          High-Speed           Digital Camera           Microfilm Camera", block_left_start, block_top_start + ((lineNum++) * lineHeight), block_left_start + 600, 100);

        //    // Draw the text for Scanning Information
        //    g.DrawString("Imaging Progress", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 550 + volumeBuffer));

        //    // Variables
        //    block_top_start = topMargin + 580 + volumeBuffer;
        //    lineNum = 0;

        //    // Print the block of data
        //    print_information_line(g, "Hard Drive: ", " ", block_left_start, block_top_start + ((lineNum++) * lineHeight), block_left_start + 340, 100);
        //    print_imaging_progress_line(g, block_left_start, block_top_start + ((lineNum++) * lineHeight));
        //    print_imaging_progress_line(g, block_left_start, block_top_start + ((lineNum++) * lineHeight));
        //    print_imaging_progress_line(g, block_left_start, block_top_start + ((lineNum++) * lineHeight));
        //    print_imaging_progress_line(g, block_left_start, block_top_start + ((lineNum++) * lineHeight));

        //    if (volumeBuffer == 0)
        //    {
        //        print_imaging_progress_line(g, block_left_start, block_top_start + ((lineNum++) * lineHeight));
        //        print_imaging_progress_line(g, block_left_start, block_top_start + ((lineNum++) * lineHeight));
        //        print_imaging_progress_line(g, block_left_start, block_top_start + ((lineNum++) * lineHeight));
        //    }
        //    else
        //    {
        //        volumeBuffer -= (3 * lineHeight);
        //        if (volumeBuffer < 0)
        //            volumeBuffer = 0;
        //    }

        //    // Draw the text for Additional Notes
        //    g.DrawString("Additional Notes", largerFont, textBrush, new Point(leftMargin + 4, topMargin + 820 + volumeBuffer));

        //    // Variables
        //    block_top_start = topMargin + 850 + volumeBuffer;
        //    lineNum = 0;

        //    // Print the block of data
        //    g.DrawLine(textPen, block_left_start, block_top_start + ((lineNum) * lineHeight) + 14, rightMargin, block_top_start + ((lineNum++) * lineHeight) + 14);
        //    g.DrawLine(textPen, block_left_start, block_top_start + ((lineNum) * lineHeight) + 14, rightMargin, block_top_start + ((lineNum++) * lineHeight) + 14);
        //    g.DrawLine(textPen, block_left_start, block_top_start + ((lineNum) * lineHeight) + 14, rightMargin, block_top_start + ((lineNum++) * lineHeight) + 14);
        //    g.DrawLine(textPen, block_left_start, block_top_start + ((lineNum) * lineHeight) + 14, rightMargin, block_top_start + ((lineNum++) * lineHeight) + 14);
        //}


		private void print_imaging_progress_line( Graphics g, int x, int y )
		{
			print_information_line( g, "Name:   ", " ", x, y, x + 340, 100 );
			print_information_line( g, "Date: ", "    /     /", x + 360, y, x + 480, 100 );
			print_information_line( g, "Last Page Scanned: ", " ", x+500, y, rightMargin, 100 );
		}



		private void print_information_line( Graphics g, string infoTitle, string info, int x, int y, int length, int chars )
		{
			// Determine the indent
			int indent = infoTitle.Length * 7;

			// Determine the data string
			string data = info;
			if ( data.Length > chars )
				data = info.Substring(0, chars ) + "...";

			// Draw the data
			g.DrawString( infoTitle, boldFont, textBrush, new Point( x, y ));
			g.DrawString( data, regFont, textBrush, new Point( x + indent, y ));
			g.DrawLine( textPen, x + indent - 2, y + 14, length, y + 14 );
		}
	}
}
