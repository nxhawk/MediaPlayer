﻿using MediaPlayer.Keys;
using MediaPlayer.ViewModels;
using MediaPlayer.Views.Dialog;
using Newtonsoft.Json;
using System;
using System.Windows;
using File = System.IO.File;

namespace MediaPlayer
{
   
    public partial class App : Application
    {
        private PlaylistViewModel _playlistViewModel = new PlaylistViewModel();
        private MusicPlayerViewModel _musicPlayerViewModel = new MusicPlayerViewModel();
        private string _baseDomain = AppDomain.CurrentDomain.BaseDirectory;
        private string _playlistFile = "playlists.json";
        private string _musicFile = "music.json";
        private string _slash = "/";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new MainWindow();

            try
            {
                if (File.Exists(_baseDomain + _slash + _playlistFile))
                {
                    var json = File.ReadAllText(_baseDomain + _slash + _playlistFile);
                    if (json != null || json != "")
                    {
                        _playlistViewModel = JsonConvert.DeserializeObject<PlaylistViewModel>(json!)!;
                    }
                }

                window.PlaylistViewModel = _playlistViewModel;

                if (File.Exists(_baseDomain + _slash + _musicFile))
                {
                    var json = File.ReadAllText(_baseDomain + _slash + _musicFile);
                    if (json != null || json != "")
                    {
                        _musicPlayerViewModel = JsonConvert.DeserializeObject<MusicPlayerViewModel>(json!)!;
                    }
                }
                window.MusicPlayerViewModel = _musicPlayerViewModel;
            }
            catch {
                _playlistViewModel = new PlaylistViewModel();
                _musicPlayerViewModel = new MusicPlayerViewModel();
                window.PlaylistViewModel = _playlistViewModel;
                window.MusicPlayerViewModel = _musicPlayerViewModel;

                CustomMessageBox dialog = new CustomMessageBox("Error restoring data, application will restart");
                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                dialog.Left = (desktopWorkingArea.Right - dialog.Width)/2;
                dialog.Top = (desktopWorkingArea.Bottom - dialog.Height)/2;
                dialog.ShowDialog();
            }

            window.Show();            
        }

        private void Application_Activated(object sender, EventArgs e)
        {
            HotkeysManager.SetupSystemHook();
        }

        private void Application_Deactivated(object sender, EventArgs e)
        {
            HotkeysManager.ShutdownSystemHook();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _musicPlayerViewModel.shufflePlaylist(0);
            _musicPlayerViewModel.MediaElement.Pause();
            base.OnExit(e);

            var json = JsonConvert.SerializeObject(_playlistViewModel).ToString();
            File.WriteAllText(_baseDomain + _slash + _playlistFile, json);

            _musicPlayerViewModel.CurrentState = "Pause";
            json = JsonConvert.SerializeObject(_musicPlayerViewModel).ToString();
            File.WriteAllText(_baseDomain + _slash + _musicFile, json);

            if (_musicPlayerViewModel.CurrentMedia == null)
            {
                return;
            }
            _musicPlayerViewModel.MediaElement.Close();
        }
    }
}
