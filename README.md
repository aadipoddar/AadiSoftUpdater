# AadiSoft Updater

AadiSoftUpdater is an automatic updater for Windows applications. It checks for updates on a specified GitHub repository and updates the application by downloading and installing the latest MSI file from the repository's releases.

## Tutorial Video
[Aadi-Youtube](https://www.youtube.com/watch?v=k_cJ09gB78k)

## Features

- Check for updates on a GitHub repository.
- Download and install the latest MSI file from the repository's releases.
- Automatically uninstall the old version using the Product Code and install the new version.

## Requirements

- .NET 8
- C# 12.0

## Installation

To use AadiSoftUpdater, include the `AadiSoftUpdater.cs` file in your project.

## Usage

In Windows Forms or WPF applications, you can use the `CheckForUpdates` method in the `Form_Load` event to check for updates when the application starts.

### Check for Updates

To check for updates, use the `CheckForUpdates` method. This method checks the README file in the specified GitHub repository for a line containing "Latest Version = x.x.x.x".

```cs
using AadiSoftUpdater;

bool isUpdateAvailable = await AadiSoftUpdater.CheckForUpdates("github_username", "repository_name", "Assembly.GetExecutingAssembly().GetName().Version.ToString()");

if (isUpdateAvailable)
{
	// Update is available
}
else
{
	// No update available
}
```

### Update the Application

To update the application, use the `UpdateApp` method. This method downloads the latest MSI file from the specified GitHub repository's releases and installs it.


```cs
await AadiSoftUpdater.UpdateApp("github_username", "repository_name", "MSI_File_Name", "Product_Code_of_App");
```

## Example

Here is an example of how to use AadiSoftUpdater in your application:

```cs
using System;
using System.Reflection;
using System.Threading.Tasks;
using AadiSoftUpdater;

class Program
{
	static async Task Main(string[] args)
	{
		string githubRepoOwner = "yourGithubUsername";
		string githubRepoName = "yourRepoName";
		string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		string setupMSIName = "yourSetupMSIName";
		string productCode = "yourProductCode";

		bool isUpdateAvailable = await AadiSoftUpdater.CheckForUpdates(githubRepoOwner, githubRepoName, currentVersion);

		if (isUpdateAvailable)
		{
			if (MessageBox.Show("New Version Available. Do you want to update?", "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				await AadiSoftUpdater.UpdateApp(githubRepoOwner, githubRepoName, setupMSIName, productCode);
			}
		}
		else
		{
			MessageBox.Show("Application is Up To Date");
		}
	}
}
```

## License

This project is licensed under the MIT License.
