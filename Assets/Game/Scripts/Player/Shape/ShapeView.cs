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
        public EShapeType ShapeID { get { return ShapeType; } }
        public ESwipeDirection SwipeDirection { get { return SwipeDir; } }

        [SerializeField] private EShapeType ShapeType; 
        [SerializeField] private ESwipeDirection SwipeDir;
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
    }
}
