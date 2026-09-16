# Character animation

The original `player_sheet.svg` contains eight 32x48 frames:

0-1 idle
2-4 run
5-6 attack
7 defend

`Player.gd` switches the atlas region at runtime. This is intentionally kept as a simple first animation layer before moving to full four-direction sprite sheets and AnimatedSprite2D resources.