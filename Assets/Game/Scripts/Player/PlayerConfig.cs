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

        [Header("MoveData")]
        public float m_MoveSpeed = 5f;
        public float m_FallTimer = .5f;

        [Header("GroundCheck")]
        public float m_GroundCheckDistance = 5;
        public LayerMask m_GroundLayer;

        public LayerMask m_PlayerLayer;

        public float m_FinishLineWaitTimer =2;
        public bool m_HasPlayerFinished = false;


        [Header("Shape Transition")]
        public float m_TransitionDuration = 0.2f;
        public float m_DirectionalStretch = 1.2f;
        public float m_CrossAxisSquash = 0.85f;
        public float m_StartingScale = 0.05f;

        public void SetCurrentShape(EShapeType shapeType)
        {
            m_CurrentShapeIndex = (int)shapeType;
            m_CurrentShapeType = shapeType;
        }
    }
}
