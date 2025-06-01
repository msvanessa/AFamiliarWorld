namespace AFamiliarWorld.Bot.Commands.Models;

public class StatisticLogger
{
    public int Battles { get; set; } = 0;
    public List<string> winnerFamiliars { get; set; } = new List<string>();
    public List<string> loserFamiliars { get; set; } = new List<string>();
    public List<string> FamiliarsWonByReflectedDamage { get; set; } = new List<string>();
    public List<string> FamiliarsWonByStatusCondition { get; set; } = new List<string>();
    public List<string> FamiliarsWonByNormalDamage { get; set; } = new List<string>(); 
    public List<string> FamiliarsLostByReflectedDamage { get; set; } = new List<string>(); 
    public List<string> FamiliarsLostByStatusCondition { get; set; } = new List<string>(); 
    public List<string> FamiliarsLostByNormalDamage { get; set; } = new List<string>();
}