namespace AFamiliarWorld.Bot.Commands.Models;

public class FamiliarAction
{
    public int Damage { get; set; }
    public DamageType DamageType { get; set; }
    public string AbilityName { get; set; }
    public bool CriticalHit { get; set; } = false;
}