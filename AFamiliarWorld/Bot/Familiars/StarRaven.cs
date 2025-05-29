using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class StarRaven:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 40;
    public StarRaven()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            StarRavenAttack
        };
        var random = new Random();
        this.Name = "StarRaven";
        this.Description = "CAWWWW";
        this.Quip = "*Flaps wings*";
        this.Color = 0x1000cd;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375187492122398801/assets_task_01jvwnzsjvex2srn2tp25bzwh6_1747940621_img_3.webp?ex=68316f67&is=68301de7&hm=4084f349fb3c322008af44558140031caff771e10a9b77e1ef18231f6697b77e&=&format=webp&width=645&height=968";
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
    public async Task<FamiliarAttackingAction> StarRavenAttack()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "StarRaven Attack";
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