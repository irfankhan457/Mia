namespace MiaFantasyRun.Runtime
{
    public enum GameMode
    {
        Endless,
        Mission,
        DailyChallenge,
        Event,
        BossEscape
    }

    public enum RunState
    {
        Menu,
        Loading,
        Ready,
        Running,
        Paused,
        Reviving,
        GameOver
    }

    public sealed class GameState
    {
        public GameMode Mode { get; private set; } = GameMode.Endless;
        public RunState RunState { get; private set; } = RunState.Menu;
        public int Score { get; private set; }
        public int Coins { get; private set; }
        public int Gems { get; private set; }
        public float DistanceMeters { get; private set; }

        public void Begin(GameMode mode)
        {
            Mode = mode;
            RunState = RunState.Running;
            Score = 0;
            Coins = 0;
            Gems = 0;
            DistanceMeters = 0f;
        }

        public void AddDistance(float meters)
        {
            DistanceMeters += meters;
            Score = (int)(DistanceMeters * 10f) + Coins + Gems * 5;
        }

        public void AddCoins(int amount) => Coins += amount;
        public void AddGems(int amount) => Gems += amount;
        public void Pause() => RunState = RunState.Paused;
        public void Resume() => RunState = RunState.Running;
        public void Finish() => RunState = RunState.GameOver;
    }
}
