using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;

    public AudioClip buttonClick;
    public AudioClip meeplePlace;
    public AudioClip meepleTakeBack;
    public AudioClip reject;
    public AudioClip tilePlaced;

    public static AudioManager Instance;

    public Slider volumeSlider;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SFXSource.volume = GameConfig.Instance.volume;

        if (volumeSlider != null)
            volumeSlider.value = SFXSource.volume;

    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void SetVolume()
    {
        SFXSource.volume = volumeSlider.value;
        GameConfig.Instance.volume = volumeSlider.value;
    }    
}
