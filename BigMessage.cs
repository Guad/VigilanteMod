using System.Drawing;
using GTA;

class BigMessage
{
    private static UIText _bigmessage = new UIText("VIGILANTE", new Point(630, 100), 2.5f, Color.Goldenrod, 2, true);
    private static int _ticks;
    private static int _timer;
    private static bool _showMessage;

    public static void OnTick()
    {
        if (_ticks >= _timer)
        {
            _ticks = 0;
            _showMessage = false;
        }
        if (_showMessage)
        {
            _bigmessage.Draw();
            _ticks += 1;
        }
    }

    public static void ShowMessage(string text, int time, Color color, float size = 2.5f)
    {
        _timer = time;
        _bigmessage.Caption = text;
        
        _bigmessage.Color = color;
        _bigmessage.Scale = size;
        
        _showMessage = true;
    }
}
