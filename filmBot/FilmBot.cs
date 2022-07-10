using IMDbApiLib;
using IMDbApiLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace filmBot
{
    class FilmBot
    {
        protected TelegramBotClient Client;
        private const int MAX_NUMBER = 2;
        private String[] MENU_BUTTONS = {"Top Movies", "Top TVs", "Search by category"};
        public FilmBot()
        {
            Client = new TelegramBotClient("5330215476:AAFMZFKVdFjDP-6GP6GjaKevvDBIJGUAbAk");
            Client.OnMessage += Response;
        }
        public void Start()
        {
            Client.StartReceiving();
        }
        public void Stop()
        {
            Client.StopReceiving();
        }
        private async void Response(object sender, MessageEventArgs e)
        {
            MenuSelector(e);
        }

        private async Task MenuSelector(MessageEventArgs e)
        {
            switch (e.Message.Text)
            {
                case "/start":
                    await Client.SendTextMessageAsync(e.Message.Chat.Id, "Let`s start!",
                        replyMarkup: GenerateKeyboard(columns: 1, MENU_BUTTONS));
                    break;
                case "Menu":
                    await Client.SendTextMessageAsync(e.Message.Chat.Id, "Go on",
                        replyMarkup: GenerateKeyboard(columns: 1, MENU_BUTTONS));
                    break;
                case "Top Movies":
                    await TopMoviesAsync(e);
                    break;
                case "Top TVs":
                    await TopTVsAsync(e);
                    break;
                case "Search by category":
                    await CategoryMenuButtons(e);
                    break;
                default:
                    SearchByCategoryAsync(e);
                    break;
            }
        }

        private async Task SearchByCategoryAsync(MessageEventArgs e)
        {
            if (Enum.TryParse(e.Message.Text, out AdvancedSearchGenre category))
            {
                try
                {
                    var apiLib = new ApiLib("k_d5cm1k46");
                    var input = new AdvancedSearchInput();
                    input.Genres = category;
                    var data = await apiLib.AdvancedSearchAsync(input);
                    int i = 0;
                    foreach (var item in data.Results)
                    {
                        try
                        {
                            await Client.SendPhotoAsync(e.Message.Chat.Id, $"{item.Image}",
                                caption: $"{item.Title} \nRating: {item.IMDbRating} \nDescription: {item.Description}",
                                disableNotification: true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex.Message}");
                        }
                        i++;
                        if (i == MAX_NUMBER)
                        {
                            break;
                        }
                    }
                    if (i == 0)
                    {
                        await Client.SendTextMessageAsync(e.Message.Chat.Id, "Sorry, nothing found.", disableNotification: true);
                    }
                    await Client.SendTextMessageAsync(e.Message.Chat.Id, $"⬆️{e.Message.Text}⬆️",
                        replyMarkup: GenerateKeyboard(columns: 1, "Menu"));

                }
                catch
                {
                    await Client.SendTextMessageAsync(e.Message.Chat.Id, "Sorry, the number of requests for today has been exceeded",
                        replyMarkup: GenerateKeyboard(columns: 1, "Menu"));
                }
            }
            else
            {
                await Client.SendTextMessageAsync(e.Message.Chat.Id, $"Unknown command",
                        replyMarkup: GenerateKeyboard(columns: 1, "Menu"));
            }
                
        }


        private async Task CategoryMenuButtons(MessageEventArgs e)
        {
            await Client.SendTextMessageAsync(e.Message.Chat.Id, $"Choose category",
                replyMarkup: GenerateKeyboard(columns:3, Enum.GetNames(typeof(AdvancedSearchGenre))));
            
        }


        private async Task TopTVsAsync(MessageEventArgs e)
        {
            try
            {
                var apiLib = new ApiLib("k_d5cm1k46");
                var data = await apiLib.Top250TVsAsync();
                for (int i = 0; i < MAX_NUMBER; i++)
                {
                        await Client.SendPhotoAsync(e.Message.Chat.Id, $"{data.Items[i].Image}",
                        caption: $"{data.Items[i].FullTitle} \nRating: {data.Items[i].IMDbRating} \nCrew: {data.Items[i].Crew}",
                        disableNotification: true);
                }
                await Client.SendTextMessageAsync(e.Message.Chat.Id, $"⬆️Top {MAX_NUMBER} TVs by Imdb rating⬆️",
                        replyMarkup: GenerateKeyboard(columns: 1, "Menu"));
            }
            catch
            {
                await Client.SendTextMessageAsync(e.Message.Chat.Id, "Sorry, the number of requests for today has been exceeded",
                         replyMarkup: GenerateKeyboard(columns: 1, "Menu"));
            }
        }


        private async Task TopMoviesAsync(MessageEventArgs e)
        {
            try
            {
                var apiLib = new ApiLib("k_d5cm1k46");
                var data = await apiLib.Top250MoviesAsync();
                for (int i = 0; i < MAX_NUMBER; i++)
                {
                        await Client.SendPhotoAsync(e.Message.Chat.Id, $"{data.Items[i].Image}",
                        caption: $"{data.Items[i].FullTitle} \nRaiting: {data.Items[i].IMDbRating} \nCrew: {data.Items[i].Crew}",
                        disableNotification: true);
                }
                await Client.SendTextMessageAsync(e.Message.Chat.Id, $"⬆️Top {MAX_NUMBER} Movies by Imdb rating⬆️",
                        replyMarkup: GenerateKeyboard(columns: 1, "Menu"));
            }
            catch
            {
                await Client.SendTextMessageAsync(e.Message.Chat.Id, "Sorry, the number of requests for today has been exceeded",
                        replyMarkup: GenerateKeyboard(columns: 1, "Menu"));
            }
        }


        private static ReplyKeyboardMarkup GenerateKeyboard(int columns, params String[] keys)
        {
            var rkm = new ReplyKeyboardMarkup();
            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();
            int i = 0;
            foreach (var key in keys)
            {
                cols.Add(new KeyboardButton(key));
                i++;
                if(i == columns)
                {
                    rows.Add(cols.ToArray());
                    cols = new List<KeyboardButton>();
                    i = 0;
                }
            }
            rows.Add(cols.ToArray());
            rkm.Keyboard = rows.ToArray();
            return rkm;
        }
    }
}

