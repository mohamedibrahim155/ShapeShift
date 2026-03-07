using UnityEngine;
using Unity.Cinemachine;

public class CameraView : MonoBehaviour
{
    [Header("Main Camera")]
    public Camera m_Camera;

    [Header("Cinemachine Cameras")]
    [SerializeField] private GameplayCamera m_StartGameplayCamera;
    [SerializeField] private GameplayCamera m_FollowGameplayCamera;
    [SerializeField] private GameplayCamera m_FinishLineGameplayCamera;
    [SerializeField] private CinemachineBrain m_CinemachineBrain;

    private CameraConfig m_CameraConfig;
    private void Reset()
    {
        m_Camera = GetComponentInChildren<Camera>();
        m_CinemachineBrain = GetComponentInChildren<CinemachineBrain>();
        m_StartGameplayCamera = transform.GetChild(1).GetComponent<GameplayCamera>();
        m_FollowGameplayCamera = transform.GetChild(2).GetComponent<GameplayCamera>();
        m_FinishLineGameplayCamera = transform.GetChild(3).GetComponent<GameplayCamera>();
    }


    //returns  camera based on type
    public GameplayCamera GetCamera(ECameraType cameraType)
    {
        switch (cameraType)
        {
            case ECameraType.START_CAMERA:
                return m_StartGameplayCamera;
            case ECameraType.FOLLOW_CAMERA:
                return m_FollowGameplayCamera;
            case ECameraType.FINISHLINE_CAMERA:
                return m_FinishLineGameplayCamera;
            default:
                return null;
        }
    }

    //assigns Camera Cofnig data to this view
    public void Setup(CameraConfig cameraConfig)
    {
        m_CameraConfig = cameraConfig;

        //sets camera settings based on config data
        SetCameraSettings();
    }

    //sets camera settings based on config data
    private void SetCameraSettings()
    {
        m_StartGameplayCamera.SetCameraSettings(m_CameraConfig.StartCameraSettings);
        m_FollowGameplayCamera.SetCameraSettings(m_CameraConfig.FollowCameraSettings);
        m_FinishLineGameplayCamera.SetCameraSettings(m_CameraConfig.FinishCameraSettings);
    }

    public CinemachineBrain GetCinemachineBrain()
    {
        return m_CinemachineBrain;
    }

}
