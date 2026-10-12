# Lightbound

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
- **Ring energy:** 0-100%. Free flight (including the launch impulse) costs nothing — only shooting drains it in normal mode.
- **Recharge:** only at checkpoints, or at a **Lantern Battery** station (restores to 100%).
- **Pea shot:** small projectile, cooldown, energy cost in normal mode.
- **Low Power Mode** (state machine with hysteresis):
  - Enter when energy <= 1%.
  - Shots are free/unlimited but deal reduced damage.
  - Each hit on an enemy, or each asteroid broken by a shot, adds a tiny amount of charge.
  - Exit when energy >= 50%, or on checkpoint / Lantern Battery station (-> 100%).
  - Do not exit between 1% and 50% by any other path.
- **No friendly fire.** Players' shots never damage other players.
- Max 4 players per session.

## Lantern Battery (fixed station)
- A permanent, non-collectable station placed in the sandbox. Players use it to fully recharge the ring to 100%.
- Also ends Low Power Mode immediately (calls `RingEnergy.SetFull()`).
- Reusable with no limit in the sandbox. It is never picked up, consumed or moved.
- Activation: the player flies into its charging zone (trigger collider) and holds the interact input to charge for a short charge time. It does not recharge instantly. Releasing interact or leaving the zone cancels the charge.
- Each player in the zone charges independently.
- Server-authoritative once networking exists: the server validates that the player is in range and applies the recharge.
- Visuals: a large glowing battery/lantern landmark, readable from far away, with a clear charging effect and sound.
- Tunables (ScriptableObject, never hard-coded): charge time, zone radius, restore percent (default 100%).

## Architecture rules
- Server-authoritative: energy, damage, checkpoints, and low-power state live on the server; clients only predict movement and VFX.
- Keep tunable numbers (drain rates, damage multiplier, charge-per-hit, 1%/50% thresholds, station charge time) in ScriptableObjects, never hard-coded.
- Ring logic is a plain C# class (`RingEnergy`) with no Unity dependencies so it can be unit tested.
- One responsibility per script; prefer composition over inheritance.
- Code namespace: `Lightbound`.

## Suggested folder layout
```
Assets/
  _Project/
    Scripts/
      Player/      (FlightController, RingEnergy, PeaShooter, SurfaceAttach)
      Enemies/     (Turret, EnemyShip)
      World/       (Asteroid, AsteroidField, Checkpoint, LanternBatteryStation)
      Networking/  (SessionManager, SteamLobby)
      Voice/
      UI/          (EnergyBar, Emblem display)
    Data/          (ScriptableObjects: RingConfig, WeaponConfig, StationConfig)
    Prefabs/  Scenes/  Art/  Audio/
  Tests/EditMode/  (RingEnergy tests)
```

## Build order
1. ✅ Solo flight + energy bar in an empty scene
2. ✅ Landing on surfaces (landing, takeoff, ground walking)
3. ✅ Pea shooter + one turret enemy (click-to-fire, raycast projectile, destructible/respawning turret target)
4. Low Power Mode + Lantern Battery station + checkpoints — station itself is ✅ done (built ahead of order); Low Power Mode's state machine is ✅ done, unit tested, and now wired into gameplay via the pea shooter (reduced damage, free shots, charge-per-hit); checkpoints not started
5. ✅ Asteroid belt (wave spawner, cascading breaks, size-based health, Low Power charge-on-hit)
6. 2-player networking, then 4
7. Proximity voice, then Steam lobbies

## Working with Claude
- Use plan mode before any networking or large system.
- Write unit tests for `RingEnergy` (especially the 1% / 50% hysteresis) before wiring it into gameplay.
- Commit after each build-order step. Never refactor and add features in the same commit.
- Ask before adding a new package or dependency.
