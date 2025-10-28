using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    public List<GameObject> m_LevelParts;
    [SerializeField] private List<BlockView> m_Blocks;


    private void Reset()
    {
        m_Blocks = GetComponentsInChildren <BlockView>().ToList();
    }
    public BlockView GetViewAt(int index)
    {
        return m_Blocks[index];
    }


}
