using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;


namespace Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonVisuals : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _buttonTextField;
        [SerializeField] private bool isPumping; 
        private Vector2 initialScale;

        public bool IsPumping { get { return isPumping; } }
        public Button Button { get { return _button; } }
        public  TextMeshProUGUI ButtonTextField { get { return _buttonTextField; } }


        [Inject] private ButtonConfig buttonConfig;

        private void Awake()
        {
            initialScale = transform.localScale;

         
        }

        private void OnEnable()
        {
            if (IsPumping)
                AnimateButton();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
           transform.DOKill();
           transform.DOScale(Vector2.one * buttonConfig.m_ScaleFactor, buttonConfig.m_Duration);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isPumping)
            {
                AnimateButton();
            }
            else
                transform.DOScale(initialScale, buttonConfig.m_Duration);
        }

        public void AnimateButton()
        {

            Transform t = _button.transform;
            t.DOKill();

            t.DOScale(1f, buttonConfig.m_Duration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {

                    t.DOScale(buttonConfig.m_ScaleFactor, buttonConfig.m_PumpDuration)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo);
                });

        }


        private void Reset()
        {
            _button = GetComponent<Button>();
            _buttonTextField = GetComponentInChildren<TextMeshProUGUI>();
        }

       
    }
}
