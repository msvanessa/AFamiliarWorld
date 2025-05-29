using Newtonsoft.Json;
using AFamiliarWorld.Bot.Commands.Models;
using Discord;

namespace AFamiliarWorld.Bot.Familiars;

public class Familiar
{
    public string Name { get; set; }
    [JsonIgnore] 
    public string Description { get; set; }
    [JsonIgnore] 
    public string Quip { get; set; }
    [JsonIgnore] 
    public uint Color { get; set; }
    [JsonIgnore] 
    public string Url { get; set; }
    public int Power { get; set; }
    public int Physique { get; set; }
    public int Speed { get; set; }
    public int Willpower { get; set; }
    public int Resolve { get; set; }
    public int Cuteness { get; set; }
    public int Health  { get; set; }
    public int Luck { get; set; }
    public string FamiliarID { get; set; }
    public bool ActiveFamiliar { get; set; } = false;
    
    private List<StatusCondition> StatusConditions = new List<StatusCondition>();

    [JsonIgnore] 
    public List<Ability> Abilities { get; set; } = new List<Ability>();
    
    public Familiar()
    {
        FamiliarID = Guid.NewGuid().ToString();
    }
    public async Task<List<Ability>> GetAbilities()
    {
        return Abilities;
    }

    public async Task<List <StatusCondition>> GetStatusConditions()
    {
        return StatusConditions;
    }

    public async Task ClearStatusConditions()
    {
        this.StatusConditions.Clear();
    }
    public async Task RemoveStatusCondition(StatusCondition condition)
    {
        if (StatusConditions.Contains(condition))
        {
            StatusConditions.Remove(condition);
        }
    }
    
    public async Task AddStatusCondition(StatusCondition condition)
    {
        if (!StatusConditions.Contains(condition) || condition == StatusCondition.Poison)
        {
            StatusConditions.Add(condition);
        }
    }

    public Embed Display()
    {
        string cutie = "Blargh! I'm a cute familiar!";
        if (Cuteness < 1001)
        {
            cutie = "Repulsive";
        }
        else if (Cuteness < 2001)
        {
            cutie = "Hideous";
        }
        else if (Cuteness < 3001)
        {
            cutie = "Ugly";
        }
        else if (Cuteness < 4001)
        {
            cutie = "Unattractive";
        }
        else if (Cuteness < 5001)
        {
            cutie = "Plain";
        }
        else if (Cuteness < 6001)
        {
            cutie = "Cute-ish";
        }
        else if (Cuteness < 7001)
        {
            cutie = "Cute";
        }
        else if (Cuteness < 8001)
        {
            cutie = "Pretty";
        }
        else if (Cuteness < 9001)
        {
            cutie = "Gorgeous";
        }
        else if (Cuteness < 10001)
        {
            cutie = "Beautiful";
        }

        var embed = new EmbedBuilder()
            .WithTitle(cutie + " " + Name)
            .WithDescription(Description)
            .WithColor(new Color(Color))
            .WithThumbnailUrl(Url)
            .AddField("Power", Power, inline: true)
            .AddField("Physique", Physique, inline: true)
            .AddField("Willpower", Willpower, inline: true)
            .AddField("Resolve", Resolve, inline: true)
            .AddField("Luck", Luck, inline: true)
            .AddField("Health", Health, inline: true)
            .AddField("Speed", Speed, inline: true)
            .WithFooter(Quip);
        foreach (var ability in this.Abilities)
        {
            embed.WithFields().AddField(ability.Title, ability.Value);
        }
        return embed.Build();
    }

    public virtual async Task<FamiliarAttackingAction> Attack()
    {
        return null;
    }

    public virtual async Task<FamiliarDefendingAction> Defend(FamiliarAttackingAction attackingAction)
    {
        if (attackingAction.StatusConditions != null)
        {
            foreach (var statusCondition in attackingAction.StatusConditions)
            {
                await this.AddStatusCondition(statusCondition);
            }
        }

        var defendingAction = new FamiliarDefendingAction();
        if (attackingAction.IsTrueDamage)
        {
            defendingAction.DamageTaken = attackingAction.Damage;
        }
        else
        {
            if (attackingAction.DamageType == DamageType.Physical)
            {
                defendingAction.DamageTaken = attackingAction.Damage - this.Physique;
            }
            else if (attackingAction.DamageType == DamageType.Magical)
            {
                defendingAction.DamageTaken = (attackingAction.Damage - Resolve);
            }
        }

        return defendingAction;
    }
}
