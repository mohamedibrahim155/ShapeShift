using UnityEngine;

namespace Scripts.UI
{
    [CreateAssetMenu(fileName = "UiConfig", menuName = "Scriptable Objects/Configs/UiConfig")]
    public class UIConfig : ScriptableObject
    {
        public UICanvasView m_CanvasView;
    }
}
