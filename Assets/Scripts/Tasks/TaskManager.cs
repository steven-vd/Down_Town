using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TaskManager : MonoBehaviour {
	public static TaskManager Instance;

	[SerializeField]
	private Tilemap map, foreground, selection;
	[SerializeField]
	private List<TileData> tile_datas;
	[SerializeField]
	private GameObject PF_Worker;

	public Dictionary<TileBase, TileData> data_from_base;
	private Camera main_cam;
	private List<Task> tasks = new List<Task>();
	public List<Worker> workers = new List<Worker>();

	public static bool build_mode = false;
	public static int gold_amnt = 0;

	private void Awake() {
		Instance = this;
	}

	void Start() {
		const int width = 20;
		const int depth = -10;
		const int min_goldness = 95;
		for (int i = -width / 2; i < width / 2; ++i) {
			for (int j = 0; j > depth; --j) {
				int goldness = Random.Range(0, 100);
				if (goldness > min_goldness) {
					map.SetTile(new Vector3Int(i, j, 0), tile_datas[(int)TileData.TileType.gold].tile);
				} else {
					map.SetTile(new Vector3Int(i, j, 0), tile_datas[(int)TileData.TileType.ground_dirt_flat].tile);
				}
			}
		}

		main_cam = Camera.main;
		data_from_base = new Dictionary<TileBase, TileData>();
		foreach (TileData data in tile_datas) {
			data_from_base.Add(data.tile, data);
		}

		//DEBUG
		buildHouse(new Vector3Int(0, 1, 0));
	}

	private void Update() {
		foreach (Task task in tasks) {
			if (task.worker == null) {
				int w = task.GetClosestWorkerWithoutTask();
				if (w != -1) {
					workers[w].task = task;
					task.worker = workers[w];
				}
			}
		}
	}

	public void SpawnWorker(int x, int y) {
		workers.Add(Instantiate<GameObject>(PF_Worker, new Vector3(x, y, 10), Quaternion.identity).GetComponent<Worker>());
	}

	public void MousePressed(Vector3 mouse_pos) {
		if (!build_mode) {
			Vector2 world_pos = main_cam.ScreenToWorldPoint(mouse_pos);
			Vector3Int cell_coord = map.WorldToCell(world_pos);
			TileBase map_tile = map.GetTile(cell_coord);
			if (map_tile != null) {
				AddTask(new Task(Task.Type.mine, cell_coord, 1.0f, null));
			}
		}
	}

	public void OnClick(Vector3 mouse_pos) {
		if (build_mode) {
			//TODO ? Add a task for this???
			Vector2 world_pos = main_cam.ScreenToWorldPoint(mouse_pos);
			Vector3Int cell_coord = map.WorldToCell(world_pos);
			const int house_price = 3;
			if (gold_amnt < house_price ||
				map.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y, 0)) != null ||
				map.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y, 0)) != null ||
				map.GetTile(cell_coord) != null ||
				map.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y, 0)) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y, 0)) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y, 0)) != null ||
				foreground.GetTile(cell_coord) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y, 0)) != null ) {
				return;
			}

			gold_amnt -= house_price;
			buildHouse(cell_coord);
		}
	}

	private void buildHouse(Vector3Int pos) {
		int x = pos.x, y = pos.y;
		TileBase brick = tile_datas[(int)TileData.TileType.brick_flat].tile;
		TileBase slope_left = tile_datas[(int)TileData.TileType.brick_slope_left].tile;
		TileBase slope_right = tile_datas[(int)TileData.TileType.brick_slope_right].tile;
		TileBase door = tile_datas[(int)TileData.TileType.door].tile;

		//Left
		foreground.SetTile(new Vector3Int(x + -2, y + 0, 0), door);
		foreground.SetTile(new Vector3Int(x + -2, y + 1, 0), brick);
		foreground.SetTile(new Vector3Int(x + -2, y + 2, 0), slope_right);
		foreground.SetTile(new Vector3Int(x + -1, y + 2, 0), brick);
		foreground.SetTile(new Vector3Int(x + -1, y + 3, 0), slope_right);
		//Right
		foreground.SetTile(new Vector3Int(x + 1, y + 0, 0), door);
		foreground.SetTile(new Vector3Int(x + 1, y + 1, 0), brick);
		foreground.SetTile(new Vector3Int(x + 1, y + 2, 0), slope_left);
		foreground.SetTile(new Vector3Int(x + 0, y + 2, 0), brick);
		foreground.SetTile(new Vector3Int(x + 0, y + 3, 0), slope_left);

		SpawnWorker(x, y + 1);
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
				selection_tile = tile_datas[(int)TileData.TileType.selection_slope_left].tile;
				break;
			case TileData.TileType.ground_dirt_slope_right:
				selection_tile = tile_datas[(int)TileData.TileType.selection_slope_right].tile;
				break;
			default:
				selection_tile = tile_datas[(int)TileData.TileType.selection_flat].tile;
				break;
		}
		selection.SetTile(task.pos, selection_tile);
	}

	public Task GetTask(int i) {
		return tasks[i];
	}

	public void RemoveTask(Task task) {
		switch (task.type) {
			case Task.Type.mine:
				mineTile(task.pos);
				break;
			case Task.Type.build:
				break;
		}
		selection.SetTile(task.pos, null);
		tasks.Remove(task);
	}

	public bool isReachable(Vector3Int pos) {
		return
			map.GetTile(new Vector3Int(pos.x, pos.y + 1, 0)) == null ||
			map.GetTile(new Vector3Int(pos.x + 1, pos.y, 0)) == null ||
			map.GetTile(new Vector3Int(pos.x - 1, pos.y, 0)) == null;
	}

	private void checkIntegrity(Vector3Int pos) {
		if (foreground.GetTile(pos) != null) {
			// Destroy House
			int x, y = pos.y;
			if (foreground.GetTile(new Vector3Int(pos.x + 3, pos.y, 0)) != null) {
				x = pos.x + 2;
			} else {
				x = pos.x - 1;
			}
			//Left
			foreground.SetTile(new Vector3Int(x + -2, y + 0, 0), null);
			foreground.SetTile(new Vector3Int(x + -2, y + 1, 0), null);
			foreground.SetTile(new Vector3Int(x + -2, y + 2, 0), null);
			foreground.SetTile(new Vector3Int(x + -1, y + 2, 0), null);
			foreground.SetTile(new Vector3Int(x + -1, y + 3, 0), null);
			//Right
			foreground.SetTile(new Vector3Int(x + 1, y + 0, 0), null);
			foreground.SetTile(new Vector3Int(x + 1, y + 1, 0), null);
			foreground.SetTile(new Vector3Int(x + 1, y + 2, 0), null);
			foreground.SetTile(new Vector3Int(x + 0, y + 2, 0), null);
			foreground.SetTile(new Vector3Int(x + 0, y + 3, 0), null);

			// Kill random worker
			Destroy(workers[workers.Count - 1].gameObject);
			workers.RemoveAt(workers.Count - 1);
			return;
		} else if (map.GetTile(pos) == null) {
			return;
		}
		TileData.TileType type_below = data_from_base[map.GetTile(new Vector3Int(pos.x, pos.y - 1, 0))].type;
		if (type_below != TileData.TileType.ground_dirt_flat && type_below != TileData.TileType.gold) {
			mineTile(pos);
		}
	}

	private void mineTile(Vector3Int pos) {
		if (map.GetTile(pos) == null) {
			return;
		}
		Vector3Int right = new Vector3Int(pos.x + 1, pos.y, 0);
		Vector3Int left = new Vector3Int(pos.x - 1, pos.y, 0);
		if (data_from_base[map.GetTile(pos)].type == TileData.TileType.gold) {
			++gold_amnt;
		}
		map.SetTile(pos, null);
		if (map.GetTile(right) != null) {
			map.SetTile(right, tile_datas[(int)TileData.TileType.ground_dirt_slope_right].tile);
			checkIntegrity(new Vector3Int(pos.x + 1, pos.y + 1, 0));
		}
		if (map.GetTile(left) != null) {
			map.SetTile(left, tile_datas[(int)TileData.TileType.ground_dirt_slope_left].tile);
			checkIntegrity(new Vector3Int(pos.x - 1, pos.y + 1, 0));
		}
	}

}
