using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class SlimeCat:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
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
        this.Emoji = "<:FamiliarSlimecat:1378164226908094534>";
        this.Color = 0x0ccd00;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375193634525286622/assets_task_01jvwqea7zfdyv09ke1hzw2dfn_1747942093_img_0.webp?ex=68317520&is=683023a0&hm=990b40abd7341d042ca918904c5663f054e4c0fe2150229c7bf4d17f8bd5753e&=&format=webp&width=645&height=968";
        this.Power = 30;
        this.Physique = 15;
        
        this.Willpower = 25;
        this.Resolve = 15;
        
        this.Luck = 5;

        this.MaxHealth = 350;
        this.Health = this.MaxHealth;
        
        this.Speed = 2;
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
        action.Damage = (Power + random.Next(1, 21)) * (crit);
        action.DamageType = DamageType.Physical;
        return action;
    }
}