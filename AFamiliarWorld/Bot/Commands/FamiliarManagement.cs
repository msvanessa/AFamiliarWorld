using System.Text;
using AFamiliarWorld.Bot.Familiars;
using AFamiliarWorld.Bot.Familiars.StarterFamiliars;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace AFamiliarWorld.Bot.Commands;

public class FamiliarManagement
{
    public class FamiliarManagementModule : ModuleBase<SocketCommandContext>
    {
        [Command("createplayer")]
        [Summary("Creates a new player")]
        public async Task CreatePlayerAsync()
        {
            try
            {
                string filePath = Context.User.Id + ".json";
                if (File.Exists(filePath))
                {
                    await ReplyAsync("You already have a character");
                    return;
                }

                List<Type> StarterFamiliars = new List<Type>{typeof(Imp), typeof(PaperCraneGolem), typeof(CrystalBeetle)};
                var random = new Random();
                var player = new Player.Player();
                player.familiars = new List<Familiar>();
                
                // DELETEME
                var DELETEME = new List<Type>(){typeof(Bandcoon), typeof(Imp), typeof(PaperCraneGolem), typeof(CrystalBeetle), typeof(Batnana), typeof(BookMimic), typeof(Clockroach), typeof(LandShork), typeof(NoodleCrab), typeof(Pebblewyrm), typeof(Ropopus), typeof(SkeleMouse), typeof(SlimeCat), typeof(StarRaven)};
                foreach (var fam in DELETEME)
                {
                    var newFamiliar = (Familiar) Activator.CreateInstance(fam);
                    await newFamiliar.CalculateIVS();
                    player.familiars.Add(newFamiliar);
                }
                // DELETEME
                
                var familiar = (Familiar) Activator.CreateInstance(StarterFamiliars[random.Next(0, StarterFamiliars.Count)]);
                await familiar.CalculateIVS();
                player.familiars.Add(familiar);
                await ReplyAsync(embed:familiar.Display());
                
                FileManager.SaveUserData(Context.User.Id, player);
                await ReplyAsync("You have successfully created a new character");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
            return;
        }

        [Command("displayfamiliars")]
        [Summary("Displays your familiars")]
        public Task DisplayFamiliarsAsync()
        {
            var player = FileManager.FetchUserData(Context.User.Id);
            if (player == null)
            {
                ReplyAsync("You haven't created a profile");
                return Task.CompletedTask;
            }
            
            if (player.familiars.Count == 0)
            {
                ReplyAsync("You don't have any familiars yet!");
                return Task.CompletedTask;
            }
            var stringBuilder = new StringBuilder();
            foreach (var fam in player.familiars)
            {
                stringBuilder.AppendLine(fam.Name + ", Familiar ID: `" + fam.FamiliarID + "`");
                if(stringBuilder.ToString().Length > 1900)
                {
                    ReplyAsync(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }
            ReplyAsync(stringBuilder.ToString());
            
            return Task.CompletedTask;
        }

        [Command("scavenge")]
        [Summary("Scavenges a familiar")]
        public async Task ScavengeAsync()
        {
            var player = FileManager.FetchUserData(Context.User.Id);
            if (player == null)
            {
                await ReplyAsync("You haven't created a profile");
                return;
            }
            
            var (familiar, Gold) = await player.Scavenge();
            
            if (familiar == null)
            {
                await ReplyAsync("You can't scavenge a familiar");
            }
            else
            {
                await familiar.CalculateIVS();
                FileManager.SaveUserData(Context.User.Id, player);
                await ReplyAsync("You have successfully scavenged a " +  familiar.Name + " and gained " + Gold + " gold!");
                await ReplyAsync(embed:familiar.Display());
            }
        }
        
        [Command("activefamiliar")]
        [Summary("Sets your active familiar")]
        public async Task SetActiveFamiliarAsync(string familiarID)
        {
            var player = FileManager.FetchUserData(Context.User.Id);
            if (player == null)
            {
                await ReplyAsync("You haven't created a profile");
                return;
            }
            
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
            FileManager.SaveUserData(Context.User.Id, player);
            await ReplyAsync("You have successfully set " + familiar.Name + " as your active familiar");
        }
        
        [Command("familiar")]
        [Summary("Displays your active familiar")]
        public async Task DisplayActiveFamiliarAsync()
        {
            var player = FileManager.FetchUserData(Context.User.Id);
            if (player == null)
            {
                await ReplyAsync("You haven't created a profile");
                return;
            }
            
            var activeFamiliar = player.familiars.FirstOrDefault(f => f.ActiveFamiliar);
            if (activeFamiliar == null)
            {
                await ReplyAsync("You don't have an active familiar");
                return;
            }
            await ReplyAsync(embed:activeFamiliar.Display());
        }
        
        [Command("profile")]
        [Summary("Displays your player profile")]
        public async Task DisplayProfileAsync()
        {
            var player = FileManager.FetchUserData(Context.User.Id);
            if (player == null)
            {
                await ReplyAsync("You haven't created a profile"); 
                return;
            }
    
            // Join familiar names and stats with newlines to separate each familiar
            var familiarNames = string.Join("\n", player.familiars.Select(f => $"{f.Emoji} {(f.ActiveFamiliar ? "*":"")}{f.Name}"));
            var familiarStats = string.Join("\n", player.familiars.Select(f => $"{f.Power}/{f.Physique}/{f.Willpower}/{f.Resolve}/{f.Luck}/{f.Health}/{f.Speed}"));
    
            var embedBuilder = new EmbedBuilder()
            {
                Title = $"{Context.User.GlobalName}'s Profile",
                ThumbnailUrl = Context.User.GetAvatarUrl(),
                Color = Discord.Color.Gold,
                Footer = new EmbedFooterBuilder()
                {
                    Text = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - player.timeSinceLastScavenge > 1800 ? "Scavenge is available" : "Scavenge is unavailable"
                },
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = @"**Familiars**",
                        Value = familiarNames,
                        IsInline = true
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = @"**Pwr/Phy/Wil/Res/Luc/Hp/Spd**",
                        Value = familiarStats,
                        IsInline = true
                    }
                },
                Description = $":moneybag: {player.Gold}"
            };
    
            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}