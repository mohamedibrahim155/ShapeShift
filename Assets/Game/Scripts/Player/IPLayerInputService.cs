using System;
using UnityEngine;

namespace Scripts.Player
{
    public enum ESwipeDirection
    {
        NONE = -1,
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    }
    public interface IPLayerInputService
    {
        event Action<ESwipeDirection> OnSwipe;
        Vector2 endPosition { get; }
        Vector2 startTouchPosition { get; }

        public abstract void UpdateInputs();
        public abstract void CleanUp();

        public abstract void EnableInput(bool isActive);
    }
}
