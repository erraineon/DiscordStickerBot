using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DiscordStickerBot
{
    public class TelegramStickerPackImporterModule : ModuleBase<ICommandContext>
    {
        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramStickerPackImporterModule(ITelegramBotClient telegramBotClient)
        {
            _telegramBotClient = telegramBotClient;
        }

        [Command("import")]
        public async Task ImportAsync([Remainder] string stickerPackNameOrUrl)
        {
            var stickerPackName = GetStickerPackName(stickerPackNameOrUrl);
            try
            {
                var stickerPack = await _telegramBotClient.GetStickerSetAsync(stickerPackName);
                foreach (var sticker in stickerPack.Stickers.Where(s => !s.IsAnimated))
                {
                    var gifStream = await GetStickerGifStream(sticker);
                    await Context.Channel.SendFileAsync(gifStream, $"{sticker.FileId}.gif");
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("STICKERSET_INVALID"))
                    await ReplyAsync($"a sticker pack named {stickerPackName} could not be found");
                else
                    await ReplyAsync(e.Message);
            }
        }

        private static string GetStickerPackName(string stickerPackNameOrUrl)
        {
            const string packUrlPrefix = "https://t.me/addstickers/";
            var stickerPackName = stickerPackNameOrUrl.StartsWith(packUrlPrefix)
                ? stickerPackNameOrUrl[packUrlPrefix.Length..]
                : stickerPackNameOrUrl;
            return stickerPackName;
        }

        private async Task<MemoryStream> GetStickerGifStream(Sticker sticker)
        {
            var stickerStream = new MemoryStream();
            await _telegramBotClient.GetInfoAndDownloadFileAsync(sticker.FileId, stickerStream);
            stickerStream.Seek(0, SeekOrigin.Begin);
            return stickerStream;
        }
    }
}