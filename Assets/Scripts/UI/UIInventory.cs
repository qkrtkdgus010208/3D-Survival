using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;      // item 버릴 때 필요한 위치

    [Header("Selected Item")]           // 선택한 슬롯의 아이템 정보 표시 위한 UI
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    public Button useBtn;
    public Button dropBtn;

    private int curEquipIndex;

    private PlayerController controller;
    private PlayerCondition condition;

    private void OnEnable()
    {
        useBtn.onClick.AddListener(OnUseButton);
        dropBtn.onClick.AddListener(OnDropButton);
    }

    private void OnDisable()
    {
        useBtn.onClick.RemoveAllListeners();
        dropBtn.onClick.RemoveAllListeners();
    }

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        // Action 호출 시 필요한 함수 등록
        controller.inventory += Toggle;      // inventory 키 입력 시
        CharacterManager.Instance.Player.addItem += AddItem;  // 아이템 파밍 시

        // Inventory UI 초기화 로직들
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    // 선택한 아이템 표시할 정보창 Clear 함수
    void ClearSelectedItemWindow()
    {
        selectedItem = null;

        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    // Inventory 창 Open/Close 시 호출
    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }


    public void AddItem()
    {
        // 10강 ItemObject 로직에서 Player에 넘겨준 정보를 가지고 옴
        ItemData data = CharacterManager.Instance.Player.itemData;

        // 여러개 가질 수 있는 아이템이라면
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        // 빈 슬롯 찾기
        ItemSlot emptySlot = GetEmptySlot();

        // 빈 슬롯이 있다면
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        // 빈 슬롯 마저 없을 때
        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    // UI 정보 새로고침
    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // 슬롯에 아이템 정보가 있다면
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    // 여러개 가질 수 있는 아이템의 정보 찾아서 return
    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    // 슬롯의 item 정보가 비어있는 정보 return
    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    // Player 스크립트 먼저 수정
    // 아이템 버리기 (실제론 매개변수로 들어온 데이터에 해당하는 아이템 생성)
    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }


    // ItemSlot 스크립트 먼저 수정
    // 선택한 아이템 정보창에 업데이트 해주는 함수
    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.item.consumables.Length; i++)
        {
            selectedItemStatName.text += selectedItem.item.consumables[i].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !slots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if (selectedItem.item.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.item.consumables[i].value); break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value); break;
                }
            }
            RemoveSelctedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelctedItem();
    }

    void RemoveSelctedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            //if (slots[selectedItemIndex].equipped)
            //{
            //    UnEquip(selectedItemIndex);
            //}

            selectedItem.item = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    public bool HasItem(ItemData item, int quantity)
    {
        return false;
    }
}