using System.Collections.Generic;

public interface IStats
{
    Stat GetStat(string statName);
    Stat[] All();
}
