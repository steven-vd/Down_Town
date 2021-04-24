using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TaskManager : MonoBehaviour {
	[SerializeField]
	private Tilemap map, selection;
	[SerializeField]
	private List<TileData> tile_datas;

	[SerializeField]
	public Dictionary<TileBase, TileData> data_from_base;
	private Camera main_cam;
	private Queue<Task> tasks;

	void Start() {
		main_cam = Camera.main;
		data_from_base = new Dictionary<TileBase, TileData>();
		foreach (TileData data in tile_datas) {
			data_from_base.Add(data.tile, data);
		}
	}

	void Update() {
		if (Input.GetMouseButton(0)) {
			Vector2 world_pos = main_cam.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int cell_coord = map.WorldToCell(world_pos);
			TileBase map_tile = map.GetTile(cell_coord);
			if (map_tile != null) {
				AddTask(new Task(Task.Type.mine, cell_coord));
			}
		}
	}

	private void AddTask(Task task) {
		//TODO add queue, prob with ref to worker
		TileBase selection_tile = selection.GetTile(task.pos);
		if (selection_tile != null) {
			return;
		}
		TileBase map_tile = map.GetTile(task.pos);
		switch (data_from_base[map_tile].type) {
			case TileData.TileType.ground_dirt_slope_left:
				selection_tile = tile_datas[4].tile;
				break;
			case TileData.TileType.ground_dirt_slope_right:
				selection_tile = tile_datas[5].tile;
				break;
			default:
				selection_tile = tile_datas[3].tile;
				break;
		}
		selection.SetTile(task.pos, selection_tile);
	}
}
