
function Get-Repos() {
    $org = Invoke-RestMethod -Uri https://api.github.com/orgs/googleapis
    Invoke-RestMethod -Uri $org.repos_url
}

function Write-RepoShards([Parameter(Mandatory=$true, ValueFromPipeline=$true)]$repos, $ShardCount=8) {
    $repoCount = $repos.Length
    $prevShardEnd = -1
    for ($shard = 1; $shard -le $ShardCount; $shard += 1) {
        $shardBegin = $prevShardEnd + 1
        $shardEnd = [math]::floor($shard / $ShardCount * $repoCount) -1
        $reposJson = $repos[$shardBegin..$shardEnd] | ConvertTo-Json
        Set-Content -Path ("repos-{0:d4}" -f $shard) -Value $reposJson
        $prevShardEnd = $shardEnd
    }
}

Get-Repos | Write-RepoShards
# foreach ($repo in $repos) {
#     git clone $repo.clone_url
#     pushd .
#     try {
#         cd $repo.name
#         git log '-GcomputeMetadata[/\]+v1beta' --all -p > ../$($repo.name).log
#     }
#     finally {
#         popd
#     }
#     break
# }