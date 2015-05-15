using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using System.Drawing;

class BigMessage
{
    private static UIText bigmessage = new UIText("VIGILANTE", new Point(630, 100), 2.5f, Color.Goldenrod, 2, true);
    private static int ticks = 0;
    private static int timer = 0;
    private static bool showMessage = false;

    public static void OnTick()
    {
        if (ticks >= timer)
        {
            ticks = 0;
            showMessage = false;
        }
        if (showMessage)
        {
            bigmessage.Draw();
            ticks += 1;
        }
    }

    public static void ShowMessage(string text, int time, Color color, float size = 2.5f)
    {
        timer = time;
        bigmessage.Caption = text;
        
        bigmessage.Color = color;
        bigmessage.Scale = size;
        
        showMessage = true;
    }
}
