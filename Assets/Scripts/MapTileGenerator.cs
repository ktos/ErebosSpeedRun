using System;
using UnityEngine;

public class MapTileGenerator : MonoBehaviour
{
    public Transform floor;
    public Transform ceiling;
    public Transform wall;
    public Transform door;
    public Transform upstairs;
    public Transform downstairs;
    public Transform corridor;
    public GameObject player;
    public TextAsset level;
    public GameObject sceneSwitcher;

    public int width;
    public int height;
    public int things;
    public int seed = -1;

    private void Start()
    {
        var r = new BasicRandom(seed);

        TileType[,] tiles;
        if (level == null)
        {
            Dungeon d = new Dungeon(r, s => { Debug.Log("Dungeon: " + s); });
            d.CreateDungeon(width, height, things);

            tiles = d.GetDungeonAs2D();
        }
        else
        {
            tiles = Dungeon.FileToTileMap(level.text);
        }

        CreateLevel(tiles);
        MovePlayerToRandomTile(r, tiles);
    }

    private void MovePlayerToRandomTile(BasicRandom r, TileType[,] dungeon)
    {
        var randomTile = dungeon[0, 0];
        int x = 0, y = 0;
        while (randomTile != TileType.Floor)
        {
            x = r.Next(0, dungeon.GetLength(0));
            y = r.Next(0, dungeon.GetLength(1));
            randomTile = dungeon[x, y];
        }

        player.transform.position = new Vector3(x, 1, y);
    }

    private void CreateLevel(TileType[,] tiles)
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                switch (tiles[i, j])
                {
                    case TileType.Floor:
                        Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity);
                        Instantiate(ceiling, new Vector3(i, 4, j), Quaternion.Euler(-180f, 0, 0));
                        break;

                    case TileType.Wall:
                        var orientation = GetTileOrientation(tiles, i, j);
                        if (orientation == Direction.Unknown) orientation = Direction.North;

                        int rotate = 0;
                        switch (orientation)
                        {
                            case Direction.North:
                                rotate = 180;
                                break;
                            case Direction.East:
                                rotate = 270;
                                break;
                            case Direction.South:
                                rotate = 0;
                                break;
                            case Direction.West:
                                rotate = 90;
                                break;
                            case Direction.Unknown:
                                break;
                        }

                        Instantiate(wall, new Vector3(i, 2, j), Quaternion.Euler(0, rotate, 0));                        
                        break;

                    case TileType.Door:
                        orientation = GetTileOrientation(tiles, i, j);

                        Debug.Log(orientation);

                        int rotate2 = 0;
                        if (orientation == Direction.South)
                        {
                            tiles[i + 1, j] = TileType.Unused;
                        }
                        else if (orientation == Direction.East)
                        {
                            tiles[i, j + 1] = TileType.Unused;
                            rotate2 = 90;
                        }

                        float fixX = orientation == Direction.South ? 0.5f : 0;
                        float fixZ = orientation == Direction.East ? 0.5f : 0;

                        var createdDoor = Instantiate(door, new Vector3(i + fixX, 0, j + fixZ), Quaternion.Euler(0, rotate2, 0));
                        createdDoor.GetComponent<LoadNextScene>().sceneSwitcher = sceneSwitcher;
                        
                        //Instantiate(ceiling, new Vector3(i, 4, j), Quaternion.Euler(-180f, 0, 0));
                        break;

                    default:
                        break;
                }
            }
        }
    }

    private Direction GetTileOrientation(TileType[,] tiles, int x, int y)
    {
        int maxx = tiles.GetLength(0);
        int maxy = tiles.GetLength(1);

        if (x > maxx || x < 0 || y > maxy || y < 0)
            throw new ArgumentOutOfRangeException();

        if (tiles[x, y] == TileType.Floor || tiles[x, y] == TileType.Unused)
            throw new ArgumentException("Floor or empty tiles are not oriented.");

        if (tiles[x, y] == TileType.Wall)
        {
            if (x + 1 <= maxx - 1 && tiles[x + 1, y] == TileType.Floor) return Direction.South;
            if (x - 1 >= 0 && tiles[x - 1, y] == TileType.Floor) return Direction.North;
            if (y + 1 <= maxy - 1 && tiles[x, y + 1] == TileType.Floor) return Direction.East;
            if (y - 1 >= 0 && tiles[x, y - 1] == TileType.Floor) return Direction.West;

            return Direction.Unknown;
        }

        if (tiles[x, y] == TileType.Door)
        {
            if (x + 1 <= maxx - 1 && tiles[x + 1, y] == TileType.Door) return Direction.South;            
            if (y + 1 <= maxy - 1 && tiles[x, y + 1] == TileType.Door) return Direction.East;            

            return Direction.Unknown;
        }

        throw new Exception("Unknown orientation: " + x + "," + y);
    }
}