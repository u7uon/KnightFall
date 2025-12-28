using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;

    private float bgmVolumeBeforeMute = 0f;
    private float sfxVolumeBeforeMute = 0f;

    const string SFX_KEY = "SFX";
    const string BGM_KEY = "BGM";
    const string BGM_MUTE_KEY = "BGMMuted";
    const string SFX_MUTE_KEY = "SFXMuted";
    const string BGM_VOLUME_BEFORE_MUTE_KEY = "BGMVolBeforeMute";
    const string SFX_VOLUME_BEFORE_MUTE_KEY = "SFXVolBeforeMute";

    void OnEnable()
    {
        // Tắt listener tạm thời để tránh trigger event khi set value
        sfxSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmToggle.onValueChanged.RemoveAllListeners();
        sfxToggle.onValueChanged.RemoveAllListeners();

        // Load and apply saved settings
        LoadAudioSettings();

        // Đăng ký sự kiện sau khi đã set xong
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        bgmToggle.onValueChanged.AddListener(ToggleBGM);
        sfxToggle.onValueChanged.AddListener(ToggleSFX);
    }

    void Awake()
    {
        // Also load settings once on Awake so audio is correct early
        LoadAudioSettings();
    }

    private void LoadAudioSettings()
    {
        // Load saved volume (default: full volume)
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY, 0f);
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, 0f);

        // Stored value: 0 = not muted, 1 = muted. Default to 0 (not muted).
        bool bgmMuted = PlayerPrefs.GetInt(BGM_MUTE_KEY, 0) == 1;
        bool sfxMuted = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;

        // Load volume trước khi mute
        bgmVolumeBeforeMute = PlayerPrefs.GetFloat(BGM_VOLUME_BEFORE_MUTE_KEY, bgmVol);
        sfxVolumeBeforeMute = PlayerPrefs.GetFloat(SFX_VOLUME_BEFORE_MUTE_KEY, sfxVol);

        // Set UI values
        sfxSlider.value = sfxVol;
        bgmSlider.value = bgmVol;
        bgmToggle.isOn = !bgmMuted;
        sfxToggle.isOn = !sfxMuted;

        // Áp dụng volume vào AudioMixer
        if (bgmMuted)
        {
            mainMixer.SetFloat("BGM", -80f);
        }
        else
        {
            mainMixer.SetFloat("BGM", bgmVol);
        }
        
        if (sfxMuted)
        {
            mainMixer.SetFloat("SFX", -80f);
        }
        else
        {
            mainMixer.SetFloat("SFX", sfxVol);
        }
    }

    public void SetSFXVolume(float value)
    {
        if (sfxToggle.isOn)
        {
            mainMixer.SetFloat("SFX", value);
            PlayerPrefs.SetFloat(SFX_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public void SetBGMVolume(float value)
    {
        if (bgmToggle.isOn)
        {
            mainMixer.SetFloat("BGM", value);
            PlayerPrefs.SetFloat(BGM_KEY, value);
            PlayerPrefs.Save();
        }
    }
    
    private void ToggleBGM(bool isOn)
    {
        if (isOn)
        {
            // Bật lại âm lượng cũ
            float volumeToRestore = bgmVolumeBeforeMute;
            bgmSlider.value = volumeToRestore;
            mainMixer.SetFloat("BGM", volumeToRestore);
            PlayerPrefs.SetFloat(BGM_KEY, volumeToRestore);
            PlayerPrefs.SetInt(BGM_MUTE_KEY, 0);
        }
        else
        {
            // Lưu giá trị hiện tại trước khi tắt
            bgmVolumeBeforeMute = bgmSlider.value;
            mainMixer.SetFloat("BGM", -80f);
            PlayerPrefs.SetFloat(BGM_VOLUME_BEFORE_MUTE_KEY, bgmVolumeBeforeMute);
            PlayerPrefs.SetInt(BGM_MUTE_KEY, 1);
        }
        PlayerPrefs.Save();
    }

    private void ToggleSFX(bool isOn)
    {
        if (isOn)
        {
            float volumeToRestore = sfxVolumeBeforeMute;
            sfxSlider.value = volumeToRestore;
            mainMixer.SetFloat("SFX", volumeToRestore);
            PlayerPrefs.SetFloat(SFX_KEY, volumeToRestore);
            PlayerPrefs.SetInt(SFX_MUTE_KEY, 0);
        }
        else
        {
            sfxVolumeBeforeMute = sfxSlider.value;
            mainMixer.SetFloat("SFX", -80f);
            PlayerPrefs.SetFloat(SFX_VOLUME_BEFORE_MUTE_KEY, sfxVolumeBeforeMute);
            PlayerPrefs.SetInt(SFX_MUTE_KEY, 1);
        }
        PlayerPrefs.Save();
    }
}