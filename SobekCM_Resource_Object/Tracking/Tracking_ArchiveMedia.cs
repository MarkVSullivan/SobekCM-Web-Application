#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Tracking
{
    /// <summary> Class represents the intersection of a single media with locally archived files 
    /// ( i.e., a CD, DVD, etc.. ) and this individual digital resource </summary>
    [Serializable]
    public class Tracking_ArchiveMedia
    {
        /// <summary> Date this archive media was burned or originally archived </summary>
        public readonly DateTime Date_Burned;

        /// <summary> Range of files for this digital resource on the archive media </summary>
        public readonly string FileRange;

        /// <summary> Number of images ( usually just TIFF files ) for this digital resource on the archive media  </summary>
        public readonly int Images;

        /// <summary> Unique number identifying this archive media ( i.e., CD Number, DVD Number, etc.. ) </summary>
        public readonly string Media_Number;

        /// <summary> Overall size of the images for this digital resource on the archived media </summary>
        public readonly string Size;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Media_Number"> Unique number identifying this archive media ( i.e., CD Number, DVD Number, etc.. )</param>
        /// <param name="FileRange"> Range of files for this digital resource on the archive media</param>
        /// <param name="Images"> Number of images ( usually just TIFF files ) for this digital resource on the archive media</param>
        /// <param name="Size"> Overall size of the images for this digital resource on the archived media</param>
        /// <param name="Date_Burned"> Date this archive media was burned or originally archived</param>
        public Tracking_ArchiveMedia(string Media_Number, string FileRange, int Images, string Size, DateTime Date_Burned)
        {
            this.Media_Number = Media_Number;
            this.FileRange = FileRange;
            this.Images = Images;
            this.Size = Size;
            this.Date_Burned = Date_Burned;
        }
    }
}