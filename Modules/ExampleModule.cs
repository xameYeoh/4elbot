using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _4elBot.Modules
{
    [Name("Example")]
    public class ExampleModule : ModuleBase<SocketCommandContext>
    {
        [Command("say"), Alias("s")]
        [Summary("Make the bot say something")]
        
        public Task Say([Remainder]string text)
            => ReplyAsync(text);

        [Command("roll"), Alias("r")]
        [Summary("Rolls a number from 0 to 100")]

        public async Task Roll()
        {
            Random r = new Random();
            int rInt = r.Next(0, 100);

            await ReplyAsync($"{rInt}");
        }

        [Command("userinfo")]
        [Summary
    ("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfoAsync(
        [Summary("The (optional) user to get info from")]
        SocketUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;

            await ReplyAsync($"Username: {userInfo.Username}#{userInfo.Discriminator}" + System.Environment.NewLine + 
                $"Status: {userInfo.Status}" + System.Environment.NewLine +
                $"Role: {(userInfo as SocketGuildUser).Roles.Last()}");
        }

        [Command("whoisgay")]
        [Summary
    ("Returns info about who is gay")]
        public async Task WhoIsGay(
        [Remainder]string text)
        {
            string[] players = text.Split(' ');
            Random r = new Random();
            int rInt = r.Next(0, players.Length);
            
            await ReplyAsync($"{players[rInt]} is gay! Gratz!");
        }

        [Command("dota")]
        [Summary
    ("Returns info about who is gay")]
        public async Task Dota()        {
            Random r = new Random();
            int rInt = r.Next(0, 100);

            if(rInt<90) await ReplyAsync("No Dota today");
            else await ReplyAsync("Буля...");
        }



        public class RequireRoleAttribute : PreconditionAttribute
        {
            // Create a field to store the specified name
            private readonly string _name;

            // Create a constructor so the name can be specified
            public RequireRoleAttribute(string name) => _name = name;

            // Override the CheckPermissions method
            public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            {
                // Check if this user is a Guild User, which is the only context where roles exist
                if (context.User is SocketGuildUser gUser)
                {
                    // If this command was executed by a user with the appropriate role, return a success
                    if (gUser.Roles.Any(r => r.Name == _name))
                        // Since no async work is done, the result has to be wrapped with `Task.FromResult` to avoid compiler errors
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    // Since it wasn't, fail
                    else
                        return Task.FromResult(PreconditionResult.FromError($"You must have a role named {_name} to run this command."));
                }
                else
                    return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command."));
            }
        }




        [Group("set"), Name("Example")]
        [RequireContext(ContextType.Guild)]
        public class Set : ModuleBase
        {
            [Command("nick"), Priority(1)]
            [Summary("Change your nickname to the specified text")]
            public Task Nick([Remainder]string name)
                => Nick(Context.User as SocketGuildUser, name);

            [Command("nick"), Priority(0)]
            [Summary("Change another user's nickname to the specified text")]
            public async Task Nick(SocketGuildUser user, [Remainder]string name)
            {
                await user.ModifyAsync(x => x.Nickname = name);
                await ReplyAsync($"{user.Mention} I changed your name to **{name}**");
            }
        }
    }
}
