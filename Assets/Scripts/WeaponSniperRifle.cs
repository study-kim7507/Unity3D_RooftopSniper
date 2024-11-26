using UnityEngine;

public class WeaponSniperRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapons;  // 무기 장착 사운드

    private AudioSource audioSource;            // 사운드 재생 컴포넌트

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // 무기 장착 사운드 재생
       
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // 기존에 재생중인 사운드 정지,
        audioSource.clip = clip;    // 새로운 사운드 clip으로 교체
        audioSource.Play();         // 사운드 재생
    }
}
