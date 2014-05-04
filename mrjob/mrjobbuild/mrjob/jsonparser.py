#JSON parser

import json

newyorktimes = {
  "status":"OK",
  "copyright":"Copyright (c) 2008 The New York Times Company.  All Rights Reserved.",
  "num_results":8,
  "results":[
     {
        "nyt_movie_id":135855,
        "display_title":"Big Night",
        "sort_name":"Big Night",
        "mpaa_rating":"R",
        "critics_pick":"Y",
        "thousand_best":"Y",
        "byline":"Janet Maslin",
        "headline":"",
        "capsule_review":"Restaurateur brothers stake all on one dinner. Succulent comedy.",
        "summary_short":"",
        "publication_date":"1996-03-29",
        "opening_date":"1996-09-20",
        "dvd_release_date":"1998-04-07",
        "date_updated":"2008-11-04 03:54:15",
        "seo-name":"Big-Night",
        "link":{
           "type":"article",
           "url":"http:\/\/movies.nytimes.com\/movie\/review?res=9501E6DA1539F93AA15750C0A960958260",
           "suggested_link_text":"Read the New York Times Review of Big Night"
        },
        "related_urls":[
           {
              "type":"overview",
              "url":"http:\/\/movies.nytimes.com\/movie\/135855\/Big-Night\/overview",
              "suggested_link_text":"Overview of Big Night"
           },
           {
              "type":"showtimes",
              "url":"http:\/\/movies.nytimes.com\/movie\/135855\/Big-Night\/showtimes",
              "suggested_link_text":"Tickets & Showtimes for Big Night"
           },
           {
              "type":"awards",
              "url":"http:\/\/movies.nytimes.com\/movie\/135855\/Big-Night\/details",
              "suggested_link_text":"Cast, Credits & Awards for Big Night"
           },
           {
              "type":"community",
              "url":"http:\/\/movies.nytimes.com\/movie\/135855\/Big-Night\/rnr",
              "suggested_link_text":"Readers' Reviews of Big Night"
           },
           {
              "type":"trailers",
              "url":"http:\/\/movies.nytimes.com\/movie\/135855\/Big-Night\/trailers",
              "suggested_link_text":"Trailers & Clips for Big Night"
           }
        ]
     }
  ]
}

nyt_results = newyorktimes['results']
res = nyt_results[0]
thousand = res['thousand_best']
critics = res['critics_pick']
print critics
print thousand






rottentomatoes = {
  "id": 770672122,
  "title": "Toy Story 3",
  "year": 2010,
  "genres": [
    "Animation",
    "Kids & Family",
    "Science Fiction & Fantasy",
    "Comedy"
  ],
  "mpaa_rating": "G",
  "runtime": 103,
  "critics_consensus": "Deftly blending comedy, adventure, and honest emotion, Toy Story 3 is a rare second sequel that really works.",
  "release_dates": {
    "theater": "2010-06-18",
    "dvd": "2010-11-02"
  },
  "ratings": {
    "critics_rating": "Certified Fresh",
    "critics_score": 99,
    "audience_rating": "Upright",
    "audience_score": 91
  },
  "synopsis": "Pixar returns to their first success with Toy Story 3. The movie begins with Andy leaving for college and donating his beloved toys -- including Woody (Tom Hanks) and Buzz (Tim Allen) -- to a daycare. While the crew meets new friends, including Ken (Michael Keaton), they soon grow to hate their new surroundings and plan an escape. The film was directed by Lee Unkrich from a script co-authored by Little Miss Sunshine scribe Michael Arndt. ~ Perry Seibert, Rovi",
  "posters": {
    "thumbnail": "http://content6.flixster.com/movie/11/13/43/11134356_mob.jpg",
    "profile": "http://content6.flixster.com/movie/11/13/43/11134356_pro.jpg",
    "detailed": "http://content6.flixster.com/movie/11/13/43/11134356_det.jpg",
    "original": "http://content6.flixster.com/movie/11/13/43/11134356_ori.jpg"
  },
  "abridged_cast": [
    {
      "name": "Tom Hanks",
      "characters": ["Woody"]
    },
    {
      "name": "Tim Allen",
      "characters": ["Buzz Lightyear"]
    },
    {
      "name": "Joan Cusack",
      "characters": ["Jessie the Cowgirl"]
    },
    {
      "name": "Don Rickles",
      "characters": ["Mr. Potato Head"]
    },
    {
      "name": "Wallace Shawn",
      "characters": ["Rex"]
    }
  ],
  "abridged_directors": [{"name": "Lee Unkrich"}],
  "studio": "Walt Disney Pictures",
  "alternate_ids": {"imdb": "0435761"},
  "links": {
    "self": "http://api.rottentomatoes.com/api/public/v1.0/movies/770672122.json",
    "alternate": "http://www.rottentomatoes.com/m/toy_story_3/",
    "cast": "http://api.rottentomatoes.com/api/public/v1.0/movies/770672122/cast.json",
    "clips": "http://api.rottentomatoes.com/api/public/v1.0/movies/770672122/clips.json",
    "reviews": "http://api.rottentomatoes.com/api/public/v1.0/movies/770672122/reviews.json",
    "similar": "http://api.rottentomatoes.com/api/public/v1.0/movies/770672122/similar.json"
  }
}

cast = []
director = []
title = rottentomatoes['title']
mpaa = rottentomatoes['mpaa_rating']
ratings = rottentomatoes['ratings']
critic_score = ratings['critics_score']
aud_score = ratings['audience_score']

for x in rottentomatoes['abridged_cast']:
	cast.append(x['name'])


for y in rottentomatoes['abridged_directors']:
	director.append(y['name'])


studio = rottentomatoes['studio']
posters = rottentomatoes['posters']
poster = posters['original']

class MovieD:
	def __init__(self,title,crit_score,aud_score,thous,crit,mpaa,poster,cast,direct,studio):
		self.data = [ ]
		self.studio = studio
		self.title = title
		self.crit_score = crit_score
		self.aud_score = aud_score
		self.thous = thous
		self.crit = crit
		self.mpaa = mpaa
		self.poster = poster
		self.cast = cast
		self.direct = direct
		self.studio = studio

Toy_Story = MovieD(title,critic_score,aud_score,thousand,critics,mpaa,poster,cast,director,studio)
print Toy_Story

