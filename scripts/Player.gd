extends CharacterBody2D

const SPEED := 125.0
const SHEET := preload("res://assets/player_sheet.svg")
var facing := Vector2.DOWN
var state := "idle"
var anim_time := 0.0
var action_time := 0.0
var step_time := 0.0
var sprite: Sprite2D

func _ready() -> void:
    z_index = 20
    collision_layer = 1
    collision_mask = 1
    var shape := CollisionShape2D.new()
    var circle := CircleShape2D.new()
    circle.radius = 5.0
    shape.shape = circle
    add_child(shape)
    sprite = Sprite2D.new()
    sprite.texture = SHEET
    sprite.region_enabled = true
    sprite.region_rect = Rect2(0, 0, 32, 48)
    sprite.position = Vector2(0, -16)
    sprite.texture_filter = CanvasItem.TEXTURE_FILTER_NEAREST
    sprite.scale = Vector2(1.35, 1.35)
    add_child(sprite)

func _physics_process(delta: float) -> void:
    anim_time += delta
    action_time = maxf(action_time - delta, 0.0)
    if Input.is_action_just_pressed("attack") and action_time <= 0.0:
        state = "attack"
        action_time = 0.34
    elif Input.is_action_just_pressed("defend") and action_time <= 0.0:
        state = "defend"
        action_time = 0.45
    if action_time > 0.0:
        velocity = Vector2.ZERO
    else:
        var input := Input.get_vector("move_left", "move_right", "move_up", "move_down")
        if absf(input.x) > absf(input.y): input.y = 0.0
        else: input.x = 0.0
        velocity = input * SPEED
        if input != Vector2.ZERO:
            facing = input
            state = "run"
            step_time += delta
        else:
            state = "idle"
            step_time = 0.0
    move_and_slide()
    position.x = clampf(position.x, 24.0, 1256.0)
    position.y = clampf(position.y, 30.0, 744.0)
    update_animation()

func update_animation() -> void:
    if sprite == null: return
    var frame := 0
    if state == "idle": frame = int(anim_time * 2.0) % 2
    elif state == "run": frame = 2 + (int(step_time * 9.0) % 3)
    elif state == "attack": frame = 5 if action_time > 0.17 else 6
    elif state == "defend": frame = 7
    sprite.region_rect = Rect2(frame * 32, 0, 32, 48)
    sprite.flip_h = facing == Vector2.LEFT
    sprite.modulate = Color(0.78, 0.86, 1.0, 1.0) if state == "defend" else Color.WHITE
    sprite.position.y = -16.0 + (absf(sin(step_time * 9.0)) * 1.5 if state == "run" else 0.0)
