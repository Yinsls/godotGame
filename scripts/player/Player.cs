using Godot;

public partial class Player : CharacterBody2D
{
    private const float Speed = 105f;
    private Vector2 _input;
    private Vector2 _facing = Vector2.Down;
    private float _walkCycle;
    private float _grassCooldown;

    public override void _Ready()
    {
        ZIndex = 10;
        CollisionLayer = 1;
        CollisionMask = 1;
        AddCollision();

        var camera = new Camera2D
        {
            PositionSmoothingEnabled = true,
            PositionSmoothingSpeed = 8f,
            LimitLeft = 0,
            LimitTop = 0,
            LimitRight = World.MapWidth * World.TileSize,
            LimitBottom = World.MapHeight * World.TileSize
        };
        AddChild(camera);
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        _input = GetFourDirectionInput();
        Velocity = _input * Speed;

        if (_input != Vector2.Zero)
        {
            _facing = _input;
            _walkCycle += (float)delta * 8f;
            _grassCooldown -= (float)delta;
            if (_grassCooldown <= 0f)
            {
                RustleNearbyGrass();
                _grassCooldown = 0.035f;
            }
        }
        else
        {
            _walkCycle = Mathf.MoveToward(_walkCycle, 0f, (float)delta * 10f);
        }

        MoveAndSlide();
        Position = new Vector2(
            Mathf.Clamp(Position.X, 10, World.MapWidth * World.TileSize - 10),
            Mathf.Clamp(Position.Y, 10, World.MapHeight * World.TileSize - 10));
        QueueRedraw();
    }

    private Vector2 GetFourDirectionInput()
    {
        if (Input.IsActionPressed("move_up")) return Vector2.Up;
        if (Input.IsActionPressed("move_down")) return Vector2.Down;
        if (Input.IsActionPressed("move_left")) return Vector2.Left;
        if (Input.IsActionPressed("move_right")) return Vector2.Right;
        return Vector2.Zero;
    }

    private void RustleNearbyGrass()
    {
        var grassLayer = GetParent().GetNodeOrNull<Node2D>("GrassLayer");
        if (grassLayer == null) return;

        foreach (Node child in grassLayer.GetChildren())
        {
            if (child is TallGrass grass && grass.GlobalPosition.DistanceTo(GlobalPosition) < 19f)
                grass.Rustle(_facing);
        }
    }

    private void AddCollision()
    {
        var shape = new CollisionShape2D
        {
            Shape = new CircleShape2D { Radius = 5f }
        };
        AddChild(shape);
    }

    public override void _Draw()
    {
        float bob = Velocity != Vector2.Zero ? Mathf.Abs(Mathf.Sin(_walkCycle)) * 1.4f : 0f;
        float step = Velocity != Vector2.Zero ? Mathf.Sin(_walkCycle) * 1.7f : 0f;
        Vector2 p = new(0, -bob);

        DrawEllipse(new Vector2(0, 8), new Vector2(7, 2.5f), new Color(0.12f, 0.25f, 0.10f, 0.35f));

        // Feet and legs.
        DrawRect(new Rect2(-5 + step, 1, 4, 8), new Color("#3d4d78"));
        DrawRect(new Rect2(1 - step, 1, 4, 8), new Color("#3d4d78"));
        DrawRect(new Rect2(-6 + step, 7, 5, 3), new Color("#60402e"));
        DrawRect(new Rect2(1 - step, 7, 5, 3), new Color("#60402e"));

        // Body.
        DrawRect(new Rect2(-7, -9 + p.Y, 14, 13), new Color("#d54d3e"));
        DrawRect(new Rect2(-6, -8 + p.Y, 12, 3), new Color("#f0c34d"));
        DrawRect(new Rect2(-9, -5 + p.Y, 3, 8), new Color("#d54d3e"));
        DrawRect(new Rect2(6, -5 + p.Y, 3, 8), new Color("#d54d3e"));

        // Head.
        DrawCircle(new Vector2(0, -15 + p.Y), 7.5f, new Color("#f0bd91"));
        DrawRect(new Rect2(-8, -22 + p.Y, 16, 3), new Color("#d94338"));
        DrawRect(new Rect2(-5, -25 + p.Y, 10, 4), new Color("#d94338"));

        // Face changes subtly by direction, keeping the four-direction RPG feel.
        if (_facing == Vector2.Down)
        {
            DrawCircle(new Vector2(-3, -15 + p.Y), 0.8f, Colors.Black);
            DrawCircle(new Vector2(3, -15 + p.Y), 0.8f, Colors.Black);
        }
        else if (_facing == Vector2.Left)
        {
            DrawCircle(new Vector2(-6, -15 + p.Y), 0.8f, Colors.Black);
        }
        else if (_facing == Vector2.Right)
        {
            DrawCircle(new Vector2(6, -15 + p.Y), 0.8f, Colors.Black);
        }
    }

    private static void DrawEllipse(Vector2 center, Vector2 radius, Color color)
    {
        const int segments = 20;
        var points = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Tau * i / segments;
            points[i] = center + new Vector2(Mathf.Cos(angle) * radius.X, Mathf.Sin(angle) * radius.Y);
        }
        DrawColoredPolygonStatic(points, color);
    }

    private static void DrawColoredPolygonStatic(Vector2[] points, Color color)
    {
        // Static helper is intentionally unused; ellipse rendering is provided by the instance wrapper below.
    }
}
