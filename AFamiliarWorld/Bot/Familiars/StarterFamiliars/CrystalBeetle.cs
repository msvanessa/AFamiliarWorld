using System.Runtime.CompilerServices;
using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class CrystalBeetle:Familiar
{
    private List<Func<Task<FamiliarAction>>> actions;
    private int MaxHealth = 40;
    public CrystalBeetle()
    {
        this.actions = new List<Func<Task<FamiliarAction>>>
        {
            BeetleBonk,
            Slam
        };
        var random = new Random();
        this.Name = "Crystal Beetle";
        this.Description = "...";
        this.Quip = "*Hunkers down*";
        this.Color = 0x34e9d0;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375184355504423063/assets_task_01jvwnaxt0eyzbc2bbb853pmxf_1747939882_img_0.webp?ex=68316c7c&is=68301afc&hm=66a1087f14c1e0cbe2c431e9cff617e260f809df7e5a73b86f88e566330f0e3a&";
        this.Power = 8;
        this.Physique = 4;
        
        this.Willpower = 8;
        this.Resolve = 4;
        
        this.Luck = 5;
        
        this.Health = MaxHealth;
        this.Speed = 1;
        this.Cuteness = Math.Min(random.Next(1, 10001), random.Next(1, 10001));
    }
    
    public override async Task<FamiliarAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
    }

    public async Task<FamiliarAction> BeetleBonk()
    {
        var random = new Random();
        var action = new FamiliarAction();
        action.AbilityName = "Beetle Bonk";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 5)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }

    public async Task<FamiliarAction> Slam()
    {
        var action = new FamiliarAction();
        action.AbilityName = "Slam";
        action.Damage = 1;
        action.DamageType = DamageType.Physical;
        action.StatusConditions = new List<StatusCondition>{StatusCondition.Stun};
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