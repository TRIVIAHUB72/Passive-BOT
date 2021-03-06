﻿namespace PassiveBOT.Context
{
    using System;
    using System.Threading.Tasks;

    using Discord;
    using Discord.Addons.Interactive;
    using Discord.Commands;
    using Discord.WebSocket;

    /// <summary>
    ///     The module base.
    /// </summary>
    public abstract class Base : ModuleBase<Context>
    {
        /// <summary>
        ///     Gets or sets Our Custom Interactive Base
        /// </summary>
        public Interactive Interactive { get; set; }

        /// <summary>
        ///     Sends a Message that will do a custom action upon reactions
        /// </summary>
        /// <param name="data">The main settings used for the Message</param>
        /// <param name="fromSourceUser">True = Only the user who invoked this method can invoke the callback</param>
        /// <returns>The Message sent</returns>
        public Task<IUserMessage> InlineReactionReplyAsync(ReactionCallbackData data, bool fromSourceUser = true)
        {
            return Interactive.SendMessageWithReactionCallbacksAsync(SocketContext(), data, fromSourceUser);
        }

        /// <summary>
        ///     Waits for the next Message. NOTE: Your run-mode must be async or this will lock up.
        /// </summary>
        /// <param name="criterion">The criterion for the Message</param>
        /// <param name="timeout">Time to wait before exiting</param>
        /// <returns>The Message received</returns>
        public Task<SocketMessage> NextMessageAsync(ICriterion<SocketMessage> criterion, TimeSpan? timeout = null)
        {
            return Interactive.NextMessageAsync(SocketContext(), criterion, timeout);
        }

        /// <summary>
        ///     Waits until a new Message is sent in the channel.
        ///     RunMode MUST be set to async to use this
        /// </summary>
        /// <param name="fromSourceUser">Command invoker only</param>
        /// <param name="inSourceChannel">Context.Channel only</param>
        /// <param name="timeout">Time before exiting</param>
        /// <returns>The Message received</returns>
        public Task<SocketMessage> NextMessageAsync(bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null)
        {
            return Interactive.NextMessageAsync(SocketContext(), fromSourceUser, inSourceChannel, timeout);
        }

        /// <summary>
        ///     This will generate a paginated Message which allows users to use reactions to change the content of the Message
        /// </summary>
        /// <param name="pager">Our paginated Message</param>
        /// <param name="reactionList">The reaction config</param>
        /// <param name="fromSourceUser">True = only Context.User may react to the Message</param>
        /// <returns>The Message that was sent</returns>
        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ReactionList reactionList, bool fromSourceUser = true)
        {
            var criterion = new Criteria<SocketReaction>();
            if (fromSourceUser)
            {
                criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            }

            return PagedReplyAsync(pager, criterion, reactionList);
        }

        /// <summary>
        ///     Sends a paginated Message
        /// </summary>
        /// <param name="pager">The paginated Message</param>
        /// <param name="criterion">The criterion for the reply</param>
        /// <param name="reactions">Customized reaction list</param>
        /// <returns>The Message sent.</returns>
        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ICriterion<SocketReaction> criterion, ReactionList reactions)
        {
            return Interactive.SendPaginatedMessageAsync(SocketContext(), pager, reactions, criterion);
        }

        /// <summary>
        ///     Reply in the server and then delete after the provided delay.
        /// </summary>
        /// <param name="message">
        ///     The Message.
        /// </param>
        /// <param name="timeout">
        ///     The timeout.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<IUserMessage> ReplyAndDeleteAsync(string message, TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(5);
            var msg = await Context.Channel.SendMessageAsync(message).ConfigureAwait(false);
            _ = Task.Delay(timeout.Value).ContinueWith(_ => msg.DeleteAsync().ConfigureAwait(false)).ConfigureAwait(false);
            return msg;
        }

        /// <summary>
        ///     Send a Message that self destructs after a certain period of time
        /// </summary>
        /// <param name="content">The text of the Message being sent</param>
        /// <param name="embed">A build embed to be sent</param>
        /// <param name="timeout">The time it takes before the Message is deleted</param>
        /// <param name="options">Request Options</param>
        /// <returns>The Message that was sent</returns>
        public Task<IUserMessage> ReplyAndDeleteAsync(string content, Embed embed = null, TimeSpan? timeout = null, RequestOptions options = null)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return Interactive.ReplyAndDeleteAsync(SocketContext(), content, false, embed, timeout, options);
        }

        /// <summary>
        ///     Reply in the server. Shorthand for Context.Channel.SendMessageAsync()
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="embed">
        ///     The embed.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<IUserMessage> ReplyAsync(string message, Embed embed = null)
        {
            await Context.Channel.TriggerTypingAsync();
            return await ReplyAsync(message, false, embed);
        }

        /// <summary>
        ///     Shorthand for  replying with just an embed
        /// </summary>
        /// <param name="embed">
        ///     The embed.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<IUserMessage> ReplyAsync(EmbedBuilder embed)
        {
            return ReplyAsync(string.Empty, false, embed.Build());
        }

        /// <summary>
        ///     Shorthand for  replying with just an embed
        /// </summary>
        /// <param name="embed">
        ///     The embed.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<IUserMessage> ReplyAsync(Embed embed)
        {
            return ReplyAsync(string.Empty, false, embed);
        }

        /// <summary>
        ///     Rather than just replying, we can spice things up a bit and embed them in a small message
        /// </summary>
        /// <param name="message">
        ///     The text that will be contained in the embed
        /// </param>
        /// <param name="color">
        ///     The color.
        /// </param>
        /// <returns>
        ///     The message that was sent
        /// </returns>
        public Task<IUserMessage> SimpleEmbedAsync(string message, Color? color = null)
        {
            var embed = new EmbedBuilder { Description = message, Color = color ?? Color.DarkRed };
            return ReplyAsync(string.Empty, false, embed.Build());
        }

        /// <summary>
        ///     This is just a shorthand conversion from out custom context to a socket context, for use in things like Interactive
        /// </summary>
        /// <returns>A new SocketCommandContext</returns>
        private SocketCommandContext SocketContext()
        {
            return new SocketCommandContext(Context.Client.GetShardFor(Context.Guild), Context.Message);
        }
    }
}