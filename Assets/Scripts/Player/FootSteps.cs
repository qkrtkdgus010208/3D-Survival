using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footstepClips;     // 랜덤 재생시켜줄 오디오클립들
    private AudioSource audioSource;      // 오디오 클립을 재생시킬 오디오소스
    private Rigidbody _rigidbody;
    public float footstepThreshold;       // 플레이 가능한 움직임 크기(조건)
    public float footstepRate;            // 오디오 플레이 주기
    private float footStepTime;           // 마지막 플레이 시간

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // velocity(변화량)가 0보다 작으면 추락중
        // velcotiy가 0보다(대략 0.1보다)크면 올라가는 중
        if (Mathf.Abs(_rigidbody.velocity.y) < 0.1f) // 땅에 붙어 있을 때
        {
            if (_rigidbody.velocity.magnitude > footstepThreshold) // 움직일 때
            {
                if (Time.time - footStepTime > footstepRate) // 상호작용 로직 복습
                {
                    footStepTime = Time.time;
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
                }
            }
        }
    }
}