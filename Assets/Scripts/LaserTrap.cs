using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(LineRenderer))] // Line Renderer 컴포넌트 강제 추가
public class LaserTrap : MonoBehaviour
{
    [Header("레이저 설정")]
    [SerializeField] private float laserDistance = 10f; // 레이저 감지 최대 거리
    [SerializeField] private LayerMask detectionLayer; // 감지할 레이어 마스크
    [SerializeField] private float laserWidth = 0.05f; // 레이저 굵기
    [SerializeField] private int damageAmount = 10; // 레이저 데미지

    [SerializeField] private LineRenderer lineRenderer;
    private Transform laserOrigin;
    private bool isPlayerDetected = false;

    Coroutine coroutine; 

    private void Awake()
    {
        laserOrigin = transform;

        InitializeLaser();
    }

    private void InitializeLaser()
    {
        lineRenderer.positionCount = 2; // 레이저는 시작점과 끝점, 2개의 점이 필요
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;

        lineRenderer.SetPosition(0, laserOrigin.position);
        lineRenderer.SetPosition(1, laserOrigin.position + laserOrigin.forward * laserDistance);
    }

    private void FixedUpdate()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        RaycastHit hit;

        bool hitSomething = Physics.Raycast(
            laserOrigin.position,
            laserOrigin.forward,
            out hit,
            laserDistance,
            detectionLayer
        );

        if (hitSomething)
        {
            if (hit.collider.TryGetComponent(out IDamagable damagable))
            {
                if (!isPlayerDetected)
                {
                    isPlayerDetected = true;
                    damagable.TakePhysicalDamage(damageAmount);
                    if (coroutine != null)
                        StopCoroutine(coroutine);
                    coroutine = StartCoroutine(CheckPlayerDetected());
                }
            }
        }
    }

    private IEnumerator CheckPlayerDetected()
    {
        yield return new WaitForSeconds(1f);
        isPlayerDetected = false;
    }
}