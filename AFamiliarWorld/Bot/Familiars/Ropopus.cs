namespace AFamiliarWorld.Bot.Familiars;

public class Ropopus:Familiar
{
    public Ropopus()
    {
        var random = new Random();
        this.Name = "Ropopus";
        this.Description = "._.";
        this.Color = 0x925252;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375198791673581618/assets_task_01jvwrgw9ze34t1wb04gj76wmw_1747943275_img_0.webp?ex=683179ed&is=6830286d&hm=d4160412efe492d4441bf076d19bd064abf59fcadf1e8694472911a966430cc3&=&format=webp&width=645&height=968";
        this.Power = 60;
        this.Physique = 0;
        this.Speed = 0;
        this.Reflex = 0;
        this.Willpower = 0;
        this.Resolve = 0;
        this.Health = 250;
        this.Cuteness = random.Next(1, 10001);
    }
    
    public override async Task SpecialAbility()
    {
        Console.WriteLine("*Wraps around u*");
    }
}