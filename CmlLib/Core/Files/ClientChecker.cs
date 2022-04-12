﻿using System;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public sealed class ClientChecker : IFileChecker
    {
        public bool CheckHash { get; set; } = true;

        public DownloadFile[]? CheckFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            progress?.Report(new DownloadFileChangedEventArgs(MFile.Minecraft, this, version.Jar, 1, 0));
            DownloadFile? result = checkClientFile(path, version);
            progress?.Report(new DownloadFileChangedEventArgs(MFile.Minecraft, this, version.Jar, 1, 1));

            if (result == null)
                return null;
            else
                return new [] { result };
        }

        public async Task<DownloadFile[]?> CheckFilesTaskAsync(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            progress?.Report(new DownloadFileChangedEventArgs(MFile.Minecraft, this, version.Jar, 1, 0));
            DownloadFile? result = await Task.Run(() => checkClientFile(path, version))
                .ConfigureAwait(false);
            progress?.Report(new DownloadFileChangedEventArgs(MFile.Minecraft, this, version.Jar, 1, 1));

            if (result == null)
                return null;
            else
                return new [] { result };
        }

        private DownloadFile? checkClientFile(MinecraftPath path, MVersion version)
        {
            var id = version.Jar;
            var url = version.Client?.Url;
            
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(id))
                return null;
            
            var clientPath = path.GetVersionJarPath(id);

            if (!IOUtil.CheckFileValidation(clientPath, version.Client?.Sha1, CheckHash))
            {
                return new DownloadFile(clientPath, url)
                {
                    Type = MFile.Minecraft,
                    Name = id
                };
            }
            
            return null;
        }
    }
}
