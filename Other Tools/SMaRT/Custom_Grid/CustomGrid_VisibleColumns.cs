using System;
using System.Collections;
using System.Collections.Generic;

namespace DLC.Custom_Grid
{
	/// <summary> CustomGrid_VisibleColumns is an object used to reference
	/// just the columns which are visible. </summary>
	public class CustomGrid_VisibleColumns : IEnumerable
	{
		/// <summary> Stores the complete list of columns in the collection </summary>
		private List<CustomGrid_ColumnStyle> columnCollection;

		/// <summary> Constructor for a new CustomGrid_VisibleColumns object </summary>
		/// <param name="columnCollection"> Collection of columns </param>
		public CustomGrid_VisibleColumns( List<CustomGrid_ColumnStyle> columnCollection )
		{
			// Save the entire collection
			this.columnCollection = columnCollection;
		}

		/// <summary> Gets the nth visible column style object, by index </summary>
		public CustomGrid_ColumnStyle this[ int index ]
		{
			get
			{
				int visibleCount = -1;

				// Iterate through each column
				foreach( CustomGrid_ColumnStyle thisCol in columnCollection )
				{
					// See if this column is visible
					if ( thisCol.Visible )
						visibleCount++;

					// If this matches the index, return this column
					if ( visibleCount == index )
						return thisCol;
				}

				// Return NULL since the index was invalid
				return null;	
			}
		}

		/// <summary> Returns the number of visible columns in the column collection </summary>
		public int Count
		{
			get
			{
				int returnVal = 0;

				// Iterate through each column
				foreach( CustomGrid_ColumnStyle thisCol in columnCollection )
				{
					if ( thisCol.Visible )
						returnVal++;
				}

				// Return number of visible columns
				return returnVal;	
			}
		}

		/// <summary> Return an enumerator to step through this collection of visible column styles. </summary>
		/// <returns> A Type-Safe CustomGrid_VisibleColumns_Enumerator</returns>
		/// <remarks> This version is used in the C# Compiler to detect type conflicts at compilation. </remarks>
		public CustomGrid_VisibleColumns_Enumerator GetEnumerator()
		{
			return new CustomGrid_VisibleColumns_Enumerator(this);
		}

		/// <summary> Return an enumerator to step through this collection of visible column styles. </summary>
		/// <returns> A IEnumerator object to step through this collection of visible column styles. </returns>
		/// <remarks> Explicit interface implementation to support interoperability with other common 
		/// language runtime-compatible langueages. </remarks>
		IEnumerator IEnumerable.GetEnumerator()
		{	
			return (IEnumerator) new CustomGrid_VisibleColumns_Enumerator(this);
		}

		/// <summary> Inner class implements the <see cref="IEnumerator"/> interface and iterates through 
		/// the <see cref="CustomGrid_VisibleColumns"/> collection object composed of <see cref="CustomGrid_ColumnStyle"/> objects 
		/// for this volume. <br/> <br/> </summary>
		/// <remarks> Inclusion of this strongly-typed iterator allows the use of the foreach .. in structure to 
		/// iterate through all of the <see cref="CustomGrid_ColumnStyle"/> objects in the 
		/// <see cref="CustomGrid_VisibleColumns"/> collection object. The example in the <see cref="CustomGrid_VisibleColumns"/> collection.
		/// demonstrates this use.</remarks>
		public class CustomGrid_VisibleColumns_Enumerator : IEnumerator
		{
			/// <summary> Stores position for this enumerator </summary>
			int position = -1;

			/// <summary> Reference to the <see cref="CustomGrid_VisibleColumns"/> collecction to iterate through. </summary>
			private CustomGrid_VisibleColumns styles;

			/// <summary> Constructore creates a new CustomGrid_VisibleColumns_Enumerator to iterate through
			/// the <see cref="CustomGrid_VisibleColumns"/> collection. </summary>
            /// <param name="styleCollection"> Collection of styles </param>
			public CustomGrid_VisibleColumns_Enumerator( CustomGrid_VisibleColumns styleCollection )
			{
				styles = styleCollection;
			}

			/// <summary> Move to the next <see cref="CustomGrid_ColumnStyle"/> in this <see cref="CustomGrid_VisibleColumns"/> collection. </summary>
			/// <returns> TRUE if successful, otherwise FALSE </returns>
			/// <remarks> Method is required by the IEnumerator interface. </remarks>
			public bool MoveNext()
			{
				if ( position < ( styles.Count - 1 ))
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

			/// <summary> Return the current <see cref="CustomGrid_ColumnStyle"/> from the <see cref="CustomGrid_VisibleColumns"/> collection. </summary>
			/// <remarks> This type-safe version is used in the C# Compiler to detect type conflicts at compilation. </remarks>
			public CustomGrid_ColumnStyle Current
			{
				get
				{
					return styles[position];
				}
			}

			/// <summary> Return the current object from the <see cref="CustomGrid_VisibleColumns"/> collection. </summary>
			/// <remarks> Explicit interface implementation to support interoperability with other common 
			/// language runtime-compatible langueages. </remarks>
			object IEnumerator.Current
			{
				get
				{	
					return styles[position];
				}
			}
		}
	}
}
