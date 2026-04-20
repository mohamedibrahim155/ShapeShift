using Scripts.UI;
using UnityEngine;
using Zenject;
using DG.Tweening;
using System;

namespace Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        public EWindowID ID;
        public bool m_OpenOnStart = false;
        public CanvasGroup canvasGroup;

        public event Action OnWindowOpened = delegate { };
        public event Action OnWindowClosed = delegate { };
        public virtual void Reset()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        }


        public virtual void Open(float time = 0.5f) 
        {
            canvasGroup.DOFade(1, time);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;


            OnWindowOpened.Invoke();
        }
        public virtual void Close(float time = 0.5f) 
        {

            canvasGroup.DOFade(0, time);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            OnWindowClosed.Invoke();

        }

    }
}
