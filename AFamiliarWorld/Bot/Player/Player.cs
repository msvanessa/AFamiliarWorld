using AFamiliarWorld.Bot.Familiars;

namespace AFamiliarWorld.Bot.Player;

public class Player
{
    public List<Familiar> familiars { get; set; }
    private List<Type> AvailableFamiliars = new List<Type>(){typeof(Imp), typeof(PaperCraneGolem), typeof(CrystalBeetle), typeof(Batnana), typeof(BookMimic), typeof(Clockroach), typeof(LandShork), typeof(NoodleCrab), typeof(Pebblewyrm), typeof(Ropopus), typeof(SkeleMouse), typeof(SlimeCat), typeof(StarRaven)};
    public long timeSinceLastScavenge = 0;
    public int Gold { get; set; } = 100;
    
    public Player()
    {
        
    }
    

    public async Task <(Familiar, int)> Scavenge()
    {
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random();
        int gainedGold = 0;
        
        if (timeSinceLastScavenge + 0 < time)
        {
            var familiar = (Familiar) Activator.CreateInstance(AvailableFamiliars[random.Next(0, AvailableFamiliars.Count)]);
            familiars.Add(familiar);
            gainedGold += random.Next(1, 51);
            Gold += gainedGold;
            timeSinceLastScavenge = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return (familiar, gainedGold);
        }
        else
        {
            return (null, -1);
        }
    }
}