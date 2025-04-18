using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ASM_02
{
    internal class MyTextBlock : TextBlock
    {
        public string numberString;
        public string picture;
        public MyTextBlock(int r, int c)
        {
            numberString = "\n    " + r + c;
        }
        
        public void ShowPicture()
        {
            FontSize = 55;
            Text = picture;
        }

        public void ShowNumber()
        {
            FontSize = 22;
            Text = numberString;
        }
    }
}
