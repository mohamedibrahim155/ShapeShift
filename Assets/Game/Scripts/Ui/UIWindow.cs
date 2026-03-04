using Scripts.UI;
using UnityEngine;
using Zenject;

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


        public virtual void Open() 
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = false;
        }
        public virtual void Close() 
        {
            gameObject.SetActive(false);

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
