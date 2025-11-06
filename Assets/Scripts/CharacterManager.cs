using UnityEngine;

// 싱글톤 패턴
public class CharacterManager : MonoBehaviour
{
    private static CharacterManager _instance;
    public static CharacterManager Instance
    {
        get
        {
            // 할당되지 않았을 때, 외부에서 CharacterManager.Instance 로 접근하는 경우
            // 게임 오브젝트를 만들어주고 CharacterManager 스크립트를 AddComponent
            if (_instance == null)
            {
                _instance = new GameObject("CharacerManager").AddComponent<CharacterManager>();
            }
            return _instance;
        }
    }

    // 나중에 수정될 경우를 고려하여 원본(_player)과 접근(Player)을 구별
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }
    private Player _player;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}