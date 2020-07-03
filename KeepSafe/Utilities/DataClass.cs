using System;
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

        User _user;
        public User User
        {
            set
            {
                _user = value;
                Application.Current.Properties[nameof(User)] = JsonConvert.SerializeObject(_user);
                OnPropertyChanged(nameof(User));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if ((_user == null ? false : _user.Id != 0) && Application.Current.Properties.ContainsKey("user"))
                {
                    _user = JsonConvert.DeserializeObject<User>(Application.Current.Properties[nameof(User)].ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
                if (_user == null)
                {
                    _user = new User();
                }

                return _user;
            }
        }

        bool _IsVerified;
        public bool IsLoggedIn
        {
            set
            {
                _IsVerified = value;
                Application.Current.Properties[nameof(IsLoggedIn)] = JsonConvert.SerializeObject(_IsVerified);
                OnPropertyChanged(nameof(IsLoggedIn));
                Application.Current.SavePropertiesAsync();
            }
            get
            {
                if ( Application.Current.Properties.ContainsKey("IsVerified"))
                {
                    _IsVerified = JsonConvert.DeserializeObject<bool>(Application.Current.Properties["IsVerified"].ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }

                return _IsVerified;
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
            Faye.Instance.UnsubscribeAll();
            //Delete Cache Data
            Application.Current.Properties.Remove(nameof(BuildNumber));
            Application.Current.Properties.Remove(nameof(Token));
            Application.Current.Properties.Remove(nameof(ClientId));
            Application.Current.Properties.Remove(nameof(Uid));
            Application.Current.Properties.Remove(nameof(User));
            Token = null;
            User = null;
            IsLoggedIn = false;
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
