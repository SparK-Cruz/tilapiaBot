﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Telegram.Bot;

namespace Tilápia
{
    internal static class Program
    {
        static string trend;
        static long lastTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        static TelegramBotClient botClient = new TelegramBotClient(System.IO.File.ReadAllText(@"telegramTokenAPI"));
        static dynamic anubisTrendAPIdata;
        static dynamic coinList;

        static void Main(string[] args)
        {
            Console.WriteLine("Tilápia Bot Iniciado (UTC)\n");
            coinList = JsonConvert.DeserializeObject(new WebClient().DownloadString("https://api.coinpaprika.com/v1/coins/"));
            botClient.OnMessage += botClient_OnMessage;
            botClient.StartReceiving();

            while (true)
            {
                Thread.Sleep(300000);
                coinList = JsonConvert.DeserializeObject(new WebClient().DownloadString("https://api.coinpaprika.com/v1/coins/"));
            }
        }

        static void botClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (e.Message.Text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n" + e.Message.Text);
                    if (e.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                    {
                        telegramEnviarMensagem(e.Message.Chat.Id, @"Necessário colocar uma mensagem aqui", true);
                    }
                    else
                    {
                        telegramEnviarMensagem(e.Message.Chat.Id, @"Necessário colocar aqui uma mensagem ainda", true);
                    }
                }

                if (e.Message.Text.StartsWith("/valor", StringComparison.OrdinalIgnoreCase) | e.Message.Text.StartsWith("/bitcoin", StringComparison.OrdinalIgnoreCase) | e.Message.Text.StartsWith("/btc", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n" + e.Message.Text);
                    WebClient getBitcoinPrice = new WebClient();
                    StringBuilder mensagemPrice = new StringBuilder();
                    DateTimeOffset agoraUTC = DateTime.UtcNow;
                    dynamic ticker = JsonConvert.DeserializeObject(getBitcoinPrice.DownloadString("https://blockchain.info/ticker"));
                    mensagemPrice.AppendLine("Hoje, " + agoraUTC.Date.ToShortDateString() + ", " + agoraUTC.Hour.ToString().PadLeft(2, '0') + ':' + agoraUTC.Minute.ToString().PadLeft(2, '0') + " (UTC)");
                    mensagemPrice.AppendLine("*Um bitcoin* vale *R$ " + ticker.BRL.last + '*');
                    mensagemPrice.AppendLine("*Um bitcoin* vale *U$ " + ticker.USD.last + '*');
                    mensagemPrice.AppendLine("*1 real* vale só *BTC " + Math.Round(1 / Convert.ToDouble(ticker.BRL.last), 8).ToString("0" + '.' + "###############") + '*');

                    telegramEnviarMensagem(e.Message.Chat.Id, mensagemPrice.ToString(), true);
                }

                if (e.Message.Text.StartsWith("/bitatlas", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n" + e.Message.Text);
                    WebClient getBitcoinPrice = new WebClient();
                    StringBuilder mensagemPrice = new StringBuilder();
                    DateTimeOffset agoraUTC = DateTime.UtcNow;
                    dynamic ticker = JsonConvert.DeserializeObject(getBitcoinPrice.DownloadString("https://blockchain.info/ticker"));
                    string bitAtlasValor = getBitcoinPrice.DownloadString("https://bitatlas.cf/price");
                    mensagemPrice.AppendLine("Hoje, " + agoraUTC.Date.ToShortDateString() + ", " + agoraUTC.Hour.ToString().PadLeft(2, '0') + ':' + agoraUTC.Minute.ToString().PadLeft(2, '0') + " (UTC)");
                    mensagemPrice.AppendLine("*Um bitcoin* vale `R$ " + ticker.BRL.last.ToString().Replace(".", ",") + '`');
                    mensagemPrice.AppendLine("*Um bitAtlas* vale `R$ " + bitAtlasValor.ToString().Replace(".", ",") + '`');
                    mensagemPrice.AppendLine();
                    mensagemPrice.AppendLine("_Atlas Quantum: rendimento que não tem fim_");

                    telegramEnviarMensagem(e.Message.Chat.Id, mensagemPrice.ToString(), true);
                    botClient.SendStickerAsync(e.Message.Chat.Id, "CAADAgAD1QQAAs7Y6AuD1Fx6th6oRBYE");
                }

                if (e.Message.Text.StartsWith("/medoeganancia", StringComparison.OrdinalIgnoreCase) || (e.Message.Text.StartsWith("/fg", StringComparison.OrdinalIgnoreCase)) || (e.Message.Text.StartsWith("/sentimento", StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("\n" + e.Message.Text);
                    telegramEnviarMensagem(e.Message.Chat.Id, "Medo e ganância do cryptomercado[⠀](https://alternative.me/crypto/fear-and-greed-index.png)", false);
                }

                if (e.Message.Text.StartsWith("/global", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n" + e.Message.Text);
                    StringBuilder mensagem = new StringBuilder();
                    DateTimeOffset agoraUTC = DateTime.UtcNow;
                    dynamic ticker = JsonConvert.DeserializeObject(new WebClient().DownloadString("https://api.coinpaprika.com/v1/global"));

                    mensagem.AppendLine("*Dados Globais Criptomercado*").AppendLine();
                    mensagem.AppendLine("`" + agoraUTC.Date.ToShortDateString() + ", " + agoraUTC.Hour.ToString("##").PadLeft(2, '0') + ':' + agoraUTC.Minute.ToString("##").PadLeft(2, '0') + " (UTC)`");
                    mensagem.AppendLine("*Marketcap:* U$ `" + ticker.market_cap_usd + ".00`");
                    mensagem.AppendLine("*Volume 1D:* U$ `" + ticker.volume_24h_usd + ".00`");
                    mensagem.AppendLine("*Dominance:* " + ticker.bitcoin_dominance_percentage + " %");
                    mensagem.AppendLine("*Moedas Catalogadas:* " + ticker.cryptocurrencies_number);
                    mensagem.AppendLine("*Mudança Marketcap 24h:* " + ticker.market_cap_change_24h + '%');
                    mensagem.AppendLine("*Mudança Volume em 24h:* " + ticker.volume_24h_change_24h + '%');


                    telegramEnviarMensagem(e.Message.Chat.Id, mensagem.ToString(), true);
                }

                if (e.Message.Text.StartsWith("/info", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n" + e.Message.Text);
                    StringBuilder mensagem = new StringBuilder();
                    DateTimeOffset agoraUTC = DateTime.UtcNow;

                    string[] comando = e.Message.Text.Split(' ', 2);

                    if (comando.Length < 2)
                    {
                        mensagem.Append("digite o código da moeda após o comando. Ex: `/info BTC`");
                    }
                    else
                    {
                        string coinID = GetCoinID(comando[1], coinList);

                        if (coinID == null)
                        {
                            telegramEnviarMensagem(e.Message.Chat.Id, "Moeda não encontrada. Certifique-se que digitou corretamento. Talvéz a moeda que você digitou não esteja listada em nosso indexador", true);
                            return;
                        }

                        dynamic info = JsonConvert.DeserializeObject(new WebClient().DownloadString("https://api.coinpaprika.com/v1/coins/" + coinID));

                        if (info.rank.ToString() != "0") { mensagem.Append("*" + info.rank.ToString() + " - *"); }
                        mensagem.AppendLine("*" + info.name + " (" + info.symbol + ")*");
                        mensagem.AppendLine();
                        mensagem.AppendLine("_" + info.description + "_").AppendLine();
                        if (info.whitepaper.link != null) { mensagem.AppendLine("*Whitepaper*: [acessar](" + info.whitepaper.link + ")").AppendLine(); }
                        mensagem.AppendLine("*Detalhes:*");
                        if (info.type != null) { mensagem.AppendLine("Tipo: `" + info.type.ToString() + "`"); }
                        if (info.is_new != null) { mensagem.AppendLine("É nova: `" + TFSN(info.is_new.ToString()) + "`"); }
                        if (info.is_active != null) { mensagem.AppendLine("Está ativa: `" + TFSN(info.is_active.ToString()) + "`"); }
                        if (info.open_source != null) { mensagem.AppendLine("Open Source: `" + TFSN(info.open_source.ToString()) + "`"); }
                        if (info.hardware_wallet != null) { mensagem.AppendLine("Hardware Wallet: `" + TFSN(info.hardware_wallet.ToString()) + "`"); }
                        if (info.development_status != null) { mensagem.AppendLine("Estado de desenvolvimento: `" + info.development_status + "`"); }
                        if (info.org_structure != null) { mensagem.AppendLine("Estrutura Organizacional: `" + info.org_structure + "`"); }
                        if (info.hash_algorithm != null) { mensagem.AppendLine("Algoritmo de Hash: `" + info.hash_algorithm + "`"); }
                        if (info.proof_type != null) { mensagem.AppendLine("Proof type: `" + info.proof_type + "`"); }
                        mensagem.AppendLine();
                        if (info.links != null) { mensagem.AppendLine("*Links:*"); }
                        if (info.links.explorer != null) { mensagem.AppendLine("[explorer](" + info.links.explorer[0] + ")"); }
                        if (info.links.facebook != null) { mensagem.AppendLine("[facebook](" + info.links.facebook[0] + ")"); }
                        if (info.links.reddit != null) { mensagem.AppendLine("[reddit](" + info.links.reddit[0] + ")"); }
                        if (info.links.source_code != null) { mensagem.AppendLine("[código fonte](" + info.links.source_code[0] + ")"); }
                        if (info.links.website != null) { mensagem.AppendLine("[website](" + info.links.website[0] + ")"); }
                        if (info.links.youtube != null) { mensagem.AppendLine("[youtube](" + info.links.youtube[0] + ")"); }
                        if (info.links.medium != null) { mensagem.AppendLine("[medium](" + info.links.medium[0] + ")"); }
                    }

                    telegramEnviarMensagem(e.Message.Chat.Id, mensagem.ToString(), true);
                }
            }
        }

        static string TFSN(string estado)
        {
            if (String.Equals(estado, "true", StringComparison.OrdinalIgnoreCase))
            {
                return "sim";
            }
            else
            {
                return "não";
            }
        }

        static string GetCoinID(string busca, dynamic coinList)
        {
            for (int i = 0; i < ((JArray)coinList).Count; i++)
            {
                if (String.Equals(coinList[i].id.ToString(), busca, StringComparison.OrdinalIgnoreCase) | String.Equals(coinList[i].name.ToString(), busca, StringComparison.OrdinalIgnoreCase) | String.Equals(coinList[i].symbol.ToString(), busca, StringComparison.OrdinalIgnoreCase))
                {
                    return coinList[i].id;
                }
            }
            return null;
        }


        public static void telegramEnviarMensagem(Telegram.Bot.Types.ChatId chatID, string mensagem, bool disablePreview)
        {
            botClient.SendTextMessageAsync(chatID, mensagem, Telegram.Bot.Types.Enums.ParseMode.Markdown, disablePreview);

            Console.WriteLine("Mensagem enviada para " + getChatNome(chatID) + " (" + chatID + ')');
        }

        static string getChatNome(Telegram.Bot.Types.ChatId chatID)
        {
            string nomeChat = null;

            try
            {
                Telegram.Bot.Types.Chat chat = botClient.GetChatAsync(chatID).Result;

                if (chat.Type == Telegram.Bot.Types.Enums.ChatType.Channel || chat.Type == Telegram.Bot.Types.Enums.ChatType.Group || chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup && chat.Title.ToString().Length > 0)
                {
                    nomeChat += chat.Title;
                }
                if (chat.Type == Telegram.Bot.Types.Enums.ChatType.Private && chat.FirstName.Length > 0)
                {
                    if (!String.IsNullOrEmpty(nomeChat)) { nomeChat += ' '; }
                    nomeChat += chat.FirstName;
                }
                if (chat.Type == Telegram.Bot.Types.Enums.ChatType.Private && chat.LastName.Length > 0)
                {
                    if (!String.IsNullOrEmpty(nomeChat)) { nomeChat += ' '; }
                    nomeChat += chat.LastName;
                }
            }
            catch
            {
                nomeChat = "indefinido";
            }

            return nomeChat;
        }

        public static DateTimeOffset UnixTimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTimeOffset dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
            return dtDateTime;
        }

    }
}
