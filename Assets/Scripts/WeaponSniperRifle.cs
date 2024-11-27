using System.Collections;
using UnityEngine;

public class WeaponSniperRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapons;  // 무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipAiming;          // 무기 조준 / 조준 해제 사운드
    [SerializeField]
    private AudioClip audioClipFire;            // 총알 발사 사운드

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

    [Header("Bullet")]
    [SerializeField]
    private Transform MuzzleTransform;
    [SerializeField]
    private GameObject BulletPrefab;

    [Header("Recoil")]
    [SerializeField]
    private float recoilAmount = 1.0f;          // 총기 반동 크기
    [SerializeField]   
    private float recoilDuration = 0.1f;        // 총기 반동 지속 시간


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

    public void Fire()
    {
        StartCoroutine(ApplyRecoil());

        // 총알이 날아갈 방향을 현재 플레이어가 바라보는 방향으로 (카메라의 전방) 설정
        Vector3 direction = mainCamera.transform.forward;
        BulletPrefab.GetComponent<Bullet>().Direction = direction;
        
        // 총알을 인스턴스화하고 초기 위치를 총구의 위치로 설정
        GameObject go = Instantiate(BulletPrefab);
        go.transform.position = MuzzleTransform.position;

        PlaySound(audioClipFire);   // 총알 발사 사운드 재생

    }

    private IEnumerator ApplyRecoil()
    {
        // TODO: 수정 필요
        Vector3 originalPosition = Camera.main.transform.localPosition;
        Vector3 recoilPosition = originalPosition - Camera.main.transform.forward * recoilAmount;

        float elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            Camera.main.transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래 위치로 복귀
        elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            Camera.main.transform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPosition; // 최종 위치 보정
    }

}
