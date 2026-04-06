using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody Rigidbody;
        public Transform m_ShapeParent;


        private PlayerConfig playerConfig;
        private Dictionary<EShapeType,ShapeView> ShapeViews = new();
        private Dictionary<ESwipeDirection,ShapeView> ShapesViewByDirections = new();
        [SerializeField] private List<ShapeView> shapesList = new List<ShapeView>();

        [SerializeField] private PlayerShapeHighlighterView PlayerShapeHighlighterView;
       

        private Coroutine _transitionRoutine;

        public void ShowShape(EShapeType shapeType)
        {
            HideAllShapes();

            if (!ShapeViews.TryGetValue(shapeType, out ShapeView shape))
                return;

            shape.transform.localScale = Vector3.one;
            shape.Show();
        }

        public void PlayShapeTransition(EShapeType fromShapeType, EShapeType toShapeType, ESwipeDirection  swipeDirection)
        {
            if (playerConfig == null)
                return;

            if (!ShapeViews.TryGetValue(fromShapeType, out ShapeView fromShape))
                return;

            if (!ShapeViews.TryGetValue(toShapeType, out ShapeView toShape))
                return;

            if (fromShape == toShape)
            {
                toShape.Show();
                toShape.transform.localScale = Vector3.one;
                return;
            }

            if (_transitionRoutine != null)
            {
                StopCoroutine(_transitionRoutine);
            }

            _transitionRoutine = StartCoroutine(TransitionShapes(fromShape, toShape, swipeDirection));
        }


        public void HideAllShapes()
        {
            foreach (var item in ShapeViews)
            {
                item.Value.Hide();
            }
        }

        public void Initialize(PlayerConfig config)
        {
            playerConfig = config;

            ShapeViews.Clear();
            ShapesViewByDirections.Clear();

            foreach (ShapeView shape in shapesList)
            {
                if (shape == null) continue;

                AddShape(shape);
                shape.transform.localScale = Vector3.one;
                shape.Hide();
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

        public Dictionary<EShapeType, ShapeView> GetShapes()
        {
            return ShapeViews;
        }

        private IEnumerator TransitionShapes(ShapeView fromShape, ShapeView toShape, ESwipeDirection swipeDirection)
        {
            if (fromShape == null || toShape == null)
            {
                yield break;
            }

            fromShape.Show();
            toShape.Show();

            float startScale = Mathf.Clamp01(playerConfig.m_StartingScale);
            fromShape.transform.localScale = Vector3.one;
            toShape.transform.localScale = Vector3.one * startScale;

            SetShapeCollision(fromShape, false);
            SetShapeCollision(toShape, true);

            float duration = Mathf.Max(0.01f,playerConfig.m_TransitionDuration);
            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                float easedT = Mathf.SmoothStep(0f, 1f, t);
                float pulse = Mathf.Sin(easedT * Mathf.PI);

                float outScale = Mathf.Lerp(1f, 0f, easedT);
                float inScale = Mathf.Lerp(startScale, 1f, easedT);

                fromShape.transform.localScale = ComputeDirectionalScale(outScale, pulse, swipeDirection, false);
                toShape.transform.localScale = ComputeDirectionalScale(inScale, pulse, swipeDirection, true);

                yield return null;
            }

            fromShape.transform.localScale = Vector3.one;
            fromShape.Hide();
            SetShapeCollision(fromShape, false);

            toShape.transform.localScale = Vector3.one;
            toShape.Show();
            SetShapeCollision(toShape, true);

            _transitionRoutine = null;
        }

        private Vector3 ComputeDirectionalScale(float baseUniformScale, float pulse, ESwipeDirection direction, bool isIncomingShape)
        {
            float axisStretch = Mathf.Lerp(1f,playerConfig.m_DirectionalStretch, pulse);
            float axisSquash = Mathf.Lerp(1f, playerConfig.m_CrossAxisSquash, pulse);

            if (!isIncomingShape)
            {
                axisStretch = Mathf.Lerp(axisStretch, axisSquash, 0.6f);
            }

            Vector3  directional = direction switch
            {
                ESwipeDirection.LEFT => new Vector3(axisStretch, axisSquash, axisSquash),
                ESwipeDirection.RIGHT => new Vector3(axisStretch, axisSquash, axisSquash),
                ESwipeDirection.UP => new Vector3(axisSquash, axisSquash, axisStretch),
                ESwipeDirection.DOWN => new Vector3(axisSquash, axisSquash, axisStretch),
                _ => new Vector3(axisSquash, axisStretch, axisSquash)
            };


            float uniform = Mathf.Max(0f, baseUniformScale);
            return Vector3.Scale(Vector3.one * uniform, directional);
        }

        private static void SetShapeCollision(ShapeView shape, bool enabled)
        {
            if (shape == null || shape.Collider == null)
            {
                return;
            }

            shape.Collider.enabled = enabled;
        }

        public void SetShapeCollidersEnabled(bool enabled)
        {
            foreach (var item in ShapeViews)
            {
                SetShapeCollision(item.Value, enabled);
            }
        }

        public void ClearShapeContraints()
        {
            foreach (var item in ShapeViews)
            {
                if (item.Value.Rigidbody != null)
                {
                    item.Value.Rigidbody.constraints = RigidbodyConstraints.None;
                }
            }
        }


        public void ShowHightLight(Vector3 position, Color color, EShapeType type)
        {
            PlayerShapeHighlighterView.transform.position = position;
            PlayerShapeHighlighterView.UpdateShapeColor(color);
            PlayerShapeHighlighterView.Hide();
            PlayerShapeHighlighterView.ShowShape(type);
        }

        public void HideHighlight()
        {
            PlayerShapeHighlighterView.Hide();
        }
        public void SetAllShapeRigidbodyKinematic(bool isKinematic)
        {
            foreach (var item in ShapeViews)
            {
                item.Value.Rigidbody.isKinematic = isKinematic;
            }
        }


        private void Reset()
        {
            Rigidbody = GetComponent<Rigidbody>();
            shapesList = GetComponentsInChildren<ShapeView>().ToList();
            PlayerShapeHighlighterView = GetComponentInChildren<PlayerShapeHighlighterView>();
        }

    

    }
}
