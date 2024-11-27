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
    private Transform MuzzleTransform;
    [SerializeField]
    private GameObject BulletPrefab;

    [Header("Recoil")]
    [SerializeField]
    private float recoilAmount = 1.0f;          // �ѱ� �ݵ� ũ��
    [SerializeField]   
    private float recoilDuration = 0.1f;        // �ѱ� �ݵ� ���� �ð�


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // ���� ���� ���� ���
       
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // ������ ������� ���� ����,
        audioSource.clip = clip;    // ���ο� ���� clip���� ��ü
        audioSource.Play();         // ���� ���
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

        // �Ѿ��� ���ư� ������ ���� �÷��̾ �ٶ󺸴� �������� (ī�޶��� ����) ����
        Vector3 direction = mainCamera.transform.forward;
        BulletPrefab.GetComponent<Bullet>().Direction = direction;
        
        // �Ѿ��� �ν��Ͻ�ȭ�ϰ� �ʱ� ��ġ�� �ѱ��� ��ġ�� ����
        GameObject go = Instantiate(BulletPrefab);
        go.transform.position = MuzzleTransform.position;

        PlaySound(audioClipFire);   // �Ѿ� �߻� ���� ���

    }

    private IEnumerator ApplyRecoil()
    {
        // TODO: ���� �ʿ�
        Vector3 originalPosition = Camera.main.transform.localPosition;
        Vector3 recoilPosition = originalPosition - Camera.main.transform.forward * recoilAmount;

        float elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            Camera.main.transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ���� ��ġ�� ����
        elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            Camera.main.transform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPosition; // ���� ��ġ ����
    }

}
