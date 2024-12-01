using System.Collections;
using UnityEditor.Build.Content;
using UnityEngine;

public class WeaponSniperRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipAiming;          // 무기 조준 / 조준 해제 사운드
    [SerializeField]
    private AudioClip audioClipFire;            // 총알 발사 사운드

    private AudioSource audioSource;            // 사운드 재생 컴포넌트

    [Header("Camera Settings")]
    [SerializeField]
    private GameObject maskedCamera;            // 줌 모드시 (저격) 플레이어와 총이 보이지 않도록
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float scopedFOV = 10.0f;
    private float normalFOV;

    [Header("Scope Overlay")]
    [SerializeField]
    private GameObject scopeOverlay;

    [Header("Bullet Settings")]
    [SerializeField]
    private Transform muzzleTransform;
    [SerializeField]
    private GameObject bulletPrefab;

    [Header("Casing Settings")]
    [SerializeField]
    private Transform casingSpawnPoint;
    [SerializeField]
    private GameObject casingPrefab;

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffectPrefab;       // 총구 이펙트

    [Header("Recoil Settings")]
    [SerializeField]
    private float recoilAmount = 3.0f;                // 반동의 각도 (조정 가능)
    [SerializeField]
    private float recoilDuration = 0.1f;              // 반동 지속 시간

    private bool isScoped = false;
    private Coroutine scopedBreathetheCoroutine;
    private Quaternion baseRotation;
    private bool isRecoiling = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ToggleMode()
    {
        audioSource.PlayOneShot(audioClipAiming);
        if (scopeOverlay.activeSelf) OnUnscoped();
        else OnScoped();
    }

    private void OnUnscoped()
    {
        ToggleScopeOverlay();
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

    private void OnScoped()
    {
        ToggleScopeOverlay();
        maskedCamera.SetActive(true);

        normalFOV = mainCamera.fieldOfView;

        mainCamera.fieldOfView = scopedFOV;
        maskedCamera.GetComponent<Camera>().fieldOfView = scopedFOV;

        isScoped = true;

        // 스코프 모드에 들어갈 때 기준 회전값을 현재 카메라 회전값으로 설정
        baseRotation = mainCamera.transform.localRotation;

        if (scopedBreathetheCoroutine == null)
        {
            scopedBreathetheCoroutine = StartCoroutine(ScopedBreathing());
        }
    }

    public void SetFOV(bool isWheelUp)
    {
        float currentFOV = maskedCamera.GetComponent<Camera>().fieldOfView;
        currentFOV = isWheelUp ? Mathf.Clamp(--currentFOV, 0.0f, normalFOV) : Mathf.Clamp(++currentFOV, 0.0f, normalFOV);
        maskedCamera.GetComponent<Camera>().fieldOfView = currentFOV;
    }

    private Vector3 CalculateBulletHitPosition()
    {
        float bulletSpeed = bulletPrefab.GetComponent<Bullet>().Speed;
        float bulletLifeTime = bulletPrefab.GetComponent<Bullet>().LifeTime;

        float maxDistance = bulletSpeed * bulletLifeTime;   // 거리 = 속력 * 시간, 총알이 생성되는 지점에서 최대로 날아갈 수 있는 거리

        RaycastHit hit;
        Ray r = mainCamera.ViewportPointToRay(Vector3.one / 2);

        Vector3 hitPosition = r.origin + r.direction * maxDistance;

        if (Physics.Raycast(r, out hit, maxDistance))
        {
            hitPosition = hit.point;
        }
        
        return hitPosition;
    }

    public void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = muzzleTransform.position;   // 총알의 초기 생성 지점의 위치

        // 총알의 초기 생성 지점의 위치와 총알의 충돌 지점의 위치를 기반으로 총알이 날아갈 방향을 계산하여 설정
        Vector3 bulletHitPosition = CalculateBulletHitPosition();
        Vector3 direction = (bulletHitPosition - bullet.transform.position).normalized;
        bullet.GetComponent<Bullet>().Direction = direction;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        audioSource.PlayOneShot(audioClipFire);

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

    public void ToggleScopeOverlay()
    {
        scopeOverlay.gameObject.SetActive(!scopeOverlay.gameObject.activeSelf);
    }
}
