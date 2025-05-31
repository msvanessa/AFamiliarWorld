using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Ropopus:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 40;
    public Ropopus()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            RopopusAttack
        };
        var random = new Random();
        this.Name = "Ropopus";
        this.Description = "._.";
        this.Quip = "*Wraps around u";
        this.Emoji = "<:FamiliarRopopus:1378164220612317294>";
        this.Color = 0x925252;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375198791673581618/assets_task_01jvwrgw9ze34t1wb04gj76wmw_1747943275_img_0.webp?ex=683179ed&is=6830286d&hm=d4160412efe492d4441bf076d19bd064abf59fcadf1e8694472911a966430cc3&=&format=webp&width=645&height=968";
        this.Power = 40;
        this.Physique = 15;
        
        this.Willpower = 40;
        this.Resolve = 15;
        
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
    public async Task<FamiliarAttackingAction> RopopusAttack()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Ropopus Attack";
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