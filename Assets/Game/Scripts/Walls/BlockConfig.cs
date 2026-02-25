using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "BlockConfig", menuName = "Scriptable Objects/Configs/BlockConfig")]
public class BlockConfig : ScriptableObject
{
    public LayerMask m_CollisionLayer;
    public List<BlockView> Blocks;
}
