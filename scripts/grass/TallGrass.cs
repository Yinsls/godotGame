using Godot;

public partial class TallGrass : Node2D
{
    private float _phase;
    private double _time;
    private float _rustle;
    private Vector2 _rustleDirection;

    public override void _Ready()
    {
        _phase = (float)GD.RandRange(0.0, Mathf.Tau);
        _time = GD.RandRange(0.0, 5.0);
        ZIndex = 0;
        QueueRedraw();
    }

    public override void _Process(double delta)
    {
        _time += delta;
        _rustle = Mathf.MoveToward(_rustle, 0f, (float)delta * 5.5f);
        QueueRedraw();
    }

    public void Rustle(Vector2 direction)
    {
        _rustle = Mathf.Max(_rustle, 1f);
        _rustleDirection = direction;
        QueueRedraw();
    }

    public override void _Draw()
    {
        float wind = Mathf.Sin((float)_time * 1.5f + _phase) * 0.10f;
        float impact = _rustle * 0.42f;
        float direction = _rustleDirection.X != 0 ? _rustleDirection.X : _rustleDirection.Y * 0.45f;
        float bend = wind + impact * direction;

        DrawLine(Vector2.Zero, new Vector2(-4, -13).Rotated(bend), new Color("#356f31"), 2.4f, true);
        DrawLine(Vector2.Zero, new Vector2(0, -17).Rotated(bend * 1.15f), new Color("#4f8e3d"), 2.8f, true);
        DrawLine(Vector2.Zero, new Vector2(5, -12).Rotated(bend * 0.85f), new Color("#2d652d"), 2.4f, true);

        if (_rustle > 0.05f)
        {
            float wave = Mathf.Sin(_rustle * Mathf.Pi);
            DrawLine(new Vector2(-5, -7), new Vector2(-5 + direction * 3f, -10 - wave * 3f), new Color(0.65f, 0.82f, 0.38f, 0.65f), 1.2f, true);
        }
    }
}
