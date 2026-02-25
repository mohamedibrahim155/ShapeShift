using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    public List<GameObject> m_LevelParts;
    [SerializeField] private Transform StartPoint;
    [SerializeField] private Transform EndPoint;
    [SerializeField] private Transform GroundTransform;

    public Collider Collider;

    private List<BlockView> _blockViews = new List<BlockView>();

    public void AddBlock(BlockView blockView)
    {
        _blockViews.Add(blockView);
    }

    public BlockView GetBlock(int index)
    {
        return _blockViews[index];
    }

    public void Reset()
    {
        Collider = GetComponentInChildren<Collider>();
    }



}
