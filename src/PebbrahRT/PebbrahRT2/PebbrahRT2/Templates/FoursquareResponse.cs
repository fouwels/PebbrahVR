﻿using System.Collections.Generic;

namespace PebbrahRT2.Templates
{

    public class Meta
    {
        public int code { get; set; }
    }

    public class Contact
    {
        public string twitter { get; set; }
        public string phone { get; set; }
        public string formattedPhone { get; set; }
    }

    public class Location
    {
        public string address { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int distance { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string cc { get; set; }
        public string crossStreet { get; set; }
        public double myStarting { get; set; }
    }

    public class Icon
    {
        public string prefix { get; set; }
        public string suffix { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
        public string pluralName { get; set; }
        public string shortName { get; set; }
        public Icon icon { get; set; }
        public bool primary { get; set; }
    }

    public class Stats
    {
        public int checkinsCount { get; set; }
        public int usersCount { get; set; }
        public int tipCount { get; set; }
    }

    public class Specials
    {
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class HereNow
    {
        public int count { get; set; }
        public List<object> groups { get; set; }
    }

    public class VenuePage
    {
        public string id { get; set; }
    }

    public class Reservations
    {
        public string url { get; set; }
    }

    public class Menu
    {
        public string type { get; set; }
        public string label { get; set; }
        public string anchor { get; set; }
        public string url { get; set; }
        public string mobileUrl { get; set; }
    }

    public class Venue
    {
        public string id { get; set; }
        public string name { get; set; }
        public Contact contact { get; set; }
        public Location location { get; set; }
        public string canonicalUrl { get; set; }
        public List<Category> categories { get; set; }
        public bool verified { get; set; }
        public bool restricted { get; set; }
        public Stats stats { get; set; }
        public Specials specials { get; set; }
        public HereNow hereNow { get; set; }
        public string referralId { get; set; }
        public string url { get; set; }
        public VenuePage venuePage { get; set; }
        public string storeId { get; set; }
        public Reservations reservations { get; set; }
        public Menu menu { get; set; }
    }

    public class Response
    {
        public List<Venue> venues { get; set; }
    }

    public class FoursquareResponseRootObject
    {
        public Meta meta { get; set; }
        public Response response { get; set; }
    }
}
