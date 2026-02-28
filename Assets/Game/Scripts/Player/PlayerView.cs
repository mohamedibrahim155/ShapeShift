using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Scripts.Player
{
    public class PlayerView : MonoBehaviour
    {
         public Rigidbody Rigidbody;
        public Transform m_ShapeParent;

        [SerializeField] public Dictionary<EShapeType,ShapeView> ShapeViews = new();
        [SerializeField] public Dictionary<ESwipeDirection,ShapeView> ShapesViewByDirections = new();

        [Inject] private PlayerConfig playerConfig;

        public void EnableShape(EShapeType shapeType)
        {
            playerConfig.SetCurrentShape(shapeType);

            ShapeViews[shapeType].Show();

        }

        public void EnableShape(ESwipeDirection direction)
        {
            ShapeView currentshape = GetShape(direction);

            playerConfig.SetCurrentShape(currentshape.ShapeID);
            ShapesViewByDirections[direction].Show();

        }

        public void ChangeShape(EShapeType eShapeType)
        {
            DisableShape(playerConfig.m_CurrentShapeType);
            EnableShape(eShapeType);
        }

        public void ChangeShapeForDirection(ESwipeDirection swipeDirection)
        {
            DisableShape(playerConfig.m_CurrentShapeType);
            EnableShape(swipeDirection);
        }


        public void DisableShape(EShapeType shapeIndex)
        {
            ShapeViews[shapeIndex].Hide();
        }

        public void SpawnShapes(DiContainer diContainer)
        {
            m_ShapeParent = new GameObject("ShapeParent").transform;
            m_ShapeParent.transform.parent = (transform);


            foreach (ShapeConfig item in playerConfig.m_ListOfShapes)
            {
                ShapeView shapeInstance = diContainer.InstantiatePrefabForComponent<ShapeView>(item.m_ShapeView);
                shapeInstance.transform.SetParent(m_ShapeParent);

                AddShape(shapeInstance, item);

                shapeInstance.Hide();

            }
        }

        private void AddShape(ShapeView shapeInstance, ShapeConfig shapeConfig)
        {
            shapeInstance.Setup(shapeConfig.m_ShapeType, shapeConfig.m_SwipeDirection, m_ShapeParent);

            ShapeViews.Add(shapeConfig.m_ShapeType, shapeInstance);
            ShapesViewByDirections.Add(shapeConfig.m_SwipeDirection, shapeInstance);
        }

        public ShapeView GetShape(EShapeType shapeType)
        {
            return ShapeViews[shapeType];
        }
        public ShapeView GetShape(ESwipeDirection direction)
        {
            return ShapesViewByDirections[direction];
        }

        private void Reset()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

    }
}
