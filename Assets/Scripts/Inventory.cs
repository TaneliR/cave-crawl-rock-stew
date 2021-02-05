using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region SingletonPattern
    public static Inventory instance;
    private void Awake() {
        if (instance != null) {
            Debug.LogWarning("Alert! Multipler inventories in scene!");
            return;
        }
        instance = this;
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 20;

    public List<Item> items = new List<Item>();

    public bool Add (Item item) {
        if (!item.isDefaultItem) {

            if (items.Count >= space) {
                Debug.Log("Inventory full!");
                return false;
            }

            items.Add(item);
            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
        return true;
    }

    public void Remove(Item item) {
        items.Remove(item);
        if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
    }
}
