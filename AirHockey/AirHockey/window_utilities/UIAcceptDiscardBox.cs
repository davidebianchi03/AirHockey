using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using window_utilities;

namespace AirHockey.window_utilities
{
    class UIAcceptDiscardBox
    {
        /*
            Classe per creare e visualizzare una messagebox con la possibilità di poter scegliere tra varie opzioni [pulsanti] 
        */
        public string Content { get; set; } = "";
        public string Title { get; set; } = "";
        public RenderWindow ParentWindow { get; set; } = null;
        public Font Font { get; set; } = null;
        private UIButton ButtonAccept = null;
        private UIButton ButtonDiscard = null;
        private UIWindow ChildWindow = null;
        private Thread DisplayThread = null;
        public VideoMode VideoMode { get; set; }
        public bool IsOpen { get; set; } = false;
        private int ResponseCode = Cancel;
        public const int Ok = 0;
        public const int Cancel = 1;

        public UIAcceptDiscardBox(VideoMode mode, string title, string content, RenderWindow ParentWindow, Font Font)
        {
            this.ParentWindow = ParentWindow;
            this.Font = Font;
            this.VideoMode = mode;
            this.Title = title;
            this.Content = content;
            DisplayThread = new Thread(ShowThreadMethod);
        }

        private void ButtonAcceptPressedCallback(object sender, EventArgs e)
        {
            ChildWindow.Close();
            ResponseCode = Ok;
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
            ButtonAccept = new UIButton(ChildWindow, Font);
            ButtonAccept.content = "OK";
            ButtonAccept.BorderThickness = 2;
            ButtonAccept.BorderColor = Color.White;
            ButtonAccept.textColor = Color.White;
            ButtonAccept.FillColor = Color.Black;
            ButtonAccept.Size = new SFML.System.Vector2f(100, 50);
            ButtonAccept.Position = new SFML.System.Vector2f((ChildWindow.Size.X / 2) - ButtonAccept.Size.X - 5, ChildWindow.Size.Y - 75);
            ButtonAccept.textSize = 18;
            ButtonAccept.ButtonPressed += ButtonAcceptPressedCallback;
            //Button discard
            ButtonDiscard = new UIButton(ChildWindow, Font);
            ButtonDiscard.content = "Annulla";
            ButtonDiscard.BorderThickness = 2;
            ButtonDiscard.BorderColor = Color.White;
            ButtonDiscard.textColor = Color.White;
            ButtonDiscard.FillColor = Color.Black;
            ButtonDiscard.Size = new SFML.System.Vector2f(100, 50);
            ButtonDiscard.Position = new SFML.System.Vector2f((ChildWindow.Size.X / 2) + 5, ChildWindow.Size.Y - 75);
            ButtonDiscard.textSize = 18;
            ButtonDiscard.ButtonPressed += ButtonDiscardPressedCallback;

            Text ContentLbl = new Text(Content, Font);
            ContentLbl.CharacterSize = 16;
            ContentLbl.FillColor = Color.White;
            ContentLbl.Position = new SFML.System.Vector2f((ChildWindow.Size.X / 2) - (ContentLbl.GetGlobalBounds().Width / 2), 10);
            IsOpen = true;
            while (ChildWindow.IsOpen && IsOpen)
            {
                ChildWindow.DispatchEvents();
                ChildWindow.Display();
                ButtonAccept.draw();
                ButtonDiscard.draw();
                ChildWindow.Draw(ContentLbl);
                Thread.Sleep(33);
            }
            IsOpen = false;
        }

        public int getResponseCode()
        {
            return ResponseCode;
        }

        private void ButtonDiscardPressedCallback(object sender, EventArgs e)
        {
            ChildWindow.Close();
            ResponseCode = Cancel;
        }
    }
}
