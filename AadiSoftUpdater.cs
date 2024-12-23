using System.Diagnostics;

namespace AadiSoftUpdater;

/// <summary>
/// Automatic Updater for Windows Applications.
/// Check for updates on the Github repository by using CheckForUpdates().
/// Update the App by using UpdateApp().
/// It Downloads the MSI file from the Github Repository Releases and Installs it.
/// It deletes the App using the Product Code and Installs the new MSI file.
/// The Github Repo Should be a Public Repository
/// </summary>
public static class AadiSoftUpdater
{
	/// <summary>
	/// Check for updates on the Github repository.
	/// It uses the Readme File in the Github Account.
	/// The readme File should have a text "Latest Version = 1.0.0.0" in it.
	/// It Returns true if update is Found.
	/// </summary>
	/// <param name="githubRepoOwner">Username of the Github Account</param>
	/// <param name="githubRepoName">Username of the Github Account</param>
	/// <param name="currentVersion">Pass the Current Version of the Application or Just Pass this - Assembly.GetExecutingAssembly().GetName().Version.ToString() and update the Assembly Version in the Properties Window</param>
	/// <returns></returns>
	public static async Task<bool> CheckForUpdates(string githubRepoOwner, string githubRepoName, string currentVersion)
	{
		string fileContent = await GetLatestVersionFromGithub(githubRepoOwner, githubRepoName);

		if (fileContent.Contains("Latest Version = "))
		{
			string latestVersion = fileContent.Substring(fileContent.IndexOf("Latest Version = ") + 17, 7);

			if (latestVersion != currentVersion) return true;
		}

		return false;
	}

	private static async Task<string> GetLatestVersionFromGithub(string githubRepoOwner, string githubRepoName)
	{
		string fileUrl = $"https://raw.githubusercontent.com/{githubRepoOwner}/{githubRepoName}/refs/heads/main/README.md";

		using (HttpClient client = new HttpClient())
		{
			string cacheBuster = DateTime.UtcNow.Ticks.ToString();
			string requestUrl = $"{fileUrl}?cb={cacheBuster}";
			return await client.GetStringAsync(requestUrl);
		}
	}


	/// <summary>
	/// Update the App by downloading the MSI file from the Github Repository Releases and Installing it.
	/// </summary>
	/// <param name="githubRepoOwner">Username of the Github Account</param>
	/// <param name="githubRepoName">Username of the Github Account</param>
	/// <param name="setupMSIName">Single MSI File name (without the extension)</param>
	/// <param name="productCode">Product code of the MSI that you can get from the Setup Project Properties Page</param>
	/// <returns></returns>
	public static async Task UpdateApp(string githubRepoOwner, string githubRepoName, string setupMSIName, string productCode)
	{
		var url = $"https://github.com/{githubRepoOwner}/{githubRepoName}/releases/latest/download/{setupMSIName}.msi";
		var filePath = Path.Combine(Path.GetTempPath(), $"{setupMSIName}.msi");

		using (HttpClient client = new HttpClient())
		using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
		using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
		using (Stream streamToWriteTo = File.Open(filePath, FileMode.Create))
			await streamToReadFrom.CopyToAsync(streamToWriteTo);

		DeleteAndInstallApp(filePath, productCode);
	}

	private static void DeleteAndInstallApp(string filePath, string productCode)
	{
		string batchFilePath = Path.Combine(Path.GetTempPath(), "update.bat");

		string batchScript = $@"
@echo off
echo Uninstalling program...
msiexec /x {{{productCode}}} /qb

echo Starting setup file...
start """" ""{filePath}""
";

		File.WriteAllText(batchFilePath, batchScript);
		Process.Start(new ProcessStartInfo(batchFilePath) { UseShellExecute = true });
		Environment.Exit(0);
	}
}
