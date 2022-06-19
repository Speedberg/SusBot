using System;
using System.Collections.Generic;

namespace Speedberg.SusBot.Modules.Utility
{
    public abstract class AtlassianStatusResponse
    {
        public abstract class Root
        {

        }
    }

    public class GithubStatusJSON : AtlassianStatusResponse
    {
        public class Page
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string time_zone { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class Component
        {
            public string id { get; set; }
            public string name { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public int position { get; set; }
            public string description { get; set; }
            public bool showcase { get; set; }
            public string start_date { get; set; }
            public object group_id { get; set; }
            public string page_id { get; set; }
            public bool group { get; set; }
            public bool only_show_if_degraded { get; set; }
        }

        public class Status
        {
            public string indicator { get; set; }
            public string description { get; set; }
        }

        public class Root : AtlassianStatusResponse.Root
        {
            public Page page { get; set; }
            public List<Component> components { get; set; }
            public List<object> incidents { get; set; }
            public List<object> scheduled_maintenances { get; set; }
            public Status status { get; set; }
        }
    }

    public class SpotifyStatusJSON : AtlassianStatusResponse
    {
        public class Page
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string time_zone { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class Component
        {
            public string id { get; set; }
            public string name { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public int position { get; set; }
            public object description { get; set; }
            public bool showcase { get; set; }
            public object start_date { get; set; }
            public string group_id { get; set; }
            public string page_id { get; set; }
            public bool group { get; set; }
            public bool only_show_if_degraded { get; set; }
            public List<string> components { get; set; }
        }

        public class Status
        {
            public string indicator { get; set; }
            public string description { get; set; }
        }

        public class Root : AtlassianStatusResponse.Root
        {
            public Page page { get; set; }
            public List<Component> components { get; set; }
            public List<object> incidents { get; set; }
            public List<object> scheduled_maintenances { get; set; }
            public Status status { get; set; }
        }
    }

    public class SmhwStatusJSON : AtlassianStatusResponse
    {
        public class Page
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string time_zone { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class Component
        {
            public string id { get; set; }
            public string name { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public int position { get; set; }
            public string description { get; set; }
            public bool showcase { get; set; }
            public object start_date { get; set; }
            public object group_id { get; set; }
            public string page_id { get; set; }
            public bool group { get; set; }
            public bool only_show_if_degraded { get; set; }
        }

        public class Status
        {
            public string indicator { get; set; }
            public string description { get; set; }
        }

        public class Root : AtlassianStatusResponse.Root
        {
            public Page page { get; set; }
            public List<Component> components { get; set; }
            public List<object> incidents { get; set; }
            public List<object> scheduled_maintenances { get; set; }
            public Status status { get; set; }
        }
    }

    public class DiscordStatusJSON : AtlassianStatusResponse
    {
        public class Page
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class Status
        {
            public string description { get; set; }
            public string indicator { get; set; }
        }

        public class Root : AtlassianStatusResponse.Root
        {
            public Page page { get; set; }
            public Status status { get; set; }
        }
    }

    public class TwitterStatusJSON : AtlassianStatusResponse
    {
        public class Page
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public string TimeZone { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class Status
        {
            public string Indicator { get; set; }
            public string Description { get; set; }
        }

        public class Root : AtlassianStatusResponse.Root
        {
            public Page Page { get; set; }
            public Status Status { get; set; }
        }
    }

    public class RedditStatusJSON : AtlassianStatusResponse
    {
        public class Page
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public string TimeZone { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class Status
        {
            public string Indicator { get; set; }
            public string Description { get; set; }
        }

        public class Root : AtlassianStatusResponse.Root
        {
            public Page Page { get; set; }
            public Status Status { get; set; }
        }
    }
}