using Godot;
using System;

public partial class World : Node2D
{
    public const int TileSize = 16;
    public const int MapWidth = 80;
    public const int MapHeight = 48;
    public const int WorldWidth = MapWidth * TileSize;
    public const int WorldHeight = MapHeight * TileSize;

    private readonly Random _random = new(20260915);
    private Player _player = null!;
    private readonly Vector2[] _trees = new Vector2[18];
    private readonly Vector2[] _rocks = new Vector2[8];

    public override void _Ready()
    {
        ZIndex = 0;
        BuildDecorations();
        BuildCollisions();
        _player = new Player { Name = "Player", Position = new Vector2(40 * TileSize, 24 * TileSize) };
        AddChild(_player);
        QueueRedraw();
    }

    private void BuildDecorations()
    {
        for (int i = 0; i < _trees.Length; i++)
            _trees[i] = RandomMapPosition(70);

        for (int i = 0; i < _rocks.Length; i++)
            _rocks[i] = RandomMapPosition(50);
    }

    private Vector2 RandomMapPosition(int safeRadius)
    {
        Vector2 p;
        Vector2 center = new(40 * TileSize, 24 * TileSize);
        do
        {
            p = new Vector2(_random.Next(3, MapWidth - 3) * TileSize + 8, _random.Next(3, MapHeight - 3) * TileSize + 8);
        } while (p.DistanceTo(center) < safeRadius);
        return p;
    }

    private void BuildCollisions()
    {
        foreach (Vector2 p in _trees)
            AddObstacle(p + new Vector2(0, 8), new Vector2(9, 6));

        foreach (Vector2 p in _rocks)
            AddObstacle(p, new Vector2(7, 5));
    }

    private void AddObstacle(Vector2 position, Vector2 halfSize)
    {
        var body = new StaticBody2D { Position = position };
        body.AddChild(new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = halfSize * 2f }
        });
        AddChild(body);
    }

    public override void _Draw()
    {
        // Grass base.
        DrawRect(new Rect2(0, 0, WorldWidth, WorldHeight), new Color("#78ad50"));

        // Subtle 16x16 tile texture.
        for (int y = 0; y < MapHeight; y++)
        for (int x = 0; x < MapWidth; x++)
        {
            if ((x * 17 + y * 31) % 7 == 0)
                DrawRect(new Rect2(x * TileSize + 3, y * TileSize + 5, 2, 2), new Color("#6b9f49"));
        }

        DrawPath();
        DrawTallGrass();

        foreach (Vector2 p in _rocks)
            DrawRock(p);

        foreach (Vector2 p in _trees)
            DrawTree(p);
    }

    private void DrawPath()
    {
        for (int y = 0; y < MapHeight; y++)
        {
            int x = 9 + (int)Math.Round(Math.Sin(y * 0.24) * 4);
            for (int i = 0; i < 5; i++)
            {
                Rect2 r = new Rect2((x + i) * TileSize, y * TileSize, TileSize, TileSize);
                DrawRect(r, new Color("#c6a46a"));
                DrawRect(new Rect2(r.Position + new Vector2(3, 4), new Vector2(2, 2)), new Color("#b18e5b"));
            }
        }
    }

    private void DrawTallGrass()
    {
        var patches = new (int x, int y, int w, int h)[]
        {
            (20, 6, 14, 10), (45, 5, 17, 12), (58, 27, 14, 13), (15, 30, 18, 9), (39, 35, 13, 8)
        };

        foreach (var patch in patches)
        for (int y = 0; y < patch.h; y++)
        for (int x = 0; x < patch.w; x++)
        {
            float px = (patch.x + x) * TileSize + 8;
            float py = (patch.y + y) * TileSize + 13;
            DrawLine(new Vector2(px - 5, py), new Vector2(px - 2, py - 10), new Color("#397b38"), 2);
            DrawLine(new Vector2(px, py), new Vector2(px + 1, py - 13), new Color("#579746"), 2);
            DrawLine(new Vector2(px + 5, py), new Vector2(px + 3, py - 9), new Color("#2f6e32"), 2);
        }
    }

    private void DrawTree(Vector2 p)
    {
        // Trunk.
        DrawRect(new Rect2(p.X - 4, p.Y + 2, 8, 16), new Color("#76502f"));
        // Layered canopy for a simple pixel-RPG silhouette.
        DrawCircle(p + new Vector2(0, -8), 17, new Color("#285f35"));
        DrawCircle(p + new Vector2(-10, -3), 11, new Color("#347540"));
        DrawCircle(p + new Vector2(10, -3), 11, new Color("#347540"));
        DrawCircle(p + new Vector2(0, -16), 11, new Color("#40834a"));
        DrawRect(new Rect2(p.X - 10, p.Y - 5, 20, 4), new Color("#4c8d4c"));
    }

    private void DrawRock(Vector2 p)
    {
        var points = new[]
        {
            p + new Vector2(-10, 6), p + new Vector2(-7, -5), p + new Vector2(0, -9),
            p + new Vector2(9, -3), p + new Vector2(8, 6), p + new Vector2(0, 9)
        };
        DrawColoredPolygon(points, new Color("#777c74"));
        DrawLine(p + new Vector2(-5, -3), p + new Vector2(2, -6), new Color("#a8aaa1"), 2);
    }
}