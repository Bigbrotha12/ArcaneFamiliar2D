using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace InterOperability
{
    public class BackendMessenger : IBackend
    {
        [System.Serializable]
        private class RegistrationResponse
        {
            public bool isRegistered;
        }
        [System.Serializable]
        private class SessionResponse
        {
            public int expiration;
            public int max_expiration;
        }
        [System.Serializable]
        private class SaveRequest
        {
            public string game_codehash;
            public UserData game_savedata;
            public bool progress;
        }
        private Dictionary<string, string> Path = new Dictionary<string, string>(){
        {"Base", "https://arcane-familiars.netlify.app/.netlify/functions/session"},
        {"Register", "/v1/user/register"},
        {"Login", "/v1/user/login"},
        {"Logout", "/v1/user/logout"},
        {"Save", "/v1/user/save"},
        {"Load", "/v1/user/load"}
    };

        public async Task<bool> GetRegistrationStatus(string address)
        {
            string url = Path["Base"] + Path["Register"];
            UserAuthentication authentication = new UserAuthentication()
            {
                eth_address = address,
                eth_timestamp = "",
                eth_signature = ""
            };

            UnityWebRequest request =
                await SendSignedDWRequest(url, UnityWebRequest.kHttpVerbGET, authentication);

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                RegistrationResponse response = JsonUtility.FromJson<RegistrationResponse>(json);
                request.Dispose();
                return response.isRegistered;
            }
            else
            {
                Debug.LogError(request.error);
                request.Dispose();
                throw new Exception();
            }
        }

        public async Task<SessionData> GetSessionStatus(UserAuthentication authentication)
        {
            string url = Path["Base"] + Path["Login"];

            UnityWebRequest request =
                await SendSignedDWRequest(url, UnityWebRequest.kHttpVerbGET, authentication);

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                SessionResponse response = JsonUtility.FromJson<SessionResponse>(json);
                request.Dispose();
                return new SessionData(response.expiration, response.max_expiration);
            }
            else
            {
                Debug.LogError(request.error);
                request.Dispose();
                throw new Exception();
            }
        }

        public async Task<SessionData> RegisterUser(UserAuthentication authentication)
        {
            string url = Path["Base"] + Path["Register"];
            UnityWebRequest request =
                await SendSignedDWRequest(url, UnityWebRequest.kHttpVerbPOST, authentication);

            if (request.result == UnityWebRequest.Result.Success)
            {
                string result = request.downloadHandler.text;
                request.Dispose();
                SessionResponse response = JsonUtility.FromJson<SessionResponse>(result);
                return new SessionData(response.expiration, response.max_expiration);
            }
            else
            {
                Debug.LogError(request.error);
                request.Dispose();
                throw new Exception();
            }
        }

        public async Task<SessionData> LogInUser(UserAuthentication authentication)
        {
            string url = Path["Base"] + Path["Login"];
            UnityWebRequest request =
                await SendSignedDWRequest(url, UnityWebRequest.kHttpVerbPUT, authentication);

            if (request.result == UnityWebRequest.Result.Success)
            {
                string result = request.downloadHandler.text;
                request.Dispose();
                SessionResponse response = JsonUtility.FromJson<SessionResponse>(result);
                return new SessionData(response.expiration, response.max_expiration);
            }
            else
            {
                Debug.LogError(request.error);
                request.Dispose();
                throw new Exception();
            }
        }

        public async Task<bool> LogOutUser(UserAuthentication authentication)
        {
            string url = Path["Base"] + Path["Logout"];
            UnityWebRequest request =
                await SendSignedDWRequest(url, UnityWebRequest.kHttpVerbDELETE, authentication);

            if (request.result == UnityWebRequest.Result.Success)
            {
                request.Dispose();
                return true;
            }
            else
            {
                Debug.LogError(request.error);
                request.Dispose();
                return false;
            }
        }

        public async Task<bool> SaveUserData(UserAuthentication authentication, string codeHash, UserData data, bool progressFlag)
        {
            SaveRequest saveRequest = new SaveRequest()
            {
                game_codehash = codeHash,
                game_savedata = data,
                progress = progressFlag
            };
            string payload = JsonConvert.SerializeObject(saveRequest);
            string url = Path["Base"] + Path["Save"];
            
            UnityWebRequest request =
                await SendSignedUPRequest(url, UnityWebRequest.kHttpVerbPOST, payload, authentication);

            if (request.result == UnityWebRequest.Result.Success)
            {
                request.Dispose();
                return true;
            }
            else
            {
                Debug.LogError(request.error);
                request.Dispose();
                return false;
            }
        }

        public async Task<UserData> LoadUserData(UserAuthentication authentication)
        {
            string url = Path["Base"] + Path["Load"];

            UnityWebRequest request =
                await SendSignedDWRequest(url, UnityWebRequest.kHttpVerbGET, authentication);

            if (request.result == UnityWebRequest.Result.Success)
            {
                string result = request.downloadHandler.text;
                request.Dispose();
                return JsonConvert.DeserializeObject<UserData>(result);
            }
            else
            {
                string message = request.error;
                request.Dispose();
                throw new Exception(message);
            }
        }

        private async Task<UnityWebRequest> SendSignedDWRequest(string url, string method, UserAuthentication authentication)
        {
            UnityWebRequest request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;

            request.SetRequestHeader("eth_address", authentication.eth_address);
            request.SetRequestHeader("eth_timestamp", authentication.eth_timestamp);
            request.SetRequestHeader("eth_signature", authentication.eth_signature);
            AsyncOperation serverResponse = request.SendWebRequest();

            while (serverResponse.isDone == false)
            {
                await Task.Delay(100);
            }
            return request;
        }

        private async Task<UnityWebRequest> SendSignedUPRequest(string url, string method, string data, UserAuthentication authentication)
        {
            UnityWebRequest request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;
            byte[] payload = Encoding.UTF8.GetBytes(data);
            request.uploadHandler = new UploadHandlerRaw(payload);
            request.disposeUploadHandlerOnDispose = true;

            request.SetRequestHeader("eth_address", authentication.eth_address);
            request.SetRequestHeader("eth_timestamp", authentication.eth_timestamp);
            request.SetRequestHeader("eth_signature", authentication.eth_signature);
            request.SetRequestHeader("Content-Type", "application/json");
            AsyncOperation serverResponse = request.SendWebRequest();

            while (serverResponse.isDone == false)
            {
                await Task.Delay(100);
            }
            return request;
        }
    }
}