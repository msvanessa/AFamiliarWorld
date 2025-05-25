namespace AFamiliarWorld.Bot.Familiars;

public class Clockroach:Familiar
{
    public Clockroach()
    {
        var random = new Random();
        this.Name = "Clockroach";
        this.Description = "I clock, I roach, I clockroach";
        this.Color = 0xa75600;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375190322996580412/assets_task_01jvwpn0mqff4vf7b0de9s151x_1747941299_img_1.webp?ex=6831720a&is=6830208a&hm=2ebf5e6cba5c4784f875f7398494e1a82e1269342cc85746db8f9280230d3407&=&format=webp&width=645&height=968";
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
        Console.WriteLine("*Winds you up*");
    }
}