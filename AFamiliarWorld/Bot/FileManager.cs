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
}