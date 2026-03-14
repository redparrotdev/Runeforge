using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Graphics;

public sealed class Animation
{
    public IReadOnlyList<Sprite> Frames { get; init; }
    public TimeSpan FrameTime { get; init; }

    public Animation(IEnumerable<Sprite> frames, TimeSpan frameTime)
    {
        Frames = [..frames];
        FrameTime = frameTime;
    }

    public Animation(IEnumerable<Sprite> frames) : this(frames, TimeSpan.FromSeconds(1) / frames.Count())
    {
    }
}
