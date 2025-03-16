using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class BackpackUI : MonoBehaviour
{
    public GameObject uiPanel; // inventory panel
    public GameObject slotPrefab; // slot prefab
    public Transform slotParent; // slot grid

    private List<GameObject> slots = new List<GameObject>();

    private bool isInventoryOpen = false; 

    public GraphicRaycaster raycaster;
    private GameObject activeSlot = null; 

    void Update()
    {

        if (GetUIObjectUnderCursor(out GameObject uiObject) && uiObject.CompareTag("Slot"))
        {
            activeSlot = uiObject;
        }

        // open/close invetory
        if (Input.GetMouseButtonDown(0) && CheckMouseHit())
        {
            ToggleInventory();
        }

        // removing object
        if (Input.GetMouseButtonUp(0) && activeSlot != null)
        {
            RetrieveItem(activeSlot.GetComponent<ItemHolder>().item);
            activeSlot = null;
            CloseInventory();
        }
    }

    private bool GetUIObjectUnderCursor(out GameObject uiObject)
    {
        uiObject = null;
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        if (results.Count > 0)
        {
            uiObject = results[0].gameObject;
            return true;
        }

        return false;
    }

    private bool CheckMouseHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        uiPanel.SetActive(isInventoryOpen);
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        uiPanel.SetActive(false);
    }

    public void UpdateInventory(List<GameObject> items)
    {
        // clear slots
        foreach (GameObject slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();

        // add slot
        foreach (GameObject item in items)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            newSlot.GetComponent<Image>().sprite = item.GetComponent<Item>().icon;
            newSlot.AddComponent<ItemHolder>().item = item;
            newSlot.GetComponent<Button>().onClick.AddListener(() => RetrieveItem(item));
            slots.Add(newSlot);
        }
    }

    private void RetrieveItem(GameObject item)
    {
        if (item == null) return;

        Backpack backpack = FindObjectOfType<Backpack>();
        if (backpack != null)
        {
            backpack.RetrieveItem(item);
            UpdateInventory(backpack.GetStoredItems());
        }
    }
}
