using Engine.Debugging;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine.Components.Graphics;

public class SpriteAnimatorComponent : SpriteComponent
{
    public enum LoopMode
    {
        Loop,
        Once,
        OnceClamp
    }

    public enum AnimationState
    {
        Running,
        Completed
    }

    [DebugExpose]
    public float FrameTime { get; set; }

    public Animation CurrentAnimation { get; private set; }

    [DebugExpose]
    public string CurrentAnimationName { get; private set; }

    [DebugExpose]
    public LoopMode CurrentLoopMode { get; private set; }

    [DebugExpose]
    public AnimationState CurrentAnimationState { get; private set; }

    [DebugExpose]
    public int CurrentFrame { get; private set; }

    [DebugExpose]
    public int FrameCount { get; private set; }

    [DebugExpose]
    public float CurrentElapsedTime { get; private set; }

    [DebugExpose]
    public float FrameTimeLeft { get; private set; }

    public Dictionary<string, Animation> Animations { get; private set; }

    public event Action<SpriteAnimatorComponent, string> OnAnimationCompleted;

    public SpriteAnimatorComponent() : this([])
    { }

    public SpriteAnimatorComponent(Dictionary<string, Animation> animations) : base(null)
    {
        Animations = animations;
    }

    public void Play(string name, LoopMode loopMode = LoopMode.Loop)
    {
        CurrentAnimationState = AnimationState.Running;
        CurrentAnimation = Animations[name];
        CurrentAnimationName = name;
        CurrentLoopMode = loopMode;
        CurrentElapsedTime = 0f;
        FrameCount = CurrentAnimation.Frames.Count;
        FrameTime = (float)CurrentAnimation.FrameTime.TotalSeconds;

        SetFrame(0);
    }

    public SpriteAnimatorComponent AddAnimation(string name, Animation animation)
    {
        Animations[name] = animation;

        return this;
    }

    public void SetFrame(int frame)
    {
        CurrentFrame = frame;
        FrameTimeLeft = FrameTime;
        var region = CurrentAnimation.Frames[frame];
        Sprite = new Sprite(region);
    }

    public void NextFrame()
    {
        switch (CurrentLoopMode)
        {
            case LoopMode.Loop:
                SetFrame((CurrentFrame + 1) % FrameCount);
                break;
            case LoopMode.Once:
            case LoopMode.OnceClamp:
                var newFrame = CurrentFrame + 1;
                if (newFrame >= FrameCount)
                {
                    CompleteCurrentAnimation();
                    if (CurrentLoopMode is LoopMode.Once) SetFrame(0);
                    break;
                }

                SetFrame(newFrame);
                break;
        }
    }

    public void CompleteCurrentAnimation()
    {
        CurrentAnimationState = AnimationState.Completed;
        CurrentElapsedTime = 0f;

        OnAnimationCompleted?.Invoke(this, CurrentAnimationName);
    }

    public override void Update(GameTime gameTime)
    {
        bool noNeedToUpdate = CurrentAnimationState != AnimationState.Running
                              || CurrentAnimation is null;

        if (noNeedToUpdate) return;
        CurrentElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        FrameTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (ShouldPlayNextFrame())
        {
            NextFrame();
        }
    }

    private bool ShouldPlayNextFrame()
    {
        return FrameTimeLeft <= 0f;
    }
}
