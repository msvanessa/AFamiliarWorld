using AFamiliarWorld.Bot.Familiars;

namespace AFamiliarWorld.Bot.Player;

public class Player
{
    public List<Familiar> familiars { get; set; }
    private List<Type> AvailableFamiliars = new List<Type>(){typeof(Imp), typeof(PaperCraneGolem), typeof(CrystalBeetle), typeof(Batnana), typeof(BookMimic), typeof(Clockroach), typeof(LandShork), typeof(NoodleCrab), typeof(Pebblewyrm), typeof(Ropopus), typeof(SkeleMouse), typeof(SlimeCat), typeof(StarRaven)};
    public long timeSinceLastScavenge = 0;
    
    public Player()
    {
        
    }

    public async Task <Familiar> Scavenge()
    {
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random();
        
        if (timeSinceLastScavenge + 0 < time)
        {
            var familiar = (Familiar) Activator.CreateInstance(AvailableFamiliars[random.Next(0, AvailableFamiliars.Count)]);
            familiars.Add(familiar);
            timeSinceLastScavenge = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return familiar ;
        }
        else
        {
            return null;
        }
    }
}