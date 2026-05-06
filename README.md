# ShapeShift

ShapeShift is a Unity-based 3D shape-matching runner where the player must quickly switch forms to pass through matching obstacle walls. The game combines simple forward movement with fast recognition and timing: choose the correct shape, survive the obstacle, and reach the finish line.

## Overview

In each run, the player moves through a linear level filled with shape-based wall obstacles. To pass safely, the active player shape must match the wall opening. Correct matches build a perfect-hit score, while incorrect matches fail the level.

The current prototype includes:

- Four playable shapes: Cube, Sphere, Cylinder, and Triangle
- Shape switching through swipe-style input
- Progress tracking from spawn to finish line
- Level completion and retry flow
- Animated score feedback for perfect matches
- Camera transitions at the finish line
- Particle and skybox feedback for gameplay events

## Controls

### Mobile

- Swipe up, down, left, or right to change shape

### Desktop

- Click and drag with the mouse to simulate a swipe
- Arrow keys or directional input also trigger shape changes

## Running The Project

### Requirements

- Unity `6000.0.48f1`

### Setup

1. Open the project through Unity Hub using Unity `6000.0.48f1`
2. Allow Unity to import packages and assets
3. Open `Assets/Game/Scenes/SampleScene.unity` if it is not already loaded
4. Press Play in the Unity Editor

The project currently uses a single playable scene configured in Build Settings.

## Project Structure

```text
Assets/
  Game/
    Configs/         ScriptableObject configs for gameplay systems
    Scenes/          Playable scene assets
    Scripts/
      Camera/        Gameplay and finish-line camera systems
      GameService/   Update loop and game lifecycle services
      LevelService/  Level spawning, chunk assembly, finish line logic
      Player/        Player input, movement, shape switching, state machine
      Score/         Perfect-match score logic and animated score UI
      SkyboxService/ Runtime sky color transitions
      UI/            Main menu, HUD, fail screen, complete screen
      Walls/         Shape wall blocks, collision listeners, wall feedback
  Plugins/           Third-party libraries and bundled plugins
```

## Screenshots
<img width="208" height="421" alt="Screenshot 2026-05-06 152234" src="https://github.com/user-attachments/assets/89f7a083-8b8f-4ee2-9136-5a8ab464b6d3" />
<img width="208" height="421" alt="Screenshot 2026-05-06 151914" src="https://github.com/user-attachments/assets/9e5aeb5b-9096-47a1-a0df-68ed9f695acc" />
<img width="208" height="421" alt="Screenshot 2026-05-06 151854" src="https://github.com/user-attachments/assets/3d395332-fc8b-409f-807b-d1955f05c127" />

https://github.com/user-attachments/assets/c0c2f38f-dd44-4fbd-bceb-1e762708091e




## Architecture

ShapeShift is structured around service-based gameplay systems and Zenject-powered dependency injection.

Core systems include:

- `PlayerService`
  Controls player spawning, active shape state, collision checks, death, and level completion behavior.
- `LevelService`
  Builds levels from reusable chunk prefabs, tracks the active level, and handles restart and next-level flow.
- `UIService`
  Manages menu and gameplay windows.
- `ScoreService`
  Tracks perfect collisions and updates score feedback.
- `CameraService`
  Handles follow camera behavior and finish-line camera transitions.
- `SkyService`
  Applies dynamic skybox color changes during gameplay.

## Dependencies

This project currently uses:

- Unity 6
- C#
- Universal Render Pipeline (URP)
- Unity Input System
- Cinemachine
- TextMeshPro
- Zenject
- DOTween

Additional third-party art and VFX assets are also included in the repository.

## Current State

This project is in prototype / active development form. The main gameplay loop is present and playable, but some systems still appear to be in progress or placeholder.

## Known Issues / TODO

- The project previously shipped with minimal external documentation
- Only one gameplay scene is currently configured
- Level progression wraps across a small configured level list
- Some UI actions, such as `Remove Ads`, are placeholder-only
- Some features in code, such as beast-mode feedback, may need clearer final gameplay design
- Project-specific automated tests are not currently documented
