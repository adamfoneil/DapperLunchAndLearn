using Dapper;
using DapperLunchAndLearn;
using DapperLunchAndLearn.Models;
using System.Linq;

using var cn = Connection.Open();

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
