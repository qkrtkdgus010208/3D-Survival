using DG.Tweening;
using UnityEngine;

public class MovePad : MonoBehaviour
{
    public Vector3 endOffset;
    public float moveDuration = 5f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Tween moveTween;

    private void Awake()
    {
        startPosition = transform.position;
        endPosition = startPosition + endOffset;

        MovePlatform();
    }

    private void MovePlatform()
    {
        if (moveDuration <= 0) return;

        moveTween = transform.DOMove(endPosition, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject)
            .SetUpdate(UpdateType.Fixed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}