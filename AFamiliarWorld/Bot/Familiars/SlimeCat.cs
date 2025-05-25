namespace AFamiliarWorld.Bot.Familiars;

public class SlimeCat:Familiar
{
    public SlimeCat()
    {
        var random = new Random();
        this.Name = "SlimeCat";
        this.Description = "Mlem :3";
        this.Color = 0x0ccd00;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375193634525286622/assets_task_01jvwqea7zfdyv09ke1hzw2dfn_1747942093_img_0.webp?ex=68317520&is=683023a0&hm=990b40abd7341d042ca918904c5663f054e4c0fe2150229c7bf4d17f8bd5753e&=&format=webp&width=645&height=968";
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
        Console.WriteLine("*Coughs up treasure*");
    }
}