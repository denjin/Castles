using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Battle {
	public class PathRequestManager : MonoBehaviour {
		//queue to store all incoming requests for paths
		Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
		//the current request we're processing
		PathRequest currentRequest;
		//reference to this manager to enforce singleton status
		private static PathRequestManager instance;
		//a check for whether we're currently handling a path
		bool isProcessingPath;
		//reference to the map class
		MapManager mapManager;

		/**
		 * If this oject already exists, return the existing instance of it, or if it's yet to be instantiated then create it
		 */
		public static PathRequestManager Instance {
			get { 
				return instance ?? (instance = new GameObject("PathRequestManager").AddComponent<PathRequestManager>());
			}
		}

		void Awake() {
			//save the map reference
			mapManager = GameObject.Find("BattleManager").GetComponent<BattleManager>().map;
		}

		public void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback) {
			PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
			instance.pathRequestQueue.Enqueue(newRequest);
			instance.TryProcessNext();
		}

		public void TryProcessNext() {
			if (!isProcessingPath && pathRequestQueue.Count > 0) {
				currentRequest = pathRequestQueue.Dequeue();
				isProcessingPath = true;
				mapManager.StartFindPath(currentRequest.pathStart, currentRequest.pathEnd);
			}
		}

		public void FinishedProcessingPath(Vector2[] path, bool success) {
			currentRequest.callback(path, success);
			isProcessingPath = false;
			TryProcessNext();
		}

		struct PathRequest {
			public Vector2 pathStart;
			public Vector2 pathEnd;
			public Action<Vector2[], bool> callback;

			public PathRequest(Vector2 _pathStart, Vector2 _pathEnd, Action<Vector2[], bool> _callback) {
				pathStart = _pathStart;
				pathEnd = _pathEnd;
				callback = _callback;
			}
		}
	}
}