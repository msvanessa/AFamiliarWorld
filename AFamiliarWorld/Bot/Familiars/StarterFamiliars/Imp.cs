using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Imp:Familiar
{
    private int MaxHealth = 40;
    private List<Func<Task<FamiliarAction>>> actions;
    public Imp()
    {
        
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
            Fireball,
            Scratch
        };
        var random = new Random();
        this.Name = "Imp";
        this.Description = "Hehe grumble grumble";
        this.Quip = "*angy*";
        this.Color = 0xff0000;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375183010936258620/assets_task_01jvwmzenxe9t9j2et7ft2ggqs_1747939506_img_1.webp?ex=68316b3b&is=683019bb&hm=0eebbb630ed6711395d42981d2b4fbdefc50ffd535b8720dbc5a16f9a4ce9c94&";
        this.Power = 8;
        this.Physique = 4;
        
        this.Willpower = 8;
        this.Resolve = 4;
        
        this.Luck = 5;
        
        this.Health = MaxHealth;
        this.Speed = 1;
        this.Cuteness = random.Next(1, 10001);
    }

    public async Task<FamiliarAction> Fireball()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "Imp Firebol";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 5)) * (crit);
        action.DamageType = DamageType.Magical;
        return action;
    }
    
    public async Task<FamiliarAction> Scratch()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "Imp Scratch";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 5)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }
    public override async Task SpecialAbility()
    {
        Console.WriteLine("FIREBOL");
    }
    public override async Task<FamiliarAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
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