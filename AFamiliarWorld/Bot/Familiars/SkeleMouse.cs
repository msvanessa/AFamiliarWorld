namespace AFamiliarWorld.Bot.Familiars;

public class SkeleMouse:Familiar
{
    public SkeleMouse()
    {
        var random = new Random();
        this.Name = "SkeleMouse";
        this.Description = "Yip yip!";
        this.Color = 0xc7bdbd;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375185078405173270/assets_task_01jvwng9ypectvwtxbmwcqyx94_1747940061_img_1.webp?ex=68316d28&is=68301ba8&hm=4fb5f179c149b3d623b72afff86a558ab9bed01dba98f24aed7512bb8e6a1d7b&=&format=webp&width=645&height=968";
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
        Console.WriteLine("*Brings u smol piece of cheese*");
    }
}