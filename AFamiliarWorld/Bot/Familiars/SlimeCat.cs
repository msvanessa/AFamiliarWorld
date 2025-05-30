using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class SlimeCat:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 40;
    public SlimeCat()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            SlimeCatAttack
        };
        var random = new Random();
        this.Name = "SlimeCat";
        this.Description = "Mlem :3";
        this.Quip = "*Coughs up treasure*";
        this.Color = 0x0ccd00;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375193634525286622/assets_task_01jvwqea7zfdyv09ke1hzw2dfn_1747942093_img_0.webp?ex=68317520&is=683023a0&hm=990b40abd7341d042ca918904c5663f054e4c0fe2150229c7bf4d17f8bd5753e&=&format=webp&width=645&height=968";
        this.Power = 4;
        this.Physique = 4;
        
        this.Willpower = 4;
        this.Resolve = 4;
        
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
    public async Task<FamiliarAttackingAction> SlimeCatAttack()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "SlimeCat Attack";
        int crit = random.Next(1, 101) < Luck ? 2 : 1;
        if (crit == 2)
        {
            action.CriticalHit = true;
        }
        action.Damage = (Power + random.Next(1, 5)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }
}