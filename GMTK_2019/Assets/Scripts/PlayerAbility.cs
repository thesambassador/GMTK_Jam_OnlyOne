using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
	protected PlayerControls _player;
	public bool HeldByPlayer = false;
	public bool CanBePickedUp = true;

	public string AbilityName;
	public Sprite AbilitySprite;
	public string AbilityDescription;

    public void PickupAbility(PlayerControls player) {
		HeldByPlayer = true;
		_player = player;
		this.gameObject.SetActive(false);

		AbilityPanel.SetAbilityInfo(this);

		InternalPickup();
	}

	public void UseAbility() {
		InternalUse();
	}

	public void DropAbility(Vector2 dropPosition) {
		InternalDrop();

		this.gameObject.SetActive(true);
		this.transform.position = dropPosition;
		CanBePickedUp = false;
		_player = null;
	}

	public void UpdateAbility() {
		InternalUpdate();
	}

	protected virtual void InternalPickup() { }
	protected virtual void InternalUse() { }
	protected virtual void InternalDrop() { }
	protected virtual void InternalUpdate() { }

	public void OnTriggerExit2D(Collider2D collision) {
		if(collision.tag == "Player") {
			CanBePickedUp = true;
		}
	}
}
