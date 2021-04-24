using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject {
	public enum TileType {
		ground_dirt_slope_left = 0,
		ground_dirt_slope_right,
		ground_dirt_flat,
		selection_slope_left,
		selection_slope_right,
		selection_flat,
		gold,
	}

	public TileBase tile;
	public TileType type;
}
