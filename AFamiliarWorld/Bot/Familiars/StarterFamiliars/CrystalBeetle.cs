namespace AFamiliarWorld.Bot.Familiars;

public class CrystalBeetle:Familiar
{
    public CrystalBeetle()
    {
        var random = new Random();
        this.Name = "Crystal Beetle";
        this.Description = "...";
        this.Color = 0x34e9d0;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375184355504423063/assets_task_01jvwnaxt0eyzbc2bbb853pmxf_1747939882_img_0.webp?ex=68316c7c&is=68301afc&hm=66a1087f14c1e0cbe2c431e9cff617e260f809df7e5a73b86f88e566330f0e3a&";
        this.Power = 72;
        this.Physique = 78;
        this.Speed = 19;
        this.Reflex = 23;
        this.Willpower = 48;
        this.Resolve = 60;
        this.Health = 100;
        this.Cuteness = Math.Min(random.Next(1, 10001), random.Next(1, 10001));
    }
    
    public override async Task SpecialAbility()
    {
        Console.WriteLine("*Hunkers down*");
    }
}