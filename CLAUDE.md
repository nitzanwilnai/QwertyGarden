# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Qwerty Garden** is a 2D cozy typing game built in Unity 6 (6000.0.66f2). Players type words to grow flowers — each keyboard key maps to a flower type. Features include a shop system, prestige mechanics, achievements, and Steam integration.

Single scene: `Assets/Scenes/MainGameScene.unity`

## Architecture

This project follows **Data-Oriented Design (DOD)** as described in *Data-Oriented Design for Games* by Nitzan Wilnai. Use the `unity-dod-architecture` skill for full architectural rules. Key principles:

- **Balance** — static config loaded from `Resources/balance.bytes` (binary, parsed at tool time from ScriptableObjects)
- **MetaData / KeyboardData** — mutable player state, persisted via binary IO classes with versioned formats
- **Logic** — static class with pure static functions. Takes data in, transforms, outputs data. Never references MonoBehaviours
- **Board** — MonoBehaviour handling input + visual rendering. Calls Logic functions, syncs visuals from data
- **Game** — Singleton MonoBehaviour owning Balance, MetaData, KeyboardData. Drives Board and menu state transitions

Menu states flow through `MENU_STATE` enum: TITLE, MAIN_MENU, GARDEN_SELECTION, KEYBOARD_SELECTION, FLOWER_SELECTION, EDIT_GARDEN, IN_GAME, SETTINGS, PRESTIGE.

### Visual Classes

`*Visual` classes (EditFlowersVisual, PrestigeVisual, TitleVisual, CommonVisual) manage UI state and rendering. They use the **GUIRef** pattern — UI elements are referenced by string keys on prefab-attached GUIRef MonoBehaviours.

### Particle System (DOD)

Pure DOD implementation: `ParticleSystemLogic` (static functions) + `ParticleSystemBoard` (MonoBehaviour pooling/rendering) with parallel arrays for Position, Velocity, Time, Color, Radius.

### Data Persistence

Binary save/load with version migration chains:
- `MetaDataIO`: TryLoadMeta (current), TryLoadMetaV4, TryLoadMetaV2
- `KeyboardDataIO`: LoadKeyboard (current), LoadKeyboardV3, LoadKeyboardV2

When adding new fields, increment the version and add `if (version >= N)` guards.

## Build Commands

Custom build menu in Unity Editor (`Assets/Editor/BuildGame.cs`):
- `QwertyGarden/Build/Mac` — macOS .app
- `QwertyGarden/Build/PC` — Windows .exe (64-bit)
- `QwertyGarden/Build/Steamdeck` — Linux x86_64
- Demo variants add `DEMO` scripting define (limits flowers to 4)
- Video variant adds `VIDEO` scripting define

## Key Conventions

- **C# naming**: Public functions PascalCase, private helpers camelCase, member fields `m_` prefix
- **No allocation during gameplay**: Arrays pre-allocated in Init/AllocateGameData; use array+count instead of List<T>
- **Object pooling**: Pre-instantiate GameObjects, activate/deactivate — never Instantiate/Destroy during play
- **Steam integration**: Guarded by preprocessor directives (`#if` with Steam defines)
- **Namespace**: Tool utilities live in `CommonTools` namespace
- **Game types**: LESSON (structured typing) and COZY (free-form)

## Dependencies

- **Steamworks.NET** (2025.163.0) — Steam API
- **Unity Input System** (1.17.0) — keyboard input
- **URP** (17.0.4) — 2D rendering pipeline
- **Unity 2D Feature** (2.0.1) — sprites and 2D tools
