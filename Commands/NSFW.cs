﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using PassiveBOT.Handlers;
using PassiveBOT.preconditions;
using PassiveBOT.strings;

namespace PassiveBOT.Commands
{
    [Ratelimit(1, 2, Measure.Seconds)]
    [CheckNsfw]
    public class Nsfw : ModuleBase
    {
        [Command("RedditNSFW", RunMode = RunMode.Async)]
        [Summary("RedditNSFW <sub>")]
        [Remarks("Get a random post from first 25 in hot of a sub")]
        public async Task RedditNSFW(string subreddit = null)
        {
            if (subreddit == null)
            {
                await ReplyAsync("Please give a subreddit to browse.");
            }
            if (subreddit == null)
            {
                await ReplyAsync("Please give a subreddit to browse.");
            }
            var rnd = new Random();
            var checkcache = CommandHandler.SubReddits.FirstOrDefault(x => String.Equals(x.title, subreddit, StringComparison.CurrentCultureIgnoreCase));
            if (checkcache != null && checkcache.LastUpdate > DateTime.UtcNow - TimeSpan.FromHours(6))
            {
                var imgx = checkcache.Posts[rnd.Next(checkcache.Posts.Count)];
                var objx = RedditHelper.isimage(imgx.Url.ToString());
                var embedx = new EmbedBuilder
                {
                    Title = imgx.Title,
                    Url = $"https://reddit.com{imgx.Permalink}",
                    Footer = new EmbedFooterBuilder
                    {
                        Text = objx.extension
                    }
                };
                await ReplyAsync(objx.url); await ReplyAsync("", false, embedx.Build());
            }
            else
            {
                var r = new RedditSharp.Reddit();
                var sub = r.GetSubreddit(subreddit);

                if (sub.NSFW)
                {
                    await ReplyAsync("Please use the NSFW Reddit command for NSFW Images");
                    return;
                }

                var num1 = sub.Hot.GetListing(150).Where(x => RedditHelper.isimage(x.Url.ToString()).isimage).ToList();
                var img = num1[rnd.Next(num1.Count)];
                var obj = RedditHelper.isimage(img.Url.ToString());
                var embed = new EmbedBuilder
                {
                    Title = img.Title,
                    Url = $"https://reddit.com{img.Permalink}",
                    Footer = new EmbedFooterBuilder
                    {
                        Text = obj.extension
                    }
                };
                await ReplyAsync(obj.url); await ReplyAsync("", false, embed.Build());

                CommandHandler.SubReddits.RemoveAll(x => string.Equals(x.title, subreddit, StringComparison.CurrentCultureIgnoreCase));
                CommandHandler.SubReddits.Add(new CommandHandler.SubReddit
                {
                    title = subreddit,
                    LastUpdate = DateTime.UtcNow,
                    Posts = num1
                });
            }
        }

        [Command("tits", RunMode = RunMode.Async)]
        [Summary("tits")]
        [Alias("boobs", "rack")]
        [Remarks("Fetches some sexy titties")]
        public async Task BoobsAsync()
        {
            JToken obj;
            var rnd = new Random().Next(0, 10229);
            using (var http = new HttpClient())
            {
                obj = JArray.Parse(await http.GetStringAsync($"http://api.oboobs.ru/boobs/{rnd}"))[0];
            }
            var builder = new EmbedBuilder
            {
                ImageUrl = $"http://media.oboobs.ru/{obj["preview"]}",
                Description = $"Tits Database Size: 10229\n Image Number: {rnd}",
                Title = "Tits",
                Url = $"http://adult.passivenation.com/18217229/http://media.oboobs.ru/{obj["preview"]}"
            };


            await ReplyAsync("", false, builder.Build());
        }

        [Command("Ass", RunMode = RunMode.Async)]
        [Summary("ass")]
        [Remarks("Sexy Ass!")]
        public async Task BumsAsync()
        {
            JToken obj;
            var rnd = new Random().Next(0, 4222);
            using (var http = new HttpClient())
            {
                obj = JArray.Parse(await http.GetStringAsync($"http://api.obutts.ru/butts/{rnd}"))[0];
            }
            var builder = new EmbedBuilder
            {
                ImageUrl = $"http://media.obutts.ru/{obj["preview"]}",
                Description = $"Ass Database Size: 4222\n Image Number: {rnd}",
                Title = "Ass",
                Url = $"http://adult.passivenation.com/18217229/http://media.obutts.ru/{obj["preview"]}"
            };
            await ReplyAsync("", false, builder.Build());
        }

        [Command("nsfw")]
        [Summary("nsfw")]
        [Alias("nude", "porn")]
        [Remarks("Sexy Stuff!")]
        public async Task Porn()
        {
            var str = NsfwStr.Nsfw;
            var rnd = new Random();
            var result = rnd.Next(0, str.Length);

            var builder = new EmbedBuilder()
                .WithTitle("NSFW")
                .WithUrl($"http://adult.passivenation.com/18217229/{str[result]}")
                .WithImageUrl(str[result])
                .WithFooter(x =>
                {
                    x.WithText($"PassiveBOT | {result}/{str.Length}");
                    x.WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
                });

            await ReplyAsync("", false, builder.Build());
        }

        [Command("sfw")]
        [Summary("sfw")]
        [Remarks("Porn meets MS Paint")]
        public async Task Sfw()
        {
            var str = NsfwStr.Sfw;
            var rnd = new Random();
            var result = rnd.Next(0, str.Length);

            var builder = new EmbedBuilder()
                .WithTitle("SFW")
                .WithUrl($"http://adult.passivenation.com/18217229/{str[result]}")
                .WithImageUrl(str[result])
                .WithFooter(x =>
                {
                    x.WithText($"PassiveBOT | {result}/{str.Length}");
                    x.WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
                });

            await ReplyAsync("", false, builder.Build());
        }

        [Command("nsfwvid")]
        [Summary("nsfwvid")]
        [Remarks("Pornhub + Bots = win?")]
        public async Task Nsfwvid()
        {
            var str = NsfwStr.Nsfwvid;
            var rnd = new Random();
            var result = rnd.Next(0, str.Length);

            var builder = new EmbedBuilder()
                .WithTitle("Click for Random Video")
                .WithUrl("http://adult.passivenation.com/18217229/{str[result]}")
                .WithFooter(x =>
                {
                    x.WithText($"PassiveBOT | {result}/{str.Length}");
                    x.WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
                });

            await ReplyAsync("", false, builder.Build());
        }

        [Command("pussy")]
        [Summary("pussy")]
        [Remarks(";)")]
        public async Task Pussy()
        {
            var str = NsfwStr.Pussy;
            var rnd = new Random();
            var result = rnd.Next(0, str.Length);

            var builder = new EmbedBuilder()
                .WithTitle("Pussy")
                .WithUrl($"http://adult.passivenation.com/18217229/{str[result]}")
                .WithFooter(x =>
                {
                    x.WithText($"PassiveBOT | {result}/{str.Length}");
                    x.WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
                })
                .WithImageUrl(str[result]);

            await ReplyAsync("", false, builder.Build());
            //await ReplyAsync(str[result]);
        }

        [Command("nsfwgif")]
        [Summary("nsfwgif")]
        [Remarks("Gifs")]
        public async Task Ngif()
        {
            var str = NsfwStr.Nsfwgif;
            var rnd = new Random();
            var result = rnd.Next(0, str.Length);

            var builder = new EmbedBuilder()
                .WithTitle("NSFW GIF")
                .WithUrl($"http://adult.passivenation.com/18217229/{str[result]}")
                .WithFooter(x =>
                {
                    x.WithText($"PassiveBOT | {result}/{str.Length}");
                    x.WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
                });
               // .WithImageUrl(str[result]);

            await ReplyAsync("", false, builder.Build());

            await ReplyAsync(str[result]);
        }

        [Command("Rule34", RunMode = RunMode.Async)]
        [Alias("R34")]
        [Summary("Rule34 <tags>")]
        [Remarks("Search Rule34 Porn using tags")]
        public async Task R34(params string[] Tags)
        {
            var result = await HentaiAsync(new HttpClient(), new Random(), NsfwType.Rule34, Tags.ToList());
            if (result == null)
            {
                await ReplyAsync("No Results.");
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    ImageUrl = result,
                    Title = "View On Site [R34]",
                    Url = $"http://adult.passivenation.com/18217229/{result}",
                    Footer = new EmbedFooterBuilder { Text = string.Join(", ", Tags)}
                };
                await ReplyAsync("", false, embed.Build());
            }
        }

        [Command("Yandere", RunMode = RunMode.Async)]
        [Summary("Yandere <tags>")]
        [Remarks("Search Yandere Porn using tags")]
        public async Task Yandere(params string[] Tags)
        {
            var result = await HentaiAsync(new HttpClient(), new Random(), NsfwType.Yandere, Tags.ToList());
            if (result == null)
            {
                await ReplyAsync("No Results.");
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    ImageUrl = result,
                    Title = "View On Site [Yandere]",
                    Url = $"http://adult.passivenation.com/18217229/{result}",
                    Footer = new EmbedFooterBuilder { Text = string.Join(", ", Tags) }
                };
                await ReplyAsync("", false, embed.Build());
            }
        }

        [Command("Gelbooru", RunMode = RunMode.Async)]
        [Summary("Gelbooru <tags>")]
        [Remarks("Search Gelbooru Porn using tags")]
        public async Task Gelbooru(params string[] Tags)
        {
            var result = await HentaiAsync(new HttpClient(), new Random(), NsfwType.Gelbooru, Tags.ToList());
            if (result == null)
            {
                await ReplyAsync("No Results.");
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    ImageUrl = result,
                    Title = "View On Site [Gelbooru]",
                    Url = $"http://adult.passivenation.com/18217229/{result}",
                    Footer = new EmbedFooterBuilder { Text = string.Join(", ", Tags) }
                };
                await ReplyAsync("", false, embed.Build());
            }
        }

        /*
        [Command("Danbooru", RunMode = RunMode.Async)]
        [Summary("Danbooru <tags>")]
        [Remarks("Search Danbooru Porn using tags")]
        public async Task Danbooru(params string[] Tags)
        {
            var result = await HentaiAsync(new HttpClient(), new Random(), NsfwType.Danbooru, Tags.ToList());
            if (result == null)
            {
                await ReplyAsync("No Results.");
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    ImageUrl = result,
                    Title = "View On Site [Danbooru]",
                    Url = $"http://adult.passivenation.com/18217229/{result}",
                    Footer = new EmbedFooterBuilder { Text = string.Join(", ", Tags) }
                };
                await ReplyAsync("", false, embed.Build());
            }
        }
        */

        [Command("Cureninja", RunMode = RunMode.Async)]
        [Summary("Cureninja <tags>")]
        [Remarks("Search Cureninja Porn using tags")]
        public async Task Cureninja(params string[] Tags)
        {
            var result = await HentaiAsync(new HttpClient(), new Random(), NsfwType.Cureninja, Tags.ToList());
            if (result == null)
            {
                await ReplyAsync("No Results.");
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    ImageUrl = result,
                    Title = "View On Site [Cureninja]",
                    Url = $"http://adult.passivenation.com/18217229/{result}",
                    Footer = new EmbedFooterBuilder { Text = string.Join(", ", Tags) }
                };
                await ReplyAsync("", false, embed.Build());
            }
        }

        [Command("Konachan", RunMode = RunMode.Async)]
        [Summary("Konachan <tags>")]
        [Remarks("Search Konachan Porn using tags")]
        public async Task Konachan(params string[] Tags)
        {
            var result = await HentaiAsync(new HttpClient(), new Random(), NsfwType.Konachan, Tags.ToList());
            if (result == null)
            {
                await ReplyAsync("No Results.");
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    ImageUrl = result,
                    Title = "View On Site [Konachan]",
                    Url = $"http://adult.passivenation.com/18217229/{result}",
                    Footer = new EmbedFooterBuilder { Text = string.Join(", ", Tags) }
                };
                await ReplyAsync("", false, embed.Build());
            }
        }

        public enum NsfwType
        {
            Rule34,
            Yandere,
            Gelbooru,
            Konachan,
            Danbooru,
            Cureninja,
        }

        public static async Task<string> HentaiAsync(HttpClient HttpClient, Random Random, NsfwType NsfwType, List<string> Tags)
        {
            string Url = null;
            string Result = null;
            MatchCollection Matches = null;
            Tags = !Tags.Any() ? new[] { "boobs", "tits", "ass", "sexy", "neko" }.ToList() : Tags;
            switch (NsfwType)
            {
                case NsfwType.Danbooru: Url = $"http://danbooru.donmai.us/posts?page={Random.Next(0, 15)}{string.Join("+", Tags.Select(x => x.Replace(" ", "_")))}"; break;
                case NsfwType.Gelbooru: Url = $"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags={string.Join("+", Tags.Select(x => x.Replace(" ", "_")))}"; break;
                case NsfwType.Rule34: Url = $"http://rule34.xxx/index.php?page=dapi&s=post&q=index&limit=100&tags={string.Join("+", Tags.Select(x => x.Replace(" ", "_")))}"; break;
                case NsfwType.Cureninja: Url = $"https://cure.ninja/booru/api/json?f=a&o=r&s=1&q={string.Join("+", Tags.Select(x => x.Replace(" ", "_")))}"; break;
                case NsfwType.Konachan: Url = $"http://konachan.com/post?page={Random.Next(0, 5)}&tags={string.Join("+", Tags.Select(x => x.Replace(" ", "_")))}"; break;
                case NsfwType.Yandere: Url = $"https://yande.re/post.xml?limit=25&page={Random.Next(0, 15)}&tags={string.Join("+", Tags.Select(x => x.Replace(" ", "_")))}"; break;
            }
            var Get = await HttpClient.GetStringAsync(Url).ConfigureAwait(false);
            switch (NsfwType)
            {
                case NsfwType.Danbooru: Matches = Regex.Matches(Get, "data-large-file-url=\"(.*)\""); break;
                case NsfwType.Yandere:
                case NsfwType.Gelbooru:
                case NsfwType.Rule34: Matches = Regex.Matches(Get, "file_url=\"(.*?)\" "); break;
                case NsfwType.Cureninja: Matches = Regex.Matches(Get, "\"url\":\"(.*?)\""); break;
                case NsfwType.Konachan: Matches = Regex.Matches(Get, "<a class=\"directlink smallimg\" href=\"(.*?)\""); break;
            }
            if (!Matches.Any()) return null;
            switch (NsfwType)
            {
                case NsfwType.Danbooru: Result = $"http://danbooru.donmai.us/{Matches[Random.Next(Matches.Count)].Groups[1].Value}"; break;
                case NsfwType.Konachan:
                case NsfwType.Gelbooru:
                case NsfwType.Yandere:
                case NsfwType.Rule34: Result = $"{Matches[Random.Next(Matches.Count)].Groups[1].Value}"; break;
                case NsfwType.Cureninja: Result = Matches[Random.Next(Matches.Count)].Groups[1].Value.Replace("\\/", "/"); break;
                default:
                    return null;
            }
            Result = Result.EndsWith("/") ? Result.Substring(0, Result.Length - 1) : Result;
            return Result;
        }
    }
}