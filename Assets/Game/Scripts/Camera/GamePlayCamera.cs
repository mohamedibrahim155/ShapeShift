using UnityEngine;
using Unity.Cinemachine;
public class GameplayCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_CinemachineCamera;
    [SerializeField] private CinemachineFollow m_Follow;
    [SerializeField] private CinemachineRotationComposer m_rotationComposer;

    public CinemachineCamera CinemachineCamera => m_CinemachineCamera;
    public CinemachineFollow Follow => m_Follow;
    public CinemachineRotationComposer RotationComposer => m_rotationComposer;

    private void Reset()
    {
        m_CinemachineCamera = GetComponent<CinemachineCamera>();
        m_Follow = GetComponent<CinemachineFollow>();
        m_rotationComposer = GetComponent<CinemachineRotationComposer>();
    }


    //sets camera settings based on provided camera settings data
    public virtual void SetCameraSettings(CinimachineCameraSettings cameraSettings)
    {
        if (m_Follow != null)
            m_Follow.FollowOffset = cameraSettings.FollowOffset;
        m_rotationComposer.TargetOffset = cameraSettings.TargetOffset;
        m_rotationComposer.Composition.ScreenPosition = cameraSettings.ScreenPosition;
    }

    // Call this when you respawn / set a new player
 

    public void SetCameraEnabled(bool state)
    {
        gameObject.SetActive(state);
        m_CinemachineCamera.enabled = state;

    }

}
