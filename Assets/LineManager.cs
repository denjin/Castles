using UnityEngine;
using System.Collections;

public class LineManager : MonoBehaviour {
	LineRenderer line;
	
	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer>();
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		line.sortingLayerID = spriteRenderer.sortingLayerID;
		line.sortingOrder = spriteRenderer.sortingOrder;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
