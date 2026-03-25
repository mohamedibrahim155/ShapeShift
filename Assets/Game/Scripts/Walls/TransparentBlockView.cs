using DG.Tweening;
using System;
using UnityEngine;

public class TransparentBlockView : MonoBehaviour
{
    private Vector3 initalScale;
    private Color initalColor;

    [SerializeField] private MeshRenderer MeshRenderer;
    private void Start()
    {
        initalScale = transform.localScale;
        initalColor = MeshRenderer.sharedMaterial.color;
    }
    public void AnimateScaling(float scaleFactor, float duration, TweenCallback OnCompleteScaling)
    {

        transform.DOScale(Vector3.one * scaleFactor, duration).OnComplete(OnCompleteScaling).SetEase(Ease.OutBack);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        transform.localScale = initalScale;
    }

    public void SetColor(Color color)
    {
        //alphaValue
        color.a = 0.5f;
        MeshRenderer.material.color = (color);
    }

    private void Reset()
    {
        MeshRenderer = GetComponentInChildren<MeshRenderer>(true);
    }

}
