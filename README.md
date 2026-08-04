<br/>
<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=unity,cs,dotnet,git,github,rider,blender" />
  </a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.3.13f1-black?logo=unity" />
  <img src="https://img.shields.io/badge/Render%20Pipeline-URP%2017-blue" />
  <img src="https://img.shields.io/badge/Input-New%20Input%20System-green" />
  <img src="https://img.shields.io/badge/Platform-Mobile%20%7C%20PC-lightgrey" />
  <img src="https://img.shields.io/badge/Status-In%20Development-orange" />
</p>

<div align="center">

**Demo Video:**

[![Match Them All - Demo](https://img.youtube.com/vi/I7RJ6TIFnhs/maxresdefault.jpg)](https://youtu.be/I7RJ6TIFnhs)

</div>

## Table of Contents
- **[Introduction](#introduction)**
- **[Gameplay Features](#gameplay-features)**
- **[Meta Game — Hangar & Plane Building](#meta-game--hangar--plane-building)**
- **[Architecture & Design Patterns](#architecture--design-patterns)**
- **[Unity Features Used](#unity-features-used)**
- **[Tech Stack](#tech-stack)**
- **[Project Structure](#project-structure)**
- **[Roadmap](#roadmap)**

<section id="introduction">

<h1 align="center">Match Them All — 3D Match & Merge Puzzle</h1>

**Match Them All** is a 3D casual match & merge puzzle game built in Unity 6 with the Universal Render Pipeline, designed with a mobile-first mindset.

Items are dropped into a physics-driven play area and scattered with an explosion force. The player taps an item, it flies into a limited slot bar with a squash & stretch animation, and **three identical items merge and vanish**. Every level ships with its own goals and time limit — clear the goals before the clock runs out, without letting the slot bar fill up.

Between levels the player returns to the **hangar**, where the wrenches earned during gameplay are spent to build a plane piece by piece: install parts, paint them, pick variations, watch the plane get completed and take off — then a new blueprint starts.

The project is built as a **complete game loop**, not a prototype: gameplay scene + meta scene, persistent save data, an economy, boosters, a revive flow, an async loading screen and a fully event-driven architecture.

</section>

<section id="gameplay-features">

<h2>Gameplay Features</h2>

- **Match & Merge Core Loop**
  - Tap-to-collect items with raycast picking and outline highlight feedback
  - Items are auto-sorted in the slot bar so identical types always land next to each other
  - Three of a kind rise into the air, smash together and merge with particle + sound feedback
  - Limited slot count — the bar filling up with no pending merge means the run is over


- **Data-Driven Levels**
  - Each level is a `ScriptableObject`: item types, spawn amounts, goal flags and time limit
  - Goals are tracked live and the UI reacts through events — no polling
  - Level progress is persisted, so the player continues from where they left off


- **Boosters** — each one is an independent mechanic with its own animation flow
  - **Hammer** — smashes three identical items on the board, deliberately preferring non-goal items
  - **Wind** — a helicopter flies in and its propeller wind lifts, swirls and pulls the items using real physics forces (lift + vortex + centre pull) applied in `FixedUpdate`
  - **Plane Bomb** — an area-clear mechanic with its own flight animation
  - **Freeze Time** — pauses the level timer behind a full-screen ice shader effect
  - Boosters are **refunded automatically** if the level ends or the player revives while their animation is still playing


- **Fail & Revive Flow**
  - Two distinct fail types: *time is up* and *slots are full*, each with its own second-chance behaviour
  - Reviving after a time-out grants bonus time
  - Reviving with a full bar spawns a luggage vehicle that drives across the runway and spills the collected items back onto the board


- **Feel & Feedback**
  - DOTween-driven sequences everywhere: jump, somersault, squash & stretch, slot bump, punch scale
  - Pooled 3D spatial audio with a voice-stealing limit so overlapping SFX never blow up the audio budget
  - Pooled particle effects per effect type

</section>

<section id="meta-game--hangar--plane-building">

<h2>Meta Game — Hangar & Plane Building</h2>

- **Blueprint-Based Plane Construction**
  - Every plane is a `PlaneBluePrintSo` made of ordered **build stages**, and every stage holds a set of parts
  - Parts are `ScriptableObject`s with two modification types: **Install** (spawn a 3D part into a socket) and **Paint** (swap the part's texture)
  - A skeleton prefab with typed sockets defines exactly where each part is mounted


- **Variations & Live Preview**
  - A part can offer multiple variations (different meshes or paint textures)
  - Selecting a variation previews it **directly on the plane in the scene** before the player confirms
  - Cancelling reverts the preview cleanly — preview and commit are strictly separated in code


- **Progression & Persistence**
  - Wrenches are spent to build; the build progress bar and counters animate as the player advances
  - Save state (active plane, current stage, built parts, completed planes) is serialized to JSON and stored in `PlayerPrefs`
  - Finishing a plane triggers a celebration: the camera switches, the plane docks, taxis to the runway and takes off — then the next blueprint is loaded


- **Economy & Lives**
  - Coins, wrenches, hearts and per-booster inventories in a single resource system with change events
  - Heart refill timer that ticks down in real time, or can be skipped by paying coins
  - Orbital hangar camera (Cinemachine) with drag-to-look and pinch-to-zoom

</section>

<section id="architecture--design-patterns">

<h2>Architecture & Design Patterns</h2>

- **Observer Pattern / Static Event Bus** — `GameEvents` and `LobbyEvents` are the backbone of the project. Managers never reference each other to communicate; they broadcast and react. Every subscription is made in `OnEnable` and released in `OnDisable`, so no listener ever leaks between scenes.


- **Singleton Managers** — long-lived systems (`GameManager`, `EconomyManager`, `SoundManager`, `EffectManager`, `SceneLoaderManager`, …) expose a guarded `Instance` that destroys duplicates on `Awake`.


- **ScriptableObject-Driven Data** — levels, item databases, the audio library, plane blueprints and plane parts all live in assets, not in code. New content can be authored entirely from the Unity Editor without touching a single script.


- **Object Pooling** — `UnityEngine.Pool.ObjectPool<T>` powers both the audio emitters and one dedicated pool per particle effect type. Emitters return themselves to their pool when they finish, and frequent sounds are tracked in a `LinkedList` to enforce a voice-stealing limit.


- **Interface-Based Interaction** — the input layer only knows `IInteractable` (`Select` / `Deselect` / `Interact` / `ReSpawn`). It never knows what an `Item` is, so new interactable object types plug in with zero input-code changes.


- **State Machine** — `GameManager` owns a `GameState` enum (Playing, Paused, LevelCompleted, LevelFailed, GameOver) and is the single authority over `Time.timeScale`.


- **Separation of Concerns** — a consistent layering across both scenes:
  - `Managers` → state, rules and persistence
  - `Controllers` → visuals and animation sequences
  - `UI/Managers` + `UI/Components` → presentation only, driven by events
  - `Mechanics` → self-contained booster & revive behaviors
  - `Static`, `Structs`, `Enums`, `Interfaces` → shared contracts


- **Single Responsibility & Composition** — small, focused `MonoBehaviour`s (`PlaneSocket`, `SafeAreaFitter`, `PropellerSpinner`, card components) composed on prefabs instead of large god-objects. Parameter objects (`SoundData`, `EffectData`, `SavedPartData`, `EffectPoolSetup`) keep call sites readable.


- **Command-Like Booster Flow** — the UI only *requests* a booster, `BoosterManager` validates the cost through `EconomyManager` and dispatches it to the matching mechanic, which then owns its own animation, abort and refund logic.

</section>

<section id="unity-features-used">

<h2>Unity Features Used</h2>

- **Universal Render Pipeline (URP 17)** with separate **Mobile** and **PC** pipeline assets & renderers
- **Shader Graph** — outline, glow, full-screen ice/freeze effect, and a custom plane paint-transition shader animated through `Shader.PropertyToID`-cached properties
- **Cinemachine 3** — `CinemachineCamera` + `CinemachineOrbitalFollow` for the hangar orbit camera, dynamic tracking-target switching for the takeoff sequence
- **New Input System** — pointer-based tap handling, drag & pinch input for the lobby camera
- **Physics** — `Rigidbody` items, `AddExplosionForce` for level scattering, per-frame `AddForce` / `AddTorque` for the wind booster
- **`UnityEngine.Pool`** — built-in object pooling for sound and effect emitters
- **Async Scene Loading** — `LoadSceneAsync` with `allowSceneActivation` control, a fading loading screen and a real progress bar
- **Persistence** — `JsonUtility` + `PlayerPrefs` for hangar, economy and level save data
- **UI** — uGUI, TextMesh Pro, Canvas Groups, a custom **Safe Area Fitter** for notched devices, `UI Particle` for canvas-space VFX
- **3D Spatial Audio** — linear rolloff, follow-target emitters that stick to moving objects (helicopter, plane, vehicle)
- **Editor Tooling** — `[CreateAssetMenu]` authoring assets, `NaughtyAttributes` conditional inspectors, tooltips and headers on every serialized field, plus custom editor helper scripts

</section>

<section id="tech-stack">

<h2>Tech Stack</h2>

**Engine & Language**
- Unity 6 (6000.3.13f1)
- C# / .NET
- Universal Render Pipeline + Shader Graph

**Unity Packages**
- Cinemachine 3
- Input System
- TextMesh Pro / uGUI
- AI Navigation, Timeline

**Third-Party Plugins & Assets**
- DOTween (Demigiant) — all animation sequencing
- NaughtyAttributes — inspector attributes
- UI Particle (ParticleEffectForUGUI) — particles inside Canvas
- Cartoon FX Remaster — VFX library
- Toony Colors Pro — stylized shading
- Low-poly aircraft, hangar and vehicle art packs

**Persistence**
- `PlayerPrefs` + `JsonUtility`

</section>

<section id="project-structure">

<h2>Project Structure</h2>

```
Assets/_Project/Scripts/
├── Managers/          # Game, Level, Goal, Economy, Time, Merge, Input, Sound, Effect, SceneLoader
├── Mechanics/         # Booster & revive mechanics (Hammer, Wind, PlaneBomb, SlotClear)
├── Lobby/
│   ├── Managers/      # PlaneBuild, PlaneSocket, HeartRefill, LobbyInput
│   ├── Controllers/   # PlaneVisual, PlaneBuildPreview
│   ├── ScriptableObjects/
│   ├── UI/            # Lobby UI managers & card components
│   └── Static/        # LobbyEvents
├── UI/                # Gameplay UI managers & components
├── LevelDesign/       # ItemSpawner + LevelDataSo
├── ItemScripts/       # Item, merge data, item database
├── Static/            # GameEvents
├── Structs/ Enums/ Interfaces/ Components/
└── Editor/            # Editor helper tools
```

Both scenes — `LobbyScene` (meta) and `GameScene` (gameplay) — are wired entirely through the event buses.

</section>

<section id="roadmap">

<h2>Roadmap</h2>

This project is still under active development. Planned next steps:

- **More aircraft models** — additional blueprints with their own build stages, parts and paint variations, extending the hangar progression far beyond the current plane set.
- **New mergeable object types** — replacing the current primitive shapes with themed, hand-modelled objects and introducing new merge sets per level theme.
- **Leaderboard** — a global scoring & ranking system so players can compare level times and progression with others.
- **Dependency Injection with VContainer** — migrating away from singletons and static event buses toward constructor-injected, testable dependencies with clearly scoped lifetimes.

</section>
