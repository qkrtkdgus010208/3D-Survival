using UnityEngine;

// 인터랙션 가능한 객체에 상속할 인터페이스
public interface IInteractable
{
    public string GetInteractPrompt();  // UI에 표시할 정보
    public void OnInteract();           // 인터랙션 호출
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}";
        return str;
    }

    public void OnInteract()
    {
        //Player 스크립트 먼저 수정
        //Player 스크립트에 상호작용 아이템 data 넘기기.
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}