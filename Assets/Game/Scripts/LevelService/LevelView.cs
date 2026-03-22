using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class LevelView : MonoBehaviour
{

    public Collider Collider;
    public MeshRenderer MeshRenderer;
    [SerializeField] private List<BlockView> _blockViews = new List<BlockView>();

    private const float OFFSET_Y = 3;
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
        MeshRenderer = GetComponent<MeshRenderer>();
        _blockViews = GetComponentsInChildren<BlockView>(true).ToList();
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Vector3 bounds = MeshRenderer.bounds.size + Vector3.up * OFFSET_Y;

        Gizmos.DrawWireCube(transform.position, bounds);
    }

    public float GetZBounds()
    {
       Bounds bounds = MeshRenderer.bounds;
        return bounds.size.z;
    }

    public List<BlockView> GetBlocks() { return _blockViews; }

    public bool HasBlocks()
    {
        return _blockViews.Count > 0;
    }



}
