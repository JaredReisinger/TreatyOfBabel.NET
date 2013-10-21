#TreatyOfBabel.NET

A .NET implementation of the [Treaty of Babel](http://babel.ifarchive.org)
specification for text-adventure games.  Where the specification is very
C-like, however, this .NET version should feel a lot more natural for .NET
developers: the different game format providers are discovered via MEF, and
some of the APIs have been made more .NET friendly.

###Other Tools

In addition to the TreatyofBabel.dll utility library, there are two programs
that use the library: IffDump and GameLibrary.  IffDump is a command-line tool
that reports the structure of an IFF file.  GameLibrary is a graphical
front-end that shows the extracted information about each game under a
directory (recursively).

##Background

Growing up in the 1980s, I spent a lot of time playing
[Infocom](http://en.wikipedia.org/wiki/Infocom) games like
[Zork](http://en.wikipedia.org/wiki/Zork), and
[Hitchhiker's Guide to the Galaxy](http://en.wikipedia.org/wiki/The_Hitchhiker%27s_Guide_to_the_Galaxy_(computer_game)).
It turns out I'm not the only person who continues to enjoy the medium
of interactive ficton.  There's a sizable, and thriving, subculture of
game authors, and many different kinds of games.

###This seems cool... where can I find out more?

Searching for "interactive fiction" in your favorite search engine is a
good way to start.  [IFWiki](http://www.ifwiki.org/) is the Wikipedia of
interactive fiction, and there are some other good sources out there as well.
If you're looking for games, one of the oldest collections is
[The Interactive Fiction Archive](http://www.ifarchive.org/).  There's
also [The Interactive Fiction Database (IFDB)](http://ifdb.tads.org/), which
is like IMDb, but for text-adventure games.  IFDB can help you find games
that match your style of play, from adventure-style collect-the-treasure
quests, to solve-the-puzzle games, to the more avant-garde explorations of
the interactive fiction world.

###How can I play a game?

You'll need an interpreter that can handle the specific format of game you
want to play.  The Game Library will simply ask Windows to open the game
file; it's the interpreter that actually handles starting the game.  You can
use [Gargoyle](http://ccxvii.net/gargoyle/), or
[Zoom](http://www.logicalshift.co.uk/unix/zoom/), or take a look at the
[IFDB Meta Installer](http://ifdb.tads.org/plugins/index).

