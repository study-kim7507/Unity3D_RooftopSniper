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
    private Transform muzzleTransform;
    [SerializeField]
    private GameObject bulletPrefab;

    [Header("Casing")]
    [SerializeField]
    private Transform casingSpawnPoint;
    [SerializeField]
    private GameObject casingPrefab;

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffectPrefab;       // 총구 이펙트

    [Header("Recoil")]
    [SerializeField]
    private float recoilAmount = 3.0f;      // 반동의 각도 (조정 가능)
    [SerializeField]
    private float recoilDuration = 0.1f;    // 반동 지속 시간

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // 무기 장착 사운드 재생
        PlaySound(audioClipTakeOutWeapons);

    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // 기존에 재생중인 사운드 정지,
        audioSource.clip = clip;    // 새로운 사운드 clip으로 교체
        audioSource.Play();         // 사운드 재생
    }

    public void ToggleMode()
    {
        PlaySound(audioClipAiming);
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
        // 총알이 날아갈 방향을 현재 플레이어가 바라보는 방향으로 (카메라의 전방) 설정
        Vector3 direction = mainCamera.transform.forward;                
        
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.GetComponent<Bullet>().Direction = direction;            // 총알이 날아갈 방향
        bullet.transform.position = muzzleTransform.position;           // 총알의 초기 위치
        bullet.transform.rotation = Quaternion.LookRotation(direction); // 총알을 날아갈 방향으로 회전시킴

        PlaySound(audioClipFire);   // 총알 발사 사운드 재생

        // 총알 발사 파티클 재생
        GameObject muzzleFlashEffect = Instantiate(muzzleFlashEffectPrefab, muzzleTransform);
        muzzleFlashEffect.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DeactivateMuzzleFlashEffect(muzzleFlashEffect));

        // 탄피 생성
        GameObject casing = Instantiate(casingPrefab);
        casing.transform.position = casingSpawnPoint.position;

        // 수직 반동 추가
        StartCoroutine(Recoil());
    }

    private IEnumerator Recoil()
    {
        // 수직 반동 효과를 위해 카메라를 위로 회전

        // 카메라의 원래 회전을 저장
        Quaternion originalRotation = mainCamera.transform.localRotation;
 
        float elapsedTime = 0f;

        // 반동 애니메이션
        while (elapsedTime < recoilDuration)
        {
            float t = elapsedTime / recoilDuration;
            // 카메라를 위로 회전
            mainCamera.transform.localRotation = originalRotation * Quaternion.Euler(-recoilAmount * (1 - t), 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 원래 회전으로 복귀
        mainCamera.transform.localRotation = originalRotation;
    }


    private IEnumerator DeactivateMuzzleFlashEffect(GameObject muzzleFlashEffect)
    {
        yield return new WaitForSeconds(muzzleFlashEffect.GetComponent<ParticleSystem>().main.duration);

        Destroy(muzzleFlashEffect);
    }
}
