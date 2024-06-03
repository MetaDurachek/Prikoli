
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;

public class Track
{
    // убери публичные сеттеры
    public string Artist { get; set; }
    public string Album { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }

    public Track(string artist, string album, string title, int releaseYear)
    {
        Artist = artist;
        Album = album;
        Title = title;
        ReleaseYear = releaseYear;
    }
}

public class Album
{
    // Title наверн стоит сделать readonly
    public string Title;
    public List<Track> Tracks;

    public Album(string title)
    {
        Title = title;
        Tracks = new List<Track>();
    }

    public void AddTrack(Track track)
    {
        Tracks.Add(track);
    }
}

public class MusicLibrary
{
    public List<Album> Albums;

    public MusicLibrary()
    {
        Albums = new List<Album>();
    }

    public void AddAlbum(Album album)
    {
        Albums.Add(album);
    }

    public void AddTrack(string artist, string album, string title, int releaseYear)
    {
        Album albumObject = Albums.Find(a => a.Title == album);
        if (albumObject == null)
        {
            albumObject = new Album(album);
            AddAlbum(albumObject);
        }
        albumObject.AddTrack(new Track(artist, album, title, releaseYear));
    }

    public void RemoveTrack(string title)
    {
        foreach (Album album in Albums)
        {
            album.Tracks.RemoveAll(t => t.Title == title);
        }
    }
    
    // абстрагируйся в SearchByArtist и SearchByTitle, сделай новый приватный метод SearchBy, который получает лямбду (или как там у вас в шарпе)
    // чтоб методы SearchByArtist и SearchByTitle вызывали а-ля SearchBy(a => a.track.Artist.Contains(Artist)

    public Track[] SearchByArtist(string artist)
    {
        List<Track> result = new List<Track>();
        foreach (Album album in Albums)
        {
            foreach (Track track in album.Tracks)
            {
                if (track.Artist.Contains(artist)) // сделай, чтоб регистр букв игнорировался
                {
                    result.Add(track);
                }
            }
        }
        return result.ToArray();
    }

    public Track[] SearchByTitle(string title)
    {
        List<Track> result = new List<Track>();
        foreach (Album album in Albums)
        {
            foreach (Track track in album.Tracks)
            {
                if (track.Title == title)
                {
                    result.Add(track);
                }
            }
        }
        return result.ToArray();
    }

    public Track[] SearchByAlbum(string album)
    {
        List<Track> result = new List<Track>();
        Album albumObject = Albums.Find(a => a.Title.Contains(album));
        if (albumObject != null)
        {
            result.AddRange(albumObject.Tracks);
        }
        return result.ToArray();
    }

    public Track[] SearchByReleaseYear(int releaseYear)
    {
        List<Track> result = new List<Track>();
        foreach (Album album in Albums)
        {
            foreach (Track track in album.Tracks)
            {
                if (track.ReleaseYear == releaseYear)
                {
                    result.Add(track);
                }
            }
        }
        return result.ToArray();
    }

    public void SaveData(string filename)
    {
        try
        {
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(filename, json);
            Console.WriteLine("Сохранение файлов...");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка сохранения файлов: " + ex.Message);
        }
    }

    public static MusicLibrary LoadData(string filename)
    {
        try
        {
            string json = File.ReadAllText(filename);
            MusicLibrary library = JsonSerializer.Deserialize<MusicLibrary>(json);
            return library;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка загрузки файлов: " + ex.Message);
            return new MusicLibrary();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string filename = "data.json";
        MusicLibrary library;
        if (File.Exists(filename))
        {
            try
            {
                library = MusicLibrary.LoadData(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка загрузки файлов: " + ex.Message);
                library = new MusicLibrary();
            }
        }
        else
        {
            library = new MusicLibrary();
        }

        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Добавить трек");
            Console.WriteLine("2. Удалить трек");
            Console.WriteLine("3. Поиск по исполнителю");
            Console.WriteLine("4. Поиск по названию");
            Console.WriteLine("5. Поиск по альбому");
            Console.WriteLine("6. Поиск по году выпуска");
            Console.WriteLine("7. Вывести всю библиотеку");
            Console.WriteLine("8. Выход");

            int choice;
            try
            {
                choice = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Неправильный ввод. Пожалуйста, введите число.");
                continue;
            }

            // Вынеси все действия (непосредственно логику не работу с консолью) в новый класс MusicLibraryManager
            // чтоб разделить работу с пользовательским вводом и логику работы
            switch (choice)
            {
                case 1:
                    Console.Write("Введите имя исполнителя: ");
                    string artist = Console.ReadLine();
                    Console.Write("Введите название альбома: ");
                    string album = Console.ReadLine();
                    Console.Write("Введите название трека: ");
                    string title = Console.ReadLine();
                    Console.Write("Введите год выпуска: ");
                    int releaseYear = Convert.ToInt32(Console.ReadLine());
                    library.AddTrack(artist, album, title, releaseYear);
                    library.SaveData(filename);
                    break;
                case 2:
                    Console.Write("Введите название трека для удаления: ");
                    string trackToRemove = Console.ReadLine();
                    library.RemoveTrack(trackToRemove);
                    library.SaveData(filename);
                    break;
                case 3:
                    Console.Write("Введите имя исполнителя для поиска: ");
                    string artistToSearch = Console.ReadLine();
                    Track[] tracksByArtist = library.SearchByArtist(artistToSearch);
                    foreach (Track track in tracksByArtist)
                    {
                        Console.WriteLine($"Исполнитель: {track.Artist}, Альбом: {track.Album}, Название: {track.Title}, Год выпуска: {track.ReleaseYear}");
                    }
                    break;
                case 4:
                    Console.Write("Введите название трека для поиска: ");
                    string titleToSearch = Console.ReadLine();
                    Track[] tracksByTitle = library.SearchByTitle(titleToSearch);
                    foreach (Track track in tracksByTitle)
                    {
                        Console.WriteLine($"Исполнитель: {track.Artist}, Альбом: {track.Album}, Название: {track.Title}, Год выпуска: {track.ReleaseYear}");
                    }
                    break;
                case 5:
                    Console.Write("Введите название альбома для поиска: ");
                    string albumToSearch = Console.ReadLine();
                    Track[] tracksByAlbum = library.SearchByAlbum(albumToSearch);
                    foreach (Track track in tracksByAlbum)
                    {
                        Console.WriteLine($"Исполнитель: {track.Artist}, Альбом: {track.Album}, Название: {track.Title}, Год выпуска: {track.ReleaseYear}");
                    }
                    break;
                case 6:
                    Console.Write("Введите год выпуска для поиска: ");
                    int releaseYearToSearch = Convert.ToInt32(Console.ReadLine());
                    Track[] tracksByReleaseYear = library.SearchByReleaseYear(releaseYearToSearch);
                    foreach (Track track in tracksByReleaseYear)
                    {
                        Console.WriteLine($"Исполнитель: {track.Artist}, Альбом: {track.Album}, Название: {track.Title}, Год выпуска: {track.ReleaseYear}");
                    }
                    break;
                case 7:
                    foreach (Album albumObject in library.Albums)
                    {
                        Console.WriteLine($"Альбом: {albumObject.Title}");
                        foreach (Track track in albumObject.Tracks)
                        {
                            Console.WriteLine($"Исполнитель: {track.Artist}, Название: {track.Title}, Год выпуска: {track.ReleaseYear}");
                        }
                    }
                    break;
                case 8:
                    library.SaveData(filename);
                    return;
                default:
                    Console.WriteLine("Неправильный выбор. Пожалуйста, выберите снова.");
                    break;
            }
        }
    }
}
