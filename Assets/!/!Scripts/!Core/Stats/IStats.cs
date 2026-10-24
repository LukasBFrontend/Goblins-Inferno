public interface IStats
{
    Stat GetStat(string statName);
    Stat[] All();
}
