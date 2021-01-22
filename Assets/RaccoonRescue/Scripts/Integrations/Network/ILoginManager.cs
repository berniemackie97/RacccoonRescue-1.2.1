
public interface ILoginManager {
	void LoginWithFB (string accessToken, string titleId);

	void UpdateName (string userName);

	bool IsYou (string playFabId);
}


