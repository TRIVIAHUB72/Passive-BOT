﻿namespace PassiveBOT.Services
{
    using System.Collections.Generic;

    using Raven.Client.Documents;

    public class TagService
    {
        private static IDocumentStore Store { get; set; }

        public TagService(IDocumentStore store)
        {
            Store = store;
        }

        public TagSetup GetTagSetup(ulong guildId)
        {
            using (var session = Store.OpenSession())
            {
                var tagSetup = session.Load<TagSetup>($"{guildId}-Tags") ?? new TagSetup(guildId);
                session.Dispose();
                return tagSetup;
            }
        }

        /// <summary>
        /// The tag setup.
        /// </summary>
        public class TagSetup
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TagSetup"/> class.
            /// </summary>
            /// <param name="guildId">
            /// The guild id.
            /// </param>
            public TagSetup(ulong guildId)
            {
                GuildId = guildId;
            }

            public void Save()
            {
                using (var session = Store.OpenSession())
                {
                    session.Store(this, $"{GuildId}-Tags");
                    session.SaveChanges();
                }
            }

            /// <summary>
            /// Gets the guild id.
            /// </summary>
            public ulong GuildId { get; }

            /// <summary>
            /// Gets or sets a value indicating whether tags are enabled.
            /// </summary>
            public bool Enabled { get; set; } = true;

            /// <summary>
            /// Gets or sets the tags.
            /// </summary>
            public Dictionary<string, Tag> Tags { get; set; } = new Dictionary<string, Tag>();

            /// <summary>
            /// The tag.
            /// </summary>
            public class Tag
            {
                /// <summary>
                /// Gets or sets the name.
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the content.
                /// </summary>
                public string Content { get; set; }

                /// <summary>
                /// Gets or sets the use count
                /// </summary>
                public int Uses { get; set; } = 0;

                /// <summary>
                /// Gets or sets the creator id.
                /// </summary>
                public ulong CreatorId { get; set; }

                /// <summary>
                /// Gets or sets the creator name.
                /// </summary>
                public string Creator { get; set; }
            }
        }
    }
}
