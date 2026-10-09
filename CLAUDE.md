# Space Lanterns

Indie co-op multiplayer "friend slop" game. First-person, zero-g space flight, ring powers, silly food chest emblems. Target: Steam, solo or online co-op, **max 4 players**, proximity voice chat. Visual/UI inspiration: Peak (chunky UI, soft pastel low-poly look). All names, emblems and lore must be original.

## Stack
- Unity (C#), URP
- Netcode for GameObjects (swap to Photon Fusion only if needed)
- Facepunch.Steamworks for lobbies/invites
- Photon Voice or Dissonance for proximity chat

## Current goal
Sandbox proof of concept. Do not build menus, progression or cosmetics yet.

## Core design rules (source of truth)
- **Flight:** 6DOF zero-g thrust, light drag, first-person camera.
- **Landing:** players can touch down on surfaces (rocks, ships) and align to them. Landing does NOT recharge the ring.
- **Ring energy:** 0-100%. Flying, boosting and shooting drain it.
- **Recharge:** only at checkpoints, or by picking up a **Lantern Battery** (restores to 100%).
- **Pea shot:** small projectile, cooldown, energy cost in normal mode.
- **Low Power Mode** (state machine with hysteresis):
  - Enter when energy <= 1%.
  - Shots are free/unlimited but deal reduced damage.
  - Each hit on an enemy, or each asteroid broken by a shot, adds a tiny amount of charge.
  - Exit when energy >= 50%, or on checkpoint / Lantern Battery (-> 100%).
  - Do not exit between 1% and 50% by any other path.
- **No friendly fire.** Players' shots never damage other players.
- Max 4 players per session.

## Architecture rules
- Server-authoritative: energy, damage, checkpoints, and low-power state live on the server; clients only predict movement and VFX.
- Keep tunable numbers (drain rates, damage multiplier, charge-per-hit, 1%/50% thresholds) in ScriptableObjects, never hard-coded.
- Ring logic is a plain C# class (`RingEnergy`) with no Unity dependencies so it can be unit tested.
- One responsibility per script; prefer composition over inheritance.

## Suggested folder layout
```
Assets/
  _Project/
    Scripts/
      Player/      (FlightController, RingEnergy, PeaShooter, SurfaceAttach)
      Enemies/     (Turret, EnemyShip)
      World/       (Asteroid, AsteroidField, Checkpoint, LanternBattery)
      Networking/  (SessionManager, SteamLobby)
      Voice/
      UI/          (EnergyBar, Emblem display)
    Data/          (ScriptableObjects: RingConfig, WeaponConfig)
    Prefabs/  Scenes/  Art/  Audio/
Tests/EditMode/    (RingEnergy tests)
```

## Build order
1. Solo flight + energy bar in an empty scene
2. Landing on surfaces
3. Pea shooter + one turret enemy
4. Low Power Mode + Lantern Battery + checkpoints
5. Asteroid belt (breakable asteroids)
6. 2-player networking, then 4
7. Proximity voice, then Steam lobbies

## Working with Claude
- Use plan mode before any networking or large system.
- Write unit tests for `RingEnergy` (especially the 1% / 50% hysteresis) before wiring it into gameplay.
- Commit after each build-order step. Never refactor and add features in the same commit.
- Ask before adding a new package or dependency.
