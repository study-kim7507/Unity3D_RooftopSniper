using System.Collections;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 5.0f;      // 탄피가 생성된 후 얼마 뒤에 사라질 것인지
    [SerializeField]
    private float casingSpin = 1.0f;    // 탄피가 회전하는 속력 계수
    [SerializeField]
    private AudioClip[] audioClips;     // 탄피가 부딪혔을 때 재생되는 사운드

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
        // 여러 개의 탄피 사운드 중 임의의 사운드 선택
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
