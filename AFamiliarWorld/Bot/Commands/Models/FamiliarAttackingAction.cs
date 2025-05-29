namespace AFamiliarWorld.Bot.Commands.Models;

public class FamiliarAttackingAction
{
    public int Damage { get; set; }
    public DamageType DamageType { get; set; }
    public string AbilityName { get; set; }
    public bool CriticalHit { get; set; } = false;
    public List<StatusCondition>? StatusConditions { get; set; }
    public bool IsTrueDamage { get; set; } = false;
}