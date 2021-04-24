using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TaskManager : MonoBehaviour {
	public static TaskManager Instance;

	[SerializeField]
	private Tilemap map, selection;
	[SerializeField]
	private List<TileData> tile_datas;

	[SerializeField]
	public Dictionary<TileBase, TileData> data_from_base;
	private Camera main_cam;
	private List<Task> tasks = new List<Task>();

	private void Awake() {
		Instance = this;
	}

	void Start() {
		main_cam = Camera.main;
		data_from_base = new Dictionary<TileBase, TileData>();
		foreach (TileData data in tile_datas) {
			data_from_base.Add(data.tile, data);
		}
	}

	public void OnClick(Vector3 mouse_pos) {
		Vector2 world_pos = main_cam.ScreenToWorldPoint(mouse_pos);
		Vector3Int cell_coord = map.WorldToCell(world_pos);
		TileBase map_tile = map.GetTile(cell_coord);
		if (map_tile != null) {
			AddTask(new Task(Task.Type.mine, cell_coord, 1.0f));
		}
	}

	private void AddTask(Task task) {
		TileBase selection_tile = selection.GetTile(task.pos);
		if (selection_tile != null) {
			return;
		}

		//Add Task
		//TODO check if another task of a different type is already present?
		tasks.Add(task);
		//Visualise
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

	public int GetClosestTask(Vector2Int pos) {
		if (tasks.Count == 0) {
			return -1;
		}
		int closest = 0;
		for (int i = 0; i < tasks.Count; ++i) {
			Task t = tasks[i];
			if (System.Math.Abs(t.pos.x - pos.x) + System.Math.Abs(t.pos.y - pos.y) <
				System.Math.Abs(tasks[closest].pos.x - pos.x) + System.Math.Abs(tasks[closest].pos.y - pos.y)) {
				closest = i;
			}
		}
		return closest;
	}

	public Task GetTask(int i) {
		return tasks[i];
	}

	public void RemoveTask(Task task) {
		selection.SetTile(task.pos, null);
		tasks.Remove(task);
	}
}
