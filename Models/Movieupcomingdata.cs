using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Onlinemovietickitproject.Models
{
    public class Movieupcomingdata
    {
        public List<Movieapi> GetMovies()
        {
            var movies = new List<Movieapi>
            {
                new Movieapi
                {
                    Title = "Inception",
                    PosterPath = "https://th.bing.com/th/id/OIP.ps02Cq1ZLtzBEPPpSVttKgHaLH?w=119&h=180&c=7&r=0&o=5&dpr=1.4&pid=1.7",
                    AgeRating = "13+",
                    Language = "English"
                },
                new Movieapi
                {
                    Title = "The Dark Knight",
                    PosterPath = "https://image.tmdb.org/t/p/original/a6KOgIPyVpGeXUVPqFKlM5nVfPy.jpg",
                    AgeRating = "PG-13",
                    Language = "English"
                },
                new Movieapi
                {
                    Title = "Titanic",
                    PosterPath = "https://th.bing.com/th/id/R.f45a1fd15e707647a608dc14edfaf5d3?rik=AvBk3UYwcXQnEA&riu=http%3a%2f%2fwww.freemovieposters.net%2fposters%2ftitanic_1997_6120_poster.jpg&ehk=uc%2bcZgWEDisqstuf7TkH9FncFyTg0FI1MRkg8FP%2bw7M%3d&risl=&pid=ImgRaw&r=0",
                    AgeRating = "PG-13",
                    Language = "English"
                },
                new Movieapi
                {
                    Title = "Avatar",
                    PosterPath = "https://i.jeuxactus.com/datas/divers/c/i/cinema-et-jeux-video/xl/cinema-et-jeux-video-ar-632af9b057b56.jpg",
                    AgeRating = "PG-13",
                    Language = "English"
                },
                new Movieapi
                {
                    Title = "The Matrix",
                    PosterPath = "https://tse1.mm.bing.net/th/id/OIP.8n3V_HSzCyYRZJ51mD8YNAHaLH?cb=iwc1&rs=1&pid=ImgDetMain",
                    AgeRating = "R",
                    Language = "English"
                }
            };

            return movies;
        }
    }
}


