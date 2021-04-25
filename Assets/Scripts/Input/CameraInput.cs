using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour {

	[SerializeField]
	private GameObject menu;

	void Start() {
	}

	void Update() {
		// --- Input ---

		// - Toggle Menu -
		if (Input.GetKeyDown(KeyCode.Escape)) {
			ToggleMenu();
		}

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
		}
	}

	public void ToggleMenu() {
		menu.SetActive(!menu.activeSelf);
	}

	public void Quit() {
		Application.Quit();
	}

	private void move(int x, int y) {
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
	}
}
