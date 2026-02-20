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

        public void Setup(EShapeType type, Transform parent)
        {
            ShapeID = type;
            ParentTransform = parent;
        }
    }
}
