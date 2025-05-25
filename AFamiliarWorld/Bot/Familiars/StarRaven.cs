namespace AFamiliarWorld.Bot.Familiars;

public class StarRaven:Familiar
{
    public StarRaven()
    {
        var random = new Random();
        this.Name = "StarRaven";
        this.Description = "CAWWWW";
        this.Color = 0x1000cd;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375187492122398801/assets_task_01jvwnzsjvex2srn2tp25bzwh6_1747940621_img_3.webp?ex=68316f67&is=68301de7&hm=4084f349fb3c322008af44558140031caff771e10a9b77e1ef18231f6697b77e&=&format=webp&width=645&height=968";
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
        Console.WriteLine("CAW CAW");
    }
}