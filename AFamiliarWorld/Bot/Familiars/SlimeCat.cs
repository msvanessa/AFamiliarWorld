using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class SlimeCat:Familiar
{
    private List<Func<Task<FamiliarAction>>> actions;
    private int MaxHealth = 40;
    public SlimeCat()
    {
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
        };
        var random = new Random();
        this.Name = "SlimeCat";
        this.Description = "Mlem :3";
        this.Color = 0x0ccd00;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375193634525286622/assets_task_01jvwqea7zfdyv09ke1hzw2dfn_1747942093_img_0.webp?ex=68317520&is=683023a0&hm=990b40abd7341d042ca918904c5663f054e4c0fe2150229c7bf4d17f8bd5753e&=&format=webp&width=645&height=968";
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
        Console.WriteLine("*Coughs up treasure*");
    }
    public override async Task<FamiliarAction> Attack()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "SlimeCat Attack";
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