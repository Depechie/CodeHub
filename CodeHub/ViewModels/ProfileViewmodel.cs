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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CodeHub.ViewModels
{
    public class ProfileViewmodel : AppViewmodel
    {
        public bool _isOrganization;
        public bool IsOrganization
        {
            get
            {
                return _isOrganization;
            }
            set
            {
                Set(() => IsOrganization, ref _isOrganization, value);
            }
        }

        public ObservableCollection<User> _followers;
        public ObservableCollection<User> Followers
        {
            get
            {
                return _followers;
            }
            set
            {
                Set(() => Followers, ref _followers, value);
            }
        }

        public ObservableCollection<User> _following;
        public ObservableCollection<User> Following
        {
            get
            {
                return _following;
            }
            set
            {
                Set(() => Following, ref _following, value);
            }
        }
        public async Task Load()
        {
            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); 
            }
            else
            {
                //Sending Internet available message to all viewModels
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType());

                isLoading = true;
               
                if (User != null)
                {
                    if (GlobalHelper.NewFollowActivity)
                    {
                        User = await UserUtility.getUserInfo(User.Login);
                        GlobalHelper.NewFollowActivity = false;
                    }

                    isLoggedin = true;
                    if (User.Type == AccountType.Organization)
                    {
                        IsOrganization = true;
                    }
                    else
                    {
                        if (User.Followers < 300 && User.Followers > 0)
                        {
                            Followers = await UserDataService.getAllFollowers(User.Login);
                        }

                        if (User.Following < 300 && User.Following > 0)
                        {
                            Following = await UserDataService.getAllFollowing(User.Login);
                        }
                    }
                }
                else
                {
                    isLoggedin = false;
                }
               
                isLoading = false;
            }
        }
        public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
        {
            isLoggedin = false;
            User = null;
        }
        public void ProfileTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(DeveloperProfileView), ((User)e.ClickedItem).Login);
        }
        public void RecieveSignInMessage(User user)
        {
            isLoading = true;
            if (user != null)
            {
                isLoggedin = true;
                User = user;
            }
            isLoading = false;
        }
    }
}
