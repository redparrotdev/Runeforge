using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.Extensions;
public static class ContentExtensions
{
    public const string DefaultFont22 = "Fonts/Default22";

    public static SpriteFont DefaultFont(this ContentManager content)
    {

        return content
            .Load<SpriteFont>(DefaultFont22);
    }
}

