# Scene Hierarchy

## Boot Scene

- `GameBootstrapper`
  - `SaveManager`
  - `AudioManager`
  - `AnalyticsManager`
  - `AdsManager`
  - `GooglePlayGamesManager`
- `LoadingCanvas`
- `AddressablesWarmup`

## Main Menu Scene

- `MainCamera`
- `DirectionalLight`
- `UIManager`
- `HomeScreen`
- `CharacterSelectionScreen`
- `PetSelectionScreen`
- `ShopScreen`
- `AchievementsScreen`
- `DailyRewardsScreen`
- `EventsScreen`
- `SettingsScreen`
- `LeaderboardScreen`
- `ProfileScreen`
- `MenuCharacterPreview`

## Run Scene

- `RunnerGameManager`
- `SwipeInput`
- `PlayerRoot`
  - `PlayerController`
  - `CharacterModel`
  - `Animator`
  - `PowerUpController`
- `PetFollower`
- `TrackSegmentSpawner`
- `ObstacleManager`
- `CollectableManager`
- `WeatherManager`
- `BossManager`
- `RunCamera`
- `RunHUD`
- `PauseMenu`
- `GameOverScreen`
- `ObjectPools`
  - `TrackSegmentPools`
  - `ObstaclePools`
  - `CollectablePools`
  - `VfxPools`

## Starter Scene Tool

Use the Unity menu item:

`Mia Fantasy Run > Create Starter Scene`

This creates `Assets/_MiaFantasyRun/Scenes/Main.unity` with the first required manager objects.
