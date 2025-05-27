using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Pebblewyrm:Familiar
{
    private List<Func<Task<FamiliarAction>>> actions;
    private int MaxHealth = 40;
    public Pebblewyrm()
    {
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
        };
        var random = new Random();
        this.Name = "Pebblewyrm";
        this.Description = "Rawr!";
        this.Color = 0x808080;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375190944441307277/assets_task_01jvwprr6cfwt8zdrwh2f6td02_1747941436_img_0.webp?ex=6831729e&is=6830211e&hm=63b02e99830de26dbf0f6aa5a6b922c102cc17e1ec78721aa0db4a9b5454f9c1&=&format=webp&width=645&height=968";
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
        Console.WriteLine("*Breathes gravel*");
    }
    public override async Task<FamiliarAction> Attack()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "Pebblewyrm Attack";
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