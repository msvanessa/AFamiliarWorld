using System.Runtime.CompilerServices;
using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class CrystalBeetle:Familiar
{
    private List<Func<Familiar, Task<FamiliarAttackingAction>>> actions;
    private bool _isHunkeredDown = false;
    private int _shatterPulseDamage = 0;
    public CrystalBeetle()
    {
        this.actions = new List<Func<Familiar, Task<FamiliarAttackingAction>>>
        {
            BeetleBonk,
            Slam,
            HunkerDown,
            PrismaticSpark,
            ShatterPulse
        };
        var random = new Random();
        this.Name = "Crystal Beetle";
        this.Description = "...";
        this.Quip = "*Hunkers down*";
        this.Color = 0x34e9d0;
        this.Emoji = "<:FamiliarCrystalbeetle:1378164202245587055>";
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375184355504423063/assets_task_01jvwnaxt0eyzbc2bbb853pmxf_1747939882_img_0.webp?ex=68316c7c&is=68301afc&hm=66a1087f14c1e0cbe2c431e9cff617e260f809df7e5a73b86f88e566330f0e3a&";
        this.Power = 25;
        this.Physique = 25;
        
        this.Willpower = 25;
        this.Resolve = 25;
        
        this.Luck = 5;
        
        this.MaxHealth = 400;
        this.Health = this.MaxHealth;
        
        this.Speed = 1;
        this.Cuteness = Math.Min(random.Next(1, 10001), random.Next(1, 10001));
        
        this.Abilities.Add(new Ability("Passive: Crystal Shell", $"Takes 50% less magical damage"));
        this.Abilities.Add(new Ability("Spell: Beetle Bonk", $"A basic physical attack that deals {this.Power}+1d20 damage. Has a 20% chance to crit."));
        this.Abilities.Add(new Ability("Spell: Slam", $"A physical attack that deals 10 true damage and stuns the target."));
        this.Abilities.Add(new Ability("Spell: Hunker Down", $"A defensive ability that deals 0 damage, but reduces incoming damage by your Willpower for the next attack."));
        this.Abilities.Add(new Ability("Spell: Prismatic Spark", $"A magical attack that deals {this.Willpower}+1d20 damage."));
        this.Abilities.Add(new Ability("Spell: Shatter Pulse", $"A magical attack that deals half of {this.Power}+1d20 physical damage and applies a shatter pulse. The next time you use an attack, it will deal the other half ontop of your regular attack as magical damage."));
    }
    
    public override async Task<FamiliarAttackingAction> Attack(Familiar enemyFamiliar)
    {
        var shatterDamage = 0;
        if (this._shatterPulseDamage > 0)
        {
            shatterDamage += this._shatterPulseDamage;
            this._shatterPulseDamage = 0;
        }
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        var attackingaction = await randomAbility.Invoke(enemyFamiliar);
        attackingaction.Damage += shatterDamage;
        return attackingaction;
    }

    public async Task<FamiliarAttackingAction> BeetleBonk(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Beetle Bonk";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }

    public async Task<FamiliarAttackingAction> Slam(Familiar familiar)
    {
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Slam";
        action.Damage = 10;
        action.DamageType = DamageType.Physical;
        action.IsTrueDamage = true;
        action.StatusConditions = new List<StatusCondition>{StatusCondition.Stun};
        return action;
    }
    
    public async Task<FamiliarAttackingAction> HunkerDown(Familiar familiar)
    {
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Hunker down";
        action.Damage = 0;
        action.CustomOutput = "The Crystal Beetle hunkers down, reducing incoming damage by its Willpower for the next attack";
        action.DamageType = DamageType.Physical;
        action.IsTrueDamage = true;
        this._isHunkeredDown = true;
        return action;
    }
    public async Task<FamiliarAttackingAction> ShatterPulse(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Shatter Pulse";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        var damage = (this.Willpower + random.Next(1, 21)) * (crit);
        
        action.Damage += damage / 2;
        this._shatterPulseDamage = damage / 2;
        action.DamageType = DamageType.Magical;
        
        return action;
    }
    public async Task<FamiliarAttackingAction> PrismaticSpark(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Prismatic Spark";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (this.Willpower + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Magical;
        return action;
    }
    
    public override async Task<FamiliarDefendingAction> Defend(FamiliarAttackingAction attackingAction)
    {
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
                defendingAction.DamageTaken = ((attackingAction.Damage - Resolve) / 2);
            }
        }

        if (_isHunkeredDown)
        {
            defendingAction.DamageTaken -= this.Willpower;
            if (defendingAction.DamageTaken < 0)
            {
                defendingAction.DamageTaken = 0;
            }

            this._isHunkeredDown = false;
        }
        if(defendingAction.DamageTaken < 0)
            defendingAction.DamageTaken = 0;
        
        return defendingAction;
    }
}