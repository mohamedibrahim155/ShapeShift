using System;
using UnityEngine;


public enum SwipeDirection
{
    NONE = -1,
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3
}
public interface IPLayerInputService
{
    event Action<SwipeDirection> OnSwipe;
    public Vector2 endPosition { get; }
    public Vector2 startTouchPosition { get; }

    public abstract void UpdateInputs();
}
