using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class PaperCraneGolem:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    public PaperCraneGolem()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            PapercutBarrage,
            GuillotineFold,
            ConfettiBurst
        };
        var random = new Random();
        this.Name = "Paper Crane Golem";
        this.Description = "Beep boop";
        this.Quip = "*Does the robot*";
        this.Emoji = "<:FamiliarPaperCraneGolem:1378165432602722346>";
        this.Color = 0xb66d34;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375184022317039677/assets_task_01jvwn8e9bejabs40h4cag97ft_1747939803_img_1.webp?ex=68316c2c&is=68301aac&hm=fd421e630543b8d6e3601d4223cd4070d7e4a7083c579df74e450c0ac1cddcbc&";
        this.Power = 30;
        this.Physique = 20;
        
        this.Willpower = 30;
        this.Resolve = 10;
        
        this.Luck = 5;

        this.MaxHealth = 350;
        this.Health = this.MaxHealth;
        
        this.Speed = 7;
        this.Cuteness = random.Next(1, 10001);
    }
    public override async Task<FamiliarAttackingAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
    }
    public async Task<FamiliarAttackingAction> PapercutBarrage()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Papercut Barrage";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 21) + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }
    
    public async Task<FamiliarAttackingAction> GuillotineFold()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Guillotine Fold";
        action.Damage = random.Next(0, 101) == 1 ? 9999 : 0;
        action.IsTrueDamage = true;
        action.DamageType = DamageType.Physical;
        return action;
    }
    
    public async Task<FamiliarAttackingAction> ConfettiBurst()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Confetti Burst";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Willpower + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Magical;
        action.StatusConditions = new List<StatusCondition>() { StatusCondition.Confuse};
        return action;
    }
}