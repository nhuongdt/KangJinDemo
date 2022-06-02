using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Open24.Hellper
{
    public class TelegramBot
    {
        private string Token = "934999753:AAE_fur8TYGkwj2KHoqvn0Eqn0dLIFywTkY";
        private ChatId ChatId = -526426581;
        public async void SendMessage(string message)
        {
            var botClient = new TelegramBotClient(Token);
            await botClient.SendTextMessageAsync(
              chatId: ChatId, // or a chat id: 123456789
              text: message
            );
        }

        public async void SendMessage(string message, ChatId _chatId)
        {
            var botClient = new TelegramBotClient(Token);
            await botClient.SendTextMessageAsync(
              chatId: _chatId, // or a chat id: 123456789
              text: message
            );
        }
    }
}