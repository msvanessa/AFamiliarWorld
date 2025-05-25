namespace AFamiliarWorld.Bot.Familiars;

public class NoodleCrab:Familiar
{
    public NoodleCrab()
    {
        var random = new Random();
        this.Name = "NoodleCrab";
        this.Description = "If u take my noodles I kil u :)";
        this.Color = 0x781ddf;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375194682270679240/assets_task_01jvwqk2n5f36b9zctwceetbr7_1747942313_img_2.webp?ex=6831761a&is=6830249a&hm=a019251cda1c47e2da34472283e85f4fcba4ebf9b0c58d5e614c8b724e68e727&=&format=webp&width=645&height=968";
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
        Console.WriteLine("CRABRAVE");
    }
}