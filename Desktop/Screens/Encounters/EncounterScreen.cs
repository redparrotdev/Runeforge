using Desktop.Events;
using Desktop.Extensions;
using Desktop.GUI;
using Engine.Events;
using Engine.Graphics;
using Engine.Helpers.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Desktop.Screens.Encounters;

public class EncounterScreen
{
    private const int EncounterPanelWidth = 526;
    private const int OptionsSpacing = 8;

    private readonly SpriteFont _font;

    private readonly EncounterOption[] _options = [
        null,
        null,
        null
    ];

    private TextPanel _encounterTextPanel;
    private TextPanel[] _optionsPanels;

    public EncounterScreen(ContentManager content
        , GraphicsDevice gd)
    {
        _font = content.DefaultFont();

        var panelsBg = ShapesHelper.Rectangle(
            gd
            , 32
            , 32
            , Color.Gray);

        SetupPanels(_font
            , panelsBg
            , [panelsBg, panelsBg, panelsBg]);
    }

    public void Update(GameTime gameTime)
    {
        var kbState = Keyboard.GetState();

        if (kbState.IsKeyDown(Keys.A))
        {
            EventManager.Dispatch(new GameEvents.EncounterOptionSelected(0));
        }
        else if (kbState.IsKeyDown(Keys.S))
        {
            EventManager.Dispatch(new GameEvents.EncounterOptionSelected(1));
        }
        else if (kbState.IsKeyDown(Keys.D))
        {
            EventManager.Dispatch(new GameEvents.EncounterOptionSelected(2));
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        _encounterTextPanel.Draw(spriteBatch, gameTime);
        for (int i = 0; i < 3; i++)
        {
            var option = _options[i];
            if (option is null) continue;
            _optionsPanels[i].Draw(spriteBatch, gameTime);
        }
    }

    public EncounterScreen SetEncounterText(string text)
    {
        _encounterTextPanel.Text = text;
        return this;
    }

    public EncounterScreen SetFirstOption(EncounterOption option)
    {
        return SetOption(0, option);
    }

    public EncounterScreen SetSecondOption(EncounterOption option)
    {
        return SetOption(2, option);
    }

    public EncounterScreen SetThirdOption(EncounterOption option)
    {
        return SetOption(1, option);
    }

    public EncounterScreen ClearOptions()
    {
        for (int i = 0; i < 3; i++)
        {
            _options[i] = null;
            _optionsPanels[i].Text = string.Empty;
        }

        return this;
    }

    private EncounterScreen SetOption(int index, EncounterOption option)
    {
        if (option is null)
        {
            return this;
        }

        Debug.Assert(index >= 0 && index < 3);
        _options[index] = option;
        _optionsPanels[index].Text = option.Text;
        return this;
    }

    private void SetupPanels(
        SpriteFont font
        , Texture2D encounterTextBackground
        , Texture2D[] optionsPanelsBackgrounds)
    {
        Debug.Assert(optionsPanelsBackgrounds.Length == 3);

        var fontSpacing = font.LineSpacing;
        var spacingHalf = fontSpacing / 2f;

        var encounterTextPanelSprite = new Sprite(encounterTextBackground);
        _encounterTextPanel = new TextPanel(
            string.Empty
            , _font
            , EncounterPanelWidth
            , fontSpacing * 3 + spacingHalf
            , encounterTextPanelSprite)
        {
            Position = CalculateEncounterTextPosition()
        };

        _optionsPanels = new TextPanel[3];
        var optionWidth = (EncounterPanelWidth - OptionsSpacing * 2) / 3;
        var optionY = _encounterTextPanel.Bounds.Y + _encounterTextPanel.Height + OptionsSpacing;
        for (int i = 0; i < 3; i++)
        {
            var optionX = _encounterTextPanel.Bounds.X + (optionWidth + OptionsSpacing) * i;

            var optionSprite = new Sprite(optionsPanelsBackgrounds[i]);

            _optionsPanels[i] = new TextPanel(
                string.Empty
                , _font
                , optionWidth
                , fontSpacing + spacingHalf
                , optionSprite)
            {
                Position = new Vector2(optionX, optionY)
            };
        }
    }

    private static Vector2 CalculateEncounterTextPosition()
    {
        var x = Core.ViewportAdapter.VirtualWidth / 2f - EncounterPanelWidth / 2f;
        var y = Core.ViewportAdapter.VirtualHeight / 4f;

        return new Vector2(x, y);
    }
}
