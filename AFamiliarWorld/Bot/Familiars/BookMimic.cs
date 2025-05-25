namespace AFamiliarWorld.Bot.Familiars;

public class BookMimic:Familiar
{
    public BookMimic()
    {
        var random = new Random();
        this.Name = "BookMimic";
        this.Description = "Read me pls c:<";
        this.Color = 0xa7743e;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375188921834668102/assets_task_01jvwpbm43fc7r95023q0ca9ax_1747940954_img_3.webp?ex=683170bc&is=68301f3c&hm=e8e705ab8ca5f2c7eddb82de4b7d355be1330eebe9627a83a34facf63ef911f2&=&format=webp&width=645&height=968";
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
        Console.WriteLine("*Bites u*");
    }
}