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
            StartShapeTransition(GetShape(eShapeType), ESwipeDirection.NONE);

        }

        public void ChangeShapeForDirection(ESwipeDirection swipeDirection)
        {
            ShapeView changeShape = GetShape(swipeDirection);
            StartShapeTransition(changeShape, swipeDirection);

        }


        public IEnumerator DisableShape(EShapeType shapeIndex)
        {
            ShapeView currentshape = GetShape(shapeIndex);
            yield return new WaitForSeconds(0.2f);
            currentshape.Hide();
        }

        public void Hide()
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
                AddShape(shape);
                shape.transform.localScale = Vector3.one;
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

        public void DisbaleColliders()
        {
            foreach (var item in ShapeViews)
            {
                SetShapeCollision(item.Value, false);
            }
        }

        public Dictionary<EShapeType, ShapeView> GetShapes()
        {
            return ShapeViews;
        }

        private void StartShapeTransition(ShapeView targetShape, ESwipeDirection swipeDirection)
        {
            if (targetShape == null || playerConfig == null)
            {
                return;
            }

            EShapeType currentShapeType = playerConfig.m_CurrentShapeType;
            ShapeView currentShape = GetShape(currentShapeType);

            if (currentShape == targetShape)
            {
                currentShape.Show();
                currentShape.transform.localScale = Vector3.one;
                SetShapeCollision(currentShape, true);
                return;
            }

            if (_transitionRoutine != null)
            {
                StopCoroutine(_transitionRoutine);
            }

            _transitionRoutine = StartCoroutine(TransitionShapes(currentShape, targetShape, swipeDirection));
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
            playerConfig.SetCurrentShape(toShape.ShapeID);

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

            Vector3 directional = Vector3.one;
            switch (direction)
            {
                case ESwipeDirection.LEFT:
                case ESwipeDirection.RIGHT:
                    directional = new Vector3(axisStretch, axisSquash, axisSquash);
                    break;
                case ESwipeDirection.UP:
                case ESwipeDirection.DOWN:
                    directional = new Vector3(axisSquash, axisSquash, axisStretch);
                    break;
                default:
                    directional = new Vector3(axisSquash, axisStretch, axisSquash);
                    break;
            }

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


        public void UpdateHighligherPosition(Vector3 position, Color color, EShapeType type)
        {
            PlayerShapeHighlighterView.transform.position = position;

            PlayerShapeHighlighterView.UpdateShapeColor(color);

            PlayerShapeHighlighterView.Hide();
            PlayerShapeHighlighterView.ShowShape(type);
        }

        public void HideTransparentShapes()
        {
            PlayerShapeHighlighterView.Hide();
        }


        private void Reset()
        {
            Rigidbody = GetComponent<Rigidbody>();
            shapesList = GetComponentsInChildren<ShapeView>().ToList();
            PlayerShapeHighlighterView = GetComponentInChildren<PlayerShapeHighlighterView>();
        }

    }
}
