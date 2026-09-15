# Pocket Grass Adventure

Godot 4 + C# 2D RPG prototype inspired by classic top-down monster-catching RPGs.

## Current prototype

- 16px tile-oriented world
- Four-direction player movement with WASD / arrow keys
- Camera following the player
- Ground, grass, decoration and collision layers
- Procedurally generated paths, trees and rocks
- Dense tall-grass patches
- Localized grass rustling when the player walks through grass
- Simple pixel-art-style placeholder character without external assets

## Structure

```text
Main.tscn
scripts/
├── world/World.cs
├── player/Player.cs
└── grass/TallGrass.cs
```

The procedural visuals are intentionally placeholders. The next stage can replace them with real pixel-art tiles and sprite sheets while keeping the gameplay architecture intact.
