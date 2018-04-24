using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValueAddDemo
{
    public class Page<T>
    {
        public int total_count { get; set; }      
        public List<T> Items { get; set; }
       
    }
    public class JsonResponse
    {
        public class Schema
        {
            public string Name { get; set; }
        }
        
        //JSON pa to C# classes
        public class Detail
        {
            public string Database { get; set; }
            public string Server { get; set; }
            public List<Schema> Schema { get; set; }
            public string Type { get; set; }
            public string Port { get; set; }
            public string DatabaseValue { get; set; }
        }
        public class ApplicationDetail
        {
            public string Name { get; set; }
            public List<string> Paths { get; set; }
        }
        public class RepoDetail
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
        public class ServerDetail
        {
            public string Name { get; set; }
            public List<ApplicationDetail> ApplicationDetails { get; set; }
        }

        public class Team
        {
            public string Name { get; set; }
            public List<Detail> Details { get; set; }
            public List<ServerDetail> ServerDetails { get; set; }
            public List<RepoDetail> RepoDetails { get; set; }
        }

        public class Environment
        {
            public string Name { get; set; }
            public List<Team> Team { get; set; }
        }

        public class Root
        {
            public List<Environment> Environment { get; set; }
        }

        public class RootObject
        {
            public Root root { get; set; }
            public List<Item> items { get; set; }
        }
        public class Item
        {
            public string name { get; set; }
            public string path { get; set; }           
            public Repository repository { get; set; }
        }

        public class Repository
        {
            public int id { get; set; }
            public string name { get; set; }
            public string full_name { get; set; }                        
            public string html_url { get; set; }
            public object description { get; set; }
            
        }

    }
}