using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Scripts.GameService;

namespace Scripts.Player
{
    [System.Serializable]
    public class ShapeConfig
    {
        public EShapeType m_ShapeType;
        public ESwipeDirection m_SwipeDirection;
        public ShapeView m_ShapeView;
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


        [Header("Spawn position")]
        public Vector3 m_SpawnPosition = Vector3.zero;

        [Header("Speed")]
        public float m_MoveSpeed = 5f;
        public float m_GroundCheckDistance = 5;
        public float m_FallTimer = .5f;

        public void SetCurrentShape(EShapeType shapeType)
        {
            m_CurrentShapeIndex = (int)shapeType;
            m_CurrentShapeType = shapeType;
        }
    }
}
