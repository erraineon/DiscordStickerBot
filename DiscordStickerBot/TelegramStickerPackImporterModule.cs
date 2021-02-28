using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using ImageProcessorCore.Plugins.WebP.Formats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Telegram.Bot;
using Telegram.Bot.Types;
using Image = SixLabors.ImageSharp.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

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
        public async Task ImportAsync(string stickerPackNameOrUrl, int? maxSize = null)
        {
            var stickerPackName = GetStickerPackName(stickerPackNameOrUrl);
            try
            {
                var stickerPack = await _telegramBotClient.GetStickerSetAsync(stickerPackName);
                foreach (var sticker in stickerPack.Stickers.Where(s => !s.IsAnimated))
                {
                    var gifStream = await GetStickerGifStreamAsync(sticker);
                    if (maxSize != null) gifStream = await ResizeImageAsync(gifStream, maxSize.Value);
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

        private async Task<MemoryStream> ResizeImageAsync(MemoryStream stream, int maxSize)
        {
            using var canvas = await Image.LoadAsync(stream);
            var largerDimension = Math.Max(canvas.Width, canvas.Height);
            var sizeRatio = (float) largerDimension / maxSize;
            var result = sizeRatio > 1 ? await ResizeImageAsync(canvas, sizeRatio) : stream;
            return result;
        }

        private static async Task<MemoryStream> ResizeImageAsync(Image canvas, float sizeRatio)
        {
            var resizedStream = new MemoryStream();
            canvas.Mutate(c => c.Resize((Size) (c.GetCurrentSize() / sizeRatio)));
            await canvas.SaveAsPngAsync(resizedStream);
            resizedStream.Seek(0, SeekOrigin.Begin);
            return resizedStream;
        }

        private static string GetStickerPackName(string stickerPackNameOrUrl)
        {
            const string packUrlPrefix = "https://t.me/addstickers/";
            var stickerPackName = stickerPackNameOrUrl.StartsWith(packUrlPrefix)
                ? stickerPackNameOrUrl[packUrlPrefix.Length..]
                : stickerPackNameOrUrl;
            return stickerPackName;
        }

        private async Task<MemoryStream> GetStickerGifStreamAsync(Sticker sticker)
        {
            var stickerStream = new MemoryStream();
            await _telegramBotClient.GetInfoAndDownloadFileAsync(sticker.FileId, stickerStream);
            stickerStream.Seek(0, SeekOrigin.Begin);
            var gifStream = new MemoryStream();
            new WebPFormat().Load(stickerStream).Save(gifStream, ImageFormat.Png);
            gifStream.Seek(0, SeekOrigin.Begin);
            return gifStream;
        }
    }
}