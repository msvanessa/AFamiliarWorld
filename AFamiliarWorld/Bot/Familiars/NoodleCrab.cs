using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class NoodleCrab:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 40;
    public NoodleCrab()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            NoodleCrabPinch
        };
        var random = new Random();
        this.Name = "NoodleCrab";
        this.Description = "If u take my noodles I kil u :)";
        this.Quip = "CRABRAVE";
        this.Color = 0x781ddf;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375194682270679240/assets_task_01jvwqk2n5f36b9zctwceetbr7_1747942313_img_2.webp?ex=6831761a&is=6830249a&hm=a019251cda1c47e2da34472283e85f4fcba4ebf9b0c58d5e614c8b724e68e727&=&format=webp&width=645&height=968";
        this.Power = 40;
        this.Physique = 15;
        
        this.Willpower = 40;
        this.Resolve = 10;
        
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
    public async Task<FamiliarAttackingAction> NoodleCrabPinch()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Noodle Crab Pinch";
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