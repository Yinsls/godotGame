extends Node2D

const TILE := 16
const MAP_W := 80
const MAP_H := 48
const MAP_SIZE := Vector2(MAP_W * TILE, MAP_H * TILE)
const PLAYER_SPEED := 105.0

var player := Vector2(40 * TILE, 24 * TILE)
var facing := Vector2.DOWN
var walk_time := 0.0
var moving := false
var grass: Array = []
var trees: Array = []
var rocks: Array = []
var rng := RandomNumberGenerator.new()
var camera: Camera2D

func _ready() -> void:
    rng.seed = 20260915
    _build_map_data()
    camera = Camera2D.new()
    camera.position_smoothing_enabled = true
    camera.position_smoothing_speed = 8.0
    camera.limit_left = 0
    camera.limit_top = 0
    camera.limit_right = int(MAP_SIZE.x)
    camera.limit_bottom = int(MAP_SIZE.y)
    camera.position = player
    add_child(camera)
    queue_redraw()

func _process(delta: float) -> void:
    var input := Input.get_vector("move_left", "move_right", "move_up", "move_down")
    moving = input.length() > 0.0
    if moving:
        if abs(input.x) > abs(input.y):
            input = Vector2(sign(input.x), 0)
        else:
            input = Vector2(0, sign(input.y))
        facing = input
        player += input * PLAYER_SPEED * delta
        walk_time += delta * 9.0
        _rustle_nearby_grass(input)
    else:
        walk_time = move_toward(walk_time, 0.0, delta * 8.0)

    player.x = clamp(player.x, 12.0, MAP_SIZE.x - 12.0)
    player.y = clamp(player.y, 12.0, MAP_SIZE.y - 12.0)
    camera.position = player
    queue_redraw()

func _build_map_data() -> void:
    var center := Vector2(40 * TILE, 24 * TILE)
    for i in 22:
        var p := Vector2.ZERO
        while true:
            p = Vector2(rng.randi_range(3, MAP_W - 3) * TILE + 8, rng.randi_range(3, MAP_H - 3) * TILE + 8)
            if p.distance_to(center) > 90:
                break
        trees.append(p)

    for i in 8:
        var p := Vector2.ZERO
        while true:
            p = Vector2(rng.randi_range(3, MAP_W - 3) * TILE + 8, rng.randi_range(3, MAP_H - 3) * TILE + 8)
            if p.distance_to(center) > 55:
                break
        rocks.append(p)

    var patches = [Rect2i(20, 6, 14, 10), Rect2i(45, 5, 17, 12), Rect2i(58, 27, 14, 13), Rect2i(15, 30, 18, 9), Rect2i(39, 35, 13, 8)]
    for patch in patches:
        for y in patch.size.y:
            for x in patch.size.x:
                grass.append({"pos": Vector2((patch.position.x + x) * TILE + 8, (patch.position.y + y) * TILE + 13), "bend": 0.0, "phase": rng.randf_range(0.0, TAU)})

func _rustle_nearby_grass(direction: Vector2) -> void:
    for blade in grass:
        if blade.pos.distance_to(player) < 25.0:
            blade.bend = direction.x if direction.x != 0 else direction.y * 0.5

func _draw() -> void:
    draw_rect(Rect2(Vector2.ZERO, MAP_SIZE), Color("#78ad50"))
    for y in MAP_H:
        for x in MAP_W:
            if (x * 17 + y * 31) % 7 == 0:
                draw_rect(Rect2(x * TILE + 3, y * TILE + 5, 2, 2), Color("#6b9f49"))
    _draw_path()
    _draw_grass()
    _draw_rocks()
    _draw_trees()
    _draw_player()

func _draw_path() -> void:
    for y in range(4, MAP_H - 4):
        var x := 9 + int(round(sin(y * 0.24) * 4.0))
        for i in 5:
            var r := Rect2((x + i) * TILE, y * TILE, TILE, TILE)
            draw_rect(r, Color("#c6a46a"))
            draw_rect(Rect2(r.position + Vector2(3, 4), Vector2(2, 2)), Color("#b18e5b"))

func _draw_grass() -> void:
    var t := Time.get_ticks_msec() * 0.0015
    for blade in grass:
        var p: Vector2 = blade.pos
        var idle := sin(t + blade.phase) * 1.5
        var bend: float = idle + blade.bend * 6.0
        draw_line(p + Vector2(-5, 0), p + Vector2(-2 + bend, -10), Color("#397b38"), 2.0)
        draw_line(p, p + Vector2(1 + bend, -13), Color("#579746"), 2.0)
        draw_line(p + Vector2(5, 0), p + Vector2(3 + bend, -9), Color("#2f6e32"), 2.0)
        blade.bend = move_toward(blade.bend, 0.0, 0.14)

func _draw_trees() -> void:
    for p in trees:
        draw_rect(Rect2(p.x - 4, p.y + 2, 8, 16), Color("#76502f"))
        draw_circle(p + Vector2(0, -8), 17, Color("#285f35"))
        draw_circle(p + Vector2(-10, -3), 11, Color("#347540"))
        draw_circle(p + Vector2(10, -3), 11, Color("#347540"))
        draw_circle(p + Vector2(0, -16), 11, Color("#40834a"))
        draw_rect(Rect2(p.x - 10, p.y - 5, 20, 4), Color("#4c8d4c"))

func _draw_rocks() -> void:
    for p in rocks:
        var points := PackedVector2Array([p + Vector2(-10, 6), p + Vector2(-7, -5), p + Vector2(0, -9), p + Vector2(9, -3), p + Vector2(8, 6), p + Vector2(0, 9)])
        draw_colored_polygon(points, Color("#777c74"))
        draw_line(p + Vector2(-5, -3), p + Vector2(2, -6), Color("#a8aaa1"), 2.0)

func _draw_player() -> void:
    var bob := abs(sin(walk_time)) * 1.4 if moving else 0.0
    var step := sin(walk_time) * 1.7 if moving else 0.0
    var p := player + Vector2(0, -bob)
    draw_ellipse(player + Vector2(0, 8), Vector2(7, 2.5), Color(0.12, 0.25, 0.10, 0.35))
    draw_rect(Rect2(p + Vector2(-5 + step, 1), Vector2(4, 8)), Color("#3d4d78"))
    draw_rect(Rect2(p + Vector2(1 - step, 1), Vector2(4, 8)), Color("#3d4d78"))
    draw_rect(Rect2(p + Vector2(-6 + step, 7), Vector2(5, 3)), Color("#60402e"))
    draw_rect(Rect2(p + Vector2(1 - step, 7), Vector2(5, 3)), Color("#60402e"))
    draw_rect(Rect2(p + Vector2(-7, -9), Vector2(14, 13)), Color("#d54d3e"))
    draw_rect(Rect2(p + Vector2(-6, -8), Vector2(12, 3)), Color("#f0c34d"))
    draw_rect(Rect2(p + Vector2(-9, -5), Vector2(3, 8)), Color("#d54d3e"))
    draw_rect(Rect2(p + Vector2(6, -5), Vector2(3, 8)), Color("#d54d3e"))
    draw_circle(p + Vector2(0, -15), 7.5, Color("#f0bd91"))
    draw_rect(Rect2(p + Vector2(-8, -22), Vector2(16, 3)), Color("#d94338"))
    draw_rect(Rect2(p + Vector2(-5, -25), Vector2(10, 4)), Color("#d94338"))
    if facing == Vector2.DOWN:
        draw_circle(p + Vector2(-3, -15), 0.8, Color.BLACK)
        draw_circle(p + Vector2(3, -15), 0.8, Color.BLACK)
    elif facing == Vector2.LEFT:
        draw_circle(p + Vector2(-6, -15), 0.8, Color.BLACK)
    elif facing == Vector2.RIGHT:
        draw_circle(p + Vector2(6, -15), 0.8, Color.BLACK)

func draw_ellipse(center: Vector2, radius: Vector2, color: Color) -> void:
    var points := PackedVector2Array()
    for i in 24:
        var a := TAU * float(i) / 24.0
        points.append(center + Vector2(cos(a) * radius.x, sin(a) * radius.y))
    draw_colored_polygon(points, color)
