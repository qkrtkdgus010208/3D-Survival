using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float jumpPower;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Rigidbody rb))
        {
            // 기존 y의 velocity를 0으로 만들어야 일관된 점프가 가능
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
}
