using System.Runtime.InteropServices;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class FinishLineCamera : GameplayCamera
{

    [Inject] private CameraConfig cameraConfig;
    [Inject] private ICameraService cameraService;

    [SerializeField] private Transform targetTransform;
    [SerializeField] private CinemachineOrbitalFollow OrbitalFollow;
    
    private bool _HasBeenActivated = false;
    void Start()
    {
        _HasBeenActivated = false;
    }

    private void Reset()
    {
        OrbitalFollow = GetComponent<CinemachineOrbitalFollow>();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        CiniMachineRotation();
    }

    public void ActivateFinishCamera(Transform target)
    {
        _HasBeenActivated = true;
        targetTransform = target;
    }


    void CiniMachineRotation()
    {
        if (!_HasBeenActivated || !targetTransform) return;
        OrbitalFollow.HorizontalAxis.Value +=  cameraConfig.m_RotationSpeed * Time.deltaTime; // Increment the rotation angle based on the rotation speed and time
    }

   
}
