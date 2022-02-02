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
    
In this example, we build a set of `Artist` from an array of strings. Then we perform inserts of all those `Artist` instances. Then we query all of them and output them to the console.
    
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

var allArtists = await cn.QueryAsync<Artist>("SELECT * FROM [Artist] ORDER BY [Name]");

foreach (var artist in allArtists) Console.WriteLine($"{artist.Name}: {artist.Id} ({artist.CreatedBy})");
```
</details>
