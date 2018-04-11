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

        Tile[,] dungeon;
        if (level == null)
        {
            Dungeon d = new Dungeon(r, s => { Debug.Log("Logger: " + s); });
            d.CreateDungeon(width, height, things);

            dungeon = d.GetDungeonAs2D();
        }
        else
        {
            dungeon = Dungeon.FileToTileMap(level.text);
        }

        GenerateDungeon(dungeon);
        MovePlayerToRandomTile(r, dungeon);
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

    private void GenerateDungeon(Tile[,] dungeon)
    {
        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                switch (dungeon[i, j])
                {                    
                    case Tile.Floor:
                        Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity);
                        Instantiate(ceiling, new Vector3(i, 4, j), new Quaternion(45, 0, 0, 0));
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