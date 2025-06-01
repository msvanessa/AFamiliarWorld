using AFamiliarWorld.Bot.Commands.Models;
using AFamiliarWorld.Bot.Familiars;
using Newtonsoft.Json;

namespace AFamiliarWorld.Bot;

public class FileManager
{
    public static Player.Player? FetchUserData(ulong userID)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        if (!File.Exists(userID + ".json"))
        {
            return null;
        }
        var player = JsonConvert.DeserializeObject<Player.Player>(File.ReadAllText(userID + ".json"), settings);
        return player;
    }
    
    public static bool UserExists(ulong userID)
    {
        return File.Exists(userID + ".json");
    }
    
    public static void SaveUserData(ulong userID, Player.Player player)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        string json = JsonConvert.SerializeObject(player, settings);
        File.WriteAllText(userID + ".json", json);
    }
    
    public static void UpdateStatisticLogger(Familiar winner, Familiar loser, WinCondition winCondition)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        StatisticLogger logger;
        if (!File.Exists("statisticsLogger.json"))
        {
            logger = new StatisticLogger();
        }
        else
        {
            logger = JsonConvert.DeserializeObject<StatisticLogger>(File.ReadAllText("statisticsLogger.json"), settings);
        }

        logger.Battles += 1;
        switch (winCondition)
        {
            case WinCondition.NormalDamage:
                logger.FamiliarsLostByNormalDamage.Add(loser.Name);
                logger.FamiliarsWonByNormalDamage.Add(winner.Name);
                break;
            case WinCondition.ReflectedDamage:
                logger.FamiliarsLostByReflectedDamage.Add(loser.Name);
                logger.FamiliarsWonByReflectedDamage.Add(winner.Name);
                break;
            case WinCondition.StatusCondition:
                logger.FamiliarsLostByStatusCondition.Add(loser.Name);
                logger.FamiliarsWonByStatusCondition.Add(winner.Name);
                break;
        }
        logger.winnerFamiliars.Add(winner.Name);
        logger.loserFamiliars.Add(loser.Name);
        
        string json = JsonConvert.SerializeObject(logger, settings);
        File.WriteAllText("statisticsLogger.json", json);
    }
}