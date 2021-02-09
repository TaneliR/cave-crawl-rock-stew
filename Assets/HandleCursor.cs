using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCursor : MonoBehaviour
{
    #region SingletonPattern

    public static HandleCursor instance;

    void Awake() {
        instance = this;
    }

    #endregion

    public Texture2D defaultCursor;
    public Texture2D errorCursor;
    public Texture2D pickupCursor;
    public Texture2D attackCursor;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public void setCursor(InteractableType type) {
        switch(type){
            case InteractableType.PickUp: {
                Cursor.SetCursor(pickupCursor, hotSpot, cursorMode);
                break;
            }
            case InteractableType.Enemy: {
                Cursor.SetCursor(attackCursor, hotSpot, cursorMode);
                break;
            }
            default: {
                Cursor.SetCursor(errorCursor, hotSpot, cursorMode);
                Debug.LogWarning("Unknown interactable! No cursor.");
                break;
            }
        }
    }
    public void setCursor() {
        Cursor.SetCursor(defaultCursor, hotSpot, cursorMode);
    }
}
