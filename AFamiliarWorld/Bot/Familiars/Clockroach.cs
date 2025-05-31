using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Clockroach:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    public Clockroach()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            ClockroachAttack
        };
        var random = new Random();
        this.Name = "Clockroach";
        this.Description = "I clock, I roach, I clockroach";
        this.Quip = "*Winds you up*";
        this.Color = 0xa75600;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375190322996580412/assets_task_01jvwpn0mqff4vf7b0de9s151x_1747941299_img_1.webp?ex=6831720a&is=6830208a&hm=2ebf5e6cba5c4784f875f7398494e1a82e1269342cc85746db8f9280230d3407&=&format=webp&width=645&height=968";
        this.Power = 40;
        this.Physique = 10;
        
        this.Willpower = 40;
        this.Resolve = 15;
        
        this.Luck = 5;

        this.MaxHealth = 250;
        this.Health = this.MaxHealth;
        
        this.Speed = 100;
        this.Cuteness = random.Next(1, 10001);
    }
    public override async Task<FamiliarAttackingAction> Attack()
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke();
    }
    public async Task<FamiliarAttackingAction> ClockroachAttack()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Clockroach Attack";
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