using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlayerView : MonoBehaviour
{

    [SerializeField] GameObject[] shapeTransforms;

    [Inject] private PlayerConfig playerConfig;

    private Dictionary<EShapeType, int> shapeTypeToIndex = new Dictionary<EShapeType, int>()
    {
        { EShapeType.CUBE, 0 },
        { EShapeType.SPHERE, 1 },
        { EShapeType.CYLINDER, 2 },
        { EShapeType.CAPSULE, 3 }
    };
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

    public void EnableShape(EShapeType shapeType)
    {
        int shapeIndex = shapeTypeToIndex[shapeType];
        shapeTransforms[shapeIndex].SetActive(true);
    }


    public void DisableShape(int shapeIndex)
    {
         shapeTransforms[shapeIndex].SetActive(false);
    }

  
}
