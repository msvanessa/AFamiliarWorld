namespace AFamiliarWorld.Bot.Familiars;

public class Pebblewyrm:Familiar
{
    public Pebblewyrm()
    {
        var random = new Random();
        this.Name = "Pebblewyrm";
        this.Description = "Rawr!";
        this.Color = 0x808080;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375190944441307277/assets_task_01jvwprr6cfwt8zdrwh2f6td02_1747941436_img_0.webp?ex=6831729e&is=6830211e&hm=63b02e99830de26dbf0f6aa5a6b922c102cc17e1ec78721aa0db4a9b5454f9c1&=&format=webp&width=645&height=968";
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
        Console.WriteLine("*Breathes gravel*");
    }
}