using Engine.Abstractions.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.GUI;

public class TextPanel : Panel
{
    public string Text { get; set; }
    public SpriteFont Font { get; }
    public Color TextColor { get; set; } = Color.Black;

    public TextPanel(
        string text
        , SpriteFont font
        , float width
        , float height
        , ISprite background) 
        : base(width, height, background)
    {
        Text = text;
        Font = font;
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!IsVisible) return;

        base.Draw(spriteBatch, gameTime);

        if (Text is not null)
        {
            var lines = Text.Split('\n');
            float maxWidth = 0f;
            float totalHeight = 0f;

            // Measure all lines
            foreach (var line in lines)
            {
                var size = Font.MeasureString(line);
                if (size.X > maxWidth) maxWidth = size.X;
                totalHeight += size.Y;
            }

            // Centered position (relative to panel)
            var panelCenter = new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
            var origin = new Vector2(maxWidth / 2, totalHeight / 2);

            float y = panelCenter.Y - origin.Y;
            foreach (var line in lines)
            {
                var lineSize = Font.MeasureString(line);
                var linePos = new Vector2(panelCenter.X - lineSize.X / 2, y);
                spriteBatch.DrawString(Font, line, linePos, TextColor);
                y += lineSize.Y;
            }
        }
    }


}
