using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;


namespace Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonVisuals : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Button button;
        private Vector2 initialScale;

        public float scaleFactor = 0.9f;

        [Inject] private ButtonConfig buttonConfig;

        private void Awake()
        {
            initialScale = transform.localScale;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Button Pressed");
           transform.DOScale(Vector2.one * buttonConfig.m_ScaleFactor, buttonConfig.m_Duration);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
           transform.DOScale(initialScale, buttonConfig.m_Duration);
        }

        private void Reset()
        {
            button = GetComponent<Button>();
        }

       
    }
}
