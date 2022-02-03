This is intended for a live demo using a certain [DotNetFiddle](https://dotnetfiddle.net/Bni3CE) to learn [Dapper](https://github.com/DapperLib/Dapper), a great library for working with SQL in C#.

# The database schema

To do anything with Dapper, you need a database to play with. I've created something really simple to store music metadata.

![img](https://adamosoftware.blob.core.windows.net/images/5I6RP2L4P0.png)

These tables were built from these [model classes](https://github.com/adamfoneil/DapperLunchAndLearn/tree/master/DapperLunchAndLearn/Models).

# Exercises

All of the exercises below require these first statements:

```csharp
using Dapper;
using DapperLunchAndLearn;
using DapperLunchAndLearn.Models;

using var cn = Connection.Open();
```

<details>
<summary>Ex 1: A simple insert</summary>

This is a single insert using one of a model class instance as a parameter. Fill in `<your name>` with a string that is your name.
    
```csharp
await cn.ExecuteAsync("INSERT INTO [Artist] ([Name], [CreatedBy]) VALUES (@Name, @CreatedBy)", new Artist()
{
    Name = "Talking Heads",
    CreatedBy = <your name>
});
```
</details>

<details>
<summary>Ex 2: Insert and capture generated Id</summary>

Here we perform a similar insert, but capture the generated Id value.

```csharp
var id = await cn.QuerySingleAsync<int>(
    @"INSERT INTO [Artist] ([Name], [CreatedBy]) VALUES (@Name, @CreatedBy);
    SELECT SCOPE_IDENTITY()", new Artist()
{
    Name = "Celine Dion",
    CreatedBy = "adamo"
});

Console.WriteLine($"Id = {id}");
```
</details>

<details>
    <summary>Ex 3: Insert several rows</summary>
    
In this example, we build a set of `Artist` from an array of strings using LINQ `Select`. Then we insert of all those `Artist` instances individually. Fill in your own artists!
    
```csharp
var artists = new[]
{
    "Duran Duran", "Devo", "Midnight Oil"
}.Select(name => new Artist() 
{ 
    Name = name, 
    CreatedBy = "adamo" 
});

foreach (var artist in artists)
{
    await cn.ExecuteAsync("INSERT INTO [Artist] ([Name], [CreatedBy]) VALUES (@Name, @CreatedBy)", artist);
}
```
</details>

<details>
    <summary>Ex 4: Select all and display info by creator</summary>
    
Now we're querying data! We take a plain "flat" result set with no grouping and use LINQ `GroupBy` to shape the output. Watch for opportunities to combine LINQ and SQL to do useful things. Don't see LINQ and Dapper as mutually exclusive.
    
```csharp
var allArtists = await cn.QueryAsync<Artist>("SELECT * FROM [Artist] ORDER BY [Name]");

foreach (var creatorGrp in allArtists.GroupBy(row => row.CreatedBy))
{
    Console.WriteLine($"{creatorGrp.Key} ({creatorGrp.Count()})");
    foreach (var artist in creatorGrp)
    {
        Console.WriteLine($"\t{artist.Name}: {artist.Id}");
    }    
}
```
</details>

<details>
    <summary>Ex 5: Related inserts</summary>
    
Let's build a structure of related artists, albums, and songs and insert them together, chaining the foreign keys properly. In case we end up adding duplicate artists names, I made the inserts idempotent. Fill in your own artists, albums, and as much song data as you feel like.
    
```csharp
var data = new Artist[]
{
    new Artist
    {
        Name = "Paul Simon",
        Albums = new Album[]
        {
            new Album() 
            { 
                Title = "Graceland",
                Songs = new Song[]
                {
                    new Song() { Title = "Boy in the Bubble", TrackNumber = 1 },
                    new Song() { Title = "Graceland", TrackNumber = 2 }
                }
            },
            new Album() { Title = "Rhythm of the Saints" },
            new Album() { Title = "So Beautiful or So What" }
        }
    },
    new Artist
    {
        Name = "U2",
        Albums = new Album[]
        {
            new Album() { Title = "Boy" },
            new Album() { Title = "War" },
            new Album() { Title = "The Joshua Tree" }
        }
    }
};

foreach (var artist in data)
{
    artist.CreatedBy = "adamo";
    var artistId = await cn.QuerySingleAsync<int>(
        @"INSERT INTO [Artist] ([Name], [CreatedBy]) 
        SELECT @Name, @CreatedBy WHERE NOT EXISTS(SELECT 1 FROM [Artist] WHERE [Name]=@Name);
        SELECT [Id] FROM [Artist] WHERE [Name]=@Name", artist);

    foreach (var album in artist.Albums)
    {
        album.ArtistId = artistId;
        album.CreatedBy = artist.CreatedBy;
        var albumId = await cn.QuerySingleAsync<int>(
            @"INSERT INTO [Album] ([ArtistId], [Title], [CreatedBy])
            SELECT @ArtistId, @Title, @CreatedBy WHERE NOT EXISTS(SELECT 1 FROM [Album] WHERE [ArtistId]=@ArtistId AND [Title]=@Title);
            SELECT [Id] FROM [Album] WHERE [ArtistId]=@ArtistId AND [Title]=@Title", album);

        if (album.Songs == null) continue;

        foreach (var song in album.Songs)
        {
            song.AlbumId = albumId;
            song.CreatedBy = album.CreatedBy;
            await cn.ExecuteAsync(
                @"INSERT INTO [Song] ([AlbumId], [Title], [TrackNumber], [CreatedBy])
                SELECT @AlbumId, @Title, @TrackNumber, @CreatedBy WHERE NOT EXISTS(SELECT 1 FROM [Song] WHERE [AlbumId]=@AlbumId AND [Title]=@Title)", song);
        }
    }
}
```
</details>

<details>
    <summary>Ex 6: Fill in some missing year data</summary>
    
Let's fill in some missing year data for albums already inserted, then query it to see what we have. Notice how the artist name is joined into the results. Without this, we would see only the `ArtistId` in the output. For that to work, we have to add an `ArtistName` column ([property](https://github.com/adamfoneil/DapperLunchAndLearn/blob/master/DapperLunchAndLearn/Models/Album.cs#L25)) to our model class. I use the `[NotMapped]` attribute to make sure I don't create it as an actual column in the database.
    
```csharp
var updates = new Album[]
{
    new Album() { Title = "Boy", Year = 1980 },
    new Album() { Title = "Graceland", Year = 1986 },
    new Album() { Title = "The Joshua Tree", Year = 1987 }
};

foreach (var album in updates)
{
    await cn.ExecuteAsync("UPDATE [Album] SET [Year]=@Year WHERE [Title]=@Title", album);
}

var allAbums = await cn.QueryAsync<Album>(
    @"SELECT 
        [alb].*, 
        [art].[Name] AS [ArtistName]
    FROM 
        [Album] [alb] INNER JOIN [Artist] [art] ON [alb].[ArtistId]=[art].[Id]
    WHERE 
        [alb].[Year] IS NOT NULL");

foreach (var album in allAbums) Console.WriteLine($"{album.ArtistName}: {album.Title} ({album.Year}), entered by {album.CreatedBy}");
```
</details>

<details>
    <summary>Ex 7: Query related data</summary>

Often you'll need to query related together, such as header and detail rows. I believe there's a clever way to map nested data with Dapper, but the way I do it that I understand is to do several queries at the outset, then divide up the results by some key value using LINQ `ToLookup`. From a performance standpoint, the important thing is to avoid queries within loops. Instead, execute one database roundtrip, then use LINQ methods to shape the results in memory in a useful way.
    
This example queries artists, albums, and songs, and groups them by their respective parent key value. Then it outputs everything to the console.
  
```csharp
var allArtists = await cn.QueryAsync<Artist>("SELECT * FROM [Artist]");
var albumsByArtist = (await cn.QueryAsync<Album>("SELECT * FROM [Album]")).ToLookup(row => row.ArtistId);
var songsByAlbum = (await cn.QueryAsync<Song>("SELECT * FROM [Song] ORDER BY [TrackNumber]")).ToLookup(row => row.AlbumId);

foreach (var artist in allArtists)
{
    artist.Albums = albumsByArtist[artist.Id];
    foreach (var album in artist.Albums) album.Songs = songsByAlbum[album.Id];

    Console.WriteLine(artist.Name);
    foreach (var album in artist.Albums)
    {
        Console.WriteLine($"\t{album.Title} ({album.Year})");
        foreach (var song in album.Songs)
        {
            Console.WriteLine($"\t\t{song.TrackNumber}: {song.Title}");
        }
    }
}
```   
</details>

<details>
    <summary>Ex 8: Simpler inserts with Dommel</summary>
    
The [Dommel](https://github.com/HenkMollema/Dommel) library offers some extension methods to make it simple to insert and update from model classes. Fill in your own artist name and user name. By default, Dommel assumes that your table names are the plural form of your class name. You can override this with the [Table](https://github.com/adamfoneil/DapperLunchAndLearn/blob/master/DapperLunchAndLearn/Models/Artist.cs#L8) attribute. Dommel also seems to require an explicit `Key` property. I set this in my [BaseTable](https://github.com/adamfoneil/DapperLunchAndLearn/blob/master/DapperLunchAndLearn/Models/Conventions/BaseTable.cs#L8) so all my models inherit it.
     
```csharp
var objId = await cn.InsertAsync(new Artist()
{
    Name = "Beck",
    CreatedBy = "<your name>"
});

var id = Convert.ToInt32(objId);

Console.WriteLine($"id = {id}");
```
</details>

<details>
    <summary>Ex 9: Update with Dommel</summary>
    
Updating a row is similarly very easy with Dommel. Here, I'm creating a new row, capturing its `Id`, updating, then fetching it again.
    
```csharp
var artist = new Artist()
{
    Name = "Elvis Presley",
    CreatedBy = "adamo"
};

var objId = await cn.InsertAsync(artist);
artist.Id = Convert.ToInt32(objId);

artist.Name = "John Williams";
await cn.UpdateAsync(artist);

artist = await cn.GetAsync<Artist>(objId);

// should be John Williams
Console.WriteLine(artist.Name);
```
</details>

# More
- There's a pretty active ecosystem of Dapper-related packages and extensions out there. See [NuGet.org](https://www.nuget.org/packages?q=dapper)
- I have a couple packages of my own:
    - [Dapper.QX](https://github.com/adamfoneil/Dapper.QX) for more powerful and testable inline SQL
    - [Dapper.Repository](https://github.com/adamfoneil/Dapper.Repository) to let you implement Dapper in the repository pattern
