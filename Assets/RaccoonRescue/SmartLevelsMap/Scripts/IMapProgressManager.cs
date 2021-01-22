public interface IMapProgressManager
{
    int LoadLevelStarsCount(int level);
    void SaveLevelStarsCount(int level, int starsCount);
    void ClearLevelProgress(int level);
}
