using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Bandcoon:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 300;
    public Bandcoon()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            Moneysack,
            Eattrash,
            Pistolwhip,
            Washfood
        };
        var random = new Random();
        this.Name = "Bandcoon";
        this.Description = "*Steals your gun*";
        this.Emoji = "<:FamiliarBandcoon:1378164191721947218>";
        this.Quip = "*MINE MINE MINE*";
        this.Color = 0x1dd3df;
        this.Url = "https://cdn.discordapp.com/attachments/1246170699362729995/1377615519489851412/assets_task_01jwd0kxy8e74ty8skmt7nxay8_1748488577_img_0.webp?ex=68399bee&is=68384a6e&hm=0e377038e71fc6f3bdd5e5141f68d611c7c0f2827945eaa9ffe8e4e28a5885d2&";
        this.Power = 40;
        this.Physique = 15;
        
        this.Willpower = 40;
        this.Resolve = 15;
        
        this.Luck = 10;
        
        this.Health = MaxHealth;
        this.Speed = 1;
        this.Cuteness = random.Next(1, 10001);
        this.Abilities.Add(new Ability("Spell: Moneysack", $"Grabs its money sack and attack them with it. Deals 1-30 damage and has a chance to apply any status condition."));
        this.Abilities.Add(new Ability("Spell: Eat trash", $"Eats trash and gains a random stat boost. Can also apply poison to itself."));
        this.Abilities.Add(new Ability("Spell: Pistol whip", $"Whips them with its pistol. Deals {this.Power}+1d20 damage and has a chance to hit itself for 50 damage."));
        this.Abilities.Add(new Ability("Spell: Wash food", $"Washes its food and attacks them. Deals {this.Power}+1d20 damage and removes a random status condition from itself."));
    }
    
    public override async Task<FamiliarAttackingAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
    }
    public async Task<FamiliarAttackingAction> Moneysack()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Moneysack";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        

        var option = new List<FamiliarAttackingAction>()
        {
            new FamiliarAttackingAction()
            {
                Damage = (Power + random.Next(1, 21)) * (crit),
                DamageType = DamageType.Physical
            },
            new FamiliarAttackingAction()
            {
                Damage = (Willpower + random.Next(1, 21)) * (crit),
                DamageType = DamageType.Magical
            },
            new FamiliarAttackingAction()
            {
                Damage = 1,
                DamageType = DamageType.Physical,
                IsTrueDamage = true,
                StatusConditions = new List<StatusCondition>() { StatusCondition.Stun }
            },
            new FamiliarAttackingAction()
            {
                Damage = 2,
                DamageType = DamageType.Physical,
                IsTrueDamage = true,
                StatusConditions = new List<StatusCondition>() { StatusCondition.Burn }
            },
            new FamiliarAttackingAction()
            {
                Damage = 2,
                DamageType = DamageType.Physical,
                IsTrueDamage = true,
                StatusConditions = new List<StatusCondition>() { StatusCondition.Poison }
            },
            new FamiliarAttackingAction()
            {
                Damage = 2,
                DamageType = DamageType.Physical,
                IsTrueDamage = true,
                StatusConditions = new List<StatusCondition>() { StatusCondition.Bleed }
            },
            new FamiliarAttackingAction()
            {
                Damage = 2,
                DamageType = DamageType.Physical,
                IsTrueDamage = true,
                StatusConditions = new List<StatusCondition>() { StatusCondition.Confuse }
            }
        };
        var selectedOption = option[random.Next(option.Count)];
        action.Damage = selectedOption.Damage;
        action.DamageType = selectedOption.DamageType;
        action.IsTrueDamage = selectedOption.IsTrueDamage;
        action.StatusConditions = selectedOption.StatusConditions;
        
        return action;
    }
    
    public async Task<FamiliarAttackingAction> Eattrash()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Eat trash";
        action.Damage = 0;
        action.DamageType = DamageType.Physical;
        var option = random.Next(0, 9);
        switch (option)
        {
            case 0:
                this.Power += 10;
                break;
            case 1:
                this.Physique += 10;
                break;
            case 2:
                this.Willpower += 10;
                break;
            case 3:
                this.Resolve += 10;
                break;
            case 4:
                this.Luck += 5;
                break;
            case 5:
                this.Speed += 10;
                break;
            case 6:
                if ((await this.GetStatusConditions()).Contains(StatusCondition.Bleed))
                {
                    this.Health += 25;
                }
                else
                {
                    this.Health += 50;
                }

                break;
            case 8:
                await this.AddStatusCondition(StatusCondition.Poison);
                break;
        }
        
        return action;
    }
    public async Task<FamiliarAttackingAction> Pistolwhip()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Pistol whip";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 31)) * (crit);
        action.DamageType = DamageType.Physical;

        if (random.Next(1, 101) < this.Luck)
        {
            this.Health -= 50;
            action.Damage = 0;
            action.AbilityName += " (and hits itself instead for 50 damage)";
        }
        
        return action;
    }

    public async Task<FamiliarAttackingAction> Washfood()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Wash food";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 41)) * (crit);
        action.DamageType = DamageType.Physical;
        var conditions = await this.GetStatusConditions();
        if (conditions.Count > 0)
            await RemoveStatusCondition(conditions[random.Next(0, conditions.Count)]);
        return action;
    }
}