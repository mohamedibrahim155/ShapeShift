using UnityEngine;

namespace Scripts.Player
{
    public class InvisibleShapeView : ShapeView
    {
        [SerializeField] private Material material;


        private void Awake()
        {
            material = MeshRenderer.sharedMaterial;
        }

        public void SetColor(Color materialColor)
        {
            material.color = materialColor;
        }

    }
}
