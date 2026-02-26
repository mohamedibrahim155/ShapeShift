using System.Diagnostics.Contracts;
using UnityEngine;

namespace Scripts.Player
{
    public class ShapeView : MonoBehaviour
    {
        public Collider Collider;
        public MeshRenderer MeshRenderer;
        public Rigidbody Rigidbody;
        public Transform ParentTransform;
        public EShapeType ShapeID;
        public ESwipeDirection SwipeDirection { get; private set; }
        public void Reset()
        {
            Collider = GetComponentInChildren<Collider>();
            MeshRenderer = GetComponentInChildren<MeshRenderer>();
            Rigidbody = GetComponentInChildren<Rigidbody>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Setup(EShapeType type, ESwipeDirection swipeDirection,Transform parent)
        {
            ShapeID = type;
            SwipeDirection = swipeDirection;
            ParentTransform = parent;
        }
    }
}
