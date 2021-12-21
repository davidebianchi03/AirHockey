using SFML.Graphics;
using SFML.System;
using System;
using System.Threading;

namespace window_utilities
{
    public class UIButton
    {
        public string content { get; set; }//define button content
        public Color textColor { get; set; }//define text color

        public int textSize { get; set; }

        public Vector2f Size { get; set; }

        public Vector2f Position { get; set; }

        public int TextSize { get; set; }

        public Color FillColor { get; set; }

        private RenderWindow window;

        public Font Font { get; set; }

        public int BorderThickness { get; set; }

        public Color BorderColor { get; set; }
        public bool Enable { get; set; } = true;

        //constructors
        public UIButton(RenderWindow window, Font Font)
        {
            content = "";
            textColor = Color.Black;
            this.Font = Font;
            Size = new Vector2f(0, 0);
            Position = new Vector2f(0, 0);
            TextSize = 0;
            FillColor = Color.White;
            this.window = window;
            BorderColor = Color.Black;
            BorderThickness = 0;
            window.MouseButtonReleased += MouseButtonReleasedCallback;
            window.MouseButtonPressed += MousePressedCallback;
        }


        public UIButton(RenderWindow window, string content, Font Font)
        {
            this.content = content;
            textColor = Color.Black;
            this.Font = Font;
            Size = new Vector2f(0, 0);
            Position = new Vector2f(0, 0);
            TextSize = 0;
            FillColor = Color.White;
            this.window = window;
            BorderColor = Color.Black;
            BorderThickness = 0;
            window.MouseButtonReleased += MouseButtonReleasedCallback;
            window.MouseButtonPressed += MousePressedCallback;
        }

        public UIButton(RenderWindow window, string content, Color textColor, Font Font)
        {
            this.content = content;
            this.textColor = textColor;
            this.Font = Font;
            Size = new Vector2f(0, 0);
            Position = new Vector2f(0, 0);
            TextSize = 0;
            FillColor = Color.White;
            this.window = window;
            BorderColor = Color.Black;
            BorderThickness = 0;
            window.MouseButtonReleased += MouseButtonReleasedCallback;
            window.MouseButtonPressed += MousePressedCallback;
        }

        public void draw()
        {
            Text text_to_show = new Text(content, Font);
            text_to_show.CharacterSize = (uint)textSize;
            text_to_show.FillColor = textColor;
            float content_width = text_to_show.GetLocalBounds().Width;
            float content_height = text_to_show.GetLocalBounds().Height;

            int content_x = (int)Position.X + (int)Size.X / 2 - (int)content_width / 2;
            int content_y = (int)Position.Y + (int)Size.Y / 2 - (int)content_height / 2 - 5;
            text_to_show.Position = new Vector2f(content_x, content_y);

            //text_to_show.Position = Position;

            RectangleShape shape = new RectangleShape();
            shape.Size = Size;
            shape.Position = Position;
            shape.FillColor = FillColor;
            shape.OutlineColor = BorderColor;
            shape.OutlineThickness = BorderThickness;

            window.Draw(shape);
            window.Draw(text_to_show);

        }

        private Vector2f original_size;
        private Vector2f original_position;
        private Vector2f new_size;
        private Vector2f new_position;

        private void MousePressedCallback(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            //controllo se il mouse è stato premuto nello spazio del pulsante
            int mouse_x = e.X;
            int mouse_y = e.Y;
            if ((mouse_x >= Position.X && mouse_y >= Position.Y && mouse_x <= Position.X + Size.X && mouse_y <= Position.Y + Size.Y) && Enable)
            {
                original_size = Size;
                original_position = Position;
                new_size = new Vector2f(original_size.X - 4, original_size.Y - 4);
                new_position = new Vector2f(original_position.X + 2, original_position.Y + 2);
                Size = new_size;
                Position = new_position;
            }

        }

        private void MouseButtonReleasedCallback(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            //controllo se il mouse è stato premuto nello spazio del pulsante
            int mouse_x = e.X;
            int mouse_y = e.Y;
            if ((mouse_x >= Position.X && mouse_y >= Position.Y && mouse_x <= Position.X + Size.X && mouse_y <= Position.Y + Size.Y) && Enable)
            {
                ButtonPressed?.Invoke(this, EventArgs.Empty);//richiamo l'evento

                Size = original_size;
                Position = original_position;
            }
        }

        public event EventHandler ButtonPressed;//evento pulsante premuto

    }
}
