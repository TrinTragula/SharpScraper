using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapySharp.Network;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Text.RegularExpressions;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Bson;

namespace SharpScraper
{
    /* Interfaccia per il mio scraper
     * Avrà le seguenti 3 funzioni: */
    interface IScraper
    {
        List<string> getRecentPaste(); // Ottieni gli ultimi paste più recenti da http://pastebin.com/archive
        string getPasteText(string url); // Ottieni il testo del paste all'url indicato (/stringa), da appendere a http://pastebin.com/raw
        bool isKeywordPresent(string text, string regex); // Controlla se il paste contiene una delle keyword che stiamo cercando
    }

    // Definizone classe per pastebin
    class Scraper : IScraper
    {
        // url hardcoded di pastebin, da cambiare nel caso cambi interfaccia web
        public string pasteBinUrl = "http://pastebin.com/archive";
        public string pasteBinArchiveUrl = "http://pastebin.com/archive";
        public string pasteBinRawUrl = "http://pastebin.com/raw";
        public int timeOut = 10000; // tempo da attendere fra successive richieste in ms (per non essere bloccati e rispettare i TOS)
        private int idScraper;
        public static int numberOfScrapers = 0;
        ScrapingBrowser scraperBrowser;

        public Scraper()
        {
            // Creo un browser per scaricare la pagian delgi ultimi paste
            this.scraperBrowser = new ScrapingBrowser();
            this.scraperBrowser.AllowAutoRedirect = true;
            this.scraperBrowser.AllowMetaRedirect = true;
            this.idScraper = numberOfScrapers;
            numberOfScrapers++;
        }

        // Ottieni gli ultimi paste più recenti da http://pastebin.com/archive
        public List<string> getRecentPaste()
        {
            List<string> pastebins = new List<string>();
            // Effettuo la richiesta
            WebPage responsePage = scraperBrowser.NavigateToPage(new Uri("http://pastebin.com/archive"));
            // Uso HtmlAgilityPack per selezionare l'elemento della pagina HTML con classe maintable
            // ovvero la tabella in cui sono contenuti i link ai paste recenti
            var pastebinsTable = responsePage.Html.CssSelect(".maintable").First();
            // Seleziono i membri della tabella
            var row = pastebinsTable.SelectNodes("tr/td");
            // Ogni paste è composto da tre elementi
            // Il primo elemento è il link in formato <a href="/pagina">
            // Il secondo è quanto tempo fa è stato creato il paste
            // Il terzo indica il linguaggio di programmazione, se presente, usato nel paste
            // Ciclo dunque con icnrementni di 3
            for (int i = 0; i < row.Count; i += 3)
            {
                // Ottengo il link da dentro le virgolette
                string s = row[i].LastChild.OuterHtml;
                int start = s.IndexOf('"') + 1;
                int end = s.IndexOf('"', start);
                string actualLink = s.Substring(start, end - start);
                // Ottengo il tempo ( potrebbe essermi utile in futuro )
                string timeAgo = row[i + 1].LastChild.OuterHtml;
                // Ottengo il linguaggio usato
                string languageUsed = row[i + 2].LastChild.OuterHtml;
                // Aggiungo il link alla lista
                pastebins.Add(actualLink);
            }
            return pastebins;
        }

        public string getPasteText(string url)
        {
            // Scarico il paste da pastebin.com/raw/url, in puro testo
            string actualUrl = pasteBinRawUrl + url;
            try
            {
                WebPage responsePage = scraperBrowser.NavigateToPage(new Uri(actualUrl));
                return responsePage.Content;
            }
            catch
            {
                string response = "404 not found";
                return response;
            }


        }

        // Funzione per confrontare una stringa con un'espressione regolare
        public bool isKeywordPresent(string text, string regex)
        {
            Regex r = new Regex(regex);
            bool isKeywordHere = r.IsMatch(text);
            return isKeywordHere;
        }

        // ROutine per cominciare il monitoraggio
        public void startScraping(string regex, Database mongoDB)
        {
            decimal i = 0;
            string paste;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Cerco secondo la regex {0}...", regex);
            Console.ResetColor();
            // loop senza fine, finché l'utent non preme ctrl+c
            for (;;)
            {
                Console.WriteLine("");
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Richiesta numero {0}... (timeout di 30s per evitare un ban, ctrl+c per uscire)", i + 1);
                Console.ResetColor();
                // Tra ogni batch di richieste, aspetta un po' di più
                Thread.Sleep(30000);
                List<string> pasteList = getRecentPaste();
                Console.WriteLine("{0} paste ottenuti!", pasteList.Count());          
                for (int j = 0; j < pasteList.Count(); j++)
                {
                    Thread.Sleep(timeOut); //per non generare troppo traffico, aspetta tra una richiesta e l'altra
                    paste = getPasteText(pasteList[j]);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\rCerco paste {0} di {1}   ", j+1, pasteList.Count());
                    Console.ResetColor();
                    if (isKeywordPresent(paste, regex))
                    {
                        Console.WriteLine("");
                        string actualUrl = pasteBinRawUrl + pasteList[j];
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("### Trovata una corrispondenza! Url: {0} ###", actualUrl);
                        Console.ResetColor();
                        // Inserisci nel database il paste che hai scoeprto e corrisponde ai nsotir criteri di ricerca
                        mongoDB.insertPaste(paste, actualUrl);
                        Console.WriteLine("");
                    }
                }
                // Incremento per ricordare quante richieste ho fatto
                i++;
            }

        }

    }
}
