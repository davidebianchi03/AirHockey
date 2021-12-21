using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace window_utilities
{
    //finestra che quando viene ridimensionata non perde la risoluzione
    public class UIWindow : RenderWindow
    {
        public UIWindow(VideoMode mode, string title) : base(mode, title)
        {
            this.Resized += UIWindowResizedCallback;
        }

        private void UIWindowResizedCallback(object sender, SizeEventArgs e)
        {
            View v = this.GetView();
            //faccio in modo che la view no cambi di dimensione
            v.Size = new SFML.System.Vector2f(e.Width, e.Height);
            //faccio in modo che la view rimanga sempre allineata in alto a sinistra
            //il centro della view (ha le stesse dimensioni della finestra) corrisponde al centro della finestra
            v.Center = new SFML.System.Vector2f(e.Width / 2, e.Height / 2);
            this.SetView(v);
            this.Display();
        }
    }
}
