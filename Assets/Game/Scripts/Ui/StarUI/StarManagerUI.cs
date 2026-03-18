using UnityEngine;

public class StarManagerUI : MonoBehaviour
{
    [SerializeField] private StarUIView[] starsViews;

    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    private const int MAXSTARS = 3;
    public void SetStars(int filledStars)
    {
        int starCount = Mathf.Clamp(filledStars, 0, MAXSTARS);

        for (int i = 0; i < starsViews.Length; i++)
        {
            if (starCount - 1 < i) 
            {
                UpdateStarSpite(starsViews[i], filledStar);
            }
            else
            {
                UpdateStarSpite(starsViews[i], emptyStar);
            }
        }

    }

    [ContextMenu("ResetStars")]
    public void ResetStars()
    {
        if (starsViews == null) return;
        
        foreach (var starView in starsViews)
        {
            UpdateStarSpite(starView, emptyStar);
        }
    }

    private void UpdateStarSpite(StarUIView view ,Sprite sprite)
    {
        if (view  == null || sprite == null)
        {
            return;
        }

        view.SetSprite(sprite);
    }

    private void Reset()
    {
        starsViews = GetComponentsInChildren<StarUIView>();
    }
}
