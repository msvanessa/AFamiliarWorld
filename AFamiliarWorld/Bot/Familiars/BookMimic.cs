using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class BookMimic:Familiar
{
    private List<Func<Familiar, Task<FamiliarAttackingAction>>> actions;
    private int MaxHealth = 300;
    public BookMimic()
    {
        this.actions = new List<Func<Familiar, Task<FamiliarAttackingAction>>>
        {
            BookBite
        };
        var random = new Random();
        this.Name = "BookMimic";
        this.Description = "Read me pls c:<";
        this.Emoji = "<:FamiliarBook:1378164196457320551>";
        this.Quip = "*Bites u*";
        this.Color = 0xa7743e;
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375188921834668102/assets_task_01jvwpbm43fc7r95023q0ca9ax_1747940954_img_3.webp?ex=683170bc&is=68301f3c&hm=e8e705ab8ca5f2c7eddb82de4b7d355be1330eebe9627a83a34facf63ef911f2&=&format=webp&width=645&height=968";
        this.Power = 20;
        this.Physique = 10;
        
        this.Willpower = 35;
        this.Resolve = 15;
        
        this.Luck = 5;

        this.MaxHealth = 300;
        this.Health = this.MaxHealth;
        
        this.Speed = 5;
        this.Cuteness = random.Next(1, 10001);
    }
    public override async Task<FamiliarAttackingAction> Attack(Familiar enemyFamiliar)
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke(enemyFamiliar);
    }
    
    public async Task<FamiliarAttackingAction> BookBite(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "BookBite";
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