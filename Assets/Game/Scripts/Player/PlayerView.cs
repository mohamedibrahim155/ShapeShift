using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Scripts.Player
{
    public class PlayerView : MonoBehaviour
    {
        public Transform m_ShapeParent;

        [SerializeField] public List<ShapeView> shapeTransforms;

        [Inject] private PlayerConfig playerConfig;

        public void EnableShape(EShapeType shapeType)
        {
            int shapeIndex = (int)shapeType;

            playerConfig.SetCurrentShape(shapeType);

            shapeTransforms[shapeIndex].Show();

        }

        public void ChangeShape(EShapeType eShapeType)
        {
            DisableShape(playerConfig.m_CurrentShapeType);
            EnableShape(eShapeType);
        }


        public void DisableShape(EShapeType shapeIndex)
        {
            shapeTransforms[(int)shapeIndex].Hide();
        }

        public void SpawnShapes(DiContainer diContainer)
        {
            m_ShapeParent = new GameObject("ShapeParent").transform;
            m_ShapeParent.transform.parent = (transform);

            shapeTransforms = new List<ShapeView>(new ShapeView[playerConfig.m_ListOfShapes.Count]);

            foreach (ShapeConfig item in playerConfig.m_ListOfShapes)
            {
                ShapeView shapeInstance = diContainer.InstantiatePrefabForComponent<ShapeView>(item.m_ShapeView);
                shapeInstance.transform.SetParent(m_ShapeParent);

                shapeInstance.Hide();
                shapeInstance.Setup(item.m_ShapeType, m_ShapeParent);
                shapeTransforms[(int)item.m_ShapeType] = shapeInstance;
            }
        }

        public ShapeView GetShape(EShapeType shapeType)
        {
            return shapeTransforms[(int)shapeType];
        }

    }
}
