using Godot;
using System;

public partial class Main : Node2D
{
    private Player _player = null!;
    private readonly Random _random = new();
    private const int GrassCount = 420;
    private const float WorldWidth = 960f;
    private const float WorldHeight = 540f;

    public override void _Ready()
    {
        QueueRedraw();
        CreateGrass();
        _player = new Player { Position = new Vector2(WorldWidth / 2f, WorldHeight / 2f) };
        AddChild(_player);
    }

    private void CreateGrass()
    {
        for (int i = 0; i < GrassCount; i++)
        {
            var blade = new GrassBlade
            {
                Position = new Vector2(_random.Next(20, 940), _random.Next(40, 510)),
                Rotation = (float)_random.NextDouble() * 0.35f - 0.175f,
                Scale = Vector2.One * (0.7f + (float)_random.NextDouble() * 0.7f)
            };
            AddChild(blade);
        }
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(0, 0, WorldWidth, WorldHeight), new Color("#72ad4c"));
        // Soft patches give the otherwise procedural grass a Pokémon-like field feel.
        for (int y = 0; y < 12; y++)
            for (int x = 0; x < 20; x++)
                if ((x + y) % 3 == 0)
                    DrawCircle(new Vector2(x * 50 + 25, y * 50 + 35), 24, new Color(0.38f, 0.64f, 0.25f, 0.14f));
    }

    private partial class GrassBlade : Node2D
    {
        private float _phase;
        private float _bend;
        private double _time;

        public override void _Ready()
        {
            _phase = (float)GD.RandRange(0.0, Math.PI * 2.0);
            _time = GD.RandRange(0.0, 3.0);
            ZIndex = 2;
            QueueRedraw();
        }

        public override void _Process(double delta)
        {
            _time += delta;
            // A tiny idle sway keeps the field alive without looking like a windstorm.
            _bend = Mathf.Sin((float)_time * 1.4f + _phase) * 0.035f;
            QueueRedraw();
        }

        public void Rustle(Vector2 direction)
        {
            _bend += Mathf.Clamp(direction.X * 0.32f + direction.Y * 0.15f, -0.42f, 0.42f);
            QueueRedraw();
        }

        public override void _Draw()
        {
            DrawLine(Vector2.Zero, new Vector2(-3, -12).Rotated(_bend), new Color("#397332"), 2.5f, true);
            DrawLine(Vector2.Zero, new Vector2(1, -16).Rotated(_bend), new Color("#4d8737"), 3f, true);
            DrawLine(Vector2.Zero, new Vector2(5, -10).Rotated(_bend), new Color("#2f692e"), 2.5f, true);
        }
    }

    private partial class Player : CharacterBody2D
    {
        private const float Speed = 170f;
        private float _walkTime;
        private Vector2 _lastVelocity;
        private AnimatedSprite2D? _sprite;

        public override void _Ready()
        {
            ZIndex = 10;
            CollisionLayer = 0;
            CollisionMask = 0;
            QueueRedraw();
        }

        public override void _PhysicsProcess(double delta)
        {
            Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            Velocity = input * Speed;
            MoveAndSlide();
            Position = new Vector2(Mathf.Clamp(Position.X, 24, WorldWidth - 24), Mathf.Clamp(Position.Y, 40, WorldHeight - 20));

            if (input.LengthSquared() > 0.01f)
            {
                _walkTime += (float)delta * 10f;
                _lastVelocity = input;
                QueueRedraw();
                TriggerGrass(input);
            }
            else
            {
                _walkTime = 0;
                QueueRedraw();
            }
        }

        private void TriggerGrass(Vector2 direction)
        {
            foreach (Node node in GetParent().GetChildren())
            {
                if (node is GrassBlade grass && grass.GlobalPosition.DistanceTo(GlobalPosition) < 22f)
                    grass.Rustle(direction);
            }
        }

        public override void _Draw()
        {
            // Simple pixel-art-like character generated from primitives, so the project has no external assets.
            float bob = Velocity.LengthSquared() > 0 ? Mathf.Abs(Mathf.Sin(_walkTime)) * 2.2f : 0;
            Vector2 p = new Vector2(0, -bob);

            // shadow
            DrawEllipse(new Vector2(0, 13), new Vector2(12, 4), new Color(0.08f, 0.25f, 0.08f, 0.35f));
            // legs
            float step = Velocity.LengthSquared() > 0 ? Mathf.Sin(_walkTime) * 3f : 0;
            DrawRect(new Rect2(-8 + step, 3, 6, 10), new Color("#334b7d"));
            DrawRect(new Rect2(2 - step, 3, 6, 10), new Color("#334b7d"));
            // body
            DrawRect(new Rect2(-10, -10 + p.Y, 20, 18), new Color("#d94b3d"));
            DrawRect(new Rect2(-8, -9 + p.Y, 16, 3), new Color("#f0c64f"));
            // head
            DrawCircle(new Vector2(0, -18 + p.Y), 10, new Color("#f2c59b"));
            // hair/hat
            DrawRect(new Rect2(-11, -28 + p.Y, 22, 5), new Color("#e94e40"));
            DrawRect(new Rect2(-7, -32 + p.Y, 14, 5), new Color("#e94e40"));
            // face
            DrawCircle(new Vector2(-4, -18 + p.Y), 1.2f, Colors.Black);
            DrawCircle(new Vector2(4, -18 + p.Y), 1.2f, Colors.Black);
        }

        private void DrawEllipse(Vector2 center, Vector2 radius, Color color)
        {
            const int segments = 24;
            var points = new Vector2[segments];
            for (int i = 0; i < segments; i++)
            {
                float a = Mathf.Tau * i / segments;
                points[i] = center + new Vector2(Mathf.Cos(a) * radius.X, Mathf.Sin(a) * radius.Y);
            }
            DrawColoredPolygon(points, color);
        }
    }
}
