using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour {
	void Start() {
	}

	void Update() {
		// --- Input ---
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
		//Mouse click
		if (Input.GetMouseButton(0)) {
			TaskManager.Instance.OnClick(Input.mousePosition);
		}
	}
	private void move(int x, int y) {
		float speed = InputManager.cam_move_speed * Time.deltaTime;
		transform.Translate(x * speed, y * speed, 0);
	}
}
