namespace AFamiliarWorld.Bot.Familiars;

public class PaperCraneGolem:Familiar
{
    public PaperCraneGolem()
    {
        var random = new Random();
        this.Name = "Paper Crane Golem";
        this.Description = "Beep boop";
        this.Color = 0xb66d34;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375184022317039677/assets_task_01jvwn8e9bejabs40h4cag97ft_1747939803_img_1.webp?ex=68316c2c&is=68301aac&hm=fd421e630543b8d6e3601d4223cd4070d7e4a7083c579df74e450c0ac1cddcbc&";
        this.Power = 60;
        this.Physique = 48;
        this.Speed = 72;
        this.Reflex = 78;
        this.Willpower = 19;
        this.Resolve = 23;
        this.Health = 250;
        this.Cuteness = random.Next(1, 10001);
    }
    
    public override async Task SpecialAbility()
    {
        Console.WriteLine("*Does the robot*");
    }
}