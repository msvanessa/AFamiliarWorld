using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class PaperCraneGolem:Familiar
{
    private int MaxHealth = 40;
    private List<Func<Task<FamiliarAction>>> actions;
    public PaperCraneGolem()
    {
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
            PaperCranePunch
        };
        var random = new Random();
        this.Name = "Paper Crane Golem";
        this.Description = "Beep boop";
        this.Quip = "*Does the robot*";
        this.Color = 0xb66d34;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375184022317039677/assets_task_01jvwn8e9bejabs40h4cag97ft_1747939803_img_1.webp?ex=68316c2c&is=68301aac&hm=fd421e630543b8d6e3601d4223cd4070d7e4a7083c579df74e450c0ac1cddcbc&";
        this.Power = 8;
        this.Physique = 4;
        
        this.Willpower = 2;
        this.Resolve = 7;
        
        this.Luck = 5;
        
        this.Health = MaxHealth;
        this.Speed = 1;
        this.Cuteness = random.Next(1, 10001);
    }
    public override async Task<FamiliarAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
    }
    public async Task<FamiliarAction> PaperCranePunch()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "Paper Crane Punch";
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
        var StatusConditions = await GetStatusConditions();
        await AddStatusCondition(action.StatusCondition);
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