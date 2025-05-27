using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class StarRaven:Familiar
{
    private List<Func<Task<FamiliarAction>>> actions;
    private int MaxHealth = 40;
    public StarRaven()
    {
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
        };
        var random = new Random();
        this.Name = "StarRaven";
        this.Description = "CAWWWW";
        this.Color = 0x1000cd;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375187492122398801/assets_task_01jvwnzsjvex2srn2tp25bzwh6_1747940621_img_3.webp?ex=68316f67&is=68301de7&hm=4084f349fb3c322008af44558140031caff771e10a9b77e1ef18231f6697b77e&=&format=webp&width=645&height=968";
        this.Power = 4;
        this.Physique = 10;
        
        this.Willpower = 2;
        this.Resolve = 7;
        
        this.Luck = 5;
        
        this.Health = MaxHealth;
        this.Speed = 1;
        this.Cuteness = random.Next(1, 10001);
    }
    
    public override async Task SpecialAbility()
    {
        Console.WriteLine("CAW CAW");
    }
    public override async Task<FamiliarAction> Attack()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "StarRaven Attack";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 5)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }

    public override async Task<int> Defend(FamiliarAction action)
    {
        int damage = -100;
        if (action.DamageType == DamageType.Physical)
        {
            damage = action.Damage - Physique;
        }
        else if (action.DamageType == DamageType.Magical)
        {
            damage = (action.Damage - Resolve);
        }
        if (damage < 1)
        {
            damage = 1;
        }
        return damage;
    }
}