# SharpScraper
A pastebin scraper written in C#, using MongoDB as a db.

![](http://pastebin.com/i/pastebin_logo_side_outline.png)


## What you need
You need to have a MongoDB database up and running in order to get it to work. The code should be commented well enough (actually, it's in italian, I'll probably translate it one day)

## Why is it so slow?
The program a lot between requests not to get banned by pastebin very strict policy against scraping.
Don't reduce the timeout between calls. If you do it, you'll probably have your ip address blocked in a matter of minutes.
[Here is a statement from Pastebin about scraping their website](http://pastebin.com/scraping)

## Who made it?
I made it, Daniele aka TrinTragula. It's completly free to use, share and modify. Just a random project I made to help myself learn C# and MongoDB.
