namespace MiaFantasyRun.Services
{
    public interface ISaveService
    {
        void Initialize();
        SaveData Load();
        void Save(SaveData data);
        void Delete();
    }
}
