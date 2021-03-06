﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
    public class RepoListViewmodel : AppViewmodel
    {
        public string _login;
        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                Set(() => Login, ref _login, value);
            }
        }

        public bool _zeroRepoCount;
        public bool ZeroRepoCount
        {
            get
            {
                return _zeroRepoCount;
            }
            set
            {
                Set(() => ZeroRepoCount, ref _zeroRepoCount, value);
            }
        }
        public ObservableCollection<Repository> _repositories;
        public ObservableCollection<Repository> Repositories
        {
            get
            {
                return _repositories;
            }
            set
            {
                Set(() => Repositories, ref _repositories, value);
            }

        }
        public async Task Load(string login)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels           
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels

                isLoading = true;
                Repositories = await RepositoryUtility.GetRepositoriesForUser(login);
                Login = login;
                if(Repositories.Count == 0)
                {
                    ZeroRepoCount = true;
                }
                else
                {
                    ZeroRepoCount = false;
                }
                isLoading = false;
                
            }

        }
        public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(RepoDetailView), e.ClickedItem as Repository);
        }
        public async void RefreshCommand(object sender, EventArgs e)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;

                    isLoading = true;
                    Repositories = await RepositoryUtility.GetRepositoriesForUser(Login);
                    if (Repositories.Count == 0)
                    {
                        ZeroRepoCount = true;
                    }
                    else
                    {
                        ZeroRepoCount = false;
                    }
                       
                    isLoading = false;
            }
        }
    }
}
