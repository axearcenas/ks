﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KeepSafe.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using KeepSafe.Helpers.Faye;
using DryIoc;
using System.Runtime.CompilerServices;
using FastExpressionCompiler.LightExpression;
using KeepSafe.ViewModels;

namespace KeepSafe
{
    public class DataClass : BaseNotify
    {
        static DataClass dataClass;
        public static DataClass GetInstance
        {
            get
            {
                if (dataClass == null)
                {
                    dataClass = new DataClass();
                }
                return dataClass;
            }
        }

        public DataClass()
        {

        }

        Server DefaultServer()
        {
            return new Server()
            {
                Name = Constants.SERVER_NAME,
                Api = Constants.URL_STABLE,
                Notification = Constants.URL_FAYE_NOTIFICATION,
                IsSecured = Constants.IS_SERVER_SECURE
            };
        }

        bool _IsUpdateShown;
        public bool IsUpdateShown
        {
            set
            {
                _IsUpdateShown = value;
                Application.Current.Properties["IsUpdateShown"] = JsonConvert.SerializeObject(_IsUpdateShown);
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if (Application.Current.Properties.ContainsKey("IsUpdateShown"))
                {
                    _IsUpdateShown = JsonConvert.DeserializeObject<bool>(Application.Current.Properties["IsUpdateShown"].ToString());
                }
                return _IsUpdateShown;
            }
        }

        string _LastLatestVersion;
        public string LastLatestVersion
        {
            set
            {
                _LastLatestVersion = value;
                Application.Current.Properties["LastLatestVersion"] = JsonConvert.SerializeObject(_LastLatestVersion);
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if (Application.Current.Properties.ContainsKey("LastLatestVersion"))
                {
                    _LastLatestVersion = JsonConvert.DeserializeObject<string>(Application.Current.Properties["LastLatestVersion"].ToString());
                }
                return _LastLatestVersion;
            }
        }

        Server _CurrentServer;
        public Server CurrentServer
        {
            set
            {
                _CurrentServer = value;
                Application.Current.Properties["CurrentServer"] = JsonConvert.SerializeObject(_CurrentServer);
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if (_CurrentServer == null && Application.Current.Properties.ContainsKey("CurrentServer"))
                {
                    _CurrentServer = JsonConvert.DeserializeObject<Server>(Application.Current.Properties["CurrentServer"].ToString());
                }
                else if (_CurrentServer == null)
                {
                    _CurrentServer = DefaultServer();
                }
                return _CurrentServer;
            }
        }

        string _token;
        public string Token
        {
            set
            {
                _token = value;
                Application.Current.Properties["token"] = _token;
                OnPropertyChanged(nameof(Token));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if (string.IsNullOrEmpty(_token) && Application.Current.Properties.ContainsKey("token"))
                {
                    _token = Application.Current.Properties["token"].ToString();
                }

                return _token;
            }
        }

        string _clientId;
        public string ClientId
        {
            set
            {
                _clientId = value;
                Application.Current.Properties["client_id"] = _clientId;
                OnPropertyChanged(nameof(ClientId));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if (string.IsNullOrEmpty(_clientId) && Application.Current.Properties.ContainsKey("client_id"))
                {
                    _clientId = Application.Current.Properties["client_id"].ToString();
                }
                return _clientId;
            }
        }

        string _uid;
        public string Uid
        {
            set
            {
                _uid = value;
                Application.Current.Properties["uid"] = _uid;
                OnPropertyChanged(nameof(Uid));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if (string.IsNullOrEmpty(_uid) && Application.Current.Properties.ContainsKey("uid"))
                {
                    _uid = Application.Current.Properties["uid"].ToString();
                }
                return _uid;
            }
        }

        User _User;
        public User User
        {
            set
            {
                _User = value;
                Application.Current.Properties[nameof(User)] = JsonConvert.SerializeObject(_User);
                OnPropertyChanged(nameof(User));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if ((_User == null ? true : _User.Id != 0) && Application.Current.Properties.ContainsKey(nameof(User)))
                {
                    _User = JsonConvert.DeserializeObject<User>(Application.Current.Properties[nameof(User)].ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
                if (_User == null)
                {
                    _User = new User();
                }

                return _User;
            }
        }

        Business _Business;
        public Business Business
        {
            set
            {
                _Business = value;
                Application.Current.Properties[nameof(Business)] = JsonConvert.SerializeObject(_Business);
                OnPropertyChanged(nameof(Business));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if ((_Business == null ? true : _Business.Id != 0) && Application.Current.Properties.ContainsKey(nameof(Business)))
                {
                    _Business = JsonConvert.DeserializeObject<Business>(Application.Current.Properties[nameof(Business)].ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
                if (_Business == null)
                {
                    _Business = new Business();
                }

                return _Business;
            }
        }

        UserType _AccountType;
        public UserType AccountType
        {
            set
            {
                _AccountType = value;
                Application.Current.Properties["AccountType"] = _AccountType;
                OnPropertyChanged(nameof(_AccountType));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if (string.IsNullOrEmpty(_uid) && Application.Current.Properties.ContainsKey("AccountType"))
                {
                    _AccountType = (UserType)Application.Current.Properties["AccountType"];
                }
                return _AccountType;
            }
        }

        // bool _IsLoggedIn;
        // public bool IsLoggedIn
        UserType _LoginType;
        public UserType LoginType
        {
            set
            {
                _LoginType = value;
                Application.Current.Properties[nameof(LoginType)] = JsonConvert.SerializeObject(_LoginType);
                OnPropertyChanged(nameof(LoginType));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if ( Application.Current.Properties.ContainsKey(nameof(LoginType)))
                {
                    _LoginType = JsonConvert.DeserializeObject<UserType>(Application.Current.Properties[nameof(LoginType)].ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }

                return _LoginType;
            }
        }

        int _BuildNumber;
        public int BuildNumber
        {
            get { return GetDataClassProperty(_BuildNumber); }
            set { SetDataClassProperty(ref _BuildNumber, value); }
        }

        public async Task Logout()
        {
            //TODO: ADD Cache to Delete
            //Logout Facebook Account
            //Plugin.FacebookClient.CrossFacebookClient.Current.Logout();
            //Unsubscribe All Channel in Socket
            //Faye.Instance.UnsubscribeAll();
            //Delete Cache Data

            Application.Current.Properties.Remove(nameof(BuildNumber));
            Application.Current.Properties.Remove(nameof(Token));
            Application.Current.Properties.Remove(nameof(ClientId));
            Application.Current.Properties.Remove(nameof(Uid));
            Application.Current.Properties.Remove(nameof(User));
            Application.Current.Properties.Remove(nameof(AccountType));
            Token = null;
            User = null;
            LoginType = UserType.None;
            await Application.Current.SavePropertiesAsync();
        }

        public async Task SaveData(object data, string key)
        {
            Application.Current.Properties[key] = JsonConvert.SerializeObject(data);
            await Application.Current.SavePropertiesAsync();
        }

        Task SetDataClassProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            oldValue = newValue;
            Application.Current.Properties[propertyName] = JsonConvert.SerializeObject(newValue);
            OnPropertyChanged(propertyName);
            return Application.Current.SavePropertiesAsync();
        }

        T GetDataClassProperty<T>(T value, [CallerMemberName] string propertyName = "", Func<T,bool> expression = null)
        {
            if ((value == null && expression == null ? true : expression.Invoke(value) && Application.Current.Properties.ContainsKey(propertyName)))
            {
                value = JsonConvert.DeserializeObject<T>(
                    Application.Current.Properties[nameof(User)].ToString(),
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            if (value == null)
            {
                value = default;
            }

            return value;
        }
    }
}
