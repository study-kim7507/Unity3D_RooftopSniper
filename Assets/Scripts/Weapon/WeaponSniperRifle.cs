using System.Collections;
using UnityEditor.Build.Content;
using UnityEngine;

public class WeaponSniperRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipAiming;          // ���� ���� / ���� ���� ����
    [SerializeField]
    private AudioClip audioClipFire;            // �Ѿ� �߻� ����

    private AudioSource audioSource;            // ���� ��� ������Ʈ

    [Header("Camera Settings")]
    [SerializeField]
    private GameObject maskedCamera;            // �� ���� (����) �÷��̾�� ���� ������ �ʵ���
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
    private GameObject muzzleFlashEffectPrefab;       // �ѱ� ����Ʈ

    [Header("Recoil Settings")]
    [SerializeField]
    private float recoilAmount = 3.0f;                // �ݵ��� ���� (���� ����)
    [SerializeField]
    private float recoilDuration = 0.1f;              // �ݵ� ���� �ð�

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

        // ������ ��忡 �� �� ���� ȸ������ ���� ī�޶� ȸ�������� ����
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

        float maxDistance = bulletSpeed * bulletLifeTime;   // �Ÿ� = �ӷ� * �ð�, �Ѿ��� �����Ǵ� �������� �ִ�� ���ư� �� �ִ� �Ÿ�

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
        bullet.transform.position = muzzleTransform.position;   // �Ѿ��� �ʱ� ���� ������ ��ġ

        // �Ѿ��� �ʱ� ���� ������ ��ġ�� �Ѿ��� �浹 ������ ��ġ�� ������� �Ѿ��� ���ư� ������ ����Ͽ� ����
        Vector3 bulletHitPosition = CalculateBulletHitPosition();
        Vector3 direction = (bulletHitPosition - bullet.transform.position).normalized;
        bullet.GetComponent<Bullet>().Direction = direction;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        audioSource.PlayOneShot(audioClipFire);

        // �Ѿ� �߻� ����Ʈ
        GameObject muzzleFlashEffect = Instantiate(muzzleFlashEffectPrefab, muzzleTransform);
        muzzleFlashEffect.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DeactivateMuzzleFlashEffect(muzzleFlashEffect));

        // ź�� ����
        GameObject casing = Instantiate(casingPrefab);
        casing.transform.position = casingSpawnPoint.position;

        // ���� ȸ���� ������Ʈ
        baseRotation = mainCamera.transform.localRotation;

        // Recoil ����
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
            // Recoil�� ���� ���̸� ��鸲�� �������� ����
            if (!isRecoiling)
            {
                currentAmplitude = Mathf.Min(currentAmplitude + amplitudeIncreaseRate * Time.deltaTime, maxAmplitude);

                // ��鸲 ȿ�� ����
                float pitchOffset = Mathf.Sin(Time.time * frequency) * currentAmplitude;

                mainCamera.transform.localRotation = baseRotation * Quaternion.Euler(pitchOffset, 0, 0);
            }

            yield return null;
        }

        // ������ ���� �� �⺻ ȸ�������� ����
        mainCamera.transform.localRotation = baseRotation;
    }

    private IEnumerator Recoil()
    {
        isRecoiling = true; // Recoil ���� �� �÷��� ����
        Quaternion recoilStartRotation = baseRotation; // ���� ȸ���� ����

        float elapsedTime = 0f;

        // �ݵ� �ִϸ��̼�
        while (elapsedTime < recoilDuration)
        {
            float t = elapsedTime / recoilDuration;

            // �ݵ� ȿ��: ���� ȸ�������� ���� �̵�
            mainCamera.transform.localRotation = recoilStartRotation * Quaternion.Euler(-recoilAmount * (1 - t), 0, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ݵ� �Ϸ� �� ���� ȸ�������� ����
        mainCamera.transform.localRotation = recoilStartRotation;
        isRecoiling = false; // Recoil ���� �÷��� ����
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
