using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterOperability
{
    public interface IInterOperable
    {
        // Getter
        Task<InitialData> RequestInitData();
        Task<Preferences> GetPlayerPreferences();
        Task<bool> CheckRegistration(string userAddress);
        bool LogInStatus();
        Task<bool> SessionStatus();
        Task<UserData> LoadData();
        Task<List<FamiliarMetadata>> GetOwnedFamiliars();
        Task<bool> VerifyOwnership(string[] tokenIds);

        // Setter
        Task<bool> Register();
        Task<bool> LogIn();
        Task<bool> LogOut();
        Task<bool> SaveData(UserData data, bool progressFlag);
        void SavePlayerPreferences(Preferences preferences);
    }

    public interface IBackend
    {
        Task<bool> GetRegistrationStatus(string address);
        Task<SessionData> GetSessionStatus(UserAuthentication authentication);
        Task<SessionData> RegisterUser(UserAuthentication authentication);
        Task<SessionData> LogInUser(UserAuthentication authentication);
        Task<bool> LogOutUser(UserAuthentication authentication);
        Task<bool> SaveUserData(UserAuthentication authentication, string codeHash, UserData data, bool progressFlag);
        Task<UserData> LoadUserData(UserAuthentication authentication);
    }

    public interface IBrowser
    {
        Task<UserAuthentication> RequestAuthentication(int timeout = 60000);
        Task<InitialData> RequestInitialData(int timeout = 60000);
        Task<Preferences> RequestPreferenceData(int timeout = 60000);
        void UpdateRegistrationStatus();
        void UpdatePlayerPreferences(Preferences preferences);
    }

    public interface IBlockchain
    {
        Task<List<FamiliarMetadata>> FetchOwnFamiliars(string address);
        Task<bool> Verify(string[] tokenIds, string address);
    }
}