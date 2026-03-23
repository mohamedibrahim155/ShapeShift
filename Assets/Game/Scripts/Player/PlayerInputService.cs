using System;
using UnityEngine;
using Zenject;
using Scripts.GameService;
using Unity.VisualScripting;

namespace Scripts.Player
{
    public class PlayerInputService : IPLayerInputService
    {
        private bool _isInputEnabled = false;
        public Vector2 endPosition { get; private set; }

        public Vector2 startTouchPosition { get; private set; }

        public event Action<ESwipeDirection> OnSwipe;


        private PlayerConfig m_PlayerConfig;

        private IGameLoopService m_GameLoopService;

        private bool wasPressed = false;

        [Inject]
        private void Construct(PlayerConfig config, IGameLoopService gameloop)
        {
            m_PlayerConfig = config;
            m_GameLoopService = gameloop;
        }

        // Update touch inputs and detect swipe direction
        public void UpdateInputs()
        {
            if (!_isInputEnabled) return;

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

                    if (distance.magnitude < m_PlayerConfig.m_SwipeThreshold * m_PlayerConfig.m_SwipeThreshold)
                    {
                        return;
                    }

                    if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
                    {
                        if (distance.x > 0)
                        {
                            //Right Swipe
                            OnSwipe?.Invoke(ESwipeDirection.RIGHT);
                            Debug.Log("Right");
                        }
                        else
                        {
                            //Left Swipe
                            OnSwipe?.Invoke(ESwipeDirection.LEFT);

                            Debug.Log("Left");
                        }
                    }
                    else
                    {
                        if (distance.y > 0)
                        {
                            //Up Swipe
                            OnSwipe?.Invoke(ESwipeDirection.UP);

                            Debug.Log("Up");
                        }
                        else
                        {
                            //Down Swipe
                            OnSwipe?.Invoke(ESwipeDirection.DOWN);
                            Debug.Log("Down");
                        }
                    }
                }

            }

#endif

#if UNITY_EDITOR_WINDOWS || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                startTouchPosition = Input.mousePosition;
                wasPressed = true;
            }
            else if (Input.GetMouseButtonUp(0) && wasPressed)
            {
                endPosition = Input.mousePosition;
                Vector2 distance = endPosition - startTouchPosition;
                if (distance.magnitude < m_PlayerConfig.m_SwipeThreshold * m_PlayerConfig.m_SwipeThreshold)
                {
                    return;
                }
                if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
                {
                    if (distance.x > 0)
                    {
                        //Right Swipe
                        OnSwipe?.Invoke(ESwipeDirection.RIGHT);
                        Debug.Log("Windows mouse swipe Right");
                    }
                    else
                    {
                        //Left Swipe
                        OnSwipe?.Invoke(ESwipeDirection.LEFT);
                        Debug.Log("Windows mouse swipe Left");
                    }
                }
                else
                {
                    if (distance.y > 0)
                    {
                        //Up Swipe
                        OnSwipe?.Invoke(ESwipeDirection.UP);
                        Debug.Log("Windows mouse swipe Up");
                    }
                    else
                    {
                        //Down Swipe
                        OnSwipe?.Invoke(ESwipeDirection.DOWN);
                        Debug.Log("Windows mouse swipe Down");
                    }
                }

                wasPressed = false;

            }

            Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (inputAxis.magnitude > 0.1f)
            {
                if (Mathf.Abs(inputAxis.x) > Mathf.Abs(inputAxis.y))
                {
                    if (inputAxis.x > 0)
                    {
                        //Right Swipe
                        OnSwipe?.Invoke(ESwipeDirection.RIGHT);
                        Debug.Log("Windows key - Right");
                    }
                    else
                    {
                        //Left Swipe
                        OnSwipe?.Invoke(ESwipeDirection.LEFT);
                        Debug.Log("Windows key -Left");
                    }
                }
                else
                {
                    if (inputAxis.y > 0)
                    {
                        //Up Swipe
                        OnSwipe?.Invoke(ESwipeDirection.UP);
                        Debug.Log("Windows key -Up");
                    }
                    else
                    {
                        //Down Swipe
                        OnSwipe?.Invoke(ESwipeDirection.DOWN);
                        Debug.Log("Windows key -Down");
                    }
                }
            }


#endif


        }

        public void CleanUp()
        {
            DestroyInputController();
        }

        public void EnableInput(bool isActive)
        {
            _isInputEnabled = isActive;
        }

        public void SpawnInputController()
        {
            m_GameLoopService.OnUpdateTick += UpdateInputs;

            _isInputEnabled = true;
        }

        public void DestroyInputController()
        {
            m_GameLoopService.OnUpdateTick -= UpdateInputs;
            _isInputEnabled = false;

        }
    }
}

