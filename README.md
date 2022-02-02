This is intended for a live demo using [DotNetFiddle](https://dotnetfiddle.net/) to learn [Dapper](https://github.com/DapperLib/Dapper), a great library for working with SQL in C#.

To setup your fiddle environment, add the NuGet package **AO.DapperLunchAndLearn** and you'll be able to do these exercises.

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

This is a single insert using one of a model class instance as a parameter.
    
```csharp
await cn.ExecuteAsync("INSERT INTO [Artist] ([Name], [CreatedBy]) VALUES (@Name, @CreatedBy)", new Artist()
{
    Name = "Talking Heads",
    CreatedBy = "adamo"
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
    <summary>Ex 3: Insert several rows, and output all of them</summary>
    
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
    <summary>Ex 4: select all and display info by creator</summary>
    
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
