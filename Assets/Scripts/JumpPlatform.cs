using System.Collections;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private float jumpPower;
    [SerializeField] private float delay = 3f;

    private Coroutine currentJumpCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rb))
        {
            if (currentJumpCoroutine != null)
                StopCoroutine(currentJumpCoroutine);
            currentJumpCoroutine = StartCoroutine(JumpAfterDelay(rb));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentJumpCoroutine != null)
        {
            StopCoroutine(currentJumpCoroutine);
            currentJumpCoroutine = null;
            Debug.Log("점프 예약 취소됨.");
        }
    }

    private IEnumerator JumpAfterDelay(Rigidbody rb)
    {
        yield return new WaitForSeconds(delay);

        // 기존 y의 velocity를 0으로 만들어야 일관된 점프가 가능
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        currentJumpCoroutine = null;
    }
}
