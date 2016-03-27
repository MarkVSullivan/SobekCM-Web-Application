using System.Collections.Generic;

namespace SobekCM.Library.ItemViewer.Menu
{
    public class Item_MenuItem
    {
        public readonly string MenuStripText;

        public readonly string MidMenuText;

        public readonly string SubMenuText;

        public readonly string Link;

        public readonly string Code;

        public Item_MenuItem()
        {
            // Do nothing
        }

        public Item_MenuItem(string MenuStripText, string MidMenuText, string SubMenuText, string Link, string Code)
        {
            this.MenuStripText = MenuStripText;
            this.MidMenuText = MidMenuText;
            this.SubMenuText = SubMenuText;
            this.Link = Link;
            this.Code = Code;
        }
    }
}
