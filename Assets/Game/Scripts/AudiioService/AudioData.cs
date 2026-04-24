using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/Audio/AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField] private string _key;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] [Range(0,1)] private float volume = 1f;
    [SerializeField] [Range(0, 1)] private float pitch = 1f;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool spatial = false;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;


    public string Key { get => _key; set => _key = value; }
    public AudioClip AudioClip { get => _audioClip; set => _audioClip = value; }
    public float Volume { get => volume; set => volume = value; }
    public float Pitch { get => pitch; set => pitch = value; }
    public bool Loop { get => loop; set => loop = value; }
    public bool Spatial { get => spatial; set => spatial = value; }
    public float MinDistance { get => minDistance; set => minDistance = value; }
    public float MaxDistance { get => maxDistance; set => maxDistance = value; }

}
