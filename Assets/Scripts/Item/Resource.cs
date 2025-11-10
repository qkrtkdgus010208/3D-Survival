using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;   // 드랍 할 아이템 정보(SO)
    public int quantityPerHit = 1;  // 한 번에 드랍할 갯수
    public int capacity;          // 드랍 가능 횟수

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)
        {
            if (capacity <= 0) break;

            capacity -= 1;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
        }

        if (capacity <= 0)
        {
            Destroy(gameObject);
        }
    }
}