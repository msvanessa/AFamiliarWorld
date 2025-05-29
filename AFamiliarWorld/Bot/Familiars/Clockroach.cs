using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Clockroach:Familiar
{
    private List<Func<Task<FamiliarAction>>> actions;
    private int MaxHealth = 40;
    public Clockroach()
    {
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
            ClockroachAttack
        };
        var random = new Random();
        this.Name = "Clockroach";
        this.Description = "I clock, I roach, I clockroach";
        this.Quip = "*Winds you up*";
        this.Color = 0xa75600;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375190322996580412/assets_task_01jvwpn0mqff4vf7b0de9s151x_1747941299_img_1.webp?ex=6831720a&is=6830208a&hm=2ebf5e6cba5c4784f875f7398494e1a82e1269342cc85746db8f9280230d3407&=&format=webp&width=645&height=968";
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
    public async Task<FamiliarAction> ClockroachAttack()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "Clockroach Attack";
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
        if (action.StatusConditions != null)
        {
            foreach (var statusCondition in action.StatusConditions)
            {
                await this.AddStatusCondition(statusCondition);
            }
        }
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