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

	public Task(Type type, Vector3Int pos) {
		this.type = type;
		this.pos = pos;
	}
}
