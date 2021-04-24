using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour {

	private Rigidbody2D rb;
	private float task_progress = 0.0f;
	public Task task = null;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	void Update() {
		if (task != null) {
			float dir = task.pos.x - transform.position.x;
			const float work_range = 0.3f;
			const float work_range_move_factor = 1.5f;
			float task_dist = Mathf.Abs(dir);
			if (task_dist * work_range_move_factor > work_range) {
				if (task_dist > work_range) {
					task_progress = 0;
				}
				//Move toward closest task
				if (dir > 0) {
					move(1);
				} else {
					move(-1);
				}
			} else {
				//Work on task
				const float task_completion_speed = 1.0f;
				task_progress += task_completion_speed * Time.deltaTime;
				if (task.completion_time <= task_progress) {
					TaskManager.Instance.RemoveTask(task);
					task_progress = 0;
					task = null;
				}
			}
		}
	}

	private void move(int dir) {
		const float speed = 3f;
		//transform.Translate(new Vector3(dir * speed * Time.deltaTime, 0, 0));
		//rb.MovePosition(new Vector2(dir * speed * Time.deltaTime, 0));
		rb.velocity = new Vector2(speed * dir, rb.velocity.y);
	}
}
