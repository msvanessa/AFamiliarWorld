using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Ropopus:Familiar
{
    private List<Func<Task<FamiliarAction>>> actions;
    private int MaxHealth = 40;
    public Ropopus()
    {
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
            RopopusAttack
        };
        var random = new Random();
        this.Name = "Ropopus";
        this.Description = "._.";
        this.Quip = "*Wraps around u";
        this.Color = 0x925252;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375198791673581618/assets_task_01jvwrgw9ze34t1wb04gj76wmw_1747943275_img_0.webp?ex=683179ed&is=6830286d&hm=d4160412efe492d4441bf076d19bd064abf59fcadf1e8694472911a966430cc3&=&format=webp&width=645&height=968";
        this.Power = 4;
        this.Physique = 10;
        
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
    public async Task<FamiliarAction> RopopusAttack()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "Ropopus Attack";
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