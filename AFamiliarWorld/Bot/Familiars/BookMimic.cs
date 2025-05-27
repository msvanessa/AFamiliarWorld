using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class BookMimic:Familiar
{
    private List<Func<Task<FamiliarAction>>> actions;
    private int MaxHealth = 40;
    public BookMimic()
    {
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
        };
        var random = new Random();
        this.Name = "BookMimic";
        this.Description = "Read me pls c:<";
        this.Color = 0xa7743e;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375188921834668102/assets_task_01jvwpbm43fc7r95023q0ca9ax_1747940954_img_3.webp?ex=683170bc&is=68301f3c&hm=e8e705ab8ca5f2c7eddb82de4b7d355be1330eebe9627a83a34facf63ef911f2&=&format=webp&width=645&height=968";
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
        Console.WriteLine("*Bites u*");
    }
    public override async Task<FamiliarAction> Attack()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "Bite";
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