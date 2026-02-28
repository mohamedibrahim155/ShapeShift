using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class LevelView : MonoBehaviour
{

    public Collider Collider;
    public MeshRenderer MeshRenderer;
    private List<BlockView> _blockViews = new List<BlockView>();

    [SerializeField] private Vector3 Size;

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
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Size);
    }

    public float GetZBounds()
    {
       Bounds bounds = MeshRenderer.bounds;
        return bounds.size.z;
    }



}
