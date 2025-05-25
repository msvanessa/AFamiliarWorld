namespace AFamiliarWorld.Bot.Familiars;

public class Batnana:Familiar
{
    public Batnana()
    {
        var random = new Random();
        this.Name = "Batnana";
        this.Description = "Monch";
        this.Color = 0xfff940;
        this.Url = "https://images-ext-1.discordapp.net/external/hXFeRnzIiH3xoipky40Dc43yl1Cp74oZ6-I-kDrlyJU/%3Fst%3D2025-05-22T19%253A01%253A17Z%26se%3D2025-05-28T20%253A01%253A17Z%26sks%3Db%26skt%3D2025-05-22T19%253A01%253A17Z%26ske%3D2025-05-28T20%253A01%253A17Z%26sktid%3Da48cca56-e6da-484e-a814-9c849652bcb3%26skoid%3D8ebb0df1-a278-4e2e-9c20-f2d373479b3a%26skv%3D2019-02-02%26sv%3D2018-11-09%26sr%3Db%26sp%3Dr%26spr%3Dhttps%252Chttp%26sig%3DFbAjfUp4i8o%252FLU5%252F6FRJUmWwy%252B%252F5oveMN%252BSz5EUCWPs%253D%26az%3Doaivgprodscus/https/videos.openai.com/vg-assets/assets%252Ftask_01jvwsv4a4fztrxx68g5mkxfdj%252F1747944610_img_3.webp?format=webp&width=645&height=968";
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
        Console.WriteLine("I monch 'nana");
    }
}