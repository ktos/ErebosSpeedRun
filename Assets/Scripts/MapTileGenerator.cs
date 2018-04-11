// original version: http://www.roguebasin.com/index.php?title=C-Sharp_Example_of_Dungeon-Building_Algorithm

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Use this for initialization
    void Start()
    {
        var r = new BasicRandom();
        //Dungeon d = new Dungeon(r, s => { Debug.Log("Logger: " + s); });
        //d.CreateDungeon(width, height, things);        

        //var dungeon = d.GetDungeonAs2D();

        var dungeon = Dungeon.FileToTileMap(level.text);

        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                switch (dungeon[i, j])
                {
                    case Tile.Corridor:
                        Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity);
                        Instantiate(ceiling, new Vector3(i, 4, j), new Quaternion(45, 0, 0, 0));
                        break;

                    case Tile.DirtFloor:
                        Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity);
                        Instantiate(ceiling, new Vector3(i, 4, j), new Quaternion(45, 0, 0, 0));
                        break;

                    case Tile.DirtWall:
                        Instantiate(wall, new Vector3(i, 1, j), Quaternion.identity);
                        break;

                    case Tile.StoneWall:
                        Instantiate(wall, new Vector3(i, 1, j), Quaternion.identity);
                        break;

                    case Tile.Door:
                        Instantiate(door, new Vector3(i, 0, j), Quaternion.identity);
                        Instantiate(ceiling, new Vector3(i, 4, j), new Quaternion(45, 0, 0, 0));
                        break;

                    //case Tile.Upstairs:
                    //    Instantiate(upstairs, new Vector3(i, 0, j), Quaternion.identity);
                    //    break;

                    //case Tile.Downstairs:
                    //    Instantiate(downstairs, new Vector3(i, 0, j), Quaternion.identity);
                    //    break;

                    //case Tile.Chest:
                    //    Instantiate(chest, new Vector3(i, 0, j), Quaternion.identity);
                    //    break;

                    default:
                        break;
                }

            }
        }

        var randomTile = dungeon[0, 0];
        int x = 0, y = 0;
        while (randomTile != Tile.DirtFloor)
        {
            x = r.Next(0, dungeon.GetLength(0));
            y = r.Next(0, dungeon.GetLength(1));
            randomTile = dungeon[x, y];
        }

        player.transform.position = new Vector3(x, 1, y);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public enum Tile
{
    Corridor,
    Unused,
    DirtFloor,
    DirtWall,
    StoneWall,
    Door,
    Upstairs,
    Downstairs,
    Chest
}

public enum Direction
{
    North, East, West, South
}

public class Dungeon
{

    // misc. messages to print
    const string MsgXSize = "X size of dungeon: \t";

    const string MsgYSize = "Y size of dungeon: \t";

    const string MsgMaxObjects = "max # of objects: \t";

    const string MsgNumObjects = "# of objects made: \t";

    // max size of the map
    int xmax = 500; //columns
    int ymax = 500; //rows

    // size of the map
    int _xsize;
    int _ysize;

    // number of "objects" to generate on the map
    int _objects;

    // define the %chance to generate either a room or a corridor on the map
    // BTW, rooms are 1st priority so actually it's enough to just define the chance
    // of generating a room
    const int ChanceRoom = 75;

    // our map
    Tile[] _dungeonMap = { };

    readonly IRandomize _rnd;

    readonly Action<string> _logger;

    public static Tile[,] FileToTileMap(string fileContents)
    {
        var lines = fileContents.Split('\n');
        var dim = lines[0].Split(',');

        var tileMap = new Tile[int.Parse(dim[0]), int.Parse(dim[1])];


        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                switch (lines[i + 1][j])
                {
                    case 'X':
                        tileMap[i, j] = Tile.StoneWall;
                        break;

                    case 'F':
                        tileMap[i, j] = Tile.DirtFloor;
                        break;

                    case ' ':
                        tileMap[i, j] = Tile.Unused;
                        break;
                }

                Debug.Log("i=" + i + " j=" + j);
            }
        }

        return tileMap;
    }

    public Dungeon(IRandomize rnd, Action<string> logger)
    {
        _rnd = rnd;
        _logger = logger;
    }

    public int Corridors
    {
        get;
        private set;
    }

    public static bool IsWall(int x, int y, int xlen, int ylen, int xt, int yt, Direction d)
    {
        Func<int, int, int> a = GetFeatureLowerBound;

        Func<int, int, int> b = IsFeatureWallBound;
        switch (d)
        {
            case Direction.North:
                return xt == a(x, xlen) || xt == b(x, xlen) || yt == y || yt == y - ylen + 1;
            case Direction.East:
                return xt == x || xt == x + xlen - 1 || yt == a(y, ylen) || yt == b(y, ylen);
            case Direction.South:
                return xt == a(x, xlen) || xt == b(x, xlen) || yt == y || yt == y + ylen - 1;
            case Direction.West:
                return xt == x || xt == x - xlen + 1 || yt == a(y, ylen) || yt == b(y, ylen);
        }

        throw new InvalidOperationException();
    }

    public static int GetFeatureLowerBound(int c, int len)
    {
        return c - len / 2;
    }

    public static int IsFeatureWallBound(int c, int len)
    {
        return c + (len - 1) / 2;
    }

    public static int GetFeatureUpperBound(int c, int len)
    {
        return c + (len + 1) / 2;
    }

    public static IEnumerable<PointI> GetRoomPoints(int x, int y, int xlen, int ylen, Direction d)
    {
        // north and south share the same x strategy
        // east and west share the same y strategy
        Func<int, int, int> a = GetFeatureLowerBound;
        Func<int, int, int> b = GetFeatureUpperBound;

        switch (d)
        {
            case Direction.North:
                for (var xt = a(x, xlen); xt < b(x, xlen); xt++) for (var yt = y; yt > y - ylen; yt--) yield return new PointI { X = xt, Y = yt };
                break;
            case Direction.East:
                for (var xt = x; xt < x + xlen; xt++) for (var yt = a(y, ylen); yt < b(y, ylen); yt++) yield return new PointI { X = xt, Y = yt };
                break;
            case Direction.South:
                for (var xt = a(x, xlen); xt < b(x, xlen); xt++) for (var yt = y; yt < y + ylen; yt++) yield return new PointI { X = xt, Y = yt };
                break;
            case Direction.West:
                for (var xt = x; xt > x - xlen; xt--) for (var yt = a(y, ylen); yt < b(y, ylen); yt++) yield return new PointI { X = xt, Y = yt };
                break;
            default:
                yield break;
        }
    }

    public Tile GetCellType(int x, int y)
    {
        try
        {
            return this._dungeonMap[x + this._xsize * y];
        }
        catch (IndexOutOfRangeException)
        {
            //new { x, y }.Dump("exceptional");
            throw;
        }
    }

    public int GetRand(int min, int max)
    {
        return _rnd.Next(min, max);
    }

    public bool MakeCorridor(int x, int y, int length, Direction direction)
    {
        // define the dimensions of the corridor (er.. only the width and height..)
        int len = this.GetRand(2, length);
        const Tile Floor = Tile.Corridor;

        int xtemp;
        int ytemp = 0;

        switch (direction)
        {
            case Direction.North:
                // north
                // check if there's enough space for the corridor
                // start with checking it's not out of the boundaries
                if (x < 0 || x > this._xsize) return false;
                xtemp = x;

                // same thing here, to make sure it's not out of the boundaries
                for (ytemp = y; ytemp > (y - len); ytemp--)
                {
                    if (ytemp < 0 || ytemp > this._ysize) return false; // oh boho, it was!
                    if (GetCellType(xtemp, ytemp) != Tile.Unused) return false;
                }

                // if we're still here, let's start building
                Corridors++;
                for (ytemp = y; ytemp > (y - len); ytemp--)
                {
                    this.SetCell(xtemp, ytemp, Floor);
                }

                break;

            case Direction.East:
                // east
                if (y < 0 || y > this._ysize) return false;
                ytemp = y;

                for (xtemp = x; xtemp < (x + len); xtemp++)
                {
                    if (xtemp < 0 || xtemp > this._xsize) return false;
                    if (GetCellType(xtemp, ytemp) != Tile.Unused) return false;
                }

                Corridors++;
                for (xtemp = x; xtemp < (x + len); xtemp++)
                {
                    this.SetCell(xtemp, ytemp, Floor);
                }

                break;

            case Direction.South:
                // south
                if (x < 0 || x > this._xsize) return false;
                xtemp = x;

                for (ytemp = y; ytemp < (y + len); ytemp++)
                {
                    if (ytemp < 0 || ytemp > this._ysize) return false;
                    if (GetCellType(xtemp, ytemp) != Tile.Unused) return false;
                }

                Corridors++;
                for (ytemp = y; ytemp < (y + len); ytemp++)
                {
                    this.SetCell(xtemp, ytemp, Floor);
                }

                break;
            case Direction.West:
                // west
                if (ytemp < 0 || ytemp > this._ysize) return false;
                ytemp = y;

                for (xtemp = x; xtemp > (x - len); xtemp--)
                {
                    if (xtemp < 0 || xtemp > this._xsize) return false;
                    if (GetCellType(xtemp, ytemp) != Tile.Unused) return false;
                }

                Corridors++;
                for (xtemp = x; xtemp > (x - len); xtemp--)
                {
                    this.SetCell(xtemp, ytemp, Floor);
                }

                break;
        }

        // woot, we're still here! let's tell the other guys we're done!!
        return true;
    }

    public IEnumerable<Tuple<PointI, Direction>> GetSurroundingPoints(PointI v)
    {
        var points = new[]
                         {
                                 Tuple.Create(new PointI { X = v.X, Y = v.Y + 1 }, Direction.North),
                                 Tuple.Create(new PointI { X = v.X - 1, Y = v.Y }, Direction.East),
                                 Tuple.Create(new PointI { X = v.X , Y = v.Y-1 }, Direction.South),
                                 Tuple.Create(new PointI { X = v.X +1, Y = v.Y  }, Direction.West),

                             };
        return points.Where(p => InBounds(p.Item1));
    }

    public IEnumerable<Tuple<PointI, Direction, Tile>> GetSurroundings(PointI v)
    {
        return
            this.GetSurroundingPoints(v)
                .Select(r => Tuple.Create(r.Item1, r.Item2, this.GetCellType(r.Item1.X, r.Item1.Y)));
    }

    public bool InBounds(int x, int y)
    {
        return x > 0 && x < this.xmax && y > 0 && y < this.ymax;
    }

    public bool InBounds(PointI v)
    {
        return this.InBounds(v.X, v.Y);
    }

    public bool MakeRoom(int x, int y, int xlength, int ylength, Direction direction)
    {
        // define the dimensions of the room, it should be at least 4x4 tiles (2x2 for walking on, the rest is walls)
        int xlen = this.GetRand(4, xlength);
        int ylen = this.GetRand(4, ylength);

        // the tile type it's going to be filled with
        const Tile Floor = Tile.DirtFloor;

        const Tile Wall = Tile.DirtWall;
        // choose the way it's pointing at

        var points = GetRoomPoints(x, y, xlen, ylen, direction).ToArray();

        // Check if there's enough space left for it
        if (
            points.Any(
                s =>
                s.Y < 0 || s.Y > this._ysize || s.X < 0 || s.X > this._xsize || this.GetCellType(s.X, s.Y) != Tile.Unused)) return false;
        _logger(
                  string.Format(
                      "Making room:int x={0}, int y={1}, int xlength={2}, int ylength={3}, int direction={4}",
                      x,
                      y,
                      xlength,
                      ylength,
                      direction));

        foreach (var p in points)
        {
            this.SetCell(p.X, p.Y, IsWall(x, y, xlen, ylen, p.X, p.Y, direction) ? Wall : Floor);
        }

        // yay, all done
        return true;
    }

    public Tile[] GetDungeon()
    {
        return this._dungeonMap;
    }

    public char GetCellTile(int x, int y)
    {
        switch (GetCellType(x, y))
        {
            case Tile.Unused:
                return char.MinValue;
            case Tile.DirtWall:
                return 'X';
            case Tile.DirtFloor:
                return '_';
            case Tile.StoneWall:
                return 'S';
            case Tile.Corridor:
                return '#';
            case Tile.Door:
                return 'D';
            case Tile.Upstairs:
                return '+';
            case Tile.Downstairs:
                return '-';
            case Tile.Chest:
                return 'C';
            default:
                throw new ArgumentOutOfRangeException("x,y");
        }
    }

    //used to print the map on the screen
    public void ShowDungeon()
    {
        for (int y = 0; y < this._ysize; y++)
        {
            for (int x = 0; x < this._xsize; x++)
            {
                Console.Write(GetCellTile(x, y));
            }

            if (this._xsize <= xmax) Console.WriteLine();
        }
    }

    public Tile[,] GetDungeonAs2D()
    {
        Tile[,] tiles = new Tile[_xsize, _ysize];

        for (int y = 0; y < this._ysize; y++)
        {
            for (int x = 0; x < this._xsize; x++)
            {
                tiles[x, y] = GetCellType(x, y);
            }

            //if (this._xsize <= xmax) Console.WriteLine();
        }

        return tiles;
    }

    public Direction RandomDirection()
    {
        int dir = this.GetRand(0, 4);
        switch (dir)
        {
            case 0:
                return Direction.North;
            case 1:
                return Direction.East;
            case 2:
                return Direction.South;
            case 3:
                return Direction.West;
            default:
                throw new InvalidOperationException();
        }
    }

    //and here's the one generating the whole map
    public bool CreateDungeon(int inx, int iny, int inobj)
    {
        this._objects = inobj < 1 ? 10 : inobj;

        // adjust the size of the map, if it's smaller or bigger than the limits
        if (inx < 3) this._xsize = 3;
        else if (inx > xmax) this._xsize = xmax;
        else this._xsize = inx;

        if (iny < 3) this._ysize = 3;
        else if (iny > ymax) this._ysize = ymax;
        else this._ysize = iny;

        Console.WriteLine(MsgXSize + this._xsize);
        Console.WriteLine(MsgYSize + this._ysize);
        Console.WriteLine(MsgMaxObjects + this._objects);

        // redefine the map var, so it's adjusted to our new map size
        this._dungeonMap = new Tile[this._xsize * this._ysize];

        // start with making the "standard stuff" on the map
        this.Initialize();

        /*******************************************************************************
        And now the code of the random-map-generation-algorithm begins!
        *******************************************************************************/

        // start with making a room in the middle, which we can start building upon
        this.MakeRoom(this._xsize / 2, this._ysize / 2, 20, 16, RandomDirection()); // getrand saken f????r att slumpa fram riktning p?? rummet

        // keep count of the number of "objects" we've made
        int currentFeatures = 1; // +1 for the first room we just made

        // then we sart the main loop
        for (int countingTries = 0; countingTries < 1000; countingTries++)
        {
            // check if we've reached our quota
            if (currentFeatures == this._objects)
            {
                break;
            }

            // start with a random wall
            int newx = 0;
            int xmod = 0;
            int newy = 0;
            int ymod = 0;
            Direction? validTile = null;

            // 1000 chances to find a suitable object (room or corridor)..
            for (int testing = 0; testing < 1000; testing++)
            {
                newx = this.GetRand(1, this._xsize - 1);
                newy = this.GetRand(1, this._ysize - 1);

                if (GetCellType(newx, newy) == Tile.DirtWall || GetCellType(newx, newy) == Tile.Corridor)
                {
                    var surroundings = this.GetSurroundings(new PointI() { X = newx, Y = newy });

                    // check if we can reach the place
                    var canReach =
                        surroundings.FirstOrDefault(s => s.Item3 == Tile.Corridor || s.Item3 == Tile.DirtFloor);
                    if (canReach == null)
                    {
                        continue;
                    }
                    validTile = canReach.Item2;
                    switch (canReach.Item2)
                    {
                        case Direction.North:
                            xmod = 0;
                            ymod = -1;
                            break;
                        case Direction.East:
                            xmod = 1;
                            ymod = 0;
                            break;
                        case Direction.South:
                            xmod = 0;
                            ymod = 1;
                            break;
                        case Direction.West:
                            xmod = -1;
                            ymod = 0;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }


                    // check that we haven't got another door nearby, so we won't get alot of openings besides
                    // each other

                    if (GetCellType(newx, newy + 1) == Tile.Door) // north
                    {
                        validTile = null;

                    }

                    else if (GetCellType(newx - 1, newy) == Tile.Door) // east
                        validTile = null;
                    else if (GetCellType(newx, newy - 1) == Tile.Door) // south
                        validTile = null;
                    else if (GetCellType(newx + 1, newy) == Tile.Door) // west
                        validTile = null;


                    // if we can, jump out of the loop and continue with the rest
                    if (validTile.HasValue) break;
                }
            }

            if (validTile.HasValue)
            {
                // choose what to build now at our newly found place, and at what direction
                int feature = this.GetRand(0, 100);
                if (feature <= ChanceRoom)
                { // a new room
                    if (this.MakeRoom(newx + xmod, newy + ymod, 17, 12, validTile.Value))
                    {
                        currentFeatures++; // add to our quota

                        // then we mark the wall opening with a door
                        this.SetCell(newx, newy, Tile.Door);

                        // clean up infront of the door so we can reach it
                        this.SetCell(newx + xmod, newy + ymod, Tile.DirtFloor);
                    }
                }
                //else if (feature >= ChanceRoom)
                //{ // new corridor
                //    if (this.MakeCorridor(newx + xmod, newy + ymod, 17, validTile.Value))
                //    {
                //        // same thing here, add to the quota and a door
                //        currentFeatures++;

                //        this.SetCell(newx, newy, Tile.Door);
                //    }
                //}
            }
        }

        /*******************************************************************************
        All done with the building, let's finish this one off
        *******************************************************************************/
        //AddSprinkles();

        // all done with the map generation, tell the user about it and finish
        Console.WriteLine(MsgNumObjects + currentFeatures);

        return true;
    }

    void Initialize()
    {
        for (int y = 0; y < this._ysize; y++)
        {
            for (int x = 0; x < this._xsize; x++)
            {
                //// ie, making the borders of unwalkable walls
                //if (y == 0 || y == this._ysize - 1 || x == 0 || x == this._xsize - 1)
                //{
                //    this.SetCell(x, y, Tile.StoneWall);
                //}
                //else
                //{                        // and fill the rest with dirt
                //    this.SetCell(x, y, Tile.Unused);
                //}
                this.SetCell(x, y, Tile.Unused);
            }
        }
    }

    // setting a tile's type
    void SetCell(int x, int y, Tile celltype)
    {
        this._dungeonMap[x + this._xsize * y] = celltype;
    }


}

internal class Tuple
{
    internal static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
    {
        return new Tuple<T1, T2>() { Item1 = item1, Item2 = item2 };
    }

    internal static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
    {
        return new Tuple<T1, T2, T3>() { Item1 = item1, Item2 = item2, Item3 = item3 };
    }
}

public class PointI
{
    public int X { get; internal set; }
    public int Y { get; internal set; }
}

public interface IRandomize
{
    int Next(int min, int max);
}

public class BasicRandom : IRandomize
{
    readonly System.Random rnd;

    public BasicRandom()
    {
        rnd = new System.Random();
    }

    public int Next(int min, int max)
    {
        return rnd.Next(min, max);
    }
}

public class Tuple<T1, T2>
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
}

public class Tuple<T1, T2, T3>
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
    public T3 Item3 { get; set; }
}