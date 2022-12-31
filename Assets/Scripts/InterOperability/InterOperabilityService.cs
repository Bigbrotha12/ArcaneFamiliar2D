using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace InterOperability
{
    [System.Serializable]
    public class UserAuthentication
    {
        public string eth_address;
        public string eth_timestamp;
        public string eth_signature;
    }
    
    public class SessionData {

        private int _expiration;
        private int _max_expiration;

        public SessionData(int expiration, int max_expiration)
        {
            _expiration = expiration;
            _max_expiration = max_expiration;
        }
        
        public bool IsLoggedIn() {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
            return Convert.ToInt32(unixTime) < _expiration;
        }

        public bool IsExpired() 
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
            return Convert.ToInt32(unixTime) < _max_expiration;
        }
    }

    public class InterOperabilityService : IInterOperable
    {
        private UserAuthentication _authentication;
        private SessionData _session;
        private IBlockchain _blockchain;
        private IBrowser _browser;
        private IBackend _backend;
        private string _codeHash = "e32e703a20098305652a91ed3370235a27f244557ddae05f8afc0fc46e3bdc8a";
        public bool IsLoggedIn { get; private set; }

        // Constructor
        public InterOperabilityService()
        {
            _blockchain = new BlockchainMessenger();
            _browser = new BrowserMessenger();
            _backend = new BackendMessenger();
        }

        public bool LogInStatus()
        {
            if(_session == null) { return false; }
            return _session.IsLoggedIn();
        }

        public async Task<InitialData> RequestInitData()
        {
            try
            {
                return await _browser.RequestInitialData();
            }
            catch (System.Exception error)
            {
                Debug.LogError(error.Message);
                return new InitialData()
                {
                    address = "",
                    isRegistered = false
                };
            }
        }

        public async Task<bool> CheckRegistration(string userAddress)
        {
            try
            {
                return await _backend.GetRegistrationStatus(userAddress);
            }
            catch (System.Exception error)
            {
                Debug.LogError(error);
                return false;
            }
        }

        public async Task<bool> SessionStatus()
        {
            if (_session is null) { return false; }
            if (_session.IsLoggedIn()) { return true; }
            if (_authentication == null) { Debug.LogError("Authentication is required."); }

            try
            {
                _session = await _backend.GetSessionStatus(_authentication);
                return _session.IsLoggedIn();
            }
            catch (System.Exception error)
            {
                Debug.LogError(error);
                return false;
            }
        }

        public async Task<bool> Register()
        {
            if (_authentication == null) { _authentication = await _browser.RequestAuthentication(); }

            try
            {
                _session = await _backend.RegisterUser(_authentication);
                return _session.IsLoggedIn();
            }
            catch (System.Exception error)
            {
                Debug.LogError(error);
                return false;
            }
        }

        public async Task<bool> LogIn()
        {
            // if authorization data isn't available, request it
            if (_session == null || !_session.IsLoggedIn()) { _authentication = await _browser.RequestAuthentication(); }
            if (_authentication == null) { throw new InvalidOperationException("Authentication is required."); }

            try
            {
                _session = await _backend.LogInUser(_authentication);
                IsLoggedIn = _session.IsLoggedIn();
                return IsLoggedIn;
            }
            catch (System.Exception error)
            {
                Debug.LogError(error);
                return false;
            }
        }

        public async Task<bool> LogOut()
        {
            if (_session != null && (!_session.IsExpired() || _authentication == null)) { return true; }
            _session = null;
            _authentication = null;
            return await _backend.LogOutUser(_authentication);
        }

        public async Task<bool> SaveData(UserData data, bool progressFlag)
        {
            if (_authentication == null) { throw new InvalidOperationException("Authentication is required."); }
            return await _backend.SaveUserData(_authentication, GetCodeHash(), data, progressFlag);
        }

        public async Task<UserData> LoadData()
        {
            if (_authentication == null) { throw new InvalidOperationException("Authentication is required."); }
            return await _backend.LoadUserData(_authentication);
        }

        public async Task<List<FamiliarMetadata>> GetOwnedFamiliars()
        {
            if (_authentication == null) { throw new Exception("Authentication is required."); }
            return await _blockchain.FetchOwnFamiliars(_authentication.eth_address);
        }

        public async Task<Preferences> GetPlayerPreferences()
        {
            return await _browser.RequestPreferenceData();
        }

        public void SavePlayerPreferences(Preferences preference)
        {
            try
            {
                _browser.UpdatePlayerPreferences(preference);
       
            } catch (System.Exception error)
            {
                Debug.LogError(error);
            }
        }

        public async Task<bool> VerifyOwnership(string[] tokenIds)
        {
            return await _blockchain.Verify(tokenIds, _authentication.eth_address);
        }

        private string GetCodeHash()
        {
            // v0.1.0
            return _codeHash;
        }
    }
}