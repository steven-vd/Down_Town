using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task {
	public enum Type {
		mine,
		build,
	}

	public Type type;
	public Vector3Int pos;
	public float completion_time;
	public Worker worker;

	public int GetClosestWorkerWithoutTask() {
		if (TaskManager.Instance.workers.Count == 0) {
			return -1;
		}
		int closest = -1;
		float closest_dist = Mathf.Sqrt(
			Mathf.Pow(pos.x - TaskManager.Instance.workers[0].gameObject.transform.position.x, 2) +
			Mathf.Pow(pos.y - TaskManager.Instance.workers[0].gameObject.transform.position.y, 2)) + 1;
		for (int i = 0; i < TaskManager.Instance.workers.Count; ++i) {
			Worker w = TaskManager.Instance.workers[i];
			if (w.task == null) {
				if (Mathf.Sqrt(
					Mathf.Pow(pos.x - w.gameObject.transform.position.x, 2) +
					Mathf.Pow(pos.y - w.gameObject.transform.position.y, 2)) < closest_dist) {
					closest = i;
					closest_dist = Mathf.Sqrt(
						Mathf.Pow(pos.x - TaskManager.Instance.workers[closest].gameObject.transform.position.x, 2) +
						Mathf.Pow(pos.y - TaskManager.Instance.workers[closest].gameObject.transform.position.y, 2));
				}
			}
		}
		return closest;
	}

	public Task(Type type, Vector3Int pos, float completion_time, Worker worker) {
		this.type = type;
		this.pos = pos;
		this.completion_time = completion_time;
		this.worker = worker;
	}
}
