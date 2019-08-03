using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDoubleJump : PlayerAbility
{
	protected override void InternalPickup() {
		_player.SetNumJumps(1);
	}

	protected override void InternalUpdate() {

	}

	protected override void InternalDrop() {
		_player.SetNumJumps(0);
	}
	protected override void InternalUse() {

	}
}
