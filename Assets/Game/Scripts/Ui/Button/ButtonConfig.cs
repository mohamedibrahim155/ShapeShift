using UnityEngine;
namespace Scripts.UI
{
    [CreateAssetMenu(fileName = "ButtonConfig", menuName = "Scriptable Objects/Configs/ButtonConfig")]
    public class ButtonConfig : ScriptableObject
    {
        public float m_ScaleFactor = 0.9f;
        public float m_Duration = 0.1f;
        public float m_PumpDuration = 0.5f;
    }
}
