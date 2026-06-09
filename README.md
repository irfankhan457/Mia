# Mia's Fantasy Run

Unity 6 Android endless-runner scaffold for a family-friendly fantasy game.

This repository contains a production-oriented Unity layout, gameplay systems, service interfaces, monetization/analytics stubs, ScriptableObject data architecture, prefab folders, scene setup tooling, and Google Play release documentation.

## Target

- Unity 6
- Android 10 minimum SDK
- Latest Android target SDK through Unity settings
- URP-ready project structure
- Addressables-ready content layout
- Firebase Analytics and Crashlytics integration points
- AdMob and Google Play Games integration points

## Open In Unity

1. Open Unity Hub.
2. Add this folder as an existing project:
   `C:\Users\irfan\OneDrive\Documents\Mia`
3. Use Unity 6.
4. Let Unity resolve packages from `Packages/manifest.json`.
5. In Unity, run `Mia Fantasy Run > Create Starter Scene`.

## Main Folders

- `Assets/_MiaFantasyRun/Scripts`: Clean gameplay, data, UI, and service code.
- `Assets/_MiaFantasyRun/ScriptableObjects`: Character, pet, world, mission, and power-up data.
- `Assets/_MiaFantasyRun/Prefabs`: Characters, pets, obstacles, collectables, track segments, and UI prefabs.
- `Assets/_MiaFantasyRun/Addressables`: Future remote/loadable content groups.
- `Docs`: Store, build, deployment, asset, and testing documentation.

## Current Scope

The project is a complete implementation foundation, not a finished 3D art package. Unity-specific binary assets, final animations, final audio, Firebase configuration files, AdMob production IDs, Google Play Games IDs, and store screenshots must be added in Unity before release.
