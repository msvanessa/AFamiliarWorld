using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace AFamiliarWorld.Bot.Commands;

public class FamiliarBattles
{
    public class FamiliarBattleModule : ModuleBase<SocketCommandContext>
    {
        [Command("challenge")]
        [Summary("Challenges another user to a familiar battle")]
        public async Task ChallengeAsync(SocketUser user)
        {
            try
            {


                if (user.IsBot)
                {
                    await ReplyAsync("You cannot challenge a bot.");
                    return;
                }


                await ReplyAsync($"You have challenged {user.Username} to a familiar battle!");

                
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };
                if (!File.Exists(Context.User.Id + ".json") || !File.Exists(user.Id + ".json"))
                {
                    await ReplyAsync(
                        "Both players must have a profile created. Use !createplayer to create a profile.");
                    return;
                }

                var player =
                    JsonConvert.DeserializeObject<Player.Player>(File.ReadAllText(Context.User.Id + ".json"), settings);
                var opponent =
                    JsonConvert.DeserializeObject<Player.Player>(File.ReadAllText(user.Id + ".json"), settings);
                var activeFamiliar = player.familiars.FirstOrDefault(f => f.ActiveFamiliar);
                if (activeFamiliar == null)
                {
                    await ReplyAsync("You don't have an active familiar");
                    return;
                }

                var opponentActiveFamiliar = opponent.familiars.FirstOrDefault(f => f.ActiveFamiliar);
                if (opponentActiveFamiliar == null)
                {
                    await ReplyAsync($"{user.Username} doesn't have an active familiar");
                    return;
                }
                
                await ReplyAsync($"{Context.User.Username}'s {activeFamiliar.Name} :crossed_swords: {user.Username}'s {opponentActiveFamiliar.Name}");

                var random = new Random();
                var firstAttackerName = activeFamiliar.Speed >= opponentActiveFamiliar.Speed ? Context.User.Username : user.Username;
                var secondAttackerName = activeFamiliar.Speed >= opponentActiveFamiliar.Speed ? Context.User.Username : user.Username;
                var firstAttacker = activeFamiliar.Speed >= opponentActiveFamiliar.Speed ? activeFamiliar : opponentActiveFamiliar;
                var secondAttacker = activeFamiliar.Speed >= opponentActiveFamiliar.Speed ? activeFamiliar : opponentActiveFamiliar;

                if (firstAttacker == secondAttacker)
                {
                    var goingFirst = random.Next(1, 3);
                    firstAttacker = goingFirst == 1 ? activeFamiliar : opponentActiveFamiliar;
                    firstAttackerName = goingFirst == 1 ? Context.User.Username : user.Username;
                    secondAttacker = goingFirst == 2 ? activeFamiliar : opponentActiveFamiliar;
                    secondAttackerName = goingFirst == 2 ? Context.User.Username : user.Username;
                }

                while (firstAttacker.Health >= 0 && secondAttacker.Health >= 0)
                {
                    var attack = await firstAttacker.Attack();
                    var defend = await secondAttacker.Defend(attack);
                    secondAttacker.Health -= defend;
                    var criticalHit = attack.CriticalHit == true ? "**" : "";
                    await ReplyAsync(
                        $"{criticalHit}{firstAttackerName}'s {firstAttacker.Name} attacks {secondAttackerName}'s {secondAttacker.Name} with {attack.AbilityName} for {defend} damage! {secondAttackerName}'s {secondAttacker.Name} has {secondAttacker.Health} health remaining.{criticalHit}");
                    if (secondAttacker.Health <= 0)
                    {
                        await ReplyAsync($"{firstAttackerName}'s {firstAttacker.Name} has defeated {secondAttackerName}'s {secondAttacker.Name}!");
                        break;
                    }
                    await Task.Delay(3000);
                    attack = await secondAttacker.Attack();
                    defend = await firstAttacker.Defend(attack);
                    firstAttacker.Health -= defend;
                    criticalHit = attack.CriticalHit == true ? "**" : "";
                    await ReplyAsync(
                        $"{criticalHit}{secondAttackerName}'s {secondAttacker.Name} attacks {firstAttackerName}'s {firstAttacker.Name} with {attack.AbilityName} for {defend} damage! {firstAttackerName}'s {firstAttacker.Name} has {firstAttacker.Health} health remaining.{criticalHit}");
                    if (firstAttacker.Health <= 0)
                    {
                        await ReplyAsync($"{secondAttackerName}'s {secondAttacker.Name} has defeated {firstAttackerName}'s {firstAttacker.Name}!");
                        break;
                    }

                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync($"An error occurred: {ex.Message}");
            }
        }

            private async Task<SocketMessage> WaitForMessageAsync(SocketUser user, IMessageChannel channel, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<SocketMessage>();

            Task Handler(SocketMessage message)
            {
                if (message.Author.Id == user.Id && message.Channel.Id == channel.Id)
                {
                    tcs.TrySetResult(message);
                }
                return Task.CompletedTask;
            }

            Context.Client.MessageReceived += Handler;
            var delayTask = Task.Delay(timeout);
            var completedTask = await Task.WhenAny(tcs.Task, delayTask);
            Context.Client.MessageReceived -= Handler;

            return completedTask == tcs.Task ? await tcs.Task : null;
        }
    }
    
}