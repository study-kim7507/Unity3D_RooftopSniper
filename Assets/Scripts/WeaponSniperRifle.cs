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
        // �Ѿ��� ���ư� ������ ���� �÷��̾ �ٶ󺸴� �������� (ī�޶��� ����) ����
        Vector3 direction = mainCamera.transform.forward;                
        
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.GetComponent<Bullet>().Direction = direction;            // �Ѿ��� ���ư� ����
        bullet.transform.position = muzzleTransform.position;           // �Ѿ��� �ʱ� ��ġ
        bullet.transform.rotation = Quaternion.LookRotation(direction); // �Ѿ��� ���ư� �������� ȸ����Ŵ

        PlaySound(audioClipFire);   // �Ѿ� �߻� ���� ���

        // �Ѿ� �߻� ��ƼŬ ���
        GameObject muzzleFlashEffect = Instantiate(muzzleFlashEffectPrefab, muzzleTransform);
        muzzleFlashEffect.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DeactivateMuzzleFlashEffect(muzzleFlashEffect));

        // ź�� ����
        GameObject casing = Instantiate(casingPrefab);
        casing.transform.position = casingSpawnPoint.position;

        // ���� �ݵ� �߰�
        StartCoroutine(Recoil());
    }

    private IEnumerator Recoil()
    {
        // ���� �ݵ� ȿ���� ���� ī�޶� ���� ȸ��

        // ī�޶��� ���� ȸ���� ����
        Quaternion originalRotation = mainCamera.transform.localRotation;
 
        float elapsedTime = 0f;

        // �ݵ� �ִϸ��̼�
        while (elapsedTime < recoilDuration)
        {
            float t = elapsedTime / recoilDuration;
            // ī�޶� ���� ȸ��
            mainCamera.transform.localRotation = originalRotation * Quaternion.Euler(-recoilAmount * (1 - t), 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ȸ������ ����
        mainCamera.transform.localRotation = originalRotation;
    }


    private IEnumerator DeactivateMuzzleFlashEffect(GameObject muzzleFlashEffect)
    {
        yield return new WaitForSeconds(muzzleFlashEffect.GetComponent<ParticleSystem>().main.duration);

        Destroy(muzzleFlashEffect);
    }
}
