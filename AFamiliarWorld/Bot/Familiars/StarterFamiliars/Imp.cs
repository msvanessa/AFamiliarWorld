namespace AFamiliarWorld.Bot.Familiars;

public class Imp:Familiar
{
    public Imp()
    {
        var random = new Random();
        this.Name = "Imp";
        this.Description = "Hehe grumble grumble";
        this.Color = 0xff0000;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375183010936258620/assets_task_01jvwmzenxe9t9j2et7ft2ggqs_1747939506_img_1.webp?ex=68316b3b&is=683019bb&hm=0eebbb630ed6711395d42981d2b4fbdefc50ffd535b8720dbc5a16f9a4ce9c94&";
        this.Power = 19;
        this.Physique = 23;
        this.Speed = 48;
        this.Reflex = 60;
        this.Willpower = 78;
        this.Resolve = 72;
        this.Health = 250;
        this.Cuteness = random.Next(1, 10001);
    }
    
    public override async Task SpecialAbility()
    {
        Console.WriteLine("FIREBOL");
    }
}