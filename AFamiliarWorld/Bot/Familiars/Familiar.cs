using Discord;

namespace AFamiliarWorld.Bot.Familiars;

public class Familiar
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Avatar { get; set; }
    public uint Color { get; set; }
    public string Url { get; set; }
    public int Power { get; set; }
    public int Physique { get; set; }
    public int Speed { get; set; }
    public int Reflex { get; set; }
    public int Willpower { get; set; }
    public int Resolve { get; set; }
    public int Cuteness { get; set; }
    public int Health  { get; set; }
    public string FamiliarID { get; set; }
    public bool ActiveFamiliar { get; set; } = false;
    
    public Familiar()
    {
        FamiliarID = Guid.NewGuid().ToString();
    }

    public virtual async Task SpecialAbility()
    {
        
    }

    public Embed Display()
    {
        var embed = new EmbedBuilder()
            .WithTitle(Name)
            .WithDescription(Description)
            .WithColor(new Color(Color))
            .WithCurrentTimestamp()
            .WithThumbnailUrl(Url)
            .AddField("Power", Power, inline: true)
            .AddField("Physique", Physique, inline: true)
            .AddField("Speed", Speed, inline: true)
            .AddField("Reflex", Reflex, inline: true)
            .AddField("Willpower", Willpower, inline: true)
            .AddField("Resolve", Resolve, inline: true)
            .AddField("Health", Health, inline: true)
            .Build(); // Build returns a Discord.Embed object
        return embed;
    }
}