using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySpawnBlock : PlayerAbility
{
	public static PushableBlock LastSpawnedBlock;
	public PushableBlock BlockPrefab;
	public Grid grid;

	protected override void InternalPickup() {
		if (grid == null) {
			grid = GameObject.FindObjectOfType<Grid>();
		}
	}

	protected override void InternalUpdate() {

	}

	protected override void InternalDrop() {
		_player.CanPush = false;
	}
	protected override void InternalUse() {
		print("test");
		if (_player.OnGround) {
			Vector2 direction = Vector2.right;
			if (!_player.facingRight) direction = Vector2.left;

			if (!_player.boxCaster.BoxcastInDirection(direction, 1, _player.GroundLayers)) {
				Vector2 targetPosition = (Vector2)_player.transform.position + direction;
				print("Target: " + targetPosition);
				Vector3Int cellCoordinates = grid.WorldToCell(targetPosition);
				print("Grid: " + cellCoordinates);
				Vector2 snappedPosition = grid.CellToWorld(cellCoordinates);
				snappedPosition.x += direction.x / 2;
				snappedPosition.y += .5f;
				print("Snapped: " + snappedPosition);
				//fast hack so i don't have to think, but spawning a block to the left is always offset 1 space, so i'll just... you know, hack it
				if (!_player.facingRight) {
					snappedPosition.x += 1;
				}
				_player.SoundPlayer.PlaySound(PlayerSound.SpawnBox);
				SpawnBlock(snappedPosition);

			}
			else {
				_player.SoundPlayer.PlaySound(PlayerSound.Error);
			}

		}
		
	}

	private void SpawnBlock(Vector2 position) {
		if(LastSpawnedBlock == null) {
			LastSpawnedBlock = Instantiate(BlockPrefab);
		}
		LastSpawnedBlock.transform.position = position;
		LastSpawnedBlock.CheckGround();
	}
}
