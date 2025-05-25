namespace AFamiliarWorld.Bot.Familiars;

public class LandShork:Familiar
{
    public LandShork()
    {
        var random = new Random();
        this.Name = "LandShork";
        this.Description = "My name jef";
        this.Color = 0x1dd3df;
        this.Url = "https://images-ext-1.discordapp.net/external/OmhTzBTivkzoP91kdb1_Dj5jy2ihSsrwRSd0ta8_bBo/%3Fst%3D2025-05-22T18%253A01%253A36Z%26se%3D2025-05-28T19%253A01%253A36Z%26sks%3Db%26skt%3D2025-05-22T18%253A01%253A36Z%26ske%3D2025-05-28T19%253A01%253A36Z%26sktid%3Da48cca56-e6da-484e-a814-9c849652bcb3%26skoid%3D8ebb0df1-a278-4e2e-9c20-f2d373479b3a%26skv%3D2019-02-02%26sv%3D2018-11-09%26sr%3Db%26sp%3Dr%26spr%3Dhttps%252Chttp%26sig%3Dm%252Bn%252BGiME%252BM5zq7cNYxwjHcm3F69NgrgoALnVoGduC90%253D%26az%3Doaivgprodscus/https/videos.openai.com/vg-assets/assets%252Ftask_01jvwrrcrdf15r90xpwzknnd52%252F1747943505_img_2.webp?format=webp&width=645&height=968";
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
        Console.WriteLine("*Monches u*");
    }
}