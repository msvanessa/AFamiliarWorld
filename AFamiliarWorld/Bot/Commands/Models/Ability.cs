namespace AFamiliarWorld.Bot.Commands.Models;

public class Ability(string title, string value)
{
    public string Title { get; set; } = title;
    public string Value { get; set; } = value;
}