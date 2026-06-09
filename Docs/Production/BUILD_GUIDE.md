# Android Build Guide

## Unity Settings

- Unity version: Unity 6.
- Platform: Android.
- Minimum API Level: Android 10, API 29.
- Target API Level: highest installed stable SDK.
- Scripting Backend: IL2CPP.
- Target Architectures: ARMv7 and ARM64 for testing, ARM64 required for release.
- Managed Stripping Level: Medium.
- Input Handling: Both old and new input during transition.
- Orientation: Portrait.

## Required Before First Release Build

1. Import Firebase Unity SDK.
2. Add `google-services.json` to `Assets`.
3. Import Google Mobile Ads Unity plugin.
4. Replace test AdMob IDs in `AdsManager`.
5. Import Google Play Games plugin.
6. Replace placeholder leaderboard IDs in `GooglePlayGamesManager`.
7. Configure URP asset and assign it in graphics settings.
8. Build all production ScriptableObjects and assign catalog references.
9. Create final scenes and add them to Build Settings.
10. Generate signed Android App Bundle.

## Build Steps

1. Open `File > Build Profiles`.
2. Select Android.
3. Enable `Build App Bundle`.
4. Use a release keystore.
5. Build to `Builds/Android/MiasFantasyRun.aab`.
6. Upload to an internal testing track first.

## Release Defines

Add scripting define symbols when the matching SDKs are installed:

- `FIREBASE_ANALYTICS`
- `FIREBASE_CRASHLYTICS`
- `GOOGLE_PLAY_GAMES`
- `ADMOB_PRODUCTION`
