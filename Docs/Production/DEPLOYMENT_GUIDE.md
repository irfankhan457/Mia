# Deployment Guide

## Internal Test Track

1. Create the app in Google Play Console.
2. Complete app content declarations.
3. Upload the first signed `.aab`.
4. Add internal testers.
5. Verify install, launch, first run, ad behavior, sign-in, leaderboard submission, save restore, and crash reporting.

## Pre-Launch Checklist

- No placeholder app icon.
- No test AdMob unit IDs in production builds.
- Firebase dashboard receives analytics.
- Crashlytics receives test crash.
- Leaderboards submit sandbox scores.
- Rewarded ads are opt-in.
- Interstitials do not appear during active input.
- No pay-to-win upgrades.
- Privacy policy URL is live.
- Terms of service URL is live.
- Age rating questionnaire completed honestly.

## Rollout Plan

- Internal testing: 20 to 50 devices.
- Closed testing: 500 to 1,000 installs.
- Open testing: monitor retention, crash-free sessions, ad fill, ANR rate.
- Production rollout: 5 percent, then 25 percent, then 50 percent, then 100 percent if stable.
