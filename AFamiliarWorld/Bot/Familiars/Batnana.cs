using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Batnana:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private bool isEvading = false;
    public Batnana()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            Monch,
            EvasiveFlutter,
            PotassiumSurge,
            FruitfulShriek,
            Scritchies
        };
        var random = new Random();
        this.Name = "Batnana";
        this.Description = "Monch";
        this.Quip = "*Monches 'nana*";
        this.Color = 0xfff940;
        this.Url = "https://images-ext-1.discordapp.net/external/hXFeRnzIiH3xoipky40Dc43yl1Cp74oZ6-I-kDrlyJU/%3Fst%3D2025-05-22T19%253A01%253A17Z%26se%3D2025-05-28T20%253A01%253A17Z%26sks%3Db%26skt%3D2025-05-22T19%253A01%253A17Z%26ske%3D2025-05-28T20%253A01%253A17Z%26sktid%3Da48cca56-e6da-484e-a814-9c849652bcb3%26skoid%3D8ebb0df1-a278-4e2e-9c20-f2d373479b3a%26skv%3D2019-02-02%26sv%3D2018-11-09%26sr%3Db%26sp%3Dr%26spr%3Dhttps%252Chttp%26sig%3DFbAjfUp4i8o%252FLU5%252F6FRJUmWwy%252B%252F5oveMN%252BSz5EUCWPs%253D%26az%3Doaivgprodscus/https/videos.openai.com/vg-assets/assets%252Ftask_01jvwsv4a4fztrxx68g5mkxfdj%252F1747944610_img_3.webp?format=webp&width=645&height=968";
        this.Power = 30;
        this.Physique = 15;
        
        this.Willpower = 30;
        this.Resolve = 15;
        
        this.Luck = 5;

        this.MaxHealth = 325;
        this.Health = this.MaxHealth;
        
        this.Speed = 13;
        this.Cuteness = random.Next(1, 10001);
        
        this.Abilities.Add(new Ability("Spell: MONCH!", $"A physical attack that deals {this.Power}+1d20 damage and heals 10 health."));
        this.Abilities.Add(new Ability("Spell: Evasive Flutter", $"A physical attack that deals 0 damage and evades the next attack, reflecting 20 damage back to the attacker."));
        this.Abilities.Add(new Ability("Spell: Potassium Surge", $"A physical attack that deals 0 damage and increases the Batnana's Willpower and Power by 10."));
        this.Abilities.Add(new Ability("Spell: FruitfulShriek", $"A physical attack that deals 10 true damage and stuns the target."));
        this.Abilities.Add(new Ability("Spell: Scritchies", $"A physical attack that deals 20 true damage and causes the target to bleed."));
    }
    public override async Task<FamiliarAttackingAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
    }
    public async Task<FamiliarAttackingAction> Monch()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "MONCH!";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }

        if (!(await this.GetStatusConditions()).Contains(StatusCondition.Bleed))
        {
            this.Health += 10;
        }

        action.Damage = (Power + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }

    public async Task<FamiliarAttackingAction> EvasiveFlutter()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Evasive Flutter";
        this.isEvading = true;
        action.Damage = 0;
        action.DamageType = DamageType.Physical;
        return action;
    }
    
    public async Task<FamiliarAttackingAction> PotassiumSurge()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Potassium Surge";
        action.Damage = 0;
        action.DamageType = DamageType.Physical;
        this.Willpower += 10;
        this.Power += 10;
        return action;
    }
    
    public async Task<FamiliarAttackingAction> FruitfulShriek()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Fruitful Shriek";
        action.Damage = 10;
        action.DamageType = DamageType.Physical;
        action.IsTrueDamage = true;
        action.StatusConditions = new List<StatusCondition>{StatusCondition.Stun};
        return action;
    }
    
    public async Task<FamiliarAttackingAction> Scritchies()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Fruitful Shriek";
        action.Damage = 20;
        action.DamageType = DamageType.Physical;
        action.IsTrueDamage = true;
        action.StatusConditions = new List<StatusCondition>{StatusCondition.Bleed};
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
        if (!isEvading)
        {
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
        }
        else
        {
            defendingAction.DamageTaken = 0;
            defendingAction.IsReflecting = true;
            defendingAction.DamageReflected = 20;
            defendingAction.DamageReflectedMessage = "Evasive Flutter";
        }

        return defendingAction;
    }
}