﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatBot.Clients.Helpers;
using ChatBot.Clients.Models;
using ChatBot.Clients.Services.BotService;
using Xamarin.Forms;

namespace ChatBot.Clients.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        IBotService service;

        #region Properties
        private ObservableCollection<Activity> activity;

        public ObservableCollection<Activity> Activities
        {
            get { return activity; }
            set { SetProperty(ref activity, value); }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        #endregion

        public ICommand LoadCommand => new Command(async () => await Load());
        public ICommand SendCommand => new Command(async () => await Send());

        public ChatViewModel(IBotService service)
        {
            this.service = service;
            Activities = new ObservableCollection<Activity>();
        }

        public async Task Load()
        {
            IsBusy = true;
            var activity = await service.Connect();
            Activities.Add(activity);
            IsBusy = false;
        }

        private async Task Send()
        {
            if (Text != null && Text != "")
            {
                var messageToSend = new Activity()
                {
                    From = new User() { Id = Guid.NewGuid().ToString(), Name = Settings.UserName },
                    Text = this.Text,
                    ConversationId = new ConversationId() { Id = Settings.ConversationId },
                    Timestamp = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    Type = "message"
                };
                Activities.Add(messageToSend);
                Text = string.Empty;
                MessagingCenter.Send<object, object>(this, "AutoScroll", Activities.Last());
                IsBusy = true;
                var activity = await service.SendMessage(messageToSend);
                Activities.Add(activity);
                IsBusy = false;
                MessagingCenter.Send<object, object>(this, "AutoScroll", Activities.Last());
            }
            
        }
    }
}
