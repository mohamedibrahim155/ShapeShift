using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class StarUIView : MonoBehaviour
{
    [SerializeField] private Image starImage;

    public void SetSprite(Sprite sprite)
    {
        starImage.sprite = sprite;
    }

    public void SetVisibility(bool isVisible)
    {
        starImage.DOFade(isVisible ? 1 : 0, 0.5f);
    }

    private void Reset()
    {
        starImage = GetComponentInChildren<Image>();
    }
}
