using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;    // 상호작용 오브젝트 체크 시간
    private float lastCheckTime;       // 마지막 상호작용 체크 시간
    public float maxcheckDistance;
    public float checkDistanceOne;     // 1인칭 최대 체크 거리
    public float checkDistanceThree;     // 3인칭 최대 체크 거리
    public LayerMask layerMask;

    public GameObject curInteractGameObject;  // 현재 상호작용 게임오브젝트
    private IInteractable curInteractable;    // 현재 상호작용 인터페이스

    public TextMeshProUGUI promptText;
    private Camera camera;

    void Start()
    {
        camera = Camera.main;
        maxcheckDistance = checkDistanceOne;
    }

    private void OnEnable()
    {
        CharacterManager.Instance.Player.controller.OnChangeView += ChangeView;
    }

    private void OnDisable()
    {
        CharacterManager.Instance.Player.controller.OnChangeView -= ChangeView;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            // 2강 Ray 관련 로직 복습하기
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray, out RaycastHit hit, maxcheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }

    private void ChangeView()
    {
        maxcheckDistance = CharacterManager.Instance.Player.controller.IsthirdPersonView ? checkDistanceThree : checkDistanceOne;
    }
}