using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Pebblewyrm:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 40;
    public Pebblewyrm()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            PebblewyrmAttack
        };
        var random = new Random();
        this.Name = "Pebblewyrm";
        this.Description = "Rawr!";
        this.Quip = "*Breathes gravel*";
        this.Color = 0x808080;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375190944441307277/assets_task_01jvwprr6cfwt8zdrwh2f6td02_1747941436_img_0.webp?ex=6831729e&is=6830211e&hm=63b02e99830de26dbf0f6aa5a6b922c102cc17e1ec78721aa0db4a9b5454f9c1&=&format=webp&width=645&height=968";
        this.Power = 4;
        this.Physique = 10;
        
        this.Willpower = 2;
        this.Resolve = 7;
        
        this.Luck = 5;
        
        this.Health = MaxHealth;
        this.Speed = 1;
        this.Cuteness = random.Next(1, 10001);
    }
    public override async Task<FamiliarAttackingAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
    }
    public async Task<FamiliarAttackingAction> PebblewyrmAttack()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Pebblewyrm Attack";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 5)) * (crit);
        action.DamageType = DamageType.Physical;
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

        var defendingAction = new FamiliarDefendingAction()
        {
            
        };
        
        if (attackingAction.DamageType == DamageType.Physical)
        {
            defendingAction.DamageTaken = attackingAction.Damage - Physique;
        }
        else if (attackingAction.DamageType == DamageType.Magical)
        {
            defendingAction.DamageTaken = (attackingAction.Damage - Resolve);
        }
        
        return defendingAction;
    }
}