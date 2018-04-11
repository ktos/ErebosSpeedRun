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

    public int width;
    public int height;
    public int things;
    public int seed = -1;

    private void Start()
    {
        var r = new BasicRandom(seed);

        Tile[,] tiles;
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

    private void MovePlayerToRandomTile(BasicRandom r, Tile[,] dungeon)
    {
        var randomTile = dungeon[0, 0];
        int x = 0, y = 0;
        while (randomTile != Tile.Floor)
        {
            x = r.Next(0, dungeon.GetLength(0));
            y = r.Next(0, dungeon.GetLength(1));
            randomTile = dungeon[x, y];
        }

        player.transform.position = new Vector3(x, 1, y);
    }

    private void CreateLevel(Tile[,] tiles)
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                switch (tiles[i, j])
                {                    
                    case Tile.Floor:
                        Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity);
                        Instantiate(ceiling, new Vector3(i, 4, j), Quaternion.Euler(-180f, 0, 0));
                        break;

                    case Tile.Wall:
                        Instantiate(wall, new Vector3(i, 1, j), Quaternion.identity);
                        break;

                    case Tile.Door:
                        Instantiate(door, new Vector3(i, 0, j), Quaternion.identity);
                        Instantiate(ceiling, new Vector3(i, 4, j), new Quaternion(45, 0, 0, 0));
                        break;

                    default:
                        break;
                }
            }
        }
    }
}