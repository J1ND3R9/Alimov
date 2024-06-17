using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;

namespace TRPO
{
    public static class Effects
    {
        public static void DarkEffect(Window win)
        {
            win.Opacity = 0.5;
            BlurEffect objBlur = new BlurEffect();
            objBlur.Radius = 1;
            win.Effect = objBlur;
        }

        public static void ClearEffect(Window win)
        {
            win.Opacity = 1;
            win.Effect = null;
        }
    }
}
