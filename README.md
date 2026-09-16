# Pocket Grass Adventure

A small original 2D pixel-RPG prototype built for Godot 4.7.

## Current build

- Standalone GDScript runtime: the playable scene does not depend on .NET/C#.
- Original SVG character material with idle, running, attack and defense frames.
- Four-direction movement with WASD / arrow keys.
- J: attack. K: defend.
- Camera follows the player.
- Large field with dirt path, trees, rocks and dense tall-grass patches.
- Tall grass reacts locally when the player walks through it.

## Controls

| Key | Action |
|---|---|
| WASD / Arrow keys | Move |
| J | Attack |
| K | Defend |

## Direction

This repository intentionally starts with a dependency-free playable foundation. The next layer is a real TileSet/TileMap map, richer four-direction sprite sheets, frame-based AnimationPlayer/AnimatedSprite2D animations, NPCs, dialogue, map transitions and encounters.
