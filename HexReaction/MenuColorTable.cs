using System.Drawing;
using System.Windows.Forms;

namespace HexReaction
{
    class MenuColorTable : ProfessionalColorTable
    {
        public MenuColorTable()
        {
            // see notes
            base.UseSystemColors = false;
        }
        public override System.Drawing.Color MenuBorder
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        public override System.Drawing.Color MenuItemBorder
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        public override Color MenuItemSelected
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        public override Color MenuStripGradientBegin
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        public override Color MenuStripGradientEnd
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        
        public override Color ImageMarginGradientBegin
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        public override Color ImageMarginGradientEnd
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
        public override Color ImageMarginGradientMiddle
        {
            get { return Color.FromArgb(27, 27, 28); }
        }
    }
}
