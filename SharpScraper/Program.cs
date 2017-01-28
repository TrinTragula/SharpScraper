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

// to do: threading and premere q per uscire

namespace SharpScraper
{
    class Program
    {
        // Per comodità, stampa una lsita di stringhe
        static void stampaLista(List<string> lista)
        {
            for (int i = 0; i < lista.Count(); i++)
            {
                Console.WriteLine(lista[i]);
            }
        }

        static void Main(string[] args)
        {
            string banner =
@" ############################################################################ 
 #             SharpScraper - A pastebin scraper written in c#              # 
 #                      Mady by Daniele Scarinci in 2017.                   # 
 #                           Open source and free.                          # 
 ############################################################################ 

";

            var mongoDB = new Database();
            var sharpScraper = new Scraper();
            // Scegliere qua il regex da usare
            string regex = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            Console.SetWindowSize( Math.Min(78, Console.LargestWindowWidth), Math.Min(30, Console.LargestWindowHeight));
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(banner);
            Console.ResetColor();

            sharpScraper.startScraping(regex, mongoDB);
            Console.ReadKey();

        }
    }
}
