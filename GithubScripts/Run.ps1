
Param([switch]$GetRepos, [int]$Shard)

function Get-Repos() {
    $org = Invoke-RestMethod -Uri https://api.github.com/orgs/googleapis
    Invoke-RestMethod -Uri $org.repos_url
}

$repoShardFileFormat = "repos-{0:d4}.json"
function Write-RepoShards([Parameter(Mandatory=$true, ValueFromPipeline=$true)]$repos, $shardCount=8) {
    $repoCount = $repos.Length
    $prevShardEnd = -1
    for ($shardNumber = 1; $shardNumber -le $shardCount; $shardNumber += 1) {
        $shardBegin = $prevShardEnd + 1
        $shardEnd = [math]::floor($shardNumber / $shardCount * $repoCount) - 1
        $reposJson = $repos[$shardBegin..$shardEnd] | ConvertTo-Json
        Set-Content -Path ($repoShardFileFormat -f $shardNumber) -Value $reposJson
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

if ($GetRepos) {
    Get-Repos | Write-RepoShards
}

if ($Shard) {
    $repos = Get-Content ($repoShardFileFormat -f $Shard) | ConvertFrom-Json
    $outputDir = if (Test-Path SearchResults) {
        Get-Item SearchResults
    } else {
        New-Item -ItemType "directory" SearchResults
    }
    foreach ($repo in $repos) {
        "Cloning $($repo.name)..." | Write-Host
        Clone-Repo $repo.clone_url $repo.name "Clones"
        "Searching $($repo.name)..." | Write-Host
        Search-Repo (Join-Path "Clones" $repo.name) $outputDir
    }
}