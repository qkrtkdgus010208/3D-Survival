using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;

    private List<IDamagable> things = new List<IDamagable>();

    private void Start()
    {
        // 유니티 공식문서 통해 매개변수 역할 확인해보기
        // 링크(여기 클릭)
        InvokeRepeating("DealDamage", 0, damageRate);
    }

    void DealDamage()
    {
        // things 리스트에 추가 된 IDamagable 객체의 데미지 함수 호출
        for (int i = 0; i < things.Count; i++)
        {
            things[i].TakePhysicalDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌된 객체에 IDamagable이 상속되어 있으면 List에 추가
        if (other.TryGetComponent(out IDamagable damagable))
        {
            things.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Exit 되는 객체에 IDamagable이 상속되어 있으면 List에서 제거
        if (other.TryGetComponent(out IDamagable damagable))
        {
            things.Remove(damagable);
        }
    }
}