using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Graphics;

public sealed class Animation
{
    public IReadOnlyList<Sprite> Frames { get; init; }
    public float FrameRate { get; init; }

    public Animation(IEnumerable<Sprite> frames, float frameTime)
    {
        Frames = [..frames];
        FrameRate = frameTime;
    }

    public Animation(IEnumerable<Sprite> frames) : this(frames, 60f)
    {
    }
}
