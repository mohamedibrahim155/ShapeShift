using System.Runtime.InteropServices;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class FinishLineCamera : GameplayCamera
{

    [Inject] private CameraConfig cameraConfig;
    [Inject] private ICameraService cameraService;

    [SerializeField] private CinemachineOrbitalFollow OrbitalFollow;
    
    private bool isActive;

    private void Reset()
    {
        OrbitalFollow = GetComponent<CinemachineOrbitalFollow>();
    }

    private void OnEnable()
    {
        cameraService.OnFinishlineCameraActived += Activate;
    }

    private void OnDisable()
    {
        cameraService.OnFinishlineCameraActived -= Activate;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        CiniMachineRotation();
    }

    //Activates on Finishline Triggers
    public void Activate()
    {
        isActive =  true;
    }


    void CiniMachineRotation()
    {
        if (!isActive) return;
        OrbitalFollow.HorizontalAxis.Value +=  cameraConfig.m_RotationSpeed * Time.deltaTime; // Increment the rotation angle based on the rotation speed and time
    }

   
}
