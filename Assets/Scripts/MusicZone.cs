using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeTime;          // 변화 시간 주기
    public float maxVolume;         // 최대 볼륨
    private float targetVolume;     // 타겟 볼륨 (0과 maxVolume을 오갈 예정)

    private void Start()
    {
        targetVolume = 0.0f;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = targetVolume;
        audioSource.Play();

    }

    private void Update()
    {
        // Approximately는 근사값 체크 → 이유를 꼭 학습하기
        // (0.1f, 1*0.1f, 1f/10을 == 으로 비교하면 true가 나올까?) 
        if (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            // 유니티 공식문서에서 각 매개변수 역할 확인
            // (링크 여기)
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, (maxVolume / fadeTime) * Time.deltaTime);
        }
    }

    // 뮤직존에 플레이어가 들어오면 타겟 볼륨을 maxVolume으로
    // (점점 커지게)
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("진입");
        if (other.CompareTag("Player"))
        {
            Debug.Log("진입1");
            targetVolume = maxVolume;
        }
    }

    // 뮤직존에 플레이어가 들어오면 타겟 볼륨을 0으로
    // (점점 작아지게)
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("탈출");
        if (other.CompareTag("Player"))
        {
            Debug.Log("탈출1");
            targetVolume = 0.0f;
        }
    }
}
