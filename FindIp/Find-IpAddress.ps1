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

$n = foreach ($projectId in $projectIdsIOwn) {
    $instances = gcloud compute instances list -q --project=$projectId --format=json | ConvertFrom-Json
    foreach ($instance in $instances) {
        foreach ($networkInterface in $instance.networkInterfaces) {
            Write-Host $networkInterface.networkIp
            if ($networkInterface.networkIp -eq $IpAddress) {
                $instance
            }
            foreach ($accessConfig in $networkInterface.accessConfigs) {
                Write-Host $accessConfig.natIP
                if ($accessConfig.natIP -eq $IpAddress) {
                    $instance
                }
            }
        }
    }
}
Write-Progress -Activity "$IpAddress not found"