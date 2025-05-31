using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars.StarterFamiliars;

public class Imp:Familiar
{
    private int MaxHealth = 300;
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    public Imp()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            Firebolt,
            Scratch,
            Sting,
            WingFlap
        };
        
        var random = new Random();
        this.Name = "Imp";
        this.Description = "Hehe grumble grumble";
        this.Emoji = "<:FamiliarImp:1378164205827391529>";
        this.Quip = "*angy*";
        this.Color = 0xff0000;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1375183010936258620/assets_task_01jvwmzenxe9t9j2et7ft2ggqs_1747939506_img_1.webp?ex=68316b3b&is=683019bb&hm=0eebbb630ed6711395d42981d2b4fbdefc50ffd535b8720dbc5a16f9a4ce9c94&";
        this.Power = 40;
        this.Physique = 10;
        
        this.Willpower = 50;
        this.Resolve = 10;
        
        this.Luck = 5;
        
        this.Health = MaxHealth;
        this.Speed = 1;
        this.Cuteness = random.Next(1, 10001);
        
        this.Abilities.Add(new Ability("Spell: Firebolt", $"A fiery attack that deals {this.Willpower}+1d20 damage and has a 20% chance to burn the target."));
        this.Abilities.Add(new Ability("Spell: Scratch", $"A physical attack that deals {this.Power}+1d20 damage."));
        this.Abilities.Add(new Ability("Spell: Sting", $"A physical attack that deals 20 true damage and poisons the target."));
        this.Abilities.Add(new Ability("Spell: Wing Flap", $"A magical attack that deals 0 damage and clears all status conditions, transferring them to the target."));
    }

    public async Task<FamiliarAttackingAction> WingFlap()
    {
        var action = new FamiliarAttackingAction()
        {
            AbilityName = "Wing flap",
            Damage = 0,
            CriticalHit = false,
            DamageType = DamageType.Magical,
            StatusConditions = (await this.GetStatusConditions()).ToList(),
            IsTrueDamage = true
        };

        await this.ClearStatusConditions();
        return action;
    }

    public async Task<FamiliarAttackingAction> Sting()
    {
        var random = new Random();
        var crit = random.Next(1, 101) < this.Luck;
        var action = new FamiliarAttackingAction()
        {
            AbilityName = "Sting",
            Damage = crit ? 4:2,
            CriticalHit = random.Next(1, 101) < this.Luck,
            DamageType = DamageType.Physical,
            StatusConditions = new List<StatusCondition>() { StatusCondition.Poison },
            IsTrueDamage = true
        };
        return action;
    }
    
    public async Task<FamiliarAttackingAction> Firebolt()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Firebolt";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Willpower + random.Next(1, 21)) * (crit);
        bool burn = random.Next(1, 101) < 21;
        if (burn)
        {
            action.StatusConditions = new List<StatusCondition>() { StatusCondition.Burn };
        }
        action.DamageType = DamageType.Magical;
        return action;
    }
    
    public async Task<FamiliarAttackingAction> Scratch()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Imp Scratch";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }
    public override async Task<FamiliarAttackingAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
    }
}