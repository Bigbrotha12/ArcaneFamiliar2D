using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace InterOperability
{
    public class BrowserMessenger : IBrowser
    {
        [DllImport("__Internal")]
        private static extern void Web3Authentication();
        [DllImport("__Internal")]
        private static extern void Web3InitialData();
        [DllImport("__Internal")]
        private static extern void Web3PlayerPreferences();
        [DllImport("__Internal")]
        private static extern void UpdateRegistration();
        [DllImport("__Internal")]
        private static extern void UpdatePlayerPreferences(string message);

        [System.Serializable]
        private class AuthenticationResponse
        {
            public bool Success;
            public string Error;
            public UserAuthentication Authentication;
        }
        [System.Serializable]
        private class InitializationResponse
        {
            public bool Success;
            public string Error;
            public InitialData InitData;
        }
        [System.Serializable]
        private class PreferenceResponse
        {
            public bool Success;
            public string Error;
            public Preferences PreferenceData;
        }

        private AuthenticationResponse _authResponse;
        private InitializationResponse _initResponse;
        private PreferenceResponse _preferenceResponse;
        private bool _requestInProgress;
        private bool _isDone;

        public async Task<UserAuthentication> RequestAuthentication(int timeout = 60000)
        {
            return await Task.Run(async () =>
            {
                if (_requestInProgress) { throw new Exception("A request is currently in progress."); }
                _requestInProgress = true;
                _isDone = false;
                Web3Authentication();
                
                int time = 0;
                while(_isDone == false && time < timeout)
                {
                    await Task.Delay(1000);
                    time += 1000;
                }

                if(_authResponse == null || !_authResponse.Success)
                {
                    if(time >= timeout) 
                    {
                        throw new Exception("Request to browser timed out.");
                    } 
                    throw new Exception("Error obtaining user signature");
                }
                
                _requestInProgress = false;
                return _authResponse.Authentication;
            });
        }

        public async Task<InitialData> RequestInitialData(int timeout = 60000)
        {
            return await Task.Run(async () =>
            {
                if (_requestInProgress) { throw new Exception("A request is currently in progress."); }
                _requestInProgress = true;

                Web3InitialData();
                _isDone = false;
                int time = 0;
                while(_isDone == false && time < timeout)
                {
                    await Task.Delay(1000);
                    time += 1000;
                }

                if(_initResponse == null || !_initResponse.Success)
                {
                    if(time >= timeout) 
                    {
                        throw new Exception("Request to browser timed out.");
                    } 
                    throw new Exception("Error obtaining user signature");
                }

                _requestInProgress = false;
                return _initResponse.InitData;
            });
        }

        public async Task<Preferences> RequestPreferenceData(int timeout = 60000)
        {
            return await Task.Run(async () =>
            {
                if (_requestInProgress) { throw new Exception("A request is currently in progress."); }
                _requestInProgress = true;

                Web3PlayerPreferences();
                _isDone = false;
                int time = 0;
                while(_isDone == false && time < timeout)
                {
                    await Task.Delay(1000);
                    time += 1000;
                }

                if(_initResponse == null || !_initResponse.Success)
                {
                    if(time >= timeout) 
                    {
                        throw new Exception("Request to browser timed out.");
                    } 
                    throw new Exception("Error obtaining user signature");
                }

                _requestInProgress = false;
                return _preferenceResponse.PreferenceData;
            });
        }

        public void UpdateRegistrationStatus()
        {
            UpdateRegistration();
        }

        public void UpdatePlayerPreferences(Preferences preferences)
        {
            string message = JsonConvert.SerializeObject(preferences);
            UpdatePlayerPreferences(message);
        }

        private void ReceiveAuthentication(string authentication)
        {
            try
            {
                _authResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(authentication);

            }
            catch (System.Exception error)
            {
                Debug.LogError(error);
            }
            finally
            {
                _isDone = true;
            }
        }

        private void ReceiveInitialData(string initialData)
        {
            try
            {
                _initResponse = JsonConvert.DeserializeObject<InitializationResponse>(initialData);

            }
            catch (System.Exception error)
            {
                Debug.LogError(error);
            }
            finally
            {
                _isDone = true;
            }
        }

        private void ReceivePlayerPreferences(string preferences)
        {
            try
            {
                _preferenceResponse = JsonConvert.DeserializeObject<PreferenceResponse>(preferences);
            }
            catch (System.Exception error)
            {
                Debug.LogError(error);
            }
            finally
            {
                _isDone = true;
            }
        }
    }
}