﻿using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    public Transform interactionTransform;
    public InteractableType interactableType;

    private bool isFocus = false;
    public Player player;

    private bool hasInteracted = false;

    public virtual void Interact () {
        Debug.Log("Interacting with: " + transform.name);
    }

    private void Update() {
        if (isFocus && !hasInteracted) {
            float distance = Vector3.Distance(player.transform.position, interactionTransform.position);
            if (distance <= radius) {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public void OnFocused (Player _player) {
        isFocus = true;
        player = _player;
        hasInteracted = false;
    }

    public void OnDefocused() {
        isFocus = false;
        player = null;
        hasInteracted = false;
    }

    private void OnDrawGizmosSelected() {
        if (interactionTransform == null) {
            interactionTransform = transform;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }

    private void OnMouseEnter() {
        HandleCursor.instance.setCursor(interactableType);
    }
    private void OnMouseExit() {
        HandleCursor.instance.setCursor();
    }
}

public enum InteractableType { PickUp, Enemy, Stairs, Door, Trap, Switch, Hidden }