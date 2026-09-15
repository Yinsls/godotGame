using Godot;
using System;

public partial class World : Node2D
{
    public const int TileSize = 16;
    public const int MapWidth = 80;
    public const int MapHeight = 48;
    public static readonly Rect2 WorldRect = new(0, 0, MapWidth * TileSize, MapHeight * TileSize);

    private readonly Random _random = new();
    private Node2D _groundLayer = null!;
    private Node2D _grassLayer = null!;
    private Node2D _decorationLayer = null!;
    private Node2D _collisionLayer = null!;
    private Player _player = null!;

    public override void _Ready()
    {
        _groundLayer = new Node2D { Name = "GroundLayer", ZIndex = 0 };
        _grassLayer = new Node2D { Name = "GrassLayer", ZIndex = 2 };
        _decorationLayer = new Node2D { Name = "DecorationLayer", ZIndex = 3 };
        _collisionLayer = new Node2D { Name = "CollisionLayer", ZIndex = 4 };
        AddChild(_groundLayer);
        AddChild(_grassLayer);
        AddChild(_decorationLayer);
        AddChild(_collisionLayer);

        DrawGround();
        CreatePathsAndObstacles();
        CreateGrassPatches();
        CreatePlayer();
    }

    private void DrawGround()
    {
        var ground = new GroundVisual { Name = "Ground" };
        _groundLayer.AddChild(ground);
    }

    private void CreatePathsAndObstacles()
    {
        // A simple winding path gives the field a readable RPG-map silhouette.
        for (int y = 4; y < MapHeight - 4; y++)
        {
            int x = 8 + (int)(Math.Sin(y * 0.28) * 5);
            for (int i = 0; i < 5; i++)
            {
                var tile = new PathTile { Position = new Vector2((x + i) * TileSize, y * TileSize) };
                _groundLayer.AddChild(tile);
            }
        }

        // Trees/rocks are deliberately positioned away from the starting point.
        for (int i = 0; i < 26; i++)
        {
            Vector2 pos;
            do
            {
                pos = new Vector2(_random.Next(2, MapWidth - 2) * TileSize + 8, _random.Next(2, MapHeight - 2) * TileSize + 8);
            } while (pos.DistanceTo(new Vector2(40 * TileSize, 24 * TileSize)) < 110f);

            if (i % 4 == 0)
            {
                var rock = new Rock { Position = pos };
                _decorationLayer.AddChild(rock);
                AddCollision(pos, new Vector2(11, 9));
            }
            else
            {
                var tree = new Tree { Position = pos };
                _decorationLayer.AddChild(tree);
                AddCollision(pos + new Vector2(0, 9), new Vector2(11, 7));
            }
        }
    }

    private void CreateGrassPatches()
    {
        // Dense patches rather than isolated blades: this is the foundation for encounter grass later.
        var patches = new (int x, int y, int w, int h)[]
        {
            (20, 6, 14, 10), (45, 5, 17, 12), (58, 26, 14, 13), (15, 30, 18, 9), (39, 35, 13, 8)
        };

        foreach (var patch in patches)
        {
            for (int y = 0; y < patch.h; y++)
            for (int x = 0; x < patch.w; x++)
            {
                var grass = new TallGrass
                {
                    Position = new Vector2((patch.x + x) * TileSize + 8, (patch.y + y) * TileSize + 11)
                };
                _grassLayer.AddChild(grass);
            }
        }
    }

    private void CreatePlayer()
    {
        _player = new Player { Name = "Player", Position = new Vector2(40 * TileSize, 24 * TileSize) };
        AddChild(_player);
        _player.MoveAndSlide();
    }

    private void AddCollision(Vector2 position, Vector2 size)
    {
        var body = new StaticBody2D { Position = position };
        var shape = new CollisionShape2D { Shape = new RectangleShape2D { Size = size * 2f } };
        body.AddChild(shape);
        _collisionLayer.AddChild(body);
    }

    public partial class GroundVisual : Node2D
    {
        public override void _Ready() => QueueRedraw();

        public override void _Draw()
        {
            DrawRect(World.WorldRect, new Color("#78ad50"));
            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                var shade = ((x * 17 + y * 31) % 5 == 0) ? new Color("#72a64b") : new Color("#7db557");
                DrawRect(new Rect2(x * TileSize, y * TileSize, TileSize, TileSize), shade);
            }
        }
    }

    public partial class PathTile : Node2D
    {
        public override void _Ready() => QueueRedraw();
        public override void _Draw() => DrawRect(new Rect2(-8, -8, 16, 16), new Color("#c7a968"));
    }

    public partial class Tree : Node2D
    {
        public override void _Ready() => QueueRedraw();
        public override void _Draw()
        {
            DrawRect(new Rect2(-3, 0, 6, 13), new Color("#765132"));
            DrawCircle(new Vector2(0, -5), 14, new Color("#2f6d3b"));
            DrawCircle(new Vector2(-7, -2), 9, new Color("#397a42"));
            DrawCircle(new Vector2(7, -2), 9, new Color("#397a42"));
            DrawCircle(new Vector2(0, -11), 9, new Color("#438647"));
        }
    }

    public partial class Rock : Node2D
    {
        public override void _Ready() => QueueRedraw();
        public override void _Draw()
        {
            var points = new[] { new Vector2(-10, 7), new Vector2(-7, -5), new Vector2(1, -9), new Vector2(10, -3), new Vector2(8, 7) };
            DrawColoredPolygon(points, new Color("#777b72"));
            DrawLine(new Vector2(-5, -3), new Vector2(2, -6), new Color("#a5a79b"), 2);
        }
    }
}
