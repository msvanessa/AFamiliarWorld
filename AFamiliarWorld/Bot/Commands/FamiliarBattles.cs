using System.Text;
using AFamiliarWorld.Bot.BattleGenerator;
using AFamiliarWorld.Bot.Commands.Models;
using AFamiliarWorld.Bot.Familiars;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Rest;
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
                
                await ReplyAsync($"You have challenged {user.Username} to a familiar battle! <@{user.Id}>, type 'accept' to accept the challenge within 30 seconds.");
                var response = await WaitForMessageAsync(user, Context.Channel, TimeSpan.FromSeconds(30));
                if (response == null || !response.Content.Equals("accept", StringComparison.OrdinalIgnoreCase))
                {
                    await ReplyAsync($"{user.Username} did not accept the challenge in time.");
                    return;
                }
                
                await ReplyAsync($"{user.Username} accepted the challenge!");
                await Context.Channel.TriggerTypingAsync();
                var pvpImage = new PvPImage();
                var winner = await Dual(Context, user, pvpImage);
                if (winner == null) return;
                
                var winnerPlayer = FileManager.FetchUserData(winner.Id);
                winnerPlayer.Gold += 50;
                FileManager.SaveUserData(winner.Id, winnerPlayer);
                
                var loser = winner.Id == Context.User.Id ? user : Context.User;
                var loserPlayer = FileManager.FetchUserData(loser.Id);
                loserPlayer.Gold -= 50;
                FileManager.SaveUserData(loser.Id, loserPlayer);
                
                await Context.Channel.SendFileAsync(await pvpImage.CompileMemoryStreams(), "pvp_battle.gif");
                var fileBytes = Encoding.UTF8.GetBytes(string.Join("\n", debugOutput));
                var fileStream = new MemoryStream(fileBytes);
                await Context.Channel.SendFileAsync(fileStream, "debug_output.txt");
                debugOutput.Clear();
            }
            catch (Exception ex)
            {
                await ReplyAsync($"An error occurred: {ex.Message}");
            }
        }

        private async Task<SocketUser?> Dual(SocketCommandContext Context, SocketUser user, PvPImage pvpImage)
        { 
            var player = FileManager.FetchUserData(Context.User.Id);
            var opponent = FileManager.FetchUserData(user.Id);
            if (player == null || opponent == null)
            {
                await ReplyAsync("One of the players does not have a profile. Please create a profile using the `!createplayer` command.");
                return null;
            }   
            
            var activeFamiliar = player.familiars.FirstOrDefault(f => f.ActiveFamiliar);
            if (activeFamiliar == null)
            {
                await ReplyAsync("You don't have an active familiar");
                return null;
            }

            var opponentActiveFamiliar = opponent.familiars.FirstOrDefault(f => f.ActiveFamiliar);
            if (opponentActiveFamiliar == null)
            {
                await ReplyAsync($"{user.Username} doesn't have an active familiar");
                return null;
            }
            
            var (firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser) = await CalculateSpeed(activeFamiliar, Context.User, opponentActiveFamiliar, user);
            var firstAttackerSpeed = firstAttacker.Speed;
            var secondAttackerSpeed = secondAttacker.Speed;
            await pvpImage.GeneratePvPImage(firstAttacker, secondAttacker, $"{Context.User.Username}'s {activeFamiliar.Name} VS {user.Username}'s {opponentActiveFamiliar.Name}");
            while (true)
            {
                if (firstAttacker.Speed != firstAttackerSpeed || secondAttacker.Speed != secondAttackerSpeed) // If the speed of either familiar changes, recalculate the turn order
                {
                    (firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser) = await CalculateSpeed(activeFamiliar, Context.User, opponentActiveFamiliar, user);
                    firstAttackerSpeed = firstAttacker.Speed;
                    secondAttackerSpeed = secondAttacker.Speed;
                }
                
                var winner = await DoTurn(firstAttacker, secondAttacker, firstAttacker, secondAttacker, firstAttackerUser, secondAttackerUser, Context, pvpImage);
                if (winner.Item1 != null && winner.Item2 != null)
                {
                    return winner.Item2;
                };
                winner = await DoTurn(secondAttacker, firstAttacker, firstAttacker, secondAttacker, secondAttackerUser, firstAttackerUser, Context, pvpImage);
                if (winner.Item1 != null && winner.Item2 != null)
                {
                    return winner.Item2;
                };
                // (Familiar familiarToCheck, Familiar firstAttacker, Familiar secondAttacker, SocketUser user, SocketCommandContext Context)
                await CheckStatusCondition(firstAttacker, firstAttacker, secondAttacker, firstAttackerUser, pvpImage);
                await CheckStatusCondition(secondAttacker, firstAttacker, secondAttacker, secondAttackerUser, pvpImage);
                
                var firstWinner = await CheckWinner(firstAttacker, secondAttacker, firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser, pvpImage);
                if (firstWinner)
                {
                    FileManager.UpdateStatisticLogger(firstAttacker, secondAttacker, WinCondition.StatusCondition);
                    return firstAttackerUser;
                }
                
                var secondWinner = await CheckWinner(firstAttacker, secondAttacker, secondAttacker, secondAttackerUser, firstAttacker, firstAttackerUser, pvpImage);
                if (secondWinner)
                {
                    FileManager.UpdateStatisticLogger(secondAttacker, firstAttacker, WinCondition.StatusCondition);
                    return secondAttackerUser;
                }
            }
        }
        private static async Task<(Familiar, SocketUser, Familiar, SocketUser)> CalculateSpeed(Familiar challengerFamiliar, SocketUser challengerUser, Familiar challengedFamiliar, SocketUser challengedUser)
        {
            var firstAttackerUser = challengerFamiliar.Speed >= challengedFamiliar.Speed ? challengerUser : challengedUser;
            var secondAttackerUser = challengerFamiliar.Speed >= challengedFamiliar.Speed ? challengerUser : challengedUser;
            var firstAttacker = challengerFamiliar.Speed >= challengedFamiliar.Speed ? challengerFamiliar : challengedFamiliar;
            var secondAttacker = challengerFamiliar.Speed >= challengedFamiliar.Speed ? challengerFamiliar : challengedFamiliar;
            var random = new Random();
            if (firstAttacker == secondAttacker)
            {
                var goingFirst = random.Next(1, 3);
                firstAttacker = goingFirst == 1 ? challengerFamiliar : challengedFamiliar;
                firstAttackerUser = goingFirst == 1 ? challengerUser : challengedUser;
                secondAttacker = goingFirst == 2 ? challengerFamiliar : challengedFamiliar;
                secondAttackerUser = goingFirst == 2 ? challengerUser : challengedUser;
            }
            return (firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser);
        }
        
        private async Task<(Familiar?, SocketUser?)> DoTurn(Familiar attacker, Familiar defender, Familiar firstAttacker, Familiar secondAttacker,
            SocketUser attackerUser, SocketUser defenderUser,
            SocketCommandContext context, PvPImage pvpImage)
        {
            var firstAttackerStatusConditions = await attacker.GetStatusConditions();
            var random = new Random();
            if (!firstAttackerStatusConditions.Contains(StatusCondition.Stun))
            {
                var confused = random.Next(1, 101) <= 50;
                if ((await attacker.GetStatusConditions()).Contains(StatusCondition.Confuse) && confused)
                {
                    
                    attacker.Health -= 20;
                    await UpdateImage(firstAttacker, secondAttacker,
                        $"{attackerUser.Username}'s {attacker.Name} is confused and hurts itself in its confusion for 20 damage!", pvpImage);
                }
                else
                {
                    var attack = await attacker.Attack(defender);
                    var defend = await defender.Defend(attack);
                    defender.Health -= defend.DamageTaken;

                    var criticalHit = attack.CriticalHit ? "***" : "";
                    var output = attack.CustomOutput ?? $"{criticalHit}{attackerUser.Username}'s {attacker.Name} attacks using {attack.AbilityName} for {defend.DamageTaken} damage!{criticalHit}";
                
                    await UpdateImage(firstAttacker, secondAttacker, output, pvpImage);
            

                    var winner = await CheckWinner(firstAttacker, secondAttacker, attacker, attackerUser, defender, defenderUser, pvpImage);
                    if (winner)
                    {
                        FileManager.UpdateStatisticLogger(attacker, defender, WinCondition.NormalDamage);
                        return (attacker, attackerUser);
                    }
                    winner = await CheckWinner(firstAttacker, secondAttacker, defender, defenderUser, attacker, attackerUser, pvpImage);
                    if (winner)
                    {
                        FileManager.UpdateStatisticLogger(defender, attacker, WinCondition.NormalDamage);
                        return (defender, defenderUser);
                    }

                    if (defend.IsReflecting)
                    {
                        if (!attack.IsTrueDamage && attack.Damage != 0)
                        {
                            attacker.Health -= defend.DamageReflected;
                            await UpdateImage(firstAttacker, secondAttacker,
                                $"{defenderUser.Username}'s {defender.Name} reflects {defend.DamageReflected} damage back using its {defend.DamageReflectedMessage}!",
                                pvpImage);

                            winner = await CheckWinner(firstAttacker, secondAttacker, defender, defenderUser, attacker,
                                attackerUser, pvpImage);
                            if (winner)
                            {
                                FileManager.UpdateStatisticLogger(defender, attacker, WinCondition.ReflectedDamage);
                                return (defender, defenderUser);

                            }
                        }
                    }
                   
                    winner = await CheckWinner(firstAttacker, secondAttacker, defender, defenderUser, attacker,
                        attackerUser, pvpImage);
                    if(winner) return (defender, defenderUser);
            
                }
            }
            else
            {
                await attacker.RemoveStatusCondition(StatusCondition.Stun);
                await UpdateImage(firstAttacker, secondAttacker,
                    $"{attackerUser.Username}'s {attacker.Name} is stunned and cannot attack this turn.", pvpImage);

            }
            return (null, null);
        }
        private List<string> debugOutput = new List<string>();
        private async Task UpdateImage(Familiar firstAttacker, Familiar secondAttacker, string battleText, PvPImage pvpImage)
        {
            await pvpImage.GeneratePvPImage(firstAttacker, secondAttacker, battleText);
            debugOutput.Add(battleText);
        }
        
        private async Task CheckStatusCondition(Familiar familiarToCheck, Familiar firstAttacker, Familiar secondAttacker, SocketUser user, PvPImage pvpImage)
        {
            var removeConfuse = false;
            foreach (var statusCondition in (await familiarToCheck.GetStatusConditions()).Distinct())
            {
                switch (statusCondition)
                {
                    case StatusCondition.Burn:
                        familiarToCheck.Health -= 20;

                        await UpdateImage(firstAttacker, secondAttacker,
                            $"{user.Username}'s {familiarToCheck.Name} is burning! They take 20 fire damage.", pvpImage);
                        break;
                    case StatusCondition.Poison:
                        var damage = ((await familiarToCheck.GetStatusConditions()).Count(s=>s == StatusCondition.Poison)) * 10;
                        familiarToCheck.Health -= damage;
                        
                        await UpdateImage(firstAttacker, secondAttacker,
                            $"{user.Username}'s {familiarToCheck.Name} is poisoned! They take {damage} poison damage.", pvpImage);
                        break;
                    case StatusCondition.Confuse:
                        var random = new Random();
                        removeConfuse = random.Next(1, 101) <= 20;

                        break;
                    case StatusCondition.None:
                        break;
                }
            }
            if (removeConfuse)
            {
                await familiarToCheck.RemoveStatusCondition(StatusCondition.Confuse);
                await UpdateImage(firstAttacker, secondAttacker,
                    $"{user.Username}'s {familiarToCheck.Name} snaps out of confusion!", pvpImage);
            }
        }
        
        private async Task<bool> CheckWinner(Familiar firstAttacker, Familiar secondAttacker, Familiar winnerFamiliar, SocketUser winnerUser, Familiar loserFamiliar, SocketUser loserUser, PvPImage pvpImage)
        {
            if (loserFamiliar.Health <= 0)
            {
                await UpdateImage(firstAttacker, secondAttacker,
                    $"{winnerUser.Username}'s {winnerFamiliar.Name} wins! {winnerUser.Username} gains 50 of {loserUser.Username}'s Gold !", pvpImage);
                return true;
            }
            return false;
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