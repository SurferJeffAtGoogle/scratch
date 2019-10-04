
Param([switch]$GetRepos, [switch]$AllShards, [int]$Shard, [string]$ShardFile)

function Get-Repos() {
    $org = Invoke-RestMethod -Uri https://api.github.com/orgs/googleapis
    $repos = Invoke-RestMethod -FollowRelLink -Uri $org.repos_url
    # Flatten the array.
    $result = $repos | % {$_}
    $result
}

$repoShardFileFormat = "repos-{0:d4}.json"
$repoShardFileMask = "repos-*.json"
function Write-RepoShards([Parameter(Mandatory=$true)]$repos, $shardCount=16) {
    $repoCount = $repos.Length
    $prevShardEnd = -1
    for ($shardNumber = 1; $shardNumber -le $shardCount; $shardNumber += 1) {
        $shardBegin = $prevShardEnd + 1
        $shardEnd = [math]::floor($shardNumber / $shardCount * $repoCount) - 1
        $reposSlice = $repos[$shardBegin..$shardEnd]
        $outPath = ($repoShardFileFormat -f $shardNumber)
        "Writing $($reposSlice.name -Join ',') to $outPath"
        Set-Content -Path $outPath -Value ($reposSlice | ConvertTo-Json)
        $prevShardEnd = $shardEnd
    }
}

function Search-Repo($repoDir, $outputDir) {
    $repoName = Split-Path -Leaf $repoDir
    Push-Location
    try {
        Set-Location $repoDir
        git log '-ScomputeMetadata[/\]+v1beta' --pickaxe-regex --all -p > `
            (Join-Path $outputDir "$($repoName)-computeMetadata-v1beta.log")
        git log '-ScomputeMetadata' --all -p > `
            (Join-Path $outputDir "$($repoName)-computeMetadata-all.log")
    }
    finally {
        Pop-Location
    }
}

function Clone-Repo($repoCloneUrl, $repoName, $clonesDir) {
    Push-Location
    try {
        $clonesDir = if (Test-Path $clonesDir) {
            Get-Item $clonesDir
        } else {
            New-Item -ItemType "directory" $clonesDir
        }
        Set-Location $clonesDir
        if (Test-Path -Path $repoName) {
            Set-Location $repoName
            git checkout master
            git pull
        } else {
            git clone $repoCloneUrl
        }
    }
    finally {
        Pop-Location
    }
}

function Get-OutputDir() {
    if (Test-Path SearchResults) {
        Get-Item SearchResults
    } else {
        New-Item -ItemType "directory" SearchResults
    }
}

function Search-Repos($repos, $outputDir) {
    foreach ($repo in $repos) {
        "Cloning $($repo.name)..." | Write-Host
        Clone-Repo $repo.clone_url $repo.name "Clones"
        "Searching $($repo.name)..." | Write-Host
        Search-Repo (Join-Path "Clones" $repo.name) $outputDir
    }
}

if ($GetRepos) {
    Write-RepoShards (Get-Repos)
}

if ($AllShards) {
    $files = Get-ChildItem -Filter $repoShardFileMask
    $jobs = foreach ($file in $files) {
        "Starting $file..." | Write-Host 
        Start-Job -FilePath "Run.ps1" -ArgumentList $null,"$file"
    }
    foreach ($job in $jobs) {
        Receive-Job -Job $job -Wait
        Remove-Job -Job $job
    }
}

if ($ShardFile) {
    "Shard file: $ShardFile" | Write-Host
    $repos = Get-Content $ShardFile | ConvertFrom-Json
    Search-Repos $repos (Get-OutputDir)
}

if ($ShardNumber) {
    $repos = Get-Content ($repoShardFileFormat -f $ShardNumber) | ConvertFrom-Json
    Search-Repos $repos (Get-OutputDir)
}