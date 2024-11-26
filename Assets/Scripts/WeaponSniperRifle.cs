using System.Collections;
using UnityEngine;

public class WeaponSniperRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapons;  // ���� ���� ����

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
}
