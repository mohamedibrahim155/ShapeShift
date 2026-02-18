using UnityEngine;

[CreateAssetMenu(fileName = "BlockConfig", menuName = "Scriptable Objects/Configs/BlockConfig")]
public class BlockConfig : ScriptableObject
{
    public LayerMask m_CollisionLayer;
}
