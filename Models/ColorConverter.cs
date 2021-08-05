using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWeb.Models
{
    public class ColorConverter
    {
        public int FromColorToInt(Color Color)
        {
            return Color.ToArgb();
        }
        //public Color FromIntToColor(int Color)
    }
}
