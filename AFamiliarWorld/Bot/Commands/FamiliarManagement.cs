using AFamiliarWorld.Bot.Familiars;
using Discord.Commands;
using Newtonsoft.Json;

namespace AFamiliarWorld.Bot.Commands;

public class FamiliarManagement
{
    public class FamiliarManagementModule : ModuleBase<SocketCommandContext>
    {
        [Command("createplayer")]
        [Summary("Creates a new player")]
        public Task CreatePlayerAsync()
        {
            try
            {
                string filePath = Context.User.Id + ".json";
                if (File.Exists(filePath))
                {
                    ReplyAsync("You already have a character");
                    return Task.CompletedTask;
                }

                List<Type> StarterFamiliars = new List<Type>{typeof(Imp), typeof(PaperCraneGolem), typeof(CrystalBeetle)};
                var random = new Random();
                var player = new Player.Player();
                player.familiars = new List<Familiar>();
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };
                var familiar = (Familiar) Activator.CreateInstance(StarterFamiliars[random.Next(0, StarterFamiliars.Count)]);
                player.familiars.Add(familiar);
                ReplyAsync(embed:familiar.Display());
                string json = JsonConvert.SerializeObject(player, settings);
                File.WriteAllText(filePath, json);
                ReplyAsync("You have successfully created a new character");
            }
            catch (Exception ex)
            {
                ReplyAsync(ex.Message);
            }
            return Task.CompletedTask;
        }

        [Command("displayfamiliars")]
        [Summary("Displays your familiars")]
        public Task DisplayFamiliarsAsync()
        {
            string filePath = Context.User.Id + ".json";
            if (!File.Exists(filePath))
            {
                ReplyAsync("You haven't created a profile");
                return Task.CompletedTask;
            }
            
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            var player = JsonConvert.DeserializeObject<Player.Player>(File.ReadAllText(Context.User.Id + ".json"), settings);
            if (player.familiars.Count == 0)
            {
                ReplyAsync("You don't have any familiars yet!");
                return Task.CompletedTask;
            }

            foreach (var fam in player.familiars)
            {
                ReplyAsync(fam.Name + ", Familiar ID: `" + fam.FamiliarID + "`");
                fam.SpecialAbility().Wait();
            }
            
            return Task.CompletedTask;
        }

        [Command("scavenge")]
        [Summary("Scavenges a familiar")]
        public async Task ScavengeAsync()
        {
            string filePath = Context.User.Id + ".json";
            if (!File.Exists(filePath))
            {
                ReplyAsync("You haven't created a profile");
                return;
            }
            
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            var player = JsonConvert.DeserializeObject<Player.Player>(await File.ReadAllTextAsync(Context.User.Id + ".json"), settings);
            var familiar = await player.Scavenge();
            string json = JsonConvert.SerializeObject(player, settings);
            await File.WriteAllTextAsync(filePath, json);
            if (familiar == null)
            {
                await ReplyAsync("You can't scavenge a familiar");
            }
            else
            {
                await ReplyAsync("You have successfully scavenged a " +  familiar.Name);
                await ReplyAsync(embed:familiar.Display());
            }
        }
        
        [Command("activefamiliar")]
        [Summary("Sets your active familiar")]
        public async Task SetActiveFamiliarAsync(string familiarID)
        {
            string filePath = Context.User.Id + ".json";
            if (!File.Exists(filePath))
            {
                await ReplyAsync("You haven't created a profile");
                return;
            }
            
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            var player = JsonConvert.DeserializeObject<Player.Player>(await File.ReadAllTextAsync(Context.User.Id + ".json"), settings);
            var familiar = player.familiars.FirstOrDefault(f => f.FamiliarID == familiarID);
            if (familiar == null)
            {
                await ReplyAsync("You don't have a familiar with that ID");
                return;
            }
            
            foreach (var fam in player.familiars)
            {
                fam.ActiveFamiliar = false;
            }
            
            familiar.ActiveFamiliar = true;
            string json = JsonConvert.SerializeObject(player, settings);
            await File.WriteAllTextAsync(filePath, json);
            await ReplyAsync("You have successfully set " + familiar.Name + " as your active familiar");
        }
        
        [Command("familiar")]
        [Summary("Displays your active familiar")]
        public async Task DisplayActiveFamiliarAsync()
        {
            string filePath = Context.User.Id + ".json";
            if (!File.Exists(filePath))
            {
                await ReplyAsync("You haven't created a profile");
                return;
            }
            
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            var player = JsonConvert.DeserializeObject<Player.Player>(await File.ReadAllTextAsync(Context.User.Id + ".json"), settings);
            var activeFamiliar = player.familiars.FirstOrDefault(f => f.ActiveFamiliar);
            if (activeFamiliar == null)
            {
                await ReplyAsync("You don't have an active familiar");
                return;
            }
            await ReplyAsync(embed:activeFamiliar.Display());
        }
    }
}