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
                
                
                var player = FileManager.FetchUserData(Context.User.Id);
                var opponent = FileManager.FetchUserData(user.Id);
                if (player == null || opponent == null)
                {
                    await ReplyAsync("One of the players does not have a profile. Please create a profile using the `!createplayer` command.");
                    return;
                }   
                
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
                
                var embed = new EmbedBuilder();
                embed.WithColor(Discord.Color.Red);
                embed.WithThumbnailUrl("https://cdn.discordapp.com/attachments/803309924746395691/1376828611625226351/vs-or-versus-sign-competition-symbol-vector.jpg?ex=6836bf11&is=68356d91&hm=63c3f47d344d24890ab6e3b91b8ad8ec4f4587ea081d8ad831e4ac88350fcb01&");
                embed.WithTitle($"{Context.User.Username}'s {activeFamiliar.Name} :crossed_swords: {user.Username}'s {opponentActiveFamiliar.Name}");
                var reply = await Context.Channel.SendMessageAsync(embed: embed.Build());
                
                var (firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser) = await CalculateSpeed(activeFamiliar, Context.User, opponentActiveFamiliar, user);
                var firstAttackerSpeed = firstAttacker.Speed;
                var secondAttackerSpeed = secondAttacker.Speed;
                while (true)
                {
                    if (firstAttacker.Speed != firstAttackerSpeed || secondAttacker.Speed != secondAttackerSpeed) // If the speed of either familiar changes, recalculate the turn order
                    {
                        (firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser) = await CalculateSpeed(activeFamiliar, Context.User, opponentActiveFamiliar, user);
                        firstAttackerSpeed = firstAttacker.Speed;
                        secondAttackerSpeed = secondAttacker.Speed;
                    }
                    
                    var firstAttackerIsWinner = await DoTurn(firstAttacker, secondAttacker, firstAttackerUser, secondAttackerUser, embed, reply, Context);
                    if (firstAttackerIsWinner) break;
                    
                    await Task.Delay(3000);
                    
                    var secondAttackerIsWinner = await DoTurn(secondAttacker, firstAttacker, secondAttackerUser, firstAttackerUser, embed, reply, Context);
                    if (secondAttackerIsWinner) break;
                    
                    await CheckStatusCondition(firstAttacker, firstAttackerUser, embed, reply);
                    await CheckStatusCondition(secondAttacker, secondAttackerUser, embed, reply);
                    
                    var firstWinner = CheckWinner(Context, firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser);
                    if (firstWinner) break;
                    
                    var secondWinner = CheckWinner(Context, secondAttacker, secondAttackerUser, firstAttacker, firstAttackerUser);
                    if (secondWinner) break;
                    if (embed.Fields.Count > 20)
                    {
                        embed = new EmbedBuilder();
                        embed.WithColor(Discord.Color.Red);
                        embed.WithThumbnailUrl(
                            "https://cdn.discordapp.com/attachments/803309924746395691/1376828611625226351/vs-or-versus-sign-competition-symbol-vector.jpg?ex=6836bf11&is=68356d91&hm=63c3f47d344d24890ab6e3b91b8ad8ec4f4587ea081d8ad831e4ac88350fcb01&");
                        embed.WithTitle(
                            $"{Context.User.Username}'s {activeFamiliar.Name} :crossed_swords: {user.Username}'s {opponentActiveFamiliar.Name}");
                        reply = await Context.Channel.SendMessageAsync(embed: embed.Build());
                    }

                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync($"An error occurred: {ex.Message}");
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
        
        private static async Task<bool> DoTurn(Familiar firstAttacker, Familiar secondAttacker, SocketUser firstAttackerUser, SocketUser secondAttackerUser, EmbedBuilder embed, RestUserMessage reply, SocketCommandContext context)
        {
            var firstAttackerStatusConditions = await firstAttacker.GetStatusConditions();
            var random = new Random();
            if (!firstAttackerStatusConditions.Contains(StatusCondition.Stun))
            {
                var confused = random.Next(1, 101) <= 50;
                if ((await firstAttacker.GetStatusConditions()).Contains(StatusCondition.Confuse) && confused)
                {
                    
                    firstAttacker.Health -= 3;
                    embed.WithFields().AddField(
                        $"{firstAttackerUser.Username}'s {firstAttacker.Name} is confused and hurts itself in its confusion for 3 damage!",
                        $"{firstAttackerUser.Username}'s {firstAttacker.Name} has {firstAttacker.Health} health remaining.");
                    await reply.ModifyAsync(
                        new Action<MessageProperties>(props => { props.Embed = embed.Build(); }));
                }
                else
                {
                    var attack = await firstAttacker.Attack();
                    var defend = await secondAttacker.Defend(attack);
                    secondAttacker.Health -= defend.DamageTaken;

                    var criticalHit = attack.CriticalHit == true ? "***" : "";
                    embed.WithFields().AddField(
                        $"{criticalHit}{firstAttackerUser.Username}'s {firstAttacker.Name} attacks {secondAttackerUser.Username}'s {secondAttacker.Name} with {attack.AbilityName} for {defend.DamageTaken} damage!{criticalHit}",
                        $"{secondAttackerUser.Username}'s {secondAttacker.Name} has {secondAttacker.Health} health remaining.");
                    await reply.ModifyAsync(
                        new Action<MessageProperties>(props => { props.Embed = embed.Build(); }));

                    var winner = CheckWinner(context, secondAttacker, secondAttackerUser, firstAttacker,
                        firstAttackerUser);
                    if (winner) return winner;
                    winner = CheckWinner(context, firstAttacker, firstAttackerUser, secondAttacker,
                        secondAttackerUser);
                    if (winner) return winner;

                    if (defend.IsReflecting)
                    {
                        firstAttacker.Health -= defend.DamageReflected;
                        embed.WithFields().AddField(
                            $"{firstAttackerUser.Username}'s {firstAttacker.Name} reflects {defend.DamageReflected} damage back to {secondAttackerUser.Username}'s {secondAttacker.Name} using its {defend.DamageReflectedMessage}!",
                            $"{secondAttackerUser.Username}'s {secondAttacker.Name} has {secondAttacker.Health} health remaining.");
                    }

                    winner = CheckWinner(context, secondAttacker, firstAttackerUser, firstAttacker,
                        firstAttackerUser);
                    return winner;
            
                }
            }
            else
            {
                embed.WithFields().AddField("Stunned!", $"{firstAttackerUser.Username}'s {firstAttacker.Name} is stunned and cannot attack this turn.");
                await reply.ModifyAsync(
                    new Action<MessageProperties>(props => { props.Embed = embed.Build(); }));
                await firstAttacker.RemoveStatusCondition(StatusCondition.Stun);
            }
            return false;
        }
        
        private static async Task CheckStatusCondition(Familiar familiar, SocketUser user, EmbedBuilder embed, RestUserMessage reply)
        {
            var removeConfuse = false;
            foreach (var statusCondition in (await familiar.GetStatusConditions()).Distinct())
            {
                switch (statusCondition)
                {
                    case StatusCondition.Burn:
                        familiar.Health -= 2;
                        embed.WithFields().AddField($"{user.Username}'s {familiar.Name} is burning! They take 2 fire damage.", $"{user.Username}'s {familiar.Name} has {familiar.Health} health remaining.");
                        await reply.ModifyAsync(
                            new Action<MessageProperties>(props => { props.Embed = embed.Build(); }));
                        break;
                    case StatusCondition.Poison:
                        var damage = (await familiar.GetStatusConditions()).Count(s=>s == StatusCondition.Poison);
                        familiar.Health -= damage;
                        embed.WithFields().AddField($"{user.Username}'s {familiar.Name} is poisoned! They take {damage} poison damage.", $"{user.Username}'s {familiar.Name} has {familiar.Health} health remaining.");
                        await reply.ModifyAsync(
                            new Action<MessageProperties>(props => { props.Embed = embed.Build(); }));
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
                await familiar.RemoveStatusCondition(StatusCondition.Confuse);
                embed.WithFields().AddField($"{user.Username}'s {familiar.Name} snaps out of confusion!", $"{user.Username}'s {familiar.Name} is no longer confused.");
                await reply.ModifyAsync(
                    new Action<MessageProperties>(props => { props.Embed = embed.Build(); }));
            }
        }
        
        private static bool CheckWinner(SocketCommandContext context, Familiar winnerFamiliar, SocketUser winnerUser, Familiar loserFamiliar, SocketUser loserUser)
        {
            if (loserFamiliar.Health <= 0)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Discord.Color.Red);
                embed.WithTitle($"{winnerUser.Username}'s {winnerFamiliar.Name} wins!");
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