using System.Net.Quic;
using System.Runtime.CompilerServices;
using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Clockroach : Familiar
{
    private List<Func<Familiar, Task<FamiliarAttackingAction>>> actions;
    private int RewindHealth = 225;
    public Clockroach()
    {
        this.actions = new List<Func<Familiar, Task<FamiliarAttackingAction>>>
        {
            GearjammerBite,
            Timespark,
            Flickerstrike,
            Selfdestruct,
            SelfRewind
        };
        var random = new Random();
        this.Name = "Clockroach";
        this.Description = "I clock, I roach, I clockroach";
        this.Emoji = "<:FamiliarClockRoach:1378164199426883634>";
        this.Quip = "*Winds you up*";
        this.Color = 0xa75600;
        this.Url =
            "https://media.discordapp.net/attachments/1246170699362729995/1375190322996580412/assets_task_01jvwpn0mqff4vf7b0de9s151x_1747941299_img_1.webp?ex=6831720a&is=6830208a&hm=2ebf5e6cba5c4784f875f7398494e1a82e1269342cc85746db8f9280230d3407&=&format=webp&width=645&height=968";
        this.Power = 40;
        this.Physique = 10;

        this.Willpower = 40;
        this.Resolve = 15;

        this.Luck = 10;

        this.MaxHealth = 250;
        this.Health = this.MaxHealth;

        this.Speed = 100;
        this.Cuteness = random.Next(1, 10001);
    }

    public override async Task<FamiliarAttackingAction> Attack(Familiar enemyFamiliar)
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke(enemyFamiliar);
    }

    public async Task<FamiliarAttackingAction> GearjammerBite(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Gearjammer Bite";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }

        action.Damage = (Power + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }

    public async Task<FamiliarAttackingAction> Timespark(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Timespark";
        int crit = Math.Min(random.Next(1, 101), random.Next(1, 101)) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }

        action.Damage = (Willpower + random.Next(1, 31)) * (crit);
        action.DamageType = DamageType.Magical;
        return action;
    }

    public async Task<FamiliarAttackingAction> Flickerstrike(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Flickerstrike";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Speed / 2) * (crit);
        action.DamageType = DamageType.Physical;
        Speed += 10;
        return action;
    }

    public async Task<FamiliarAttackingAction> Selfdestruct(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Self Destruct";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + Willpower + random.Next(1, 21)) * (crit);
        Health -= action.Damage / 3;
        action.DamageType = DamageType.Physical;
        return action;
    }

    public async Task<FamiliarAttackingAction> SelfRewind(Familiar familiar)
    {
        var action = new FamiliarAttackingAction()
        
        {
            AbilityName = "Self-Rewind",
            Damage = 0,
            CriticalHit = false,
            DamageType = DamageType.Magical,
            StatusConditions = (await this.GetStatusConditions()).ToList(),
            IsTrueDamage = true
        };
        Health = RewindHealth;
        return action;
    }
    public override async Task<FamiliarDefendingAction> Defend(FamiliarAttackingAction attackingAction)
    {
        RewindHealth = Health;
        if (attackingAction.StatusConditions != null)
        {
            foreach (var statusCondition in attackingAction.StatusConditions)
            {
                await this.AddStatusCondition(statusCondition);
            }
        }

        var defendingAction = new FamiliarDefendingAction();
        if (attackingAction.IsTrueDamage)
        {
            defendingAction.DamageTaken = attackingAction.Damage;
        }
        else
        {
            if (attackingAction.DamageType == DamageType.Physical)
            {
                defendingAction.DamageTaken = attackingAction.Damage - this.Physique;
            }
            else if (attackingAction.DamageType == DamageType.Magical)
            {
                defendingAction.DamageTaken = (attackingAction.Damage - Resolve);
            }
        }
        if(defendingAction.DamageTaken < 0)
            defendingAction.DamageTaken = 0;
        return defendingAction;
    }
}