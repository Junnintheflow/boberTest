using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class Inventory : MonoBehaviour
{
    [Header("Параметры")]
    [SerializeField] private int maxSlots = 1; 

    public static Inventory Instance { get; private set; }
    public List<Item> items = new List<Item>(); 
    public int selectedItemIndex = -1; 
    public Transform handSlot; 

    public bool IsThrowing { get; private set; } //upd fix флажок для анимации выброса
    private bool isInteracting = false; // флажок для взаимодействия

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  
        }
        else
        {
            Destroy(gameObject);  
            return;
        }
    }

    void Update()
    {
        HandleScroll();
        HandleKeySelection();

        UpdateHandSlotPosition();

        if (Input.GetMouseButtonDown(1)) 
            {
            ThrowItem();
            }
    }


    public void AddItem(Item newItem)
    {
        if (items.Count < maxSlots)
        {
            items.Add(newItem);
            FindFirstObjectByType<InventoryUI>().UpdateInventory(GetItemSprites());
            Debug.Log("подобрал предмет: " + newItem.itemName);
            UpdateUI(); 
        }
        else
        {
            Debug.Log("инвентарь полон");
        }
    }

    private void UpdateUI()
    {

    }

    public Sprite[] GetItemSprites()
    {
        Sprite[] icons = new Sprite[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            icons[i] = items[i].itemIcon; 
        }
        return icons;
    }

// ПОЗИЦИЯ К КАМЕРЕ

    void UpdateHandSlotPosition()
    {
        if (Camera.main == null) return;

        Transform cam = Camera.main.transform;

        Vector3 offset = cam.right * 0.3f  
                    + cam.up * -0.2f   
                    + cam.forward * 0.5f; 

        handSlot.position = Vector3.Lerp(handSlot.position, cam.position + offset, Time.deltaTime * 10f);
        
        Quaternion targetRotation = cam.rotation * Quaternion.Euler(10, 20, 0);
        handSlot.rotation = Quaternion.Slerp(handSlot.rotation, targetRotation, Time.deltaTime * 10f);
    }

// ПЕРЕКЛЮЧЕНИЕ

    void HandleScroll()
    {
        if (items.Count == 0) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) SelectNextItem();
        if (scroll < 0) SelectPreviousItem();
    }

    void HandleKeySelection()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectItem(i);
                break;
            }
        }
    }

    void SelectNextItem()
    {
        selectedItemIndex = (selectedItemIndex + 1) % items.Count;
        UpdateSelectedItem();
    }

    void SelectPreviousItem()
    {
        selectedItemIndex = (selectedItemIndex - 1 + items.Count) % items.Count;
        UpdateSelectedItem();
    }

    void SelectItem(int index)
    {
        selectedItemIndex = index;
        UpdateSelectedItem();
    }

    void UpdateSelectedItem()
    {
        if (isInteracting) return; // блокирую смену предмета при текущем взаимодействии (check)

        isInteracting = true; // point начала взаимодействия

        if (selectedItemIndex >= 0 && selectedItemIndex < items.Count)
        {
            Item selectedItem = items[selectedItemIndex];

            if (selectedItem.transform.parent == handSlot && selectedItem.gameObject.activeSelf)
            {
                return; 
            }
        }

        foreach (Transform child in handSlot)
        {
            if (selectedItemIndex >= 0 && selectedItemIndex < items.Count && child == items[selectedItemIndex].transform)
            {
                continue; 
            }

            child.DOScale(Vector3.zero, 0.3f)
                .OnComplete(() => child.gameObject.SetActive(false));
        }

        if (selectedItemIndex >= 0 && selectedItemIndex < items.Count)
        {
            Item selectedItem = items[selectedItemIndex];

            selectedItem.gameObject.SetActive(true);

            selectedItem.transform.SetParent(null);
            selectedItem.transform.SetParent(handSlot);

            selectedItem.transform.localPosition = new Vector3(0, -0.5f, 0.2f);
            selectedItem.transform.localRotation = Quaternion.identity;
            selectedItem.transform.localScale = Vector3.zero;

            selectedItem.transform.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutBack);
            selectedItem.transform.DOScale(Vector3.one, 0.3f)
                .OnComplete(() => isInteracting = false);
        }
        else 
        {
            isInteracting = false;
        }

        InventoryUI.Instance.UpdateInventoryUI(selectedItemIndex);
    }

// ВЫБРАСЫВАНИЕ

    public void ThrowItem()
    {
        if (isInteracting || selectedItemIndex < 0 || selectedItemIndex >= items.Count)
        {
            return;
        }

        isInteracting = true; 
        IsThrowing = true;

        Item selectedItem = items[selectedItemIndex];

        items.RemoveAt(selectedItemIndex);
        FindFirstObjectByType<InventoryUI>().UpdateInventory(GetItemSprites());

        selectedItemIndex = -1;

        FindFirstObjectByType<InventoryUI>().UpdateInventory(GetItemSprites());

        ThrowItemInWorld(selectedItem);

        InventoryUI.Instance.UpdateInventoryUI(selectedItemIndex);

        StartCoroutine(ResetStates(0.5f));
        
    }

    void ThrowItemInWorld(Item item)
    {
        item.transform.SetParent(null);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = item.gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        Vector3 throwDirection = FindFirstObjectByType<ItemPickup>().holdPoint.forward + Vector3.up * 0.5f; 
        rb.AddForce(throwDirection * 5f, ForceMode.Impulse); 
        
        StartCoroutine(EnableColliderAfterDelay(item.gameObject, 0.1f)); 
    }

    IEnumerator EnableColliderAfterDelay(GameObject itemObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider itemCollider = itemObject.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }
    }

    IEnumerator ResetStates(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsThrowing = false;
        isInteracting = false; 
    }
}