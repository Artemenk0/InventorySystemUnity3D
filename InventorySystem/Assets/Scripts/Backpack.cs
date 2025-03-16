using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class Backpack : MonoBehaviour
{
    private List<GameObject> storedItems = new List<GameObject>(); // objects list
    public Transform backpackStoragePoint; // items return point

    private static readonly string serverUrl = "https://wadhub.manerai.com/api/inventory/status";
    private static readonly string authToken = "Bearer KPEr6nYdMAY46aSy8CEznasAgsWM84Nx7SKM4Q85sPq6c7sTWF6xzhxPrDh8MaP";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item")) 
        {
            StoreItem(other.gameObject);
            SendInventoryUpdate(other.gameObject.name, "added"); 
        }
    }

    // saved items list
    public List<GameObject> GetStoredItems()
    {
        return storedItems;
    }

    // save objects 
    private void StoreItem(GameObject item)
    {
        storedItems.Add(item);
        item.SetActive(false);

        // update UI
        FindObjectOfType<BackpackUI>().UpdateInventory(storedItems); 
    }

    // removing item
    public void RetrieveItem(GameObject item)
    {
        if (storedItems.Contains(item))
        {
            storedItems.Remove(item);
            item.SetActive(true);
            item.transform.position = backpackStoragePoint.position;

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null) rb.useGravity = true;

            FindObjectOfType<BackpackUI>().UpdateInventory(storedItems); // update UI

            SendInventoryUpdate(item.name, "removed");
        }
    }

    // send updates
    private async void SendInventoryUpdate(string itemName, string action)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", authToken);

            string json = $"{{\"item\":\"{itemName}\",\"action\":\"{action}\"}}";
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(serverUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Debug.Log($"Server response: {await response.Content.ReadAsStringAsync()}");
            }
            else
            {
                Debug.LogError($"Error sending data: {response.StatusCode}");
            }
        }
    }
}
