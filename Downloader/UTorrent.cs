﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UTorrentAPI;
using System.Net;

namespace DerpScrapper.Downloader
{
    static class UTorrent
    {
        private static Uri webGuiUri = new Uri("http://localhost:8080/gui");
        private static NetworkCredential webGuiCreds = new NetworkCredential("admin", "webapi");

        private static UTorrentClient _client;
        private static UTorrentClient client
        {
            get
            {
                if (_client == null)
                {
                    _client = new UTorrentClient(webGuiUri, webGuiCreds.UserName, webGuiCreds.Password);
                }
                return _client;
            }
        }

        public static void AddMagnet(string magnet)
        {
            client.Torrents.AddUrl(magnet);
        }

        public static void AddTorrent(Uri url)
        {
            client.Torrents.AddUrl(url.AbsolutePath);
        }

        public static void AddTorrent(string filePath)
        {
            client.Torrents.AddFile(filePath);
        }
    }
}