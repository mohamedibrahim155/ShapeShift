using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlayerView : MonoBehaviour
{

    [SerializeField] GameObject[] shapeTransforms;

    [Inject] private PlayerConfig playerConfig;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableShape(int shapeIndex)
    {  
         shapeTransforms[shapeIndex].SetActive(true);
    }

    public void DisableShape(int shapeIndex)
    {
         shapeTransforms[shapeIndex].SetActive(false);
    }

  
}
