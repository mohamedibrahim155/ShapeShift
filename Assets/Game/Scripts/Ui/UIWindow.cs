using Scripts.UI;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        public EWindowID ID;
        public bool m_OpenOnStart = false;
        public CanvasGroup canvasGroup;

        private void Reset()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        }


        public virtual void Open(float time = 0.5f) 
        {
            gameObject.SetActive(true);
            canvasGroup.DOFade(1, time);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        public virtual void Close(float time = 0.5f) 
        {
            gameObject.SetActive(false);

            canvasGroup.DOFade(0, time);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

    }
}
