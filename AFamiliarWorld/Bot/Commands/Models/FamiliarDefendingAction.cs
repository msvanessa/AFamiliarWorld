namespace AFamiliarWorld.Bot.Commands.Models;

public class FamiliarDefendingAction
{
    public int DamageTaken { get; set; }
    public int DamageReflected { get; set; }
    public string? DamageReflectedMessage { get; set; }
    public bool IsReflecting { get; set; } = false;
}