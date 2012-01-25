using System;
using System.Collections;

namespace SobekCM.Bib_Package.Builder
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
		internal Builder_Page_File this[ int index ]
		{
			get	
			{	
				// Check that this node exists exists
				if ( ( index >= List.Count ) || ( index < 0) )
					throw new Exception("Requested Page_File #" + index  + " and this Page_File does not exist.");

				// Return the requested MXF Page_File
				return ((Builder_Page_File) (List[index]));	
			}
			set	{	List[index] = value;	}
		}

		/// <summary> Add a new Page_File to this collection. </summary>
		/// <param name="newPage_File"> <see cref="Builder_Page_File"/> object for this new Page_File </param>
		internal void Add( Builder_Page_File newPage_File )
		{
			// Add this constructed Page_File to the collection
			List.Add( newPage_File );
		}

		/// <summary> Insert a new page into this collection, based on the file name </summary>
		/// <param name="newPage_File"></param>
		/// <returns></returns>
		internal int Insert( Builder_Page_File newPage_File )
		{
			if ( List.Count == 0 )
			{
				return List.Add( newPage_File );
			}

			if ( newPage_File.CompareTo( this[0]) <=0 )
			{
				List.Insert(0, newPage_File );
				return 0;
			}

			if ( newPage_File.CompareTo( this[List.Count-1]) >= 0 )
			{
				return List.Add( newPage_File );
			}

			return Recursive_Sort_Insert( newPage_File, 0, List.Count-1);
		}

		internal int Recursive_Sort_Insert( Builder_Page_File newPage_File, int start, int end)
		{
			int startIndex = start;
			int endIndex = end;
            int midIndex = (int)Math.Ceiling((double)(endIndex - startIndex) / 2) + startIndex;			
			if ( endIndex - startIndex <=1)
			{
				if ( newPage_File.CompareTo( this[startIndex] ) <= 0 )
				{
					List.Insert( startIndex, newPage_File );
					return startIndex;
				}
				else
				{
					List.Insert( endIndex, newPage_File );
					return endIndex;
				}
			}
			else
			{				
				if ( newPage_File.CompareTo( this[midIndex] ) < 0 )
					endIndex = midIndex;
				else
					startIndex = midIndex;

				return Recursive_Sort_Insert( newPage_File, startIndex, endIndex);				
			}
		}

		/// <summary> Remove an existing Page_File under from this collection. </summary>
		/// <param name="nodeid"> node id for this workflow </param>
		/// <exception cref="Exception"> Throws a <see cref="Exception"/> if there is no
		/// an Page_File which matches the Page_File requested to be removed. </exception>
		internal void Remove( int nodeid )
		{
			// Check that this BaseFileName exists
			if ( !Contains(nodeid) )
				throw new Exception("No Page_File '" + nodeid + "' exists for this division");

			// Remove this Page_File, since it existed
			List.Remove( nodeid );
		}

		/// <summary> Check to see if there is a Page_File in this collection already </summary>
		/// <param name="nodeid"> node id for this workflow </param>
		/// <returns>TRUE if the provided work flow id is already part of this Page_File Collection </returns>
		internal bool Contains( int nodeid )
		{
			return List.Contains( nodeid );                
		}
		
		/// <summary> Return an enumerator to step through this collection of tree nodes. </summary>
		/// <returns> A Type-Safe MXF_IncludedFileEnumerator</returns>
		/// <remarks> This version is used in the C# Compiler to detect type conflicts at compilation. </remarks>
		public new Builder_Page_File_Enumerator GetEnumerator()
		{
			return new Builder_Page_File_Enumerator(this);
		}

		/// <summary> Return an enumerator to step through this collection of tree nodes. </summary>
		/// <returns> A IEnumerator object to step through this collection of tree nodes. </returns>
		/// <remarks> Explicit interface implementation to support interoperability with other common 
		/// language runtime-compatible langueages. </remarks>
        IEnumerator IEnumerable.GetEnumerator()
		{	
			return (IEnumerator) new Builder_Page_File_Enumerator(this);
		}

		/// <summary> Inner class implements the <see cref="IEnumerator"/> interface and iterates through 
		/// the <see cref="Builder_Page_File_Collection"/> object composed of <see cref="Builder_Page_File"/> objects 
		/// for this volume. <br/> <br/> </summary>
		/// <remarks> Inclusion of this strongly-typed iterator allows the use of the foreach .. in structure to 
		/// iterate through all of the <see cref="Builder_Page_File"/> objects in the 
		/// <see cref="Builder_Page_File_Collection"/> object. The example in the <see cref="Builder_Page_File_Collection"/>
		/// demonstrates this use.</remarks>
		public class Builder_Page_File_Enumerator : IEnumerator
		{
			/// <summary> Stores position for this enumerator </summary>
			int position = -1;

			/// <summary> Reference to the <see cref="Builder_Page_File_Collection"/> to iterate through. </summary>
			private Builder_Page_File_Collection nodes;

			/// <summary> Constructore creates a new Page_File_Enumerator to iterate through
			/// the <see cref="Builder_Page_File_Collection"/>. </summary>
			/// <param name="nodeCollection"> Collection of nodes </param>
			internal Builder_Page_File_Enumerator( Builder_Page_File_Collection nodeCollection )
			{
				nodes = nodeCollection;
			}

			/// <summary> Move to the next <see cref="Builder_Page_File"/> in this <see cref="Builder_Page_File_Collection"/>. </summary>
			/// <returns> TRUE if successful, otherwise FALSE </returns>
			/// <remarks> Method is required by the IEnumerator interface. </remarks>
			public bool MoveNext()
			{
				if ( position < ( nodes.Count - 1 ))
				{
					position++;
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary> Reset to the position just before the first position.  
			/// Ready for the MoveNext() method to be called. </summary>
			/// <remarks> Method is required by the IEnumerator interface. </remarks>
			public void Reset()
			{
				position = -1;
			}

			/// <summary> Return the current <see cref="Builder_Page_File"/> from the <see cref="Builder_Page_File_Collection"/>. </summary>
			/// <remarks> This type-safe version is used in the C# Compiler to detect type conflicts at compilation. </remarks>
            public Builder_Page_File Current
			{
				get
				{
					return nodes[position];
				}
			}

			/// <summary> Return the current object from the <see cref="Builder_Page_File_Collection"/>. </summary>
			/// <remarks> Explicit interface implementation to support interoperability with other common 
			/// language runtime-compatible langueages. </remarks>
			object IEnumerator.Current
			{
				get
				{	
					return nodes[position];
				}
			}
		}
	}
}
