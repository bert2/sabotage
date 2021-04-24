# sabotage

This is a simple Git GUI for research purposes. It's main shtick is that it allows you to view the contents of multiple branches in parallel. I built it for myself to better understand when exactly merge conflicts happen. Its simplified UI helps to easily provoke conflicts.

![Screenshot](screenshot.png "Screenshot")

## features & limitations

Only a small subset of Git's features are supported. Here is a list of the things sabotage can do:

- Open an existing repository or open any directory and let sabotage initialize a new repository in there.
- Browse the contents of local branches.
- Checkout remote branches to browse them.
- Edit the files of a checked out branch.
- Add/rename/delete files and folders in the working directory.
- Commit all working tree changes (there's no staging area).
- Create a new branch starting from a local branch.
- Rename and delete local branches.
- Merge a branch into the currently checked-out branch.
- Cherry-pick a commit into the currently checked-out branch.
- Working tree status summary in a format similar to [posh-git](https://github.com/dahlbyk/posh-git#git-status-summary-information).

There is no possibility to see the commit history or a commit graph. A graph of branches would certainly be helpful, though. Maybe later.

No fetching from or pushing to remote repositories. Use your favorite real Git client to interact with them.

Only works on Windows systems, because it uses WPF.

## installation

There is no installer, but you can download a pre-compiled version in the [releases](https://github.com/bert2/sabotage/releases) section.

## building from source

1. Install the [.NET 5.0 SDK](https://dotnet.microsoft.com/download). If you have [chocolatey](https://chocolatey.org/) installed you can execute the command below in PowerShell:

```powershell
PS> choco install dotnet-sdk
```

2. Install [Git](https://git-scm.com/). It's actually not needed for sabotage, but your life won't be complete without it.

```powershell
PS> choco install git
```

3. Download the [source](https://github.com/bert2/sabotage/archive/refs/heads/main.zip) or clone it using Git. You did install it, right?

```powershell
PS> git clone https://github.com/bert2/sabotage.git
```

4. Build & run sabotage:

```powershell
PS> cd sabotage
PS> dotnet run -p .\sabotage\sabotage.csproj
```

## known issues

- Opening bigger repositories might freeze the UI for a couple of seconds. This is because everything is done in the UI thread and nothing is off-loaded into a background thread. Since sabotage is only built for smaller toy repositories, I see no need to change that.

## credits

- [LibGit2Sharp](https://github.com/libgit2/libgit2sharp) handles all the Git stuff.
- [MaterialDesignInXAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) & [MaterialDesignExtensions](https://github.com/spiegelp/MaterialDesignExtensions) make it sexy.
