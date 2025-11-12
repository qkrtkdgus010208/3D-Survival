using DG.Tweening;
using UnityEngine;

public class MovePlatform : MonoBehaviour
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

        Move();
    }

    private void Move()
    {
        if (moveDuration <= 0) return;

        moveTween = transform.DOMove(endPosition, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject)
            .SetUpdate(UpdateType.Fixed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}