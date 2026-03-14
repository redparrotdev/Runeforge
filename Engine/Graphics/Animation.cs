using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Graphics;

public sealed class Animation
{
    public IReadOnlyList<TextureRegion> Frames { get; init; }
    public TimeSpan FrameTime { get; init; }

    public Animation(IEnumerable<TextureRegion> frames, TimeSpan frameTime)
    {
        Frames = [..frames];
        FrameTime = frameTime;
    }

    public Animation(IEnumerable<TextureRegion> frames) : this(frames, TimeSpan.FromSeconds(1) / frames.Count())
    {
    }
}
