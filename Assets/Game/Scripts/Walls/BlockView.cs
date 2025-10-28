using UnityEngine;

public class BlockView : MonoBehaviour
{
    [SerializeField] private EBlockType m_BlockType;
    [SerializeField] private Collider m_Collider;

    private void Reset()
    {
        m_Collider = GetComponent<Collider>();
    }

}
