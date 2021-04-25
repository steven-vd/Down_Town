using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour {
	public static TaskManager Instance;

	private Dictionary<int, GameObject> house_fxs = new Dictionary<int, GameObject>();

	[SerializeField]
	private GameObject house_fx;

	[SerializeField]
	private Tilemap map, foreground, selection, background;
	public Tilemap tmp_selection;
	[SerializeField]
	private List<TileData> tile_datas;
	[SerializeField]
	private GameObject PF_Worker;

	[SerializeField]
	private Text ui_text_gold;

	public Dictionary<TileBase, TileData> data_from_base;
	private Camera main_cam;
	private List<Task> tasks = new List<Task>();
	public List<Worker> workers = new List<Worker>();

	public static bool build_mode = false;
	public static int _gold_amnt;

	private const int map_initial_width = 40;
	private const int map_initial_depth = -15;

	public static int map_left;
	public static int map_right;
	public static int map_depth;

	private bool should_reset = false;

	private void Awake() {
		Instance = this;
	}

	void Start() {
		main_cam = Camera.main;
		data_from_base = new Dictionary<TileBase, TileData>();
		foreach (TileData data in tile_datas) {
			data_from_base.Add(data.tile, data);
		}
		init();
		ui_text_gold.text = "";
	}

	private void init() {
		map.ClearAllTiles();
		foreground.ClearAllTiles();
		selection.ClearAllTiles();
		tasks.Clear();

		_gold_amnt = 3;
		AddGold(0);
		map_left = -map_initial_width / 2;
		map_right = map_initial_width / 2;
		map_depth = map_initial_depth;
		main_cam.transform.position = new Vector3(0, 2, 0);
		for (int i = -map_initial_width / 2; i < map_initial_width / 2; ++i) {
			populateColumn(i);
		}
		build_mode = true;
	}

	private void Update() {
		if (should_reset) {
			should_reset = false;
			init();
			return;
		}
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

	public void AddGold(int amnt) {
		_gold_amnt += amnt;
		ui_text_gold.text = "Gold: " + _gold_amnt;
		if (amnt > 0) {
			SoundManager.Instance.PlayGoldDing();
		}
	}

	public Vector2Int GetEdges() {
		int left = -map_initial_width / 2 + 1, right = map_initial_width / 2 - 1;

		while (map.GetTile(new Vector3Int(left--, 0, 0)) == null) { }
		while (map.GetTile(new Vector3Int(right++, 0, 0)) == null) { }

		return new Vector2Int(left, right);
	}

	public void SpawnWorker(int x, int y) {
		workers.Add(Instantiate<GameObject>(PF_Worker, new Vector3(x, y, 10), Quaternion.identity).GetComponent<Worker>());
	}

	public void OnMouseMove(Vector3 mouse_pos) {
		Vector2 world_pos = main_cam.ScreenToWorldPoint(mouse_pos);
		Vector3Int cell_coord = map.WorldToCell(world_pos);
		if (!build_mode) {
			tmp_selection.color = new Color(0, 1, 0, 0.4f);
			tmp_selection.SetTile(cell_coord, tile_datas[(int)TileData.TileType.selection_flat].tile);
		} else {
			int x = cell_coord.x, y = cell_coord.y;
			TileBase brick = tile_datas[(int)TileData.TileType.brick_flat].tile;
			TileBase slope_left = tile_datas[(int)TileData.TileType.brick_slope_left].tile;
			TileBase slope_right = tile_datas[(int)TileData.TileType.brick_slope_right].tile;
			TileBase door = tile_datas[(int)TileData.TileType.door].tile;

			if (map.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y - 1, 0)) == null ||
				data_from_base[map.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y - 1, 0))].type != TileData.TileType.ground_dirt_flat ||
				map.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y - 1, 0)) == null ||
				data_from_base[map.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y - 1, 0))].type != TileData.TileType.ground_dirt_flat ||
				map.GetTile(new Vector3Int(cell_coord.x + 0, cell_coord.y - 1, 0)) == null ||
				data_from_base[map.GetTile(new Vector3Int(cell_coord.x + 0, cell_coord.y - 1, 0))].type != TileData.TileType.ground_dirt_flat ||
				map.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y - 1, 0)) == null ||
				data_from_base[map.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y - 1, 0))].type != TileData.TileType.ground_dirt_flat ||
				map.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y, 0)) != null ||
				map.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y, 0)) != null ||
				map.GetTile(cell_coord) != null ||
				map.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y, 0)) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y, 0)) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y, 0)) != null ||
				foreground.GetTile(cell_coord) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y, 0)) != null) {
				tmp_selection.color = new Color(1, 0, 0, 0.4f);
			} else {
				tmp_selection.color = new Color(0, 1, 0, 0.4f);
			}

			//Left
			tmp_selection.SetTile(new Vector3Int(x + -2, y + 0, 0), door);
			tmp_selection.SetTile(new Vector3Int(x + -2, y + 1, 0), brick);
			tmp_selection.SetTile(new Vector3Int(x + -2, y + 2, 0), slope_right);
			tmp_selection.SetTile(new Vector3Int(x + -1, y + 2, 0), brick);
			tmp_selection.SetTile(new Vector3Int(x + -1, y + 3, 0), slope_right);
			//Right
			tmp_selection.SetTile(new Vector3Int(x + 1, y + 0, 0), door);
			tmp_selection.SetTile(new Vector3Int(x + 1, y + 1, 0), brick);
			tmp_selection.SetTile(new Vector3Int(x + 1, y + 2, 0), slope_left);
			tmp_selection.SetTile(new Vector3Int(x + 0, y + 2, 0), brick);
			tmp_selection.SetTile(new Vector3Int(x + 0, y + 3, 0), slope_left);
		}
	}

	public void MousePressed(Vector3 mouse_pos) {
		if (!build_mode) {
			Vector2 world_pos = main_cam.ScreenToWorldPoint(mouse_pos);
			Vector3Int cell_coord = map.WorldToCell(world_pos);
			TileBase map_tile = map.GetTile(cell_coord);
			if (map_tile != null) {
				if (data_from_base[map_tile].type == TileData.TileType.gold) {
					AddTask(new Task(Task.Type.mine, cell_coord, 2.5f, null));
				} else {
					AddTask(new Task(Task.Type.mine, cell_coord, 1.0f, null));
				}
			}
		}
	}

	public void OnRight(Vector3 mouse_pos) {
		Vector2 world_pos = main_cam.ScreenToWorldPoint(mouse_pos);
		Vector3Int cell_coord = map.WorldToCell(world_pos);
		RemoveTask(GetTaskAt(cell_coord), false);
	}

	public void OnClick(Vector3 mouse_pos) {
		if (build_mode) {
			//TODO ? Add a task for this???
			Vector2 world_pos = main_cam.ScreenToWorldPoint(mouse_pos);
			Vector3Int cell_coord = map.WorldToCell(world_pos);
			const int house_price = 3;
			if (_gold_amnt < house_price ||
				map.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y - 1, 0)) == null ||
				data_from_base[map.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y - 1, 0))].type != TileData.TileType.ground_dirt_flat ||
				map.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y - 1, 0)) == null ||
				data_from_base[map.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y - 1, 0))].type != TileData.TileType.ground_dirt_flat ||
				map.GetTile(new Vector3Int(cell_coord.x + 0, cell_coord.y - 1, 0)) == null ||
				data_from_base[map.GetTile(new Vector3Int(cell_coord.x + 0, cell_coord.y - 1, 0))].type != TileData.TileType.ground_dirt_flat ||
				map.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y - 1, 0)) == null ||
				data_from_base[map.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y - 1, 0))].type != TileData.TileType.ground_dirt_flat ||
				map.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y, 0)) != null ||
				map.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y, 0)) != null ||
				map.GetTile(cell_coord) != null ||
				map.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y, 0)) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x - 2, cell_coord.y, 0)) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x - 1, cell_coord.y, 0)) != null ||
				foreground.GetTile(cell_coord) != null ||
				foreground.GetTile(new Vector3Int(cell_coord.x + 1, cell_coord.y, 0)) != null) {
				return;
			}

			AddGold(-house_price);
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
		//Background
		background.SetTile(new Vector3Int(x - 1, y + 0, 0), brick);
		background.SetTile(new Vector3Int(x - 1, y + 1, 0), brick);
		background.SetTile(new Vector3Int(x + 0, y + 0, 0), brick);
		background.SetTile(new Vector3Int(x + 0, y + 1, 0), brick);

		//FX
		house_fxs.Add(x, Instantiate<GameObject>(house_fx, new Vector3(pos.x, pos.y + 1, 0), Quaternion.identity));

		SpawnWorker(x, y + 1);
	}

	private void AddTask(Task task) {
		TileBase selection_tile = selection.GetTile(task.pos);
		if (selection_tile != null) {
			return;
		}

		//Add Task
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

	public Task GetTaskAt(Vector3Int cell_coord) {
		foreach (Task t in tasks) {
			if (t.pos.x == cell_coord.x && t.pos.y == cell_coord.y) {
				return t;
			}
		}
		return null;
	}

	public void RemoveTask(Task task, bool success) {
		if (task == null) {
			return;
		}
		if (success) {
			mineTile(task.pos);
		}
		selection.SetTile(task.pos, null);
		tasks.Remove(task);
	}

	public bool isReachable(Vector3Int pos) {
		return
			foreground.GetTile(new Vector3Int(pos.x, pos.y + 1, 0)) == null &&
			(
				map.GetTile(new Vector3Int(pos.x, pos.y + 1, 0)) == null ||
				map.GetTile(new Vector3Int(pos.x + 1, pos.y, 0)) == null ||
				map.GetTile(new Vector3Int(pos.x - 1, pos.y, 0)) == null
			);
	}

	private void checkIntegrity(Vector3Int pos) {
		if (foreground.GetTile(pos) != null) {
			SoundManager.Instance.PlayCrash((pos.x - main_cam.transform.position.x) / 100.0f);
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

			GameObject tmp_house_fx = house_fxs[x];
			house_fxs.Remove(x);
			Destroy(tmp_house_fx);

			// Kill random worker
			Destroy(workers[workers.Count - 1].gameObject);
			workers.RemoveAt(workers.Count - 1);
			if (workers.Count == 0) {
				should_reset = true;
			}
			return;
		} else if (map.GetTile(pos) == null) {
			return;
		}
		TileData.TileType type_below = data_from_base[map.GetTile(new Vector3Int(pos.x, pos.y - 1, 0))].type;
		if (type_below != TileData.TileType.ground_dirt_flat && type_below != TileData.TileType.gold) {
			mineTile(pos);
		}
	}

	private const int min_goldness = 96;
	private void populateRow(int row) {
		for (int x = map_left; x < map_right; ++x) {
			background.SetTile(new Vector3Int(x, row, 0), tile_datas[(int)TileData.TileType.ground_dirt_flat].tile);
			int goldness = Random.Range(0, 100);
			if (goldness > min_goldness) {
				map.SetTile(new Vector3Int(x, row, 0), tile_datas[(int)TileData.TileType.gold].tile);
			} else {
				map.SetTile(new Vector3Int(x, row, 0), tile_datas[(int)TileData.TileType.ground_dirt_flat].tile);
			}
		}
	}

	private void populateColumn(int col) {
		for (int depth = 0; depth > map_depth; --depth) {
			background.SetTile(new Vector3Int(col, depth, 0), tile_datas[(int)TileData.TileType.ground_dirt_flat].tile);
			int goldness = Random.Range(0, 100);
			if (goldness > min_goldness) {
				map.SetTile(new Vector3Int(col, depth, 0), tile_datas[(int)TileData.TileType.gold].tile);
			} else {
				map.SetTile(new Vector3Int(col, depth, 0), tile_datas[(int)TileData.TileType.ground_dirt_flat].tile);
			}
		}
	}

	private void mineTile(Vector3Int pos) {
		if (map.GetTile(pos) == null) {
			return;
		}
		Vector3Int right = new Vector3Int(pos.x + 1, pos.y, 0);
		Vector3Int left = new Vector3Int(pos.x - 1, pos.y, 0);
		if (data_from_base[map.GetTile(pos)].type == TileData.TileType.gold) {
			AddGold(1);
		}
		map.SetTile(pos, null);

		if (map.GetTile(right) != null) {
			map.SetTile(right, tile_datas[(int)TileData.TileType.ground_dirt_slope_right].tile);
			checkIntegrity(new Vector3Int(pos.x + 1, pos.y + 1, 0));
		} else if (right.x + 1 > map_right) {
			populateColumn(right.x);
			++map_right;
		}

		if (map.GetTile(left) != null) {
			map.SetTile(left, tile_datas[(int)TileData.TileType.ground_dirt_slope_left].tile);
			checkIntegrity(new Vector3Int(pos.x - 1, pos.y + 1, 0));
		} else if (left.x - 2 < map_left) {
			populateColumn(left.x);
			--map_left;
		}

		if (pos.y + map_initial_depth < map_depth) {
			populateRow(pos.y + map_initial_depth + 1);
			--map_depth;
		}
	}

}
