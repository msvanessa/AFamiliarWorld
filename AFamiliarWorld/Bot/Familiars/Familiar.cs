using AFamiliarWorld.Bot.Commands.Models;
using Discord;

namespace AFamiliarWorld.Bot.Familiars;

public class Familiar
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Avatar { get; set; }
    public string Quip { get; set; }
    public uint Color { get; set; }
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
    
    public Familiar()
    {
        FamiliarID = Guid.NewGuid().ToString();
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
            .WithFooter(Quip)
            .Build(); // Build returns a Discord.Embed object
        return embed;
    }

    public virtual async Task<FamiliarAction> Attack()
    {
        return null;
    }

    public virtual async Task<int> Defend(FamiliarAction action)
    {
        return -1;
    }
}