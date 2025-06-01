using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class StarRaven:Familiar
{
    private List<Func<Familiar, Task<FamiliarAttackingAction>>> actions;
    public StarRaven()
    {
        this.actions = new List<Func<Familiar, Task<FamiliarAttackingAction>>>
        {
            Peck,
            EnvelopingVoid,
            ColorSpray,
            Starburn,
            GuidingStar
        };
        var random = new Random();
        this.Name = "StarRaven";
        this.Description = "CAWWWW";
        this.Quip = "*Flaps wings*";
        this.Emoji = "<:FamiliarStarRaven:1378164229630070895>";
        this.Color = 0x1000cd;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375187492122398801/assets_task_01jvwnzsjvex2srn2tp25bzwh6_1747940621_img_3.webp?ex=68316f67&is=68301de7&hm=4084f349fb3c322008af44558140031caff771e10a9b77e1ef18231f6697b77e&=&format=webp&width=645&height=968";
        this.Power = 25;
        this.Physique = 15;
        
        this.Willpower = 30;
        this.Resolve = 15;
        
        this.Luck = 5;

        this.MaxHealth = 300;
        this.Health = this.MaxHealth;
        
        this.Speed = 11;
        this.Cuteness = random.Next(1, 10001);
    }
    public override async Task<FamiliarAttackingAction> Attack(Familiar enemyFamiliar)
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke(enemyFamiliar);
    }
    public async Task<FamiliarAttackingAction> Peck(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Peck";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        // Should reduce enemy's physique by 2
        action.Damage = (Power + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }

    public async Task<FamiliarAttackingAction> EnvelopingVoid(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Enveloping Void";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        // Should reduce enemy's resolve by  2
        action.Damage = (Willpower + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Magical;
        return action;
    }
    
    public async Task<FamiliarAttackingAction> ColorSpray(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Color Spray";
        action.Damage = 0;
        action.StatusConditions = new List<StatusCondition>() { StatusCondition.Confuse, StatusCondition.Stun };
        action.DamageType = DamageType.Magical;
        return action;
    }
    
    public async Task<FamiliarAttackingAction> Starburn(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Starburn";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Willpower + random.Next(1, 21)) * (crit);
        action.StatusConditions = new List<StatusCondition>() { StatusCondition.Burn };
        action.DamageType = DamageType.Magical;
        return action;
    }
    
    public async Task<FamiliarAttackingAction> GuidingStar(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Guiding Star";
        action.Damage = 0;
        action.IsTrueDamage = true;
        action.DamageType = DamageType.Magical;
        this.Willpower += 10;
        this.Power += 10;
        this.Health += 30;
        if ((await this.GetStatusConditions()).Contains(StatusCondition.Bleed))
        {
            this.Health -= 15;
        }
        return action;
    }
}