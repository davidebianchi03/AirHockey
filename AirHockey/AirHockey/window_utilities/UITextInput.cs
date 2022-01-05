using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 IMPORTANTE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    Questa classe funziona solo con tastiera con layout italiano
 */

namespace window_utilities
{
    public class UITextInput
    {
        public Font Font { get; set; }//font utilizzato nella text input
        public Vector2f Size { get; set; }//dimensione dell'input del testo
        public Vector2f Position { get; set; }//posizione dell'input del testo
        public Color BackgroundColor { get; set; }//colore dello sfondo
        public Color ForegroundColor { get; set; }//colore del testo
        public int BorderThickness { get; set; }
        public Color BorderColor { get; set; }

        public string Content;

        public int TextSize { get; set; } = 16;

        private RenderWindow window;//text input target window
        public int TextAlignment { get; set; }//allineamento del testo all'interno dell'input
        public const int AlignmentLeft = 1;//allineamento del testo a sinistra
        public const int AlignmentCenter = 2;//allineamento del testo in centro
        public const int AlignmentRight = 3;//allineamento del testo a destra
        private Text text;

        private bool textInputSelected;
        private int cursorPosition;
        public bool Enable { get; set; } = true;


        //Costruttori
        public UITextInput(Font Font, RenderWindow window)
        {
            this.Font = Font;
            this.window = window;
            Size = new Vector2f();
            Position = new Vector2f();
            BackgroundColor = Color.White;
            ForegroundColor = Color.Black;
            BorderThickness = 0;
            BorderColor = Color.Black;
            Content = "";
            TextSize = 12;
            textInputSelected = false;
            window.MouseButtonPressed += MousePressedCallback;
            window.MouseButtonReleased += MouseRealesedCallback;
            window.KeyReleased += KeyPressedCallback;
            text = new Text();
            cursorPosition = Content.Length;
        }

        public UITextInput(Font Font, RenderWindow window, string Content)
        {
            this.Font = Font;
            this.window = window;
            this.Content = Content;
            Size = new Vector2f();
            Position = new Vector2f();
            BackgroundColor = Color.White;
            ForegroundColor = Color.Black;
            BorderThickness = 0;
            BorderColor = Color.Black;
            TextSize = 12;
            textInputSelected = false;
            window.MouseButtonPressed += MousePressedCallback;
            window.MouseButtonReleased += MouseRealesedCallback;
            text = new Text();
            text.DisplayedString = Content;
            cursorPosition = Content.Length;
        }

        //Metodo per disegnare l'input del testo
        public void draw()
        {
            //disegno il container
            RectangleShape container = new RectangleShape(Size);
            container.Position = Position;
            container.FillColor = BackgroundColor;
            container.OutlineColor = BorderColor;
            container.OutlineThickness = BorderThickness;
            window.Draw(container);
            //disegno il testo
            text.Position = new Vector2f(Position.X + 10, Position.Y + (Size.Y / 2) - (TextSize / 2));
            text.Font = Font;
            text.CharacterSize = (uint)TextSize;
            text.FillColor = ForegroundColor;

            text.DisplayedString = "";

            for (int i = 0; i <= Content.Length; i++)
            {
                if (i == cursorPosition && ShowTextCursor)
                {
                    RectangleShape cursor = new RectangleShape();
                    cursor.Size = new Vector2f(2, TextSize + 4);
                    cursor.FillColor = ForegroundColor;
                    cursor.Position = new Vector2f(text.GetGlobalBounds().Left + text.GetGlobalBounds().Width, text.GetGlobalBounds().Top - 4);
                    window.Draw(cursor);
                }

                if (i < Content.Length)
                {
                    text.DisplayedString += Content[i];
                }
            }


            window.Draw(text);
        }

        private bool mousePressed = false;
        private bool ShowTextCursor = false;

        private void MouseRealesedCallback(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (mousePressed == true)
            {
                int mouse_x = e.X;
                int mouse_y = e.Y;
                mousePressed = false;
                if (mouse_x >= Position.X && mouse_y >= Position.Y && mouse_x <= Position.X + Size.X && mouse_y <= Position.Y + Size.Y)
                {
                    //mouse premuto e rilasciato nel container del text input
                    textInputSelected = true;
                    Thread TextCursorThread = new Thread(KeyboardCursorAnimation);
                    TextCursorThread.Start();
                }
                else
                {
                    textInputSelected = false;
                }
            }
        }

        private void KeyboardCursorAnimation()
        {
            while (textInputSelected && window.IsOpen)
            {
                ShowTextCursor = !ShowTextCursor;
                Thread.Sleep(750);
            }
            ShowTextCursor = false;
        }

        private void MousePressedCallback(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            mousePressed = true;
        }

        private void KeyPressedCallback(object sender, SFML.Window.KeyEventArgs e)
        {
            if (textInputSelected)
            {
                if (((int)e.Code) >= 0 && ((int)e.Code) <= 25)
                {
                    //si tratta di una lettera
                    if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift))//Modificatore shift premuto
                    {
                        Content = Content.Insert(cursorPosition, e.Code.ToString());
                        cursorPosition++;
                    }
                    else
                    {
                        Content = Content.Insert(cursorPosition, e.Code.ToString().ToLower());
                        cursorPosition++;
                    }
                }
                else if (e.Code == Keyboard.Key.Backspace)
                {
                    //Tasto Backspace
                    if (cursorPosition > 0)
                    {
                        Content = Content.Remove(cursorPosition - 1, 1);
                        cursorPosition--;
                    }
                }
                else if (((int)e.Code) >= 26 && ((int)e.Code) <= 36)
                {
                    //Visualizzazione dei numeri
                    Content = Content.Insert(cursorPosition, (((int)e.Code) - 26).ToString());
                    cursorPosition++;
                }
                else if (e.Code == Keyboard.Key.Right)
                {
                    //Freccetta a destra
                    if (cursorPosition + 1 <= Content.Length)
                    {
                        cursorPosition++;
                    }
                }
                else if (e.Code == Keyboard.Key.Left)
                {
                    //Freccetta a destra
                    if (cursorPosition - 1 >= 0)
                    {
                        cursorPosition--;
                    }
                }
                else if (e.Code == Keyboard.Key.Delete)
                {
                    if (cursorPosition < Content.Length)
                    {
                        Content = Content.Remove(cursorPosition, 1);
                    }
                }
                else if(e.Code == Keyboard.Key.Period)
                {
                    //punto
                    Content = Content.Insert(cursorPosition, ".");
                    cursorPosition++;
                }
                else
                {
                    Console.Beep(500, 100);
                }

                KeyPressed?.Invoke(this, EventArgs.Empty);
                
            }
        }

        public event EventHandler KeyPressed;
    }

}
