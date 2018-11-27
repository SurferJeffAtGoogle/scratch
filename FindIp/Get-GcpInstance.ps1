if ($false) {
Param(
    [string][Parameter(Mandatory=$true)] $IpAddress
)

$myAccount = (gcloud config list --format=json | ConvertFrom-Json).core.account
$projectIds = (gcloud projects list --format=json | ConvertFrom-Json).ProjectId
$projectIdsIOwn = foreach ($projectId in $projectIds) {
    Write-Status "Inspecting $projectId..."
    $bindings = (gcloud projects get-iam-policy $projectId --format=json | ConvertFrom-Json).bindings
    foreach ($binding in $bindings) {
        if ($binding.role -eq 'roles/owner') {
            if ($binding.members.Contains("user:$myAccount")) {
                $projectId
                break;
            }
        }
    }
}
$projectIdsIOwn
}

$IpAddress= '35.199.164.200'

function Get-GcpInstance {
    param (
        [string][Parameter(Mandatory=$true)] $IpAddress,
        [string[]][Parameter(Mandatory=$true)] $ProjectIds
    )
    $activity = "Searching for $IpAddress"
    $progress = 0
    try {
        foreach ($projectId in $projectIds) {
            Write-Progress -Activity $activity `
                -PercentComplete (100 * $progress / $projectIds.Count) `
                -CurrentOperation $projectId
            $progress += 1
            $instances = gcloud compute instances list -q --project=$projectId --format=json | ConvertFrom-Json
            foreach ($instance in $instances) {
                foreach ($networkInterface in $instance.networkInterfaces) {
                    if ($networkInterface.networkIp -eq $IpAddress) {
                        return $instance                    
                    }
                    foreach ($accessConfig in $networkInterface.accessConfigs) {
                        if ($accessConfig.natIP -eq $IpAddress) {
                            return $instance
                        }
                    }
                }
            }
        }
    }
    finally {
        Write-Progress -Activity $activity -Completed
    }
}

Get-GcpInstance $IpAddress $projectIdsIOwn