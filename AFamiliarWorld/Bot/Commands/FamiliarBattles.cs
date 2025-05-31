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
                
                var firstAttackerIsWinner = await DoTurn(firstAttacker, secondAttacker, firstAttacker, secondAttacker, firstAttackerUser, secondAttackerUser, Context, pvpImage);
                if (firstAttackerIsWinner)
                {
                    await UpdateImage(firstAttacker, secondAttacker,
                        $"{firstAttackerUser.Username}'s {firstAttacker.Name} has won !", pvpImage);
                    return firstAttackerUser;
                };
                
                var secondAttackerIsWinner = await DoTurn(secondAttacker, firstAttacker, firstAttacker, secondAttacker, secondAttackerUser, firstAttackerUser, Context, pvpImage);
                if (secondAttackerIsWinner)
                {
                    await UpdateImage(firstAttacker, secondAttacker,
                        $"{secondAttackerUser.Username}'s {secondAttacker.Name} has won !", pvpImage);
                    return secondAttackerUser;
                };
                // (Familiar familiarToCheck, Familiar firstAttacker, Familiar secondAttacker, SocketUser user, SocketCommandContext Context)
                await CheckStatusCondition(firstAttacker, firstAttacker, secondAttacker, firstAttackerUser, Context, pvpImage);
                await CheckStatusCondition(secondAttacker, firstAttacker, secondAttacker, secondAttackerUser, Context, pvpImage);
                
                var firstWinner = CheckWinner(Context, firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser);
                if (firstWinner)
                {
                    await UpdateImage(firstAttacker, secondAttacker,
                        $"{firstAttackerUser.Username}'s {firstAttacker.Name} has won !", pvpImage);
                    return firstAttackerUser;
                }
                
                var secondWinner = CheckWinner(Context, secondAttacker, secondAttackerUser, firstAttacker, firstAttackerUser);
                if (secondWinner)
                {
                    await UpdateImage(firstAttacker, secondAttacker,
                        $"{secondAttackerUser.Username}'s {secondAttacker.Name} has won !", pvpImage);
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
        
        private async Task<bool> DoTurn(Familiar attacker, Familiar defender, Familiar firstAttacker, Familiar secondAttacker,
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
                    
                    attacker.Health -= 3;
                    await UpdateImage(firstAttacker, secondAttacker,
                        $"{attackerUser.Username}'s {attacker.Name} is confused and hurts itself in its confusion for 3 damage!", pvpImage);
                }
                else
                {
                    var attack = await attacker.Attack();
                    var defend = await defender.Defend(attack);
                    defender.Health -= defend.DamageTaken;

                    var criticalHit = attack.CriticalHit == true ? "***" : "";
                    
                    await UpdateImage(firstAttacker, secondAttacker,
                        $"{criticalHit}{attackerUser.Username}'s {attacker.Name} attacks {defenderUser.Username}'s {defender.Name} with {attack.AbilityName} for {defend.DamageTaken} damage!{criticalHit}",
                        pvpImage);

                    var winner = CheckWinner(context, defender, defenderUser, attacker,
                        attackerUser);
                    if (winner) return winner;
                    winner = CheckWinner(context, attacker, attackerUser, defender,
                        defenderUser);
                    if (winner) return winner;

                    if (defend.IsReflecting)
                    {
                        attacker.Health -= defend.DamageReflected;
                        await UpdateImage(firstAttacker, secondAttacker,
                            $"{attackerUser.Username}'s {attacker.Name} reflects {defend.DamageReflected} damage back to {defenderUser.Username}'s {defender.Name} using its {defend.DamageReflectedMessage}!",
                            pvpImage);
                    }
                   
                    winner = CheckWinner(context, defender, attackerUser, attacker,
                        attackerUser);
                    return winner;
            
                }
            }
            else
            {
                await attacker.RemoveStatusCondition(StatusCondition.Stun);
                await UpdateImage(firstAttacker, secondAttacker,
                    $"{attackerUser.Username}'s {attacker.Name} is stunned and cannot attack this turn.", pvpImage);

            }
            return false;
        }
        private List<string> debugOutput = new List<string>();
        private async Task UpdateImage(Familiar firstAttacker, Familiar secondAttacker, string battleText, PvPImage pvpImage)
        {
            await pvpImage.GeneratePvPImage(firstAttacker, secondAttacker, battleText);
            debugOutput.Add(battleText);
        }
        
        private async Task CheckStatusCondition(Familiar familiarToCheck, Familiar firstAttacker, Familiar secondAttacker, SocketUser user, SocketCommandContext Context, PvPImage pvpImage)
        {
            var removeConfuse = false;
            foreach (var statusCondition in (await familiarToCheck.GetStatusConditions()).Distinct())
            {
                switch (statusCondition)
                {
                    case StatusCondition.Burn:
                        familiarToCheck.Health -= 2;

                        await UpdateImage(firstAttacker, secondAttacker,
                            $"{user.Username}'s {familiarToCheck.Name} is burning! They take 2 fire damage.", pvpImage);
                        break;
                    case StatusCondition.Poison:
                        var damage = (await familiarToCheck.GetStatusConditions()).Count(s=>s == StatusCondition.Poison);
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
        
        private static bool CheckWinner(SocketCommandContext context, Familiar winnerFamiliar, SocketUser winnerUser, Familiar loserFamiliar, SocketUser loserUser)
        {
            if (loserFamiliar.Health <= 0)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Discord.Color.Red);
                embed.WithTitle($"{winnerUser.Username}'s {winnerFamiliar.Name} wins! {winnerUser.Username} gains 50 of {loserUser.Username}'s Gold !");
                embed.WithImageUrl("https://cdn.discordapp.com/attachments/803309924746395691/1376829532660568064/victory-pop-up-golden-assets-award-with-crown-for-game-illustration-golden-banner-with-wings-and-red-flags-vector.jpg?ex=6836bfec&is=68356e6c&hm=ffe35dc277c4b053ae396cf905581e05767837143ef2fffa6305203be11c08e7&");
                embed.WithThumbnailUrl(winnerUser.GetAvatarUrl());
                context.Channel.SendMessageAsync(embed: embed.Build());
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