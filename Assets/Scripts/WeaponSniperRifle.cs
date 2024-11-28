using System.Collections;
using UnityEngine;

public class WeaponSniperRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapons;  // ���� ���� ����
    [SerializeField]
    private AudioClip audioClipAiming;          // ���� ���� / ���� ���� ����
    [SerializeField]
    private AudioClip audioClipFire;            // �Ѿ� �߻� ����

    private AudioSource audioSource;            // ���� ��� ������Ʈ

    [Header("Scope Overlay")]
    [SerializeField]
    private GameObject scopeOverlay;

    [Header("Camera")]
    [SerializeField]
    private GameObject maskedCamera;            // �� ���� (����) �÷��̾�� ���� ������ �ʵ���
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
    private GameObject muzzleFlashEffectPrefab;       // �ѱ� ����Ʈ

    [Header("Recoil")]
    [SerializeField]
    private float recoilAmount = 3.0f;      // �ݵ��� ���� (���� ����)
    [SerializeField]
    private float recoilDuration = 0.1f;    // �ݵ� ���� �ð�

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
        // ���� ���� ���� ���
        PlaySound(audioClipTakeOutWeapons);

    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // ������ ������� ���� ����,
        audioSource.clip = clip;    // ���ο� ���� clip���� ��ü
        audioSource.Play();         // ���� ���
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
        // �Ѿ� ���� �� �߻�
        Vector3 direction = mainCamera.transform.forward;
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.GetComponent<Bullet>().Direction = direction;
        bullet.transform.position = muzzleTransform.position;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        PlaySound(audioClipFire);

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
}
