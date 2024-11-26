using System.Collections;
using UnityEngine;

public class WeaponSniperRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapons;  // 무기 장착 사운드

    private AudioSource audioSource;            // 사운드 재생 컴포넌트

    [Header("Scope Overlay")]
    [SerializeField]
    private GameObject scopeOverlay;


    [Header("Camera")]
    [SerializeField]
    private GameObject maskedCamera;            // 줌 모드시 (저격) 플레이어와 총이 보이지 않도록
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private float scopedFOV = 15.0f;
    private float normalFOV;

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

    public void ToggleMode()
    {
        if (scopeOverlay.activeSelf) OnUnscoped();
        else StartCoroutine(OnScoped());
    }

    private void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        maskedCamera.SetActive(false);

        mainCamera.fieldOfView = normalFOV;
        maskedCamera.GetComponent<Camera>().fieldOfView = normalFOV;
    }

    private IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(0.45f);

        scopeOverlay.SetActive(true);
        maskedCamera.SetActive(true);

        normalFOV = mainCamera.fieldOfView;

        mainCamera.fieldOfView = scopedFOV;
        maskedCamera.GetComponent<Camera>().fieldOfView = scopedFOV;
    }
}
