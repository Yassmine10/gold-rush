/// <summary>Valeurs de référence pour garder les objectifs identiques entre menus et niveaux.</summary>
public static class LevelBalance
{
    public static int GetMinimumCoins(int level)
    {
        return level switch
        {
            1 => 8,
            2 => 10,
            3 => 12,
            _ => 8
        };
    }

    public static int GetMaximumCoins(int level)
    {
        return level switch
        {
            1 => 26,
            2 => 30,
            3 => 24,
            _ => 26
        };
    }

    public static string GetDifficulty(int level)
    {
        return level switch
        {
            1 => "FACILE",
            2 => "MOYEN",
            3 => "DIFFICILE",
            _ => "FACILE"
        };
    }
}
