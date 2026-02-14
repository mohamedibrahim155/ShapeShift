using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/Configs/Player")]
public class PlayerConfig : ScriptableObject
{
    public PlayerView m_PlayerView;
    public float m_SwipeThreshold = 0.5f;

    [Header("Shapes")]
    public int m_CurrentShapeIndex = 0;
    public EShapeType m_CurrentShapeType;


    public void SetCurrentShape(EShapeType shapeType)
    {
        m_CurrentShapeIndex = (int)shapeType;
        m_CurrentShapeType = shapeType;
    }
}
