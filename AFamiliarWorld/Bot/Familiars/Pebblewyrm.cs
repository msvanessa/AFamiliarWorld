using AFamiliarWorld.Bot.Commands.Models;

namespace AFamiliarWorld.Bot.Familiars;

public class Pebblewyrm:Familiar
{
    private List<Func<Familiar, Task<FamiliarAttackingAction>>> actions;
    public Pebblewyrm()
    {
        this.actions = new List<Func<Familiar, Task<FamiliarAttackingAction>>>
        {
            PebblewyrmAttack
        };
        var random = new Random();
        this.Name = "Pebblewyrm";
        this.Description = "Rawr!";
        this.Quip = "*Breathes gravel*";
        this.Color = 0x808080;
        this.Emoji = "<:FamiliarPebblewyrm:1378164940665524244>";
        this.Url = "https://media.discordapp.net/attachments/1246170699362729995/1375190944441307277/assets_task_01jvwprr6cfwt8zdrwh2f6td02_1747941436_img_0.webp?ex=6831729e&is=6830211e&hm=63b02e99830de26dbf0f6aa5a6b922c102cc17e1ec78721aa0db4a9b5454f9c1&=&format=webp&width=645&height=968";
        this.Power = 30;
        this.Physique = 20;
        
        this.Willpower = 30;
        this.Resolve = 20;
        
        this.Luck = 5;

        this.MaxHealth = 400;
        this.Health = this.MaxHealth;
        
        this.Speed = 9;
        this.Cuteness = random.Next(1, 10001);
    }
    public override async Task<FamiliarAttackingAction> Attack(Familiar enemyFamiliar)
    {
        var random = new Random();
        var randomAbility = actions[random.Next(actions.Count)];
        return await randomAbility.Invoke(enemyFamiliar);
    }
    public async Task<FamiliarAttackingAction> PebblewyrmAttack(Familiar familiar)
    {
        var random = new Random();
        var action = new FamiliarAttackingAction();
        action.AbilityName = "Pebblewyrm Attack";
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