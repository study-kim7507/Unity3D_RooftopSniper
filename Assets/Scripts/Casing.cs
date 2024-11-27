using System.Collections;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 5.0f;      // ź�ǰ� ������ �� �� �ڿ� ����� ������
    [SerializeField]
    private float casingSpin = 1.0f;    // ź�ǰ� ȸ���ϴ� �ӷ� ���
    [SerializeField]
    private AudioClip[] audioClips;     // ź�ǰ� �ε����� �� ����Ǵ� ����

    private Rigidbody rigidBody;
    private AudioSource audioSource;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        rigidBody.linearVelocity = Vector3.right;
        rigidBody.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                Random.Range(-casingSpin, casingSpin),
                                                Random.Range(-casingSpin, casingSpin));

        StartCoroutine(DestroyAfterTime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���� ���� ź�� ���� �� ������ ���� ����
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
