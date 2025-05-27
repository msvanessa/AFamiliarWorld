using AFamiliarWorld.Bot.Commands.Models;
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


                await ReplyAsync($"You have challenged {user.Username} to a familiar battle! <@{user.Id}>, type 'accept' to accept the challenge within 30 seconds.");
                var response = await WaitForMessageAsync(user, Context.Channel, TimeSpan.FromSeconds(30));
                if (response == null || !response.Content.Equals("accept", StringComparison.OrdinalIgnoreCase))
                {
                    await ReplyAsync($"{user.Username} did not accept the challenge in time.");
                    return;
                }
                
                await ReplyAsync($"{user.Username} accepted the challenge!");
                
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
                    var firstAttackerStatusConditions = await firstAttacker.GetStatusConditions();
                    if (!firstAttackerStatusConditions.Contains(StatusCondition.Stun))
                    {
                            
                        
                        var attack = await firstAttacker.Attack();
                        var defend = await secondAttacker.Defend(attack);
                        secondAttacker.Health -= defend;
                        var criticalHit = attack.CriticalHit == true ? "***" : "";
                        
                        embed.WithFields().AddField($"{criticalHit}{firstAttackerUser.Username}'s {firstAttacker.Name} attacks {secondAttackerUser.Username}'s {secondAttacker.Name} with {attack.AbilityName} for {defend} damage!{criticalHit}", $"{secondAttackerUser.Username}'s {secondAttacker.Name} has {secondAttacker.Health} health remaining.");
                        await reply.ModifyAsync(new Action<MessageProperties>(props =>
                        {
                            props.Embed = embed.Build();
                        }));
       
                        if (secondAttacker.Health <= 0)
                        {
                            var victorEmbed = new EmbedBuilder();
                            victorEmbed.WithColor(Discord.Color.Gold);
                            victorEmbed.WithTitle($"{firstAttackerUser.Username}'s {firstAttacker.Name} wins!");
                            victorEmbed.WithImageUrl(
                                "https://cdn.discordapp.com/attachments/803309924746395691/1376829532660568064/victory-pop-up-golden-assets-award-with-crown-for-game-illustration-golden-banner-with-wings-and-red-flags-vector.jpg?ex=6836bfec&is=68356e6c&hm=ffe35dc277c4b053ae396cf905581e05767837143ef2fffa6305203be11c08e7&");
                            victorEmbed.WithThumbnailUrl(firstAttackerUser.GetAvatarUrl());
                            await ReplyAsync(embed: victorEmbed.Build());
                            break;
                        }
                    }
                    else
                    {
                        embed.WithFields().AddField("Stunned!", $"{firstAttackerUser.Username}'s {firstAttacker.Name} is stunned and cannot attack this turn.");
                        await firstAttacker.RemoveStatusCondition(StatusCondition.Stun);
                    }
                    await Task.Delay(3000);
                    
                    var secondAttackerStatusConditions = await secondAttacker.GetStatusConditions();
                    if (!secondAttackerStatusConditions.Contains(StatusCondition.Stun))
                    {
                        var attack = await secondAttacker.Attack();
                        var defend = await firstAttacker.Defend(attack);
                        firstAttacker.Health -= defend;
                        var criticalHit = attack.CriticalHit == true ? "**" : "";

                        embed.WithFields().AddField(
                            $"{criticalHit}{secondAttackerUser.Username}'s {secondAttacker.Name} attacks {firstAttackerUser.Username}'s {firstAttacker.Name} with {attack.AbilityName} for {defend} damage!{criticalHit}",
                            $"{firstAttackerUser.Username}'s {firstAttacker.Name} has {firstAttacker.Health} health remaining.");
                        await reply.ModifyAsync(
                            new Action<MessageProperties>(props => { props.Embed = embed.Build(); }));

                        if (firstAttacker.Health <= 0)
                        {
                            var victorEmbed = new EmbedBuilder();
                            victorEmbed.WithColor(Discord.Color.Gold);
                            victorEmbed.WithTitle($"{secondAttackerUser.Username}'s {secondAttacker.Name} wins!");
                            victorEmbed.WithImageUrl(
                                "https://cdn.discordapp.com/attachments/803309924746395691/1376829532660568064/victory-pop-up-golden-assets-award-with-crown-for-game-illustration-golden-banner-with-wings-and-red-flags-vector.jpg?ex=6836bfec&is=68356e6c&hm=ffe35dc277c4b053ae396cf905581e05767837143ef2fffa6305203be11c08e7&");
                            victorEmbed.WithThumbnailUrl(secondAttackerUser.GetAvatarUrl());
                            await ReplyAsync(embed: victorEmbed.Build());

                            break;
                        }
                    }
                    else
                    {
                        embed.WithFields().AddField("Stunned!", $"{secondAttackerUser.Username}'s {secondAttacker.Name} is stunned and cannot attack this turn.");
                        await secondAttacker.RemoveStatusCondition(StatusCondition.Stun);
                    }

                    foreach (var statuscondition in await firstAttacker.GetStatusConditions())
                    {
                        switch (statuscondition)
                        {
                            case StatusCondition.Burn:
                                firstAttacker.Health -= 2;
                                embed.WithFields().AddField($"{firstAttackerUser.Username}'s {firstAttacker.Name} is burning! They take 2 fire damage.", $"{firstAttackerUser.Username}'s {firstAttacker.Name} has {firstAttacker.Health} health remaining.");
                                break;
                            case StatusCondition.None:
                                break;
                        }
                    }
                    foreach (var statuscondition in await secondAttacker.GetStatusConditions())
                    {
                        switch (statuscondition)
                        {
                            case StatusCondition.Burn:
                                secondAttacker.Health -= 2;
                                embed.WithFields().AddField($"{secondAttackerUser.Username}'s {secondAttacker.Name} is burning! They take 2 fire damage.", $"{secondAttackerUser.Username}'s {secondAttacker.Name} has {secondAttacker.Health} health remaining.");
                                break;
                            case StatusCondition.None:
                                break;
                        }
                    }
                    if (secondAttacker.Health <= 0)
                    {
                        var victorEmbed = new EmbedBuilder();
                        victorEmbed.WithColor(Discord.Color.Gold);
                        victorEmbed.WithTitle($"{firstAttackerUser.Username}'s {firstAttacker.Name} wins!");
                        victorEmbed.WithImageUrl(
                            "https://cdn.discordapp.com/attachments/803309924746395691/1376829532660568064/victory-pop-up-golden-assets-award-with-crown-for-game-illustration-golden-banner-with-wings-and-red-flags-vector.jpg?ex=6836bfec&is=68356e6c&hm=ffe35dc277c4b053ae396cf905581e05767837143ef2fffa6305203be11c08e7&");
                        victorEmbed.WithThumbnailUrl(firstAttackerUser.GetAvatarUrl());
                        await ReplyAsync(embed: victorEmbed.Build());
                        break;
                    }
                    if (firstAttacker.Health <= 0)
                    {
                        var victorEmbed = new EmbedBuilder();
                        victorEmbed.WithColor(Discord.Color.Gold);
                        victorEmbed.WithTitle($"{secondAttackerUser.Username}'s {secondAttacker.Name} wins!");
                        victorEmbed.WithImageUrl(
                            "https://cdn.discordapp.com/attachments/803309924746395691/1376829532660568064/victory-pop-up-golden-assets-award-with-crown-for-game-illustration-golden-banner-with-wings-and-red-flags-vector.jpg?ex=6836bfec&is=68356e6c&hm=ffe35dc277c4b053ae396cf905581e05767837143ef2fffa6305203be11c08e7&");
                        victorEmbed.WithThumbnailUrl(secondAttackerUser.GetAvatarUrl());
                        await ReplyAsync(embed: victorEmbed.Build());

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