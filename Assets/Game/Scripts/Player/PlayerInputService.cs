using System;
using UnityEngine;
using Zenject;
using Scripts.GameService;
public class PlayerInputService : IPLayerInputService
{
    public Vector2 endPosition { get; private set; }

    public Vector2 startTouchPosition { get; private set; }

    public event Action<SwipeDirection> OnSwipe;


    private PlayerConfig m_PlayerConfig;
    private IGameLoopService m_GameloopService;



    [Inject]
    private void Construct(PlayerConfig config,  IGameLoopService gameloop)
    {
        m_PlayerConfig = config;
        m_GameloopService = gameloop;

        m_GameloopService.OnUpdateTick += UpdateInputs;
    }


    public void UpdateInputs()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endPosition = touch.position;
                Vector2 distance = endPosition - startTouchPosition;

                if(distance.magnitude < m_PlayerConfig.m_SwipeThreshold * m_PlayerConfig.m_SwipeThreshold)
                {
                    return;
                }

                if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
                {
                    if (distance.x > 0)
                    {
                        OnSwipe?.Invoke(SwipeDirection.RIGHT);
                        Debug.Log("Right");
                    }
                    else
                    {
                        OnSwipe?.Invoke(SwipeDirection.LEFT);

                        Debug.Log("Left");
                    }
                }
                else
                {
                    if (distance.y > 0)
                    {
                        OnSwipe?.Invoke(SwipeDirection.UP);

                        Debug.Log("Up");
                    }
                    else
                    {
                        OnSwipe?.Invoke(SwipeDirection.DOWN);
                        Debug.Log("Down");
                    }
                }
            }

        }

#endif

    }
}

