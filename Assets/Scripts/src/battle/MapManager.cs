using UnityEngine;
using System.Collections;
using SpriteTile;

namespace Battle {

public class MapManager {
		private Camera mainCamera;
		//base size of the map
		private int levelWidth;
		private int levelHeight;
		//amount to add to the map to get it to fit properly into the iso map
		private Int2 buffer;
		//height / width of the generated iso map
		private Int2 mapSize;
		//the size of each tile
		private float tileSize = 0.16f;
		//how many different tiles do we have?
		private int numTiles = 4;
		
		private float seed;
		private float noise;

		public MapManager(Camera _mainCamera, int _levelWidth = 100, int _levelHeight = 100) {
			//initialise the camera
			mainCamera = _mainCamera;
			Tile.SetCamera(mainCamera);

			//setup map variables
			levelWidth = _levelWidth;
			levelHeight = _levelHeight;

			//add a buffer to the size of the level
			buffer = new Int2(levelWidth * 2, levelHeight * 2);
			//create the level
			Tile.NewLevel(new Int2(buffer.x, buffer.y), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
			Tile.AddLayer(new Int2(buffer.x, buffer.y), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
			//save the size of the map
			mapSize = Tile.GetMapSize();
			
			//setup the noise function
			seed = Random.value;
			
			//build the level
			for (int tY = 0; tY < levelHeight; tY++) {
				for (int tX = 0; tX < levelWidth; tX++) {
					//select a tile
					int tile1 = 1;
					//position the tile
					Int2 position = new Int2(tX, tY);
					//set the tile
					Tile.SetTile(position, 0, 0, tile1);
				}
			}
			
			//set the cameras position to the first tiles position
			Int2 middle = new Int2(levelWidth / 2, levelHeight / 2);
			Vector3 tilePosition = Tile.MapToWorldPosition(middle, 0);
			Vector3 camPosition = new Vector3(tilePosition.x, tilePosition.y, -10f);
			mainCamera.transform.position = camPosition;
		}

		public Int2 GetSize() {
			return new Int2(levelWidth, levelHeight);
		}
		

		public Vector3 TileToWorld(Int2 _tile) {
			return Tile.MapToWorldPosition(_tile);
			
		}
		
		private int GetPerlinNoise(int x, int y, float _seed, int _numTiles) {
			float noiseVal = Mathf.PerlinNoise(_seed + x, _seed + y) * (_numTiles + 1);
			int _int = (int)Mathf.Floor(noiseVal);
			if (_int < 0) {
				_int = 0;
			}
			return _int;
		}
		
		
	}
}
