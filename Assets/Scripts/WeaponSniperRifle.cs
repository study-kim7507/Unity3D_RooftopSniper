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

    private bool isScoped = false;
    private Coroutine scopedBreathetheCoroutine;
    private Quaternion baseRotation;
    private bool isRecoiling = false;

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

        isScoped = false;

        if (scopedBreathetheCoroutine != null)
        {
            StopCoroutine(scopedBreathetheCoroutine);
            scopedBreathetheCoroutine = null;
        }
    }

    private IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(0.45f);

        scopeOverlay.SetActive(true);
        maskedCamera.SetActive(true);

        normalFOV = mainCamera.fieldOfView;

        mainCamera.fieldOfView = scopedFOV;
        maskedCamera.GetComponent<Camera>().fieldOfView = scopedFOV;

        isScoped = true;

        if (scopedBreathetheCoroutine == null)
        {
            scopedBreathetheCoroutine = StartCoroutine(ScopedBreathing());
        }
    }

    public void Fire()
    {
        // 총알 생성 및 발사
        Vector3 direction = mainCamera.transform.forward;
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.GetComponent<Bullet>().Direction = direction;
        bullet.transform.position = muzzleTransform.position;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        PlaySound(audioClipFire);

        // 총알 발사 이펙트
        GameObject muzzleFlashEffect = Instantiate(muzzleFlashEffectPrefab, muzzleTransform);
        muzzleFlashEffect.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DeactivateMuzzleFlashEffect(muzzleFlashEffect));

        // 탄피 생성
        GameObject casing = Instantiate(casingPrefab);
        casing.transform.position = casingSpawnPoint.position;

        // 기준 회전값 업데이트
        baseRotation = mainCamera.transform.localRotation;

        // Recoil 실행
        StartCoroutine(Recoil());
    }

    private IEnumerator ScopedBreathing()
    {
        float baseAmplitude = 0.3f;
        float maxAmplitude = 1.0f;
        float frequency = 1.0f;
        float amplitudeIncreaseRate = 0.1f;

        float currentAmplitude = baseAmplitude;

        while (isScoped)
        {
            // Recoil이 진행 중이면 흔들림을 적용하지 않음
            if (!isRecoiling)
            {
                currentAmplitude = Mathf.Min(currentAmplitude + amplitudeIncreaseRate * Time.deltaTime, maxAmplitude);

                // 흔들림 효과 적용
                float pitchOffset = Mathf.Sin(Time.time * frequency) * currentAmplitude;

                mainCamera.transform.localRotation = baseRotation * Quaternion.Euler(pitchOffset, 0, 0);
            }

            yield return null;
        }

        // 스코프 해제 시 기본 회전값으로 복구
        mainCamera.transform.localRotation = baseRotation;
    }

    private IEnumerator Recoil()
    {
        isRecoiling = true; // Recoil 동작 중 플래그 설정
        Quaternion recoilStartRotation = baseRotation; // 기준 회전값 저장

        float elapsedTime = 0f;

        // 반동 애니메이션
        while (elapsedTime < recoilDuration)
        {
            float t = elapsedTime / recoilDuration;

            // 반동 효과: 기준 회전값에서 위로 이동
            mainCamera.transform.localRotation = recoilStartRotation * Quaternion.Euler(-recoilAmount * (1 - t), 0, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 반동 완료 후 기준 회전값으로 복구
        mainCamera.transform.localRotation = recoilStartRotation;
        isRecoiling = false; // Recoil 종료 플래그 해제
    }

    private IEnumerator DeactivateMuzzleFlashEffect(GameObject muzzleFlashEffect)
    {
        yield return new WaitForSeconds(muzzleFlashEffect.GetComponent<ParticleSystem>().main.duration);

        Destroy(muzzleFlashEffect);
    }
}
