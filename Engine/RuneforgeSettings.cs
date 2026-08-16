using Microsoft.Xna.Framework;

namespace Engine;

public sealed class RuneforgeSettings
{
    public static RuneforgeSettings Default => new()
    {
        Title = "Runeforge",
        Widht = 1280,
        Height = 720,
    };

    public required string Title { get; init; }
    public required int Widht { get; init; }
    public required int Height { get; init; }

    public string ContentFolder { get; init; } = "Content";
    public bool FixedFPS { get; init; } = false;
    public int TargetFPS { get; init; } = 60;
    public bool IsMouseVisible { get; init; } = true;
    public bool DebugEnabled { get; init; } = true;

    public Color ClearColor { get; init; } = Color.CornflowerBlue;

    public bool CloseOnEscapeOrBack { get; init; } = true;
}
