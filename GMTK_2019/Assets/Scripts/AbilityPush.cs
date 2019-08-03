using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPush : PlayerAbility
{
	protected override void InternalPickup() {
		_player.CanPush = true;
	}

	protected override void InternalUpdate() {

	}

	protected override void InternalDrop() {
		_player.CanPush = false;
	}
	protected override void InternalUse() {

	}
}
