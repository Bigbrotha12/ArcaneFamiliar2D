using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace InterOperability
{
    public class BlockchainMessenger : IBlockchain
    {
        public bool HasNext { get; private set; }
        private string _cursor;
        private readonly string _IMXSandbox = "https://api.sandbox.x.immutable.com/v1/assets";
        private readonly string _collectionSandbox = "0xb7eaa855fa6432d0597f297bace4613c33a075d1";
        //private readonly string _resultSize = "50";
        [System.Serializable]
        private class API_Response
        {
            public string cursor;
            public int remaining;
            public List<API_Result> result;
        }
        [System.Serializable]
        private class API_Result
        {
            public FamiliarMetadata metadata;
        }
        [System.Serializable]
        private class VerifyResponse
        {
            public string user;
        }

        public async Task<List<FamiliarMetadata>> FetchOwnFamiliars(string address)
        {
            Dictionary<string, string> query = new Dictionary<string, string>()
            {
                //{ "page_size", _resultSize },
                { "user", address },
                { "collection", _collectionSandbox },
            };
            if (HasNext && _cursor != null) { query.Add("cursor", _cursor); }

            byte[] queryString = UnityWebRequest.SerializeSimpleForm(query);
            string url = _IMXSandbox + "?" + Encoding.UTF8.GetString(queryString);

            UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;

            AsyncOperation serverResponse = request.SendWebRequest();

            while (serverResponse.isDone == false)
            {
                await Task.Delay(100);
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                // API_Response.result contains array of up to 50 tokens
                // API_Response.result[*].metadata contains familiar metadata
                // API.Response.cursor cursor for next page of result
                // API.Response.remaining is 1 if there are more familiars to fetch
                API_Response API_Response = JsonConvert.DeserializeObject<API_Response>(request.downloadHandler.text);

                List<Familiars> result = new List<FamiliarMetadata>();
                foreach (API_Result token in API_Response.result) { result.Add(token.metadata); }
                HasNext = API_Response.remaining == 1;
                _cursor = API_Response.cursor;
                request.Dispose();
                return result;
            }
            else
            {
                Debug.Log(request.error);
                request.Dispose();
                throw new Exception();
            }
        }

        public async Task<bool> Verify(string[] tokenIds, string address)
        {
            bool verification = true;
            foreach (string id in tokenIds)
            {
                string url = _IMXSandbox + "/" + _collectionSandbox + "/" + id;
                VerifyResponse response = await SendVerifyRequest(url);
                if (response.user != address) { verification = false; }
            }
            return verification;
        }

        private async Task<VerifyResponse> SendVerifyRequest(string url)
        {
            UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;

            AsyncOperation serverResponse = request.SendWebRequest();

            while (serverResponse.isDone == false)
            {
                await Task.Delay(100);
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                VerifyResponse result = JsonConvert.DeserializeObject<VerifyResponse>(request.downloadHandler.text);
                request.Dispose();
                return result;
            }
            else
            {
                Debug.Log(request.error);
                request.Dispose();
                return new VerifyResponse() { user = "" };
            }
        }
    }
}