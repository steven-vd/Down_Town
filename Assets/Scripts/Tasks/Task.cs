using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Task {
	public enum Type {
		mine,
		build,
	}

	public Type type;
	public Vector3Int pos;
	public float completion_time;

	public Task(Type type, Vector3Int pos, float completion_time) {
		this.type = type;
		this.pos = pos;
		this.completion_time = completion_time;
	}
}
