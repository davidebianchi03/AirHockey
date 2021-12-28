using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using window_utilities;

namespace AirHockey.window_utilities
{
    class UIMessageBox
    {
        public string Content { get; set; } = "";
        public string Title { get; set; } = "";
        public RenderWindow ParentWindow { get; set; } = null;
        public Font Font { get; set; } = null;
        private UIButton ButtonClose = null;//pulsante per chiudere la finestra
        private UIWindow ChildWindow = null;
        private Thread DisplayThread = null;
        public VideoMode VideoMode { get; set; }
        public bool IsOpen { get; set; } = false;


        public UIMessageBox(VideoMode mode, string title, string content, RenderWindow ParentWindow, Font Font)
        {
            this.ParentWindow = ParentWindow;
            this.Font = Font;
            this.VideoMode = mode;
            this.Title = title;
            this.Content = content;
            DisplayThread = new Thread(ShowThreadMethod);
        }

        private void ButtonPressedCallback(object sender, EventArgs e)
        {
            ChildWindow.Close();
        }

        public void Show()
        {
            DisplayThread.Start();
            DisplayThread.Join();
        }

        public void Close()
        {
            DisplayThread.Abort();
            ChildWindow.Close();
        }

        private void ShowThreadMethod()
        {
            ChildWindow = new UIWindow(VideoMode, Title);
            ChildWindow.Closed += (s, e) => ChildWindow.Close();
            ButtonClose = new UIButton(ChildWindow, Font);
            ButtonClose.content = "OK";
            ButtonClose.BorderThickness = 2;
            ButtonClose.BorderColor = Color.White;
            ButtonClose.textColor = Color.White;
            ButtonClose.FillColor = Color.Black;
            ButtonClose.Size = new SFML.System.Vector2f(100, 50);
            ButtonClose.Position = new SFML.System.Vector2f((ChildWindow.Size.X / 2) - (ButtonClose.Size.X / 2), ChildWindow.Size.Y - 75);
            ButtonClose.textSize = 18;
            ButtonClose.ButtonPressed += ButtonPressedCallback;
            Text ContentLbl = new Text(Content, Font);
            ContentLbl.CharacterSize = 16;
            ContentLbl.FillColor = Color.White;
            ContentLbl.Position = new SFML.System.Vector2f((ChildWindow.Size.X / 2) - (ContentLbl.GetGlobalBounds().Width / 2), 10);
            IsOpen = true;
            while (ChildWindow.IsOpen && IsOpen)
            {
                ChildWindow.DispatchEvents();
                ChildWindow.Display();
                ButtonClose.draw();
                ChildWindow.Draw(ContentLbl);
                Thread.Sleep(33);
            }
            IsOpen = false;
        }

    }
}
