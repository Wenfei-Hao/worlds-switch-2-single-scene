# Single-Scene Dual-Realm Prototype (Time-Lamp Masking Demo)

This Unity project is a small 3D prototype that explores **time / phase shifting** using a **single scene + shader mask**, instead of the more common “duplicate two levels and swap scenes” approach.

The player carries a **time-lamp**: wherever the lamp shines, an overlapping **inner world** becomes visible and solid. Outside of the lamp’s radius, the level looks and behaves like a normal environment.

---

## Overview

- **Engine:** Unity 3D, Built-in Render Pipeline  
- **Language:** C#  
- **Core idea:** Both the **outer world** and the **inner world** are placed in the *same* scene.  
  A custom shader and scripts control where the inner world is rendered and where its colliders are enabled.

This prototype is meant as a **mechanics + tech demo** for level design with local “phase switching”:
bridges that only exist in the inner world, time-locked doors, platforms that appear and disappear under the lamp, etc.

---

## Core Mechanics

- **Time-Lamp Reveal**
  - A spherical mask is centered on the player (or a lamp object).
  - Inside the mask, inner-world geometry is revealed with a distinct color grade and glowing edge.
  - Outside the mask, inner-world objects are completely hidden.

- **Synced Physics**
  - Inner-world platforms and obstacles only have active **Colliders** when they are inside the reveal sphere.
  - Visual reveal and physical affordance are tied to the same distance function, so there is no “invisible wall” or “ghost platform” mismatch.

- **Local, Not Global, Switching**
  - Only a *local region* of the world shifts state at any time.
  - This enables puzzles where players must plan their route by moving the lamp instead of teleporting between two full scenes.

---

## Technical Highlights

- **Custom Shader – `InnerWorldReveal`**
  - Surface shader in the Built-in pipeline.
  - Computes a **world-space spherical mask** using the Euclidean distance from each fragment to the reveal center.
  - Applies:
    - A configurable **tint** for the inner world.
    - A smooth **glowing ring** at the boundary (using an analytical function of the distance) to make the “time rift” clearly visible.

- **Mask Controller – `RevealMaskController`**
  - Tracks a `source` transform (usually the player root or camera).
  - Each frame, updates the reveal center & radius on all inner-world materials.
  - Exposes current center/radius as public properties so other systems (e.g., physics) can reuse the same spatial condition.

- **Physics Bridge – `RevealCollider`**
  - Component for inner-world platforms, doors, etc.
  - Enables or disables attached Colliders depending on their distance to the reveal center.
  - Guarantees that “visible inner world” and “solid inner world” are always consistent.

---

## Project Structure (Key Elements)

- `Scenes/SampleScene`  
  Single demo scene containing:
  - `World_Outer`: regular environment geometry.
  - `World_Inner`: inner-world variants (walls, platforms, test objects).
  - `PlayerRoot`: simple character controller + camera.
  - `WorldRevealController`: drives the reveal mask.

- `Shaders/InnerWorldReveal.shader`  
  Inner-world surface shader with distance-based masking and edge glow.

- `Scripts/RevealMaskController.cs`  
  Updates shader parameters and exposes current reveal center/radius.

- `Scripts/RevealCollider.cs`  
  Toggles Colliders for inner-world objects based on the same mask.

---

## How to Run

1. Open the project in **Unity 2022 LTS** (Built-in Render Pipeline).
2. Load `Scenes/SampleScene`.
3. Press **Play**.
4. Use the provided character controls (e.g., WASD + mouse) to move around.
5. Watch how the time-lamp reveals inner-world walls and platforms; only when they are inside the glowing sphere do they become walkable.

---

## Possible Extensions

This prototype is intentionally minimal, but it can be extended in several directions:

- Replace the spherical mask with a **cone / spotlight volume** for more precise “lamp” behavior.
- Add **AI and enemy states** that only exist or are active in the inner world.
- Design puzzle levels that require switching between inner/outer paths, time-locked doors, and phase-only shortcuts.

The goal of this project is not to be a full game, but to demonstrate a **clean, mathematically grounded implementation** of single-scene dual-realm traversal that is friendly to both performance and level design iteration.
