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
		private float tileWidth = 0.64f;
		private float tileHeight = 0.32f;
		//how many different tiles do we have?
		private int numTiles = 4;
		//arrays to hold references to the tiles [makes conversion easier]
		private Int2[,] cartTiles;
		private Int2[,] isoTiles;
		
		private float seed;
		private float noise;

		public MapManager(Camera _mainCamera, int _levelWidth = 20, int _levelHeight = 20) {
			//initialise the camera
			mainCamera = _mainCamera;
			Tile.SetCamera(mainCamera);

			//setup map variables
			levelWidth = _levelWidth;
			levelHeight = _levelHeight;

			//add a buffer to the size of the level
			buffer = new Int2(levelWidth * 2, levelHeight * 2);
			//create the level
			Tile.NewLevel(new Int2(buffer.x, buffer.y), 3, new Vector2(tileWidth, tileHeight), 0, LayerLock.None);
			Tile.AddLayer(new Int2(buffer.x, buffer.y), 3, new Vector2(tileWidth, tileHeight), 0, LayerLock.None);
			//save the size of the map
			mapSize = Tile.GetMapSize();
			//initialise the tile arrays
			cartTiles = new Int2[levelWidth, levelHeight];
			isoTiles = new Int2[buffer.x, buffer.y];
			
			//setup the noise function
			seed = Random.value;
			
			//build the level
			for (int tY = 0; tY < levelHeight; tY++) {
				for (int tX = 0; tX < levelWidth; tX++) {
					//select a tile
					int tile1 = GetPerlinNoise(tX, tY, seed, 4);
					//position the tile
					Int2 position = new Int2(tX + tY, tX - tY + levelHeight);
					//set the tile
					Tile.SetTile(position, 0, 0, tile1);
					//store a reference to the tile [baseTile]
					cartTiles[tX, tY] = position;
					isoTiles[position.x, position.y] = new Int2(tX, tY);
					
					//add decorations
					if (tile1 == 0) {
						int dec = Random.Range(0, 4);
						Tile.SetTile(position, 1, 0, 8 + dec);
					}
				}
			}
			
			//set the cameras position to the first tiles position
			Int2 middle = new Int2(levelWidth / 2, levelHeight / 2);
			Vector3 tilePosition = Tile.MapToWorldPosition(CartTileToIsoTile(middle), 0);
			Vector3 camPosition = new Vector3(tilePosition.x, tilePosition.y, -10f);
			mainCamera.transform.position = camPosition;
			
			//set the draw order
			int order = 0;
			for (int y = mapSize.y - 1; y > -1; y--) {
				for (int x = mapSize.x - 1; x > -1; x--) {
					Tile.SetOrder(new Int2(x, y), order);
					order++;
				}
			}
		}

		public Int2 GetSize() {
			return new Int2(levelWidth, levelHeight);
		}
		
		/**
		 * Convert coordinates of a tile in tiles[,] to one used by SpriteTile
		 * @param	Int2	tile	the starting tile
		 */
		public Int2 CartTileToIsoTile(Int2 tile) {
			return cartTiles[tile.x, tile.y];
		}
		
		/**
		 * Convert coordinates of a tile in SpriteTile to one in tiles[,]
		 * @param	Int2	tile	the starting tile
		 * @return	Int2			the referenced tile, returns (-1, -1) if no tile
		 **/
		public Int2 IsoTileToCartTile(Int2 tile) {
			if (Tile.GetTile(tile) != TileInfo.empty) {
				return isoTiles[tile.x, tile.y];
			} else {
				return new Int2(-1, -1);
			}
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
