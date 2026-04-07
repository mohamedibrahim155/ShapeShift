using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerShapeHighlighterView : MonoBehaviour
    {
        [SerializeField] private List<InvisibleShapeView> shapeViews = new List<InvisibleShapeView>();

        private void Reset()
        {
            shapeViews =  GetComponentsInChildren<InvisibleShapeView>().ToList();
        }
        private void OnEnable()
        {
            DisableColliders();
        }


        public void ShowShape(EShapeType shapeType)
        {
            foreach (var shapeView in shapeViews)
            {
                if (shapeView.ShapeID == shapeType)
                {
                    shapeView.Show();
                }
            }
        }

        public void Hide()
        {
            foreach (var shapeView in shapeViews)
            {
               shapeView.Hide();
            }
        }

        public void UpdateShapeColor(Color color)
        {
            foreach (var shapeView in shapeViews)
            {
                shapeView.SetColor(color);
            }
        }

        private void DisableColliders()
        {
            foreach (var item in shapeViews)
            {
                item.Collider.enabled = false;
            }
        }
    }
}
