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
                
                var random = new Random();
                var firstAttackerUser = activeFamiliar.Speed >= opponentActiveFamiliar.Speed ? Context.User : user;
                var secondAttackerUser = activeFamiliar.Speed >= opponentActiveFamiliar.Speed ? Context.User : user;
                var firstAttacker = activeFamiliar.Speed >= opponentActiveFamiliar.Speed ? activeFamiliar : opponentActiveFamiliar;
                var secondAttacker = activeFamiliar.Speed >= opponentActiveFamiliar.Speed ? activeFamiliar : opponentActiveFamiliar;

                if (firstAttacker == secondAttacker)
                {
                    var goingfirst = random.Next(1, 3);
                    firstAttacker = goingfirst == 1 ? activeFamiliar : opponentActiveFamiliar;
                    firstAttackerUser = goingfirst == 1 ? Context.User : user;
                    secondAttacker = goingfirst == 2 ? activeFamiliar : opponentActiveFamiliar;
                    secondAttackerUser = goingfirst == 2 ? Context.User : user;
                }

                while (firstAttacker.Health >= 0 && secondAttacker.Health >= 0)
                {
                    var firstAttackerIsWinner = await DoTurn(firstAttacker, secondAttacker, firstAttackerUser, secondAttackerUser, embed, reply, Context);
                    if (firstAttackerIsWinner) break;
                    
                    await Task.Delay(3000);
                    
                    var secondAttackerIsWinner = await DoTurn(secondAttacker, firstAttacker, secondAttackerUser, firstAttackerUser, embed, reply, Context);
                    if (secondAttackerIsWinner) break;
                    
                    await CheckStatusCondition(firstAttacker, firstAttackerUser, embed);
                    await CheckStatusCondition(secondAttacker, secondAttackerUser, embed);
                    
                    var firstWinner = CheckWinner(Context, firstAttacker, firstAttackerUser, secondAttacker, secondAttackerUser);
                    if (firstWinner) break;
                    
                    var secondWinner = CheckWinner(Context, secondAttacker, secondAttackerUser, firstAttacker, firstAttackerUser);
                    if (secondWinner) break;
                    
                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync($"An error occurred: {ex.Message}");
            }
        }

        private static async Task<bool> DoTurn(Familiar firstAttacker, Familiar secondAttacker, SocketUser firstAttackerUser, SocketUser secondAttackerUser, EmbedBuilder embed, RestUserMessage reply, SocketCommandContext context)
        {
            var secondAttackerStatusConditions = await secondAttacker.GetStatusConditions();
            if (!secondAttackerStatusConditions.Contains(StatusCondition.Stun))
            {
                var attack = await secondAttacker.Attack();
                var defend = await firstAttacker.Defend(attack);
                firstAttacker.Health -= defend;
                var criticalHit = attack.CriticalHit == true ? "***" : "";

                embed.WithFields().AddField(
                    $"{criticalHit}{secondAttackerUser.Username}'s {secondAttacker.Name} attacks {firstAttackerUser.Username}'s {firstAttacker.Name} with {attack.AbilityName} for {defend} damage!{criticalHit}",
                    $"{firstAttackerUser.Username}'s {firstAttacker.Name} has {firstAttacker.Health} health remaining.");
                await reply.ModifyAsync(
                    new Action<MessageProperties>(props => { props.Embed = embed.Build(); }));
                var winner = CheckWinner(context, secondAttacker, secondAttackerUser, firstAttacker, firstAttackerUser);
                return winner;
            }
            else
            {
                embed.WithFields().AddField("Stunned!", $"{secondAttackerUser.Username}'s {secondAttacker.Name} is stunned and cannot attack this turn.");
                await secondAttacker.RemoveStatusCondition(StatusCondition.Stun);
            }

            return false;
        }
        private static async Task CheckStatusCondition(Familiar familiar, SocketUser user, EmbedBuilder embed)
        {
            foreach (var statusCondition in await familiar.GetStatusConditions())
            {
                switch (statusCondition)
                {
                    case StatusCondition.Burn:
                        familiar.Health -= 2;
                        embed.WithFields().AddField($"{user.Username}'s {familiar.Name} is burning! They take 2 fire damage.", $"{user.Username}'s {familiar.Name} has {familiar.Health} health remaining.");
                        break;
                    case StatusCondition.None:
                        break;
                }
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