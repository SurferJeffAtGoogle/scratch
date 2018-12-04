# Copyright(c) 2018 Google Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License"); you may not
# use this file except in compliance with the License. You may obtain a copy of
# the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
# WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
# License for the specific language governing permissions and limitations under
# the License.

<#
.SYNOPSIS
    Given an IP address, finds a GCP Compute instance with the ip address.
.EXAMPLE
    PS C:\> .\Get-GcpInstance.ps1 --IpAddress 1.2.3.4
.OUTPUTS
    The GCP instance information.
#>

Param(
    [string][Parameter(Mandatory=$true)] $IpAddress
)
$myAccount = (gcloud config list --format=json | ConvertFrom-Json).core.account
$projectIds = (gcloud projects list --format=json | ConvertFrom-Json).ProjectId
$projectIdsIOwn = foreach ($projectId in $projectIds) {
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
            gcloud sql instances list --project=$projectId --filter="PRIMARY_ADDRESS=$IpAddress"--format=json | convertfrom-json
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