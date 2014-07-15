using System;
using System.Windows.Forms;
using System.Runtime.InteropServices ;

namespace DLC.Custom_Grid
{

    /// <summary> Delegate is used by the <see cref="Custom_Panel_w_Scroll_Events" /> class when a vertical scroll
    /// change is requested </summary>
    /// <param name="scroll_change"> Change to the current scroll </param>
	public delegate void Vertical_Scroll_Requested_Delegate( int scroll_change );


	/// <summary> Class extends the Panel object and adds events to be fired when this panel scrolls.  <br /> <br /> </summary>
	/// <remarks> Written by Mark Sullivan (2005) </remarks>
	public class Custom_Panel_w_Scroll_Events : Panel
	{
		private const int WM_HSCROLL = 0x114;
		private const int WM_VSCROLL = 0x115;
		private const uint ESB_DISABLE_BOTH = 0x3;
		private const uint SB_HORZ = 0; 
		private const uint SB_VERT = 1;

        /// <summary> References the external SetScrollPos method in the user32.dll </summary>
        /// <param name="hWnd"></param>
        /// <param name="nBar"></param>
        /// <param name="nPos"></param>
        /// <param name="bRedraw"></param>
        /// <returns></returns>
		[DllImport("user32.dll")]
		static public extern int SetScrollPos(System.IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        /// <summary> References the external GetScrollPos method in the user32.dll </summary>
        /// <param name="hWnd"></param>
        /// <param name="nBar"></param>
        /// <returns></returns>
		[DllImport("user32.dll")]
		static public extern int GetScrollPos(System.IntPtr hWnd, int nBar);

        /// <summary> References the external EnableScrollBar method in the user32.dll </summary>
        /// <param name="hWnd"></param>
        /// <param name="wSBflags"></param>
        /// <param name="wArrows"></param>
        /// <returns></returns>
		[DllImport("user32.dll")]
		static public extern bool EnableScrollBar(System.IntPtr hWnd, uint wSBflags, uint wArrows);

		/// <summary> Horizontal scroll position has changed event </summary>
		public event ScrollEventHandler HorizontalScrollValueChanged;

		/// <summary> Vertical scroll position has changed event </summary>
		public event ScrollEventHandler VerticalScrollValueChanged;

		/// <summary> The user has requested this scroll, either by the mouse wheel,
		/// or by hitting page-up / page-down. </summary>
		public event Vertical_Scroll_Requested_Delegate Vertical_Scroll_Requested;

		/// <summary> Constructor for a new instance of this class </summary>
		public Custom_Panel_w_Scroll_Events() : base()
		{
			// Disable the auto vertical scrolling
			EnableScrollBar(this.Handle, SB_VERT, ESB_DISABLE_BOTH);
		}

		/// <summary> Gets or sets the current horizontal scroll position </summary>
		public int AutoScrollHPos
		{
			get 
			{ 
				int returnVal = GetScrollPos(this.Handle, (int)SB_HORZ); 
				return GetScrollPos(this.Handle, (int)SB_HORZ); 
			}
			set { SetScrollPos(this.Handle, (int)SB_HORZ, value, true); }
		}

		/// <summary> Gets or sets the current vertical scroll position </summary>
		public int AutoScrollVPos
		{
			get { return GetScrollPos(this.Handle, (int)SB_VERT); }
			set { SetScrollPos(this.Handle, (int)SB_VERT, value, true); }
		}


		#region Methods to capture, and fire, scroll events

		/// <summary>
		/// Intercept scroll messages to send notifications
		/// </summary>
		/// <param name="m">Message parameters</param>
		protected override void WndProc(ref Message m)
		{


			// Was this a horizontal scroll message?
			if ( m.Msg == WM_HSCROLL ) 
			{
				if ( HorizontalScrollValueChanged != null ) 
				{
					uint wParam = (uint)m.WParam.ToInt32();
					HorizontalScrollValueChanged( this, 
						new ScrollEventArgs( 
						GetEventType( wParam & 0xffff), (int)(wParam >> 16) ) );
				}
			} 
				// or a vertical scroll message?
			if ( m.Msg == WM_VSCROLL )
			{
				if ( VerticalScrollValueChanged != null )
				{
					uint wParam = (uint)m.WParam.ToInt32();
					VerticalScrollValueChanged( this, 
						new ScrollEventArgs( 
						GetEventType( wParam & 0xffff), (int)(wParam >> 16) ) );
				}
			}
			else
			{
				// Let the control process the message
				base.WndProc (ref m);
			}
		}

		// Based on SB_* constants
		private static ScrollEventType [] _events =
			new ScrollEventType[] {
									  ScrollEventType.SmallDecrement,
									  ScrollEventType.SmallIncrement,
									  ScrollEventType.LargeDecrement,
									  ScrollEventType.LargeIncrement,
									  ScrollEventType.ThumbPosition,
									  ScrollEventType.ThumbTrack,
									  ScrollEventType.First,
									  ScrollEventType.Last,
									  ScrollEventType.EndScroll
								  };
		/// <summary>
		/// Decode the type of scroll message
		/// </summary>
		/// <param name="wParam">Lower word of scroll notification</param>
		/// <returns></returns>
		private ScrollEventType GetEventType( uint wParam )
		{
			if ( wParam < _events.Length )
				return _events[wParam];
			else
				return ScrollEventType.EndScroll;
		}

		#endregion

		#region Overriding methods to capture PAGE UP and PAGE DOWN, as well as Scroll Events

		/// <summary> Override the standard IsInputKey method to intercept the TAB key </summary>
		/// <param name="keyData">Key data </param>
		/// <returns> TRUE if the key press is handled, otherwise FALSE </returns>
		protected override bool IsInputKey(Keys keyData)
		{
			if ((keyData == Keys.PageDown ) || (keyData == Keys.PageUp ))
				return true;
			else
				return base.IsInputKey(keyData);
		}

		/// <summary> Override standard PreProcessMessage catches Inputs of BackSpace and Deletes </summary>
		/// <param name="msg">PreProcessMessage</param>
		public override bool PreProcessMessage(ref Message msg)
		{
			if ( msg.Msg == 256 )
			{
				Keys keyData = ((Keys) (int) msg.WParam) |ModifierKeys;
				Keys keyCode = ((Keys) (int) msg.WParam);

				if ( keyCode == Keys.PageUp )
				{
					// Fire the event
					if ( this.Vertical_Scroll_Requested != null )
						this.Vertical_Scroll_Requested( -1 * this.Height );
				}

				if ( keyCode == Keys.PageDown )
				{
					// Fire the event
					if ( this.Vertical_Scroll_Requested != null )
						this.Vertical_Scroll_Requested( this.Height );
				}
			}
			
			// Can't type anything in here
			return false;
		}

        /// <summary> Overrides standard OnMouseWheel method to control the scrolling on this panel </summary>
        /// <param name="e"> Mouse event arguments </param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			// Fire the event for a scroll requested
			if ( this.Vertical_Scroll_Requested != null )
			{
				this.Vertical_Scroll_Requested( -1 * e.Delta );
			}			
		}


		#endregion
	}
}
