#region Using directives

using System;
using System.Collections;

#endregion

namespace SobekCM_Resource_Database.Builder
{
    /// <summary>
    /// Summary description for Page_File_Collection.
    /// </summary>
    public class Builder_Page_File_Collection : CollectionBase, IEnumerable
    {
        /// <summary> Constructor for the Page_File_Collection object. </summary>
        internal Builder_Page_File_Collection()
        {
            // Empty constructor
        }

        /// <summary> Address a single Page_File from this Collection, by the node id. </summary>
        /// <exception cref="Exception"> Throws a <see cref="Exception"/> if there is 
        /// an Page_File requested which does not exist. </exception>
        internal Builder_Page_File this[int Index]
        {
            get
            {
                // Check that this node exists exists
                if ((Index >= List.Count) || (Index < 0))
                    throw new Exception("Requested Page_File #" + Index + " and this Page_File does not exist.");

                // Return the requested MXF Page_File
                return ((Builder_Page_File) (List[Index]));
            }
            set { List[Index] = value; }
        }

        #region IEnumerable Members

        /// <summary> Return an enumerator to step through this collection of tree nodes. </summary>
        /// <returns> A IEnumerator object to step through this collection of tree nodes. </returns>
        /// <remarks> Explicit interface implementation to support interoperability with other common 
        /// language runtime-compatible langueages. </remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Builder_Page_File_Enumerator(this);
        }

        #endregion

        /// <summary> Add a new Page_File to this collection. </summary>
        /// <param name="NewPageFile"> <see cref="Builder_Page_File"/> object for this new Page_File </param>
        internal void Add(Builder_Page_File NewPageFile)
        {
            // Add this constructed Page_File to the collection
            List.Add(NewPageFile);
        }

        /// <summary> Insert a new page into this collection, based on the file name </summary>
        /// <param name="NewPageFile"></param>
        /// <returns></returns>
        internal int Insert(Builder_Page_File NewPageFile)
        {
            if (List.Count == 0)
            {
                return List.Add(NewPageFile);
            }

            if (NewPageFile.CompareTo(this[0]) <= 0)
            {
                List.Insert(0, NewPageFile);
                return 0;
            }

            if (NewPageFile.CompareTo(this[List.Count - 1]) >= 0)
            {
                return List.Add(NewPageFile);
            }

            return Recursive_Sort_Insert(NewPageFile, 0, List.Count - 1);
        }

        internal int Recursive_Sort_Insert(Builder_Page_File NewPageFile, int Start, int End)
        {
            int startIndex = Start;
            int endIndex = End;
            int midIndex = (int) Math.Ceiling((double) (endIndex - startIndex)/2) + startIndex;
            if (endIndex - startIndex <= 1)
            {
                if (NewPageFile.CompareTo(this[startIndex]) <= 0)
                {
                    List.Insert(startIndex, NewPageFile);
                    return startIndex;
                }
                
                List.Insert(endIndex, NewPageFile);
                return endIndex;
            }
            
            if (NewPageFile.CompareTo(this[midIndex]) < 0)
                endIndex = midIndex;
            else
                startIndex = midIndex;

            return Recursive_Sort_Insert(NewPageFile, startIndex, endIndex);
        }

        /// <summary> Remove an existing Page_File under from this collection. </summary>
        /// <param name="Nodeid"> node id for this workflow </param>
        /// <exception cref="Exception"> Throws a <see cref="Exception"/> if there is no
        /// an Page_File which matches the Page_File requested to be removed. </exception>
        internal void Remove(int Nodeid)
        {
            // Check that this BaseFileName exists
            if (!Contains(Nodeid))
                throw new Exception("No Page_File '" + Nodeid + "' exists for this division");

            // Remove this Page_File, since it existed
            List.Remove(Nodeid);
        }

        /// <summary> Check to see if there is a Page_File in this collection already </summary>
        /// <param name="Nodeid"> node id for this workflow </param>
        /// <returns>TRUE if the provided work flow id is already part of this Page_File Collection </returns>
        internal bool Contains(int Nodeid)
        {
            return List.Contains(Nodeid);
        }

        /// <summary> Return an enumerator to step through this collection of tree nodes. </summary>
        /// <returns> A Type-Safe MXF_IncludedFileEnumerator</returns>
        /// <remarks> This version is used in the C# Compiler to detect type conflicts at compilation. </remarks>
        public new Builder_Page_File_Enumerator GetEnumerator()
        {
            return new Builder_Page_File_Enumerator(this);
        }

        #region Nested type: Builder_Page_File_Enumerator

        /// <summary> Inner class implements the <see cref="IEnumerator"/> interface and iterates through 
        /// the <see cref="Builder_Page_File_Collection"/> object composed of <see cref="Builder_Page_File"/> objects 
        /// for this volume. <br/> <br/> </summary>
        /// <remarks> Inclusion of this strongly-typed iterator allows the use of the foreach .. in structure to 
        /// iterate through all of the <see cref="Builder_Page_File"/> objects in the 
        /// <see cref="Builder_Page_File_Collection"/> object. The example in the <see cref="Builder_Page_File_Collection"/>
        /// demonstrates this use.</remarks>
        public class Builder_Page_File_Enumerator : IEnumerator
        {
            /// <summary> Reference to the <see cref="Builder_Page_File_Collection"/> to iterate through. </summary>
            private readonly Builder_Page_File_Collection nodes;

            /// <summary> Stores position for this enumerator </summary>
            private int position = -1;

            /// <summary> Constructore creates a new Page_File_Enumerator to iterate through
            /// the <see cref="Builder_Page_File_Collection"/>. </summary>
            /// <param name="NodeCollection"> Collection of nodes </param>
            internal Builder_Page_File_Enumerator(Builder_Page_File_Collection NodeCollection)
            {
                nodes = NodeCollection;
            }

            /// <summary> Return the current <see cref="Builder_Page_File"/> from the <see cref="Builder_Page_File_Collection"/>. </summary>
            /// <remarks> This type-safe version is used in the C# Compiler to detect type conflicts at compilation. </remarks>
            public Builder_Page_File Current
            {
                get { return nodes[position]; }
            }

            #region IEnumerator Members

            /// <summary> Move to the next <see cref="Builder_Page_File"/> in this <see cref="Builder_Page_File_Collection"/>. </summary>
            /// <returns> TRUE if successful, otherwise FALSE </returns>
            /// <remarks> Method is required by the IEnumerator interface. </remarks>
            public bool MoveNext()
            {
                if (position < (nodes.Count - 1))
                {
                    position++;
                    return true;
                }
                
                return false;
            }

            /// <summary> Reset to the position just before the first position.  
            /// Ready for the MoveNext() method to be called. </summary>
            /// <remarks> Method is required by the IEnumerator interface. </remarks>
            public void Reset()
            {
                position = -1;
            }

            /// <summary> Return the current object from the <see cref="Builder_Page_File_Collection"/>. </summary>
            /// <remarks> Explicit interface implementation to support interoperability with other common 
            /// language runtime-compatible langueages. </remarks>
            object IEnumerator.Current
            {
                get { return nodes[position]; }
            }

            #endregion
        }

        #endregion
    }
}