using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Battle {
	public class JobManager : MonoBehaviour {
		Queue<Job> jobQueue = new Queue<Job>();
	}

	struct Job {
			public Vector2 location;
			public Action<Vector2[], bool> callback;

			public Job(Vector2 _location, Vector2 _pathEnd, Action<Vector2[], bool> _callback) {
				location = _location;
				callback = _callback;
			}
		}
}