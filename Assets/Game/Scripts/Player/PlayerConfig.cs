using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeConfig
{
    public EShapeType m_ShapeType;
    public GameObject m_ShapeView;
}

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/Configs/Player")]
public class PlayerConfig : ScriptableObject
{
    public PlayerView m_PlayerView;
    public List<ShapeConfig> m_ListOfShapes;
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
