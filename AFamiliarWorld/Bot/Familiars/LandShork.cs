using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class LandShork:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 40;
    public LandShork()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            LandShorkMonch
        };
        var random = new Random();
        this.Name = "LandShork";
        this.Description = "My name jef";
        this.Quip = "*Monches u*";
        this.Color = 0x1dd3df;
        this.Url = "https://images-ext-1.discordapp.net/external/OmhTzBTivkzoP91kdb1_Dj5jy2ihSsrwRSd0ta8_bBo/%3Fst%3D2025-05-22T18%253A01%253A36Z%26se%3D2025-05-28T19%253A01%253A36Z%26sks%3Db%26skt%3D2025-05-22T18%253A01%253A36Z%26ske%3D2025-05-28T19%253A01%253A36Z%26sktid%3Da48cca56-e6da-484e-a814-9c849652bcb3%26skoid%3D8ebb0df1-a278-4e2e-9c20-f2d373479b3a%26skv%3D2019-02-02%26sv%3D2018-11-09%26sr%3Db%26sp%3Dr%26spr%3Dhttps%252Chttp%26sig%3Dm%252Bn%252BGiME%252BM5zq7cNYxwjHcm3F69NgrgoALnVoGduC90%253D%26az%3Doaivgprodscus/https/videos.openai.com/vg-assets/assets%252Ftask_01jvwrrcrdf15r90xpwzknnd52%252F1747943505_img_2.webp?format=webp&width=645&height=968";
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
    public async Task<FamiliarAttackingAction> LandShorkMonch()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "LandShork Monch";
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