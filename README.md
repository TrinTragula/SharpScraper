# SharpScraper
A pastebin scraper written in C#, using MongoDB to store findings.

![](http://pastebin.com/i/pastebin_logo_side_outline.png)


## What you need
In order to get it to work you need to have a MongoDB database up and running. The code should be commented well enough (actually, it's in italian, I'll probably translate it someday)

## Why is it so slow?
It waits a lot between requests in order not to get banned by pastebin very strict policy aginst abuse.
Don't reduce them. If you do it, you'll probably have your ip address blocked.
[Here is Pastebin statement about scraping their website](http://pastebin.com/scraping)

## Who made it?
I made it, Daniele aka TrinTragula. It's completly free to use, share and modify. Just a random project I made to help me learn C# and MongoDB.
