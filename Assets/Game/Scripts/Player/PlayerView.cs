using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Scripts.Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody Rigidbody;
        public Transform m_ShapeParent;
        public Animator Animator;


        private PlayerConfig playerConfig;
        private Dictionary<EShapeType,ShapeView> ShapeViews = new();
        private Dictionary<ESwipeDirection,ShapeView> ShapesViewByDirections = new();
        private List<ShapeView> shapesList = new List<ShapeView>();

        private void Start()
        {
        }
        public void EnableShape(EShapeType shapeType)
        {
            playerConfig.SetCurrentShape(shapeType);

            GetShape(shapeType).Show();
        }

        public void EnableShape(ESwipeDirection direction)
        {
            ShapeView currentshape = GetShape(direction);
            EShapeType ID = currentshape.ShapeID;

            //Sets ID
            playerConfig.SetCurrentShape(ID);

            currentshape.Show();

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
            ShapeView currentshape = GetShape(shapeIndex);

            currentshape.Hide();
        }

        public void Initialize(PlayerConfig config)
        {
            playerConfig = config;

            foreach (ShapeView shape in shapesList)
            {
                AddShape(shape);
            }

            //Disable all shapes at the start of the game
            foreach (var item in ShapeViews)
            {
                item.Value.Hide();
            }
        }

        private void AddShape(ShapeView view)
        {
            ShapeViews.Add(view.ShapeID, view);
            ShapesViewByDirections.Add(view.SwipeDirection, view);
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
            Animator = GetComponent<Animator>();
            shapesList = GetComponentsInChildren<ShapeView>().ToList();
        }

        public void DisbaleColliders()
        {
            foreach (var item in ShapeViews)
            {
                item.Value.Collider.enabled = false;
            }
        }

    }
}
