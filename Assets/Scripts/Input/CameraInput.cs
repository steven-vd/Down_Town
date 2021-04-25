using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraInput : MonoBehaviour {

	[SerializeField]
	private GameObject menu;
	[SerializeField]
	private Text personal_deepest_ui_text;
	[SerializeField]
	private GameObject sky;

	private void Start() {
		personal_deepest_ui_text.text = "Personal Best: " + TaskManager.Instance.personal_deepest;
	}

	void Update() {
		// --- Input ---

		// - Toggle Menu -
		if (Input.GetKeyDown(KeyCode.Escape)) {
			ToggleMenu();
		}

		TaskManager.Instance.tmp_selection.ClearAllTiles();
		if (menu.activeSelf) {
			return;
		}

		// - Camera Movement -
		if (Input.GetKey(InputManager.cam_up)) {
			move(0, 1);
		}
		if (Input.GetKey(InputManager.cam_left)) {
			move(-1, 0);
		}
		if (Input.GetKey(InputManager.cam_down)) {
			move(0, -1);
		}
		if (Input.GetKey(InputManager.cam_right)) {
			move(1, 0);
		}

		TaskManager.Instance.OnMouseMove(Input.mousePosition);
		// - Mouse Left Click - 
		if (Input.GetMouseButtonDown(0)) {
			TaskManager.Instance.OnClick(Input.mousePosition);
		}
		// - Mouse Right Click - 
		if (Input.GetMouseButton(1)) {
			TaskManager.Instance.OnRight(Input.mousePosition);
		}
		// - Mouse Btn Hold -
		if (Input.GetMouseButton(0)) {
			TaskManager.Instance.MousePressed(Input.mousePosition);
		}
		// - Switch Build / Mine Mode -
		if (Input.GetKeyDown(InputManager.switch_mode)) {
			TaskManager.build_mode = !TaskManager.build_mode;
			if (TaskManager.Instance.hint_stage == 13) {
				TaskManager.Instance.hint.text = "Left Mouse Button to Mine";
				++TaskManager.Instance.hint_stage;
			}
		}
	}

	public void ToggleMenu() {
		++TaskManager.Instance.hint_stage;
		menu.SetActive(!menu.activeSelf);
		menu.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Resume";
		TaskManager.Instance.AddGold(0);
		TaskManager.Instance.should_time = true;
		if (menu.activeSelf) {
			personal_deepest_ui_text.text = "Personal Best: " + TaskManager.Instance.personal_deepest;
		} else {
			personal_deepest_ui_text.text = "";
		}
	}

	public void Quit() {
		Application.Quit();
	}

	private void move(int x, int y) {
		if (TaskManager.Instance.hint_stage == 11) {
			TaskManager.Instance.hint.text = "Left Click to build house";
			++TaskManager.Instance.hint_stage;
			if (TaskManager.Instance.workers.Count > 0 && TaskManager.Instance.hint_stage == 12) {
				TaskManager.Instance.hint.text = "Press <Space> to toggle Build/Mine Mode";
				++TaskManager.Instance.hint_stage;
			}
		}
		//Check left right bounds
		const int cam_margin = 10;
		if (transform.position.x + x - cam_margin < TaskManager.map_left || transform.position.x + x + cam_margin > TaskManager.map_right) {
			return;
		}
		//Check top bottom bounds
		const int top = 15;
		if (transform.position.y + y - cam_margin < TaskManager.map_depth || transform.position.y + y + cam_margin > top) {
			return;
		}

		float speed = InputManager.cam_move_speed * Time.deltaTime;
		transform.Translate(x * speed, y * speed, 0);
		sky.transform.position = new Vector3(transform.position.x, 4, 14);
	}
}
