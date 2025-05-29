using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class SkeleMouse:Familiar
{
    private List<Func<Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 40;
    public SkeleMouse()
    {
        this.actions = new List<Func<Task<FamiliarAttackingAction>>>
        {
            SkeleMouseAttack
        };
        var random = new Random();
        this.Name = "SkeleMouse";
        this.Description = "Yip yip!";
        this.Quip = "*Brings u smol piece of cheese*";
        this.Color = 0xc7bdbd;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375185078405173270/assets_task_01jvwng9ypectvwtxbmwcqyx94_1747940061_img_1.webp?ex=68316d28&is=68301ba8&hm=4fb5f179c149b3d623b72afff86a558ab9bed01dba98f24aed7512bb8e6a1d7b&=&format=webp&width=645&height=968";
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
    public async Task<FamiliarAttackingAction> SkeleMouseAttack()
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "SkeleMouse Attack";
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