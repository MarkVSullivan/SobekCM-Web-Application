#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Stores the serial hierarchy information associated with this resource </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Serial_Info
    {
        private SortedList<int, Single_Serial_Hierarchy> hierarchy;

        /// <summary> Constructor creates a new instance of the Serial_Info class </summary>
        public Serial_Info()
        {
            // Do nothing
        }

        /// <summary> Gets the number of hierarchies in this serial hierarchy </summary>
        public int Count
        {
            get
            {
                if (hierarchy == null)
                    return 0;
                else
                    return hierarchy.Count;
            }
        }

        /// <summary> Address a single hierarchy from this collection, by index </summary>
        /// <exception cref="Exception"> Throws a <see cref="Exception"/> if the hierarchy requested does not exist. </exception>
        public Single_Serial_Hierarchy this[int index]
        {
            get
            {
                // Check that this node exists exists
                if ((hierarchy == null) || (index >= hierarchy.Count) || (index < 0))
                    throw new Exception("Requested serial hierarchy #" + index + " and this serial hierarchy does not exist.");

                // Return the requested hierarchy file
                return hierarchy.Values[index];
            }
        }

        /// <summary> Clear all the existing hierarchical information </summary>
        public void Clear()
        {
            if (hierarchy != null)
                hierarchy.Clear();
        }

        /// <summary> Add a new level to the serial hierarchy associated with this resource </summary>
        /// <param name="Level">Serial hierarchy level</param>
        /// <param name="Order">Order for this to display along with other items in the same level</param>
        /// <param name="Display">Text to display for this hierarchical level</param>
        public void Add_Hierarchy(int Level, int Order, string Display)
        {
            if (hierarchy == null)
                hierarchy = new SortedList<int, Single_Serial_Hierarchy>();

            if (!hierarchy.ContainsKey(Level))
            {
                hierarchy.Add(Level, new Single_Serial_Hierarchy(Order, Display));
            }
        }

        /// <summary> Returns the METS formatted XML string for the serial hierarchy information </summary>
        /// <param name="sobekcm_namespace">METS extension schema namespace to use</param>
        /// <param name="results">Results stream to write the METS-encoded serial information </param>
        /// <returns>METS formatted XML string with the serial hierarchy information</returns>
        internal void Add_METS(string sobekcm_namespace, TextWriter results)
        {
            if ((hierarchy == null) || (hierarchy.Count == 0))
            {
                return;
            }

            results.Write("<" + sobekcm_namespace + ":serial>\r\n");
            for (int i = 1; i <= Count; i++)
            {
                results.Write(hierarchy.Values[i - 1].toMETS(i, sobekcm_namespace) + "\r\n");
            }
            results.Write("</" + sobekcm_namespace + ":serial>\r\n");
        }

        #region Nested type: Single_Serial_Hierarchy

        /// <summary> Single bit of serial hierarchy data to be associated with this resource </summary>
        /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
        [Serializable]
        public class Single_Serial_Hierarchy : XML_Writing_Base_Type
        {
            private string display;
            private int order;

            /// <summary> Constructor creates a new instance of the Single_Serial_Hierarchy class </summary>
            /// <param name="Order">Order for this to display along with other items in the same level</param>
            /// <param name="Display">Text to display for this hierarchical level</param>
            public Single_Serial_Hierarchy(int Order, string Display)
            {
                order = Order;
                display = Display;
            }

            /// <summary> Gets and sets the order for this to display along with other items in the same level </summary>
            public int Order
            {
                get { return order; }
                set { order = value; }
            }

            /// <summary> Gets or sets the text to display for this hierarchical level </summary>
            public string Display
            {
                get { return display; }
                set { display = value; }
            }

            internal string Display_XML
            {
                get { return base.Convert_String_To_XML_Safe(display); }
            }

            /// <summary> Returns this single serial hierarchy in METS formatted XML </summary>
            /// <param name="level">Serial hierarchy level for this element</param>
            /// <param name="myNamespace">METS extension schema namespace to use</param>
            /// <returns>METS formatted XML for this single serial hierarchical data</returns>
            internal string toMETS(int level, string myNamespace)
            {
                return "<" + myNamespace + ":SerialHierarchy level=\"" + level + "\" order=\"" + order + "\">" + base.Convert_String_To_XML_Safe(display) + "</" + myNamespace + ":SerialHierarchy>";
            }
        }

        #endregion
    }
}