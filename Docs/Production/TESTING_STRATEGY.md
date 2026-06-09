# Testing Strategy

## Editor Tests

- Save data serialization and signature verification.
- Mission progress completion.
- Daily reward claim rules.
- Shop purchase currency checks.
- Character and pet selection.

## Play Mode Tests

- Swipe left and right lane limits.
- Jump and double jump limits.
- Slide collider height restore.
- Collectable pickup updates run state.
- Obstacle hit triggers game over.
- Power-up duration reset.

## Device Tests

- Low-end Android 10 phone.
- Mid-range Android 12 or 13 phone.
- High refresh-rate Android phone.
- Tablet form factor.

## Performance Gates

- 60 FPS target during normal running.
- No major GC spikes during repeated obstacle spawning.
- Memory remains stable after 20 minutes.
- Scene loading uses Addressables and loading UI.
- Battery usage is acceptable over 15 minute sessions.

## Release Validation

- Fresh install.
- Upgrade install over previous build.
- Offline run.
- Airplane mode launch.
- Ad unavailable flow.
- Firebase disabled/failure flow.
- Google Play sign-in cancel flow.
- Save tamper detection.
